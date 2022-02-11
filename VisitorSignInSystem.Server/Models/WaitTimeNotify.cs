using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class WaitTimeNotify
    {
        public string Mail { get; set; }
        public ulong Category { get; set; }
        public byte MaxWaitTimeMinutes { get; set; }

        public virtual Category CategoryNavigation { get; set; }
    }
}
