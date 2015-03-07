using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using Scrumr.Database;

namespace Scrumr.Client
{
    public class ViewDirector
    {
        public static async void RemoveEntity<T>(T entity, ScrumrContext context) where T : Entity
        {
            if (await (App.Current.MainWindow as MainWindow).ShowMessageAsync("Scrumr", "Are you sure you wish to delete this " + entity.GetType().Name + "?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
                return;

            if (typeof(T) == typeof(Sprint))
                context.Sprints.Remove(entity as Sprint);
            else if (typeof(T) == typeof(Feature))
                context.Features.Remove(entity as Feature);
            else if (typeof(T) == typeof(Ticket))
                context.Tickets.Remove(entity as Ticket);
            else
                throw new NotSupportedException("Unexpected entity being removed. Operation failed.");

            await context.SaveChangesAsync();
        }

        public static MenuItem CreateMenuItem(string text, System.Action action)
        {
            var newItem = new MenuItem();
            newItem.Header = text;
            newItem.FontSize = 16;
            newItem.Padding = new Thickness(20, 10, 10, 20);
            newItem.VerticalContentAlignment = VerticalAlignment.Center;
            newItem.Click += (s, e) => action.Invoke();

            return newItem;
        }
    }
}
