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
        public Ticket()
            : base(null)
        {
        }

        public Ticket(ScrumrContext context)
            : base(context)
        {
        }

        [LargeText]
        public string Description { get; set; }

        [Foreign]
        public int FeatureId { get; set; }

        [Foreign]
        public int SprintId { get; set; }

        public int ProjectTicketId { get; set; }

        [JsonIgnore]
        public Feature Feature
        {
            get
            {
                return Context.Features[FeatureId];
            }
            set
            {
                FeatureId = value.ID;
            }
        }
        [JsonIgnore]
        public Sprint Sprint
        {
            get
            {
                return Context.Sprints[SprintId];
            }
            set
            {
                SprintId = value.ID;
            }
        }

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

        [JsonIgnore]
        public bool IsBacklogged
        {
            get { return Sprint.IsBacklog; }
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
