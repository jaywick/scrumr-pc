using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    [Table("Projects")]
    public class Project : Entity
    {
        public Project() { }

        public Project(string name)
        {
            Name = name;
        }

        public long NextProjectTicketId { get; set; }

        public ICollection<Sprint> Sprints { get; set; }

        public ICollection<Feature> Features { get; set; }

        [Foreign]
        public long? BacklogId { get; set; }

        [Foreign]
        public long? DefaultFeatureId { get; set; }

        public virtual Sprint Backlog { get; set; }

        public virtual Feature DefaultFeature { get; set; }

        public IEnumerable<Ticket> GetTickets(ScrumrContext context, Feature feature, Sprint sprint)
        {
            return context.Tickets
                .Where(x => x.FeatureId == feature.ID)
                .Where(x => x.SprintId == sprint.ID);
        }
    }
}
