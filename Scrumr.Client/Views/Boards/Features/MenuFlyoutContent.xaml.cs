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
    public partial class MenuFlyoutContent : UserControl
    {
        private bool _lockProjectSelection = false;

        public event Action RequestEditProject;
        public event Action RequestChooseFile;
        public event Action RequestCreateFile;
        public event Action RequestNewTicket;
        public event Action RequestNewFeature;
        public event Action RequestNewSprint;
        public event Action RequestNewProject;
        public event Action RequestShowHideClosedTickets;
        public event Action RequestShowHideEmptyFeatures;

        private ScrumrContext Context { get; set; }

        public MenuFlyoutContent()
        {
            InitializeComponent();
        }

        public void Load(ScrumrContext context)
        {
            Context = context;

            AddTicket.Click += (s, e) => RequestNewTicket();
            AddSprint.Click += (s, e) => RequestNewSprint();
            AddFeature.Click += (s, e) => RequestNewFeature();
            AddProject.Click += (s, e) => RequestNewProject();

            EditProject.Click += (s, e) => RequestEditProject();

            LoadDatabase.Click += (s, e) => RequestChooseFile();
            NewDatabase.Click += (s, e) => RequestCreateFile();

            ShowHideClosedTickets.Click += (s, e) => RequestShowHideClosedTickets();
            ShowHideEmptyFeatures.Click += (s, e) => RequestShowHideEmptyFeatures();
        }
    }
}
