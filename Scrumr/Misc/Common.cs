using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrumr
{
    class Common
    {
        public static MenuItem CreateMenuItem(string text, System.Action action)
        {
            var newItem = new MenuItem();
            newItem.Header = text;
            newItem.Click += (s, e) => action.Invoke();

            return newItem;
        }
    }

    class InvalidInputException : Exception
    {
        private PropertiesView.PropertyItem item;
        private object rawValue;

        public InvalidInputException(PropertiesView.PropertyItem item, object rawValue, Exception innerException)
            : base(String.Format("Cannot convert {0} to an {1} using the value {2}", item.Name, item.Type.Name, rawValue.ToString()), innerException)
        {
        }

    }
}
