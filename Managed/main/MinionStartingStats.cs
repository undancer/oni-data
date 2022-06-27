using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using TUNING;
using UnityEngine;

public class MinionStartingStats : ITelepadDeliverable
{
	public string Name;

	public string NameStringKey;

	public string GenderStringKey;

	public List<Trait> Traits = new List<Trait>();

	public int rarityBalance;

	public Trait stressTrait;

	public Trait joyTrait;

	public Trait congenitaltrait;

	public string stickerType;

	public int voiceIdx;

	public Dictionary<string, int> StartingLevels = new Dictionary<string, int>();

	public Personality personality;

	public List<Accessory> accessories = new List<Accessory>();

	public bool IsValid;

	public Dictionary<SkillGroup, float> skillAptitudes = new Dictionary<SkillGroup, float>();

	public MinionStartingStats(bool is_starter_minion, string guaranteedAptitudeID = null, string guaranteedTraitID = null)
	{
		if (is_starter_minion)
		{
			int index = UnityEngine.Random.Range(0, Db.Get().Personalities.GetStartingPersonalities().Count);
			personality = Db.Get().Personalities.GetStartingPersonalities()[index];
		}
		else
		{
			int idx = UnityEngine.Random.Range(0, Db.Get().Personalities.Count);
			personality = Db.Get().Personalities[idx];
		}
		voiceIdx = UnityEngine.Random.Range(0, 4);
		Name = personality.Name;
		NameStringKey = personality.nameStringKey;
		GenderStringKey = personality.genderStringKey;
		Traits.Add(Db.Get().traits.Get(MinionConfig.MINION_BASE_TRAIT_ID));
		List<ChoreGroup> disabled_chore_groups = new List<ChoreGroup>();
		GenerateAptitudes(guaranteedAptitudeID);
		int pointsDelta = GenerateTraits(is_starter_minion, disabled_chore_groups, guaranteedAptitudeID, guaranteedTraitID);
		GenerateAttributes(pointsDelta, disabled_chore_groups);
		KCompBuilder.BodyData bodyData = CreateBodyData(personality);
		foreach (AccessorySlot resource in Db.Get().AccessorySlots.resources)
		{
			if (resource.accessories.Count == 0)
			{
				continue;
			}
			Accessory accessory = null;
			if (resource == Db.Get().AccessorySlots.HeadShape)
			{
				accessory = resource.Lookup(bodyData.headShape);
				if (accessory == null)
				{
					personality.headShape = 0;
				}
			}
			else if (resource == Db.Get().AccessorySlots.Mouth)
			{
				accessory = resource.Lookup(bodyData.mouth);
				if (accessory == null)
				{
					personality.mouth = 0;
				}
			}
			else if (resource == Db.Get().AccessorySlots.Eyes)
			{
				accessory = resource.Lookup(bodyData.eyes);
				if (accessory == null)
				{
					personality.eyes = 0;
				}
			}
			else if (resource == Db.Get().AccessorySlots.Hair)
			{
				accessory = resource.Lookup(bodyData.hair);
				if (accessory == null)
				{
					personality.hair = 0;
				}
			}
			else if (resource == Db.Get().AccessorySlots.HatHair || resource == Db.Get().AccessorySlots.HairAlways || resource == Db.Get().AccessorySlots.Hat)
			{
				accessory = resource.accessories[0];
			}
			else if (resource == Db.Get().AccessorySlots.Body)
			{
				accessory = resource.Lookup(bodyData.body);
				if (accessory == null)
				{
					personality.body = 0;
				}
			}
			else if (resource == Db.Get().AccessorySlots.Arm)
			{
				accessory = resource.Lookup(bodyData.arms);
			}
			if (accessory == null)
			{
				accessory = resource.accessories[0];
			}
			accessories.Add(accessory);
		}
	}

