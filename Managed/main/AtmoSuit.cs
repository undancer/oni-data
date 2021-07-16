using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AtmoSuit")]
public class AtmoSuit : KMonoBehaviour
{
	private static readonly EventSystem.IntraObjectHandler<AtmoSuit> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<AtmoSuit>(delegate(AtmoSuit component, object data)
	{
		component.RefreshStatusEffects(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1697596308, OnStorageChangedDelegate);
	}

	private void RefreshStatusEffects(object data)
	{
		if (this == null)
		{
			return;
		}
		Equippable component = GetComponent<Equippable>();
		bool flag = GetComponent<Storage>().Has(GameTags.AnyWater);
		if (!(component.assignee != null && flag))
		{
			return;
		}
		Ownables soleOwner = component.assignee.GetSoleOwner();
		if (!(soleOwner != null))
		{
			return;
		}
		GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
		if ((bool)targetGameObject)
		{
			AssignableSlotInstance slot = ((KMonoBehaviour)component.assignee).GetComponent<Equipment>().GetSlot(component.slot);
			Effects component2 = targetGameObject.GetComponent<Effects>();
			if (component2 != null && !component2.HasEffect("SoiledSuit") && !slot.IsUnassigning())
			{
				component2.Add("SoiledSuit", should_save: true);
			}
		}
	}
}
