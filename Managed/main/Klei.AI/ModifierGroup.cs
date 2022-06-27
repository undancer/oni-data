using System.Collections.Generic;

namespace Klei.AI
{
	public class ModifierGroup<T> : Resource
	{
		public List<T> modifiers = new List<T>();

		public T this[int idx] => modifiers[idx];

		public int Count => modifiers.Count;

		public IEnumerator<T> GetEnumerator()
		{
			return modifiers.GetEnumerator();
		}

		public ModifierGroup(string id, string name)
			: base(id, name)
		{
		}

		public void Add(T modifier)
		{
			modifiers.Add(modifier);
		}
	}
}
