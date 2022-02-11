using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class VisitorMetric
    {
        public int Id { get; set; }
        public string Agent { get; set; }
        public string Kiosk { get; set; }
        public int Location { get; set; }
        public bool IsHandicap { get; set; }
        public ulong VisitCategoryId { get; set; }
        public string AssignedCounter { get; set; }
        public DateTime Created { get; set; }
        public DateTime? CallDuration { get; set; }
    }
}
