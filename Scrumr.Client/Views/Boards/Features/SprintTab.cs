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
        public static SprintTab NewSprint { get; set; }

        public Sprint Sprint { get; set; }

        static SprintTab()
        {
            AllSprints = new SprintTab("All");
            NewSprint = new SprintTab("+");
        }

        public SprintTab(string header)
        {
            Header = header;
        }

        public SprintTab(Sprint sprint)
        {
            Sprint = sprint;
            Header = sprint.Name;
        }

        public string Header { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as SprintTab;

            if (other == null)
                return false;

            return this.GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            if (Sprint == null)
                return base.GetHashCode();

            return Sprint.ID.GetHashCode();
        }
    }
}
