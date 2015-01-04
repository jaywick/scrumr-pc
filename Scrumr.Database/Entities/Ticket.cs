using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
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
            get
            {
                if (Sprint == null)
                    return null;

                return Sprint.Project;
            }
        }

        [NotMapped]
        public long? ProjectId
        {
            get
            {
                if (Sprint == null)
                    return null;

                return Sprint.ProjectId;
            }
        }

        [NotMapped]
        public bool IsOpen
        {
            get { return State == TicketState.Open; }
        }
        
        public void Open()
        {
            State = TicketState.Open;
        }

        public void Close()
        {
            State = TicketState.Closed;
        }
    }
}