	private int GenerateTraits(bool is_starter_minion, List<ChoreGroup> disabled_chore_groups, string guaranteedAptitudeID = null, string guaranteedTraitID = null)
	{
		int statDelta = 0;
		List<string> selectedTraits = new List<string>();
		KRandom randSeed = new KRandom();
		Trait trait = (stressTrait = Db.Get().traits.Get(personality.stresstrait));
		Trait trait2 = (joyTrait = Db.Get().traits.Get(personality.joyTrait));
		stickerType = personality.stickerType;
		Trait trait3 = Db.Get().traits.TryGet(personality.congenitaltrait);
		if (trait3 == null || trait3.Name == "None")
		{
			congenitaltrait = null;
		}
		else
		{
			congenitaltrait = trait3;
		}
		Func<List<DUPLICANTSTATS.TraitVal>, bool, bool> func = delegate(List<DUPLICANTSTATS.TraitVal> traitPossibilities, bool positiveTrait)
		{
			if (Traits.Count > DUPLICANTSTATS.MAX_TRAITS)
			{
				return false;
			}
			Mathf.Abs(Util.GaussianRandom());
			int num6 = traitPossibilities.Count;
			int num7;
			if (!positiveTrait)
			{
				if (DUPLICANTSTATS.rarityDeckActive.Count < 1)
				{
					DUPLICANTSTATS.rarityDeckActive.AddRange(DUPLICANTSTATS.RARITY_DECK);
				}
				if (DUPLICANTSTATS.rarityDeckActive.Count == DUPLICANTSTATS.RARITY_DECK.Count)
				{
					DUPLICANTSTATS.rarityDeckActive.ShuffleSeeded(randSeed);
				}
				num7 = DUPLICANTSTATS.rarityDeckActive[DUPLICANTSTATS.rarityDeckActive.Count - 1];
				DUPLICANTSTATS.rarityDeckActive.RemoveAt(DUPLICANTSTATS.rarityDeckActive.Count - 1);
			}
			else
			{
				List<int> list = new List<int>();
				if (is_starter_minion)
				{
					list.Add(rarityBalance - 1);
					list.Add(rarityBalance);
					list.Add(rarityBalance);
					list.Add(rarityBalance + 1);
				}
				else
				{
					list.Add(rarityBalance - 2);
					list.Add(rarityBalance - 1);
					list.Add(rarityBalance);
					list.Add(rarityBalance + 1);
					list.Add(rarityBalance + 2);
				}
				list.ShuffleSeeded(randSeed);
				num7 = list[0];
				num7 = Mathf.Max(DUPLICANTSTATS.RARITY_COMMON, num7);
				num7 = Mathf.Min(DUPLICANTSTATS.RARITY_LEGENDARY, num7);
			}
			List<DUPLICANTSTATS.TraitVal> list2 = new List<DUPLICANTSTATS.TraitVal>(traitPossibilities);
			for (int num8 = list2.Count - 1; num8 > -1; num8--)
			{
				if (list2[num8].rarity != num7)
				{
					list2.RemoveAt(num8);
					num6--;
				}
			}
			list2.ShuffleSeeded(randSeed);
			foreach (DUPLICANTSTATS.TraitVal item in list2)
			{
				if (!DlcManager.IsContentActive(item.dlcId))
				{
					num6--;
				}
				else if (selectedTraits.Contains(item.id))
				{
					num6--;
				}
				else
				{
					Trait trait5 = Db.Get().traits.TryGet(item.id);
					if (trait5 == null)
					{
						Debug.LogWarning("Trying to add nonexistent trait: " + item.id);
						num6--;
					}
					else if (is_starter_minion && !trait5.ValidStarterTrait)
					{
						num6--;
					}
					else if (item.doNotGenerateTrait)
					{
						num6--;
					}
					else if (AreTraitAndAptitudesExclusive(item, skillAptitudes))
					{
						num6--;
					}
					else if (is_starter_minion && guaranteedAptitudeID != null && AreTraitAndArchetypeExclusive(item, guaranteedAptitudeID))
					{
						num6--;
					}
					else
					{
						if (!AreTraitsMutuallyExclusive(item, selectedTraits))
						{
							selectedTraits.Add(item.id);
							statDelta += item.statBonus;
							rarityBalance += (positiveTrait ? (-item.rarity) : item.rarity);
							Traits.Add(trait5);
							if (trait5.disabledChoreGroups != null)
							{
								for (int j = 0; j < trait5.disabledChoreGroups.Length; j++)
								{
									disabled_chore_groups.Add(trait5.disabledChoreGroups[j]);
								}
							}
							return true;
						}
						num6--;
					}
				}
			}
			return false;
		};
		int num = 0;
		int num2 = 0;
		if (is_starter_minion)
		{
			num = 1;
			num2 = 1;
		}
		else
		{
			if (DUPLICANTSTATS.podTraitConfigurationsActive.Count < 1)
			{
				DUPLICANTSTATS.podTraitConfigurationsActive.AddRange(DUPLICANTSTATS.POD_TRAIT_CONFIGURATIONS_DECK);
			}
			if (DUPLICANTSTATS.podTraitConfigurationsActive.Count == DUPLICANTSTATS.POD_TRAIT_CONFIGURATIONS_DECK.Count)
			{
				DUPLICANTSTATS.podTraitConfigurationsActive.ShuffleSeeded(randSeed);
			}
			num = DUPLICANTSTATS.podTraitConfigurationsActive[DUPLICANTSTATS.podTraitConfigurationsActive.Count - 1].first;
			num2 = DUPLICANTSTATS.podTraitConfigurationsActive[DUPLICANTSTATS.podTraitConfigurationsActive.Count - 1].second;
			DUPLICANTSTATS.podTraitConfigurationsActive.RemoveAt(DUPLICANTSTATS.podTraitConfigurationsActive.Count - 1);
		}
		int num3 = 0;
		int num4 = 0;
		int num5 = (num2 + num) * 4;
		if (!string.IsNullOrEmpty(guaranteedTraitID))
		{
			DUPLICANTSTATS.TraitVal traitVal = DUPLICANTSTATS.GetTraitVal(guaranteedTraitID);
			if (traitVal.id == guaranteedTraitID)
			{
				Trait trait4 = Db.Get().traits.TryGet(traitVal.id);
				bool positiveTrait2 = trait4.PositiveTrait;
				selectedTraits.Add(traitVal.id);
				statDelta += traitVal.statBonus;
				rarityBalance += (positiveTrait2 ? (-traitVal.rarity) : traitVal.rarity);
				Traits.Add(trait4);
				if (trait4.disabledChoreGroups != null)
				{
					for (int i = 0; i < trait4.disabledChoreGroups.Length; i++)
					{
						disabled_chore_groups.Add(trait4.disabledChoreGroups[i]);
					}
				}
				if (positiveTrait2)
				{
					num3++;
				}
				else
				{
					num4++;
				}
			}
		}
		while (num5 > 0 && (num4 < num2 || num3 < num))
		{
			if (num4 < num2 && func(DUPLICANTSTATS.BADTRAITS, arg2: false))
			{
				num4++;
			}
			if (num3 < num && func(DUPLICANTSTATS.GOODTRAITS, arg2: true))
			{
				num3++;
			}
			num5--;
		}
		if (num5 > 0)
		{
			IsValid = true;
		}
		return statDelta;
	}

