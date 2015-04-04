using Scrumr.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client
{
    class SprintTab
    {
        public static SprintTab AllSprints { get; set; }
        public Sprint Sprint { get; set; }

        static SprintTab()
        {
            AllSprints = new SprintTab(null);
        }

        public SprintTab(Sprint sprint)
        {
            Sprint = sprint;
        }

        public string Header
        {
            get { return Sprint.IfNotNull(x => x.Name) ?? "All"; }
        }
    }
}
