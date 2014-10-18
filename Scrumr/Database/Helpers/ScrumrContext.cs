﻿using System;
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

            if (type == typeof(TicketType))
                return TicketTypes.ToList<Entity>();

            throw new ArgumentException(string.Format("Collection of '{0}' does not exist in Context", type.Name));
        }
    }
}
