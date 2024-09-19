using FanOutFanIn.DataContracts;
using FanOutFanIn.Functions.Orchestrations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using System.Text;
using System.Text.Json;

namespace FanOutFanIn.Functions.Starters
{
    public class FanOutFanInStart
    {
        [Function("FanOutFanIn_HttpStart")]
        public async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        HttpRequestData req,
        [DurableClient] DurableTaskClient client)
        {
            var input = await req.GetFromBody<SentimentUserInput>();            

            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(FanOutFanInOrchestrator), input: input);

            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
