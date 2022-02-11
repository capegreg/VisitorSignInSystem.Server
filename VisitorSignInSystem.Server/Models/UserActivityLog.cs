using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class UserActivityLog
    {
        public string Who { get; set; }
        public string Wat { get; set; }
        public DateTime Wen { get; set; }
    }
}
