using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class AgentMetric
    {
        public string AuthName { get; set; }
        public int? Today { get; set; }
        public int? Wtd { get; set; }
        public int? Mtd { get; set; }
        public int? Ytd { get; set; }
        public int? CallTimeToday { get; set; }
        public int? CallTimeWtd { get; set; }
        public int? CallTimeMtd { get; set; }
        public int? CallTimeYtd { get; set; }
    }
}
