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
                Text = propertyItem.IsNew ? "0" : propertyItem.Value.ToString(),
            };
        }

        private static bool isNumericOnly(string text)
        {
            var pattern = @"^"          // start of string
                        + @"[-+]?"      // optional plus or minus sign
                        + @"[0-9]*"     // 0 or more digits
                        + @"\.?"        // optional decimal point
                        + @"[0-9]+"     // 1 or more digits
                        + @"$";         // end of string

            return new Regex(pattern).IsMatch(text);
        }

        public override object Value
        {
            get { return Convert.ToDouble((View as TextBox).Text); }
        }

        public override bool IsValid
        {
            get
            {
                return isNumericOnly((View as TextBox).Text);
            }
        }
    }
}
