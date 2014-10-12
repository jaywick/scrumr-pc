using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Scrumr
{
    public class PropertyItem
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public Attribute[] Attributes { get; set; }
        public object Value { get; set; }

        public PropertyItem(string name, Type type, Attribute[] attributes, object value = null)
        {
            Name = name;
            Type = type;
            Attributes = attributes;
            Value = value;
        }
    }

    public abstract class PropertyView
    {
        public PropertyItem Property { get; private set; }
        public UIElement View { get; protected set; }
        public abstract object Value { get; }
        public virtual bool IsHidden { get; protected set; }

        protected PropertyView(PropertyItem propertyItem)
        {
            Property = propertyItem;
        }

        public static PropertyView Create(PropertyItem propertyItem, Context context)
        {
            if (propertyItem.Attributes.Any(x => x is KeyAttribute))
                return new HiddenKeyPropertyView(propertyItem);

            if (propertyItem.Attributes.Any(x => x is ForeignAttribute))
                return new DataListPropertyView(propertyItem, context);

            if (propertyItem.Attributes.Any(x => x is EnumerationAttribute))
                return new EnumListPropertyView(propertyItem);

            if (propertyItem.Type == typeof(bool))
                return new CheckPropertyView(propertyItem);

            if (propertyItem.Type == typeof(string))
                return new TextPropertyView(propertyItem);

            if (propertyItem.Type == typeof(int) || propertyItem.Type == typeof(double) || propertyItem.Type == typeof(float) || propertyItem.Type == typeof(decimal))
                return new NumericPropertyView(propertyItem);

            return null;
        }
    }

    public class TextPropertyView : PropertyView
    {
        public TextPropertyView(PropertyItem propertyItem)
            : base(propertyItem)
        {
            View = new TextBox
            {
                Text = propertyItem.Value.ToString(),
            };
        }

        public override object Value
        {
            get { return (View as TextBox).Text; }
        }
    }

    public class NumericPropertyView : PropertyView
    {
        public NumericPropertyView(PropertyItem propertyItem)
            : base(propertyItem)
        {
            View = new TextBox
            {
                Text = propertyItem.Value.ToString(),
            };

            View.PreviewTextInput += (s, e) =>
            {
                var pattern = @"^"          // start of string
                            + @"[-+]?"      // optional plus or minus sign
                            + @"[0-9]*"     // 0 or more digits
                            + @"\.?"        // optional decimal point
                            + @"[0-9]+"     // 1 or more digits
                            + @"$";         // end of string

                e.Handled = new Regex(pattern).IsMatch(e.Text);
            };
        }

        public override object Value
        {
            get { return Convert.ToDouble((View as TextBox).Text); }
        }
    }

    public class CheckPropertyView : PropertyView
    {
        public CheckPropertyView(PropertyItem propertyItem)
            : base(propertyItem)
        {
            View = new CheckBox
            {
                IsChecked = Convert.ToBoolean(propertyItem.Value),
            };
        }

        public override object Value
        {
            get { return (View as CheckBox).IsChecked; }
        }
    }

    public class EnumListPropertyView : PropertyView
    {
        private Type _enum;

        public EnumListPropertyView(PropertyItem propertyItem)
            : base(propertyItem)
        {
            var enumInfo = propertyItem.Attributes.Single(x => x is EnumerationAttribute) as EnumerationAttribute;
            _enum = enumInfo.Enumeration;

            View = new ComboBox
            {
                ItemsSource = Enum.GetNames(_enum).ToList(),
            };
        }

        public override object Value
        {
            get { return Enum.Parse(_enum, (View as ComboBox).SelectedItem.ToString()); }
        }
    }

    public class DataListPropertyView : PropertyView
    {
        public DataListPropertyView(PropertyItem propertyItem, Context context)
            : base(propertyItem)
        {
            var foreignSource = propertyItem.Attributes.Single(x => x is ForeignAttribute) as ForeignAttribute;
            var collection = context.GetForeign(foreignSource.ForeignEntity);
            var selected = collection.Single(x => x.ID == (int)propertyItem.Value);

            View = new ComboBox
            {
                ItemsSource = collection,
                DisplayMemberPath = "Name",
                SelectedItem = selected,
            };
        }

        public override object Value
        {
            get
            {
                var listBox = View as ComboBox;
                return (listBox.SelectedItem as Entity).ID;
            }
        }
    }

    public class HiddenKeyPropertyView : PropertyView
    {
        public HiddenKeyPropertyView(PropertyItem propertyItem)
            : base(propertyItem)
        {
            IsHidden = true;
        }

        public override object Value
        {
            get { return Property.Value; }
        }
    }
}
