using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Scrumr
{
    public class Sprint : Entity
    {
        [IgnoreRender]
        public long ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
    }
}
