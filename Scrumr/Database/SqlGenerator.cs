using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrumr;
using System.Reflection;

namespace Scrumr
{
    class SqlGenerator
    {
        private static Dictionary<Type, string> _typeMap = new Dictionary<Type, string>
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

        private static readonly string PrimaryKeyColumnFormat = "`{0}` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT";
        private static readonly string ForeignKeyColumnFormat = "`{0}` INTEGER";
        private static readonly string GeneralColumnFormat = "`{0}` {1}";
        private static readonly string NotNullKeyPhrase = "NOT NULL";

        public static string GenerateCreateScriptFor(Type type)
        {
            var columns = String.Join(", ", GenerateColumnDefinitions(type));
            return String.Format("CREATE TABLE {0} ({1});", type.Name + "s", columns);
        }

        private static IEnumerable<string> GenerateColumnDefinitions(Type entityType)
        {
            // get all primary keys
            var identityColumn = entityType.GetProperties()
                .Where(x => x.HasAttribute<PrimaryAttribute>());

            // get all entity references used for navigation
            var navigationItems = entityType.GetProperties()
                .Where(x => x.PropertyType.BaseType == typeof(Entity));

            // get all collections used for navigation
            var navigationCollections = entityType.GetProperties()
                .Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericArguments().First().BaseType == typeof(Entity));

            // get all int columns that are the actual foreign keys
            var referenceIdentityColumns = entityType.GetProperties()
                .Where(x => x.HasAttribute<ForeignAttribute>());

            // get all properties that aren't to be mapped
            var unmappedColumns = entityType.GetProperties()
                .Where(x => x.HasAttribute<System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute>());

            // get all remaining columns
            var dataColumns = entityType.GetProperties()
                .Except(identityColumn)
                .Except(navigationItems)
                .Except(referenceIdentityColumns)
                .Except(navigationCollections)
                .Except(unmappedColumns);

            yield return GeneratePrimaryKeyDefinition(identityColumn.Single());

            foreach (var info in referenceIdentityColumns)
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
            var type = info.PropertyType;

            return String.Format(ForeignKeyColumnFormat, info.Name, type.Name + "s", "ID");
        }

        private static string GenerateColumnDefinition(PropertyInfo info)
        {
            return String.Format(GeneralColumnFormat, info.Name, GetSqlDataType(info.PropertyType));
        }

        private static string GetSqlDataType(Type clrType)
        {
            if (clrType.IsEnum)
                return _typeMap[typeof(Enum)];

            if (clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>))
                return GetNullableType(clrType);

            return _typeMap[clrType] + " " + NotNullKeyPhrase;
        }

        private static string GetNullableType(Type clrType)
        {
            var innerType = clrType.GetGenericArguments().First();
            return _typeMap[innerType];
        }
    }
}
