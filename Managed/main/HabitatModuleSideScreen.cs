using UnityEngine;
using UnityEngine.UI;

public class HabitatModuleSideScreen : SideScreenContent
{
	private Clustercraft targetCraft;

	public GameObject moduleContentContainer;

	public GameObject modulePanelPrefab;

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
		return target.GetComponent<Clustercraft>() != null && GetPassengerModule(target.GetComponent<Clustercraft>()) != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		targetCraft = target.GetComponent<Clustercraft>();
		PassengerRocketModule passengerModule = GetPassengerModule(targetCraft);
		RefreshModulePanel(passengerModule);
	}

	private PassengerRocketModule GetPassengerModule(Clustercraft craft)
	{
		CraftModuleInterface component = craft.GetComponent<CraftModuleInterface>();
		foreach (Ref<RocketModuleCluster> clusterModule in component.ClusterModules)
		{
			PassengerRocketModule component2 = clusterModule.Get().GetComponent<PassengerRocketModule>();
			if (component2 != null)
			{
				return component2;
			}
		}
		return null;
	}

	private void RefreshModulePanel(PassengerRocketModule module)
	{
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		Image reference = component.GetReference<Image>("icon");
		reference.sprite = Def.GetUISprite(module.gameObject).first;
		KButton reference2 = component.GetReference<KButton>("button");
		reference2.ClearOnClick();
		reference2.onClick += delegate
		{
			ClusterManager.Instance.SetActiveWorld(module.GetComponent<ClustercraftExteriorDoor>().GetTargetWorld().id);
			ManagementMenu.Instance.CloseAll();
		};
		LocText reference3 = component.GetReference<LocText>("label");
		reference3.SetText(module.gameObject.GetProperName());
	}
}
