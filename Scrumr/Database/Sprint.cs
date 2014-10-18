﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Scrumr
{
    public class Sprint : Entity
    {
        //[Key]
        //public Int64 ID { get; set; }
        //
        //public string Name { get; set; }

        [IgnoreRender]
        public Int64 ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
    }
}