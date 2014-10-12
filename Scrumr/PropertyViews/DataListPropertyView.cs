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
        private IEnumerable<Entity> _collection;

        public DataListPropertyView(PropertyItem propertyItem, Context context)
            : base(propertyItem)
        {
            var foreignSource = propertyItem.Attributes.Single(x => x is ForeignAttribute) as ForeignAttribute;
            _collection = context.GetCollection(foreignSource.ForeignEntity);

            var selected = propertyItem.IsNew ? null : _collection.Single(x => x.ID == (int)propertyItem.Value);

            View = new ComboBox
            {
                ItemsSource = _collection,
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
                var listBox = View as ComboBox;
                var entity = listBox.SelectedItem as Entity;

                if (entity == null)
                    return false;

                return _collection.Where(x => x.ID == entity.ID).Count() == 1;
            }
        }
    }
}
