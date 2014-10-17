using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class Ticket : Entity
    {
        public Int64 ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Int64 FeatureId { get; set; }

        public Int64 SprintId { get; set; }

        public Int64 TypeId { get; set; }

        public Int64 ProjectTicketId { get; set; }
    }
}
