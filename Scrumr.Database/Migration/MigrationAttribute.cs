using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrumr.Database.Migration
{
    class MigrationAttribute : Attribute
    {
        public int FromVersion { get; set; }
        public int ToVersion { get; set; }

        public MigrationAttribute(int from, int to)
        {
            FromVersion = from;
            ToVersion = to;
        }
    }
}
