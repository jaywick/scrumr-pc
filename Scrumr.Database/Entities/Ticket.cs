using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrumr.Core.Extensions;

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
        public Guid FeatureId { get; set; }

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

        public TicketType Type { get; set; }

        public TicketState State { get; set; }

        public DateTime? LastClosed { get; set; }

        [JsonIgnore]
        public Sprint Sprint
        {
            get
            {
                if (Feature == null)
                    return null;

                return Feature.Sprint;
            }
        }

        [JsonIgnore]
        public Project Project
        {
            get
            {
                if (Feature == null)
                    return null;

                return Feature.Sprint.Project;
            }
        }

        [JsonIgnore]
        public Guid? ProjectId
        {
            get
            {
                if (Feature == null)
                    return null;

                return Feature.Sprint.ProjectId;
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
            get { return Feature.Sprint.IsBacklog; }
        }

        public void Open()
        {
            State = TicketState.Open;
        }

        public void Close()
        {
            State = TicketState.Closed;
            LastClosed = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", ID.ToSubcode(), Name);
        }
    }
}
