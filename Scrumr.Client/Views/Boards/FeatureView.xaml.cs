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
        public Database.Project Project { get; set; }

        public FeatureView()
        {
            InitializeComponent();
        }

        public void NewTicket()
        {
        }

        public void NewFeature()
        {
        }

        public void NewSprint()
        {
        }

        public void NewProject()
        {
        }

        public void Update()
        {
        }
    }
}
