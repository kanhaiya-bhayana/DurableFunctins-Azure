using FanOutFanIn.DataContracts;
using FanOutFanIn.Service.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanOutFanIn.Functions.Activities
{
    public class CalculateUserSentiment
        (ILogger<CalculateUserSentiment> _logger,
        ICalculateProbabilityService _calculatorProbabilityService)
    {
        [Function(nameof(CalculateSentiment))]
        public async Task<SentimentResult> CalculateSentiment([ActivityTrigger] int userId)
        {
            return await _calculatorProbabilityService.Calculate(userId);
        }
    }
}
