using HumanInteraction.DataContracts;
using HumanInteraction.Functions.Activities;
using HumanInteraction.Functions.Starters;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace HumanInteraction.Functions.Orchestrations
{
    public class ApprovalWorkflowOrchestrator(ILogger<ApprovalWorkflowOrchestrator> _logger)
    {
        [Function("ApprovalWorkflowOrchestrator")]
        public async Task<string> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var input = context.GetInput<ExpenseReport>();

            // Call the validation function
            bool isValid = await context.CallActivityAsync<bool>(nameof(ValidateExpenseReport.ValidateReport), input);

            if (!isValid)
            {
                throw new Exception("Expense report is not valid");
            }

            // Wait for human approval
            var approvalStatus = await context.WaitForExternalEvent<string>(nameof(ApprovalEventTrigger.ApprovalTrigger));

            _logger.LogInformation($"[Completed]: {nameof(RunOrchestrator)}");

            if (approvalStatus == "Approved")
            {
                await context.CallActivityAsync(nameof(ProcessExpenseReport.ProcessReport), input);

                return "Expense report approved and processed";
            }
            else
            {
                return "Expense report rejected";
            }
        }
    }
}
