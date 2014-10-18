﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class Ticket : Entity
    {
        public string Description { get; set; }

        [IgnoreRender]
        public Int64 FeatureId { get; set; }

        [IgnoreRender]
        public Int64 SprintId { get; set; }

        [IgnoreRender]
        public Int64 TypeId { get; set; }

        public Int64 ProjectTicketId { get; set; }

        [ForeignKey("FeatureId")]
        public Feature Feature { get; set; }

        [ForeignKey("SprintId")]
        public Sprint Sprint { get; set; }

        [ForeignKey("TypeId")]
        public TicketType Type { get; set; }
    }
}