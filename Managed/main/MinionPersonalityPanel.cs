using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MinionPersonalityPanel : TargetScreen
{
	public GameObject attributesLabelTemplate;

	private GameObject bioPanel;

	private GameObject traitsPanel;

	private DetailsPanelDrawer bioDrawer;

	private DetailsPanelDrawer traitsDrawer;

	public MinionEquipmentPanel panel;

	private SchedulerHandle updateHandle;

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<MinionIdentity>() != null;
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
	}

	public override void OnSelectTarget(GameObject target)
	{
		panel.SetSelectedMinion(target);
		panel.Refresh();
		base.OnSelectTarget(target);
		Refresh();
	}

	public override void OnDeselectTarget(GameObject target)
	{
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		if (panel == null)
		{
			panel = GetComponent<MinionEquipmentPanel>();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		bioPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		traitsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		bioDrawer = new DetailsPanelDrawer(attributesLabelTemplate, bioPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		traitsDrawer = new DetailsPanelDrawer(attributesLabelTemplate, traitsPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
	}

	protected override void OnCleanUp()
	{
		updateHandle.ClearScheduler();
		base.OnCleanUp();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (panel == null)
		{
			panel = GetComponent<MinionEquipmentPanel>();
		}
		Refresh();
		ScheduleUpdate();
	}

	private void ScheduleUpdate()
	{
		updateHandle = UIScheduler.Instance.Schedule("RefreshMinionPersonalityPanel", 1f, delegate
		{
			Refresh();
			ScheduleUpdate();
		});
	}

	private GameObject AddOrGetLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject = null;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
		}
		else
		{
			gameObject = Util.KInstantiate(attributesLabelTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(value: true);
		return gameObject;
	}

	private void Refresh()
	{
		if (base.gameObject.activeSelf && !(selectedTarget == null) && !(selectedTarget.GetComponent<MinionIdentity>() == null))
		{
			RefreshBio();
			RefreshTraits();
		}
	}

	private void RefreshBio()
	{
		MinionIdentity component = selectedTarget.GetComponent<MinionIdentity>();
		if (!component)
		{
			bioPanel.SetActive(value: false);
			return;
		}
		bioPanel.SetActive(value: true);
		bioPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.PERSONALITY.GROUPNAME_BIO;
		bioDrawer.BeginDrawing().NewLabel(string.Concat(DUPLICANTS.NAMETITLE, component.name)).NewLabel(string.Concat(DUPLICANTS.ARRIVALTIME, GameUtil.GetFormattedCycles(((float)GameClock.Instance.GetCycle() - component.arrivalTime) * 600f, "F0", forceCycles: true)))
			.Tooltip(string.Format(DUPLICANTS.ARRIVALTIME_TOOLTIP, component.arrivalTime + 1f, component.name))
			.NewLabel(string.Concat(DUPLICANTS.GENDERTITLE, string.Format(Strings.Get($"STRINGS.DUPLICANTS.GENDER.{component.genderStringKey.ToUpper()}.NAME"), component.gender)))
			.NewLabel(string.Format(Strings.Get($"STRINGS.DUPLICANTS.PERSONALITIES.{component.nameStringKey.ToUpper()}.DESC"), component.name))
			.Tooltip(string.Format(Strings.Get(string.Format("STRINGS.DUPLICANTS.DESC_TOOLTIP", component.nameStringKey.ToUpper())), component.name));
		MinionResume component2 = selectedTarget.GetComponent<MinionResume>();
		if (component2 != null && component2.AptitudeBySkillGroup.Count > 0)
		{
			bioDrawer.NewLabel(string.Concat(UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.NAME, "\n")).Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.TOOLTIP, selectedTarget.name));
			foreach (KeyValuePair<HashedString, float> item in component2.AptitudeBySkillGroup)
			{
				if (item.Value != 0f)
				{
					SkillGroup skillGroup = Db.Get().SkillGroups.Get(item.Key);
					bioDrawer.NewLabel("  • " + skillGroup.Name).Tooltip(string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, skillGroup.Name, item.Value));
				}
			}
		}
		bioDrawer.EndDrawing();
	}

	private void RefreshTraits()
	{
		if (!selectedTarget.GetComponent<MinionIdentity>())
		{
			traitsPanel.SetActive(value: false);
			return;
		}
		traitsPanel.SetActive(value: true);
		traitsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_TRAITS;
		traitsDrawer.BeginDrawing();
		foreach (Trait trait in selectedTarget.GetComponent<Traits>().TraitList)
		{
			traitsDrawer.NewLabel(trait.Name).Tooltip(trait.GetTooltip());
		}
		traitsDrawer.EndDrawing();
	}
}
