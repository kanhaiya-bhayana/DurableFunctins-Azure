using LeaveApproval.DataContracts;
using LeaveApproval.FunctionApp.Activities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;


namespace LeaveApproval.FunctionApp.Orchestrator
{
    public class LeaveApprovalOrchestrator
    {
        [Function("RunOrchestrator")]
        public async Task<Object> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var leaveApplication = context.GetInput<LeaveApplication>();

            // Step 1: Process leave application (e.g., basic validation)
            bool isValid = await context.CallActivityAsync<bool>(
                nameof(ProcessLeaveApplication.ProcessApplication), leaveApplication);

            if (!isValid)
            {
                // Return a rejection result if validation fails
                return new LeaveApprovalResult(
                    leaveApplication.EmployeeId,
                    false,
                    "System",
                    context.CurrentUtcDateTime,
                    "Leave application is invalid.");
            }

            // Step 2: Notify manager for approval
            await context.CallActivityAsync(
                "NotifyManagerActivity", leaveApplication);

            // Step 3: Wait for manager's decision
            using (var timeoutCts = new CancellationTokenSource())
            {
                DateTime dueTime = context.CurrentUtcDateTime.AddSeconds(40); // Manager has 2 days to respond
                Task timeoutTask = context.CreateTimer(dueTime, timeoutCts.Token);

                Task<bool> approvalEvent = context.WaitForExternalEvent<bool>("ManagerApproval");

                Task winner = await Task.WhenAny(approvalEvent, timeoutTask);

                if (winner == approvalEvent)
                {
                    // Cancel the timeout task
                    timeoutCts.Cancel();

                    bool isApproved = approvalEvent.Result;

                    // Step 4: Notify employee of the decision
                    await context.CallActivityAsync(
                        nameof(NotifyEmployee.NotifyEmployeeActivity),
                        new LeaveApprovalResult(
                            leaveApplication.EmployeeId,
                            isApproved,
                            "Manager123", // Replace with actual manager ID
                            context.CurrentUtcDateTime,
                            isApproved ? "Leave approved." : "Leave rejected."
                        ));
                    // leave approved enter in database - > 
                    // Return the approval result
                    return new LeaveApprovalResult(
                        leaveApplication.EmployeeId,
                        isApproved,
                        "Manager123",
                        context.CurrentUtcDateTime,
                        isApproved ? "Leave approved." : "Leave rejected.");
                }
                else
                {
                    // Timeout occurred
                    //_logger.LogWarning("Manager did not respond in time.");

                    // Optionally, you can set a default action or notify the employee
                    await context.CallActivityAsync(
                        nameof(NotifyEmployee.NotifyEmployeeActivity),
                        new LeaveApprovalResult(
                            leaveApplication.EmployeeId,
                            false,
                            "System",
                            context.CurrentUtcDateTime.Date,
                            "Leave request timed out due to no response from manager."
                        ));
                    LeaveApprovalResult res = new LeaveApprovalResult(
                        leaveApplication.EmployeeId,
                        false,
                        "System",
                        context.CurrentUtcDateTime.Date,
                        "Manager did not respond in time.");
                    return "hi";
                }
            }
        }
    }
}
