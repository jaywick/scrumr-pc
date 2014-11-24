using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Scrumr
{
    public abstract class PropertyView
    {
        public PropertyItem Property { get; private set; }
        public UIElement View { get; protected set; }
        public abstract object Value { get; set; }
        public abstract bool IsValid { get; }
        public bool IsNew { get; private set; }
        public virtual bool IsHidden { get; protected set; }

        protected PropertyView(PropertyItem propertyItem)
        {
            Property = propertyItem;
        }

        public static PropertyView Create(PropertyItem propertyItem, ScrumrContext context)
        {
            if (propertyItem.IsRenderingIgnored)
                return null;

            if (propertyItem.IsEntityType)
                return new DataListPropertyView(propertyItem, context);

            if (propertyItem.IsEnumType)
                return new EnumPropertyView(propertyItem);

            if (propertyItem.IsType<bool>())
                return new CheckPropertyView(propertyItem);

            if (propertyItem.IsType<string>())
                return new TextPropertyView(propertyItem, isLongAnswer: propertyItem.Attributes.Has<LongAnswerAttribute>());

            if (propertyItem.IsNumericType)
                return new NumericPropertyView(propertyItem);

            throw new NotSupportedException("Unknown property view");
        }
    }

    [DebuggerDisplay("{Name} ({Type}) = {Value}")]
    public class PropertyItem
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public Type ParentEntityType { get; set; }
        public Attribute[] Attributes { get; set; }
        public object Value { get; set; }
        public bool IsNew { get; set; }

        public PropertyItem(string name, Type type, Type entityType, Attribute[] attributes, object value = null)
        {
            Name = name;
            Type = type;
            ParentEntityType = entityType;
            Attributes = attributes;
            Value = value;

            IsNew = (value == null);
        }

        public int Order
        {
            get
            {
                var orderAttribute = Attributes.Get<RenderOrderAttribute>();

                if (orderAttribute == null)
                    return int.MaxValue;
                else
                    return orderAttribute.Order;
            }
        }

        public bool IsRenderingIgnored
        {
            get { return Attributes.IsOneOf(typeof(IgnoreRenderAttribute), typeof(NotMappedAttribute)); }
        }

        public bool IsEntityType
        {
            get { return Type.BaseType == typeof(Entity); }
        }

        public bool IsEnumType
        {
            get { return Type.BaseType == typeof(Enum); }
        }

        public bool IsType<T>()
        {
            return Type == typeof(T);
        }

        public bool IsNumericType
        {
            get { return Type.IsOneOf(typeof(int), typeof(long), typeof(double), typeof(float), typeof(decimal)); }
        }

    }
}
