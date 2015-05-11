using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database.Migration
{
    public class Migrator
    {
        private int _fromVersion;
        private int _toVersion;

        public Migrator(int fromVersion, int toVersion)
        {
            _fromVersion = fromVersion;
            _toVersion = toVersion;
        }

        public void Upgrade(string databasePath)
        {
            var upgrades = Assembly.GetAssembly(typeof(IMigration))
                .GetTypes()
                .Where(t => typeof(IMigration).IsAssignableFrom(t))
                .Select(t => new
                {
                    MigrationAttribute = t.GetAttribute<MigrationAttribute>(),
                    SchemaUpgradeType = t,
                })
                .Where(x => x.MigrationAttribute != null && (x.MigrationAttribute.FromVersion >= _fromVersion && x.MigrationAttribute.ToVersion <= _toVersion))
                .OrderBy(x => x.MigrationAttribute.FromVersion)
                .Select(x => (IMigration)Activator.CreateInstance(x.SchemaUpgradeType));

            var rawData = System.IO.File.ReadAllText(databasePath);
            var jsonData = JObject.Parse(rawData);

            foreach (var upgrade in upgrades)
            {
                var wasUpgradeSuccess = upgrade.Upgrade(jsonData);
                
                if (!wasUpgradeSuccess)
                    throw new UpgradeFailedException(_fromVersion, _toVersion, upgrade.GetType());
	        }

            jsonData["Meta"]["SchemaVersion"] = _toVersion;

            var databaseFile = new FileInfo(databasePath);
            var backupPath = Path.Combine(databaseFile.Directory.FullName, Path.GetFileNameWithoutExtension(databasePath) + String.Format("_v{0}_backup.scrumrbk", _fromVersion));
            databaseFile.MoveTo(backupPath);

            File.WriteAllText(databasePath, jsonData.ToString());
        }
    }
}
