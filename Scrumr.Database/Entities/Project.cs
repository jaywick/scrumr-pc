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
    public class Project : Entity
    {
        public Project()
            : base(null)
        {
        }

        public Project(ScrumrContext context)
            : base(context)
        {
        }

        public Project(string name, ScrumrContext context)
            : base(context)
        {
            Name = name;
        }

        public int NextProjectTicketId { get; set; }

        [Foreign]
        public Guid? BacklogId { get; set; }

        [JsonIgnore]
        public Sprint Backlog
        {
            get
            {
                if (!BacklogId.HasValue)
                    return null;

                return Context.Sprints[BacklogId.Value];
            }
            set
            {
                BacklogId = value.ID;
            }
        }

        [JsonIgnore]
        public IEnumerable<Sprint> Sprints
        {
            get
            {
                return Context.Sprints
                    .Where(x => x.ProjectId == this.ID);
            }
        }

        [JsonIgnore]
        public IEnumerable<Feature> Features
        {
            get
            {
                return Context.Features
                    .Where(x => x.Sprint.ProjectId == this.ID);
            }
        }

        [JsonIgnore]
        public IEnumerable<Ticket> Tickets
        {
            get
            {
                return Context.Tickets
                    .Where(x => x.Feature.Sprint.ProjectId == this.ID);
            }
        }

        [JsonIgnore]
        public Sprint LatestSprint
        {
            get
            {
                return Sprints
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefault();
            }
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", ID.ToSubcode(), Name);
        }
    }
}
