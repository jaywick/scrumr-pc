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
            Created = DateTime.UtcNow;
        }

        public Entity(ScrumrContext context)
            : base(context)
        {
        }

        public string Name { get; set; }

        public DateTime Created { get; set; }
    }
}
