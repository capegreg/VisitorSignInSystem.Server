using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class CategoryMetric
    {
        public ulong Category { get; set; }
        public string Description { get; set; }
        public int CategoryToday { get; set; }
        public int CategoryWtd { get; set; }
        public int CategoryMtd { get; set; }
        public int CategoryYtd { get; set; }
        public double CallTimeToday { get; set; }
        public double CallTimeWtd { get; set; }
        public double CallTimeMtd { get; set; }
        public double CallTimeYtd { get; set; }
        public DateTime Created { get; set; }
    }
}
