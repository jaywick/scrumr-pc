using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrumr
{
    public class DataListPropertyView : PropertyView
    {
        public DataListPropertyView(PropertyItem propertyItem, Context context)
            : base(propertyItem)
        {
            var foreignSource = propertyItem.Attributes.Single(x => x is ForeignAttribute) as ForeignAttribute;
            var collection = context.GetCollection(foreignSource.ForeignEntity);

            var selected = propertyItem.IsNew ? null : collection.Single(x => x.ID == (int)propertyItem.Value);

            View = new ComboBox
            {
                ItemsSource = collection,
                DisplayMemberPath = "Name",
                SelectedItem = selected,
            };
        }

        public override object Value
        {
            get
            {
                var listBox = View as ComboBox;
                return (listBox.SelectedItem as Entity).ID;
            }
        }

        public override bool IsValid
        {
            get
            {
                return false;
            }
        }
    }
}
