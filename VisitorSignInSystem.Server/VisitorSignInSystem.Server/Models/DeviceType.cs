using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class DeviceType
    {
        public DeviceType()
        {
            GroupDevices = new HashSet<GroupDevice>();
        }

        public string Kind { get; set; }
        public bool Enabled { get; set; }
        public DateTime Created { get; set; }

        public virtual ICollection<GroupDevice> GroupDevices { get; set; }
    }
}
