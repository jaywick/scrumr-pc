using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    public class Meta
    {
        public Meta()
        {
            NextProjectIndex = 1;
            NextSprintIndex = 1;
            NextFeatureIndex = 1;
            NextTicketIndex = 1;
        }

        public Meta(int schemaVersion)
            : this()
        {
            SchemaVersion = schemaVersion;
        }

        public int SchemaVersion { get; set; }

        public int NextProjectIndex { get; set; }

        public int NextSprintIndex { get; set; }

        public int NextFeatureIndex { get; set; }

        public int NextTicketIndex { get; set; }
    }
}
