using System;
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
        public long FeatureId { get; set; }

        [IgnoreRender]
        public long SprintId { get; set; }

        [IgnoreRender]
        public long ProjectTicketId { get; set; }

        [RefersTo("Features", "ID")]
        public Feature Feature { get; set; }

        [RefersTo("Sprints", "ID")]
        public Sprint Sprint { get; set; }

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
