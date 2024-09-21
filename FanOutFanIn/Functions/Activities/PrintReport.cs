using FanOutFanIn.DataContracts;
using FanOutFanIn.Service.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FanOutFanIn.Functions.Activities
{
    public class PrintReport
    (ISentimentReportGenerator _reportGenerator
    , ILogger<PrintReport> _logger)
    {
        [Function(nameof(PrintReport))]
        public async Task Print([ActivityTrigger] List<SentimentResult> results)
        {
            _logger.LogInformation($"[Started]: {nameof(Print)}");
            await _reportGenerator.GenerateReport(results);
            _logger.LogInformation($"[Completed]: {nameof(Print)}");
        }
    }
}
