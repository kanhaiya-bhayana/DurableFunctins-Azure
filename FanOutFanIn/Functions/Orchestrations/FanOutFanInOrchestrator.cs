using FanOutFanIn.DataContracts;
using FanOutFanIn.Functions.Activities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace FanOutFanIn.Functions.Orchestrations
{
    public class FanOutFanInOrchestrator
    {
        [Function(nameof(FanOutFanInOrchestrator))]
        public async Task RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var input = context.GetContextType<SentimentUserInput>();

            // Get a list of N work items to process in parallel.
            var tasks = new List<Task<SentimentResult>>();

            for (var userId = 1; userId <= input.NumberOfUsers; userId++)
            {
                tasks.Add(context.CallActivityAsync<SentimentResult>(nameof(CalculateUserSentiment), userId));
            }
            await Task.WhenAll(tasks);

            var results = tasks.Select(x => x.Result);
            await context.CallActivityAsync(nameof(PrintReport), results);
        }
    }
}
