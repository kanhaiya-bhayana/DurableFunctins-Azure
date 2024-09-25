using HumanInteraction.DataContracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HumanInteraction.Functions.Activities
{
    public class ProcessExpenseReport(ILogger<ProcessExpenseReport> _logger)
    {
        [Function("ProcessReport")]
        public void ProcessReport([ActivityTrigger] ExpenseReport report, FunctionContext context)
        {
            _logger.LogInformation($"Processing expense report for employee: {report.EmployeeId}, Amount: {report.Amount}");
            _logger.LogInformation($"Processed expense report for employee: {report.EmployeeId}, Amount: {report.Amount}");

            // Example processing logic (e.g., save to database, notify employee, etc.)
        }
    }
}
