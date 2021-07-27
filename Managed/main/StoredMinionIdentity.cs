using System.Collections.Generic;
using System.Runtime.Serialization;
using Database;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/StoredMinionIdentity")]
public class StoredMinionIdentity : KMonoBehaviour, ISaveLoadable, IAssignableIdentity, IListableOption, IPersonalPriorityManager
{
	[Serialize]
	public string storedName;

	[Serialize]
	public string gender;

	[Serialize]
	[ReadOnly]
	public float arrivalTime;

	[Serialize]
	public int voiceIdx;

	[Serialize]
	public KCompBuilder.BodyData bodyData;

	[Serialize]
	public List<Ref<KPrefabID>> assignedItems;

	[Serialize]
	public List<Ref<KPrefabID>> equippedItems;

	[Serialize]
	public List<string> traitIDs;

	[Serialize]
	public List<ResourceRef<Accessory>> accessories;

	[Serialize]
	public List<Tag> forbiddenTags;

	[Serialize]
	public Ref<MinionAssignablesProxy> assignableProxy;

	[Serialize]
	public Dictionary<string, bool> MasteryByRoleID = new Dictionary<string, bool>();

	[Serialize]
	public Dictionary<string, bool> MasteryBySkillID = new Dictionary<string, bool>();

	[Serialize]
	public List<string> grantedSkillIDs = new List<string>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeByRoleGroup = new Dictionary<HashedString, float>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeBySkillGroup = new Dictionary<HashedString, float>();

	[Serialize]
	public float TotalExperienceGained;

	[Serialize]
	public string currentHat;

	[Serialize]
	public string targetHat;

	[Serialize]
	public Dictionary<HashedString, ChoreConsumer.PriorityInfo> choreGroupPriorities = new Dictionary<HashedString, ChoreConsumer.PriorityInfo>();

	[Serialize]
	public List<AttributeLevels.LevelSaveLoad> attributeLevels;

	[Serialize]
	public Dictionary<string, float> savedAttributeValues;

	public MinionModifiers minionModifiers;

	[Serialize]
	public string genderStringKey { get; set; }

	[Serialize]
	public string nameStringKey { get; set; }

