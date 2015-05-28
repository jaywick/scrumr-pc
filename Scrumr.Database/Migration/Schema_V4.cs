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
    [Migration(version: 4)]
    public class Schema_V4 : IMigration
    {
        public bool Upgrade(JObject data)
        {
            UpdateEntities(data["Projects"]);
            UpdateEntities(data["Features"]);
            UpdateEntities(data["Sprints"]);
            return true;
        }

        private void UpdateEntities(JToken entitySet)
        {
            foreach (var ticket in entitySet.Children())
            {
                ticket["Created"] = DateTime.UtcNow;
            }
        }
    }
}
