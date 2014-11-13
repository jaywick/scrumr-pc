using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Scrumr;

namespace Scrumr
{
    class EditTicket : EditView
    {
        public EditTicket(ScrumrContext context, Int64? projectId = null, Int64? sprintId = null, Int64? featureId = null)
            : base(typeof(Ticket), context)
        {
            var sprint = PropertyViews.SingleOrDefault(x => x.Property.Name == "Sprint") as DataListPropertyView;
            var feature = PropertyViews.SingleOrDefault(x => x.Property.Name == "Feature") as DataListPropertyView;

            SetSourceItems(context, projectId, sprint, feature);
            SetSelectedValues(sprintId, featureId, sprint, feature);
        }

        private static void SetSourceItems(ScrumrContext context, Int64? projectId, DataListPropertyView sprint, DataListPropertyView feature)
        {
            if (projectId.HasValue)
            {
                sprint.Source = context.Sprints.Where(x => x.ProjectId == projectId.Value);
                feature.Source = context.Features.Where(x => x.ProjectId == projectId.Value);
            }
            else
            {
                sprint.Source = context.Sprints;
                feature.Source = context.Features;
            }
        }

        private void SetSelectedValues(Int64? sprintId, Int64? featureId, DataListPropertyView sprint, DataListPropertyView feature)
        {
            if (sprintId.HasValue)
                sprint.Value = Context.Sprints.Get(sprintId.Value);

            if (featureId.HasValue)
                feature.Value = Context.Features.Get(featureId.Value);
        }

        public EditTicket(ScrumrContext context, Entity entity)
            : base(typeof(Ticket), context, entity)
        {
            var projectId = (entity as Ticket).Sprint.ProjectId;
        }

        protected override void OnSave(Entity entity)
        {
            var ticket = entity as Ticket;
            var nextId = ticket.Sprint.Project.NextProjectTicketId++;
            ticket.ProjectTicketId = nextId;
        }
    }
}
