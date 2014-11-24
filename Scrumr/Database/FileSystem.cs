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

                foreach (var item in entities)
                {
                    commandText.AppendLine(SqlGenerator.GenerateCreateScriptFor(item));
                }

                command.CommandText = commandText.ToString();
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        private static void PopulateSampleData(string filename)
        {
            var context = new ScrumrContext(filename);
            
            var project = new Project { ID = 1, Name = SampleProjectName };

            var backlogSprint = new Sprint { ID = 1, Name = SampleSprintName, ProjectId = 1 };
            var defaultFeature = new Feature { ID = 1, Name = SampleFeatureName, ProjectId = 1 };

            project.BacklogId = backlogSprint.ID;
            //project.DefaultFeatureId = defaultFeature.ID;

            context.Sprints.Add(backlogSprint);
            context.Features.Add(defaultFeature);
            context.Projects.Add(project);

            context.SaveChanges();
        }
    }
}
