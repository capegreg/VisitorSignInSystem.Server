using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorSignInSystem.Server.Models
{
    public class CounterDetail
    {
        public string Host { get; set; }
        public string CounterNumber { get; set; }
        public string Description { get; set; }
        public string CounterStatus { get; set; }
        public string AgentFullName { get; set; }
        public int? VisitorId { get; set; }
    }
}
