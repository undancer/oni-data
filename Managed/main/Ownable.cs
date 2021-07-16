using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Ownable : Assignable, ISaveLoadable, IGameObjectEffectDescriptor
{
	public bool tintWhenUnassigned = true;

	private Color unownedTint = Color.gray;

	private Color ownedTint = Color.white;

	public override void Assign(IAssignableIdentity new_assignee)
	{
		if (new_assignee == assignee)
		{
			return;
		}
		if (base.slot != null && new_assignee is MinionIdentity)
		{
			new_assignee = (new_assignee as MinionIdentity).assignableProxy.Get();
		}
		if (base.slot != null && new_assignee is StoredMinionIdentity)
		{
			new_assignee = (new_assignee as StoredMinionIdentity).assignableProxy.Get();
		}
		if (new_assignee is MinionAssignablesProxy)
		{
			AssignableSlotInstance slot = new_assignee.GetSoleOwner().GetComponent<Ownables>().GetSlot(base.slot);
			if (slot != null)
			{
				Assignable assignable = slot.assignable;
				if (assignable != null)
				{
					assignable.Unassign();
				}
			}
		}
		base.Assign(new_assignee);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UpdateTint();
		UpdateStatusString();
		base.OnAssign += OnNewAssignment;
		if (assignee != null)
		{
			return;
		}
		MinionStorage component = GetComponent<MinionStorage>();
		if (!component)
		{
			return;
		}
		List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
		if (storedMinionInfo.Count > 0)
		{
			Ref<KPrefabID> serializedMinion = storedMinionInfo[0].serializedMinion;
			if (serializedMinion != null && serializedMinion.GetId() != -1)
			{
				StoredMinionIdentity component2 = serializedMinion.Get().GetComponent<StoredMinionIdentity>();
				component2.ValidateProxy();
				Assign(component2);
			}
		}
	}

	private void OnNewAssignment(IAssignableIdentity assignables)
	{
		UpdateTint();
		UpdateStatusString();
	}

	private void UpdateTint()
	{
		if (!tintWhenUnassigned)
		{
			return;
		}
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		if (component != null && component.HasBatchInstanceData)
		{
			component.TintColour = ((assignee == null) ? unownedTint : ownedTint);
			return;
		}
		KBatchedAnimController component2 = GetComponent<KBatchedAnimController>();
		if (component2 != null && component2.HasBatchInstanceData)
		{
			component2.TintColour = ((assignee == null) ? unownedTint : ownedTint);
		}
	}

	private void UpdateStatusString()
	{
		KSelectable component = GetComponent<KSelectable>();
		if (!(component == null))
		{
			StatusItem statusItem = null;
			component.SetStatusItem(status_item: (assignee == null) ? Db.Get().BuildingStatusItems.Unassigned : ((assignee is MinionIdentity) ? Db.Get().BuildingStatusItems.AssignedTo : ((!(assignee is Room)) ? Db.Get().BuildingStatusItems.AssignedTo : Db.Get().BuildingStatusItems.AssignedTo)), category: Db.Get().StatusItemCategories.Main, data: this);
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(UI.BUILDINGEFFECTS.ASSIGNEDDUPLICANT, UI.BUILDINGEFFECTS.TOOLTIPS.ASSIGNEDDUPLICANT, Descriptor.DescriptorType.Requirement);
		list.Add(item);
		return list;
	}
}
