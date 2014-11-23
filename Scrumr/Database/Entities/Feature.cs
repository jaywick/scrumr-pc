using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Scrumr
{
    public class Feature : Entity
    {
        [IgnoreRender]
        public long ProjectId { get; set; }

        [RefersTo("Project", "Id")]
        public Project Project { get; set; }
    }
}
