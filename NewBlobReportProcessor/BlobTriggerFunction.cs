using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class BlobTriggerFunction
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly string _containerName;
    private static readonly string _projectDirectory = Directory.GetCurrentDirectory();

    public BlobTriggerFunction(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetValue<string>("ConnectionStrings:storageConnectionString");
        _containerName = _config.GetValue<string>("ConnectionStrings:containerName");
    }

    [Function("GetLatestBlobAndProcess")]
    public async Task Run(
        [BlobTrigger("reports/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob,
        string name,
        FunctionContext context)
    {
        var logger = context.GetLogger("GetLatestBlobAndProcess");
        logger.LogInformation($"Blob trigger function processed blob\n Name: {name}");

        // Connect to your blob container
        BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        // Create a list to store blob items
        List<BlobItem> blobItems = new List<BlobItem>();

        // Enumerate all the blobs asynchronously and add them to the list
        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            blobItems.Add(blobItem);
        }

        // Get the most recently modified blob in the container
        BlobItem latestBlob = blobItems
            .OrderByDescending(b => b.Properties.LastModified)
            .FirstOrDefault();

        if (latestBlob != null)
        {
            logger.LogInformation($"Latest blob found: {latestBlob.Name} (Last Modified: {latestBlob.Properties.LastModified})");
            await DownloadBlobAsync(latestBlob.Name, logger);
        }
        else
        {
            logger.LogWarning("No blobs found in the container.");
        }
    }

    private async Task DownloadBlobAsync(string selectedBlobName, ILogger logger)
    {
        try
        {
            // Ensure the reports directory exists
            string reportsDirectory = Path.Combine(_projectDirectory, "reports");
            if (!Directory.Exists(reportsDirectory))
            {
                Directory.CreateDirectory(reportsDirectory);
            }

            // Sanitize the blob name for use in a file path
            string sanitizedBlobName = selectedBlobName.Replace(":", "-"); // Replace colon with hyphen

            // Set the full download file path
            string downloadFilePath = Path.Combine(reportsDirectory, sanitizedBlobName);

            // Create a BlobServiceClient
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

            // Get a BlobContainerClient
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // Get a reference to the blob you want to download (dynamically set)
            BlobClient blobClient = containerClient.GetBlobClient(selectedBlobName);

            logger.LogInformation($"Starting download of blob '{selectedBlobName}' to '{downloadFilePath}'...");

            // Download the blob to a file
            BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();

            using (FileStream fs = File.OpenWrite(downloadFilePath))
            {
                await blobDownloadInfo.Content.CopyToAsync(fs);
                fs.Close();
            }

            logger.LogInformation($"Blob '{selectedBlobName}' downloaded successfully to '{downloadFilePath}'!");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error occurred while downloading blob: {ex.Message}");
        }
    }
}
