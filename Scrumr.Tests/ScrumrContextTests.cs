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
    }
}
