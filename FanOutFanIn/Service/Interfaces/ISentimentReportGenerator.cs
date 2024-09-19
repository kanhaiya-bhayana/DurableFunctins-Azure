using FanOutFanIn.DataContracts;

namespace FanOutFanIn.Service.Interfaces
{
    public interface ISentimentReportGenerator
    {
        Task<Uri> GenerateReport(List<SentimentResult> results);
    }
}
