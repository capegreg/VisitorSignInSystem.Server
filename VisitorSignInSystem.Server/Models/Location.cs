using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class Location
    {
        public Location()
        {
            VsisUsers = new HashSet<VsisUser>();
        }

        public sbyte Id { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public bool? Open { get; set; }
        public DateTime Created { get; set; }

        public virtual ICollection<VsisUser> VsisUsers { get; set; }
    }
}
