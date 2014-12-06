using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Scrumr.Client;
using Scrumr.Database;

namespace Scrumr.Client
{
    class EditTicket : EditEntityBase<Ticket>
    {
        DataListPropertyView SprintView;
        DataListPropertyView FeatureView;
        EnumPropertyView StateView;
        EnumPropertyView TypeView;

        long? _projectId;
        long? _sprintId;
        long? _featureId;

        public EditTicket(ScrumrContext context, Ticket entity = null, PropertyBag properties = null)
            : base(context, entity)
        {
            _projectId = properties.GetValue<long>("projectId") ?? (entity as Ticket).IfNotNull(x => x.Sprint).IfNotNull(x => x.ProjectId);
            _sprintId = properties.GetValue<long>("sprintId");
            _featureId = properties.GetValue<long>("featureId");

            LoadViews();
            LoadSources();
            SetSelections();
        }

        protected override IEnumerable<Expression<Func<Ticket, object>>> OnRender()
        {
            yield return x => x.Name;
            yield return x => x.Description;
            yield return x => x.Type;
            yield return x => x.State;
            yield return x => x.Sprint;
            yield return x => x.Feature;
        }

        protected override async Task OnCreated(Ticket ticket)
        {
            await Context.AddNewTicket(ticket);
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
    }
}
