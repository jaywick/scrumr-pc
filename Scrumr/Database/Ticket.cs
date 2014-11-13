using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class Ticket : Entity
    {
        [LongAnswer]
        [RenderOrder(2)]
        public string Description { get; set; }

        [IgnoreRender]
        public long FeatureId { get; set; }

        [IgnoreRender]
        public long SprintId { get; set; }

        [IgnoreRender]
        public long TypeId { get; set; }

        [IgnoreRender]
        public long StateId { get; set; }

        [IgnoreRender]
        public long ProjectTicketId { get; set; }

        [ForeignKey("FeatureId")]
        public Feature Feature { get; set; }

        [ForeignKey("SprintId")]
        public Sprint Sprint { get; set; }

        [ForeignKey("TypeId")]
        [RenderOrder(3)]
        public TicketType Type { get; set; }

        [ForeignKey("StateId")]
        [RenderOrder(4)]
        public TicketState State { get; set; }
    }
}
