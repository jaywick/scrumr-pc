using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrumr
{
    class ForeignAttribute : Attribute
    {
        public Type ForeignEntity { get; private set; }

        public ForeignAttribute(Type foreignEntity)
        {
            ForeignEntity = foreignEntity;
        }
    }
}
