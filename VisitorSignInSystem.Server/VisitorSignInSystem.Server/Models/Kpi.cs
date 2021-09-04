using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class Kpi
    {
        public DateTime KpiEvent { get; set; }
        public int AvgWaitTime { get; set; }
        public int MinWaitTime { get; set; }
        public int MaxWaitTime { get; set; }
        public int VisitsByHr { get; set; }
        public int AgentsByHr { get; set; }
        public int CategoryiesByHr { get; set; }
    }
}
