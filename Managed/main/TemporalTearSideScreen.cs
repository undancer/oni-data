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
		return component != null && temporalTear != null && temporalTear.Location == component.Location;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		targetCraft = target.GetComponent<Clustercraft>();
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		KButton reference = component.GetReference<KButton>("button");
		reference.ClearOnClick();
		reference.onClick += delegate
		{
			Clustercraft component2 = target.GetComponent<Clustercraft>();
			TemporalTear temporalTear = ClusterManager.Instance.GetComponent<ClusterPOIManager>().GetTemporalTear();
			temporalTear.ConsumeCraft(targetCraft);
		};
		RefreshPanel();
	}

	private void RefreshPanel(object data = null)
	{
		TemporalTear temporalTear = ClusterManager.Instance.GetComponent<ClusterPOIManager>().GetTemporalTear();
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		bool flag = temporalTear.IsOpen();
		LocText reference = component.GetReference<LocText>("label");
		reference.SetText(flag ? UI.UISIDESCREENS.TEMPORALTEARSIDESCREEN.BUTTON_OPEN : UI.UISIDESCREENS.TEMPORALTEARSIDESCREEN.BUTTON_CLOSED);
		KButton reference2 = component.GetReference<KButton>("button");
		reference2.isInteractable = flag;
	}
}
