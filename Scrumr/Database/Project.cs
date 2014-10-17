using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class Project : Entity
    {
        public Int64 ID { get; set; }

        public string Name { get; set; }

        public Int64 NextProjectTicketId { get; set; }
    }
}
