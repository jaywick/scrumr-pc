using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client
{
    public interface ITicketView
    {
        Database.Ticket Project { get; set; }
    }
}
