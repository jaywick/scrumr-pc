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
    public partial class AddEditView : Window
    {
        private List<AddEditItem> _items;
        private Dictionary<string, Func<object>> ContentMap;
        private Type _valueType;

        public object Result { get; set; }

        public AddEditView()
        {
            InitializeComponent();
        }

        public AddEditView(Type type)
            : this()
        {
            _valueType = type;
            _items = loadItems();
            drawItems();
        }

        private List<AddEditItem> loadItems()
        {
            var items = new List<AddEditItem>();
            foreach (var property in _valueType.GetProperties())
            {
                items.Add(new AddEditItem(property.Name, property.PropertyType));
            }
            
            return items;
        }

        private object getResult()
        {
            var newValue = Activator.CreateInstance(_valueType);
            foreach (var item in _items)
            {
                var property = _valueType.GetProperty(item.Name);
                var rawValue = ContentMap[item.Name].Invoke();
                property.SetValue(newValue, Convert.ChangeType(rawValue, item.Type));
            }

            return newValue;
        }

        private void drawItems()
        {
            ContentMap = new Dictionary<string, Func<object>>();

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

                var newInput = new TextBox();
                newInput.Margin = new Thickness(0, 5, 0, 0);
                itemGrid.Children.Add(newInput);
                Grid.SetColumn(newInput, 1);

                ContentMap.Add(item.Name, () => newInput.Text);
                Contents.Children.Add(itemGrid);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Result = getResult();
            Hide();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Result = null;
            Hide();
        }
    }
}