	private void GenerateAptitudes(string guaranteedAptitudeID = null)
	{
		int num = UnityEngine.Random.Range(1, 4);
		List<SkillGroup> list = new List<SkillGroup>(Db.Get().SkillGroups.resources);
		list.Shuffle();
		if (guaranteedAptitudeID != null)
		{
			skillAptitudes.Add(Db.Get().SkillGroups.Get(guaranteedAptitudeID), DUPLICANTSTATS.APTITUDE_BONUS);
			list.Remove(Db.Get().SkillGroups.Get(guaranteedAptitudeID));
			num--;
		}
		for (int i = 0; i < num; i++)
		{
			skillAptitudes.Add(list[i], DUPLICANTSTATS.APTITUDE_BONUS);
		}
	}

	private void GenerateAttributes(int pointsDelta, List<ChoreGroup> disabled_chore_groups)
	{
		List<string> list = new List<string>(DUPLICANTSTATS.ALL_ATTRIBUTES);
		for (int i = 0; i < list.Count; i++)
		{
			if (!StartingLevels.ContainsKey(list[i]))
			{
				StartingLevels[list[i]] = 0;
			}
		}
		foreach (KeyValuePair<SkillGroup, float> skillAptitude in skillAptitudes)
		{
			if (skillAptitude.Key.relevantAttributes.Count <= 0)
			{
				continue;
			}
			for (int j = 0; j < skillAptitude.Key.relevantAttributes.Count; j++)
			{
				if (!StartingLevels.ContainsKey(skillAptitude.Key.relevantAttributes[j].Id))
				{
					Debug.LogError("Need to add " + skillAptitude.Key.relevantAttributes[j].Id + " to TUNING.DUPLICANTSTATS.ALL_ATTRIBUTES");
				}
				StartingLevels[skillAptitude.Key.relevantAttributes[j].Id] += DUPLICANTSTATS.APTITUDE_ATTRIBUTE_BONUSES[skillAptitudes.Count - 1];
			}
		}
		List<SkillGroup> list2 = new List<SkillGroup>(skillAptitudes.Keys);
		if (pointsDelta > 0)
		{
			for (int num = pointsDelta; num > 0; num--)
			{
				list2.Shuffle();
				for (int k = 0; k < list2[0].relevantAttributes.Count; k++)
				{
					StartingLevels[list2[0].relevantAttributes[k].Id]++;
				}
			}
		}
		if (disabled_chore_groups.Count <= 0)
		{
			return;
		}
		int num2 = 0;
		int num3 = 0;
		foreach (KeyValuePair<string, int> startingLevel in StartingLevels)
		{
			if (startingLevel.Value > num2)
			{
				num2 = startingLevel.Value;
			}
			if (startingLevel.Key == disabled_chore_groups[0].attribute.Id)
			{
				num3 = startingLevel.Value;
			}
		}
		if (num2 != num3)
		{
			return;
		}
		foreach (string item in list)
		{
			if (item != disabled_chore_groups[0].attribute.Id)
			{
				int value = 0;
				StartingLevels.TryGetValue(item, out value);
				int num4 = 0;
				if (value > 0)
				{
					num4 = 1;
				}
				StartingLevels[disabled_chore_groups[0].attribute.Id] = value - num4;
				StartingLevels[item] = num2 + num4;
				break;
			}
		}
	}

