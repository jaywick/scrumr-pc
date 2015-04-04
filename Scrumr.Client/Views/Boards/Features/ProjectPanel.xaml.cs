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
    public partial class ProjectPanel : UserControl
    {
        public event Action Updated;
        public event Action<Project> RequestOpenProject;

        private ScrumrContext Context { get; set; }

        public ProjectPanel()
        {
            InitializeComponent();
        }

        public ProjectPanel(ScrumrContext context)
            : this()
        {
            Context = context;

            foreach (var ticket in context.Projects)
            {
                var projectTile = new ProjectTile(ticket);

                projectTile.RequestEdit += project => EditProject(project);
                projectTile.RequestRemove += async project => await RemoveEntity(project);
                projectTile.RequestOpenProject += project => RequestOpenProject(project);

                LayoutRoot.Children.Add(projectTile);
            }
        }

        private void EditProject(Project project)
        {
            ViewDirector.EditEntity(project, Context);
            Updated();
        }

        private async Task RemoveEntity<T>(T entity) where T : Entity
        {
            await ViewDirector.RemoveEntity(entity, Context);
            Updated();
        }
    }
}
