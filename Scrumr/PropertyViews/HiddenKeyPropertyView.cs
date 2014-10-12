using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class HiddenKeyPropertyView : PropertyView
    {
        public HiddenKeyPropertyView(PropertyItem propertyItem)
            : base(propertyItem)
        {
            IsHidden = true;
        }

        public override object Value
        {
            get { return Property.Value; }
        }
    }
}
