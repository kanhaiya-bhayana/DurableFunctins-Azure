using LeaveApproval.DataContracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveApproval.FunctionApp.Activities
{
    public class NotifyEmployee
    {
        [Function("NotifyEmployeeActivity")]
        public static void NotifyEmployeeActivity(
            [ActivityTrigger] LeaveApprovalResult approvalResult,
            ILogger log)
        {
            // Logic to notify employee (e.g., send email)
            string status = approvalResult.IsApproved ? "approved" : "rejected";
            log.LogInformation($"Employee {approvalResult.EmployeeId} notified: Leave {status}.");
        }
    }
}
