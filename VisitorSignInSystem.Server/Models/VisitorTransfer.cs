using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorSignInSystem.Server.Models
{
    public partial class VisitorTransferLog
    {
        public int Id { get; set; }
        public int VisitorId { get; set; }
        // required
        public sbyte Department { get; set; }
        public ulong VisitCategoryId { get; set; }
        public int Location { get; set; }
        public DateTime Created { get; set; }
        public DateTime? CalledTime { get; set; }

        public virtual Department DepartmentNavigation { get; set; }
    }
}
