using System;
using System.Collections;
using System.Collections.Generic;

public class BinaryHeap<T> : IEnumerable<T>, IEnumerable
{
	private IComparer<T> Comparer;

	private List<T> Items = new List<T>();

	public int Count => Items.Count;

	public BinaryHeap()
		: this((IComparer<T>)Comparer<T>.Default)
	{
	}

	public BinaryHeap(IComparer<T> comp)
	{
		Comparer = comp;
	}

	public void Clear()
	{
		Items.Clear();
	}

	public void TrimExcess()
	{
		Items.TrimExcess();
	}

	public void Insert(T newItem)
	{
		int num = Count;
		Items.Add(newItem);
		while (num > 0 && Comparer.Compare(Items[(num - 1) / 2], newItem) > 0)
		{
			Items[num] = Items[(num - 1) / 2];
			num = (num - 1) / 2;
		}
		Items[num] = newItem;
	}

	public T Peek()
	{
		if (Items.Count == 0)
		{
			throw new InvalidOperationException("The heap is empty.");
		}
		return Items[0];
	}

	public T RemoveRoot()
	{
		if (Items.Count == 0)
		{
			throw new InvalidOperationException("The heap is empty.");
		}
		T result = Items[0];
		T val = Items[Items.Count - 1];
		Items.RemoveAt(Items.Count - 1);
		if (Items.Count > 0)
		{
			int num = 0;
			while (num < Items.Count / 2)
			{
				int num2 = 2 * num + 1;
				if (num2 < Items.Count - 1 && Comparer.Compare(Items[num2], Items[num2 + 1]) > 0)
				{
					num2++;
				}
				if (Comparer.Compare(Items[num2], val) >= 0)
				{
					break;
				}
				Items[num] = Items[num2];
				num = num2;
			}
			Items[num] = val;
		}
		return result;
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		foreach (T item in Items)
		{
			yield return item;
		}
	}

	public IEnumerator GetEnumerator()
	{
		return GetEnumerator();
	}
}
