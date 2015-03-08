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
using System.ComponentModel.DataAnnotations.Schema;
using Scrumr.Database;

namespace Scrumr.Client
{
    public partial class EditView : MetroWindow
    {
        public event Action<Entity> PostUpdated;
        public event Action<Entity> PostCreated;
        public event Action<Entity> PreDeleting;
        public event Action<Renderables> PreRendering;

        public Type EntityType { get; set; }
        public Entity Entity { get; set; }
        public ScrumrContext Context { get; set; }
        public List<PropertyView> PropertyViews { get; set; }
        public List<PropertyItem> Items { get; set; }

        public Entity Result { get; private set; }

        public enum Modes
        {
            Creating,
            Updating,
        }

        public EditView(Type type, ScrumrContext context, Entity entity = null)
            : this()
        {
            if (entity == null)
            {
                Mode = Modes.Creating;
                DeleteButton.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                Mode = Modes.Updating;
                DeleteButton.Visibility = System.Windows.Visibility.Visible;
            }

            EntityType = type;
            Context = context;
            Entity = entity;
        }

        public Modes Mode { get; private set; }

        internal void Render()
        {
            Items = loadItems();
            DrawItems();
        }

        private EditView()
        {
            InitializeComponent();
        }

        private PropertyView GetView<T>()
        {
            return PropertyViews.SingleOrDefault(x => x.Property.Type == typeof(T)) as PropertyView;
        }

        private TView GetView<T, TView>() where TView : class
        {
            return PropertyViews.SingleOrDefault(x => x.Property.Type == typeof(T)) as TView;
        }

        private List<PropertyItem> loadItems()
        {
            var items = new List<PropertyItem>();
            foreach (var propertyName in GetRenderableViews())
            {
                var property = EntityType.GetProperty(propertyName);

                var currentValue = GetInitialValue(property);
                var attributes = Attribute.GetCustomAttributes(property);

                items.Add(new PropertyItem(property.Name, property.PropertyType, EntityType, attributes, currentValue));
            }

            return items;
        }

        private IEnumerable<string> GetRenderableViews()
        {
            var r = new Renderables();
            PreRendering(r);
            return r.Items;
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

                var propertyView = PropertyViews
                    .Where(x => x != null)
                    .Single(x => x.Property.Name == property.Name);

                if (!propertyView.IsValid)
                    throw new InvalidInputException(item);

                property.SetValue(result, propertyView.Value);
            }

            switch (Mode)
            {
                case Modes.Creating: if (PostCreated != null) PostCreated(result); break;
                case Modes.Updating: if (PostUpdated != null) PostUpdated(result); break;
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

            Control firstControl = null;
            foreach (var item in Items)
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
                    ((Control)propertyView.View).FontSize = 15;
                    ((Control)propertyView.View).Margin = new Thickness(10, 8, 10, 8);
                    Contents.Children.Add(itemGrid);

                    if (firstControl == null)
                        firstControl = propertyView.View as Control;

                    PropertyViews.Add(propertyView);
                }
            }

            firstControl.Focus();
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

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var confirmResult = await this.ShowMessageAsync("Confirm Delete", "This will delete all items contained within. Are you sure you wish to continue?", MessageDialogStyle.AffirmativeAndNegative);

            if (confirmResult == MessageDialogResult.Negative)
                return;

            if (PreDeleting != null)
                PreDeleting(Entity);

            DialogResult = true;
            Result = null;
            Hide();
        }
    }

    public class Renderables
    {
        public IEnumerable<string> Items { get; set; }
    }
}
