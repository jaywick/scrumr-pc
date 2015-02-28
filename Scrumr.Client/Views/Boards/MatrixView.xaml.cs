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
    public partial class MatrixView : UserControl, IBoardView
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

        public MatrixView()
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
            TopHeader.Children.Clear();
            LeftHeader.Children.Clear();

            CreateSprintColumns();
            CreateFeatureRows();
            CreateTicketCells();

            SyncHeaderScrollers();
        }

        private void CreateTicketCells()
        {
            int column = 0;
            int row = 0;

            foreach (var sprint in VisibleSprints)
            {
                foreach (var feature in VisibleFeatures)
                {
                    CreateTicketCell(column, row, sprint.ID, feature.ID);
                    row++;
                }

                column++;
                row = 0;
            }
        }

        private void CreateFeatureRows()
        {
            int i = 0;
            Board.RowDefinitions.Clear();
            LeftHeader.RowDefinitions.Clear();

            foreach (var feature in VisibleFeatures)
            {
                Board.RowDefinitions.Add(new RowDefinition { Height = new GridLength(300) });
                LeftHeader.RowDefinitions.Add(new RowDefinition { Height = new GridLength(300) });

                var headerView = new HeaderView(feature, Orientation.Vertical);
                headerView.RequestEdit += (h) => EditEntity(h as Feature);
                headerView.RequestRemove += (h) => RemoveEntity(h as Feature);

                LeftHeader.InsertAt(headerView, 0, i);

                i++;
            }
        }

        private void CreateSprintColumns()
        {
            int i = 0;
            Board.ColumnDefinitions.Clear();
            TopHeader.ColumnDefinitions.Clear();

            foreach (var sprint in VisibleSprints)
            {
                Board.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(300) });
                TopHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(300) });

                var headerView = new HeaderView(sprint, Orientation.Horizontal);
                headerView.RequestEdit += (h) => EditEntity(h as Sprint);
                headerView.RequestRemove += (h) => RemoveEntity(h as Sprint);

                TopHeader.InsertAt(headerView, i, 0);

                i++;
            }
        }

        private void CreateTicketCell(int column, int row, long sprintId, long featureId)
        {
            var cellView = new CellView(sprintId, featureId);
            Board.InsertAt(cellView, column, row);
            cellView.Drop += (s, e) => MoveTicket(e.Data.GetData<Ticket>(), sprintId, featureId);
            cellView.RequestNewTicket += (s, f) => NewTicket(s, f);

            var tickets = VisibleTickets
                .Where(t => t.SprintId == sprintId)
                .Where(t => t.FeatureId == featureId);

            foreach (var ticket in tickets)
            {
                var ticketView = new TicketView(ticket);
                cellView.Add(ticketView);

                ticketView.RequestClose += (t) => CloseTicket(t as Ticket);
                ticketView.RequestReopen += (t) => OpenTicket(t as Ticket);
                ticketView.RequestEdit += (t) => EditTicket(t as Ticket);
                ticketView.RequestRemove += (t) => RemoveEntity(t as Ticket);
            }
        }

        private void OpenTicket(Ticket ticket)
        {
            ticket.Open();
            Update();
        }

        private void CloseTicket(Ticket ticket)
        {
            ticket.Close();
            Update();
        }

        private void EditTicket(Ticket ticket)
        {
            ViewDirector.EditTicket(ticket, Context);
            Update();
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

        public void NewTicket(long sprintId, long featureId)
        {
            ViewDirector.AddTicket(Context, Project.ID, sprintId, featureId);
            Update();
        }

        private void MoveTicket(Ticket ticket, long sprintId, long featureId)
        {
            ticket.SprintId = sprintId;
            ticket.FeatureId = featureId;

            Update();
        }

        public void NewSprint()
        {
            ViewDirector.AddEntity<Sprint>(Context, Project.ID);
            Update();
        }

        public void NewFeature()
        {
            ViewDirector.AddEntity<Feature>(Context, Project.ID);
            Update();
        }

        public void NewTicket()
        {
            ViewDirector.AddTicket(Context, Project.ID);
            Update();
        }

        public void NewProject()
        {
            var project = ViewDirector.AddEntity<Project>(Context);
            Update();

            if (OnProjectAdded != null)
                OnProjectAdded.Invoke(project);
        }

        public void OnBoardScroll(object sender, ScrollChangedEventArgs e)
        {
            SyncHeaderScrollers();
        }

        private void SyncHeaderScrollers()
        {
            TopScroller.ScrollToHorizontalOffset(BoardScroller.HorizontalOffset);
            LeftScroller.ScrollToVerticalOffset(BoardScroller.VerticalOffset);
        }
    }
}
