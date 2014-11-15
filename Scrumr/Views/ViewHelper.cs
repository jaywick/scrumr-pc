using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Scrumr
{
    public class ViewHelper
    {
        public static void AddTicket<T>(DbSet<T> table, Context context, long? projectId = null, long? sprintId = null, long? featureId = null) where T : Entity
        {
            var editView = new EditTicket(context, projectId, sprintId, featureId);
            AddEntity<T>(table, context, editView);
        }

        public static void AddEntity<T>(DbSet<T> table, Context context, EditView editView = null) where T : Entity
        {
            if (editView == null)
                editView = EditView.Create<T>(context);

            if (editView.ShowDialog() == true)
                table.Add(editView.Result as T);

            context.SaveChanges();
        }

        public static void EditEntity<T>(T entity, Context context, EditView editView = null) where T : Entity
        {
            if (editView == null)
                editView = EditView.Create<T>(context, entity);

            if (editView.ShowDialog() == true)
                context.SaveChanges();
        }

        public static async void RemoveEntity<T>(T entity, Context context) where T : Entity
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
