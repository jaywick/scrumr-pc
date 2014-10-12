using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrumr
{
    public class TextPropertyView : PropertyView
    {
        public TextPropertyView(PropertyItem propertyItem)
            : base(propertyItem)
        {
            View = new TextBox
            {
                Text = propertyItem.Value.ToString(),
            };
        }

        public override object Value
        {
            get { return (View as TextBox).Text; }
        }
    }
}
