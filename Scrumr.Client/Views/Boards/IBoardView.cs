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
        event System.Action<Project> OnProjectAdded;

        void NewTicket();
        void NewFeature();
        void NewSprint();
        void NewProject();
        void Update();

        ScrumrContext Context { get; set; }
        Project Project { get; set; }
    }
}
