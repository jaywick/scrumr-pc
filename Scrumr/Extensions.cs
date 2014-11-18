﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Scrumr
{
    public static class Extensions
    {
        public static T Get<T>(this IEnumerable<T> target, long id) where T : Entity
        {
            return target.SingleOrDefault(x => x.ID == id);
        }

        public static bool Has<T>(this IEnumerable<T> target, long id) where T : Entity
        {
            return target.Get(id) != null;
        }

        public static void InsertAt(this Grid target, UIElement element, int column, int row)
        {
            target.Children.Add(element);
            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
        }

        public static T Get<T>(this Attribute[] target) where T : Attribute
        {
            return target.SingleOrDefault(x => x is T) as T;
        }

        public static bool Has<T>(this Attribute[] target) where T : Attribute
        {
            return target.Any(x => x is T);
        }

        public static bool IsOneOf(this Attribute[] target, params Type[] attributeTypes)
        {
            return attributeTypes.Any(a => target.Any(x => x.GetType() == a));
        }

        public static bool IsOneOf<T>(this T target, params T[] alternatives) where T : class
        {
            return alternatives.Contains(target);
        }

        public static TResult IfNotNull<T, TResult>(this T target, Func<T, TResult> selector)
        {
            if (target != null)
                return selector(target);

            return default(TResult);
        }

        
    }
}