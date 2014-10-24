using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
using MahApps.Metro.Controls.Dialogs;

namespace Scrumr
{
    public partial class PropertiesView : MetroWindow
    {
        private Type _entityType;
        private Entity _entity;
        private ScrumrContext _context;
        private List<PropertyItem> _items;
        private Dictionary<Expression<Func<Ticket, object>>, object> _initialValues;
        private Action<Entity> _onSaveAction;

        public List<PropertyView> PropertyViews { get; private set; }
        public Entity Result { get; private set; }

        public PropertiesView(Type type, ScrumrContext context, Dictionary<Expression<Func<Ticket, object>>, object> initialValues, Action<Entity> onSave = null)
            : this(context, type)
        {
            Mode = Modes.NewWithData;

            _initialValues = initialValues;
            _onSaveAction = onSave;

            render();
        }

        public PropertiesView(Type type, ScrumrContext context, Entity entity)
            : this(context, type)
        {
            Mode = Modes.Existing;

            _entity = entity;

            render();
        }

        public PropertiesView(Type type, ScrumrContext context)
            : this(context, type)
        {
            Mode = Modes.New;

            _entityType = type;
            _context = context;

            render();
        }

        private void render()
        {
            _items = loadItems();
            drawItems();
        }

        private PropertiesView(ScrumrContext context, Type type)
        {
            InitializeComponent();
            _entityType = type;
            _context = context;
        }

        public enum Modes
        {
            NotSet,
            New,
            NewWithData,
            Existing
        }

        public Modes Mode { get; private set; }

        private List<PropertyItem> loadItems()
        {
            var items = new List<PropertyItem>();
            foreach (var property in _entityType.GetProperties())
            {
                var currentValue = getInitialValue(property);
                var attributes = Attribute.GetCustomAttributes(property);

                items.Add(new PropertyItem(property.Name, property.PropertyType, _entityType, attributes, currentValue));
            }

            return items;
        }

        private object getInitialValue(PropertyInfo property)
        {
            switch (Mode)
            {
                case Modes.New:
                    return null;
                case Modes.NewWithData:
                    if (_initialValues == null) return null;
                    var result = _initialValues.SingleOrDefault(x => (x.Key.Body as System.Linq.Expressions.MemberExpression).Member.Name == property.Name); //todo: refactor
                    return result.Value;
                case Modes.Existing:
                    return property.GetValue(_entity);
                default:
                    throw new NotSupportedException();
            }
        }

        private Entity getResult()
        {
            Entity result = getEntity();

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

            if (_onSaveAction != null)
                _onSaveAction.Invoke(result);

            return result;
        }

        private Entity getEntity()
        {
            switch (Mode)
            {
                case Modes.New:
                case Modes.NewWithData:
                    return Activator.CreateInstance(_entityType) as Entity;
                case Modes.Existing:
                    return _entity;
                default:
                    throw new NotSupportedException();
            }
        }

        private void drawItems()
        {
            PropertyViews = new List<PropertyView>();

            Contents.Children.Clear();

            var orderedItems = _items.OrderBy(x => x.Order);
            foreach (var item in orderedItems)
            {
                var propertyView = PropertyView.Create(item, _context);

                if (propertyView != null && propertyView.View != null)
                {
                    // grid
                    var itemGrid = new Grid();
                    itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
                    itemGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    // label
                    var newLabel = new Label();
                    newLabel.Content = item.Name;
                    itemGrid.Children.Add(newLabel);
                    Grid.SetColumn(newLabel, 0);

                    // input
                    itemGrid.Children.Add(propertyView.View);
                    Grid.SetColumn(propertyView.View, 1);
                    (propertyView.View as Control).Margin = new Thickness(10, 5, 10, 5);
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
                this.ShowMessageAsync("Scrumr", ex.Message);
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
