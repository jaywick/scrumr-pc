using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class Context
    {
        public List<Sprint> Sprints { get; set; }
        public List<Feature> Features { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<Project> Projects { get; set; }

        public Context()
        {
            Sprints = new List<Sprint>();
            Features = new List<Feature>();
            Tickets = new List<Ticket>();
        }

        public IEnumerable<Entity> GetCollection(Type type)
        {
            if (type == typeof(Sprint))
                return Sprints;
            if (type == typeof(Feature))
                return Features;
            if (type == typeof(Ticket))
                return Tickets;
            if (type == typeof(Project))
                return Projects;

            return null;
        }

        public int GetNextId(Type type)
        {
            var lastId = GetCollection(type).Max(x => x.ID);
            return lastId + 1;
        }

        public int GetNextTicketId(int projectId)
        {
            var projectBasedIds = from t in Tickets
                                  join s in Sprints on t.SprintId equals s.ID
                                  join p in Projects on s.ProjectId equals projectId
                                  select t.ProjectBasedId;
            
            var lastId = projectBasedIds.Max();

            return lastId + 1;
        }
    }
}
