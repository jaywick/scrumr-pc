﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
            if (propertyItem.Attributes.Any(x => x is KeyAttribute || x is IgnoreRenderAttribute))
                return null;

            if (propertyItem.Attributes.Any(x => x is ForeignKeyAttribute))
                return new DataListPropertyView(propertyItem, context);

            if (propertyItem.Type == typeof(bool))
                return new CheckPropertyView(propertyItem);

            if (propertyItem.Type == typeof(string))
            {
                var isLongAnswer = (propertyItem.Attributes.Any(x => x is LongAnswerAttribute));
                return new TextPropertyView(propertyItem, isLongAnswer);
            }

            if (propertyItem.Type == typeof(int) || propertyItem.Type  == typeof(long) || propertyItem.Type == typeof(double) || propertyItem.Type == typeof(float) || propertyItem.Type == typeof(decimal))
                return new NumericPropertyView(propertyItem);

            return null;
        }
    }

    public class PropertyItem
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public Type EntityType { get; set; }
        public Attribute[] Attributes { get; set; }
        public object Value { get; set; }
        public bool IsNew { get; set; }
        
        public PropertyItem(string name, Type type, Type entityType, Attribute[] attributes, object value = null)
        {
            Name = name;
            Type = type;
            EntityType = entityType;
            Attributes = attributes;
            Value = value;

            IsNew = (value == null);
        }

        public int Order
        {
            get
            {
                var orderAttribute = Attributes.SingleOrDefault(x => x is RenderOrderAttribute) as RenderOrderAttribute;

                if (orderAttribute == null)
                    return int.MaxValue;
                else
                    return orderAttribute.Order;
            }
        }
    }
}
