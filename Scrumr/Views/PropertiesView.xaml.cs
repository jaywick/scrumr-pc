using System;
using System.Collections.Generic;
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
        private Context _context;
        private List<PropertyItem> _items;

        public List<PropertyView> PropertyViews { get; private set; }
        public object Result { get; private set; }

        public PropertiesView()
        {
            InitializeComponent();
        }

        public PropertiesView(Type type, Context context, Entity entity = null)
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

        private object getResult()
        {
            var result = (Mode == Modes.Existing) ? _entity : Activator.CreateInstance(_entityType);

            foreach (var item in _items)
            {
                var property = _entityType.GetProperty(item.Name);
                var propertyView = PropertyViews.Single(x => x.Property.Name == property.Name);

                if (!propertyView.IsValid)
                    throw new InvalidInputException(item);

                var rawValue = propertyView.Value;

                property.SetValue(result, rawValue);
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

                if (!propertyView.IsHidden)
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
