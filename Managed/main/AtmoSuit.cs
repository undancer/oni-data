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
		Equippable component = GetComponent<Equippable>();
		Storage component2 = GetComponent<Storage>();
		bool flag = component2.Has(GameTags.AnyWater);
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
			Equipment component3 = ((KMonoBehaviour)component.assignee).GetComponent<Equipment>();
			AssignableSlotInstance slot = component3.GetSlot(component.slot);
			Effects component4 = targetGameObject.GetComponent<Effects>();
			if (!component4.HasEffect("SoiledSuit") && !slot.IsUnassigning())
			{
				component4.Add("SoiledSuit", should_save: true);
			}
		}
	}
}
