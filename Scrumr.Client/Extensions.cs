using Scrumr.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Scrumr.Client
{
    static class Extensions
    {
        public static T Get<T>(this IEnumerable<T> target, int id) where T : Entity
        {
            return target.SingleOrDefault(x => x.ID == id);
        }

        public static bool Has<T>(this IEnumerable<T> target, int id) where T : Entity
        {
            return target.Get(id) != null;
        }

        public static void InsertAt(this Grid target, UIElement element, int column, int row)
        {
            target.Children.Add(element);
            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
        }

        public static T GetData<T>(this IDataObject target) where T : class
        {
            return target.GetData(typeof(T)) as T;
        }

        public static void FadeIn(this UIElement targetControl, Double seconds = 1.5, Action callback = null, double delay = 0.0)
        {
            // make target visible if not already
            if (targetControl.Visibility == Visibility.Collapsed || targetControl.Visibility == Visibility.Hidden)
                targetControl.Visibility = Visibility.Visible;
            else
                targetControl.Opacity = 0;

            var fadeInAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(seconds)));
            Storyboard.SetTarget(fadeInAnimation, targetControl);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));

            var sb = new Storyboard();
            sb.Children.Add(fadeInAnimation);
            sb.BeginTime = TimeSpan.FromSeconds(delay);
            sb.Begin();

            sb.Completed += (s, e) => { if (callback != null) callback.Invoke(); };
        }

        public static void FadeOut(this UIElement targetControl, Double seconds = 1.5, Action callback = null)
        {
            var startOpacity = targetControl.Opacity;

            var fadeInAnimation = new DoubleAnimation(startOpacity, 0, new Duration(TimeSpan.FromSeconds(seconds)));
            Storyboard.SetTarget(fadeInAnimation, targetControl);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));

            var sb = new Storyboard();
            sb.Children.Add(fadeInAnimation);
            sb.Completed += (s, e) => { if (callback != null) callback.Invoke(); };

            sb.Begin();
        }
    }
}
