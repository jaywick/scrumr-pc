using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrumr.Database.Migration
{
    class MigrationAttribute : Attribute
    {
        public int ToVersion { get; set; }

        public int FromVersion
        {
            get { return ToVersion - 1; }
        }

        public MigrationAttribute(int version)
        {
            ToVersion = version;
        }
    }
}
