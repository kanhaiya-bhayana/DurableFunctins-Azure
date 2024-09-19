using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FanOutFanIn.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FanOutFanIn.Service.Services
{
    public class AzureStorageProvider
        (IConfiguration _config): IAzureStorageProvider
    {
        public async Task<Uri> UploadBlobFromStreamAsync(Stream stream, string blobName, string containerName)
        {
            var blob = await GetBlobClientAsync(blobName, containerName);
            await blob.UploadAsync(stream, overwrite: true);
            return blob.Uri;
        }

        private BlobContainerClient CreateContainerClient(string containerName)
        {
            return new BlobContainerClient(_config["storageConnectionString"], containerName);
        }

        private async Task<BlobContainerClient> CreateOrGetContainerAsync(string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentNullException(nameof(containerName));
            }

            var containerClient = CreateContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

            return containerClient;
        }

        private async Task<BlobClient> GetBlobClientAsync(string blobName, string containerName)
        {
            var container = await CreateOrGetContainerAsync(containerName);
            var blobClient = container.GetBlobClient(blobName);
            return blobClient;
        }
    }
}
