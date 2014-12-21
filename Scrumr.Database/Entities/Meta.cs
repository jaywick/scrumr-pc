using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    [Table("Meta")]
    public class Meta
    {
        [Primary]
        public long ID { get; set; }

        public int SchemaVersion { get; set; }
    }
}
