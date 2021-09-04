﻿using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class VsisUser
    {
        public string AuthName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool? Active { get; set; }
        public sbyte Location { get; set; }
        public DateTime Created { get; set; }

        public virtual Location LocationNavigation { get; set; }
    }
}
