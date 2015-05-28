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

        private ProjectPanel()
        {
            InitializeComponent();
        }

        public ProjectPanel(ScrumrContext context)
            : this()
        {
            Context = context;

            foreach (var project in context.Projects)
            {
                var projectTile = new ProjectTile(project);

                projectTile.RequestEdit += p => EditProject(p);
                projectTile.RequestRemove += async p => await RemoveEntity(p);
                projectTile.RequestOpenProject += p => RequestOpenProject(p);

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
