using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    public class DatabaseContainer
    {
        public Meta Meta { get; set; }
        public List<Project> Projects { get; set; }
        public List<Feature> Features { get; set; }
        public List<Sprint> Sprints { get; set; }
        public List<Ticket> Tickets { get; set; }

        public DatabaseContainer()
        {
            Meta = new Meta();
            Projects = new List<Project>();
            Features = new List<Feature>();
            Sprints = new List<Sprint>();
            Tickets = new List<Ticket>();
        }

        public DatabaseContainer(ScrumrContext context)
        {
            Meta = context.Meta;
            Meta.NextTicketIndex = context.Tickets.NextIndex;
            Meta.NextFeatureIndex = context.Features.NextIndex;
            Meta.NextProjectIndex = context.Projects.NextIndex;
            Meta.NextSprintIndex = context.Sprints.NextIndex;

            Projects = new List<Project>(context.Projects);
            Features = new List<Feature>(context.Features);
            Sprints = new List<Sprint>(context.Sprints);
            Tickets = new List<Ticket>(context.Tickets);
        }
    }
}
