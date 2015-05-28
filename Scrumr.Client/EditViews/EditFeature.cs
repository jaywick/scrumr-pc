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
            var sprintId = properties.GetValue<Guid>("sprintId") ?? entity.SprintId;
            LoadSprintsList(sprintId);
        }

        protected override IEnumerable<Expression<Func<Feature, object>>> OnRender()
        {
            yield return x => x.Name;
            yield return x => x.Sprint;
        }

        protected override async Task OnCreated(Feature entity)
        {
            Context.Features.Insert(entity);
            await Context.SaveChangesAsync();
        }

        private void LoadSprintsList(Guid sprintId)
        {
            var sprintsView = GetView<Sprint, DataListPropertyView>();
            var parentProjectId = Context.Sprints[sprintId].ProjectId;
            sprintsView.Source = Context.Sprints.Where(x => x.ProjectId == parentProjectId);

            sprintsView.SelectItem(Context.Sprints[sprintId]);
        }

        protected override async Task OnDeleting(Feature feature)
        {
            await Context.DeleteFeature(feature);
        }
    }
}
