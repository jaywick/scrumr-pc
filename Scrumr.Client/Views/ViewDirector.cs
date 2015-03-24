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
        public static Ticket AddTicket(ScrumrContext context, int? projectId = null, int? sprintId = null, int? featureId = null)
        {
            var properties = new PropertyBag();
            properties.Add("projectId", projectId);
            properties.Add("sprintId", sprintId);
            properties.Add("featureId", featureId);

            var editor = EditEntityBase<Ticket>.Create(context, null, properties);
            return (Ticket)editor.GetResult();
        }

        public static T AddEntity<T>(ScrumrContext context, int? projectId = null) where T : Entity
        {
            var properties = new PropertyBag();
            properties.Add("projectId", projectId);

            var editor = EditEntityBase<T>.Create(context, null, properties);
            return (T)editor.GetResult();
        }

        public static void EditTicket(Ticket ticket, ScrumrContext context)
        {
            var properties = new PropertyBag();
            properties.Add("projectId", ticket.ProjectId);
            properties.Add("sprintId", ticket.SprintId);
            properties.Add("featureId", ticket.FeatureId);

            var editor = EditEntityBase<Ticket>.Create(context, ticket, properties);
            editor.Show();
        }

        public static void EditEntity<T>(T entity, ScrumrContext context, int? projectId = null) where T : Entity
        {
            var properties = new PropertyBag();
            properties.Add("projectId", projectId);

            var editor = EditEntityBase<T>.Create(context, entity, properties);
            editor.Show();
        }

        public static async Task RemoveEntity<T>(T entity, ScrumrContext context) where T : Entity
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
