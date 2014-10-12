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
        private List<PropertyItem> _items;
        private Type _valueType;
        private Entity _entity;

        public Dictionary<string, Func<object>> PropertyValueMap { get; private set; }
        public object Result { get; private set; }

        public PropertiesView()
        {
            InitializeComponent();
        }

        public PropertiesView(Type type, Entity entity = null)
            : this()
        {
            _valueType = type;
            _entity = entity;
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
            foreach (var property in _valueType.GetProperties())
            {
                var currentValue = (Mode == Modes.Existing) ? property.GetValue(_entity) : null;
                var attributes = Attribute.GetCustomAttributes(property);

                items.Add(new PropertyItem(property.Name, property.PropertyType, attributes, currentValue));
            }

            return items;
        }

        private object getResult()
        {
            var result = (Mode == Modes.Existing) ? _entity : Activator.CreateInstance(_valueType);

            foreach (var item in _items)
            {
                var property = _valueType.GetProperty(item.Name);
                var rawValue = PropertyValueMap[item.Name].Invoke();
                try
                {
                    property.SetValue(result, Convert.ChangeType(rawValue, item.Type));
                }
                catch (FormatException ex)
                {
                    throw new InvalidInputException(item, rawValue, ex);
                }
            }

            return result;
        }

        private void drawItems()
        {
            PropertyValueMap = new Dictionary<string, Func<object>>();

            Contents.Children.Clear();
            foreach (var item in _items)
            {
                var itemGrid = new Grid();
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition());
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition());

                var newLabel = new Label();
                newLabel.Content = item.Name;
                itemGrid.Children.Add(newLabel);
                Grid.SetColumn(newLabel, 0);

                var newInput = PropertyView.Create(item);

                itemGrid.Children.Add(newInput.View);
                Grid.SetColumn(newInput.View, 1);

                PropertyValueMap.Add(item.Name, () => newInput.Value);
                Contents.Children.Add(itemGrid);
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
