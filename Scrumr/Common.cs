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
        public static T Get<T>(this IEnumerable<T> target, Int64 id) where T : Entity
        {
            return target.SingleOrDefault(x => x.ID == id);
        }

        public static bool Has<T>(this IEnumerable<T> target, Int64 id) where T : Entity
        {
            return target.Get(id) != null;
        }
    }
}
