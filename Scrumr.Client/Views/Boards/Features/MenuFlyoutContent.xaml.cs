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

        public event Action<Project> ProjectSelected;
        public event Action RequestEditProject;
        public event Action RequestChooseFile;
        public event Action RequestCreateFile;
        public event Action RequestNewTicket;
        public event Action RequestNewFeature;
        public event Action RequestNewSprint;
        public event Action RequestNewProject;
        public event Action RequestShowHideClosedTickets;

        private ContextMenu _addMenu;
        private ContextMenu _projectsList;

        private ScrumrContext Context { get; set; }

        public MenuFlyoutContent()
        {
            InitializeComponent();
            ProjectsList.Items.Clear();
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

            _projectsList = new ContextMenu();
            _projectsList.Items.Add(ViewDirector.CreateMenuItem("Configure project", () => RequestEditProject()));
            _projectsList.Items.Add(ViewDirector.CreateMenuItem("Choose database", () => RequestChooseFile()));
            _projectsList.Items.Add(ViewDirector.CreateMenuItem("Create new database", () => RequestCreateFile()));
            _projectsList.Items.Add(ViewDirector.CreateMenuItem("Show/Hide closed tickets", () => RequestShowHideClosedTickets()));

            ManageProjectsButton.Click += (s, e) => _projectsList.IsOpen = true;
            _projectsList.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
            _projectsList.PlacementTarget = ManageProjectsButton;
        }

        public void SelectProject(Project project)
        {
            ProjectsList.SelectedItem = project;
        }

        private void OnProjectSelected(object s, SelectionChangedEventArgs e)
        {
            if (_lockProjectSelection)
                return;

            if (e.AddedItems.Count == 0)
                return;

            Project selectedProject;

            if (e.AddedItems.Count > 0)
                selectedProject = e.AddedItems[e.AddedItems.Count - 1] as Project;
            else
                selectedProject = e.AddedItems[0] as Project;

            if (selectedProject == null)
                return;

            ProjectSelected(selectedProject);
        }

        public void Update()
        {
            _lockProjectSelection = true;

            ProjectsList.Items.Clear();

            foreach (var item in Context.Projects)
                ProjectsList.Items.Add(item);

            _lockProjectSelection = false;
        }
    }
}
