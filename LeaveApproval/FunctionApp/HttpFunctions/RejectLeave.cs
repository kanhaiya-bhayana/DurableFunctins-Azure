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
    public static class RejectLeave
    {
        [Function("RejectLeave")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "RejectLeave/{instanceId}")] HttpRequest req,
            [DurableClient] DurableTaskClient client,
            string instanceId)
        {
            // Signal the orchestrator that the leave is rejected
            await client.RaiseEventAsync(instanceId, "ManagerApproval", false);

            return new OkObjectResult($"Leave rejected for instance ID: {instanceId}");
        }
    }
}
