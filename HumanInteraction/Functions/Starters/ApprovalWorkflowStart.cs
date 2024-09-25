using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Castle.Core.Logging;
using HumanInteraction.DataContracts;
using HumanInteraction.Functions.Orchestrations;

namespace HumanInteraction.Functions.Starters
{
    public class ApprovalWorkflowStart(ILogger<ApprovalWorkflowStart> _logger)
    {
        [Function("ApprovalWorkflow_HttpStart")]
        public async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            _logger.LogInformation($"[Started]: {nameof(HttpStart)}");
            var input = await req.GetFromBody<ExpenseReport>();
            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(ApprovalWorkflowOrchestrator), input: input);
            var response =  client.CreateCheckStatusResponse(req, instanceId);
            _logger.LogInformation($"[Completed]: {nameof(HttpStart)}");
            return response;
        }
    }
}
