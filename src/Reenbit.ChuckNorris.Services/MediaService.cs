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
        private const int ExpiredTimeForWriteFileInTempStorage = 30;

        private const int StartValidTimeForSas = -5;

        private readonly IOptions<AzureStorageBlobOptions> azureStorageBlobOptions;

        public MediaService(IOptions<AzureStorageBlobOptions> azureStorageBlobOptions)
        {
            this.azureStorageBlobOptions = azureStorageBlobOptions;
        }

        public async Task<UploadImageDto> GenerateSasTokenWithPermissionWrite(string fileExtencion, string containerName)
        {
            var guid = Guid.NewGuid();
            var fileName = $"{guid}.{fileExtencion}";
            CloudBlobContainer cloudBlobContainer = GetContainer(containerName);
            var permissions = SharedAccessBlobPermissions.Write;
            var shareAccessBlobPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(StartValidTimeForSas),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(ExpiredTimeForWriteFileInTempStorage),
                Permissions = permissions
            };

            string uploadUrl = String.Format("{0}/{1}{2}", cloudBlobContainer.Uri, fileName, cloudBlobContainer.GetSharedAccessSignature(shareAccessBlobPolicy, null));
            var uploadImageDto = new UploadImageDto
            {
                ImageName = fileName,
                ImageUploadUrl = uploadUrl
            };

            return uploadImageDto;
        }

        public async Task<string> CopyFile(string sourceName, string destinationName, string containerSourceName, string containerDestinationName)
        {
            CloudBlobContainer cloudBlobContainer = GetContainer(containerSourceName);
            CloudBlobContainer descBlobContainer = GetContainer(containerDestinationName);
            if (await CopyBlockBlobAsync(cloudBlobContainer, descBlobContainer, sourceName, destinationName))
            {
                return GenerateImageUrl("images", destinationName);
            }

            return null;
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
                if (container.GetBlockBlobReference(sourceName).Exists())
                {
                    sourceBlob = container.GetBlockBlobReference(sourceName);
                    destBlob = destinationCloudBlobContainer.GetBlockBlobReference(destinationName);
                    string copyId = await destBlob.StartCopyAsync(sourceBlob);
                    return true;
                }

                return false;
            }
            catch (StorageException e)
            {
                throw e;
            }
        }

        private static string GetSasForBlob(CloudBlockBlob blob, int sasMinutesValid)
        {
            var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(StartValidTimeForSas),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(sasMinutesValid),
            });
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", blob.Uri, sasToken);
        }
    }
}

