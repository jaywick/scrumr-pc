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

namespace Scrumr.Client.Views
{
    public partial class FeatureView : UserControl, IBoardView
    {
        public event Action<Project> OnProjectAdded;

        public Database.ScrumrContext Context { get; set; }
        public Database.Project Project { get; set; }

        public FeatureView()
        {
            InitializeComponent();
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

        public void Update()
        {
            RootItems.Children.Clear();

            foreach (var feature in Project.Features)
            {
                var featurePanel = CreateFeaturePanel(feature);
                var featureHeader = CreateFeatureHeader(feature);
                featureHeader.PreviewMouseDown += (s, e) => ToggleVisibility(featurePanel);
                RootItems.Children.Add(featureHeader);

                foreach (var ticket in feature.GetTickets(Context).OrderBy(x => x.SprintId))
                {
                    var ticketView = new TileTicketView(ticket);

                    ticketView.RequestClose += (t) => CloseTicket(t as Ticket);
                    ticketView.RequestReopen += (t) => OpenTicket(t as Ticket);
                    ticketView.RequestEdit += (t) => EditTicket(t as Ticket);
                    ticketView.RequestRemove += (t) => RemoveEntity(t as Ticket);

                    featurePanel.Children.Add(ticketView);
                }

                var addTile = new AddButtonTileView(feature, feature.Project.Backlog);
                addTile.AddFor += AddTicketFor;
                featurePanel.Children.Add(addTile);

                RootItems.Children.Add(featurePanel);
            }
        }

        private void ToggleVisibility(Panel featurePanel)
        {
            if (featurePanel.Visibility == System.Windows.Visibility.Visible)
                featurePanel.Visibility = System.Windows.Visibility.Collapsed;
            else
                featurePanel.Visibility = System.Windows.Visibility.Visible;
        }

        public void AddTicketFor(Feature feature, Sprint sprint)
        {
            ViewDirector.AddTicket(Context, sprintId: sprint.ID, featureId: feature.ID);
            Update();
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

        private void RemoveEntity<T>(T entity) where T : Entity
        {
            ViewDirector.RemoveEntity(entity, Context);
            Update();
        }

        private UIElement CreateFeatureHeader(Database.Feature feature)
        {
            return new Label
            {
                Content = feature.Name,
                FontSize = 16,
                Foreground = Brushes.Black,
            };
        }

        private Panel CreateFeaturePanel(Database.Feature feature)
        {
            return new WrapPanel
            {
                Margin = new Thickness(10),
            };
        }
    }
}
