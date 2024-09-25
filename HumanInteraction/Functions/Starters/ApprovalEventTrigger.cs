using HumanInteraction.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanInteraction.Functions.Starters
{
    public class ApprovalEventTrigger(ILogger<ApprovalEventTrigger> _logger)
    {
        [Function("ApprovalTrigger")]
        public async Task<IActionResult> ApprovalTrigger(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "approve/{instanceId}")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        string instanceId)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // Deserialize JSON using Newtonsoft.Json
            var input = JsonConvert.DeserializeObject<dynamic>(requestBody);

            string approvalStatus = input?.approvalStatus;

            if (string.IsNullOrEmpty(approvalStatus))
            {
                return new BadRequestObjectResult("Please provide an approval status");
            }

            // Raise the external event with the status
            await client.RaiseEventAsync(instanceId, "ApprovalTrigger", approvalStatus);

            _logger.LogInformation($"Approval status '{approvalStatus}' sent to instance {instanceId}");

            return new OkObjectResult(approvalStatus);
        }

    }
}
