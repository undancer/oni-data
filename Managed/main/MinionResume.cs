using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Database;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/MinionResume")]
public class MinionResume : KMonoBehaviour, ISaveLoadable, ISim200ms
{
	public enum SkillMasteryConditions
	{
		SkillAptitude,
		StressWarning,
		UnableToLearn,
		NeedsSkillPoints,
		MissingPreviousSkill
	}

	[MyCmpReq]
	private MinionIdentity identity;

	[Serialize]
	public Dictionary<string, bool> MasteryByRoleID = new Dictionary<string, bool>();

	[Serialize]
	public Dictionary<string, bool> MasteryBySkillID = new Dictionary<string, bool>();

	[Serialize]
	public List<string> GrantedSkillIDs = new List<string>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeByRoleGroup = new Dictionary<HashedString, float>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeBySkillGroup = new Dictionary<HashedString, float>();

	[Serialize]
	private string currentRole = "NoRole";

	[Serialize]
	private string targetRole = "NoRole";

	[Serialize]
	private string currentHat;

	[Serialize]
	private string targetHat;

	private Dictionary<string, bool> ownedHats = new Dictionary<string, bool>();

	[Serialize]
	private float totalExperienceGained;

	private Notification lastSkillNotification;

	private AttributeModifier skillsMoraleExpectationModifier;

	private AttributeModifier skillsMoraleModifier;

	public float DEBUG_PassiveExperienceGained;

	public float DEBUG_ActiveExperienceGained;

	public float DEBUG_SecondsAlive;

	public MinionIdentity GetIdentity => identity;

	public float TotalExperienceGained => totalExperienceGained;

	public int TotalSkillPointsGained => CalculateTotalSkillPointsGained(TotalExperienceGained);

