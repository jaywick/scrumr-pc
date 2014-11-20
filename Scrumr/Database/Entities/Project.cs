using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class Project : Entity
    {
        [IgnoreRender]
        public long NextProjectTicketId { get; set; }

        [IgnoreRender]
        public long BacklogId { get; set; }

        [IgnoreRender]
        public long DefaultFeatureId { get; set; }

        [ForeignKey("BacklogId")]
        public Sprint Backlog { get; set; }

        [ForeignKey("DefaultFeatureId")]
        public Feature DefaultFeature { get; set; }
    }
}
