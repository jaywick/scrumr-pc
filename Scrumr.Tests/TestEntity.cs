using Scrumr.Database;
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
        public TestEntity(ScrumrContext context)
            : base(context)
        {
        }

        [Primary]
        public int PrimaryKey { get; set; }

        public string Message { get; set; }

        public int? NullableValue { get; set; }

        public int AnotherValue { get; set; }

        [Foreign]
        public int? NullableForeignKeyId { get; set; }

        [Foreign]
        public int ForeignKeyId { get; set; }

        public TestForeignEntity NullableForeignKeyReference { get; set; }

        public TestForeignEntity ForeignKeyReference { get; set; }
    }

    class TestForeignEntity : Entity
    {
        public TestForeignEntity(ScrumrContext context)
            : base(context)
        {
        }
    }
}
