using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Reenbit.ChuckNorris.Domain.ConfigClasses;
using Reenbit.ChuckNorris.Domain.DTOs.ImageDTOS;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services
{
    public class MediaService : IMediaService
    {
        private const int expiredTimeForWriteFileInTempStorage = 30;
        private const int expiredTimeForReadFileInTempStorage = 360;
        private const int startValidTimeForSas = -5;
        private readonly IOptions<AzureStorageBlobOptions> azureStorageBlobOptions;

        public MediaService(IOptions<AzureStorageBlobOptions> azureStorageBlobOptions)
        {
            this.azureStorageBlobOptions = azureStorageBlobOptions;
        }

        public UploadImageDto GenerateSasTokenWithPermissioWriteInTemp(string fileName)
        {
            var guid = Guid.NewGuid();
            fileName = $"{guid}-{fileName}";
            string uploadUrl = GenerateSasToken(azureStorageBlobOptions.Value.FileTempPath, fileName);
            var uploadImageDto = new UploadImageDto
            {
                ImageName = fileName,
                ImageUploadUrl = uploadUrl
            };

            return uploadImageDto;
        }

        public static string GetSasForBlob(CloudBlockBlob blob, int sasMinutesValid)
        {
            var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(startValidTimeForSas),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(sasMinutesValid),
            });
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", blob.Uri, sasToken);
        }

        public async Task<string> CopyImageFromTempToPermanentContainer(string sourceName, string destinationName)
        {
            CloudBlobContainer cloudBlobContainer = GetContainer(azureStorageBlobOptions.Value.FileTempPath);
            CloudBlobContainer descBlobContainer = GetContainer(azureStorageBlobOptions.Value.FilePath);
            if (await CopyBlockBlobAsync(cloudBlobContainer, descBlobContainer, sourceName, destinationName))
            {
                return GenerateImageUrl("images", destinationName);
            }

            return null;
        }

        private string GenerateSasToken(string containerName, string fileName)
        {
            return GenerateSasToken(containerName, DateTime.UtcNow.AddMinutes(expiredTimeForWriteFileInTempStorage), fileName);
        }

        private string GenerateSasToken(string containerName, DateTime expiresOn, string fileName, string fileNameForRead = null)
        {
            CloudBlobContainer cloudBlobContainer = GetContainer(containerName);
            var permissions = SharedAccessBlobPermissions.Write;

            if (!string.IsNullOrEmpty(fileNameForRead))
            {
                CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(fileNameForRead);
                string getSasToken = GetSasForBlob(blob, expiredTimeForReadFileInTempStorage);
                return getSasToken;
            }

            var shareAccessBlobPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(startValidTimeForSas),
                SharedAccessExpiryTime = expiresOn,
                Permissions = permissions
            };

            string fullBlobUrl = String.Format("{0}/{1}{2}", cloudBlobContainer.Uri, fileName, cloudBlobContainer.GetSharedAccessSignature(shareAccessBlobPolicy, null));
            return fullBlobUrl;
        }

        private string GenerateImageUrl(string containerName, string fileName)
        {
            CloudBlobContainer cloudBlobContainer = GetContainer(containerName);
            string fullBlobUrl = String.Format("{0}/{1}", cloudBlobContainer.Uri, fileName);
            return fullBlobUrl;
        }

        private CloudBlobContainer GetContainer(string containerName)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(azureStorageBlobOptions.Value.ConnectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            return cloudBlobContainer;
        }

        private static async Task<bool> CopyBlockBlobAsync(CloudBlobContainer container, CloudBlobContainer destinationCloudBlobContainer, string sourceName, string destinationName)
        {
            CloudBlockBlob sourceBlob = null;
            CloudBlockBlob destBlob = null;
           

            try
            {
                // Get a block blob from the container to use as the source.
                if (container.GetBlockBlobReference(sourceName).Exists())
                {
                    sourceBlob = container.GetBlockBlobReference(sourceName);

                    // Get a reference to a destination blob (in this case, a new blob).
                    destBlob = destinationCloudBlobContainer.GetBlockBlobReference(destinationName);

                    // Get the ID of the copy operation.
                    string copyId = await destBlob.StartCopyAsync(sourceBlob);

                    // Fetch the destination blob's properties before checking the copy state.
                    await destBlob.FetchAttributesAsync();

                    return true;
                }

                return false;
            }
            catch (StorageException e)
            {
                throw;
            }
            finally
            {
                if (sourceBlob != null)
                {
                    await sourceBlob.FetchAttributesAsync();
                }
            }
        }
    }
}

