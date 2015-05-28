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
    class ConvertingTicketToSubfeature
    {
        private DisposableTestWorkspace _workspace;
        private ScrumrContext _context;
        private Ticket _ticket;

        [TestFixtureSetUp]
        public async void SetUp()
        {
            _workspace = new DisposableTestWorkspace();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _workspace.Dispose();
        }

        private async Task SetupContext()
        {
            _context = await ContextTestHelper.CreateTestDatabase(_workspace);

            var project = new Project("Project X", _context);
            await _context.AddNewProject(project);

            var feature = new Feature("Feature 1", project.Backlog);
            _context.Features.Insert(feature);

            _ticket = ContextTestHelper.CreateTestTicket("Ticket A", feature);
            await _context.AddNewTicket(_ticket);
        }

        [TestCase]
        public async void ShouldAddFeature()
        {
            await SetupContext();

            var totalFeaturesBefore = _context.Features.Count;
            await _context.ConvertTicketToSubfeature(_ticket);

            var expected = totalFeaturesBefore + 1;
            var actual = _context.Features.Count;

            Assert.AreEqual(expected, expected);
        }

        [TestCase]
        public async void ShouldCreateFeatureInSameProject()
        {
            await SetupContext();

            var newSubfeature = await _context.ConvertTicketToSubfeature(_ticket);

            var expected = _ticket.Feature.Sprint.ProjectId;
            var actual = newSubfeature.Sprint.ProjectId;

            Assert.AreEqual(expected, expected);
        }

        [TestCase]
        public async void ShouldRemoveTicket()
        {
            await SetupContext();

            await _context.ConvertTicketToSubfeature(_ticket);
            Assert.IsFalse(_context.Tickets.Any(x => x.ID == _ticket.ID));
        }
    }
}
