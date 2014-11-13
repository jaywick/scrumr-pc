using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class Entity
    {
        [Key]
        public long ID { get; set; }

        [RenderOrder(1)]
        public string Name { get; set; }
    }
}
