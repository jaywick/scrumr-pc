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
        public EditTicket(ScrumrContext context, long? projectId = null, long? sprintId = null, long? featureId = null)
            : base(typeof(Ticket), context)
        {
            var sprintView = GetDataListFromType<Sprint>();
            var featureView = GetDataListFromType<Feature>();

            SetSourceItems(projectId, sprintView, featureView);
            SetSelectedValues(sprintId, featureId, sprintView, featureView);
        }

        private DataListPropertyView GetDataListFromType<T>()
        {
            return PropertyViews.SingleOrDefault(x => x.Property.Type == typeof(T)) as DataListPropertyView;
        }

        private void SetSourceItems(long? projectId, DataListPropertyView sprint, DataListPropertyView feature)
        {
            if (projectId.HasValue)
            {
                sprint.Source = Context.Sprints.Where(x => x.ProjectId == projectId.Value);
                feature.Source = Context.Features.Where(x => x.ProjectId == projectId.Value);
            }
            else
            {
                sprint.Source = Context.Sprints;
                feature.Source = Context.Features;
            }
        }

        private void SetSelectedValues(long? sprintId, long? featureId, DataListPropertyView sprintView, DataListPropertyView featureView)
        {
            if (sprintId.HasValue)
                sprintView.Value = Context.Sprints.Get(sprintId.Value);

            if (featureId.HasValue)
                featureView.Value = Context.Features.Get(featureId.Value);
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
