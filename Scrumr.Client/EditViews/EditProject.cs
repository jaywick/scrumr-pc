using Scrumr.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client
{
    class EditProject : EditEntityBase<Project>
    {
        public EditProject(ScrumrContext context, Project entity = null, PropertyBag properties = null)
            : base(context, entity) { }

        protected override IEnumerable<Expression<Func<Project, object>>> OnRender()
        {
            yield return x => x.Name;
        }

        protected override async void OnCreated(Project project)
        {
            await Context.AddNewProjectAsync(project);
        }
    }
}
