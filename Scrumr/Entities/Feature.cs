﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrumr
{
    public class Feature : Entity
    {
        [Foreign(typeof(Project))]
        public int ProjectId { get; set; }
    }
}
