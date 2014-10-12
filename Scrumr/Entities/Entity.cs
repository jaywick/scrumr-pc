using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scrumr.Entities;

namespace Scrumr
{
    public class Entity
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }
    }
}
