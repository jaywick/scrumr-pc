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
            if (!File.Exists(filename))
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
                    commandText.AppendLine(Schema.Create(item));
                }

                command.CommandText = commandText.ToString();
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        private static void PopulateSampleData(string filename)
        {
            var context = new ScrumrContext(filename);
            
            var project = new Project { Name = SampleProjectName };
            context.Projects.Add(project);

            context.Sprints.Add(new Sprint { Name = SampleSprintName, ProjectId = project.ID });
            context.Features.Add(new Feature { Name = SampleFeatureName, ProjectId = project.ID });

            context.SaveChanges();
        }
    }
}
