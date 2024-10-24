using LeaveApproval.DataContracts;
using LeaveApproval.FunctionApp.Orchestrator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;

namespace LeaveApproval.FunctionApp.HttpFunctions
{
    public class StartLeaveApplication
    {
        [Function("StartLeaveApplication")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient starter)
        {
            // Parse request body
            var leaveApplication = await req.GetFromBody<LeaveApplication>();

            // Start the orchestration
            var instanceId = await starter.ScheduleNewOrchestrationInstanceAsync(nameof(LeaveApprovalOrchestrator.RunOrchestrator), input: leaveApplication);
            return starter.CreateCheckStatusResponse(req, instanceId);

            //return new OkObjectResult(new
            //{
            //    message = "Leave application started successfully.",
            //    instanceId = instanceId,
            //    statusQueryGetUri = $"{req.Url}/instances/{instanceId}/status"
            //});
        }
    }
}
