using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ModuleFlightUtilitySideScreen : SideScreenContent
{
	private Clustercraft targetCraft;

	public GameObject moduleContentContainer;

	public GameObject modulePanelPrefab;

	public ColorStyleSetting repeatOff;

	public ColorStyleSetting repeatOn;

	private Dictionary<IEmptyableCargo, HierarchyReferences> modulePanels = new Dictionary<IEmptyableCargo, HierarchyReferences>();

	private List<int> refreshHandle = new List<int>();

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
		if (target.GetComponent<Clustercraft>() != null && HasFlightUtilityModule(target.GetComponent<CraftModuleInterface>()))
		{
			return true;
		}
		RocketControlStation component = target.GetComponent<RocketControlStation>();
		if (component != null)
		{
			return HasFlightUtilityModule(component.GetMyWorld().GetComponent<Clustercraft>().ModuleInterface);
		}
		return false;
	}

	private bool HasFlightUtilityModule(CraftModuleInterface craftModuleInterface)
	{
		foreach (Ref<RocketModuleCluster> clusterModule in craftModuleInterface.ClusterModules)
		{
			if (clusterModule.Get().GetSMI<IEmptyableCargo>() != null)
			{
				return true;
			}
		}
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		if (target != null)
		{
			foreach (int item in refreshHandle)
			{
				target.Unsubscribe(item);
			}
			refreshHandle.Clear();
		}
		base.SetTarget(target);
		targetCraft = target.GetComponent<Clustercraft>();
		if (targetCraft == null && target.GetComponent<RocketControlStation>() != null)
		{
			targetCraft = target.GetMyWorld().GetComponent<Clustercraft>();
		}
		refreshHandle.Add(targetCraft.gameObject.Subscribe(-1298331547, RefreshAll));
		refreshHandle.Add(targetCraft.gameObject.Subscribe(1792516731, RefreshAll));
		BuildModules();
	}

	private void ClearModules()
	{
		foreach (KeyValuePair<IEmptyableCargo, HierarchyReferences> modulePanel in modulePanels)
		{
			Util.KDestroyGameObject(modulePanel.Value.gameObject);
		}
		modulePanels.Clear();
	}

	private void BuildModules()
	{
		ClearModules();
		foreach (Ref<RocketModuleCluster> clusterModule in craftModuleInterface.ClusterModules)
		{
			IEmptyableCargo sMI = clusterModule.Get().GetSMI<IEmptyableCargo>();
			if (sMI != null)
			{
				HierarchyReferences value = Util.KInstantiateUI<HierarchyReferences>(modulePanelPrefab, moduleContentContainer, force_active: true);
				modulePanels.Add(sMI, value);
				RefreshModulePanel(sMI);
			}
		}
	}

	private void RefreshAll(object data = null)
	{
		BuildModules();
	}

	private void RefreshModulePanel(IEmptyableCargo module)
	{
		HierarchyReferences hierarchyReferences = modulePanels[module];
		hierarchyReferences.GetReference<Image>("icon").sprite = Def.GetUISprite(module.master.gameObject).first;
		KButton reference = hierarchyReferences.GetReference<KButton>("button");
		reference.isInteractable = module.CanEmptyCargo();
		reference.ClearOnClick();
		reference.onClick += module.EmptyCargo;
		KButton reference2 = hierarchyReferences.GetReference<KButton>("repeatButton");
		if (module.CanAutoDeploy)
		{
			StyleRepeatButton(module);
			reference2.ClearOnClick();
			reference2.onClick += delegate
			{
				OnRepeatClicked(module);
			};
			reference2.gameObject.SetActive(value: true);
		}
		else
		{
			reference2.gameObject.SetActive(value: false);
		}
		DropDown reference3 = hierarchyReferences.GetReference<DropDown>("dropDown");
		reference3.targetDropDownContainer = GameScreenManager.Instance.ssOverlayCanvas;
		reference3.Close();
		CrewPortrait reference4 = hierarchyReferences.GetReference<CrewPortrait>("selectedPortrait");
		WorldContainer component = (module as StateMachine.Instance).GetMaster().GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<WorldContainer>();
		if (component != null && module.ChooseDuplicant)
		{
			int id = component.id;
			reference3.gameObject.SetActive(value: true);
			reference3.Initialize(Components.LiveMinionIdentities.GetWorldItems(id), OnDuplicantEntryClick, null, DropDownEntryRefreshAction, displaySelectedValueWhenClosed: true, module);
			reference3.selectedLabel.text = ((module.ChosenDuplicant != null) ? GetDuplicantRowName(module.ChosenDuplicant) : UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.SELECT_DUPLICANT.ToString());
			reference4.gameObject.SetActive(value: true);
			reference4.SetIdentityObject(module.ChosenDuplicant, jobEnabled: false);
		}
		else
		{
			reference3.gameObject.SetActive(value: false);
			reference4.gameObject.SetActive(value: false);
		}
		hierarchyReferences.GetReference<LocText>("label").SetText(module.master.gameObject.GetProperName());
	}

	private string GetDuplicantRowName(MinionIdentity minion)
	{
		MinionResume component = minion.GetComponent<MinionResume>();
		if (component != null && component.HasPerk(Db.Get().SkillPerks.CanUseRocketControlStation))
		{
			return string.Format(UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.PILOT_FMT, minion.GetProperName());
		}
		return minion.GetProperName();
	}

	private void OnRepeatClicked(IEmptyableCargo module)
	{
		module.AutoDeploy = !module.AutoDeploy;
		StyleRepeatButton(module);
	}

	private void OnDuplicantEntryClick(IListableOption option, object data)
	{
		MinionIdentity chosenDuplicant = (MinionIdentity)option;
		IEmptyableCargo emptyableCargo = (IEmptyableCargo)data;
		emptyableCargo.ChosenDuplicant = chosenDuplicant;
		HierarchyReferences hierarchyReferences = modulePanels[emptyableCargo];
		hierarchyReferences.GetReference<DropDown>("dropDown").selectedLabel.text = ((emptyableCargo.ChosenDuplicant != null) ? GetDuplicantRowName(emptyableCargo.ChosenDuplicant) : UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.SELECT_DUPLICANT.ToString());
		hierarchyReferences.GetReference<CrewPortrait>("selectedPortrait").SetIdentityObject(emptyableCargo.ChosenDuplicant, jobEnabled: false);
		RefreshAll();
	}

	private void DropDownEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		MinionIdentity minionIdentity = (MinionIdentity)entry.entryData;
		entry.label.text = GetDuplicantRowName(minionIdentity);
		entry.portrait.SetIdentityObject(minionIdentity, jobEnabled: false);
		bool flag = false;
		foreach (Ref<RocketModuleCluster> clusterModule in targetCraft.ModuleInterface.ClusterModules)
		{
			RocketModuleCluster rocketModuleCluster = clusterModule.Get();
			if (!(rocketModuleCluster == null))
			{
				IEmptyableCargo sMI = rocketModuleCluster.GetSMI<IEmptyableCargo>();
				if (sMI != null && !(((IEmptyableCargo)targetData).ChosenDuplicant == minionIdentity))
				{
					flag = flag || sMI.ChosenDuplicant == minionIdentity;
				}
			}
		}
		entry.button.isInteractable = !flag;
	}

	private void StyleRepeatButton(IEmptyableCargo module)
	{
		KButton reference = modulePanels[module].GetReference<KButton>("repeatButton");
		reference.bgImage.colorStyleSetting = (module.AutoDeploy ? repeatOn : repeatOff);
		reference.bgImage.ApplyColorStyleSetting();
	}
}
