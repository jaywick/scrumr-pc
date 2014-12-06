﻿using Scrumr.Database;
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
        public EditSprint(ScrumrContext context, Sprint sprint = null, PropertyBag properties = null)
            : base(context, sprint)
        {
            var projectId = properties.GetValue<long>("projectId");
            LoadProjectsList(projectId);
        }

        protected override IEnumerable<Expression<Func<Sprint, object>>> OnRender()
        {
            yield return x => x.Name;
            yield return x => x.Project;
        }

        protected override async Task OnCreated(Sprint sprint)
        {
            Context.Sprints.Add(sprint);
            await Context.SaveChangesAsync();
        }

        private void LoadProjectsList(long? projectId)
        {
            var projectsView = GetView<Project, DataListPropertyView>();
            projectsView.Source = Context.Projects;

            if (projectId.HasValue)
                projectsView.SelectItem(Context.Projects.Get(projectId.Value));
        }
    }
}
