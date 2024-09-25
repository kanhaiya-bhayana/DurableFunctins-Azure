using HumanInteraction.DataContracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanInteraction.Functions.Activities
{
    public class ValidateExpenseReport(ILogger<ValidateExpenseReport> _logger)
    {
        [Function("ValidateReport")]
        public bool ValidateReport([ActivityTrigger] ExpenseReport report, FunctionContext context)
        {
            _logger.LogInformation($"Validating expense report for employee: {report.EmployeeId}");

            // Example validation logic
            if (report.Amount > 0 && report.Amount <= 1000)
            {
                return true;
            }

            return false;
        }
    }
}
