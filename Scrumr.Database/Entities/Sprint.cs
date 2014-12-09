﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Scrumr.Database
{
    [Table("Sprints")]
    public class Sprint : Entity
    {
        public Sprint() { }

        public Sprint(string name, Project project)
        {
            this.Name = name;
            this.Project = project;
        }

        [Foreign]
        public long ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}
