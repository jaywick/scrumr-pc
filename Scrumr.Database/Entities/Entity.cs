using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    public class Entity : Identifiable
    {
        public Entity()
            : base(null)
        {
        }

        public Entity(ScrumrContext context)
            : base(context)
        {
        }

        public string Name { get; set; }
    }
}
