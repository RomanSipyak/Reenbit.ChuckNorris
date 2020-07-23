using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Reenbit.ChuckNorris.Domain.ConfigClasses;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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

        public string GenerateSasTokenWithPermissioWriteInTemp(string fileName)
        {

            return GenerateSasToken(azureStorageBlobOptions.Value.FileTempPath, fileName);
            //return GetSasForBlob(blob, 15);
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

            /*   var key = Base64UrlEncoder.DecodeBytes(azureStorageBlobOptions.Value.AccountKey);
               SymmetricSecurityKey signingKey = new SymmetricSecurityKey(key);
               stringToSign += "/" + azureStorageBlobOptions.Value.AccountName + "/" + azureStorageBlobOptions.Value.FileTempPath;
               HMACSHA256 hasher = new HMACSHA256(Convert.FromBase64String(azureStorageBlobOptions.Value.AccountKey));
               string sAuthTokn = "SharedKeyLite " + azureStorageBlobOptions.Value.AccountName + ":" + Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(stringToSign   )));*/
            string fullBlobUrl = String.Format("{0}/{1}{2}", cloudBlobContainer.Uri, fileName, cloudBlobContainer.GetSharedAccessSignature(shareAccessBlobPolicy, null));
            return fullBlobUrl;
        }

        private CloudBlobContainer GetContainer(string containerName)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(azureStorageBlobOptions.Value.ConnectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            return cloudBlobContainer;
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

        public async Task CopyImageFromTempToPermanentContainer(string sourceName, string destinationName)
        {
            CloudBlobContainer cloudBlobContainer = GetContainer(azureStorageBlobOptions.Value.FileTempPath);
            CloudBlobContainer descBlobContainer = GetContainer(azureStorageBlobOptions.Value.FilePath);
            await CopyBlockBlobAsync(cloudBlobContainer, descBlobContainer, sourceName, destinationName);
        }


        private static async Task CopyBlockBlobAsync(CloudBlobContainer container, CloudBlobContainer destinationCloudBlobContainer, string sourceName, string destinationName)
        {
            CloudBlockBlob sourceBlob = null;
            CloudBlockBlob destBlob = null;
            string leaseId = null;

            try
            {
                // Get a block blob from the container to use as the source.
                sourceBlob = container.GetBlockBlobReference(sourceName);

                // Lease the source blob for the copy operation to prevent another client from modifying it.
                // Specifying null for the lease interval creates an infinite lease.
                leaseId = await sourceBlob.AcquireLeaseAsync(null);

                // Get a reference to a destination blob (in this case, a new blob).
                destBlob = destinationCloudBlobContainer.GetBlockBlobReference(destinationName);

                // Ensure that the source blob exists.
                if (await sourceBlob.ExistsAsync())
                {
                    // Get the ID of the copy operation.
                    string copyId = await destBlob.StartCopyAsync(sourceBlob);

                    // Fetch the destination blob's properties before checking the copy state.
                    await destBlob.FetchAttributesAsync();

                    Console.WriteLine("Status of copy operation: {0}", destBlob.CopyState.Status);
                    Console.WriteLine("Completion time: {0}", destBlob.CopyState.CompletionTime);
                    Console.WriteLine("Bytes copied: {0}", destBlob.CopyState.BytesCopied.ToString());
                    Console.WriteLine("Total bytes: {0}", destBlob.CopyState.TotalBytes.ToString());
                    Console.WriteLine();
                }
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
            finally
            {
                // Break the lease on the source blob.
                if (sourceBlob != null)
                {
                    await sourceBlob.FetchAttributesAsync();

                    if (sourceBlob.Properties.LeaseState != LeaseState.Available)
                    {
                        await sourceBlob.BreakLeaseAsync(new TimeSpan(0));
                    }
                }
            }
        }


        /*  public void CreateStoredAccessPolicy(string policyName)
          {
              //create a stored access policy that expires in 10 hours
              //  and has permissions Read, Write, and List 
              SharedAccessBlobPolicy storedPolicy = new SharedAccessBlobPolicy()
              {
                  SharedAccessExpiryTime = DateTime.UtcNow.AddHours(10),
                  Permissions = SharedAccessBlobPermissions.Read |
                    SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List
              };

              //let's start with a new collection of permissions (this wipes out any old ones)
              BlobContainerPermissions permissions = new BlobContainerPermissions();

              //add the new policy to the container's permissions
              //since this is the only one I want, I'm going to clear the rest first
              permissions.SharedAccessPolicies.Clear();
              permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
              cloudBlobContainer.SetPermissions(permissions);
          }

          public string GetSasForBlobUsingAccessPolicy(CloudBlockBlob cloudBlockBlob)
          {
              //call to set the shared access policy on the container
              //in the real world, this would be passed in, not hardcoded!
              string sharedAccessPolicyName = CreateStoredAccessPolicy(sharedAccessPolicyName);

              //using that shared access policy, get the sas token and set the url
              string sasToken = cloudBlockBlob.GetSharedAccessSignature(null, sharedAccessPolicyName);
              return string.Format(CultureInfo.InvariantCulture, "{0}{1}", cloudBlockBlob.Uri, sasToken);
          }*/

        /*  private void Header()
       {
           try
           {
      *//*         string storageAccount = azureStorageBlobOptions.Value.AccountName; // Enter the Azure storage account name  
               string accessKey = azureStorageBlobOptions.Value.AccountKey; // Enter the Azure access key value  
               string TableName = azureStorageBlobOptions.Value.FileTempPath; //Enter the Azure Table name  
               string uri = @ "https://" + storageAccount + ".table.core.windows.net/" + resourcePath;
               HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
               request.Method = "GET";
               request.ContentType = "application/json";
               request.ContentLength = resourcePath.Length;
               request.Accept = "application/json;odata=nometadata";
               request.Headers.Add("x-ms-date", DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
               request.Headers.Add("x-ms-version", "2015-12-11");
               request.Headers.Add("Accept-Charset", "UTF-8");
               request.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
               request.Headers.Add("DataServiceVersion", "1.0;NetFx");*//*
               //string stringToSign = request.Headers["x-ms-date"] + "\n";
               stringToSign += "/" + storageAccount + "/" + resourcePath;
               HMACSHA256 hasher = new HMACSHA256(Convert.FromBase64String(accessKey));
             *//*  string sAuthTokn = "SharedKeyLite " + storageAccount + ":" + Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
               request.Headers.Add("Authorization", sAuthTokn);*//*
               Console.WriteLine("Authorization Header :", request.Headers["Authorization"]);
               Console.ReadLine();
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }*/
    }
}

