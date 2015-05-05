using Newtonsoft.Json.Linq;
using Scrumr.Database.Migration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    public class FileSystem
    {
        private const string SampleSprintName = "Backlog";
        private const string SampleFeatureName = "General";
        private const string SampleProjectName = "Project A";

        public static async Task<ScrumrContext> LoadContext(string filename, int expectedSchemaVersion)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(String.Format("Cannot find database '{0}'", filename));

            var isSchemaValid = CheckSchema(filename);

            return await ScrumrContext.Load(filename, expectedSchemaVersion);
        }

        public static async Task CreateNew(string filename, int schemaVersion, bool initialise = true)
        {
            var context = await ScrumrContext.CreateBlank(filename, schemaVersion);
            
            if (initialise)
            {
                context.Meta = new Meta(schemaVersion);
                await context.AddNewProject(new Project(context) { Name = "Project 1" });
                await context.SaveChangesAsync();
            }
        }

        public static bool CheckSchema(string filename)
        {
            var rawData = System.IO.File.ReadAllText(filename);
            var jsonData = JObject.Parse(rawData);
            int currentSchema = 0;
            
            var result = int.TryParse(jsonData["Meta"]["SchemaVersion"].ToString(), out currentSchema);

            if (!result)
                throw new SchemaMismatchException(filename);

            if (currentSchema > Meta.CurrentSchemaVersion)
                throw new SchemaMismatchException(filename, Meta.CurrentSchemaVersion, currentSchema);

            if (currentSchema == Meta.CurrentSchemaVersion)
                return true;

            var migrator = new Migrator(fromVersion: currentSchema, toVersion: Meta.CurrentSchemaVersion);
            migrator.Upgrade(filename);

            return true;
        }

    }
}
