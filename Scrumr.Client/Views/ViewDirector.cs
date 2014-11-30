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
using Scrumr.Database;

namespace Scrumr.Client
{
    public class ViewDirector
    {
        public static Ticket AddTicket(ScrumrContext context, long? projectId = null, long? sprintId = null, long? featureId = null)
        {
            /*var editView = EditView.Create<Ticket>(context, projectId, sprintId, featureId);
            editView.ShowDialog();

            return (Ticket)editView.Result;*/
            throw new NotImplementedException();
        }

        public static T AddEntity<T>(ScrumrContext context, Project project = null) where T : Entity
        {
            var editor = EditEntityBase<T>.Create(context, project: project);
            return (T)editor.GetResult();
        }

        public static void EditEntity<T>(T entity, ScrumrContext context, Project project = null) where T : Entity
        {
            var editor = EditEntityBase<T>.Create(context, entity, project);
            editor.GetResult();
        }

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
