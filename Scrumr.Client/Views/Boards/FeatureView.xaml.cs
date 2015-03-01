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
                featurePanel.Children.Add(CreateFeatureHeader(feature));

                foreach (var sprint in Project.Sprints)
                {
                    var sprintPanel = CreateSprintPanel(sprint);
                    featurePanel.Children.Add(CreateSprintHeader(sprint));
                    featurePanel.Children.Add(sprintPanel);

                    foreach (var ticket in Project.GetTickets(Context, feature, sprint))
                    {
                        var ticketView = new TileTicketView(ticket);

                        ticketView.RequestClose += (t) => CloseTicket(t as Ticket);
                        ticketView.RequestReopen += (t) => OpenTicket(t as Ticket);
                        ticketView.RequestEdit += (t) => EditTicket(t as Ticket);
                        ticketView.RequestRemove += (t) => RemoveEntity(t as Ticket);

                        sprintPanel.Children.Add(ticketView);
                    }
                }

                RootItems.Children.Add(featurePanel);
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
                FontSize = 25,
                Foreground = Brushes.Black,
            };
        }

        private UIElement CreateSprintHeader(Database.Sprint sprint)
        {
            return new Label
            {
                Content = sprint.Name,
                FontSize = 16,
                Foreground = Brushes.CadetBlue,
            };
        }

        private Panel CreateFeaturePanel(Database.Feature feature)
        {
            return new StackPanel
            {
                Margin = new Thickness(10),
            };
        }

        private Panel CreateSprintPanel(Database.Sprint sprint)
        {
            return new WrapPanel
            {
                Margin = new Thickness(10),
            };
        }
    }
}
