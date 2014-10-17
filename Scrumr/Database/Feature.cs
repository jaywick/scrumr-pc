using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrumr
{
    public class Feature : Entity
    {
        public Int64 ID { get; set; }

        public string Name { get; set; }

        public Int64 ProjectId { get; set; }
    }
}
