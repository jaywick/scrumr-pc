using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    public class Table<TIdentifiable> : IEnumerable<TIdentifiable> where TIdentifiable : Identifiable
    {
        private Dictionary<Guid, TIdentifiable> Items { get; set; }

        public Table()
        {
            Items = new Dictionary<Guid, TIdentifiable>();
        }

        public IEnumerator<TIdentifiable> GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }

        public void RemoveRange(IEnumerable<TIdentifiable> items)
        {
            foreach (var item in items.ToList())
                Remove(item);
        }

        public TIdentifiable this[Guid index]
        {
            get { return Items[index]; }
            set { Items[index] = value; }
        }

        public void Insert(TIdentifiable item)
        {
            if (item.ID == Guid.Empty)
                item.ID = Guid.NewGuid();

            Items.Add(item.ID, item);
        }

        public void InsertRange(IEnumerable<TIdentifiable> items)
        {
            foreach (var item in items)
                Insert(item);
        }

        public void Load(ScrumrContext context, IEnumerable<TIdentifiable> items)
        {
            foreach (var item in items)
            {
                item.Context = context;
                Items.Add(item.ID, item);
            }
        }

        public void Clear()
        {
            Items.Clear();
        }

        public bool Contains(TIdentifiable item)
        {
            return Items.ContainsValue(item);
        }

        public void CopyTo(TIdentifiable[] array, int arrayIndex)
        {
            Items.Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TIdentifiable item)
        {
            return Items.Remove(item.ID);
        }
    }
}
