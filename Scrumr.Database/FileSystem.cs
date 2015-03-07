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

            return await ScrumrContext.Load(filename, expectedSchemaVersion);
        }

        public static async Task CreateNew(string filename, int schemaVersion, bool initialise = true)
        {
            var context = await ScrumrContext.CreateBlank(filename, schemaVersion);
            
            if (initialise)
            {
                context.Meta.Insert(new Meta(context) { SchemaVersion = schemaVersion });
                await context.AddNewProject(new Project(context) { Name = "Project 1" });
                await context.SaveChangesAsync();
            }
        }
    }
}
