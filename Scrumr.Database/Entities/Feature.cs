using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Scrumr.Database
{
    public class Feature : Entity
    {
        public Feature()
            : base(null)
        {
        }

        public Feature(ScrumrContext context)
            : base(context)
        {
        }

        public Feature(string name, Project project)
            : base(project.Context)
        {
            this.Name = name;
            this.ProjectId = project.ID;
        }

        [Foreign, IgnoreRender]
        public int ProjectId { get; set; }

        [JsonIgnore]
        public virtual Project Project
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

        [JsonIgnore, IgnoreRender]
        public IEnumerable<Ticket> Tickets
        {
            get
            {
                return Context.Tickets
                    .Where(x => x.FeatureId == ID);
            }
        }
    }
}
