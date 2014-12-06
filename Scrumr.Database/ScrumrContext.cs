using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SQLite;
using Scrumr.Database;

namespace Scrumr.Database
{
    public class ScrumrContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Sprint> Sprints { get; set; }

        public ScrumrContext(string filename)
            : base(new SQLiteConnection()
                {
                    ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = filename, ForeignKeys = true }.ConnectionString
                }, true) { }

        public async Task LoadAllAsync()
        {
            await Projects.LoadAsync();
            await Features.LoadAsync();
            await Sprints.LoadAsync();
            await Tickets.LoadAsync();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sprint>()
                .HasKey(s => s.ID)
                .HasRequired(s => s.Project)
                .WithMany(p => p.Sprints)
                .HasForeignKey(s => s.ProjectId);

            modelBuilder.Entity<Project>()
                .HasKey(p => p.ID)
                .HasOptional(p => p.Backlog);

            modelBuilder.Entity<Feature>()
                .HasKey(s => s.ID)
                .HasRequired(s => s.Project)
                .WithMany(p => p.Features)
                .HasForeignKey(s => s.ProjectId);

            modelBuilder.Entity<Project>()
                .HasKey(p => p.ID)
                .HasOptional(p => p.DefaultFeature);
        }

        public void AddNewProject(Project project)
        {
            using (var transaction = Database.BeginTransaction())
            {
                try
                {
                    Projects.Add(project);
                    SaveChanges();

                    var feature = new Feature { Name = "General", Project = project };
                    var sprint = new Sprint { Name = "Backlog", Project = project };

                    Features.Add(feature);
                    Sprints.Add(sprint);

                    project.DefaultFeature = feature;
                    project.Backlog = sprint;
                    SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void AddNewTicket(Ticket ticket)
        {
            ticket.ProjectTicketId = ticket.Project.NextProjectTicketId++;

            Tickets.Add(ticket);
            SaveChanges();
        }
    }
}
