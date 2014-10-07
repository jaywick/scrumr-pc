using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    class AddEditItem
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public object Value { get; set; }

        public AddEditItem(string name, Type type, object value = null)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}
