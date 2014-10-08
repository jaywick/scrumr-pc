using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrumr
{
    class InvalidInputException : Exception
    {
        private AddEditItem item;
        private object rawValue;

        public InvalidInputException(AddEditItem item, object rawValue, Exception innerException)
            : base(String.Format("Cannot convert {0} to an {1} using the value {2}", item.Name, item.Type.Name, rawValue.ToString()), innerException)
        {
        }
        
    }
}
