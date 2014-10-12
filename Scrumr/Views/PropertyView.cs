using System;
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
        private Attribute[] Attributes;
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

        protected PropertyView(PropertyItem propertyItem)
        {
            Property = propertyItem;
        }

        public static PropertyView Create(PropertyItem propertyItem)
        {
            return new TextPropertyView(propertyItem);
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
}
