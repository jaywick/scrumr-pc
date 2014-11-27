using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client.Tests
{
    class TestHelper
    {
        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            var result = type.GetProperty(propertyName);

            if (result == null)
                throw new InvalidOperationException(String.Format("The type {0} does not contain any properties with the name {1}", type.Name, propertyName));

            return result;
        }
    }
}