	public int SkillsMastered
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
			{
				if (item.Value)
				{
					num++;
				}
			}
			return num;
		}
	}

	public int AvailableSkillpoints => TotalSkillPointsGained - SkillsMastered + ((GrantedSkillIDs != null) ? GrantedSkillIDs.Count : 0);

	public string CurrentRole => currentRole;

	public string CurrentHat => currentHat;

	public string TargetHat => targetHat;

	public string TargetRole => targetRole;

	public static int CalculateTotalSkillPointsGained(float experience)
	{
		return Mathf.FloorToInt(Mathf.Pow(experience / (float)SKILLS.TARGET_SKILLS_CYCLE / 600f, 1f / SKILLS.EXPERIENCE_LEVEL_POWER) * (float)SKILLS.TARGET_SKILLS_EARNED);
	}

	[OnDeserialized]
	private void OnDeserializedMethod()
	{
		if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 7))
		{
			return;
		}
		foreach (KeyValuePair<string, bool> item in MasteryByRoleID)
		{
			if (item.Value && item.Key != "NoRole")
			{
				ForceAddSkillPoint();
			}
		}
		foreach (KeyValuePair<HashedString, float> item2 in AptitudeByRoleGroup)
		{
			AptitudeBySkillGroup[item2.Key] = item2.Value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.MinionResumes.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (GrantedSkillIDs == null)
		{
			GrantedSkillIDs = new List<string>();
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
		{
			if (item.Value && Db.Get().Skills.Get(item.Key).deprecated)
			{
				list.Add(item.Key);
			}
		}
		foreach (string item2 in list)
		{
			UnmasterSkill(item2);
		}
		foreach (KeyValuePair<string, bool> item3 in MasteryBySkillID)
		{
			if (!item3.Value)
			{
				continue;
			}
			Skill skill = Db.Get().Skills.Get(item3.Key);
			foreach (SkillPerk perk in skill.perks)
			{
				if (perk.OnRemove != null)
				{
					perk.OnRemove(this);
				}
				if (perk.OnApply != null)
				{
					perk.OnApply(this);
				}
			}
			if (!ownedHats.ContainsKey(skill.hat))
			{
				ownedHats.Add(skill.hat, value: true);
			}
		}
		UpdateExpectations();
		UpdateMorale();
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		ApplyHat(currentHat, component);
		ShowNewSkillPointNotification();
	}

	public void RestoreResume(Dictionary<string, bool> MasteryBySkillID, Dictionary<HashedString, float> AptitudeBySkillGroup, List<string> GrantedSkillIDs, float totalExperienceGained)
	{
		this.MasteryBySkillID = MasteryBySkillID;
		this.GrantedSkillIDs = ((GrantedSkillIDs != null) ? GrantedSkillIDs : new List<string>());
		this.AptitudeBySkillGroup = AptitudeBySkillGroup;
		this.totalExperienceGained = totalExperienceGained;
	}

	protected override void OnCleanUp()
	{
		Components.MinionResumes.Remove(this);
		base.OnCleanUp();
	}

	public bool HasMasteredSkill(string skillId)
	{
		if (MasteryBySkillID.ContainsKey(skillId))
		{
			return MasteryBySkillID[skillId];
		}
		return false;
	}

	public void UpdateUrge()
	{
		if (targetHat != currentHat)
		{
			if (!base.gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.LearnSkill))
			{
				base.gameObject.GetComponent<ChoreConsumer>().AddUrge(Db.Get().Urges.LearnSkill);
			}
		}
		else
		{
			base.gameObject.GetComponent<ChoreConsumer>().RemoveUrge(Db.Get().Urges.LearnSkill);
		}
	}

	public void SetHats(string current, string target)
	{
		currentHat = current;
		targetHat = target;
	}

	public void SetCurrentRole(string role_id)
	{
		currentRole = role_id;
	}

	private void ApplySkillPerks(string skillId)
	{
		foreach (SkillPerk perk in Db.Get().Skills.Get(skillId).perks)
		{
			if (perk.OnApply != null)
			{
				perk.OnApply(this);
			}
		}
	}

	private void RemoveSkillPerks(string skillId)
	{
		foreach (SkillPerk perk in Db.Get().Skills.Get(skillId).perks)
		{
			if (perk.OnRemove != null)
			{
				perk.OnRemove(this);
			}
		}
	}

	public void Sim200ms(float dt)
	{
		DEBUG_SecondsAlive += dt;
		if (!GetComponent<KPrefabID>().HasTag(GameTags.Dead))
		{
			DEBUG_PassiveExperienceGained += dt * SKILLS.PASSIVE_EXPERIENCE_PORTION;
			AddExperience(dt * SKILLS.PASSIVE_EXPERIENCE_PORTION);
		}
	}

	public bool IsAbleToLearnSkill(string skillId)
	{
		Skill skill = Db.Get().Skills.Get(skillId);
		string choreGroupID = Db.Get().SkillGroups.Get(skill.skillGroup).choreGroupID;
		if (!string.IsNullOrEmpty(choreGroupID))
		{
			Traits component = GetComponent<Traits>();
			if (component != null && component.IsChoreGroupDisabled(choreGroupID))
			{
				return false;
			}
		}
		return true;
	}

	public bool BelowMoraleExpectation(Skill skill)
	{
		float num = Db.Get().Attributes.QualityOfLife.Lookup(this).GetTotalValue();
		float totalValue = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(this).GetTotalValue();
		int moraleExpectation = skill.GetMoraleExpectation();
		if (AptitudeBySkillGroup.ContainsKey(skill.skillGroup) && AptitudeBySkillGroup[skill.skillGroup] > 0f)
		{
			num += 1f;
		}
		if (totalValue + (float)moraleExpectation > num)
		{
			return false;
		}
		return true;
	}

	public bool HasMasteredDirectlyRequiredSkillsForSkill(Skill skill)
	{
		for (int i = 0; i < skill.priorSkills.Count; i++)
		{
			if (!HasMasteredSkill(skill.priorSkills[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool HasSkillPointsRequiredForSkill(Skill skill)
	{
		if (AvailableSkillpoints < 1)
		{
			return false;
		}
		return true;
	}

	public bool HasSkillAptitude(Skill skill)
	{
		if (AptitudeBySkillGroup.ContainsKey(skill.skillGroup) && AptitudeBySkillGroup[skill.skillGroup] > 0f)
		{
			return true;
		}
		return false;
	}

	public bool HasBeenGrantedSkill(Skill skill)
	{
		if (GrantedSkillIDs == null)
		{
			return false;
		}
		if (GrantedSkillIDs.Contains(skill.Id))
		{
			return true;
		}
		return false;
	}

	public bool HasBeenGrantedSkill(string id)
	{
		if (GrantedSkillIDs == null)
		{
			return false;
		}
		if (GrantedSkillIDs.Contains(id))
		{
			return true;
		}
		return false;
	}

	public SkillMasteryConditions[] GetSkillMasteryConditions(string skillId)
	{
		List<SkillMasteryConditions> list = new List<SkillMasteryConditions>();
		Skill skill = Db.Get().Skills.Get(skillId);
		if (HasSkillAptitude(skill))
		{
			list.Add(SkillMasteryConditions.SkillAptitude);
		}
		if (!BelowMoraleExpectation(skill))
		{
			list.Add(SkillMasteryConditions.StressWarning);
		}
		if (!IsAbleToLearnSkill(skillId))
		{
			list.Add(SkillMasteryConditions.UnableToLearn);
		}
		if (!HasSkillPointsRequiredForSkill(skill))
		{
			list.Add(SkillMasteryConditions.NeedsSkillPoints);
		}
		if (!HasMasteredDirectlyRequiredSkillsForSkill(skill))
		{
			list.Add(SkillMasteryConditions.MissingPreviousSkill);
		}
		return list.ToArray();
	}

	public bool CanMasterSkill(SkillMasteryConditions[] masteryConditions)
	{
		if (Array.Exists(masteryConditions, (SkillMasteryConditions element) => element == SkillMasteryConditions.UnableToLearn || element == SkillMasteryConditions.NeedsSkillPoints || element == SkillMasteryConditions.MissingPreviousSkill))
		{
			return false;
		}
		return true;
	}

	public bool OwnsHat(string hatId)
	{
		if (ownedHats.ContainsKey(hatId))
		{
			return ownedHats[hatId];
		}
		return false;
	}

	public void SkillLearned()
	{
		if (base.gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.LearnSkill))
		{
			base.gameObject.GetComponent<ChoreConsumer>().RemoveUrge(Db.Get().Urges.LearnSkill);
		}
		foreach (string item in ownedHats.Keys.ToList())
		{
			ownedHats[item] = true;
		}
		if (targetHat != null && currentHat != targetHat)
		{
			new PutOnHatChore(this, Db.Get().ChoreTypes.SwitchHat);
		}
	}

	public void MasterSkill(string skillId)
	{
		if (!base.gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.LearnSkill))
		{
			base.gameObject.GetComponent<ChoreConsumer>().AddUrge(Db.Get().Urges.LearnSkill);
		}
		MasteryBySkillID[skillId] = true;
		ApplySkillPerks(skillId);
		UpdateExpectations();
		UpdateMorale();
		TriggerMasterSkillEvents();
		GameScheduler.Instance.Schedule("Morale Tutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Morale);
		});
		if (!ownedHats.ContainsKey(Db.Get().Skills.Get(skillId).hat))
		{
			ownedHats.Add(Db.Get().Skills.Get(skillId).hat, value: false);
		}
		if (AvailableSkillpoints == 0 && lastSkillNotification != null)
		{
			Game.Instance.GetComponent<Notifier>().Remove(lastSkillNotification);
			lastSkillNotification = null;
		}
	}

	public void UnmasterSkill(string skillId)
	{
		if (MasteryBySkillID.ContainsKey(skillId))
		{
			MasteryBySkillID.Remove(skillId);
			RemoveSkillPerks(skillId);
			UpdateExpectations();
			UpdateMorale();
			TriggerMasterSkillEvents();
		}
	}

	public void GrantSkill(string skillId)
	{
		if (GrantedSkillIDs == null)
		{
			GrantedSkillIDs = new List<string>();
		}
		if (!HasBeenGrantedSkill(skillId))
		{
			MasteryBySkillID[skillId] = true;
			ApplySkillPerks(skillId);
			GrantedSkillIDs.Add(skillId);
			UpdateExpectations();
			UpdateMorale();
			TriggerMasterSkillEvents();
			if (!ownedHats.ContainsKey(Db.Get().Skills.Get(skillId).hat))
			{
				ownedHats.Add(Db.Get().Skills.Get(skillId).hat, value: false);
			}
		}
	}

	private void TriggerMasterSkillEvents()
	{
		Trigger(540773776);
		Game.Instance.Trigger(-1523247426, this);
	}

	public void ForceAddSkillPoint()
	{
		AddExperience(CalculateNextExperienceBar(TotalSkillPointsGained) - totalExperienceGained);
	}

	public static float CalculateNextExperienceBar(int current_skill_points)
	{
		return Mathf.Pow((float)(current_skill_points + 1) / (float)SKILLS.TARGET_SKILLS_EARNED, SKILLS.EXPERIENCE_LEVEL_POWER) * (float)SKILLS.TARGET_SKILLS_CYCLE * 600f;
	}

	public static float CalculatePreviousExperienceBar(int current_skill_points)
	{
		return Mathf.Pow((float)current_skill_points / (float)SKILLS.TARGET_SKILLS_EARNED, SKILLS.EXPERIENCE_LEVEL_POWER) * (float)SKILLS.TARGET_SKILLS_CYCLE * 600f;
	}

	private void UpdateExpectations()
	{
		int num = 0;
		foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
		{
			if (item.Value && !HasBeenGrantedSkill(item.Key))
			{
				Skill skill = Db.Get().Skills.Get(item.Key);
				num += skill.tier + 1;
			}
		}
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(this);
		if (skillsMoraleExpectationModifier != null)
		{
			attributeInstance.Remove(skillsMoraleExpectationModifier);
			skillsMoraleExpectationModifier = null;
		}
		if (num > 0)
		{
			skillsMoraleExpectationModifier = new AttributeModifier(attributeInstance.Id, num, DUPLICANTS.NEEDS.QUALITYOFLIFE.EXPECTATION_MOD_NAME);
			attributeInstance.Add(skillsMoraleExpectationModifier);
		}
	}

	private void UpdateMorale()
	{
		int num = 0;
		foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
		{
			if (item.Value && !HasBeenGrantedSkill(item.Key))
			{
				Skill skill = Db.Get().Skills.Get(item.Key);
				float value = 0f;
				if (AptitudeBySkillGroup.TryGetValue(new HashedString(skill.skillGroup), out value))
				{
					num += (int)value;
				}
			}
		}
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(this);
		if (skillsMoraleModifier != null)
		{
			attributeInstance.Remove(skillsMoraleModifier);
			skillsMoraleModifier = null;
		}
		if (num > 0)
		{
			skillsMoraleModifier = new AttributeModifier(attributeInstance.Id, num, DUPLICANTS.NEEDS.QUALITYOFLIFE.APTITUDE_SKILLS_MOD_NAME);
			attributeInstance.Add(skillsMoraleModifier);
		}
	}

	private void OnSkillPointGained()
	{
		Game.Instance.Trigger(1505456302, this);
		ShowNewSkillPointNotification();
		if (PopFXManager.Instance != null)
		{
			string text = MISC.NOTIFICATIONS.SKILL_POINT_EARNED.NAME.Replace("{Duplicant}", identity.GetProperName());
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, text, base.transform, new Vector3(0f, 0.5f, 0f));
		}
		new UpgradeFX.Instance(base.gameObject.GetComponent<KMonoBehaviour>(), new Vector3(0f, 0f, -0.1f)).StartSM();
	}

	private void ShowNewSkillPointNotification()
	{
		if (AvailableSkillpoints == 1)
		{
			lastSkillNotification = new ManagementMenuNotification(Action.ManageSkills, NotificationValence.Good, identity.GetSoleOwner().gameObject.GetInstanceID().ToString(), MISC.NOTIFICATIONS.SKILL_POINT_EARNED.NAME.Replace("{Duplicant}", identity.GetProperName()), NotificationType.Good, GetSkillPointGainedTooltip, identity, expires: true, 0f, delegate
			{
				ManagementMenu.Instance.OpenSkills(identity);
			});
			GetComponent<Notifier>().Add(lastSkillNotification);
		}
	}

	private string GetSkillPointGainedTooltip(List<Notification> notifications, object data)
	{
		return MISC.NOTIFICATIONS.SKILL_POINT_EARNED.TOOLTIP.Replace("{Duplicant}", ((MinionIdentity)data).GetProperName());
	}

	public void SetAptitude(HashedString skillGroupID, float amount)
	{
		AptitudeBySkillGroup[skillGroupID] = amount;
	}

	public float GetAptitudeExperienceMultiplier(HashedString skillGroupId, float buildingFrequencyMultiplier)
	{
		float value = 0f;
		AptitudeBySkillGroup.TryGetValue(skillGroupId, out value);
		return 1f + value * SKILLS.APTITUDE_EXPERIENCE_MULTIPLIER * buildingFrequencyMultiplier;
	}

	public void AddExperience(float amount)
	{
		float num = totalExperienceGained;
		float num2 = CalculateNextExperienceBar(TotalSkillPointsGained);
		totalExperienceGained += amount;
		if (base.isSpawned && totalExperienceGained >= num2 && num < num2)
		{
			OnSkillPointGained();
		}
	}

	public void AddExperienceWithAptitude(string skillGroupId, float amount, float buildingMultiplier)
	{
		float num = amount * GetAptitudeExperienceMultiplier(skillGroupId, buildingMultiplier) * SKILLS.ACTIVE_EXPERIENCE_PORTION;
		DEBUG_ActiveExperienceGained += num;
		AddExperience(num);
	}

	public bool HasPerk(HashedString perkId)
	{
		foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
		{
			if (item.Value && Db.Get().Skills.Get(item.Key).GivesPerk(perkId))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasPerk(SkillPerk perk)
	{
		foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
		{
			if (item.Value && Db.Get().Skills.Get(item.Key).GivesPerk(perk))
			{
				return true;
			}
		}
		return false;
	}

	public void RemoveHat()
	{
		RemoveHat(GetComponent<KBatchedAnimController>());
	}

	public static void RemoveHat(KBatchedAnimController controller)
	{
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		Accessorizer component = controller.GetComponent<Accessorizer>();
		if (component != null)
		{
			Accessory accessory = component.GetAccessory(hat);
			if (accessory != null)
			{
				component.RemoveAccessory(accessory);
			}
		}
		else
		{
			controller.GetComponent<SymbolOverrideController>().TryRemoveSymbolOverride(hat.targetSymbolId, 4);
		}
		controller.SetSymbolVisiblity(hat.targetSymbolId, is_visible: false);
		controller.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, is_visible: false);
		controller.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, is_visible: true);
	}

	public static void AddHat(string hat_id, KBatchedAnimController controller)
	{
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		Accessory accessory = hat.Lookup(hat_id);
		if (accessory == null)
		{
			Debug.LogWarning("Missing hat: " + hat_id);
		}
		Accessorizer component = controller.GetComponent<Accessorizer>();
		if (component != null)
		{
			Accessory accessory2 = component.GetAccessory(Db.Get().AccessorySlots.Hat);
			if (accessory2 != null)
			{
				component.RemoveAccessory(accessory2);
			}
			if (accessory != null)
			{
				component.AddAccessory(accessory);
			}
		}
		else
		{
			SymbolOverrideController component2 = controller.GetComponent<SymbolOverrideController>();
			component2.TryRemoveSymbolOverride(hat.targetSymbolId, 4);
			component2.AddSymbolOverride(hat.targetSymbolId, accessory.symbol, 4);
		}
		controller.SetSymbolVisiblity(hat.targetSymbolId, is_visible: true);
		controller.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, is_visible: true);
		controller.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, is_visible: false);
	}

	public void ApplyTargetHat()
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		ApplyHat(targetHat, component);
		currentHat = targetHat;
		targetHat = null;
	}

	public static void ApplyHat(string hat_id, KBatchedAnimController controller)
	{
		if (hat_id.IsNullOrWhiteSpace())
		{
			RemoveHat(controller);
		}
		else
		{
			AddHat(hat_id, controller);
		}
	}

	public string GetSkillsSubtitle()
	{
		return string.Format(DUPLICANTS.NEEDS.QUALITYOFLIFE.TOTAL_SKILL_POINTS, TotalSkillPointsGained);
	}

	public static bool AnyMinionHasPerk(string perk, int worldId = -1)
	{
		foreach (MinionResume item in (worldId >= 0) ? Components.MinionResumes.GetWorldItems(worldId, checkChildWorlds: true) : Components.MinionResumes.Items)
		{
			if (item.HasPerk(perk))
			{
				return true;
			}
		}
		return false;
	}

	public static bool AnyOtherMinionHasPerk(string perk, MinionResume me)
	{
		foreach (MinionResume item in Components.MinionResumes.Items)
		{
			if (!(item == me) && item.HasPerk(perk))
			{
				return true;
			}
		}
		return false;
	}

	public void ResetSkillLevels(bool returnSkillPoints = true)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
		{
			if (item.Value)
			{
				list.Add(item.Key);
			}
		}
		foreach (string item2 in list)
		{
			UnmasterSkill(item2);
		}
	}
}
