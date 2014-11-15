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
        private IEnumerable<Entity> _source = Enumerable.Empty<Entity>();

        public DataListPropertyView(PropertyItem propertyItem, Context context)
            : base(propertyItem)
        {
            var foreignSource = propertyItem.Attributes.Single(x => x is ForeignKeyAttribute) as ForeignKeyAttribute;
            var defaultSource = context.GetCollection(propertyItem.Type);

            var selected = propertyItem.IsNew
                ? null
                : defaultSource.Get((propertyItem.Value as Entity).ID);

            View = new ComboBox
            {
                ItemsSource = defaultSource,
                DisplayMemberPath = "Name",
                SelectedItem = selected,
            };
        }

        private ComboBox ActualView
        {
            get { return View as ComboBox; }
        }

        public override object Value
        {
            get
            {
                return ActualView.SelectedItem as Entity;
            }
            set
            {
                ActualView.SelectedItem = value as Entity;
            }
        }

        public IEnumerable<Entity> Source
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
                var entity = (View as ComboBox).SelectedItem as Entity;

                if (entity == null)
                    return false;

                return _source.Has(entity.ID) != null;
            }
        }
    }
}
