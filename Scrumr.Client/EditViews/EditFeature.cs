using Scrumr.Database;
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
        public EditFeature(ScrumrContext context, Feature entity = null, PropertyBag properties = null)
            : base(context, entity)
        {
            var projectId = properties.GetValue<long>("projectId");
            LoadProjectsList(projectId);
        }

        protected override IEnumerable<Expression<Func<Feature, object>>> OnRender()
        {
            yield return x => x.Name;
            yield return x => x.Project;
        }

        protected override async Task OnCreated(Feature entity)
        {
            Context.Features.Add(entity);
            await Context.SaveChangesAsync();
        }

        private void LoadProjectsList(long? projectId)
        {
            var projectsView = GetView<Project, DataListPropertyView>();
            projectsView.Source = Context.Projects;

            if (projectId.HasValue)
                projectsView.SelectItem(Context.Projects.Get(projectId.Value));
        }

        protected override async Task OnDeleting(Feature feature)
        {
            await Context.DeleteFeature(feature);
        }
    }
}
