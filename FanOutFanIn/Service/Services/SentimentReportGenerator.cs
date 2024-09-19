using Castle.Core.Logging;
using FanOutFanIn.DataContracts;
using FanOutFanIn.Service.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlTableHelper;

namespace FanOutFanIn.Service.Services
{
    public class SentimentReportGenerator
        (ILogger<SentimentReportGenerator> _logger,
        IAzureStorageProvider _storageProvider): ISentimentReportGenerator
    {
        public async Task<Uri> GenerateReport(List<SentimentResult> results)
        {
            _logger.LogInformation($"Generating report for {results.Count} users");
            var reportStream = CreateReportStream(results);

            return await _storageProvider.UploadBlobFromStreamAsync(
                reportStream,
                $"report{DateTime.UtcNow:yyyy-MM-dd}.html",
                "sentiment-reports");
        }

        private static MemoryStream CreateReportStream(List<SentimentResult> results)
        {
            var html = results.OrderByDescending(x => x.Sentiment).ToHtmlTable() ?? "";
            return new MemoryStream(Encoding.UTF8.GetBytes(html));
        }
    }
}
