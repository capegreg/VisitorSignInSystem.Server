using System;
using System.Collections.Generic;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class Department
    {
        public Department()
        {
            Transfers = new HashSet<Transfer>();
            VsisUsers = new HashSet<VsisUser>();
        }

        public sbyte Id { get; set; }
        public string DepartmentName { get; set; }
        public string Symbol { get; set; }
        public string SymbolType { get; set; }
        public sbyte OrderBy { get; set; }
        public DateTime Created { get; set; }
        public virtual ICollection<Transfer> Transfers { get; set; }
        public virtual ICollection<VsisUser> VsisUsers { get; set; }
    }
}
