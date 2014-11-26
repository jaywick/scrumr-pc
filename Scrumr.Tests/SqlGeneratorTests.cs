using System;
using NUnit.Framework;
using Scrumr;
using System.Reflection;

namespace Scrumr.Tests
{
    [TestFixture]
    public class SqlGeneratorTests
    {
        [TestCase(typeof(long?), "BIGINT")]
        [TestCase(typeof(byte?), "TINYINT")]
        [TestCase(typeof(bool?), "BIT")]
        public void ShouldReturnNullableType(Type input, string expected)
        {
            var actual = SqlGenerator.GetNullableType(input);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(typeof(long), null)]
        [TestCase(typeof(byte), null)]
        [TestCase(typeof(bool), null)]
        public void ShouldReturnNullForNonNullableTypes(Type input, string expected)
        {
            var actual = SqlGenerator.GetNullableType(input);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(typeof(long), "BIGINT NOT NULL")]
        [TestCase(typeof(long?), "BIGINT")]
        public void ShouldReturnSqlDataType(Type input, string expected)
        {
            var actual = SqlGenerator.GetSqlDataType(input);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("Message", "`Message` TEXT NOT NULL")]
        [TestCase("NullableValue", "`NullableValue` BIGINT")]
        [TestCase("AnotherValue", "`AnotherValue` BIGINT NOT NULL")]
        public void ShouldGenerateColumnDefinition(string propertyName, string expected)
        {
            var input = TestHelper.GetPropertyInfo(typeof(TestEntity), propertyName);

            var actual = SqlGenerator.GenerateColumnDefinition(input);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("ForeignKeyReference", "`ForeignKeyReference` BIGINT NOT NULL")]
        public void ShouldGenerateForeignKeyDefinition(string propertyName, string expected)
        {
            var input = TestHelper.GetPropertyInfo(typeof(TestEntity), propertyName);

            var actual = SqlGenerator.GenerateForeignKeyDefinition(input);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("PrimaryKey", "`PrimaryKey` BIGINT NOT NULL PRIMARY KEY AUTOINCREMENT")]
        public void ShouldGeneratePrimaryKeyDefinition(string propertyName, string expected)
        {
            var input = TestHelper.GetPropertyInfo(typeof(TestEntity), propertyName);

            var actual = SqlGenerator.GeneratePrimaryKeyDefinition(input);
            Assert.AreEqual(expected, actual);
        }
    }
}
