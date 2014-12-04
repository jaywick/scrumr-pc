using NUnit.Framework;
using Scrumr.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Tests
{
    [TestFixture]
    class ScrumrContextTests
    {
        private DisposableTestFiles _testFiles;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _testFiles = new DisposableTestFiles();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _testFiles.Dispose();
        }

        [TestCase]
        public void ShouldAddNewProject()
        {
            using (var database = new DisposableTestDatabase(_testFiles))
            {
                var projectName = "Test Project";
                database.Context.AddNewProject(new Project(projectName));

                Assert.AreEqual(database.Context.Projects.Single().Name, projectName);
            }
        }

        [TestCase]
        public void ShouldCreateBacklogOnAddingNewProject()
        {
            using (var database = new DisposableTestDatabase(_testFiles))
            {
                database.Context.AddNewProject(new Project("Project X"));

                var projectAdded = database.Context.Projects.Single();
                var sprintAdded = database.Context.Sprints.Single();

                Assert.AreEqual(projectAdded.Backlog, sprintAdded);
            }
        }

        [TestCase]
        public void ShouldCreateDefaultFeatureOnAddingNewProject()
        {
            using (var database = new DisposableTestDatabase(_testFiles))
            {
                database.Context.AddNewProject(new Project("Project X"));

                var projectAdded = database.Context.Projects.Single();
                var featureAdded = database.Context.Features.Single();

                Assert.AreEqual(projectAdded.DefaultFeature, featureAdded);
            }
        }

        [TestCase]
        public void ShouldSetDefaultValueForNextTicketIdOnAddNewProject()
        {
            using (var database = new DisposableTestDatabase(_testFiles))
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
            using (var database = new DisposableTestDatabase(_testFiles))
            {
                var project = new Project("Project X");
                database.Context.AddNewProject(project);
                database.Context.AddNewTicket(new Ticket { Name = "Ticket X", Description = "", Sprint = project.Backlog, Feature = project.DefaultFeature, State = TicketState.Open, Type = TicketType.Task });

                var expected = 1;
                var actual = database.Context.Tickets.Single().ProjectTicketId;

                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase]
        public void ShouldIncrementNextTicketIdOnAddingNewTicket()
        {
            using (var database = new DisposableTestDatabase(_testFiles))
            {
                var project = new Project("Project X");
                database.Context.AddNewProject(project);
                database.Context.AddNewTicket(new Ticket { Name = "Ticket 1", Description = "", Sprint = project.Backlog, Feature = project.DefaultFeature, State = TicketState.Open, Type = TicketType.Task });
                database.Context.AddNewTicket(new Ticket { Name = "Ticket 2", Description = "", Sprint = project.Backlog, Feature = project.DefaultFeature, State = TicketState.Open, Type = TicketType.Task });

                var secondTicketAdded = database.Context.Tickets.ToList()[1];

                var expected = 2;
                var actual = secondTicketAdded.ProjectTicketId;

                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase]
        public void ShouldNotIncrementProjectTicketIdOnAddingTicketToAnotherProject()
        {
            using (var database = new DisposableTestDatabase(_testFiles))
            {
                var project = new Project("Project X");
                database.Context.AddNewProject(project);
                database.Context.AddNewTicket(new Ticket { Name = "Ticket X.1", Description = "", Sprint = project.Backlog, Feature = project.DefaultFeature, State = TicketState.Open, Type = TicketType.Task });
                database.Context.AddNewTicket(new Ticket { Name = "Ticket X.2", Description = "", Sprint = project.Backlog, Feature = project.DefaultFeature, State = TicketState.Open, Type = TicketType.Task });

                var project2 = new Project("Project Y");
                database.Context.AddNewProject(project2);
                database.Context.AddNewTicket(new Ticket { Name = "Ticket Y.1", Description = "", Sprint = project2.Backlog, Feature = project2.DefaultFeature, State = TicketState.Open, Type = TicketType.Task });

                var secondProjectTicket = database.Context.Tickets.Single(x => x.Name == "Ticket Y.1");

                var expected = 1;
                var actual = secondProjectTicket.ProjectTicketId;

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
