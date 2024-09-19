using Test.Functions.Orchestrator;

namespace Test.Functions.Start
{
    public class FunctionChainingStart(ILogger<FunctionChainingStart> _logger)
    {
        [Function("FunctionChaining_HttpStart")]
        public async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext context)
        {
            _logger.LogInformation($"Started: {context.FunctionDefinition.Name}");

            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(FunctionChainingOrchestrator));
            var response = await client.CreateCheckStatusResponseAsync(req, instanceId);
            _logger.LogInformation($"Completed: {nameof(FunctionChainingStart)}");
            return response;
        }
    }
}
