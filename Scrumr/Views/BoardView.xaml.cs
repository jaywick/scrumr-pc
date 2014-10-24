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

namespace Scrumr
{
    public partial class BoardView : UserControl
    {
        public ScrumrContext Context { get; set; }

        public Dictionary<Sprint, int> SprintToColumnMap;
        public Dictionary<Feature, int> FeatureToRowMap;

        public Func<Sprint, bool> SprintFilter { get; set; }
        public Func<Feature, bool> FeatureFilter { get; set; }
        public Func<Ticket, bool> TicketFilter { get; set; }

        private Project _project;
        private ContextMenu _addMenu;

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

            SprintToColumnMap = new Dictionary<Sprint, int>();
            FeatureToRowMap = new Dictionary<Feature, int>();

            Board.Children.Clear();

            CreateSprintColumns();
            CreateFeatureRows();
            CreateAddButton();
            CreateTicketCells();
        }

        private void CreateAddButton()
        {
            var addButton = new Button { Content = "+", Style = (Style)FindResource("SquareButtonStyle") };

            _addMenu = new ContextMenu();
            _addMenu.Items.Add(ViewHelper.CreateMenuItem("New Ticket", () => NewTicket()));
            _addMenu.Items.Add(new Separator());
            _addMenu.Items.Add(ViewHelper.CreateMenuItem("New Sprint", () => NewSprint()));
            _addMenu.Items.Add(ViewHelper.CreateMenuItem("New Feature", () => NewFeature()));
            _addMenu.Items.Add(ViewHelper.CreateMenuItem("New Project", () => NewProject()));

            addButton.Click += (s, e) => _addMenu.IsOpen = true;
            _addMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            _addMenu.PlacementTarget = addButton;

            AddToGrid(addButton, 0, 0);
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
            int i = 1;
            Board.RowDefinitions.Clear();
            Board.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            foreach (var feature in VisibleFeatures)
            {
                Board.RowDefinitions.Add(new RowDefinition());
                FeatureToRowMap.Add(feature, i);

                var headerView = new HeaderView(feature);
                headerView.RequestEdit += (h) => EditEntity(h as Feature);
                headerView.RequestRemove += (h) => RemoveEntity(h as Feature);

                AddToGrid(headerView, 0, i);

                i++;
            }
        }

        private void CreateSprintColumns()
        {
            int i = 1;
            Board.ColumnDefinitions.Clear();
            Board.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            foreach (var sprint in VisibleSprints)
            {
                Board.ColumnDefinitions.Add(new ColumnDefinition());
                SprintToColumnMap.Add(sprint, i);

                var headerView = new HeaderView(sprint);
                headerView.RequestEdit += (h) => EditEntity(h as Sprint);
                headerView.RequestRemove += (h) => RemoveEntity(h as Sprint);

                AddToGrid(headerView, i, 0);

                i++;
            }
        }

        private void AddToGrid(UIElement item, int column, int row)
        {
            Board.Children.Add(item);

            Grid.SetColumn(item, column);
            Grid.SetRow(item, row);
        }

        private void CreateTicketCell(int sprintId, int featureId)
        {
            var cellView = new CellView(sprintId, featureId);
            AddToGrid(cellView, sprintId, featureId);
            cellView.Drop += (s, e) => MoveTicket(e.Data.GetData(typeof(Ticket)) as Ticket, sprintId, featureId);
            cellView.RequestNewTicket += (s, f) => NewTicket(s, f);

            var tickets = VisibleTickets
                .Where(t => t.SprintId == sprintId)
                .Where(t => t.FeatureId == featureId);

            foreach (var ticket in tickets)
            {
                var ticketView = new TicketView(ticket);
                cellView.Items.Add(ticketView);

                ticketView.RequestEdit += (t) => EditEntity(t as Ticket);
                ticketView.RequestRemove += (t) => RemoveEntity(t as Ticket);
            }
        }

        private void EditEntity<T>(T entity) where T : Entity
        {
            ViewHelper.EditEntity(entity, Context);
            Update();
        }

        private void RemoveEntity<T>(T entity) where T : Entity
        {
            ViewHelper.RemoveEntity(entity, Context);
            Update();
        }

        private void NewTicket(int sprintId, int featureId)
        {
            ViewHelper.AddTicket(Context.Tickets, Context, sprintId, featureId);
            Update();
        }

        private void MoveTicket(Ticket ticket, int sprintId, int featureId)
        {
            ticket.SprintId = sprintId;
            ticket.FeatureId = featureId;

            Update();
        }

        private void NewSprint()
        {
            ViewHelper.AddEntity<Sprint>(Context.Sprints, Context);
            Update();
        }

        private void NewFeature()
        {
            ViewHelper.AddEntity<Feature>(Context.Features, Context);
            Update();
        }

        private void NewTicket()
        {
            ViewHelper.AddTicket(Context.Tickets, Context);
            Update();
        }

        private void NewProject()
        {
            ViewHelper.AddEntity<Project>(Context.Projects, Context);
            Update();
        }

    }
}
