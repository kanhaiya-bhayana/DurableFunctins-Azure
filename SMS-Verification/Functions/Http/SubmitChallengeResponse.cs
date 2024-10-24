using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace SMS_Verification.Functions.Http
{
    public class SubmitChallengeResponse
    {
        [Function("SubmitChallenge")]
        public static async Task<IActionResult> SubmitChallenge(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [DurableClient] DurableTaskClient orchestrationClient,
            ILogger log)
        {
            string instanceId = req.Query["instanceId"];
            string code = req.Query["code"];

            if (string.IsNullOrEmpty(instanceId) || string.IsNullOrEmpty(code))
            {
                return new BadRequestObjectResult("InstanceId and code are required.");
            }

            //log.LogInformation($"Submitting challenge code {code} for instance {instanceId}.");

            await orchestrationClient.RaiseEventAsync(instanceId, "SmsChallengeResponse", int.Parse(code));

            return new OkObjectResult($"Code {code} submitted successfully.");
        }
    }
}
