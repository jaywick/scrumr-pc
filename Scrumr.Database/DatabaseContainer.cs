using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    public class DatabaseContainer
    {
        public List<Project> Projects { get; set; }
        public List<Feature> Features { get; set; }
        public List<Sprint> Sprints { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<Meta> Meta { get; set; }

        public DatabaseContainer()
        {
            Projects = new List<Project>();
            Features = new List<Feature>();
            Sprints = new List<Sprint>();
            Tickets = new List<Ticket>();
            Meta = new List<Meta>();
        }
    }
}
