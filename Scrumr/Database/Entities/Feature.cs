using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Scrumr
{
    [Table("Features")]
    public class Feature : Entity
    {
        [Foreign]
        public long ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}
