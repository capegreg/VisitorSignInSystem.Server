using System;

namespace VisitorSignInSystem.Server.Models
{
    public class Queue
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Icon { get; set; }
        public string AssignedCounter { get; set; }
        public string StatusName { get; set; }
        public DateTime QueueTimeStamp { get; set; }
    }
}
