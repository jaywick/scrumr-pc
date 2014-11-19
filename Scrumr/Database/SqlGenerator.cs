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
    class SqlGenerator
    {
        private static Dictionary<Type, string> _typeMap = _typeMap = new Dictionary<Type, string>
        {
            { typeof(byte), "TINYINT" },
            { typeof(bool), "BIT" },
            { typeof(Int16), "SMALLINT" },
            { typeof(Int32), "INT32" },
            { typeof(Int64), "BIGINT" },
            { typeof(decimal), "DECIMAL" },
            { typeof(double), "DOUBLE" },
            { typeof(float), "REAL" },
            { typeof(DateTime), "DATETIME" },
            { typeof(byte[]), "BLOB" },
            { typeof(string), "TEXT" },
            { typeof(char), "NCHAR(1)" },
            { typeof(Enum), "INTEGER" },
        };

        private static readonly string PrimaryKeyColumnFormat = "`{0}` INTEGER PRIMARY KEY AUTOINCREMENT";
        private static readonly string ForeignKeyColumnFormat = "`{0}` INTEGER REFERENCES {1} ({2})";
        private static readonly string GeneralColumnFormat = "`{0}` {1}";

        public static string GenerateCreateScriptFor(Type type)
        {
            var columns = String.Join(", ", GenerateColumnDefinitions(type));
            return String.Format("CREATE TABLE {0} ({1});", type.Name + "s", columns);
        }

        private static IEnumerable<string> GenerateColumnDefinitions(Type entityType)
        {
            // get all primary keys
            var identityColumn = entityType.GetProperties()
                .Where(x => x.HasAttribute<KeyAttribute>());

            // get all reference columns (ones with [ForeignKey] attribute)
            var referenceColumns = entityType.GetProperties()
                .Where(x => x.HasAttribute<ForeignKeyAttribute>());

            // get all foreign key columns using the reference columns' foreign key metadata
            var referenceIdentityColumns = entityType.GetProperties()
                .Where(x => referenceColumns
                    .Select(r => r.GetAttribute<ForeignKeyAttribute>().Name)
                    .Contains(x.Name));

            // get all properties that aren't to be mapped
            var unmappedColumns = entityType.GetProperties()
                .Where(x => x.HasAttribute<NotMappedAttribute>());

            // get all remaining columns
            var dataColumns = entityType.GetProperties()
                .Except(identityColumn)
                .Except(referenceColumns)
                .Except(referenceIdentityColumns)
                .Except(unmappedColumns);

            yield return GeneratePrimaryKeyDefinition(identityColumn.Single());

            foreach (var info in referenceColumns)
                yield return GenerateForeignKeyDefinition(info);

            foreach (var info in dataColumns)
                yield return GenerateColumnDefinition(info);
        }

        private static string GeneratePrimaryKeyDefinition(PropertyInfo info)
        {
            return String.Format(PrimaryKeyColumnFormat, info.Name);
        }

        private static string GenerateForeignKeyDefinition(PropertyInfo info)
        {
            var name = info.GetAttribute<ForeignKeyAttribute>().Name;

            var primaryKey = info.PropertyType.GetProperties()
                .Single(x => x.HasAttribute<KeyAttribute>());

            return String.Format(ForeignKeyColumnFormat, name, info.Name + "s", primaryKey.Name);
        }

        private static string GenerateColumnDefinition(PropertyInfo info)
        {
            return String.Format(GeneralColumnFormat, info.Name, GetSqlDataType(info.PropertyType));
        }

        private static string GetSqlDataType(Type clrType)
        {
            if (clrType.IsEnum)
                return _typeMap[typeof(Enum)];
            
            return _typeMap[clrType];
        }
    }
}
