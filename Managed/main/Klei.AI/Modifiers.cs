using System.Collections.Generic;
using System.IO;
using KSerialization;
using STRINGS;
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
					attribute = Db.Get().PlantAttributes.TryGet(initialAttribute);
				}
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

		public float GetPreModifiedAttributeValue(Attribute attribute)
		{
			return AttributeInstance.GetTotalValue(attribute, GetPreModifiers(attribute));
		}

		public string GetPreModifiedAttributeFormattedValue(Attribute attribute)
		{
			float totalValue = AttributeInstance.GetTotalValue(attribute, GetPreModifiers(attribute));
			return attribute.formatter.GetFormattedValue(totalValue, attribute.formatter.DeltaTimeSlice);
		}

		public string GetPreModifiedAttributeDescription(Attribute attribute)
		{
			float totalValue = AttributeInstance.GetTotalValue(attribute, GetPreModifiers(attribute));
			return string.Format(DUPLICANTS.ATTRIBUTES.VALUE, attribute.Name, attribute.formatter.GetFormattedValue(totalValue, GameUtil.TimeSlice.None));
		}

		public string GetPreModifiedAttributeToolTip(Attribute attribute)
		{
			return attribute.formatter.GetTooltip(attribute, GetPreModifiers(attribute), null);
		}

		private List<AttributeModifier> GetPreModifiers(Attribute attribute)
		{
			List<AttributeModifier> list = new List<AttributeModifier>();
			foreach (string initialTrait in initialTraits)
			{
				Trait trait = Db.Get().traits.Get(initialTrait);
				foreach (AttributeModifier selfModifier in trait.SelfModifiers)
				{
					if (selfModifier.AttributeId == attribute.Id)
					{
						list.Add(selfModifier);
					}
				}
			}
			MutantPlant component = GetComponent<MutantPlant>();
			if (component != null && component.MutationIDs != null)
			{
				foreach (string mutationID in component.MutationIDs)
				{
					PlantMutation plantMutation = Db.Get().PlantMutations.Get(mutationID);
					foreach (AttributeModifier selfModifier2 in plantMutation.SelfModifiers)
					{
						if (selfModifier2.AttributeId == attribute.Id)
						{
							list.Add(selfModifier2);
						}
					}
				}
			}
			return list;
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
