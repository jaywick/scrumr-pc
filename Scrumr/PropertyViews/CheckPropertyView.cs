using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrumr.Client
{
    public class CheckPropertyView : PropertyView
    {
        public CheckPropertyView(PropertyItem propertyItem)
            : base(propertyItem)
        {
            View = new CheckBox
            {
                IsChecked = Convert.ToBoolean(propertyItem.Value),
            };
        }

        public override object Value
        {
            get { return (View as CheckBox).IsChecked; }
            set { (View as CheckBox).IsChecked = (bool)value; }
        }

        public override bool IsValid
        {
            get { return true; }
        }
    }
}
