﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    [Table("Tickets")]
    public class Ticket : Entity
    {
        public string Description { get; set; }

        [Foreign]
        public long FeatureId { get; set; }

        [Foreign]
        public long SprintId { get; set; }

        public long ProjectTicketId { get; set; }

        public virtual Feature Feature { get; set; }

        public virtual Sprint Sprint { get; set; }

        public TicketType Type { get; set; }

        public TicketState State { get; set; }

        [NotMapped]
        public Project Project
        {
            get { return Sprint.IfNotNull(x => x.Project); }
        }

        [NotMapped]
        public long? ProjectId
        {
            get { return Sprint.IfNotNull(x => x.ProjectId); }
        }
    }
}
