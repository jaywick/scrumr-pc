using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Scrumr
{
    public partial class PropertiesView : Window
    {
        private Type _entityType;
        private Entity _entity;
        private ScrumrContext _context;
        private List<PropertyItem> _items;

        public List<PropertyView> PropertyViews { get; private set; }
        public Entity Result { get; private set; }

        public PropertiesView()
        {
            InitializeComponent();
        }

        public PropertiesView(Type type, ScrumrContext context, Entity entity = null)
            : this()
        {
            _entityType = type;
            _entity = entity;
            _context = context;
            _items = loadItems();
            drawItems();
        }

        public enum Modes
        {
            New,
            Existing
        }

        public Modes Mode
        {
            get
            {
                if (_entity == null)
                    return Modes.New;
                else
                    return Modes.Existing;
            }
        }

        private List<PropertyItem> loadItems()
        {
            var items = new List<PropertyItem>();
            foreach (var property in _entityType.GetProperties())
            {
                var currentValue = (Mode == Modes.Existing) ? property.GetValue(_entity) : null;
                var attributes = Attribute.GetCustomAttributes(property);

                items.Add(new PropertyItem(property.Name, property.PropertyType, _entityType, attributes, currentValue));
            }

            return items;
        }

        private Entity getResult()
        {
            Entity result = (Mode == Modes.Existing) ? _entity : Activator.CreateInstance(_entityType) as Entity;

            foreach (var item in _items)
            {
                var property = _entityType.GetProperty(item.Name);
                
                // skip primary keys and ignored
                var attributes = Attribute.GetCustomAttributes(property);
                if (attributes.Any(x => x is KeyAttribute || x is IgnoreRenderAttribute)) continue;

                var propertyView = PropertyViews.Where(x => x != null)
                                                .Single(x => x.Property.Name == property.Name);

                if (!propertyView.IsValid)
                    throw new InvalidInputException(item);

                var rawValue = propertyView.Value;
                var finalValue = Convert.ChangeType(rawValue, item.Type);

                property.SetValue(result, finalValue);
            }

            return result;
        }

        private void drawItems()
        {
            PropertyViews = new List<PropertyView>();

            Contents.Children.Clear();
            foreach (var item in _items)
            {
                var propertyView = PropertyView.Create(item, _context);

                if (propertyView != null && propertyView.View != null)
                {
                    // grid
                    var itemGrid = new Grid();
                    itemGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    itemGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    // label
                    var newLabel = new Label();
                    newLabel.Content = item.Name;
                    itemGrid.Children.Add(newLabel);
                    Grid.SetColumn(newLabel, 0);

                    // input
                    itemGrid.Children.Add(propertyView.View);
                    Grid.SetColumn(propertyView.View, 1);
                    Contents.Children.Add(itemGrid);
                }

                PropertyViews.Add(propertyView);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Result = getResult();
                DialogResult = true;
                Hide();
            }
            catch (InvalidInputException ex)
            {
                MessageBox.Show(ex.Message, "Scrumr", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Result = null;
            Hide();
        }
    }
}
