using Klei;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Equipment : Assignables
{
	private SchedulerHandle refreshHandle;

	private static readonly EventSystem.IntraObjectHandler<Equipment> SetDestroyedTrueDelegate = new EventSystem.IntraObjectHandler<Equipment>(delegate(Equipment component, object data)
	{
		component.destroyed = true;
	});

	public bool destroyed
	{
		get;
		private set;
	}

	public GameObject GetTargetGameObject()
	{
		IAssignableIdentity assignableIdentity = GetAssignableIdentity();
		MinionAssignablesProxy minionAssignablesProxy = (MinionAssignablesProxy)assignableIdentity;
		if ((bool)minionAssignablesProxy)
		{
			return minionAssignablesProxy.GetTargetGameObject();
		}
		return null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Equipment.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(1502190696, SetDestroyedTrueDelegate);
		Subscribe(1969584890, SetDestroyedTrueDelegate);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		refreshHandle.ClearScheduler();
		Components.Equipment.Remove(this);
	}

	public void Equip(Equippable equippable)
	{
		GameObject targetGameObject = GetTargetGameObject();
		KBatchedAnimController component = targetGameObject.GetComponent<KBatchedAnimController>();
		bool flag = component == null;
		if (!flag)
		{
			PrimaryElement component2 = equippable.GetComponent<PrimaryElement>();
			SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
			invalid.idx = component2.DiseaseIdx;
			invalid.count = (int)((float)component2.DiseaseCount * 0.33f);
			PrimaryElement component3 = targetGameObject.GetComponent<PrimaryElement>();
			SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid;
			invalid2.idx = component3.DiseaseIdx;
			invalid2.count = (int)((float)component3.DiseaseCount * 0.33f);
			component3.ModifyDiseaseCount(-invalid2.count, "Equipment.Equip");
			component2.ModifyDiseaseCount(-invalid.count, "Equipment.Equip");
			if (invalid2.count > 0)
			{
				component2.AddDisease(invalid2.idx, invalid2.count, "Equipment.Equip");
			}
			if (invalid.count > 0)
			{
				component3.AddDisease(invalid.idx, invalid.count, "Equipment.Equip");
			}
		}
		AssignableSlotInstance slot = GetSlot(equippable.slot);
		slot.Assign(equippable);
		Debug.Assert(targetGameObject, "GetTargetGameObject returned null in Equip");
		targetGameObject.Trigger(-448952673, equippable.GetComponent<KPrefabID>());
		equippable.Trigger(-1617557748, this);
		Attributes attributes = targetGameObject.GetAttributes();
		if (attributes != null)
		{
			foreach (AttributeModifier attributeModifier in equippable.def.AttributeModifiers)
			{
				attributes.Add(attributeModifier);
			}
		}
		SnapOn component4 = targetGameObject.GetComponent<SnapOn>();
		if (component4 != null)
		{
			component4.AttachSnapOnByName(equippable.def.SnapOn);
			if (equippable.def.SnapOn1 != null)
			{
				component4.AttachSnapOnByName(equippable.def.SnapOn1);
			}
		}
		if (component != null && equippable.def.BuildOverride != null)
		{
			component.GetComponent<SymbolOverrideController>().AddBuildOverride(equippable.def.BuildOverride.GetData(), equippable.def.BuildOverridePriority);
		}
		if ((bool)equippable.transform.parent)
		{
			Storage component5 = equippable.transform.parent.GetComponent<Storage>();
			if ((bool)component5)
			{
				component5.Drop(equippable.gameObject);
			}
		}
		equippable.transform.parent = slot.gameObject.transform;
		equippable.transform.SetLocalPosition(Vector3.zero);
		SetEquippableStoredModifiers(equippable, isStoring: true);
		equippable.OnEquip(slot);
		if (refreshHandle.TimeRemaining > 0f)
		{
			Debug.LogWarning(targetGameObject.GetProperName() + " is already in the process of changing equipment (equip)");
			refreshHandle.ClearScheduler();
		}
		CreatureSimTemperatureTransfer transferer = targetGameObject.GetComponent<CreatureSimTemperatureTransfer>();
		if (!flag)
		{
			refreshHandle = GameScheduler.Instance.Schedule("ChangeEquipment", 2f, delegate
			{
				if (transferer != null)
				{
					transferer.RefreshRegistration();
				}
			});
		}
		Game.Instance.Trigger(-2146166042);
	}

	public void Unequip(Equippable equippable)
	{
		AssignableSlotInstance slot = GetSlot(equippable.slot);
		slot.Unassign();
		equippable.Trigger(-170173755, this);
		GameObject targetGameObject = GetTargetGameObject();
		if (!targetGameObject)
		{
			return;
		}
		targetGameObject.Trigger(-1285462312, equippable.GetComponent<KPrefabID>());
		KBatchedAnimController component = targetGameObject.GetComponent<KBatchedAnimController>();
		if (!destroyed)
		{
			if (equippable.def.BuildOverride != null && component != null)
			{
				component.GetComponent<SymbolOverrideController>().TryRemoveBuildOverride(equippable.def.BuildOverride.GetData(), equippable.def.BuildOverridePriority);
			}
			Attributes attributes = targetGameObject.GetAttributes();
			if (attributes != null)
			{
				foreach (AttributeModifier attributeModifier in equippable.def.AttributeModifiers)
				{
					attributes.Remove(attributeModifier);
				}
			}
			if (!equippable.def.IsBody)
			{
				SnapOn component2 = targetGameObject.GetComponent<SnapOn>();
				if (equippable.def.SnapOn != null)
				{
					component2.DetachSnapOnByName(equippable.def.SnapOn);
				}
				if (equippable.def.SnapOn1 != null)
				{
					component2.DetachSnapOnByName(equippable.def.SnapOn1);
				}
			}
			if ((bool)equippable.transform.parent)
			{
				Storage component3 = equippable.transform.parent.GetComponent<Storage>();
				if ((bool)component3)
				{
					component3.Drop(equippable.gameObject);
				}
			}
			SetEquippableStoredModifiers(equippable, isStoring: false);
			equippable.transform.parent = null;
			equippable.transform.SetPosition(targetGameObject.transform.GetPosition() + Vector3.up / 2f);
			KBatchedAnimController component4 = equippable.GetComponent<KBatchedAnimController>();
			if ((bool)component4)
			{
				component4.SetSceneLayer(Grid.SceneLayer.Ore);
			}
			if (!(component == null))
			{
				if (refreshHandle.TimeRemaining > 0f)
				{
					refreshHandle.ClearScheduler();
				}
				Equipment instance = this;
				refreshHandle = GameScheduler.Instance.Schedule("ChangeEquipment", 1f, delegate
				{
					GameObject gameObject = ((instance != null) ? instance.GetTargetGameObject() : null);
					if ((bool)gameObject)
					{
						CreatureSimTemperatureTransfer component8 = gameObject.GetComponent<CreatureSimTemperatureTransfer>();
						if (component8 != null)
						{
							component8.RefreshRegistration();
						}
					}
				});
			}
			if (!slot.IsUnassigning())
			{
				PrimaryElement component5 = equippable.GetComponent<PrimaryElement>();
				SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
				invalid.idx = component5.DiseaseIdx;
				invalid.count = (int)((float)component5.DiseaseCount * 0.33f);
				PrimaryElement component6 = targetGameObject.GetComponent<PrimaryElement>();
				SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid;
				invalid2.idx = component6.DiseaseIdx;
				invalid2.count = (int)((float)component6.DiseaseCount * 0.33f);
				component6.ModifyDiseaseCount(-invalid2.count, "Equipment.Unequip");
				component5.ModifyDiseaseCount(-invalid.count, "Equipment.Unequip");
				if (invalid2.count > 0)
				{
					component5.AddDisease(invalid2.idx, invalid2.count, "Equipment.Unequip");
				}
				if (invalid.count > 0)
				{
					component6.AddDisease(invalid.idx, invalid.count, "Equipment.Unequip");
				}
				Durability component7 = equippable.GetComponent<Durability>();
				if (component7 != null && component7.IsWornOut())
				{
					component7.ConvertToWornObject();
				}
			}
		}
		Game.Instance.Trigger(-2146166042);
	}

	public bool IsEquipped(Equippable equippable)
	{
		return equippable.assignee is Equipment && (Equipment)equippable.assignee == this && equippable.isEquipped;
	}

	public bool IsSlotOccupied(AssignableSlot slot)
	{
		EquipmentSlotInstance equipmentSlotInstance = GetSlot(slot) as EquipmentSlotInstance;
		return equipmentSlotInstance.IsAssigned() && (equipmentSlotInstance.assignable as Equippable).isEquipped;
	}

	public void UnequipAll()
	{
		foreach (AssignableSlotInstance slot in slots)
		{
			if (slot.assignable != null)
			{
				slot.assignable.Unassign();
			}
		}
	}

	private void SetEquippableStoredModifiers(Equippable equippable, bool isStoring)
	{
		GameObject gameObject = equippable.gameObject;
		Storage.MakeItemTemperatureInsulated(gameObject, isStoring, is_initializing: false);
		Storage.MakeItemInvisible(gameObject, isStoring, is_initializing: false);
	}
}
