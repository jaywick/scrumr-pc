using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    public class Table<TIdentifiable> : IEnumerable<TIdentifiable> where TIdentifiable : Identifiable
    {
        private int nextIndex = 1;
        private Dictionary<int, TIdentifiable> _items = new Dictionary<int, TIdentifiable>();

        public IEnumerator<TIdentifiable> GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        public void RemoveRange(IEnumerable<TIdentifiable> items)
        {
            foreach (var item in items.ToList())
                Remove(item);
        }

        public TIdentifiable this[int index]
        {
            get { return _items[index]; }
        }

        public void Insert(TIdentifiable item)
        {
            if (item.ID == 0)
                item.ID = nextIndex++;

            _items.Add(item.ID, item);
        }

        public void InsertRange(IEnumerable<TIdentifiable> items)
        {
            foreach (var item in items)
                Insert(item);
        }

        public void Load(IEnumerable<TIdentifiable> items, ScrumrContext context)
        {
            foreach (var item in items)
            {
                item.Context = context;
                _items.Add(item.ID, item);
            }
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(TIdentifiable item)
        {
            return _items.ContainsValue(item);
        }

        public void CopyTo(TIdentifiable[] array, int arrayIndex)
        {
            _items.Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TIdentifiable item)
        {
            return _items.Remove(item.ID);
        }
    }
}
