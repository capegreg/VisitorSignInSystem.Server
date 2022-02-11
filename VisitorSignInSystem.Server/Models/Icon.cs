﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorSignInSystem.Server.Models
{
    public class IconInventory
    {
        public int Id { get; set; }
        public string Icon { get; set; }
        public int ControlType { get; set; }
        public string Description { get; set; }
    }
}
