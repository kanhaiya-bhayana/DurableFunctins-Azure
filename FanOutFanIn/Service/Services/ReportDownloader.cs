using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using FanOutFanIn.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FanOutFanIn.Service.Services
{
    public class ReportDownloader
        (IConfiguration _configuration): IReportDownloader
    {
        private readonly string _connectionString = _configuration.GetValue<string>("ConnectionStrings:storageConnectionString");
        private readonly string _containerName = _configuration.GetValue<string>("ConnectionStrings:storageConnectionString");
        private static string binPath = Directory.GetCurrentDirectory();
        private static string _projectDirectory = Directory.GetParent(binPath).Parent.Parent.FullName;
        public async Task<bool> DownloadReportAsync()
        {
            return await GetBlobsAsync();
        }

        private async Task<bool> GetBlobsAsync()
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

                Console.WriteLine($"Listing blobs in container '{_containerName}'");

                List<BlobItem> blobItems = new List<BlobItem>();

                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    blobItems.Add(blobItem);
                }
                BlobItem? latestBlob = blobItems
                    .OrderByDescending(b => b.Properties.LastModified)
                    .FirstOrDefault();

                if (latestBlob != null)
                {
                    Console.WriteLine($"Latest blob found: {latestBlob.Name} (Last Modified: {latestBlob.Properties.LastModified})");
                    return await DownloadBlobAsync(latestBlob.Name);
                }
                else
                {
                    Console.WriteLine("No blobs found in the container.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> DownloadBlobAsync(string selectedBlobName)
        {
            try
            {
                bool response = false;
                string reportsDirectory = Path.Combine(_projectDirectory, "reports");
                if (!Directory.Exists(reportsDirectory))
                {
                    Directory.CreateDirectory(reportsDirectory);
                }

                string sanitizedBlobName = selectedBlobName.Replace(":", "-");

                string downloadFilePath = Path.Combine(reportsDirectory, sanitizedBlobName);

                BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

                BlobClient blobClient = containerClient.GetBlobClient(selectedBlobName);

                Console.WriteLine($"Starting download of blob '{selectedBlobName}' to '{downloadFilePath}'...");

                BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();

                using (FileStream fs = File.OpenWrite(downloadFilePath))
                {
                    await blobDownloadInfo.Content.CopyToAsync(fs);
                    fs.Close();
                }

                Console.WriteLine($"Blob '{selectedBlobName}' downloaded successfully to '{downloadFilePath}'!");
                response = true;
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while downloading blob: {ex.Message}");
                return false;
            }
        }
    }
}
