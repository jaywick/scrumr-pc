using Scrumr.Client.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Scrumr.Client
{
    static class Extensions
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
    }
}
