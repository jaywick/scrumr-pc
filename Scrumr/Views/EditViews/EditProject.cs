using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    class EditProject : EditView
    {
        public EditProject(ScrumrContext context, Entity entity = null)
            : base(typeof(Project), context, entity) { }

        protected override void OnCreating(Entity entity)
        {
            var project = entity as Project;
            var feature = new Feature { Name = "General", Project = project };
            var sprint = new Sprint { Name = "Backlog", Project = project };

            Context.Features.Add(feature);
            Context.Sprints.Add(sprint);

            //project.DefaultFeature = feature;
            //project.Backlog = sprint;
        }
    }
}
