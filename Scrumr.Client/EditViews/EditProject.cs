using Scrumr.Client.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client
{
    class EditProject : EditView
    {
        public EditProject(ScrumrContext context, Entity entity = null)
            : base(typeof(Project), context, entity) { }

        protected override IEnumerable<Expression<Func<Entity, object>>> OnRendering()
        {
            yield return x => (x as Project).Name;
        }

        protected override void OnCreated(Entity entity)
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var project = entity as Project;
                    Context.Projects.Add(project);
                    Context.SaveChanges();

                    var feature = new Feature { Name = "General", Project = project };
                    var sprint = new Sprint { Name = "Backlog", Project = project };

                    Context.Features.Add(feature);
                    Context.Sprints.Add(sprint);

                    project.DefaultFeature = feature;
                    project.Backlog = sprint;
                    Context.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        protected override void OnUpdated(Entity entity)
        {
            Context.SaveChanges();
        }
    }
}
