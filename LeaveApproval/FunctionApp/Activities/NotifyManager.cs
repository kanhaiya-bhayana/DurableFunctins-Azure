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
    public class NotifyManager
    {
        [Function("NotifyManagerActivity")]
        public static void NotifyManagerActivity(
            [ActivityTrigger] LeaveApplication leaveApplication,
            ILogger log)
        {
            // Logic to notify manager (e.g., send email)
            log.LogInformation($"Manager notified about leave request from Employee {leaveApplication.EmployeeId}.");
        }
    }
}
