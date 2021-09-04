using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class GroupDevice
    {
        public int Id { get; set; }
        public string Kind { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Location { get; set; }
        public bool CanReceive { get; set; }
        public bool CanSend { get; set; }
        public bool Enabled { get; set; }
        public DateTime Created { get; set; }

        public virtual DeviceType KindNavigation { get; set; }
    }
}