	public void Apply(GameObject go)
	{
		MinionIdentity component = go.GetComponent<MinionIdentity>();
		component.SetName(Name);
		component.nameStringKey = NameStringKey;
		component.genderStringKey = GenderStringKey;
		ApplyTraits(go);
		ApplyRace(go);
		ApplyAptitudes(go);
		ApplyAccessories(go);
		ApplyExperience(go);
	}

	public void ApplyExperience(GameObject go)
	{
		foreach (KeyValuePair<string, int> startingLevel in StartingLevels)
		{
			go.GetComponent<AttributeLevels>().SetLevel(startingLevel.Key, startingLevel.Value);
		}
	}

	public void ApplyAccessories(GameObject go)
	{
		Accessorizer component = go.GetComponent<Accessorizer>();
		foreach (Accessory accessory in accessories)
		{
			component.AddAccessory(accessory);
		}
	}

	public void ApplyRace(GameObject go)
	{
		go.GetComponent<MinionIdentity>().voiceIdx = voiceIdx;
	}

	public static KCompBuilder.BodyData CreateBodyData(Personality p)
	{
		KCompBuilder.BodyData result = default(KCompBuilder.BodyData);
		result.eyes = HashCache.Get().Add($"eyes_{p.eyes:000}");
		result.hair = HashCache.Get().Add($"hair_{p.hair:000}");
		result.headShape = HashCache.Get().Add($"headshape_{p.headShape:000}");
		result.mouth = HashCache.Get().Add($"mouth_{p.mouth:000}");
		result.neck = HashCache.Get().Add($"neck_{p.neck:000}");
		result.arms = HashCache.Get().Add($"arm_{p.body:000}");
		result.body = HashCache.Get().Add($"body_{p.body:000}");
		result.hat = HashedString.Invalid;
		result.faceFX = HashedString.Invalid;
		return result;
	}

	public void ApplyAptitudes(GameObject go)
	{
		MinionResume component = go.GetComponent<MinionResume>();
		foreach (KeyValuePair<SkillGroup, float> skillAptitude in skillAptitudes)
		{
			component.SetAptitude(skillAptitude.Key.Id, skillAptitude.Value);
		}
	}

