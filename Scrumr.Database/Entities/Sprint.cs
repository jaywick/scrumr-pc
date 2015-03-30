using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Scrumr.Database
{
    public class Sprint : Entity
    {
        public Sprint()
            : base(null)
        {
        }

        public Sprint(ScrumrContext context)
            : base(context)
        {
        }

        public Sprint(string name, Project project)
            : base(project.Context)
        {
            this.Name = name;
            this.ProjectId = project.ID;
        }

        [Foreign]
        public int ProjectId { get; set; }

        [JsonIgnore]
        public Project Project
        {
            get
            {
                return Context.Projects[ProjectId];
            }
            set
            {
                ProjectId = value.ID;
            }
        }

        [JsonIgnore]
        public IEnumerable<Ticket> Tickets
        {
            get
            {
                return Context.Tickets
                    .Where(x => x.FeatureId == ID);
            }
        }

        [JsonIgnore]
        public bool IsBacklog
        {
            get
            {
                return this.ID == Project.BacklogId;
            }
        }
    }
}
