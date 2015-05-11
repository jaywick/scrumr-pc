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
        public static readonly int CurrentSchemaVersion = 4;

        public int SchemaVersion { get; set; }

        public int UpdateRevision { get; set; }

        public DateTime LastModified { get; set; }

        public Meta()
        {
        }

        public Meta(int schemaVersion)
            : this()
        {
            SchemaVersion = schemaVersion;
        }
    }
}
