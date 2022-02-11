using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class AgentMetric
    {
        public string AuthName { get; set; }        
        public int VisitorsToday { get; set; }
        public int VisitorsWtd { get; set; }
        public int VisitorsMtd { get; set; }
        public int VisitorsYtd { get; set; }
        public double CallTimeToday { get; set; }
        public double CallTimeWtd { get; set; }
        public double CallTimeMtd { get; set; }
        public double CallTimeYtd { get; set; }
        public DateTime Created { get; set; }
    }
}
