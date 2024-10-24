using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace FanOutFanIn.Functions.Activities
{
    public class DownloadReportProcess
    {
        private const string _connectionString = "DefaultEndpointsProtocol=https;AccountName=clouddevstrgacc;AccountKey=TRS4w/22G2vs3YPVOLcYkvUqkNkmWKRTQTfJCBZmkpu7E1Ywq5hQ8TG1TR5zAzETa+hNgste7qot+ASt9JZy5A==;EndpointSuffix=core.windows.net";
        private const string _containerName = "reports";
        private static string binPath = Directory.GetCurrentDirectory();
        private static string _projectDirectory = Directory.GetParent(binPath).Parent.Parent.FullName;
        private static string downloadFilePath;


        public static async Task DownloadReport()
        {
            await GetBlobsAsync();
        }

        private static async Task GetBlobsAsync()
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
                    await DownloadBlobAsync(latestBlob.Name);
                }
                else
                {
                    Console.WriteLine("No blobs found in the container.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }

        private static async Task DownloadBlobAsync(string selectedBlobName)
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while downloading blob: {ex.Message}");
            }
        }
    }
}
