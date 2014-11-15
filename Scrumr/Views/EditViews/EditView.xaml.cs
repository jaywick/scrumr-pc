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
    public abstract partial class EditView : MetroWindow
    {
        protected Type EntityType { get; set; }
        protected Entity Entity { get; set; }
        protected Context Context { get; set; }
        private List<PropertyItem> Items { get; set; }

        protected List<PropertyView> PropertyViews { get; private set; }
        public Entity Result { get; private set; }

        public enum Modes
        {
            Creating,
            Updating,
        }

        public EditView(Type type, Context context, Entity entity = null)
            : this()
        {
            Mode = entity == null
                ? Modes.Creating
                : Modes.Updating;

            EntityType = type;
            Context = context;
            Entity = entity;

            Render();
        }

        private void Render()
        {
            Items = loadItems();
            DrawItems();
        }

        private EditView()
        {
            InitializeComponent();
        }

        public Modes Mode { get; private set; }

        protected virtual void OnUpdating(Entity entity) { }

        protected virtual void OnCreating(Entity entity) { }

        protected PropertyView GetView<T>()
        {
            return PropertyViews.SingleOrDefault(x => x.Property.Type == typeof(T)) as PropertyView;
        }

        protected TView GetView<T, TView>() where TView : class
        {
            return PropertyViews.SingleOrDefault(x => x.Property.Type == typeof(T)) as TView;
        }

        private List<PropertyItem> loadItems()
        {
            var items = new List<PropertyItem>();
            foreach (var property in EntityType.GetProperties())
            {
                var currentValue = GetInitialValue(property);
                var attributes = Attribute.GetCustomAttributes(property);

                items.Add(new PropertyItem(property.Name, property.PropertyType, EntityType, attributes, currentValue));
            }

            return items;
        }

        private object GetInitialValue(PropertyInfo property)
        {
            switch (Mode)
            {
                case Modes.Creating: return null;
                case Modes.Updating: return property.GetValue(Entity);
                default: throw new NotSupportedException();
            }
        }

        private Entity GetResult()
        {
            Entity result = GetEntity();

            foreach (var item in Items)
            {
                var property = EntityType.GetProperty(item.Name);

                // skip primary keys and ignored
                var attributes = Attribute.GetCustomAttributes(property);
                if (attributes.Has<KeyAttribute>() || attributes.Has<IgnoreRenderAttribute>())
                    continue;

                var propertyView = PropertyViews
                    .Where(x => x != null)
                    .Single(x => x.Property.Name == property.Name);

                if (!propertyView.IsValid)
                    throw new InvalidInputException(item);

                var rawValue = propertyView.Value;
                var finalValue = Convert.ChangeType(rawValue, item.Type);

                property.SetValue(result, finalValue);
            }

            switch (Mode)
            {
                case Modes.Creating: OnCreating(result); break;
                case Modes.Updating: OnUpdating(result); break;
                default: throw new NotSupportedException();
            }

            return result;
        }

        private Entity GetEntity()
        {
            switch (Mode)
            {
                case Modes.Creating: return Activator.CreateInstance(EntityType) as Entity;
                case Modes.Updating: return Entity;
                default: throw new NotSupportedException();
            }
        }

        private void DrawItems()
        {
            PropertyViews = new List<PropertyView>();

            Contents.Children.Clear();

            var orderedItems = Items.OrderBy(x => x.Order);
            foreach (var item in orderedItems)
            {
                var propertyView = PropertyView.Create(item, Context);

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

                    PropertyViews.Add(propertyView);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Result = GetResult();
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

        public static EditView Create<T>(Context context, Entity entity = null)
        {
            if (typeof(T) == typeof(Feature))
                return new EditFeature(context, entity);
            else if (typeof(T) == typeof(Sprint))
                return new EditSprint(context, entity);
            else if (typeof(T) == typeof(Project))
                return new EditProject(context, entity);
            else if (typeof(T) == typeof(Ticket))
                return new EditTicket(context, entity);
            else
                throw new NotSupportedException("EditView.Create does not support this type");
        }
    }
}
