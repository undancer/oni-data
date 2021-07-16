using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubstanceTable : ScriptableObject, ISerializationCallbackReceiver
{
	private class SubstanceEqualityComparer : IEqualityComparer<Substance>
	{
		public bool Equals(Substance x, Substance y)
		{
			return x.elementID.Equals(y.elementID);
		}

		public int GetHashCode(Substance obj)
		{
			return obj.elementID.GetHashCode();
		}
	}

	[SerializeField]
	private List<Substance> list;

	public Material solidMaterial;

	public Material liquidMaterial;

	public List<Substance> GetList()
	{
		return list;
	}

	public Substance GetSubstance(SimHashes substance)
	{
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			if (list[i].elementID == substance)
			{
				return list[i];
			}
		}
		return null;
	}

	public void OnBeforeSerialize()
	{
		BindAnimList();
	}

	public void OnAfterDeserialize()
	{
		BindAnimList();
	}

	private void BindAnimList()
	{
		foreach (Substance item in list)
		{
			if (item.anim != null && (item.anims == null || item.anims.Length == 0))
			{
				item.anims = new KAnimFile[1];
				item.anims[0] = item.anim;
			}
		}
	}

	public void RemoveDuplicates()
	{
		list = list.Distinct(new SubstanceEqualityComparer()).ToList();
	}
}
