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

	private int refreshHandle = -1;

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
		foreach (Ref<RocketModule> module in craftModuleInterface.Modules)
		{
			if (module.Get().GetSMI<IEmptyableCargo>() != null)
			{
				return true;
			}
		}
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		if (refreshHandle != -1 && target != null)
		{
			target.Unsubscribe(refreshHandle);
		}
		base.SetTarget(target);
		targetCraft = target.GetComponent<Clustercraft>();
		if (targetCraft == null && target.GetComponent<RocketControlStation>() != null)
		{
			targetCraft = target.GetMyWorld().GetComponent<Clustercraft>();
		}
		refreshHandle = targetCraft.gameObject.Subscribe(-1298331547, RefreshAll);
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
		foreach (Ref<RocketModule> module in craftModuleInterface.Modules)
		{
			IEmptyableCargo sMI = module.Get().GetSMI<IEmptyableCargo>();
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
		Image reference = hierarchyReferences.GetReference<Image>("icon");
		reference.sprite = Def.GetUISprite(module.master.gameObject).first;
		KButton reference2 = hierarchyReferences.GetReference<KButton>("button");
		reference2.isInteractable = module.CanEmptyCargo();
		reference2.ClearOnClick();
		reference2.onClick += module.EmptyCargo;
		KButton reference3 = hierarchyReferences.GetReference<KButton>("repeatButton");
		if (module.CanAutoDeploy)
		{
			StyleRepeatButton(module);
			reference3.ClearOnClick();
			reference3.onClick += delegate
			{
				OnRepeatClicked(module);
			};
			reference3.gameObject.SetActive(value: true);
		}
		else
		{
			reference3.gameObject.SetActive(value: false);
		}
		DropDown reference4 = hierarchyReferences.GetReference<DropDown>("dropDown");
		reference4.targetDropDownContainer = GameScreenManager.Instance.ssOverlayCanvas;
		reference4.Close();
		CrewPortrait reference5 = hierarchyReferences.GetReference<CrewPortrait>("selectedPortrait");
		if (module.ChooseDuplicant)
		{
			RocketModule component = (module as StateMachine.Instance).GetMaster().GetComponent<RocketModule>();
			CraftModuleInterface craftInterface = component.CraftInterface;
			WorldContainer component2 = craftInterface.GetComponent<WorldContainer>();
			int id = component2.id;
			reference4.gameObject.SetActive(value: true);
			reference4.Initialize(Components.LiveMinionIdentities.GetWorldItems(id), OnDuplicantEntryClick, null, PadDropDownEntryRefreshAction, displaySelectedValueWhenClosed: true, module);
			reference4.selectedLabel.text = ((module.ChosenDuplicant != null) ? GetDuplicantRowName(module.ChosenDuplicant) : UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.SELECT_DUPLICANT.ToString());
			reference5.gameObject.SetActive(value: true);
			reference5.SetIdentityObject(module.ChosenDuplicant, jobEnabled: false);
		}
		else
		{
			reference4.gameObject.SetActive(value: false);
			reference5.gameObject.SetActive(value: false);
		}
		LocText reference6 = hierarchyReferences.GetReference<LocText>("label");
		reference6.SetText(module.master.gameObject.GetProperName());
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
		DropDown reference = hierarchyReferences.GetReference<DropDown>("dropDown");
		reference.selectedLabel.text = ((emptyableCargo.ChosenDuplicant != null) ? GetDuplicantRowName(emptyableCargo.ChosenDuplicant) : UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.SELECT_DUPLICANT.ToString());
		CrewPortrait reference2 = hierarchyReferences.GetReference<CrewPortrait>("selectedPortrait");
		reference2.SetIdentityObject(emptyableCargo.ChosenDuplicant, jobEnabled: false);
		RefreshAll();
	}

	private void PadDropDownEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		MinionIdentity minionIdentity = (MinionIdentity)entry.entryData;
		entry.label.text = GetDuplicantRowName(minionIdentity);
		entry.portrait.SetIdentityObject(minionIdentity, jobEnabled: false);
	}

	private void StyleRepeatButton(IEmptyableCargo module)
	{
		HierarchyReferences hierarchyReferences = modulePanels[module];
		KButton reference = hierarchyReferences.GetReference<KButton>("repeatButton");
		reference.bgImage.colorStyleSetting = (module.AutoDeploy ? repeatOn : repeatOff);
		reference.bgImage.ApplyColorStyleSetting();
	}
}
