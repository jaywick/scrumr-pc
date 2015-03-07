using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    public class Ticket : Entity
    {
        public string Description { get; set; }

        [Foreign]
        public int FeatureId { get; set; }

        [Foreign]
        public int SprintId { get; set; }

        public int ProjectTicketId { get; set; }

        [JsonIgnore]
        public virtual Feature Feature { get; set; }

        [JsonIgnore]
        public virtual Sprint Sprint { get; set; }

        public TicketType Type { get; set; }

        public TicketState State { get; set; }

        [JsonIgnore]
        public Project Project
        {
            get
            {
                if (Sprint == null)
                    return null;

                return Sprint.Project;
            }
        }

        [JsonIgnore]
        public int? ProjectId
        {
            get
            {
                if (Sprint == null)
                    return null;

                return Sprint.ProjectId;
            }
        }

        [JsonIgnore]
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
