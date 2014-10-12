using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class HiddenKeyPropertyView : PropertyView
    {
        private Context _context;
        private KeyAttribute _keyAttribute;

        public HiddenKeyPropertyView(PropertyItem propertyItem, Context context)
            : base(propertyItem)
        {
            IsHidden = true;
            _context = context;
            _keyAttribute = propertyItem.Attributes.Single(x => x is KeyAttribute) as KeyAttribute;
        }

        public override object Value
        {
            get
            {
                if (Property.IsNew)
                    return _context.GetNextId(Property.EntityType);
                else
                    return Property.Value;
            }
        }

        public override bool IsValid
        {
            get
            {
                var idExists = _context.GetCollection(Property.EntityType).Any(x => x.ID == (int)Value);

                if (Property.IsNew)
                    return !idExists; // new id must not exist
                else
                    return idExists; // existing id must exist
            }
        }
    }
}
