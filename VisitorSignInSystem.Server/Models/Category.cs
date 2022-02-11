using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class Category
    {
        public Category()
        {
            WaitTimeNotifies = new HashSet<WaitTimeNotify>();
        }

        public ulong Id { get; set; }
        public string Description { get; set; }
        public sbyte DepartmentId { get; set; }
        public bool? Active { get; set; }
        public string Icon { get; set; }
        public sbyte Location { get; set; }
        public DateTime Created { get; set; }

        public virtual ICollection<WaitTimeNotify> WaitTimeNotifies { get; set; }
    }
}
