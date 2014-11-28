using Scrumr.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client
{
    class EditSprint : EditEntityBase<Sprint>
    {
        public EditSprint(ScrumrContext context, Sprint sprint = null, Project project = null)
            : base(context, sprint)
        {
            LoadProjectsList(project);
        }

        protected override IEnumerable<Expression<Func<Sprint, object>>> OnRender()
        {
            yield return x => x.Name;
            yield return x => x.Project;
        }

        protected override void OnCreated(Sprint sprint)
        {
            Context.Sprints.Add(sprint);
            Context.SaveChanges();
        }

        protected override void OnUpdated(Sprint sprint)
        {
            Context.SaveChanges();
        }

        private void LoadProjectsList(Project project)
        {
            var projectsView = GetView<Project, DataListPropertyView>();
            projectsView.Source = Context.Projects;

            if (project != null)
                projectsView.SelectItem(project);
        }
    }
}
