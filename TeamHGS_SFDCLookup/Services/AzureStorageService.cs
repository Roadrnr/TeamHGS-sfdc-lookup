using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TeamHGS_SFDCLookup.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly IConfiguration _config;
        private CloudBlobContainer _container;

        public AzureStorageService(IConfiguration config)
        {
            _config = config;
            _storageAccount = new CloudStorageAccount(
                new StorageCredentials(_config["Azure:AccountName"], _config["Azure:AccountKey"]), true);
            
        }

        private async Task GetContainer(string containerName)
        {
            // Create a blob client.
            var blobClient = _storageAccount.CreateCloudBlobClient();

            // Get a reference to a container named containerName
            _container = blobClient.GetContainerReference(containerName);

            // If "containerName" doesn't exist, create it.
            if (await _container.CreateIfNotExistsAsync())
            {
                await _container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
        }
        public async Task StoreFile(string filename, IFormFile file)
        {
            await GetContainer("images");
            // Get a reference to a blob named filename.
            var uploadFile = _container.GetBlockBlobReference(filename);

            if (await uploadFile.ExistsAsync())
            {
                await uploadFile.FetchAttributesAsync();
                if (uploadFile.Properties.Length != file.Length)
                {
                    uploadFile = _container.GetBlockBlobReference(filename.GetUniqueName());
                } else
                {
                    return;
                }
            }

            // Create or overwrite the filename blob with the contents of a local file
            // named filename.
            using (var fileStream = file.OpenReadStream())
            {
                await uploadFile.UploadFromStreamAsync(fileStream);
            }
        }

        public async Task StoreFile(string filename, string container, IFormFile file)
        {
            await GetContainer(container);
            // Get a reference to a blob named filename.
            var uploadFile = _container.GetBlockBlobReference(filename);

            if (await uploadFile.ExistsAsync())
            {
                await uploadFile.FetchAttributesAsync();
                if (uploadFile.Properties.Length != file.Length)
                {
                    uploadFile = _container.GetBlockBlobReference(filename.GetUniqueName());
                } else
                {
                    return;
                }
            }

            // Create or overwrite the filename blob with the contents of a local file
            // named filename.
            using (var fileStream = file.OpenReadStream())
            {
                await uploadFile.UploadFromStreamAsync(fileStream);
            }
        }

        public async Task<string> StoreAndGetFile(string filename, string container, IFormFile file)
        {
            await GetContainer(container);
            // Get a reference to a blob named filename.
            var uploadFile = _container.GetBlockBlobReference(filename);

            if (await uploadFile.ExistsAsync())
            {
                await uploadFile.FetchAttributesAsync();
                if (uploadFile.Properties.Length == file.Length)
                {
                    uploadFile = _container.GetBlockBlobReference(filename.GetUniqueName());
                }
            }

            uploadFile.Properties.ContentType = file.ContentType;

            // Create or overwrite the filename blob with the contents of a local file
            // named filename.
            using (var fileStream = file.OpenReadStream())
            {
                await uploadFile.UploadFromStreamAsync(fileStream);
            }

            return uploadFile.Uri.ToString();
        }

        public async Task<string> GetFile(string filename, string containerName)
        {
            var url = $"{_config["BlobService:StorageUrl"]}{containerName}/";
            await GetContainer(containerName);
            // Get a reference to a blob named filename.
            var getFile = _container.GetBlockBlobReference(filename);

            if (await getFile.ExistsAsync()) return $"{url}{filename}";

            var oldFile = $"https://plainstsunami.com/images/{containerName}/{filename}";
            using (var client = new WebClient())
            {
                try
                {
                    using (var stream = client.OpenRead(oldFile))
                    {
                        await getFile.UploadFromStreamAsync(stream);
                    }
                }
                catch (Exception)
                {
                    //Do Nothing
                }
                    
            }
            return $"{url}{filename}";
        }

        public async Task DeleteFile(string filename, string containerName)
        {
            await GetContainer(containerName);
            var file = _container.GetBlockBlobReference(filename);
            await file.DeleteIfExistsAsync();
        }
    }
}
