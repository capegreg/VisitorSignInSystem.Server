using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class Category
    {
        public ulong Id { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public string Icon { get; set; }
        public sbyte Location { get; set; }
        public DateTime Created { get; set; }
    }
}
