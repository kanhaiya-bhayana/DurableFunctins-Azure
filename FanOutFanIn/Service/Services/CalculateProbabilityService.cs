using FanOutFanIn.DataContracts;
using FanOutFanIn.Service.Interfaces;
using Microsoft.Extensions.Logging;
using RandomNameGeneratorLibrary;


namespace FanOutFanIn.Service.Services
{
    public class CalculateProbabilityService
        (IPersonNameGenerator _nameGenerator,
        ILogger<CalculateProbabilityService> _logger) : ICalculateProbabilityService
    {
        public async Task<SentimentResult> Calculate(int userId)
        {
            _logger.LogInformation( $"[Started]: {nameof(Calculate)} for the user {userId}" );
            var rnd = new Random();

            // simulate some work here...
            await Task.Delay(rnd.Next(0, 5_000));
            var happinessSentiment = rnd.Next(0, 100);

            _logger.LogInformation( $"[Completed]: {nameof(Calculate)} for the user {userId}" );

            // _logger.LogInformation($"Completed sentiment calculation for user {userId}");

            // context.
            return new SentimentResult
            {
                User = GetRandomName(),
                Sentiment = happinessSentiment
            };
        }
        private string GetRandomName()
        {
            return $"{_nameGenerator.GenerateRandomFirstName()} {_nameGenerator.GenerateRandomLastName()}";
        }
    }
}
