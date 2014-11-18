using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    class RenderOrderAttribute : System.Attribute
    {
        public int Order { get; set; }
        
        public RenderOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
