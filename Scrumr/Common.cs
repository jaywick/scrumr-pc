using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Scrumr
{
    public class InvalidInputException : Exception
    {
        public InvalidInputException(PropertyItem item)
            : base(String.Format("Please enter a valid value for {0}", item.Name))
        {
        }
    }

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
    }
}
