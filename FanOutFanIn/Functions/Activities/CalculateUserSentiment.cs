using FanOutFanIn.DataContracts;
using FanOutFanIn.Service.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FanOutFanIn.Functions.Activities
{
    public class CalculateUserSentiment
        (ILogger<CalculateUserSentiment> _logger,
        ICalculateProbabilityService _calculatorProbabilityService)
    {
        [Function(nameof(CalculateSentiment))]
        public async Task<SentimentResult> CalculateSentiment([ActivityTrigger] int userId)
        {
            _logger.LogInformation($"[Started]: {nameof(CalculateSentiment)}");
            SentimentResult result = await _calculatorProbabilityService.Calculate(userId);
            _logger.LogInformation($"[Completed]: {nameof(CalculateSentiment)}");
            return result;
        }
    }
}
