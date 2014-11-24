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
        [IgnoreRender]
        public long NextProjectTicketId { get; set; }

        [IgnoreRender]
        public ICollection<Sprint> Sprints { get; set; }

        [IgnoreRender]
        public ICollection<Feature> Features { get; set; }

        [IgnoreRender]
        public long BacklogId { get; set; }

        [IgnoreRender]
        public long DefaultFeatureId { get; set; }

        public virtual Sprint Backlog { get; set; }

        public virtual Feature DefaultFeature { get; set; }
    }
}
