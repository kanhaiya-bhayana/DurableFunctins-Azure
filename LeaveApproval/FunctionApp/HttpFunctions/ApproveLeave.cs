using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveApproval.FunctionApp.HttpFunctions
{
    public static class ApproveLeave
    {
        [Function("ApproveLeave")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "ApproveLeave/{instanceId}")] HttpRequest req,
            [DurableClient] DurableTaskClient client,
            string instanceId)
        {
            // Signal the orchestrator that the leave is approved
            await client.RaiseEventAsync(instanceId, "ManagerApproval", true);

            return new OkObjectResult($"Leave approved for instance ID: {instanceId}");
        }
    }
}
