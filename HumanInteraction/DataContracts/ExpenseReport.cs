using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanInteraction.DataContracts
{
    public class ExpenseReport
    {
        public string EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
