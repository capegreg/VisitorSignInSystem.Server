using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class Agent
    {
        public string AuthName { get; set; }
        public string StatusName { get; set; }
        public int Categories { get; set; }
        public int? VisitorId { get; set; }
        public string Counter { get; set; }

        public virtual AgentStatus StatusNameNavigation { get; set; }
    }
}
