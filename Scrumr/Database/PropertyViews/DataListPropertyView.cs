using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrumr
{
    public class DataListPropertyView : PropertyView
    {
        private List<Entity> _collection;

        public DataListPropertyView(PropertyItem propertyItem, ScrumrContext context)
            : base(propertyItem)
        {
            var foreignSource = propertyItem.Attributes.Single(x => x is ForeignKeyAttribute) as ForeignKeyAttribute;
            _collection = context.GetCollection(propertyItem.Type);
            
            var selected = propertyItem.IsNew ? null : _collection.Single(x => x.ID == (propertyItem.Value as Entity).ID);

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
                return ((View as ComboBox).SelectedItem as Entity);
            }
        }

        public override bool IsValid
        {
            get
            {
                var entity = (View as ComboBox).SelectedItem as Entity;

                if (entity == null)
                    return false;

                return _collection.Where(x => x.ID == entity.ID).Count() == 1;
            }
        }
    }
}
