using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class Visitor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Kiosk { get; set; }
        public int Location { get; set; }
        public string Mobile { get; set; }
        public bool IsHandicap { get; set; }
        public bool IsTransfer { get; set; }
        public ulong VisitCategoryId { get; set; }
        public sbyte? VisitDepartmentId { get; set; }
        public string StatusName { get; set; }
        public string AssignedCounter { get; set; }
        public DateTime Created { get; set; }
        public DateTime? CalledTime { get; set; }

        public virtual VisitorStatus StatusNameNavigation { get; set; }
    }
}
