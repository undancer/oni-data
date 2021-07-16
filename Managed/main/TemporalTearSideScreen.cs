using STRINGS;
using UnityEngine;

public class TemporalTearSideScreen : SideScreenContent
{
	private Clustercraft targetCraft;

	private CraftModuleInterface craftModuleInterface => targetCraft.GetComponent<CraftModuleInterface>();

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		base.ConsumeMouseScroll = true;
	}

	public override float GetSortKey()
	{
		return 21f;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		Clustercraft component = target.GetComponent<Clustercraft>();
		TemporalTear temporalTear = ClusterManager.Instance.GetComponent<ClusterPOIManager>().GetTemporalTear();
		if (component != null && temporalTear != null)
		{
			return temporalTear.Location == component.Location;
		}
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		targetCraft = target.GetComponent<Clustercraft>();
		KButton reference = GetComponent<HierarchyReferences>().GetReference<KButton>("button");
		reference.ClearOnClick();
		reference.onClick += delegate
		{
			target.GetComponent<Clustercraft>();
			ClusterManager.Instance.GetComponent<ClusterPOIManager>().GetTemporalTear().ConsumeCraft(targetCraft);
		};
		RefreshPanel();
	}

	private void RefreshPanel(object data = null)
	{
		TemporalTear temporalTear = ClusterManager.Instance.GetComponent<ClusterPOIManager>().GetTemporalTear();
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		bool flag = temporalTear.IsOpen();
		component.GetReference<LocText>("label").SetText(flag ? UI.UISIDESCREENS.TEMPORALTEARSIDESCREEN.BUTTON_OPEN : UI.UISIDESCREENS.TEMPORALTEARSIDESCREEN.BUTTON_CLOSED);
		component.GetReference<KButton>("button").isInteractable = flag;
	}
}
