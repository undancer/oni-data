using System;
using System.Collections.Generic;
using KSerialization;
using TUNING;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	[AddComponentMenu("KMonoBehaviour/scripts/Traits")]
	public class Traits : KMonoBehaviour, ISaveLoadable
	{
		public List<Trait> TraitList = new List<Trait>();

		[Serialize]
		private List<string> TraitIds = new List<string>();

		public List<string> GetTraitIds()
		{
			return TraitIds;
		}

		public void SetTraitIds(List<string> traits)
		{
			TraitIds = traits;
		}

		protected override void OnSpawn()
		{
			foreach (string traitId in TraitIds)
			{
				if (Db.Get().traits.Exists(traitId))
				{
					Trait trait = Db.Get().traits.Get(traitId);
					AddInternal(trait);
				}
			}
			if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 15))
			{
				return;
			}
			List<DUPLICANTSTATS.TraitVal> jOYTRAITS = DUPLICANTSTATS.JOYTRAITS;
			if (!GetComponent<MinionIdentity>())
			{
				return;
			}
			bool flag = true;
			foreach (DUPLICANTSTATS.TraitVal item in jOYTRAITS)
			{
				if (HasTrait(item.id))
				{
					flag = false;
				}
			}
			if (flag)
			{
				DUPLICANTSTATS.TraitVal random = jOYTRAITS.GetRandom();
				Trait trait2 = Db.Get().traits.Get(random.id);
				Add(trait2);
			}
		}

		private void AddInternal(Trait trait)
		{
			if (!HasTrait(trait))
			{
				TraitList.Add(trait);
				trait.AddTo(this.GetAttributes());
				if (trait.OnAddTrait != null)
				{
					trait.OnAddTrait(base.gameObject);
				}
			}
		}

		public void Add(Trait trait)
		{
			DebugUtil.Assert(IsInitialized() || GetComponent<Modifiers>().IsInitialized(), "Tried adding a trait on a prefab, use Modifiers.initialTraits instead!", trait.Name, base.gameObject.name);
			if (trait.ShouldSave)
			{
				TraitIds.Add(trait.Id);
			}
			AddInternal(trait);
		}

		public bool HasTrait(string trait_id)
		{
			bool result = false;
			foreach (Trait trait in TraitList)
			{
				if (trait.Id == trait_id)
				{
					return true;
				}
			}
			return result;
		}

		public bool HasTrait(Trait trait)
		{
			foreach (Trait trait2 in TraitList)
			{
				if (trait2 == trait)
				{
					return true;
				}
			}
			return false;
		}

		public void Clear()
		{
			while (TraitList.Count > 0)
			{
				Remove(TraitList[0]);
			}
		}

		public void Remove(Trait trait)
		{
			for (int i = 0; i < TraitList.Count; i++)
			{
				if (TraitList[i] == trait)
				{
					TraitList.RemoveAt(i);
					TraitIds.Remove(trait.Id);
					trait.RemoveFrom(this.GetAttributes());
					break;
				}
			}
		}

		public bool IsEffectIgnored(Effect effect)
		{
			foreach (Trait trait in TraitList)
			{
				if (trait.ignoredEffects != null && Array.IndexOf(trait.ignoredEffects, effect.Id) != -1)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsChoreGroupDisabled(ChoreGroup choreGroup)
		{
			Trait disablingTrait;
			return IsChoreGroupDisabled(choreGroup, out disablingTrait);
		}

		public bool IsChoreGroupDisabled(ChoreGroup choreGroup, out Trait disablingTrait)
		{
			return IsChoreGroupDisabled(choreGroup.IdHash, out disablingTrait);
		}

		public bool IsChoreGroupDisabled(HashedString choreGroupId)
		{
			Trait disablingTrait;
			return IsChoreGroupDisabled(choreGroupId, out disablingTrait);
		}

		public bool IsChoreGroupDisabled(HashedString choreGroupId, out Trait disablingTrait)
		{
			foreach (Trait trait in TraitList)
			{
				if (trait.disabledChoreGroups == null)
				{
					continue;
				}
				ChoreGroup[] disabledChoreGroups = trait.disabledChoreGroups;
				for (int i = 0; i < disabledChoreGroups.Length; i++)
				{
					if (disabledChoreGroups[i].IdHash == choreGroupId)
					{
						disablingTrait = trait;
						return true;
					}
				}
			}
			disablingTrait = null;
			return false;
		}
	}
}
