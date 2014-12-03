using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    public class FileSystem
    {
        private const string DefaultDatabase = "scrumr.sqlite";
        private const string SampleSprintName = "Backlog";
        private const string SampleFeatureName = "General";
        private const string SampleProjectName = "Project A";

        public static ScrumrContext LoadContext(string filename = DefaultDatabase)
        {
            if (!File.Exists(filename) || Overwrite)
            {
                Create(filename);
                PopulateSampleData(filename);
            }

            return new ScrumrContext(filename);
        }

        public static bool Overwrite
        {
            get
            {
#if OVERWRITE
                return true;
#else
                return false;
#endif
            }
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
            context.AddNewProject(new Project { Name = "Project 1" });
        }
    }
}
