using Klei;
using Klei.AI;
using KSerialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Equipment : Assignables
{
	private SchedulerHandle refreshHandle;

	private static readonly EventSystem.IntraObjectHandler<Equipment> SetDestroyedTrueDelegate = new EventSystem.IntraObjectHandler<Equipment>(delegate(Equipment component, object data)
	{
		component.destroyed = true;
	});

	public bool destroyed { get; private set; }

	public GameObject GetTargetGameObject()
	{
		MinionAssignablesProxy minionAssignablesProxy = (MinionAssignablesProxy)GetAssignableIdentity();
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
		GameObject targetGameObject = GetTargetGameObject();
		MinionResume minionResume = ((targetGameObject != null) ? targetGameObject.GetComponent<MinionResume>() : null);
		Durability component = equippable.GetComponent<Durability>();
		if ((bool)component && (bool)minionResume && !slot.IsUnassigning() && minionResume.HasPerk(Db.Get().SkillPerks.ExosuitDurability.Id))
		{
			float num = (GameClock.Instance.GetTimeInCycles() - component.TimeEquipped) * EQUIPMENT.SUITS.SUIT_DURABILITY_SKILL_BONUS;
			component.TimeEquipped += num;
		}
		equippable.Trigger(-170173755, this);
		if (!targetGameObject)
		{
			return;
		}
		targetGameObject.Trigger(-1285462312, equippable.GetComponent<KPrefabID>());
		KBatchedAnimController component2 = targetGameObject.GetComponent<KBatchedAnimController>();
		if (!destroyed)
		{
			if (equippable.def.BuildOverride != null && component2 != null)
			{
				component2.GetComponent<SymbolOverrideController>().TryRemoveBuildOverride(equippable.def.BuildOverride.GetData(), equippable.def.BuildOverridePriority);
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
				SnapOn component3 = targetGameObject.GetComponent<SnapOn>();
				if (equippable.def.SnapOn != null)
				{
					component3.DetachSnapOnByName(equippable.def.SnapOn);
				}
				if (equippable.def.SnapOn1 != null)
				{
					component3.DetachSnapOnByName(equippable.def.SnapOn1);
				}
			}
			if ((bool)equippable.transform.parent)
			{
				Storage component4 = equippable.transform.parent.GetComponent<Storage>();
				if ((bool)component4)
				{
					component4.Drop(equippable.gameObject);
				}
			}
			SetEquippableStoredModifiers(equippable, isStoring: false);
			equippable.transform.parent = null;
			equippable.transform.SetPosition(targetGameObject.transform.GetPosition() + Vector3.up / 2f);
			KBatchedAnimController component5 = equippable.GetComponent<KBatchedAnimController>();
			if ((bool)component5)
			{
				component5.SetSceneLayer(Grid.SceneLayer.Ore);
			}
			if (!(component2 == null))
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
				PrimaryElement component6 = equippable.GetComponent<PrimaryElement>();
				PrimaryElement component7 = targetGameObject.GetComponent<PrimaryElement>();
				if (component6 != null && component7 != null)
				{
					SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
					invalid.idx = component6.DiseaseIdx;
					invalid.count = (int)((float)component6.DiseaseCount * 0.33f);
					SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid;
					invalid2.idx = component7.DiseaseIdx;
					invalid2.count = (int)((float)component7.DiseaseCount * 0.33f);
					component7.ModifyDiseaseCount(-invalid2.count, "Equipment.Unequip");
					component6.ModifyDiseaseCount(-invalid.count, "Equipment.Unequip");
					if (invalid2.count > 0)
					{
						component6.AddDisease(invalid2.idx, invalid2.count, "Equipment.Unequip");
					}
					if (invalid.count > 0)
					{
						component7.AddDisease(invalid.idx, invalid.count, "Equipment.Unequip");
					}
					if (component != null && component.IsWornOut())
					{
						component.ConvertToWornObject();
					}
				}
			}
		}
		Game.Instance.Trigger(-2146166042);
	}

	public bool IsEquipped(Equippable equippable)
	{
		if (equippable.assignee is Equipment && (Equipment)equippable.assignee == this)
		{
			return equippable.isEquipped;
		}
		return false;
	}

	public bool IsSlotOccupied(AssignableSlot slot)
	{
		EquipmentSlotInstance equipmentSlotInstance = GetSlot(slot) as EquipmentSlotInstance;
		if (equipmentSlotInstance.IsAssigned())
		{
			return (equipmentSlotInstance.assignable as Equippable).isEquipped;
		}
		return false;
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
		GameObject go = equippable.gameObject;
		Storage.MakeItemTemperatureInsulated(go, isStoring, is_initializing: false);
		Storage.MakeItemInvisible(go, isStoring, is_initializing: false);
	}
}
