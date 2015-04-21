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

        int? _projectId;
        int? _sprintId;
        int? _featureId;

        public EditTicket(ScrumrContext context, Ticket entity = null, PropertyBag properties = null)
            : base(context, entity)
        {
            _sprintId = properties.GetValue<int>("sprintId");
            _featureId = properties.GetValue<int>("featureId");

            _projectId = properties.GetValue<int>("projectId")
                ?? (entity as Ticket)
                    .IfNotNull(x => x.Sprint)
                    .IfNotNull(x => (int?)x.ProjectId);

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

        protected override async Task OnDeleting(Ticket ticket)
        {
            await Context.DeleteTicket(ticket);
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
            SprintView.Source = Context.Sprints.Where(x => x.ProjectId == _projectId.Value);
            FeatureView.Source = Context.Features.Where(x => x.ProjectId == _projectId.Value);
        }

        private void SetSelections()
        {
            if (_sprintId.HasValue)
                SprintView.Value = Context.Sprints[_sprintId.Value];
            else
                SprintView.Value = Context.Projects[_projectId.Value].Backlog;

            if (_featureId.HasValue)
                FeatureView.Value = Context.Features[_featureId.Value];
            else
                FeatureView.Value = Context.Projects[_projectId.Value].DefaultFeature;

            TypeView.Value = TicketType.Task;
            StateView.Value = TicketState.Open;
        }
    }
}
