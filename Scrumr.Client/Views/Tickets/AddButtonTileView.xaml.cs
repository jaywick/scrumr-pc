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
using Scrumr.Client;

namespace Scrumr.Client
{
    partial class AddButtonTileView : UserControl
    {
        public event Action<Feature, Sprint> AddFor;

        private Feature Feature { get; set; }
        private Sprint Sprint { get; set; }

        public AddButtonTileView(Feature feature, Sprint sprint)
        {
            InitializeComponent();
            this.Feature = feature;
            this.Sprint = sprint;
            this.PreviewMouseDown += (s, e) => AddFor.Invoke(Feature, Sprint);
        }
    }
}
