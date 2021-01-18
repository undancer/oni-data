using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	[AddComponentMenu("KMonoBehaviour/scripts/PrefabAttributeModifiers")]
	public class PrefabAttributeModifiers : KMonoBehaviour
	{
		public List<AttributeModifier> descriptors = new List<AttributeModifier>();

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
		}

		public void AddAttributeDescriptor(AttributeModifier modifier)
		{
			descriptors.Add(modifier);
		}

		public void RemovePrefabAttribute(AttributeModifier modifier)
		{
			descriptors.Remove(modifier);
		}
	}
}
