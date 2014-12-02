using Scrumr.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scrumr.Client
{
    public partial class BoardView : UserControl
    {
        public event System.Action<Project> OnProjectAdded;

        public ScrumrContext Context { get; set; }

        public Func<Sprint, bool> SprintFilter { get; set; }
        public Func<Feature, bool> FeatureFilter { get; set; }
        public Func<Ticket, bool> TicketFilter { get; set; }

        private Project _project;

        public IEnumerable<Sprint> VisibleSprints
        {
            get { return Context.Sprints.Where(SprintFilter); }
        }

        public IEnumerable<Feature> VisibleFeatures
        {
            get { return Context.Features.Where(FeatureFilter); }
        }

        public IEnumerable<Ticket> VisibleTickets
        {
            get { return Context.Tickets.Where(TicketFilter); }
        }

        public Project Project
        {
            get { return _project; }
            set
            {
                _project = value;
                Update();
            }
        }

        public BoardView()
        {
            InitializeComponent();
        }

        public void Update()
        {
            if (Project == null) throw new InvalidOperationException("Project is missing");
            if (Context == null) throw new InvalidOperationException("Context is missing");

            SprintFilter = x => x.ProjectId == Project.ID;
            FeatureFilter = x => x.ProjectId == Project.ID;
            TicketFilter = x => true;

            Board.Children.Clear();
            Horizontal.Children.Clear();
            Vertical.Children.Clear();

            CreateSprintColumns();
            CreateFeatureRows();
            CreateTicketCells();
        }

        private void CreateTicketCells()
        {
            for (int column = 0; column < VisibleSprints.Count(); column++)
            {
                for (int row = 0; row < VisibleFeatures.Count(); row++)
                {
                    CreateTicketCell(column + 1, row + 1);
                }
            }
        }

        private void CreateFeatureRows()
        {
            int i = 0;
            Board.RowDefinitions.Clear();
            Vertical.RowDefinitions.Clear();

            foreach (var feature in VisibleFeatures)
            {
                Board.RowDefinitions.Add(new RowDefinition());
                Vertical.RowDefinitions.Add(new RowDefinition());

                var headerView = new HeaderView(feature, Orientation.Vertical);
                headerView.RequestEdit += (h) => EditEntity(h as Feature);
                headerView.RequestRemove += (h) => RemoveEntity(h as Feature);

                Vertical.InsertAt(headerView, 0, i);

                i++;
            }
        }

        private void CreateSprintColumns()
        {
            int i = 0;
            Board.ColumnDefinitions.Clear();
            Horizontal.ColumnDefinitions.Clear();

            foreach (var sprint in VisibleSprints)
            {
                Board.ColumnDefinitions.Add(new ColumnDefinition());
                Horizontal.ColumnDefinitions.Add(new ColumnDefinition());

                var headerView = new HeaderView(sprint, Orientation.Horizontal);
                headerView.RequestEdit += (h) => EditEntity(h as Sprint);
                headerView.RequestRemove += (h) => RemoveEntity(h as Sprint);

                Horizontal.InsertAt(headerView, i, 0);

                i++;
            }
        }

        private void CreateTicketCell(int sprintId, int featureId)
        {
            var cellView = new CellView(sprintId, featureId);
            Board.InsertAt(cellView, sprintId - 1, featureId - 1);
            cellView.Drop += (s, e) => MoveTicket(e.Data.GetData(typeof(Ticket)) as Ticket, sprintId, featureId);
            cellView.RequestNewTicket += (s, f) => NewTicket(s, f);

            var tickets = VisibleTickets
                .Where(t => t.SprintId == sprintId)
                .Where(t => t.FeatureId == featureId);

            foreach (var ticket in tickets)
            {
                var ticketView = new TicketView(ticket);
                cellView.Add(ticketView);

                ticketView.RequestEdit += (t) => EditEntity(t as Ticket);
                ticketView.RequestRemove += (t) => RemoveEntity(t as Ticket);
            }
        }

        private void EditEntity<T>(T entity) where T : Entity
        {
            ViewDirector.EditEntity(entity, Context);
            Update();
        }

        private void RemoveEntity<T>(T entity) where T : Entity
        {
            ViewDirector.RemoveEntity(entity, Context);
            Update();
        }

        public void NewTicket(int sprintId, int featureId)
        {
            ViewDirector.AddTicket(Context, Project.ID, sprintId, featureId);
            Update();
        }

        private void MoveTicket(Ticket ticket, int sprintId, int featureId)
        {
            ticket.SprintId = sprintId;
            ticket.FeatureId = featureId;

            Update();
        }

        public void NewSprint()
        {
            ViewDirector.AddEntity<Sprint>(Context, Project);
            Update();
        }

        public void NewFeature()
        {
            ViewDirector.AddEntity<Feature>(Context, Project);
            Update();
        }

        public void NewTicket()
        {
            ViewDirector.AddTicket(Context);
            Update();
        }

        public void NewProject()
        {
            var project = ViewDirector.AddEntity<Project>(Context);
            Update();

            if (OnProjectAdded != null)
                OnProjectAdded.Invoke(project);
        }

    }
}
