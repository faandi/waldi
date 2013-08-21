using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Waldi.Engine
{
	public class NamedItemList<T> : IEnumerable<T> where T : INamedItem
	{
		protected HashSet<T> list;

		public T this [string name] {
			get {
				T p = this.FindByName (name);
				if (p == null)
				{
					throw new ArgumentOutOfRangeException("name");
				}
				return p;	
			}
		}

		public NamedItemList()
		{
			this.list = new HashSet<T>();
		}

		public NamedItemList(IEnumerable<T> list)
		{
			this.list = new HashSet<T>(list);
		}

		public T FindByName(string name)
		{
			return this.list.Where(p => p.Name == name).FirstOrDefault();	
		}

		public bool Contains(string name)
		{
			return this.FindByName(name) != null;	
		}

		public void Add(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException ("item");
			}
			if (this.Contains(item.Name))
			{
				throw new ArgumentException ("An item with the same name already exists in list ("+item.Name+","+this.list.Count+").", "item");
			}
			this.list.Add(item);
		}

		public void AddRange(IEnumerable<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException ("items");
			}
			foreach (T p in items)
			{
				this.Add (p);
			}
		}

		public void Remove(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException ("item");
			}
			this.list.Remove(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (T item in this.list)
			{
				yield return item;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}