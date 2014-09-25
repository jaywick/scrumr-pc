using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class Ticket : Entity
    {
        public int FeatureId { get; set; }
        public int SprintId { get; set; }
    }
}
