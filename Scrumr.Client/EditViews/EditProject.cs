using Scrumr.Client.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client
{
    class EditProject : EditEntityBase<Project>
    {
        public EditProject(ScrumrContext context, Project entity = null)
            : base(context, entity) { }

        protected override IEnumerable<Expression<Func<Project, object>>> OnRender()
        {
            yield return x => x.Name;
        }

        protected override void OnCreated(Project project)
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
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

        protected override void OnUpdated(Project project)
        {
            Context.SaveChanges();
        }
    }
}
