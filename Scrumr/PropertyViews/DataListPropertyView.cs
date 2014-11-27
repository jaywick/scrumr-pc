using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Scrumr.Client;
using Scrumr.Client.Database;

namespace Scrumr.Client
{
    public class DataListPropertyView : PropertyView
    {
        private IEnumerable<Entity> _source = Enumerable.Empty<Entity>();

        public DataListPropertyView(PropertyItem propertyItem, ScrumrContext context)
            : base(propertyItem)
        {
            var defaultSource = Enumerable.Empty<Entity>();

            var selected = propertyItem.IsNew
                ? null
                : defaultSource.Get((propertyItem.Value as Entity).ID);

            View = new ComboBox
            {
                DisplayMemberPath = "Name",
                SelectedItem = selected,
            };

            Source = defaultSource;
        }

        private ComboBox ActualView
        {
            get { return View as ComboBox; }
        }

        public void SelectItem(Entity entity)
        {
            ActualView.SelectedItem = entity;
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

                return _source.Has(entity.ID);
            }
        }
    }
}
