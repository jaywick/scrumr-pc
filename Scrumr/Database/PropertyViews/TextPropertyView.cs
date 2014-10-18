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
        public TextPropertyView(PropertyItem propertyItem, bool isLongAnswer = false)
            : base(propertyItem)
        {
            string value = propertyItem.IsNew ? "" : propertyItem.Value.ToString();

            View = new TextBox
            {
                Text = value,
                Height = isLongAnswer ? 100 : double.NaN,
            };
        }

        public override object Value
        {
            get { return (View as TextBox).Text; }
        }

        public override bool IsValid
        {
            get { return true; }
        }
    }
}
