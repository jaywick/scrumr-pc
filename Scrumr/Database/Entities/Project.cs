using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    [Table("Projects")]
    public class Project : Entity
    {
        public long NextProjectTicketId { get; set; }

        public ICollection<Sprint> Sprints { get; set; }

        public ICollection<Feature> Features { get; set; }

        public long BacklogId { get; set; }

        public long DefaultFeatureId { get; set; }

        public virtual Sprint Backlog { get; set; }

        public virtual Feature DefaultFeature { get; set; }
    }
}
