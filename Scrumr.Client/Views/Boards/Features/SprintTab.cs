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
