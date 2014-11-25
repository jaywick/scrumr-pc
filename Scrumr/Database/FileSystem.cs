using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    class FileSystem
    {
        private const string DefaultDatabase = "scrumr.sqlite";
        private const string SampleSprintName = "Backlog";
        private const string SampleFeatureName = "General";
        private const string SampleProjectName = "Project A";

        public static ScrumrContext LoadContext(string filename = DefaultDatabase)
        {
            if (!File.Exists(filename) || App.Overwrite)
            {
                Create(filename);
                PopulateSampleData(filename);
            }

            return new ScrumrContext(filename);
        }

        public static void Create(string filename)
        {
            var entities = typeof(ScrumrContext).GetProperties()
                .Where(x => x.PropertyType.IsGenericType)
                .Where(x => x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(x => x.PropertyType.GetGenericArguments().First());

            SQLiteConnection.CreateFile(filename);

            using (var connection = new SQLiteConnection("Data Source=" + filename))
            {
                connection.Open();

                var command = connection.CreateCommand();
                var commandText = new StringBuilder();

                foreach (var entity in entities)
                {
                    commandText.AppendLine(SqlGenerator.GenerateCreateScriptFor(entity));
                }

                command.CommandText = commandText.ToString();
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        private static void PopulateSampleData(string filename)
        {
            var context = new ScrumrContext(filename);

            var project = AddProject(context);
            context.SaveChanges();

            AddBacklog(context, project);
            AddDefaultFeature(context, project);
            context.SaveChanges();
        }

        private static void AddDefaultFeature(ScrumrContext context, Project project)
        {
            var backlogSprint = new Sprint { Name = SampleSprintName, Project = project };
            project.Backlog = backlogSprint;
            context.Sprints.Add(backlogSprint);
        }

        private static void AddBacklog(ScrumrContext context, Project project)
        {
            var defaultFeature = new Feature { Name = SampleFeatureName, Project = project };
            project.DefaultFeature = defaultFeature;
            context.Features.Add(defaultFeature);
        }

        private static Project AddProject(ScrumrContext context)
        {
            var project = new Project { Name = SampleProjectName };
            context.Projects.Add(project);
            return project;
        }

    }
}
