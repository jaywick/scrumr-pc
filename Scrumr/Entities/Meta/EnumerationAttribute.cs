using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrumr
{
    class EnumerationAttribute : Attribute
    {
        public Type Enumeration { get; private set; }

        public EnumerationAttribute(Type enumeration)
        {
            Enumeration = enumeration;
        }
    }
}
