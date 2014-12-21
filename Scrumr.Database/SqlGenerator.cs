using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scrumr.Database
{
    public class SqlGenerator
    {
        private static Dictionary<Type, string> _typeMap = new Dictionary<Type, string>
        {
            { typeof(byte), "INTEGER" },
            { typeof(bool), "INTEGER" },
            { typeof(Int16), "INTEGER" },
            { typeof(Int32), "INTEGER" },
            { typeof(Int64), "INTEGER" },
            { typeof(decimal), "REAL" },
            { typeof(double), "REAL" },
            { typeof(float), "REAL" },
            { typeof(DateTime), "DATETIME" },
            { typeof(byte[]), "BLOB" },
            { typeof(string), "TEXT" },
            { typeof(char), "INTEGER" },
            { typeof(Enum), "INTEGER" },
        };

        private static readonly string PrimaryKeyColumnFormat = "`{0}` {1} PRIMARY KEY AUTOINCREMENT";
        private static readonly string ForeignKeyColumnFormat = "`{0}` {1}";
        private static readonly string GeneralColumnFormat = "`{0}` {1}";
        private static readonly string NotNullKeyPhrase = "NOT NULL";

        public static string GenerateCreateScriptFor(Type type)
        {
            var columns = String.Join(", ", GenerateColumnDefinitions(type));
            var tableName = type.GetCustomAttribute<TableAttribute>().Name;

            return String.Format("CREATE TABLE {0} ({1});", tableName, columns);
        }

        public static IEnumerable<string> GenerateColumnDefinitions(Type entityType)
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

        public static string GeneratePrimaryKeyDefinition(PropertyInfo info)
        {
            return String.Format(PrimaryKeyColumnFormat, info.Name, GetSqlDataType(info.PropertyType));
        }

        public static string GenerateForeignKeyDefinition(PropertyInfo info)
        {
            var type = info.PropertyType;

            return String.Format(ForeignKeyColumnFormat, info.Name, GetSqlDataType(info.PropertyType));
        }

        public static string GenerateColumnDefinition(PropertyInfo info)
        {
            return String.Format(GeneralColumnFormat, info.Name, GetSqlDataType(info.PropertyType));
        }

        public static string GetSqlDataType(Type clrType)
        {
            if (clrType.IsEnum)
                return _typeMap[typeof(Enum)];

            if (clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>))
                return GetNullableType(clrType);

            return _typeMap[clrType] + " " + NotNullKeyPhrase;
        }

        public static string GetNullableType(Type clrType)
        {
            var innerType = clrType.GetGenericArguments().FirstOrDefault();

            if (innerType == null) return null;

            return _typeMap[innerType];
        }
    }
}
