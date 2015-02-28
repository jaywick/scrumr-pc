using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client
{
    public interface IBoardView
    {
        void NewTicket();
        void NewFeature();
        void NewSprint();
        void NewProject();
        void Update();

        Database.ScrumrContext Context { get; set; }
        Database.Project Project { get; set; }
    }
}
