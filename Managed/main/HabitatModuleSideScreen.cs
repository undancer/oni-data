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
		if (target.GetComponent<Clustercraft>() != null)
		{
			return GetPassengerModule(target.GetComponent<Clustercraft>()) != null;
		}
		return false;
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
		foreach (Ref<RocketModuleCluster> clusterModule in craft.GetComponent<CraftModuleInterface>().ClusterModules)
		{
			PassengerRocketModule component = clusterModule.Get().GetComponent<PassengerRocketModule>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	private void RefreshModulePanel(PassengerRocketModule module)
	{
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		component.GetReference<Image>("icon").sprite = Def.GetUISprite(module.gameObject).first;
		KButton reference = component.GetReference<KButton>("button");
		reference.ClearOnClick();
		reference.onClick += delegate
		{
			AudioMixer.instance.Start(module.interiorReverbSnapshot);
			ClusterManager.Instance.SetActiveWorld(module.GetComponent<ClustercraftExteriorDoor>().GetTargetWorld().id);
			ManagementMenu.Instance.CloseAll();
		};
		component.GetReference<LocText>("label").SetText(module.gameObject.GetProperName());
	}
}
