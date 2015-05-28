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
    [Migration(version: 5)]
    public class Schema_V5 : IMigration
    {
        public bool Upgrade(JObject data)
        {
            MoveFeaturesToProjectBacklog(data);
            return true;
        }

        private void MoveFeaturesToProjectBacklog(JObject data)
        {
            var projectsBacklogMap = new Dictionary<string, string>();
            
            foreach (var project in data["Projects"])
            {
                var projectId = project["ID"].Value<string>();
                var backlogId = project["BacklogId"].Value<string>();
                projectsBacklogMap.Add(projectId, backlogId);
            }

            foreach (var feature in data["Features"])
            {
                var projectId = feature["ProjectId"].Value<string>();
                feature["SprintId"] = projectsBacklogMap[projectId];
            }
        }
    }
}
