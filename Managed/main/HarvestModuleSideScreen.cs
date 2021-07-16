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
		if (target.GetComponent<Clustercraft>() != null)
		{
			return GetResourceHarvestModule(target.GetComponent<Clustercraft>()) != null;
		}
		return false;
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
		foreach (Ref<RocketModuleCluster> clusterModule in craft.GetComponent<CraftModuleInterface>().ClusterModules)
		{
			GameObject gameObject = clusterModule.Get().gameObject;
			if (gameObject.GetDef<ResourceHarvestModule.Def>() != null)
			{
				return gameObject.GetSMI<ResourceHarvestModule.StatesInstance>();
			}
		}
		return null;
	}

	private void RefreshModulePanel(StateMachine.Instance module)
	{
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		component.GetReference<Image>("icon").sprite = Def.GetUISprite(module.gameObject).first;
		component.GetReference<LocText>("label").SetText(module.gameObject.GetProperName());
	}

	public void SimEveryTick(float dt)
	{
		if (targetCraft.IsNullOrDestroyed())
		{
			return;
		}
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		ResourceHarvestModule.StatesInstance resourceHarvestModule = GetResourceHarvestModule(targetCraft);
		if (resourceHarvestModule != null)
		{
			GenericUIProgressBar reference = component.GetReference<GenericUIProgressBar>("progressBar");
			float num = 4f;
			float num2 = resourceHarvestModule.timeinstate % num;
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
			Storage component2 = resourceHarvestModule.GetComponent<Storage>();
			float fillPercentage = component2.MassStored() / component2.Capacity();
			reference2.SetFillPercentage(fillPercentage);
			reference2.label.SetText(ElementLoader.GetElement(SimHashes.Diamond.CreateTag()).name + ": " + GameUtil.GetFormattedMass(component2.MassStored()));
		}
	}
}
