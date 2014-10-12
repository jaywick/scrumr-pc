using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrumr
{
    public class EnumListPropertyView : PropertyView
    {
        private Type _enum;

        public EnumListPropertyView(PropertyItem propertyItem)
            : base(propertyItem)
        {
            var enumInfo = propertyItem.Attributes.Single(x => x is EnumerationAttribute) as EnumerationAttribute;
            _enum = enumInfo.Enumeration;

            var collection = Enum.GetNames(_enum).ToList();

            string selected = null;
            if (!propertyItem.IsNew)
            {
                selected = Enum.ToObject(_enum, propertyItem.Value).ToString();
            }

            View = new ComboBox
            {
                ItemsSource = collection,
                SelectedItem = selected,
            };
        }

        public override object Value
        {
            get { return Enum.Parse(_enum, (View as ComboBox).SelectedItem.ToString()); }
        }

        public override bool IsValid
        {
            get
            {
                var selection = (View as ComboBox).SelectedItem;

                if (selection == null)
                    return false;

                try
                {
                    var result = Enum.Parse(_enum, selection.ToString());
                    return result != null;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
