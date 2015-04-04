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
    public partial class MainBoard : UserControl, IBoardView
    {
        public Database.ScrumrContext Context { get; set; }

        private Database.Project _project;
        public Database.Project Project
        {
            get { return _project; }
            set
            {
                _project = value;
                Update();
            }
        }

        public MainBoard()
        {
            InitializeComponent();
        }

        public void Update()
        {
            RootItems.Children.Clear();

            foreach (var feature in Project.Features)
            {
                var featurePanel = new FeatureTicketsPanel(Context, feature);
                var featureHeader = CreateFeatureHeader(feature);

                featurePanel.Updated += () => Update();
                featurePanel.SetVisiblity(!feature.IsMinimised);

                featureHeader.PreviewMouseDown += (s, e) => ToggleVisibility(featurePanel);

                RootItems.Children.Add(featureHeader);
                RootItems.Children.Add(featurePanel);
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
    }
}
