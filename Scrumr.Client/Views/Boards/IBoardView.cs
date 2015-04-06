using Scrumr.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client
{
    public interface IBoardView
    {
        void Update(Entity entity = null);

        bool ShowClosedTickets { get; set; }

        ScrumrContext Context { get; set; }
        Project Project { get; set; }
    }
}
