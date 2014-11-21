using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    class RefersToAttribute : System.Attribute
    {
        public string Table { get; private set; }
        public string Column { get; private set; }

        public RefersToAttribute(string table, string column)
        {
            Table = table;
            Column = column;
        }
    }
}
