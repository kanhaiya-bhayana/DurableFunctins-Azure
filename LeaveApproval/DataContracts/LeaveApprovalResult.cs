using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveApproval.DataContracts
{
    public class LeaveApprovalResult
    {
        public string EmployeeId { get; set; }     
        public bool IsApproved { get; set; }       
        public string ApproverId { get; set; }     
        public DateTime ApprovalDate { get; set; } 
        public string Comments { get; set; }       // Additional comments from the approver (optional)

        // Constructor
        public LeaveApprovalResult(string employeeId, bool isApproved, string approverId, DateTime approvalDate, string comments = "")
        {
            EmployeeId = employeeId;
            IsApproved = isApproved;
            ApproverId = approverId;
            ApprovalDate = approvalDate;
            Comments = comments;
        }

        // Method to get a summary of the approval result
        public string GetSummary()
        {
            string status = IsApproved ? "approved" : "rejected";
            return $"Leave for employee {EmployeeId} was {status} by {ApproverId} on {ApprovalDate.ToShortDateString()}. Comments: {Comments}";
        }
    }
}
