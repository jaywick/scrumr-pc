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
            if (!File.Exists(filename))
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
                commandText.AppendLine("CREATE TABLE Projects (`ID` INTEGER PRIMARY KEY AUTOINCREMENT, `Name` TEXT, `NextProjectTicketId` INTEGER NOT NULL);");
                commandText.AppendLine("CREATE TABLE Sprints (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, ProjectId INTEGER REFERENCES Project (ID));");
                commandText.AppendLine("CREATE TABLE Features (`ID`	INTEGER PRIMARY KEY AUTOINCREMENT, `Name` TEXT, `ProjectId` INTEGER);");
                commandText.AppendLine("CREATE TABLE Tickets (`ID` INTEGER PRIMARY KEY AUTOINCREMENT, `Name` TEXT, `Description` TEXT, `FeatureId` INTEGER NOT NULL, `SprintId` INTEGER NOT NULL, `ProjectTicketId` INTEGER NOT NULL, `Type` BLOB NOT NULL, `State` BLOB NOT NULL)\n;");
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
