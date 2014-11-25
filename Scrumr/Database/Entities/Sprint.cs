﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Scrumr
{
    [Table("Sprints")]
    public class Sprint : Entity
    {
        public long ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}
