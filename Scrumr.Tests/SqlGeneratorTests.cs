using System;
using NUnit.Framework;
using Scrumr.Client;
using System.Reflection;
using Scrumr.Database;

namespace Scrumr.Tests
{
    [TestFixture, Category("SQL Generation")]
    public class SqlGeneratorTests
    {
        [TestCase(typeof(int?), "INTEGER")]
        [TestCase(typeof(byte?), "INTEGER")]
        [TestCase(typeof(bool?), "INTEGER")]
        public void ShouldReturnNullableType(Type input, string expected)
        {
            var actual = SqlGenerator.GetNullableType(input);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(typeof(int), null)]
        [TestCase(typeof(byte), null)]
        [TestCase(typeof(bool), null)]
        public void ShouldReturnNullForNonNullableTypes(Type input, string expected)
        {
            var actual = SqlGenerator.GetNullableType(input);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(typeof(int), "INTEGER NOT NULL")]
        [TestCase(typeof(int?), "INTEGER")]
        public void ShouldReturnSqlDataType(Type input, string expected)
        {
            var actual = SqlGenerator.GetSqlDataType(input);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("Message", "`Message` TEXT NOT NULL")]
        [TestCase("NullableValue", "`NullableValue` INTEGER")]
        [TestCase("AnotherValue", "`AnotherValue` INTEGER NOT NULL")]
        public void ShouldGenerateColumnDefinition(string propertyName, string expected)
        {
            var input = TestHelper.GetPropertyInfo(typeof(TestEntity), propertyName);

            var actual = SqlGenerator.GenerateColumnDefinition(input);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("NullableForeignKeyId", "`NullableForeignKeyId` INTEGER")]
        [TestCase("ForeignKeyId", "`ForeignKeyId` INTEGER NOT NULL")]
        public void ShouldGenerateForeignKeyDefinition(string propertyName, string expected)
        {
            var input = TestHelper.GetPropertyInfo(typeof(TestEntity), propertyName);

            var actual = SqlGenerator.GenerateForeignKeyDefinition(input);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("PrimaryKey", "`PrimaryKey` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT")]
        public void ShouldGeneratePrimaryKeyDefinition(string propertyName, string expected)
        {
            var input = TestHelper.GetPropertyInfo(typeof(TestEntity), propertyName);

            var actual = SqlGenerator.GeneratePrimaryKeyDefinition(input);
            Assert.AreEqual(expected, actual);
        }
    }
}
