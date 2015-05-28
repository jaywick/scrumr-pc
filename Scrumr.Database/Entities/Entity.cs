using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrumr.Core.Extensions;

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

        public override string ToString()
        {
            return String.Format("{0}: {1}", ID.ToSubcode(), Name);
        }
    }
}
