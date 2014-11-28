using Scrumr.Client.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client
{
    class EditFeature : EditEntityBase<Feature>
    {
        public EditFeature(ScrumrContext context, Feature entity = null, Project project = null)
            : base(context, entity)
        {
            LoadProjectsList(project);
        }

        protected override IEnumerable<Expression<Func<Feature, object>>> OnRender()
        {
            yield return x => x.Name;
            yield return x => x.Project;
        }

        protected override void OnCreated(Feature entity)
        {
            Context.Features.Add(entity);
            Context.SaveChanges();
        }

        protected override void OnUpdated(Feature entity)
        {
            Context.SaveChanges();
        }

        private void LoadProjectsList(Project project)
        {
            var projectsView = GetView<Project, DataListPropertyView>();
            projectsView.Source = Context.Projects;

            if (project != null)
                projectsView.SelectItem(project);
        }
    }
}
