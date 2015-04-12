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

        private ContextMenu _addMenu;
        private ContextMenu _manageProject;

        private ScrumrContext Context { get; set; }

        public MenuFlyoutContent()
        {
            InitializeComponent();
        }

        public void Load(ScrumrContext context)
        {
            Context = context;

            _addMenu = new ContextMenu();
            _addMenu.Items.Add(ViewDirector.CreateMenuItem("Ticket", () => RequestNewTicket()));
            _addMenu.Items.Add(ViewDirector.CreateMenuItem("Feature", () => RequestNewFeature()));
            _addMenu.Items.Add(ViewDirector.CreateMenuItem("Sprint", () => RequestNewSprint()));
            _addMenu.Items.Add(ViewDirector.CreateMenuItem("Project", () => RequestNewProject()));

            AddButton.Click += (s, e) => _addMenu.IsOpen = true;
            _addMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
            _addMenu.PlacementTarget = AddButton;

            _manageProject = new ContextMenu();
            _manageProject.Items.Add(ViewDirector.CreateMenuItem("Configure project", () => RequestEditProject()));
            _manageProject.Items.Add(ViewDirector.CreateMenuItem("Choose database", () => RequestChooseFile()));
            _manageProject.Items.Add(ViewDirector.CreateMenuItem("Create new database", () => RequestCreateFile()));
            _manageProject.Items.Add(ViewDirector.CreateMenuItem("Show/Hide closed tickets", () => RequestShowHideClosedTickets()));
            _manageProject.Items.Add(ViewDirector.CreateMenuItem("Show/Hide empty features", () => RequestShowHideEmptyFeatures()));

            ManageProjectsButton.Click += (s, e) => _manageProject.IsOpen = true;
            _manageProject.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
            _manageProject.PlacementTarget = ManageProjectsButton;
        }
    }
}
