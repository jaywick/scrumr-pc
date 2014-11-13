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

        public DataListPropertyView(PropertyItem propertyItem)
            : base(propertyItem)
        {
            View = new ComboBox
            {
                ItemsSource = Source,
                DisplayMemberPath = "Name",
            };
        }

        public override object Value
        {
            get
            {
                return ((View as ComboBox).SelectedItem as Entity);
            }
            set
            {
                (View as ComboBox).SelectedItem = value as Entity;
            }
        }

        public IEnumerable<Entity> Source
        {
            get { return _source; }
            set
            {
                _source = value;
                (View as ComboBox).ItemsSource = _source.ToList();
            }
        }

        public override bool IsValid
        {
            get
            {
                var entity = (View as ComboBox).SelectedItem as Entity;

                if (entity == null)
                    return false;

                return _source.Count(x => x.ID == entity.ID) == 1;
            }
        }
    }
}
