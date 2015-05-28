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
        internal static Ticket CreateTestTicket(string name, Feature feature)
        {
            return new Ticket(feature.Context)
            {
                Name = "Ticket X.1",
                Description = "",
                Feature = feature,
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
