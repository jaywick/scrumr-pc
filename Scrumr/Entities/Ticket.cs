using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class Ticket : Entity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Feature Feature { get; set; }
        public Sprint Sprint { get; set; }
    }
}
