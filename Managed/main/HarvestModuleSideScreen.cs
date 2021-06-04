using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class HarvestModuleSideScreen : SideScreenContent, ISimEveryTick
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
		return target.GetComponent<Clustercraft>() != null && GetResourceHarvestModule(target.GetComponent<Clustercraft>()) != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		targetCraft = target.GetComponent<Clustercraft>();
		ResourceHarvestModule.StatesInstance resourceHarvestModule = GetResourceHarvestModule(targetCraft);
		RefreshModulePanel(resourceHarvestModule);
	}

	private ResourceHarvestModule.StatesInstance GetResourceHarvestModule(Clustercraft craft)
	{
		CraftModuleInterface component = craft.GetComponent<CraftModuleInterface>();
		foreach (Ref<RocketModuleCluster> clusterModule in component.ClusterModules)
		{
			GameObject gameObject = clusterModule.Get().gameObject;
			ResourceHarvestModule.Def def = gameObject.GetDef<ResourceHarvestModule.Def>();
			if (def != null)
			{
				return gameObject.GetSMI<ResourceHarvestModule.StatesInstance>();
			}
		}
		return null;
	}

	private void RefreshModulePanel(StateMachine.Instance module)
	{
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		Image reference = component.GetReference<Image>("icon");
		reference.sprite = Def.GetUISprite(module.gameObject).first;
		LocText reference2 = component.GetReference<LocText>("label");
		reference2.SetText(module.gameObject.GetProperName());
	}

	public void SimEveryTick(float dt)
	{
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		GenericUIProgressBar reference = component.GetReference<GenericUIProgressBar>("progressBar");
		float num = 4f;
		float num2 = GetResourceHarvestModule(targetCraft).timeinstate % num;
		ResourceHarvestModule.StatesInstance resourceHarvestModule = GetResourceHarvestModule(targetCraft);
		if (resourceHarvestModule.sm.canHarvest.Get(resourceHarvestModule))
		{
			reference.SetFillPercentage(num2 / num);
			reference.label.SetText(UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_IN_PROGRESS);
		}
		else
		{
			reference.SetFillPercentage(0f);
			reference.label.SetText(UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_STOPPED);
		}
		GenericUIProgressBar reference2 = component.GetReference<GenericUIProgressBar>("diamondProgressBar");
		Storage component2 = GetResourceHarvestModule(targetCraft).GetComponent<Storage>();
		float fillPercentage = component2.MassStored() / component2.Capacity();
		reference2.SetFillPercentage(fillPercentage);
		reference2.label.SetText(ElementLoader.GetElement(SimHashes.Diamond.CreateTag()).name + ": " + GameUtil.GetFormattedMass(component2.MassStored()));
	}
}
