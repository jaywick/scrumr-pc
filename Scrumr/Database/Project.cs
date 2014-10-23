using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class Project : Entity
    {
        [IgnoreRender]
        public Int64 NextProjectTicketId { get; set; }
    }
}
