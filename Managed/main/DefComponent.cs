using System;
using UnityEngine;

[Serializable]
public class DefComponent<T> where T : Component
{
	[SerializeField]
	private T cmp;

	public DefComponent(T cmp)
	{
		this.cmp = cmp;
	}

	public T Get(StateMachine.Instance smi)
	{
		T[] components = cmp.GetComponents<T>();
		int i;
		for (i = 0; i < components.Length && !((UnityEngine.Object)components[i] == (UnityEngine.Object)cmp); i++)
		{
		}
		T[] components2 = smi.gameObject.GetComponents<T>();
		return components2[i];
	}

	public static implicit operator DefComponent<T>(T cmp)
	{
		return new DefComponent<T>(cmp);
	}
}
