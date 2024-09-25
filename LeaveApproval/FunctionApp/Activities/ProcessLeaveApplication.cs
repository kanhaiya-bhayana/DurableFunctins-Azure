using LeaveApproval.DataContracts;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveApproval.FunctionApp.Activities
{
    public class ProcessLeaveApplication
    {
        [Function("ProcessApplication")]
        public bool ProcessApplication(
            [ActivityTrigger] LeaveApplication leaveApplication)
        {
            // Basic validation logic
            int leaveDays = leaveApplication.GetLeaveDuration();
            return leaveDays > 0 && !string.IsNullOrEmpty(leaveApplication.EmployeeId);
        }
    }
}
