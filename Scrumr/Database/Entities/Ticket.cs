using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public long FeatureId { get; set; }

        public long SprintId { get; set; }

        public long ProjectTicketId { get; set; }

        public Feature Feature { get; set; }

        public Sprint Sprint { get; set; }

        public TicketType Type { get; set; }

        public TicketState State { get; set; }

        public Project Project
        {
            get { return Sprint.IfNotNull(x => x.Project); }
        }

        public long? ProjectId
        {
            get { return Sprint.IfNotNull(x => x.ProjectId); }
        }
    }
}
