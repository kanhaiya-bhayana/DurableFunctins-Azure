using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveApproval.DataContracts
{
    public class LeaveApplication
    {
        public string EmployeeId { get; set; }    
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }     
        public string LeaveType { get; set; }     // Type of leave (e.g., Sick, Vacation, Unpaid)
        public string Reason { get; set; }        // Reason for the leave (optional)

        // Constructor
        public LeaveApplication(string employeeId, DateTime startDate, DateTime endDate, string leaveType, string reason = "")
        {
            EmployeeId = employeeId;
            StartDate = startDate;
            EndDate = endDate;
            LeaveType = leaveType;
            Reason = reason;
        }

        // Method to calculate the number of days of leave requested
        public int GetLeaveDuration()
        {
            return (EndDate - StartDate).Days + 1;  // Including both start and end date
        }
    }
}
