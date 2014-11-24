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
        DataListPropertyView SprintView;
        DataListPropertyView FeatureView;
        EnumPropertyView StateView;
        EnumPropertyView TypeView;

        long? _projectId;
        long? _sprintId;
        long? _featureId;

        protected override IEnumerable<Expression<Func<Entity, object>>> OnRendering()
        {
            yield return x => (x as Ticket).Name;
            yield return x => (x as Ticket).Description;
            yield return x => (x as Ticket).Type;
            yield return x => (x as Ticket).State;
            yield return x => (x as Ticket).Sprint;
            yield return x => (x as Ticket).Feature;
        }

        public EditTicket(ScrumrContext context, long? projectId = null, long? sprintId = null, long? featureId = null)
            : base(typeof(Ticket), context)
        {
            _projectId = projectId;
            _sprintId = sprintId;
            _featureId = featureId;

            LoadViews();
            LoadSources();
            SetSelections();
        }

        public EditTicket(ScrumrContext context, Entity entity)
            : base(typeof(Ticket), context, entity)
        {
            _projectId = (entity as Ticket).Sprint.ProjectId;

            LoadViews();
            LoadSources();
        }

        private void LoadViews()
        {
            SprintView = GetView<Sprint, DataListPropertyView>();
            FeatureView = GetView<Feature, DataListPropertyView>();
            StateView = GetView<TicketState, EnumPropertyView>();
            TypeView = GetView<TicketType, EnumPropertyView>();
        }

        private void LoadSources()
        {
            if (_projectId.HasValue)
            {
                SprintView.Source = Context.Sprints.Where(x => x.ProjectId == _projectId.Value);
                FeatureView.Source = Context.Features.Where(x => x.ProjectId == _projectId.Value);
            }
            else
            {
                SprintView.Source = Context.Sprints;
                FeatureView.Source = Context.Features;
            }
        }

        private void SetSelections()
        {
            if (_sprintId.HasValue)
                SprintView.Value = Context.Sprints.Get(_sprintId.Value);

            if (_featureId.HasValue)
                FeatureView.Value = Context.Features.Get(_featureId.Value);

            TypeView.Value = TicketType.Task;
            StateView.Value = TicketState.Open;
        }

        protected override void OnCreated(Entity entity)
        {
            var ticket = entity as Ticket;
            ticket.ProjectTicketId = ticket.Project.NextProjectTicketId++;
        }
    }
}
