using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class Ticket : Entity
    {
        [Foreign(typeof(Feature))]
        public int FeatureId { get; set; }

        [Foreign(typeof(Sprint))]
        public int SprintId { get; set; }

        [Enumeration(typeof(TicketTypes))]
        public int TypeId { get; set; }
    }
}
