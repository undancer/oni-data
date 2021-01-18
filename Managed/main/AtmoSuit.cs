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
			Effects component2 = targetGameObject.GetComponent<Effects>();
			if (!component2.HasEffect("SoiledSuit"))
			{
				component2.Add("SoiledSuit", should_save: true);
			}
		}
	}
}
