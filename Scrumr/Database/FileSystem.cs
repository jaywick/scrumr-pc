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
        public const string DefaultDatabase = "scrumr.sqlite";

        public static ScrumrContext LoadContext(string filename = DefaultDatabase)
        {
            //if (!File.Exists(filename))
                Create(filename);

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
                    commandText.AppendLine(Schema.Create(item));

                commandText.AppendLine("INSERT INTO Projects (`ID`, `Name`, `NextProjectTicketId`) VALUES (1, 'Project A', 0);");
                commandText.AppendLine("INSERT INTO Sprints (`Name`, `ProjectId`) VALUES ('Backlog', 1);");
                commandText.AppendLine("INSERT INTO Features (`Name`, `ProjectId`) VALUES ('General', 1);");
                
                command.CommandText = commandText.ToString();
                command.ExecuteNonQuery();
                
                connection.Close();
            }
        }
    }
}
