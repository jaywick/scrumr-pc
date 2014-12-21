using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrumr.Database
{
    public class SchemaMismatchException : Exception
    {
        public string FilePath { get; private set; }
        public int ExpectedVersion { get; private set; }
        public int ActualVersion { get; private set; }

        public SchemaMismatchException(string filePath)
            : base(String.Format("Schema information is missing in {0}.", filePath))
        {
            FilePath = filePath;
        }

        public SchemaMismatchException(string filePath, int expectedVersion, int actualVersion)
            : base(String.Format("Schema of database '{0}' does not match application. Expected database schema to be {1}, but was {2}.", filePath, expectedVersion, actualVersion))
        {
            FilePath = filePath;
            ExpectedVersion = expectedVersion;
            ActualVersion = actualVersion;
        }
    }
}
