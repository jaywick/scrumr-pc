using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrumr.Database;
using System.IO;
using Newtonsoft.Json;
using Scrumr.Core;
using Scrumr.Database.Migration;

namespace Scrumr.Database
{
    public class ScrumrContext
    {
        public Table<Project> Projects { get; set; }
        public Table<Ticket> Tickets { get; set; }
        public Table<Feature> Features { get; set; }
        public Table<Sprint> Sprints { get; set; }
        public Meta Meta { get; set; }

        private FileInfo DatabaseFile { get; set; }

        private int ExpectedSchemaVersion { get; set; }

        private ScrumrContext()
        {
            Meta = new Meta();
            Projects = new Table<Project>();
            Tickets = new Table<Ticket>();
            Features = new Table<Feature>();
            Sprints = new Table<Sprint>();
        }

        public static async Task<ScrumrContext> CreateBlank(string filename, int schemaVersion)
        {
            var instance = new ScrumrContext();
            instance.DatabaseFile = new FileInfo(filename);
            instance.ExpectedSchemaVersion = schemaVersion;

            await instance.SaveChangesAsync();

            return instance;
        }

        public static async Task<ScrumrContext> Load(string filename, int expectedSchemaVersion)
        {
            var instance = new ScrumrContext();
            instance.DatabaseFile = new FileInfo(filename);
            instance.ExpectedSchemaVersion = expectedSchemaVersion;

            await instance.LoadDatabaseAsync();

            return instance;
        }

        //todo: mange this with attributres in in entity classes
        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
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
        }*/


        public IEnumerable<Ticket> GetTickets(ScrumrContext context, Feature feature, Sprint sprint)
        {
            return Tickets
                .Where(x => x.FeatureId == feature.ID)
                .Where(x => x.SprintId == sprint.ID);
        }

        public async Task AddNewProject(Project project)
        {
            Projects.Insert(project);
            await SaveChangesAsync();

            var feature = new Feature("General", project);
            var sprint = new Sprint("Backlog", project);

            Features.Insert(feature);
            Sprints.Insert(sprint);

            project.DefaultFeatureId = feature.ID;
            project.BacklogId = sprint.ID;
            project.NextProjectTicketId = 1;
            await SaveChangesAsync();
        }

        public async Task<Ticket> AddNewTicket(Ticket ticket)
        {
            ticket.ProjectTicketId = ticket.Project.NextProjectTicketId++;

            Tickets.Insert(ticket);
            await SaveChangesAsync();

            return ticket;
        }

        public async Task DeleteProject(Project project)
        {
            var linkedTickets = Tickets.Where(x => x.Sprint.Project.ID == project.ID).ToList();
            Tickets.RemoveRange(linkedTickets);

            var linkedFeatures = Features.Where(x => x.ProjectId == project.ID);
            Features.RemoveRange(linkedFeatures);

            var linkedSprints = Sprints.Where(x => x.ProjectId == project.ID);
            Sprints.RemoveRange(linkedSprints);

            await SaveChangesAsync();

            Projects.Remove(project);

            await SaveChangesAsync();
        }

        public async Task DeleteFeature(Feature feature)
        {
            var linkedTickets = Tickets.Where(x => x.FeatureId == feature.ID);
            Tickets.RemoveRange(linkedTickets);

            var linkedProjects = Projects.Where(x => x.DefaultFeatureId == feature.ID);
            Projects.ToList().ForEach(x => x.DefaultFeatureId = null);

            Features.Remove(feature);

            await SaveChangesAsync();
        }

        public async Task DeleteTicket(Ticket ticket)
        {
            Tickets.Remove(ticket);
            await SaveChangesAsync();
        }

        public async Task DeleteSprint(Sprint sprint)
        {
            var linkedTickets = Tickets.Where(x => x.SprintId == sprint.ID);
            Tickets.RemoveRange(linkedTickets);

            var linkedProjects = Projects.Where(x => x.BacklogId == sprint.ID);
            Projects.ToList().ForEach(x => x.BacklogId = null);

            Sprints.Remove(sprint);

            await SaveChangesAsync();
        }

        public async Task<Feature> ConvertTicketToSubfeature(Ticket ticket)
        {
            var subfeatureName = ticket.Feature.Name + "/" + ticket.Name;
            var subfeature = new Feature(subfeatureName, ticket.Feature.Project);

            Features.Insert(subfeature);
            await DeleteTicket(ticket);

            await SaveChangesAsync();

            return subfeature;
        }

        private async Task LoadDatabaseAsync()
        {
            await Task.Factory.StartNew(() => LoadDatabase());
        }

        public async Task SaveChangesAsync()
        {
            await Task.Factory.StartNew(() => SaveChanges());
        }

        private void LoadDatabase()
        {
            using (var stream = File.OpenText(DatabaseFile.FullName))
            {
                var reader = new JsonTextReader(stream);
                var database = new JsonSerializer().Deserialize<DatabaseContainer>(reader);

                Meta = database.Meta;
                Projects.Load(this, database.Projects);
                Features.Load(this, database.Features);
                Sprints.Load(this, database.Sprints);
                Tickets.Load(this, database.Tickets);

                reader.Close();
            }
        }

        public void SaveChanges()
        {
            using (var stream = File.CreateText(DatabaseFile.FullName))
            {
                var database = new DatabaseContainer(this);
                var writer = new JsonTextWriter(stream);
                var serialiser = new JsonSerializer();

                serialiser.Formatting = Formatting.Indented;
                serialiser.Serialize(writer, database);
                writer.Close();
            }
        }
    }
}
