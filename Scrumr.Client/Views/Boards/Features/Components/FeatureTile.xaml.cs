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
    partial class FeatureTile : UserControl
    {
        public event Action<Feature> RequestOpen;

        public Feature Feature { get; set; }

        public FeatureTile(Feature feature)
        {
            InitializeComponent();

            Feature = feature;

            labelName.Text = feature.Name.ToString();
            labelName.Foreground = Brushes.White;

            PreviewMouseDown += (s, e) => RequestOpen(feature);
        }
    }
}
