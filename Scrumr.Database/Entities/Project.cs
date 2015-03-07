using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    public class Project : Entity
    {
        public Project() { }

        public Project(string name)
        {
            Name = name;
        }

        public int NextProjectTicketId { get; set; }

        [JsonIgnore]
        public ICollection<Sprint> Sprints { get; set; }

        [JsonIgnore]
        public ICollection<Feature> Features { get; set; }

        [Foreign]
        public int? BacklogId { get; set; }

        [Foreign]
        public int? DefaultFeatureId { get; set; }

        [JsonIgnore]
        public virtual Sprint Backlog { get; set; }

        [JsonIgnore]
        public virtual Feature DefaultFeature { get; set; }

        public IEnumerable<Ticket> GetTickets(ScrumrContext context, Feature feature, Sprint sprint)
        {
            return context.Tickets
                .Where(x => x.FeatureId == feature.ID)
                .Where(x => x.SprintId == sprint.ID);
        }
    }
}
