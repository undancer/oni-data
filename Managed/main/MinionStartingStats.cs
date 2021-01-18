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

	public Trait stressTrait;

	public Trait joyTrait;

	public Trait congenitaltrait;

	public string stickerType;

	public int voiceIdx;

	public Dictionary<string, int> StartingLevels = new Dictionary<string, int>();

	public Personality personality;

	public List<Accessory> accessories = new List<Accessory>();

	public Dictionary<SkillGroup, float> skillAptitudes = new Dictionary<SkillGroup, float>();

	public MinionStartingStats(bool is_starter_minion, string guaranteedAptitudeID = null)
	{
		if (is_starter_minion)
		{
			int idx = UnityEngine.Random.Range(0, 29);
			personality = Db.Get().Personalities[idx];
		}
		else
		{
			int idx2 = UnityEngine.Random.Range(0, 35);
			personality = Db.Get().Personalities[idx2];
		}
		voiceIdx = UnityEngine.Random.Range(0, 4);
		Name = personality.Name;
		NameStringKey = personality.nameStringKey;
		GenderStringKey = personality.genderStringKey;
		Traits.Add(Db.Get().traits.Get(MinionConfig.MINION_BASE_TRAIT_ID));
		List<ChoreGroup> disabled_chore_groups = new List<ChoreGroup>();
		GenerateAptitudes(guaranteedAptitudeID);
		int pointsDelta = GenerateTraits(is_starter_minion, disabled_chore_groups);
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

	private int GenerateTraits(bool is_starter_minion, List<ChoreGroup> disabled_chore_groups)
	{
		int statDelta = 0;
		List<string> selectedTraits = new List<string>();
		System.Random randSeed = new System.Random();
		Trait trait = (stressTrait = Db.Get().traits.Get(personality.stresstrait));
		Trait trait2 = (joyTrait = Db.Get().traits.Get(personality.joyTrait));
		stickerType = personality.stickerType;
		Trait trait3 = Db.Get().traits.Get(personality.congenitaltrait);
		if (trait3.Name == "None")
		{
			congenitaltrait = null;
		}
		else
		{
			congenitaltrait = trait3;
		}
		Func<List<DUPLICANTSTATS.TraitVal>, bool> func = delegate(List<DUPLICANTSTATS.TraitVal> traitPossibilities)
		{
			if (Traits.Count > DUPLICANTSTATS.MAX_TRAITS)
			{
				return false;
			}
			float num2 = Util.GaussianRandom();
			List<DUPLICANTSTATS.TraitVal> list = new List<DUPLICANTSTATS.TraitVal>(traitPossibilities);
			list.ShuffleSeeded(randSeed);
			list.Sort((DUPLICANTSTATS.TraitVal t1, DUPLICANTSTATS.TraitVal t2) => -t1.probability.CompareTo(t2.probability));
			foreach (DUPLICANTSTATS.TraitVal item in list)
			{
				if (!selectedTraits.Contains(item.id))
				{
					if (item.requiredNonPositiveAptitudes != null)
					{
						bool flag2 = false;
						foreach (KeyValuePair<SkillGroup, float> skillAptitude in skillAptitudes)
						{
							if (flag2)
							{
								break;
							}
							foreach (HashedString requiredNonPositiveAptitude in item.requiredNonPositiveAptitudes)
							{
								if (requiredNonPositiveAptitude == skillAptitude.Key.IdHash && skillAptitude.Value > 0f)
								{
									flag2 = true;
									break;
								}
							}
						}
						if (flag2)
						{
							continue;
						}
					}
					if (item.mutuallyExclusiveTraits != null)
					{
						bool flag3 = false;
						foreach (string item2 in selectedTraits)
						{
							flag3 = item.mutuallyExclusiveTraits.Contains(item2);
							if (flag3)
							{
								break;
							}
						}
						if (flag3)
						{
							continue;
						}
					}
					if (num2 > item.probability)
					{
						Trait trait4 = Db.Get().traits.TryGet(item.id);
						if (trait4 == null)
						{
							Debug.LogWarning("Trying to add nonexistent trait: " + item.id);
						}
						else if (!is_starter_minion || trait4.ValidStarterTrait)
						{
							selectedTraits.Add(item.id);
							statDelta += item.statBonus;
							Traits.Add(trait4);
							if (trait4.disabledChoreGroups != null)
							{
								for (int k = 0; k < trait4.disabledChoreGroups.Length; k++)
								{
									disabled_chore_groups.Add(trait4.disabledChoreGroups[k]);
								}
							}
							return true;
						}
					}
				}
			}
			return false;
		};
		int num = (is_starter_minion ? 1 : 3);
		bool flag = false;
		while (!flag)
		{
			for (int i = 0; i < num; i++)
			{
				flag = func(DUPLICANTSTATS.BADTRAITS) || flag;
			}
		}
		flag = false;
		while (!flag)
		{
			for (int j = 0; j < num; j++)
			{
				flag = func(DUPLICANTSTATS.GOODTRAITS) || flag;
			}
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
		int num = Mathf.RoundToInt(Util.GaussianRandom() * ((float)DUPLICANTSTATS.MAX_STAT_POINTS - (float)DUPLICANTSTATS.MIN_STAT_POINTS) / 2f + (float)DUPLICANTSTATS.MIN_STAT_POINTS);
		List<string> list = new List<string>(DUPLICANTSTATS.ALL_ATTRIBUTES);
		int[] randomDistribution = DUPLICANTSTATS.DISTRIBUTIONS.GetRandomDistribution();
		for (int i = 0; i < list.Count; i++)
		{
			if (!StartingLevels.ContainsKey(list[i]))
			{
				StartingLevels[list[i]] = 0;
			}
		}
		foreach (KeyValuePair<SkillGroup, float> skillAptitude in skillAptitudes)
		{
			if (skillAptitude.Key.relevantAttributes.Count > 0)
			{
				for (int j = 0; j < skillAptitude.Key.relevantAttributes.Count; j++)
				{
					StartingLevels[skillAptitude.Key.relevantAttributes[j].Id] += DUPLICANTSTATS.APTITUDE_ATTRIBUTE_BONUSES[skillAptitudes.Count - 1];
				}
			}
		}
		list.Shuffle();
		for (int k = 0; k < list.Count; k++)
		{
			string key = list[k];
			int b = randomDistribution[Mathf.Min(k, randomDistribution.Length - 1)];
			int num2 = Mathf.Min(num, b);
			if (!StartingLevels.ContainsKey(key))
			{
				StartingLevels[key] = 0;
			}
			StartingLevels[key] += num2;
			num -= num2;
		}
		if (disabled_chore_groups.Count <= 0)
		{
			return;
		}
		int num3 = 0;
		int num4 = 0;
		foreach (KeyValuePair<string, int> startingLevel in StartingLevels)
		{
			if (startingLevel.Value > num3)
			{
				num3 = startingLevel.Value;
			}
			if (startingLevel.Key == disabled_chore_groups[0].attribute.Id)
			{
				num4 = startingLevel.Value;
			}
		}
		if (num3 != num4)
		{
			return;
		}
		foreach (string item in list)
		{
			if (item != disabled_chore_groups[0].attribute.Id)
			{
				int value = 0;
				StartingLevels.TryGetValue(item, out value);
				int num5 = 0;
				if (value > 0)
				{
					num5 = 1;
				}
				StartingLevels[disabled_chore_groups[0].attribute.Id] = value - num5;
				StartingLevels[item] = num3 + num5;
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
}
