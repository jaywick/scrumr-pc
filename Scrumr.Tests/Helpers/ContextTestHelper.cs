using Scrumr.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Tests
{
    internal class ContextTestHelper
    {
        internal static Ticket CreateTestTicket(string name, Project project)
        {
            return new Ticket
            {
                Name = "Ticket X.1",
                Description = "",
                Sprint = project.Backlog,
                Feature = project.DefaultFeature,
                State = TicketState.Open,
                Type = TicketType.Task
            };
        }

        internal async static Task<ScrumrContext> CreateTestDatabase(DisposableTestWorkspace _testFiles)
        {
            var testFile = _testFiles.Create();

            await FileSystem.CreateNew(testFile, 0, false);
            return await ScrumrContext.Load(testFile, 0);
        }
    }
}
