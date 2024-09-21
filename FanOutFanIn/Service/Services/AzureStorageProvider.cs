using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FanOutFanIn.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FanOutFanIn.Service.Services
{
    public class AzureStorageProvider
        (IConfiguration _config
        , ILogger<AzureStorageProvider> _logger): IAzureStorageProvider
    {
        public async Task<Uri> UploadBlobFromStreamAsync(Stream stream, string blobName, string containerName)
        {
            _logger.LogInformation( $"[Started]: {nameof(UploadBlobFromStreamAsync)} for the {nameof(blobName)}" );
            var blob = await GetBlobClientAsync(blobName, containerName);
            await blob.UploadAsync(stream, overwrite: true);
            _logger.LogInformation( $"[Completed]: {nameof(UploadBlobFromStreamAsync)} for the {nameof(blobName)}" );
            return blob.Uri;
        }

        private BlobContainerClient CreateContainerClient(string containerName)
        {
            return new BlobContainerClient(_config.GetValue<string>("ConnectionStrings:storageConnectionString"), containerName);
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
