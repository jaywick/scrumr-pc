using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrumr.Database;
using System.IO;
using Newtonsoft.Json;

namespace Scrumr.Database
{
    public class ScrumrContext
    {
        public Table<Project> Projects { get; set; }
        public Table<Ticket> Tickets { get; set; }
        public Table<Feature> Features { get; set; }
        public Table<Sprint> Sprints { get; set; }
        public Table<Meta> Meta { get; set; }

        private FileInfo DatabaseFile { get; set; }

        private int ExpectedSchemaVersion { get; set; }

        private ScrumrContext()
        {
            Projects = new Table<Project>();
            Tickets = new Table<Ticket>();
            Features = new Table<Feature>();
            Sprints = new Table<Sprint>();
            Meta = new Table<Meta>();
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

        public Meta SchemaInfo
        {
            get { return Meta.SingleOrDefault(); }
        }

        public void CheckSchema()
        {
            if (SchemaInfo == null)
                throw new SchemaMismatchException(DatabaseFile.FullName);

            if (SchemaInfo.SchemaVersion != ExpectedSchemaVersion)
                throw new SchemaMismatchException(DatabaseFile.FullName, ExpectedSchemaVersion, SchemaInfo.SchemaVersion);
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

        public async Task AddNewTicket(Ticket ticket)
        {
            ticket.ProjectTicketId = ticket.Project.NextProjectTicketId++;

            Tickets.Insert(ticket);
            await SaveChangesAsync();
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
            Projects.ToList().ForEach(x => x.DefaultFeatureId = 0);

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
            Projects.ToList().ForEach(x => x.BacklogId = 0);

            Sprints.Remove(sprint);

            await SaveChangesAsync();
        }

        private async Task LoadDatabaseAsync()
        {
            await Task.Run(() => LoadDatabase());
        }

        public async Task SaveChangesAsync()
        {
            await Task.Run(() => SaveChanges());
        }
        private void LoadDatabase()
        {
            using (var stream = File.OpenText(DatabaseFile.FullName))
            {
                var reader = new JsonTextReader(stream);
                var database = new JsonSerializer().Deserialize<DatabaseContainer>(reader);

                Projects.Load(database.Projects, this);
                Features.Load(database.Features, this);
                Sprints.Load(database.Sprints, this);
                Tickets.Load(database.Tickets, this);
                Meta.Load(database.Meta, this);

                reader.Close();
            }
        }

        public void SaveChanges()
        {
            using (var stream = File.CreateText(DatabaseFile.FullName))
            {
                var database = new DatabaseContainer();
                database.Projects.AddRange(Projects);
                database.Features.AddRange(Features);
                database.Sprints.AddRange(Sprints);
                database.Tickets.AddRange(Tickets);
                database.Meta.AddRange(Meta);

                var writer = new JsonTextWriter(stream);
                var serialiser = new JsonSerializer();
                serialiser.Formatting = Formatting.Indented;
                serialiser.Serialize(writer, database);
                writer.Close();
            }
        }
    }
}
