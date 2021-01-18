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

	private GameObject GetTargetGameObject()
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
		AssignableSlotInstance slot = GetSlot(equippable.slot);
		slot.Assign(equippable);
		GameObject targetGameObject = GetTargetGameObject();
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
		SnapOn component = targetGameObject.GetComponent<SnapOn>();
		if (component != null)
		{
			component.AttachSnapOnByName(equippable.def.SnapOn);
			if (equippable.def.SnapOn1 != null)
			{
				component.AttachSnapOnByName(equippable.def.SnapOn1);
			}
		}
		KBatchedAnimController component2 = targetGameObject.GetComponent<KBatchedAnimController>();
		if (component2 != null && equippable.def.BuildOverride != null)
		{
			component2.GetComponent<SymbolOverrideController>().AddBuildOverride(equippable.def.BuildOverride.GetData(), equippable.def.BuildOverridePriority);
		}
		if ((bool)equippable.transform.parent)
		{
			Storage component3 = equippable.transform.parent.GetComponent<Storage>();
			if ((bool)component3)
			{
				component3.Drop(equippable.gameObject);
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
		if (!(component2 == null))
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
		GetSlot(equippable.slot).Unassign();
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
						CreatureSimTemperatureTransfer component5 = gameObject.GetComponent<CreatureSimTemperatureTransfer>();
						if (component5 != null)
						{
							component5.RefreshRegistration();
						}
					}
				});
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
		GameObject gameObject = equippable.gameObject;
		Storage.MakeItemTemperatureInsulated(gameObject, isStoring, is_initializing: false);
		Storage.MakeItemInvisible(gameObject, isStoring, is_initializing: false);
	}
}
