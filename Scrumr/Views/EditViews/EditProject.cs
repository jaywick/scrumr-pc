using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    class EditProject : EditView
    {
        public EditProject(ScrumrContext context, Entity entity = null)
            : base(typeof(Project), context, entity) { }

        protected override IEnumerable<Expression<Func<Entity, object>>> OnRendering()
        {
            yield return x => (x as Project).Name;
        }

        protected override void OnCreated(Entity entity)
        {
            var project = entity as Project;
            var feature = new Feature { Name = "General", Project = project };
            var sprint = new Sprint { Name = "Backlog", Project = project };

            Context.Features.Add(feature);
            Context.Sprints.Add(sprint);

            project.DefaultFeature = feature;
            project.Backlog = sprint;
        }
    }
}
