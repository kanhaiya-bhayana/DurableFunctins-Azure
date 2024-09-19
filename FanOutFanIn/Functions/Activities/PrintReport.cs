using FanOutFanIn.DataContracts;
using FanOutFanIn.Service.Interfaces;
using Microsoft.Azure.Functions.Worker;

namespace FanOutFanIn.Functions.Activities
{
    public class PrintReport(ISentimentReportGenerator _reportGenerator)
    {
        [Function(nameof(PrintReport))]
        public async Task Print([ActivityTrigger] List<SentimentResult> results)
        {
            await _reportGenerator.GenerateReport(results);
        }
    }
}
