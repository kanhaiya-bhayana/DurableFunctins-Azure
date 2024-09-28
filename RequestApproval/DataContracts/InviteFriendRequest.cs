using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestApproval.DataContracts
{
    public class InviteFriendRequest
    {
        public string? Friend { get; set; }
        public int ReminderCount { get; set; }
    }
}
