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

        [IgnoreRender, RefersTo("Sprint", "ID")]
        public long? BacklogId { get; set; }

        [IgnoreRender, RefersTo("Feature", "ID")]
        public long? DefaultFeatureId { get; set; }

        [ForeignKey("ID")]
        public Sprint Backlog { get; set; }

        [ForeignKey("ID")]
        public Feature DefaultFeature { get; set; }
    }
}
