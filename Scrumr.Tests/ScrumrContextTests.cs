using NUnit.Framework;
using Scrumr.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Tests
{
    [TestFixture, Category("Context")]
    class ScrumrContextTests
    {
        private DisposableTestWorkspace _workspace;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _workspace = new DisposableTestWorkspace();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _workspace.Dispose();
        }

        [TestCase]
        public void ShouldAddNewProject()
        {
            using (var database = new DisposableTestDatabase(_workspace))
            {
                var project = new Project("Project X");
                database.Context.AddNewProject(project);

                var expected = project;
                var actual = database.Context.Projects.Single();

                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase]
        public void ShouldCreateBacklogOnAddingNewProject()
        {
            using (var database = new DisposableTestDatabase(_workspace))
            {
                database.Context.AddNewProject(new Project("Project X"));

                var projectAdded = database.Context.Projects.Single();
                var sprintAdded = database.Context.Sprints.Single();

                var expected = sprintAdded;
                var actual = projectAdded.Backlog;

                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase]
        public void ShouldCreateDefaultFeatureOnAddingNewProject()
        {
            using (var database = new DisposableTestDatabase(_workspace))
            {
                database.Context.AddNewProject(new Project("Project X"));

                var projectAdded = database.Context.Projects.Single();
                var featureAdded = database.Context.Features.Single();

                var expected = featureAdded;
                var actual = projectAdded.DefaultFeature;
                
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase]
        public void ShouldSetDefaultValueForNextTicketIdOnAddNewProject()
        {
            using (var database = new DisposableTestDatabase(_workspace))
            {
                database.Context.AddNewProject(new Project("Project X"));

                var expected = 1;
                var actual = database.Context.Projects.First().NextProjectTicketId;

                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase]
        public void ShouldReturnFirstProjectTicketIdOnAddingNewTicket()
        {
            using (var database = new DisposableTestDatabase(_workspace))
            {
                var project = new Project("Project X");
                database.Context.AddNewProject(project);
                database.Context.AddNewTicket(ContextTestHelper.CreateTestTicket("Ticket X", project));

                var expected = 1;
                var actual = database.Context.Tickets.Single().ProjectTicketId;

                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase]
        public void ShouldIncrementNextTicketIdOnAddingNewTicket()
        {
            using (var database = new DisposableTestDatabase(_workspace))
            {
                var project = new Project("Project X");
                database.Context.AddNewProject(project);
                database.Context.AddNewTicket(ContextTestHelper.CreateTestTicket("Ticket 1", project));
                database.Context.AddNewTicket(ContextTestHelper.CreateTestTicket("Ticket 2", project)); ;

                var secondTicketAdded = database.Context.Tickets.ToList()[1];

                var expected = 2;
                var actual = secondTicketAdded.ProjectTicketId;

                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase]
        public void ShouldNotIncrementProjectTicketIdOnAddingTicketToAnotherProject()
        {
            using (var database = new DisposableTestDatabase(_workspace))
            {
                var project = new Project("Project X");
                database.Context.AddNewProject(project);
                database.Context.AddNewTicket(ContextTestHelper.CreateTestTicket("Ticket X.1", project));
                database.Context.AddNewTicket(ContextTestHelper.CreateTestTicket("Ticket X.2", project));

                var project2 = new Project("Project Y");
                database.Context.AddNewProject(project2);
                database.Context.AddNewTicket(ContextTestHelper.CreateTestTicket("Ticket Y.1", project2));

                var secondProjectTicket = database.Context.Tickets.ToList()[2];

                var expected = 1;
                var actual = secondProjectTicket.ProjectTicketId;

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
