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
    public partial class MainBoard : UserControl, IBoardView
    {
        public Database.ScrumrContext Context { get; set; }

        public bool ShowClosedTickets { get; set; }

        private Sprint Sprint { get; set; }

        private Entity LastUpdatedEntity { get; set; }

        private Database.Project _project;
        public Database.Project Project
        {
            get { return _project; }
            set
            {
                _project = value;
                Update(value);
            }
        }

        public MainBoard()
        {
            InitializeComponent();
        }

        private void OpenProject(Project project)
        {
            Project = project;
            tabMain.SelectedValue = FeatureTab;
        }

        public void Update(Entity entity)
        {
            LastUpdatedEntity = entity;

            UpdateBreadcrumb();
            UpdateProjects();
            UpdateSprints();
            UpdateFeatures();

            if (LastUpdatedEntity != null && LastUpdatedEntity is Ticket)
            {
                var ticket = (Ticket)LastUpdatedEntity;
                tabSprints.SelectedItem = new SprintTab(ticket.Sprint);
            }
        }

        private void UpdateBreadcrumb()
        {
            FeatureTab.Header = Project.Name + " > Features";
        }

        private void UpdateProjects()
        {
            var projectPanel = new ProjectPanel(Context);
            projectPanel.RequestOpenProject += project => OpenProject(project);
            controlProjectPanel.Content = projectPanel;
        }

        private void UpdateSprints()
        {
            tabSprints.Items.Clear();

            tabSprints.Items.Add(SprintTab.AllSprints); // all
            foreach (var sprint in Project.Sprints)
            {
                tabSprints.Items.Add(new SprintTab(sprint));
            }
        }

        private void UpdateFeatures()
        {
            featureTicketsStack.Children.Clear();

            foreach (var feature in Project.Features)
            {
                var featurePanel = new FeatureTicketsPanel(Context, feature, Sprint, ShowClosedTickets);
                var featureHeader = CreateFeatureHeader(feature);

                featurePanel.Updated += entity => Update(entity);
                featurePanel.SetVisiblity(!feature.IsMinimised);

                featureHeader.PreviewMouseDown += (s, e) => ToggleVisibility(featurePanel);

                featureTicketsStack.Children.Add(featureHeader);
                featureTicketsStack.Children.Add(featurePanel);
            }
        }

        private void ToggleVisibility(FeatureTicketsPanel featurePanel)
        {
            featurePanel.Feature.IsMinimised = !featurePanel.Feature.IsMinimised;
            featurePanel.SetVisiblity(!featurePanel.Feature.IsMinimised);
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

        private void tabSprints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.IsNullOrEmpty())
                return;

            Sprint = e.AddedItems.Cast<SprintTab>().First().Sprint;
            UpdateFeatures();
        }
    }
}
