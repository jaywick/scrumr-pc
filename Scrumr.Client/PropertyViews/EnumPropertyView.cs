using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrumr.Client
{
    public class EnumPropertyView : PropertyView
    {
        private IEnumerable<Enum> _source = Enumerable.Empty<Enum>();
        private Type _type;

        public EnumPropertyView(PropertyItem propertyItem)
            : base(propertyItem)
        {
            _type = propertyItem.Type;
            _source = Enum.GetValues(_type).Cast<Enum>();

            var selected = propertyItem.IsNew
                ? null
                : Convert.ChangeType(propertyItem.Value, _type);

            View = new ComboBox
            {
                SelectedItem = selected,
            };

            Source = _source;
        }

        private ComboBox ActualView
        {
            get { return View as ComboBox; }
        }

        public override object Value
        {
            get
            {
                return ActualView.SelectedItem;
            }
            set
            {
                ActualView.SelectedItem = value;
            }
        }

        public IEnumerable<Enum> Source
        {
            get { return _source; }
            set
            {
                _source = value;
                ActualView.ItemsSource = _source.ToList();
            }
        }

        public override bool IsValid
        {
            get
            {
                var entity = (View as ComboBox).SelectedItem as Enum;

                if (entity == null)
                    return false;

                return Enum.IsDefined(_type, entity);
            }
        }
    }
}
