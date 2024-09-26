using LeaveApproval.DataContracts;
using LeaveApproval.FunctionApp.Orchestrator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveApproval.FunctionApp.HttpFunctions
{
    public class StartLeaveApplication
    {
        [Function("StartLeaveApplication")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient starter)
        {
            // Parse request body
            var leaveApplication = await req.GetFromBody<LeaveApplication>();

            // Start the orchestration
            var instanceId = await starter.ScheduleNewOrchestrationInstanceAsync(nameof(LeaveApprovalOrchestrator.RunOrchestrator), input: leaveApplication);

            var response = starter.CreateCheckStatusResponse(req, instanceId);
            //_logger.LogInformation($"[Completed]: {nameof(HttpStart)}");
            //return response;

            return new OkObjectResult($"Leave application started with instance ID: {instanceId}");
        }
    }
}
