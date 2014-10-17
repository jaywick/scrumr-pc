using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrumr
{
    public class GeneratedKeyPropertyView : PropertyView
    {
        private Context _context;
        private GeneratedReadOnlyAttribute _generatedAttribute;
        private int _value;

        public GeneratedKeyPropertyView(PropertyItem propertyItem, Context context)
            : base(propertyItem)
        {
            _context = context;
            _generatedAttribute = propertyItem.Attributes.Single(x => x is GeneratedReadOnlyAttribute) as GeneratedReadOnlyAttribute;

            _value = generateId(context);

            View = new TextBox
            {
                Text = _value.ToString(),
            };
        }

        private int generateId(Context context)
        {
            context.GetNextTicketId();
        }

        public override object Value
        {
            get { return _value; }
        }

        public override bool IsValid
        {
            get { return true; }
        }
    }
}
