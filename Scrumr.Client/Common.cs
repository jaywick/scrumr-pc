using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Scrumr.Client
{
    public class InvalidInputException : Exception
    {
        public InvalidInputException(PropertyItem item)
            : base(String.Format("Please enter a valid value for {0}", item.Name)) { }
    }

    public class PropertyBag
    {
        private Dictionary<string, object> _items;

        public PropertyBag()
        {
            _items = new Dictionary<string, object>();
        }

        public void Add(string key, object value)
        {
            _items.Add(key, value);
        }

        public void Remove(string key, object value)
        {
            _items.Remove(key);
        }

        public void Clear(string key, object value)
        {
            _items.Clear();
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public T Get<T>(string key) where T : class
        {
            if (!_items.ContainsKey(key))
                return null;

            return _items[key] as T;
        }

        public T2? GetValue<T2>(string key) where T2 : struct
        {
            if (!_items.ContainsKey(key))
                return null;

            return _items[key] as T2?;
        }

        public void Set(string key, object value)
        {
            if (_items.ContainsKey(key))
                _items[key] = value;
            else if (value == null)
                _items.Remove(key);
            else
                _items.Add(key, value);
        }
    }
}
