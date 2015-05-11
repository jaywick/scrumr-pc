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
    [Migration(from: 1, to: 2)]
    public class Schema_V2 : IMigration
    {
        public bool Upgrade(JObject data)
        {
            UpgradeProjectIds(data);
            UpgradeFeatureIds(data);
            UpgradeSprintIds(data);
            UpgradeTicketIds(data);
            UpgradeMetaIndexes(data);

            return true;
        }

        private void UpgradeMetaIndexes(JObject data)
        {
            /*var meta = data["Meta"];
            meta["NextProjectIndex"].Remove();
            meta["NextProjectIndex"].Remove();
            meta["NextSprintIndex"].Remove();
            meta["NextFeatureIndex"].Remove();
            meta["NextTicketIndex"].Remove();*/
        }

        private void UpgradeProjectIds(JObject data)
        {
            foreach (var project in data["Projects"].Children())
            {
                var oldId = project["ID"];
                var newId = Guid.NewGuid();
                project["ID"] = newId;

                foreach (var sprint in data["Sprints"].Children())
                {
                    if (sprint["ProjectId"].CompareValue(oldId))
                        sprint["ProjectId"] = newId;
                }

                foreach (var feature in data["Features"].Children())
                {
                    if (feature["ProjectId"].CompareValue(oldId))
                        feature["ProjectId"] = newId;
                }
            }
        }

        private void UpgradeFeatureIds(JObject data)
        {
            foreach (var feature in data["Features"].Children())
            {
                var oldId = feature["ID"];
                var newId = Guid.NewGuid();
                feature["ID"] = newId;

                foreach (var project in data["Projects"].Children())
                {
                    if (project["DefaultFeatureId"].CompareValue(oldId))
                        project["DefaultFeatureId"] = newId;

                    if (project["DefaultFeatureId"].GetValue().Equals(0L))
                        project["DefaultFeatureId"] = Guid.Empty;
                }

                foreach (var ticket in data["Tickets"].Children())
                {
                    if (ticket["FeatureId"].CompareValue(oldId))
                        ticket["FeatureId"] = newId;
                }
            }
        }

        private void UpgradeSprintIds(JObject data)
        {
            foreach (var sprint in data["Sprints"].Children())
            {
                var oldId = sprint["ID"];
                var newId = Guid.NewGuid();
                sprint["ID"] = newId;

                foreach (var project in data["Projects"].Children())
                {
                    if (project["BacklogId"].CompareValue(oldId))
                        project["BacklogId"] = newId;

                    if (project["BacklogId"].GetValue().Equals(0L))
                        project["BacklogId"] = Guid.Empty;
                }

                foreach (var ticket in data["Tickets"].Children())
                {
                    if (ticket["SprintId"].CompareValue(oldId))
                        ticket["SprintId"] = newId;
                }
            }
        }

        private void UpgradeTicketIds(JObject data)
        {
            foreach (var ticket in data["Tickets"].Children())
            {
                var oldId = ticket["ID"];
                var newId = Guid.NewGuid();
                ticket["ID"] = newId;
            }
        }
    }
}