	[OnDeserialized]
	private void OnDeserializedMethod()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 7))
		{
			int num = 0;
			foreach (KeyValuePair<string, bool> item in MasteryByRoleID)
			{
				if (item.Value && item.Key != "NoRole")
				{
					num++;
				}
			}
			TotalExperienceGained = MinionResume.CalculatePreviousExperienceBar(num);
			foreach (KeyValuePair<HashedString, float> item2 in AptitudeByRoleGroup)
			{
				AptitudeBySkillGroup[item2.Key] = item2.Value;
			}
		}
		OnDeserializeModifiers();
	}

	public bool HasPerk(SkillPerk perk)
	{
		foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
		{
			if (item.Value && Db.Get().Skills.Get(item.Key).perks.Contains(perk))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasMasteredSkill(string skillId)
	{
		if (MasteryBySkillID.ContainsKey(skillId))
		{
			return MasteryBySkillID[skillId];
		}
		return false;
	}

	protected override void OnPrefabInit()
	{
		assignableProxy = new Ref<MinionAssignablesProxy>();
		minionModifiers = GetComponent<MinionModifiers>();
		savedAttributeValues = new Dictionary<string, float>();
	}

	[OnSerializing]
	private void OnSerialize()
	{
		savedAttributeValues.Clear();
		foreach (AttributeInstance attribute in minionModifiers.attributes)
		{
			savedAttributeValues.Add(attribute.Attribute.Id, attribute.GetTotalValue());
		}
	}

	protected override void OnSpawn()
	{
		MinionConfig.AddMinionAmounts(minionModifiers);
		MinionConfig.AddMinionTraits(DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME, minionModifiers);
		ValidateProxy();
		CleanupLimboMinions();
	}

	public void OnHardDelete()
	{
		if (assignableProxy.Get() != null)
		{
			Util.KDestroyGameObject(assignableProxy.Get().gameObject);
		}
		ScheduleManager.Instance.OnStoredDupeDestroyed(this);
		Components.StoredMinionIdentities.Remove(this);
	}

	private void OnDeserializeModifiers()
	{
		foreach (KeyValuePair<string, float> savedAttributeValue in savedAttributeValues)
		{
			Attribute attribute = Db.Get().Attributes.TryGet(savedAttributeValue.Key);
			if (attribute == null)
			{
				attribute = Db.Get().BuildingAttributes.TryGet(savedAttributeValue.Key);
			}
			if (attribute != null)
			{
				if (minionModifiers.attributes.Get(attribute.Id) != null)
				{
					minionModifiers.attributes.Get(attribute.Id).Modifiers.Clear();
					minionModifiers.attributes.Get(attribute.Id).ClearModifiers();
				}
				else
				{
					minionModifiers.attributes.Add(attribute);
				}
				minionModifiers.attributes.Add(new AttributeModifier(attribute.Id, savedAttributeValue.Value, () => DUPLICANTS.ATTRIBUTES.STORED_VALUE));
			}
		}
	}

	public void ValidateProxy()
	{
		assignableProxy = MinionAssignablesProxy.InitAssignableProxy(assignableProxy, this);
	}

	private void CleanupLimboMinions()
	{
		KPrefabID component = GetComponent<KPrefabID>();
		bool flag = false;
		if (component.InstanceID == -1)
		{
			DebugUtil.LogWarningArgs("Stored minion with an invalid kpid! Attempting to recover...", storedName);
			flag = true;
			if (KPrefabIDTracker.Get().GetInstance(component.InstanceID) != null)
			{
				KPrefabIDTracker.Get().Unregister(component);
			}
			component.InstanceID = KPrefabID.GetUniqueID();
			KPrefabIDTracker.Get().Register(component);
			DebugUtil.LogWarningArgs("Restored as:", component.InstanceID);
		}
		if (component.conflicted)
		{
			DebugUtil.LogWarningArgs("Minion with a conflicted kpid! Attempting to recover... ", component.InstanceID, storedName);
			if (KPrefabIDTracker.Get().GetInstance(component.InstanceID) != null)
			{
				KPrefabIDTracker.Get().Unregister(component);
			}
			component.InstanceID = KPrefabID.GetUniqueID();
			KPrefabIDTracker.Get().Register(component);
			DebugUtil.LogWarningArgs("Restored as:", component.InstanceID);
		}
		assignableProxy.Get().SetTarget(this, base.gameObject);
		bool flag2 = false;
		foreach (MinionStorage item in Components.MinionStorages.Items)
		{
			List<MinionStorage.Info> storedMinionInfo = item.GetStoredMinionInfo();
			for (int i = 0; i < storedMinionInfo.Count; i++)
			{
				MinionStorage.Info info = storedMinionInfo[i];
				if (flag && info.serializedMinion != null && info.serializedMinion.GetId() == -1 && info.name == storedName)
				{
					DebugUtil.LogWarningArgs("Found a minion storage with an invalid ref, rebinding.", component.InstanceID, storedName, item.gameObject.name);
					info = (storedMinionInfo[i] = new MinionStorage.Info(storedName, new Ref<KPrefabID>(component)));
					item.GetComponent<Assignable>().Assign(this);
					flag2 = true;
					break;
				}
				if (info.serializedMinion != null && info.serializedMinion.Get() == component)
				{
					flag2 = true;
					break;
				}
			}
			if (flag2)
			{
				break;
			}
		}
		if (!flag2)
		{
			DebugUtil.LogWarningArgs("Found a stored minion that wasn't in any minion storage. Respawning them at the portal.", component.InstanceID, storedName);
			GameObject activeTelepad = GameUtil.GetActiveTelepad();
			if (activeTelepad != null)
			{
				MinionStorage.DeserializeMinion(component.gameObject, activeTelepad.transform.GetPosition());
			}
		}
	}

	public string GetProperName()
	{
		return storedName;
	}

	public List<Ownables> GetOwners()
	{
		return assignableProxy.Get().ownables;
	}

	public Ownables GetSoleOwner()
	{
		return assignableProxy.Get().GetComponent<Ownables>();
	}

	public bool HasOwner(Assignables owner)
	{
		return GetOwners().Contains(owner as Ownables);
	}

	public int NumOwners()
	{
		return GetOwners().Count;
	}

	public Accessory GetAccessory(AccessorySlot slot)
	{
		for (int i = 0; i < accessories.Count; i++)
		{
			if (accessories[i].Get() != null && accessories[i].Get().slot == slot)
			{
				return accessories[i].Get();
			}
		}
		return null;
	}

	public bool IsNull()
	{
		return this == null;
	}

	public string GetStorageReason()
	{
		KPrefabID component = GetComponent<KPrefabID>();
		foreach (MinionStorage item in Components.MinionStorages.Items)
		{
			foreach (MinionStorage.Info item2 in item.GetStoredMinionInfo())
			{
				if (item2.serializedMinion.Get() == component)
				{
					return item.GetProperName();
				}
			}
		}
		return "";
	}

	public bool IsPermittedToConsume(string consumable)
	{
		foreach (Tag forbiddenTag in forbiddenTags)
		{
			if (forbiddenTag == consumable)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsChoreGroupDisabled(ChoreGroup chore_group)
	{
		foreach (string traitID in traitIDs)
		{
			if (!Db.Get().traits.Exists(traitID))
			{
				continue;
			}
			Trait trait = Db.Get().traits.Get(traitID);
			if (trait.disabledChoreGroups == null)
			{
				continue;
			}
			ChoreGroup[] disabledChoreGroups = trait.disabledChoreGroups;
			for (int i = 0; i < disabledChoreGroups.Length; i++)
			{
				if (disabledChoreGroups[i].IdHash == chore_group.IdHash)
				{
					return true;
				}
			}
		}
		return false;
	}

	public int GetPersonalPriority(ChoreGroup chore_group)
	{
		if (choreGroupPriorities.TryGetValue(chore_group.IdHash, out var value))
		{
			return value.priority;
		}
		return 0;
	}

	public int GetAssociatedSkillLevel(ChoreGroup group)
	{
		return 0;
	}

	public void SetPersonalPriority(ChoreGroup group, int value)
	{
	}

	public void ResetPersonalPriorities()
	{
	}
}
