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
        private Type type;
        private Entity _entity;

        public object Result { get; set; }

        public AddEditView()
        {
            InitializeComponent();
        }

        public AddEditView(Type type, Entity entity = null)
            : this()
        {
            _valueType = type;
            _entity = entity;
            _items = loadItems();
            drawItems();
        }

        private List<AddEditItem> loadItems()
        {
            var items = new List<AddEditItem>();
            foreach (var property in _valueType.GetProperties())
            {
                object currentValue = null;

                if (_entity != null)
                    currentValue = property.GetValue(_entity);

                items.Add(new AddEditItem(property.Name, property.PropertyType, currentValue));
            }

            return items;
        }

        private object getResult()
        {
            var result = (_entity != null) ? _entity : Activator.CreateInstance(_valueType);
            
            foreach (var item in _items)
            {
                var property = _valueType.GetProperty(item.Name);
                var rawValue = ContentMap[item.Name].Invoke();
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
                
                if (_entity != null)
                    newInput.Text = item.Value.ToString();

                itemGrid.Children.Add(newInput);
                Grid.SetColumn(newInput, 1);

                ContentMap.Add(item.Name, () => newInput.Text);
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
                MessageBox.Show(ex.Message);
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
