using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Scrumr
{
    public class ScrumrContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
    }

    public class Entity { }
}
