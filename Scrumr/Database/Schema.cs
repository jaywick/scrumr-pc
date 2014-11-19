using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrumr;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Scrumr
{
    class Schema
    {
        public static string Create(Type type)
        {
            var columns = String.Join(", ", GetColumns(type));
            return String.Format("CREATE TABLE {0} ({1});", type.Name + "s", columns);
        }

        private static IEnumerable<string> GetColumns(Type entityType)
        {
            var identityColumn = entityType.GetProperties()
                .Where(x => x.HasAttribute<KeyAttribute>());

            var referenceColumns = entityType.GetProperties()
                .Where(x => x.HasAttribute<ForeignKeyAttribute>());

            var referenceIdentityColumns = entityType.GetProperties()
                .Where(x => referenceColumns
                    .Select(r => r.GetAttribute<ForeignKeyAttribute>().Name)
                    .Contains(x.Name));

            var unmappedColumns = entityType.GetProperties()
                .Where(x => x.HasAttribute<NotMappedAttribute>());

            var dataColumns = entityType.GetProperties()
                .Except(identityColumn)
                .Except(referenceColumns)
                .Except(referenceIdentityColumns)
                .Except(unmappedColumns);

            yield return GetIdFormat(identityColumn.Single());

            foreach (var info in referenceColumns)
                yield return GetReferenceFormat(info);

            foreach (var info in dataColumns)
                yield return GetDataFormat(info);
        }

        private static string GetIdFormat(PropertyInfo info)
        {
            return String.Format("`{0}` INTEGER PRIMARY KEY AUTOINCREMENT", info.Name);
        }

        private static string GetReferenceFormat(PropertyInfo info)
        {
            var name = info.GetAttribute<ForeignKeyAttribute>().Name;
            var primaryKey = info.PropertyType.GetProperties()
                .Single(x => x.HasAttribute<KeyAttribute>());
            return String.Format("`{0}` INTEGER REFERENCES {1} ({2})", name, info.Name, primaryKey.Name);
        }

        private static string GetDataFormat(PropertyInfo info)
        {
            return String.Format("`{0}` {1}", info.Name, GetDataType(info.PropertyType));
        }

        private static string GetDataType(Type type)
        {
            var mapping = new Dictionary<Type, string>
            {
                { typeof(byte), "TINYINT" },
                { typeof(bool), "BIT" },
                { typeof(Int16), "SMALLINT" },
                { typeof(Int32), "INT32" },
                { typeof(decimal), "DECIMAL" },
                { typeof(Int64), "BIGINT" },
                { typeof(double), "DOUBLE" },
                { typeof(float), "REAL" },
                { typeof(DateTime), "DATETIME" },
                { typeof(byte[]), "BLOB" },
                { typeof(string), "TEXT" },
                { typeof(char), "NCHAR(1)" },
            };

            if (type.IsEnum)
                return "INTEGER";

            return mapping[type];
        }
    }
}
