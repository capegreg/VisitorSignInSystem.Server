using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class VisitorStatus
    {
        public VisitorStatus()
        {
            Visitors = new HashSet<Visitor>();
        }

        public string StatusName { get; set; }
        public string StatusDescription { get; set; }

        public virtual ICollection<Visitor> Visitors { get; set; }
    }
}
