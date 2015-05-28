using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrumr.Database.Migration;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Scrumr.Database.Migration
{
    [Migration(version: 3)]
    public class Schema_V3 : IMigration
    {
        public bool Upgrade(JObject data)
        {
            UpgradeTickets(data);
            return true;
        }

        private void UpgradeTickets(JObject data)
        {
            foreach (var ticket in data["Tickets"].Children())
            {
                if (ticket["State"].GetValue().Equals((long)TicketState.Closed))
                    ticket["LastClosed"] = DateTime.UtcNow;

                ticket["Created"] = DateTime.UtcNow;
            }
        }
    }
}
