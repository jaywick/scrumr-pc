using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Scrumr.Database
{
    public static class Extensions
    {
        public static T Get<T>(this Attribute[] target) where T : Attribute
        {
            return target.SingleOrDefault(x => x is T) as T;
        }

        public static bool Has<T>(this IEnumerable<Attribute> target) where T : Attribute
        {
            return target.Any(x => x is T);
        }

        public static bool IsOneOf(this IEnumerable<Attribute> target, params Type[] attributeTypes)
        {
            return attributeTypes.Any(a => target.Any(x => x.GetType() == a));
        }

        public static bool IsOneOf<T>(this T target, params T[] alternatives) where T : class
        {
            return alternatives.Contains(target);
        }

        public static TResult IfNotNull<T, TResult>(this T target, Func<T, TResult> selector)
        {
            if (target != null)
                return selector(target);

            return default(TResult);
        }

        public static T GetAttribute<T>(this PropertyInfo target) where T : Attribute
        {
            return Attribute.GetCustomAttributes(target).SingleOrDefault(x => x is T) as T;
        }

        public static bool HasAttribute<T>(this PropertyInfo target) where T : Attribute
        {
            return Attribute.GetCustomAttributes(target).Any(x => x is T);
        }

        public static PropertyInfo GetExpressedProperty(this LambdaExpression target)
        {
            var memberExpression = target.Body as MemberExpression;
            var unaryExpression = target.Body as UnaryExpression;

            var result = memberExpression ?? unaryExpression.IfNotNull(x => x.Operand) as MemberExpression;
            
            return result.Member as PropertyInfo;
        }
    }
}
