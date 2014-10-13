using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrumr
{
    public class Sprint : Entity
    {
        [Foreign(typeof(Project))]
        public int ProjectId { get; set; }
    }
}
