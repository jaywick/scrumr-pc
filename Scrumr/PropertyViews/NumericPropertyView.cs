using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrumr
{
    public class NumericPropertyView : PropertyView
    {
        public NumericPropertyView(PropertyItem propertyItem)
            : base(propertyItem)
        {
            View = new TextBox
            {
                Text = propertyItem.Value.ToString(),
            };

            View.PreviewTextInput += (s, e) =>
            {
                var pattern = @"^"          // start of string
                            + @"[-+]?"      // optional plus or minus sign
                            + @"[0-9]*"     // 0 or more digits
                            + @"\.?"        // optional decimal point
                            + @"[0-9]+"     // 1 or more digits
                            + @"$";         // end of string

                e.Handled = new Regex(pattern).IsMatch(e.Text);
            };
        }

        public override object Value
        {
            get { return Convert.ToDouble((View as TextBox).Text); }
        }
    }
}
