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
}
