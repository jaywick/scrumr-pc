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
        public async Task ShouldAddNewProject()
        {
            var context = await ContextTestHelper.CreateTestDatabase(_workspace);
            var project = new Project("Project X", context);
            await context.AddNewProject(project);

            var expected = project;
            var actual = context.Projects.Single();

            Assert.AreEqual(expected, actual);
        }

        [TestCase]
        public async Task ShouldDeleteProject()
        {
            var context = await ContextTestHelper.CreateTestDatabase(_workspace);
            var project = new Project("Project X", context);
            await context.AddNewProject(project);
            await context.DeleteProject(project);

            var exists = context.Projects.Any(x => x.ID == project.ID);

            Assert.IsFalse(exists);
        }

        [TestCase]
        public async Task ShouldDeleteProjectAndFeatures()
        {
            var context = await ContextTestHelper.CreateTestDatabase(_workspace);
            var project = new Project("Project X", context);
            await context.AddNewProject(project);

            var features = new List<Feature>()
            {
                new Feature("Feature 1", project.Backlog),
                new Feature("Feature 2", project.Backlog),
                new Feature("Feature 3", project.Backlog),
            };

            context.Features.InsertRange(features);
            await context.SaveChangesAsync();

            await context.DeleteProject(project);

            var exists = context.Features.Any(x => x.Sprint.ProjectId == project.ID);

            Assert.IsFalse(exists);
        }

        [TestCase]
        public async Task ShouldDeleteProjectAndSprints()
        {
            var context = await ContextTestHelper.CreateTestDatabase(_workspace);
            var project = new Project("Project X", context);
            await context.AddNewProject(project);

            var sprints = new List<Sprint>()
            {
                new Sprint("Sprint 1", project),
                new Sprint("Sprint 2", project),
                new Sprint("Sprint 3", project),
            };

            context.Sprints.InsertRange(sprints);
            await context.SaveChangesAsync();

            await context.DeleteProject(project);

            var exists = context.Sprints.Any(x => x.ProjectId == project.ID);

            Assert.IsFalse(exists);
        }

        [TestCase]
        public async Task ShouldDeleteProjectAndTickets()
        {
            var context = await ContextTestHelper.CreateTestDatabase(_workspace);
            var project = new Project("Project X", context);
            await context.AddNewProject(project);

            var feature = new Feature(context);
            feature.Sprint = project.Backlog;

            var tickets = new List<Ticket>()
            {
                ContextTestHelper.CreateTestTicket("Ticket 1", feature),
                ContextTestHelper.CreateTestTicket("Ticket 2", feature),
                ContextTestHelper.CreateTestTicket("Ticket 3", feature),
            };

            await context.SaveChangesAsync();

            await context.DeleteProject(project);

            var exists = context.Tickets.Any();

            Assert.IsFalse(exists);
        }

        [TestCase]
        public async Task ShouldCreateBacklogOnAddingNewProject()
        {
            var context = await ContextTestHelper.CreateTestDatabase(_workspace);
            await context.AddNewProject(new Project("Project X", context));

            var projectAdded = context.Projects.Single();
            var sprintAdded = context.Sprints.Single();

            var expected = sprintAdded;
            var actual = projectAdded.Backlog;

            Assert.AreEqual(expected, actual);
        }

        [TestCase]
        public async Task ShouldSetDefaultValueForNextTicketIdOnAddNewProject()
        {
            var context = await ContextTestHelper.CreateTestDatabase(_workspace);
            await context.AddNewProject(new Project("Project X", context));

            var expected = 1;
            var actual = context.Projects.First().NextProjectTicketId;

            Assert.AreEqual(expected, actual);
        }
    }
}
