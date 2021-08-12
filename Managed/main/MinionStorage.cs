using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/MinionStorage")]
public class MinionStorage : KMonoBehaviour
{
	public struct Info
	{
		public Guid id;

		public string name;

		public Ref<KPrefabID> serializedMinion;

		public Info(string name, Ref<KPrefabID> ref_obj)
		{
			id = Guid.NewGuid();
			this.name = name;
			serializedMinion = ref_obj;
		}

		public static Info CreateEmpty()
		{
			Info result = default(Info);
			result.id = Guid.Empty;
			result.name = null;
			result.serializedMinion = null;
			return result;
		}
	}

	[Serialize]
	private List<Info> serializedMinions = new List<Info>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.MinionStorages.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.MinionStorages.Remove(this);
		base.OnCleanUp();
	}

	private KPrefabID CreateSerializedMinion(GameObject src_minion)
	{
		GameObject gameObject = Util.KInstantiate(SaveLoader.Instance.saveManager.GetPrefab(StoredMinionConfig.ID), Vector3.zero);
		gameObject.SetActive(value: true);
		MinionIdentity component = src_minion.GetComponent<MinionIdentity>();
		StoredMinionIdentity component2 = gameObject.GetComponent<StoredMinionIdentity>();
		CopyMinion(component, component2);
		RedirectInstanceTracker(src_minion, gameObject);
		component.assignableProxy.Get().SetTarget(component2, gameObject);
		Util.KDestroyGameObject(src_minion);
		return gameObject.GetComponent<KPrefabID>();
	}

	private void CopyMinion(MinionIdentity src_id, StoredMinionIdentity dest_id)
	{
		dest_id.storedName = src_id.name;
		dest_id.nameStringKey = src_id.nameStringKey;
		dest_id.gender = src_id.gender;
		dest_id.genderStringKey = src_id.genderStringKey;
		dest_id.arrivalTime = src_id.arrivalTime;
		dest_id.voiceIdx = src_id.voiceIdx;
		dest_id.bodyData = src_id.bodyData;
		Traits component = src_id.GetComponent<Traits>();
		dest_id.traitIDs = new List<string>(component.GetTraitIds());
		dest_id.assignableProxy.Set(src_id.assignableProxy.Get());
		dest_id.assignableProxy.Get().SetTarget(dest_id, dest_id.gameObject);
		Accessorizer component2 = src_id.GetComponent<Accessorizer>();
		dest_id.accessories = component2.GetAccessories();
		ConsumableConsumer component3 = src_id.GetComponent<ConsumableConsumer>();
		if (component3.forbiddenTags != null)
		{
			dest_id.forbiddenTags = new List<Tag>(component3.forbiddenTags);
		}
		MinionResume component4 = src_id.GetComponent<MinionResume>();
		dest_id.MasteryBySkillID = component4.MasteryBySkillID;
		dest_id.grantedSkillIDs = component4.GrantedSkillIDs;
		dest_id.AptitudeBySkillGroup = component4.AptitudeBySkillGroup;
		dest_id.TotalExperienceGained = component4.TotalExperienceGained;
		dest_id.currentHat = component4.CurrentHat;
		dest_id.targetHat = component4.TargetHat;
		ChoreConsumer component5 = src_id.GetComponent<ChoreConsumer>();
		dest_id.choreGroupPriorities = component5.GetChoreGroupPriorities();
		AttributeLevels component6 = src_id.GetComponent<AttributeLevels>();
		component6.OnSerializing();
		dest_id.attributeLevels = new List<AttributeLevels.LevelSaveLoad>(component6.SaveLoadLevels);
		StoreModifiers(src_id, dest_id);
		Schedulable component7 = src_id.GetComponent<Schedulable>();
		Schedule schedule = component7.GetSchedule();
		if (schedule != null)
		{
			schedule.Unassign(component7);
			Schedulable component8 = dest_id.GetComponent<Schedulable>();
			schedule.Assign(component8);
		}
	}

	private static void StoreModifiers(MinionIdentity src_id, StoredMinionIdentity dest_id)
	{
		foreach (AttributeInstance attribute in src_id.GetComponent<MinionModifiers>().attributes)
		{
			if (dest_id.minionModifiers.attributes.Get(attribute.Attribute.Id) == null)
			{
				dest_id.minionModifiers.attributes.Add(attribute.Attribute);
			}
			for (int i = 0; i < attribute.Modifiers.Count; i++)
			{
				dest_id.minionModifiers.attributes.Get(attribute.Id).Add(attribute.Modifiers[i]);
			}
		}
	}

	private static void CopyMinion(StoredMinionIdentity src_id, MinionIdentity dest_id)
	{
		dest_id.SetName(src_id.storedName);
		dest_id.nameStringKey = src_id.nameStringKey;
		dest_id.gender = src_id.gender;
		dest_id.genderStringKey = src_id.genderStringKey;
		dest_id.arrivalTime = src_id.arrivalTime;
		dest_id.voiceIdx = src_id.voiceIdx;
		dest_id.bodyData = src_id.bodyData;
		if (src_id.traitIDs != null)
		{
			dest_id.GetComponent<Traits>().SetTraitIds(src_id.traitIDs);
		}
		if (src_id.accessories != null)
		{
			dest_id.GetComponent<Accessorizer>().SetAccessories(src_id.accessories);
		}
		ConsumableConsumer component = dest_id.GetComponent<ConsumableConsumer>();
		if (src_id.forbiddenTags != null)
		{
			component.forbiddenTags = src_id.forbiddenTags.ToArray();
		}
		if (src_id.MasteryBySkillID != null)
		{
			MinionResume component2 = dest_id.GetComponent<MinionResume>();
			component2.RestoreResume(src_id.MasteryBySkillID, src_id.AptitudeBySkillGroup, src_id.grantedSkillIDs, src_id.TotalExperienceGained);
			component2.SetHats(src_id.currentHat, src_id.targetHat);
		}
		if (src_id.choreGroupPriorities != null)
		{
			dest_id.GetComponent<ChoreConsumer>().SetChoreGroupPriorities(src_id.choreGroupPriorities);
		}
		AttributeLevels component3 = dest_id.GetComponent<AttributeLevels>();
		if (src_id.attributeLevels != null)
		{
			component3.SaveLoadLevels = src_id.attributeLevels.ToArray();
			component3.OnDeserialized();
		}
		dest_id.GetComponent<Accessorizer>().ApplyAccessories();
		dest_id.assignableProxy = new Ref<MinionAssignablesProxy>();
		dest_id.assignableProxy.Set(src_id.assignableProxy.Get());
		dest_id.assignableProxy.Get().SetTarget(dest_id, dest_id.gameObject);
		Equipment equipment = dest_id.GetEquipment();
		foreach (AssignableSlotInstance slot in equipment.Slots)
		{
			Equippable equippable = slot.assignable as Equippable;
			if (equippable != null)
			{
				equipment.Equip(equippable);
			}
		}
		Schedulable component4 = src_id.GetComponent<Schedulable>();
		Schedule schedule = component4.GetSchedule();
		if (schedule != null)
		{
			schedule.Unassign(component4);
			Schedulable component5 = dest_id.GetComponent<Schedulable>();
			schedule.Assign(component5);
		}
	}

	public static void RedirectInstanceTracker(GameObject src_minion, GameObject dest_minion)
	{
		KPrefabID component = src_minion.GetComponent<KPrefabID>();
		dest_minion.GetComponent<KPrefabID>().InstanceID = component.InstanceID;
		component.InstanceID = -1;
	}

	public void SerializeMinion(GameObject minion)
	{
		CleanupBadReferences();
		KPrefabID kPrefabID = CreateSerializedMinion(minion);
		Info item = new Info(kPrefabID.GetComponent<StoredMinionIdentity>().storedName, new Ref<KPrefabID>(kPrefabID));
		serializedMinions.Add(item);
	}

	private void CleanupBadReferences()
	{
		for (int num = serializedMinions.Count - 1; num >= 0; num--)
		{
			if (serializedMinions[num].serializedMinion == null || serializedMinions[num].serializedMinion.Get() == null)
			{
				serializedMinions.RemoveAt(num);
			}
		}
	}

	private int GetMinionIndex(Guid id)
	{
		int result = -1;
		for (int i = 0; i < serializedMinions.Count; i++)
		{
			if (serializedMinions[i].id == id)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public GameObject DeserializeMinion(Guid id, Vector3 pos)
	{
		int minionIndex = GetMinionIndex(id);
		if (minionIndex < 0 || minionIndex >= serializedMinions.Count)
		{
			return null;
		}
		KPrefabID kPrefabID = serializedMinions[minionIndex].serializedMinion.Get();
		serializedMinions.RemoveAt(minionIndex);
		if (kPrefabID == null)
		{
			return null;
		}
		return DeserializeMinion(kPrefabID.gameObject, pos);
	}

	public static GameObject DeserializeMinion(GameObject sourceMinion, Vector3 pos)
	{
		GameObject gameObject = Util.KInstantiate(SaveLoader.Instance.saveManager.GetPrefab(MinionConfig.ID), pos);
		StoredMinionIdentity component = sourceMinion.GetComponent<StoredMinionIdentity>();
		MinionIdentity component2 = gameObject.GetComponent<MinionIdentity>();
		RedirectInstanceTracker(sourceMinion, gameObject);
		gameObject.SetActive(value: true);
		CopyMinion(component, component2);
		component.assignableProxy.Get().SetTarget(component2, gameObject);
		Util.KDestroyGameObject(sourceMinion);
		return gameObject;
	}

	public void DeleteStoredMinion(Guid id)
	{
		int minionIndex = GetMinionIndex(id);
		if (minionIndex >= 0)
		{
			if (serializedMinions[minionIndex].serializedMinion != null)
			{
				serializedMinions[minionIndex].serializedMinion.Get().GetComponent<StoredMinionIdentity>().OnHardDelete();
				Util.KDestroyGameObject(serializedMinions[minionIndex].serializedMinion.Get().gameObject);
			}
			serializedMinions.RemoveAt(minionIndex);
		}
	}

	public List<Info> GetStoredMinionInfo()
	{
		return serializedMinions;
	}

	public void SetStoredMinionInfo(List<Info> info)
	{
		serializedMinions = info;
	}
}
