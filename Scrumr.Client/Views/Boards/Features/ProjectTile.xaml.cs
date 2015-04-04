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
    partial class ProjectTile : UserControl
    {
        public event Action<Project> RequestOpenProject;
        public event Action<Project> RequestEdit;
        public event Action<Project> RequestRemove;

        public Project Project { get; set; }

        public ProjectTile(Project project)
        {
            InitializeComponent();

            Project = project;

            labelName.Text = project.Name.ToString();
            labelName.Foreground = Brushes.White;

            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(ViewDirector.CreateMenuItem("Edit", () => RequestEdit(project)));
            ContextMenu.Items.Add(ViewDirector.CreateMenuItem("Remove", () => RequestRemove(project)));

            MouseUp += (s, e) => RequestOpenProject(project);
        }
    }
}
