using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    class EditTicket : EditView
    {
        public EditTicket(ScrumrContext context, Int64? projectId = null, Int64? sprintId = null, Int64? featureId = null)
            : base(typeof(Ticket), context)
        {
            /*var initialValues = new Dictionary<Expression<Func<Ticket, object>>, object>();

            if (sprintId.HasValue && featureId.HasValue)
            {
                initialValues.Add(t => t.Sprint, context.Sprints.Single(x => x.ID == sprintId.Value));
                initialValues.Add(t => t.Feature, context.Features.Single(x => x.ID == featureId.Value));
            }

            var onSave = new Action<Entity>(x =>
            {
                var ticket = x as Ticket;
                var nextId = ticket.Sprint.Project.NextProjectTicketId++;
                ticket.ProjectTicketId = nextId;
            });

            AddEntityBase<T>(table, context, new PropertiesView(typeof(T), context, initialValues, onSave));*/
        }

        public EditTicket(ScrumrContext context, Entity entity = null)
            : base(typeof(Ticket), context, entity)
        {

        }
    }
}
