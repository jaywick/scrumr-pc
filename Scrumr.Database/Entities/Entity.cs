using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client.Database
{
    public class Entity
    {
        [Primary]
        public long ID { get; set; }

        public string Name { get; set; }
    }
}
