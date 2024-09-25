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


    //[Function("ApprovalWorkflow_HttpStart")]
    //public async Task<HttpResponseData> HttpStart(
    //        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
    //        [DurableClient] DurableTaskClient client,
    //        FunctionContext executionContext)
    //{
    //    _logger.LogInformation($"[Started]: {nameof(HttpStart)}");
    //    var input = await req.GetFromBody<ExpenseReport>();
    //    var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(ApprovalWorkflowOrchestrator), input: input);
    //    var response = client.CreateCheckStatusResponse(req, instanceId);
    //    _logger.LogInformation($"[Completed]: {nameof(HttpStart)}");
    //    return response;
    //}
}
