using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Scrumr
{
    public class Context : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Sprint> Sprints { get; set; }

        public List<Entity> GetCollection(Type type)
        {
            if (type == typeof(Project))
                return Projects.ToList<Entity>();

            if (type == typeof(Ticket))
                return Tickets.ToList<Entity>();

            if (type == typeof(Feature))
                return Features.ToList<Entity>();

            if (type == typeof(Sprint))
                return Sprints.ToList<Entity>();

            throw new ArgumentException(string.Format("Collection of '{0}' does not exist in Context", type.Name));
        }

        public async Task LoadAllAsync()
        {
            await Task.Run(() => LoadAll());
        }

        public void LoadAll()
        {
            Projects.Load();
            Tickets.Load();
            Features.Load();
            Sprints.Load();
        }
    }
}
