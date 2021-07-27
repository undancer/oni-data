using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MinionAssignablesProxy")]
public class MinionAssignablesProxy : KMonoBehaviour, IAssignableIdentity
{
	public List<Ownables> ownables;

	[Serialize]
	private int target_instance_id = -1;

	private bool slotsConfigured;

	private static readonly EventSystem.IntraObjectHandler<MinionAssignablesProxy> OnAssignablesChangedDelegate = new EventSystem.IntraObjectHandler<MinionAssignablesProxy>(delegate(MinionAssignablesProxy component, object data)
	{
		component.OnAssignablesChanged(data);
	});

	public IAssignableIdentity target { get; private set; }

	public bool IsConfigured => slotsConfigured;

	public int TargetInstanceID => target_instance_id;

	public GameObject GetTargetGameObject()
	{
		if (target == null && target_instance_id != -1)
		{
			RestoreTargetFromInstanceID();
		}
		KMonoBehaviour kMonoBehaviour = (KMonoBehaviour)target;
		if (kMonoBehaviour != null)
		{
			return kMonoBehaviour.gameObject;
		}
		return null;
	}

	public float GetArrivalTime()
	{
		if (GetTargetGameObject().GetComponent<MinionIdentity>() != null)
		{
			return GetTargetGameObject().GetComponent<MinionIdentity>().arrivalTime;
		}
		if (GetTargetGameObject().GetComponent<StoredMinionIdentity>() != null)
		{
			return GetTargetGameObject().GetComponent<StoredMinionIdentity>().arrivalTime;
		}
		Debug.LogError("Could not get minion arrival time");
		return -1f;
	}

	public int GetTotalSkillpoints()
	{
		if (GetTargetGameObject().GetComponent<MinionIdentity>() != null)
		{
			return GetTargetGameObject().GetComponent<MinionResume>().TotalSkillPointsGained;
		}
		if (GetTargetGameObject().GetComponent<StoredMinionIdentity>() != null)
		{
			return MinionResume.CalculateTotalSkillPointsGained(GetTargetGameObject().GetComponent<StoredMinionIdentity>().TotalExperienceGained);
		}
		Debug.LogError("Could not get minion skill points time");
		return -1;
	}

	public void SetTarget(IAssignableIdentity target, GameObject targetGO)
	{
		Debug.Assert(target != null, "target was null");
		if (targetGO == null)
		{
			Debug.LogWarningFormat("{0} MinionAssignablesProxy.SetTarget {1}, {2}, {3}. DESTROYING", GetInstanceID(), target_instance_id, target, targetGO);
			Util.KDestroyGameObject(base.gameObject);
		}
		this.target = target;
		target_instance_id = targetGO.GetComponent<KPrefabID>().InstanceID;
		base.gameObject.name = "Minion Assignables Proxy : " + targetGO.name;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ownables = new List<Ownables> { base.gameObject.AddOrGet<Ownables>() };
		Components.MinionAssignablesProxy.Add(this);
		ConfigureAssignableSlots();
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
	}

	public void ConfigureAssignableSlots()
	{
		if (slotsConfigured)
		{
			return;
		}
		Ownables component = GetComponent<Ownables>();
		Equipment component2 = GetComponent<Equipment>();
		if (component2 != null)
		{
			foreach (AssignableSlot resource in Db.Get().AssignableSlots.resources)
			{
				if (resource is OwnableSlot)
				{
					OwnableSlotInstance slot_instance = new OwnableSlotInstance(component, (OwnableSlot)resource);
					component.Add(slot_instance);
				}
				else if (resource is EquipmentSlot)
				{
					EquipmentSlotInstance slot_instance2 = new EquipmentSlotInstance(component2, (EquipmentSlot)resource);
					component2.Add(slot_instance2);
				}
			}
		}
		slotsConfigured = true;
	}

	public void RestoreTargetFromInstanceID()
	{
		if (target_instance_id == -1 || target != null)
		{
			return;
		}
		KPrefabID instance = KPrefabIDTracker.Get().GetInstance(target_instance_id);
		if ((bool)instance)
		{
			IAssignableIdentity component = instance.GetComponent<IAssignableIdentity>();
			if (component != null)
			{
				SetTarget(component, instance.gameObject);
				return;
			}
			Debug.LogWarningFormat("RestoreTargetFromInstanceID target ID {0} was found but it wasn't an IAssignableIdentity, destroying proxy object.", target_instance_id);
			Util.KDestroyGameObject(base.gameObject);
		}
		else
		{
			Debug.LogWarningFormat("RestoreTargetFromInstanceID target ID {0} was not found, destroying proxy object.", target_instance_id);
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		RestoreTargetFromInstanceID();
		if (target != null)
		{
			Subscribe(-1585839766, OnAssignablesChangedDelegate);
			Game.Instance.assignmentManager.AddToAssignmentGroup("public", this);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Game.Instance.assignmentManager.RemoveFromAllGroups(this);
		GetComponent<Ownables>().UnassignAll();
		GetComponent<Equipment>().UnequipAll();
		Components.MinionAssignablesProxy.Remove(this);
	}

	private void OnAssignablesChanged(object data)
	{
		if (!target.IsNull())
		{
			((KMonoBehaviour)target).Trigger(-1585839766, data);
		}
	}

	private void CheckTarget()
	{
		if (target != null)
		{
			return;
		}
		KPrefabID instance = KPrefabIDTracker.Get().GetInstance(target_instance_id);
		if (!(instance != null))
		{
			return;
		}
		target = instance.GetComponent<IAssignableIdentity>();
		if (target == null)
		{
			return;
		}
		MinionIdentity minionIdentity = target as MinionIdentity;
		if ((bool)minionIdentity)
		{
			minionIdentity.ValidateProxy();
			return;
		}
		StoredMinionIdentity storedMinionIdentity = target as StoredMinionIdentity;
		if ((bool)storedMinionIdentity)
		{
			storedMinionIdentity.ValidateProxy();
		}
	}

	public List<Ownables> GetOwners()
	{
		CheckTarget();
		return target.GetOwners();
	}

	public string GetProperName()
	{
		CheckTarget();
		return target.GetProperName();
	}

	public Ownables GetSoleOwner()
	{
		CheckTarget();
		return target.GetSoleOwner();
	}

	public bool HasOwner(Assignables owner)
	{
		CheckTarget();
		return target.HasOwner(owner);
	}

	public int NumOwners()
	{
		CheckTarget();
		return target.NumOwners();
	}

	public bool IsNull()
	{
		CheckTarget();
		return target.IsNull();
	}

	public static Ref<MinionAssignablesProxy> InitAssignableProxy(Ref<MinionAssignablesProxy> assignableProxyRef, IAssignableIdentity source)
	{
		if (assignableProxyRef == null)
		{
			assignableProxyRef = new Ref<MinionAssignablesProxy>();
		}
		GameObject targetGO = ((KMonoBehaviour)source).gameObject;
		MinionAssignablesProxy minionAssignablesProxy = assignableProxyRef.Get();
		if (minionAssignablesProxy == null)
		{
			GameObject obj = GameUtil.KInstantiate(Assets.GetPrefab(MinionAssignablesProxyConfig.ID), Grid.SceneLayer.NoLayer);
			minionAssignablesProxy = obj.GetComponent<MinionAssignablesProxy>();
			minionAssignablesProxy.SetTarget(source, targetGO);
			obj.SetActive(value: true);
			assignableProxyRef.Set(minionAssignablesProxy);
		}
		else
		{
			minionAssignablesProxy.SetTarget(source, targetGO);
		}
		return assignableProxyRef;
	}
}
