using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Tests
{
    class TestEntity : Entity
    {
        [Primary]
        public long PrimaryKey { get; set; }

        public string Message { get; set; }

        public long? NullableValue { get; set; }

        public long AnotherValue { get; set; }

        [Foreign]
        public long ForeignKeyId { get; set; }

        public TestForeignEntity ForeignKeyReference { get; set; }
    }

    class TestForeignEntity : Entity
    { }
}
