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
            var sprintView = GetDataListFromType<Sprint>() as DataListPropertyView;
            var featureView = GetDataListFromType<Feature>() as DataListPropertyView;
            var stateView = GetDataListFromType<TicketState>();
            var typeView = GetDataListFromType<TicketType>();

            SetSourceItems(projectId, sprintView, featureView);
            SetSelectedValues(sprintId, featureId, sprintView, featureView, stateView, typeView);
        }

        public EditTicket(ScrumrContext context, Entity entity)
            : base(typeof(Ticket), context, entity)
        {
            var projectId = (entity as Ticket).Sprint.ProjectId;

            var sprintView = GetDataListFromType<Sprint>() as DataListPropertyView;
            var featureView = GetDataListFromType<Feature>() as DataListPropertyView;
            SetSourceItems(projectId, sprintView, featureView);
        }

        private PropertyView GetDataListFromType<T>()
        {
            return PropertyViews.SingleOrDefault(x => x.Property.Type == typeof(T)) as PropertyView;
        }

        private void SetSourceItems(long? projectId, DataListPropertyView sprintView, DataListPropertyView featureView)
        {
            if (projectId.HasValue)
            {
                sprintView.Source = Context.Sprints.Where(x => x.ProjectId == projectId.Value);
                featureView.Source = Context.Features.Where(x => x.ProjectId == projectId.Value);
            }
            else
            {
                sprintView.Source = Context.Sprints;
                featureView.Source = Context.Features;
            }
        }

        private void SetSelectedValues(long? sprintId, long? featureId, PropertyView sprintView, PropertyView featureView, PropertyView stateView, PropertyView typeView)
        {
            if (sprintId.HasValue)
                sprintView.Value = Context.Sprints.Get(sprintId.Value);

            if (featureId.HasValue)
                featureView.Value = Context.Features.Get(featureId.Value);

            typeView.Value = TicketType.Task;
            stateView.Value = TicketState.Open;
        }

        protected override void OnSave(Entity entity, Modes mode)
        {
            if (mode != Modes.New)
                return;

            var ticket = entity as Ticket;
            ticket.ProjectTicketId = ticket.Sprint.Project.NextProjectTicketId++;
        }
    }
}
