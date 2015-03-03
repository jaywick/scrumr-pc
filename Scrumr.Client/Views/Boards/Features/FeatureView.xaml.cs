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

        public FeatureView()
        {
            InitializeComponent();
        }

        public void Update()
        {
            SaveView();
            RootItems.Children.Clear();
            featureVisibilityMap.Clear();

            foreach (var feature in Project.Features)
            {
                var featurePanel = new FeaturePanel(Context, feature);
                var featureHeader = CreateFeatureHeader(feature);

                featurePanel.Updated += () => Update();
                featureHeader.PreviewMouseDown += (s, e) => ToggleVisibility(featurePanel);

                RootItems.Children.Add(featureHeader);
                RootItems.Children.Add(featurePanel);
            }

            RestoreView();
        }

        private void ToggleVisibility(FeaturePanel featurePanel)
        {
            featurePanel.ToggleVisiblity();
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

        Dictionary<Feature, Visibility> featureVisibilityMap = new Dictionary<Feature, Visibility>();
        private void SaveView()
        {
            foreach (var featurePanel in RootItems.Children.OfType<FeaturePanel>())
            {
                featureVisibilityMap.Add(featurePanel.Feature, featurePanel.Visibility);
            }
        }

        private void RestoreView()
        {
            foreach (var featurePanel in RootItems.Children.OfType<FeaturePanel>())
            {
                if (featureVisibilityMap.ContainsKey(featurePanel.Feature))
                    RootItems.Visibility = featureVisibilityMap[featurePanel.Feature];
                else
                    RootItems.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
