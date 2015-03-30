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
    public partial class ProjectView : UserControl, IUpdatableView
    {
        public Database.ScrumrContext Context { get; set; }

        public event Action<Feature> RequestOpenFeature;

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

        public ProjectView()
        {
            InitializeComponent();
        }

        public ProjectView(ScrumrContext context, Database.Project project)
            : this()
        {
            this.Context = context;
            this.Project = project;
        }

        public void Update()
        {
            RootItems.Children.Clear();

            foreach (var feature in Project.Features)
            {
                var featureTile = new FeatureTile(feature);
                featureTile.RequestOpen += f => RequestOpenFeature(f);
                RootItems.Children.Add(featureTile);
            }
        }
    }
}