	public void ApplyTraits(GameObject go)
	{
		Traits component = go.GetComponent<Traits>();
		component.Clear();
		foreach (Trait trait in Traits)
		{
			component.Add(trait);
		}
		component.Add(stressTrait);
		if (congenitaltrait != null)
		{
			component.Add(congenitaltrait);
		}
		component.Add(joyTrait);
		go.GetComponent<MinionIdentity>().SetStickerType(stickerType);
		go.GetComponent<MinionIdentity>().SetName(Name);
		go.GetComponent<MinionIdentity>().SetGender(GenderStringKey);
	}

	public GameObject Deliver(Vector3 location)
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID));
		gameObject.SetActive(value: true);
		gameObject.transform.SetLocalPosition(location);
		Apply(gameObject);
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		new EmoteChore(gameObject.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", Telepad.PortalBirthAnim, null);
		return gameObject;
	}

	private bool AreTraitAndAptitudesExclusive(DUPLICANTSTATS.TraitVal traitVal, Dictionary<SkillGroup, float> aptitudes)
	{
		if (traitVal.mutuallyExclusiveAptitudes == null)
		{
			return false;
		}
		foreach (KeyValuePair<SkillGroup, float> skillAptitude in skillAptitudes)
		{
			foreach (HashedString mutuallyExclusiveAptitude in traitVal.mutuallyExclusiveAptitudes)
			{
				if (mutuallyExclusiveAptitude == skillAptitude.Key.IdHash && skillAptitude.Value > 0f)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool AreTraitAndArchetypeExclusive(DUPLICANTSTATS.TraitVal traitVal, string guaranteedAptitudeID)
	{
		if (!DUPLICANTSTATS.ARCHETYPE_TRAIT_EXCLUSIONS.ContainsKey(guaranteedAptitudeID))
		{
			Debug.LogError("Need to add attribute " + guaranteedAptitudeID + " to ARCHETYPE_TRAIT_EXCLUSIONS");
		}
		foreach (string item in DUPLICANTSTATS.ARCHETYPE_TRAIT_EXCLUSIONS[guaranteedAptitudeID])
		{
			if (item == traitVal.id)
			{
				return true;
			}
		}
		return false;
	}

	private bool AreTraitsMutuallyExclusive(DUPLICANTSTATS.TraitVal traitVal, List<string> selectedTraits)
	{
		foreach (string selectedTrait in selectedTraits)
		{
			foreach (DUPLICANTSTATS.TraitVal gOODTRAIT in DUPLICANTSTATS.GOODTRAITS)
			{
				if (selectedTrait == gOODTRAIT.id && gOODTRAIT.mutuallyExclusiveTraits != null && gOODTRAIT.mutuallyExclusiveTraits.Contains(traitVal.id))
				{
					return true;
				}
			}
			foreach (DUPLICANTSTATS.TraitVal bADTRAIT in DUPLICANTSTATS.BADTRAITS)
			{
				if (selectedTrait == bADTRAIT.id && bADTRAIT.mutuallyExclusiveTraits != null && bADTRAIT.mutuallyExclusiveTraits.Contains(traitVal.id))
				{
					return true;
				}
			}
			foreach (DUPLICANTSTATS.TraitVal cONGENITALTRAIT in DUPLICANTSTATS.CONGENITALTRAITS)
			{
				if (selectedTrait == cONGENITALTRAIT.id && cONGENITALTRAIT.mutuallyExclusiveTraits != null && cONGENITALTRAIT.mutuallyExclusiveTraits.Contains(traitVal.id))
				{
					return true;
				}
			}
			foreach (DUPLICANTSTATS.TraitVal sPECIALTRAIT in DUPLICANTSTATS.SPECIALTRAITS)
			{
				if (selectedTrait == sPECIALTRAIT.id && sPECIALTRAIT.mutuallyExclusiveTraits != null && sPECIALTRAIT.mutuallyExclusiveTraits.Contains(traitVal.id))
				{
					return true;
				}
			}
			if (traitVal.mutuallyExclusiveTraits != null && traitVal.mutuallyExclusiveTraits.Contains(selectedTrait))
			{
				return true;
			}
		}
		return false;
	}
}
