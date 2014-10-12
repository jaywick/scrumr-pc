﻿using System;
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

        public Dictionary<string, Func<object>> PropertyValueMap { get; private set; }
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
                var newInput = PropertyView.Create(item, _context);

                if (!newInput.IsHidden)
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
                    itemGrid.Children.Add(newInput.View);
                    Grid.SetColumn(newInput.View, 1);
                    Contents.Children.Add(itemGrid);
                }

                PropertyValueMap.Add(item.Name, () => newInput.Value);
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
