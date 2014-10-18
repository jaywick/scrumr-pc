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
    public class ViewHelper
    {
        public static void AddEntity<T>(DbSet<T> table, ScrumrContext context) where T : Entity
        {
            var propertiesView = new PropertiesView(typeof(T), context);

            if (propertiesView.ShowDialog() == true)
            {
                table.Add(propertiesView.Result as T);
            }

            context.SaveChanges();
        }

        public static void EditEntity<T>(T entity, ScrumrContext context) where T : Entity
        {
            var propertiesView = new PropertiesView(typeof(T), context, entity);

            if (propertiesView.ShowDialog() == true)
                context.SaveChanges();
        }

        public static void RemoveEntity<T>(T entity, ScrumrContext context) where T : Entity
        {
            if (MessageBox.Show("Are you sure you wish to delete this " + entity.GetType().Name + "?", "Scrumr", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            if (typeof(T) == typeof(Sprint))
                context.Sprints.Remove(entity as Sprint);
            else if (typeof(T) == typeof(Feature))
                context.Features.Remove(entity as Feature);
            else if (typeof(T) == typeof(Ticket))
                context.Tickets.Remove(entity as Ticket);
            else
                throw new NotSupportedException("Unexpected entity being removed. Operation failed.");

            context.SaveChanges();
        }

        public static MenuItem CreateMenuItem(string text, System.Action action)
        {
            var newItem = new MenuItem();
            newItem.Header = text;
            newItem.Click += (s, e) => action.Invoke();

            return newItem;
        }
    }
}
