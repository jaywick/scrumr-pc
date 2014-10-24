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
            AddEntityBase<T>(table, context, new PropertiesView(typeof(T), context));
        }

        public static void AddTicket<T>(DbSet<T> table, ScrumrContext context, Int64? sprintId = null, Int64? featureId = null) where T : Entity
        {
            Dictionary<string, object> initialValues = null;

            if (sprintId.HasValue && featureId.HasValue)
            {
                initialValues = new Dictionary<string, object>();
                initialValues.Add("Sprint", context.Sprints.Single(x => x.ID == sprintId.Value));
                initialValues.Add("Feature", context.Features.Single(x => x.ID == featureId.Value));
            }

            var onSave = new Action<Entity>(x =>
            {
                var ticket = x as Ticket;
                var nextId = ticket.Sprint.Project.NextProjectTicketId++;
                ticket.ProjectTicketId = nextId;
            });

            AddEntityBase<T>(table, context, new PropertiesView(typeof(T), context, initialValues, onSave));
        }

        private static void AddEntityBase<T>(DbSet<T> table, ScrumrContext context, PropertiesView propertiesView) where T : Entity
        {
            if (propertiesView.ShowDialog() == true)
                table.Add(propertiesView.Result as T);

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
