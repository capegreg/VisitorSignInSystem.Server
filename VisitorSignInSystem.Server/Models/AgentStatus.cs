using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class AgentStatus
    {
        public AgentStatus()
        {
            Agents = new HashSet<Agent>();
        }

        public string StatusName { get; set; }
        public string StatusDescription { get; set; }

        public virtual ICollection<Agent> Agents { get; set; }
    }
}
