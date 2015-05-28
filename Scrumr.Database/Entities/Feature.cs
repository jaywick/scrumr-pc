using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Scrumr.Database
{
    public class Feature : Entity
    {
        public Feature()
            : base(null)
        {
        }

        public Feature(ScrumrContext context)
            : base(context)
        {
        }

        public Feature(string name, Sprint sprint)
            : base(sprint.Context)
        {
            this.Name = name;
            this.SprintId = sprint.ID;
        }

        public bool IsArchived { get; set; }

        public bool IsMinimised { get; set; }

        [Foreign, IgnoreRender]
        public Guid SprintId { get; set; }

        [JsonIgnore]
        public virtual Sprint Sprint
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

        [JsonIgnore, IgnoreRender]
        public IEnumerable<Ticket> Tickets
        {
            get
            {
                return Context.Tickets
                    .Where(x => x.FeatureId == ID);
            }
        }
    }
}
