using System.Collections.Generic;
using System.IO;
using KSerialization;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	[AddComponentMenu("KMonoBehaviour/scripts/Modifiers")]
	public class Modifiers : KMonoBehaviour, ISaveLoadableDetails
	{
		public Amounts amounts;

		public Attributes attributes;

		public Sicknesses sicknesses;

		public List<string> initialTraits = new List<string>();

		public List<string> initialAmounts = new List<string>();

		public List<string> initialAttributes = new List<string>();

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			amounts = new Amounts(base.gameObject);
			sicknesses = new Sicknesses(base.gameObject);
			attributes = new Attributes(base.gameObject);
			foreach (string initialAmount in initialAmounts)
			{
				amounts.Add(new AmountInstance(Db.Get().Amounts.Get(initialAmount), base.gameObject));
			}
			foreach (string initialAttribute in initialAttributes)
			{
				Attribute attribute = Db.Get().CritterAttributes.TryGet(initialAttribute);
				if (attribute == null)
				{
					attribute = Db.Get().Attributes.TryGet(initialAttribute);
				}
				DebugUtil.Assert(attribute != null, "Couldn't find an attribute for id", initialAttribute);
				attributes.Add(attribute);
			}
			Traits component = GetComponent<Traits>();
			if (initialTraits == null)
			{
				return;
			}
			foreach (string initialTrait in initialTraits)
			{
				Trait trait = Db.Get().traits.Get(initialTrait);
				component.Add(trait);
			}
		}

		public void Serialize(BinaryWriter writer)
		{
			OnSerialize(writer);
		}

		public void Deserialize(IReader reader)
		{
			OnDeserialize(reader);
		}

		public virtual void OnSerialize(BinaryWriter writer)
		{
			amounts.Serialize(writer);
			sicknesses.Serialize(writer);
		}

		public virtual void OnDeserialize(IReader reader)
		{
			amounts.Deserialize(reader);
			sicknesses.Deserialize(reader);
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			if (amounts != null)
			{
				amounts.Cleanup();
			}
		}
	}
}
