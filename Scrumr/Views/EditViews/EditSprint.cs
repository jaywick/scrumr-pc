using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    class EditSprint : EditView
    {
        public EditSprint(ScrumrContext context, Entity entity = null, Project project = null)
            : base(typeof(Sprint), context, entity)
        {
            LoadProjectsList(project);
        }

        private void LoadProjectsList(Project project)
        {
            var projectsView = GetView<Project, DataListPropertyView>();
            projectsView.Source = Context.Projects;

            if (project != null)
                projectsView.SelectItem(project);
        }

        protected override IEnumerable<Expression<Func<Entity, object>>> OnRendering()
        {
            yield return x => (x as Sprint).Name;
            yield return x => (x as Sprint).Project;
        }

        protected override void OnCreated(Entity entity)
        {
            Context.Sprints.Add(entity as Sprint);
            Context.SaveChanges();
        }

        protected override void OnUpdated(Entity entity)
        {
            Context.SaveChanges();
        }
    }
}
