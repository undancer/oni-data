using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SkillsScreen : KModalScreen
{
	public new const float SCREEN_SORT_KEY = 101f;

	[SerializeField]
	private KButton CloseButton;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject Prefab_skillWidget;

	[SerializeField]
	private GameObject Prefab_skillColumn;

	[SerializeField]
	private GameObject Prefab_minion;

	[SerializeField]
	private GameObject Prefab_minionLayout;

	[SerializeField]
	private GameObject Prefab_tableLayout;

	[Header("Sort Toggles")]
	[SerializeField]
	private MultiToggle dupeSortingToggle;

	[SerializeField]
	private MultiToggle experienceSortingToggle;

	[SerializeField]
	private MultiToggle moraleSortingToggle;

	private MultiToggle activeSortToggle;

	private bool sortReversed;

	[Header("Duplicant Animation")]
	[SerializeField]
	private GameObject duplicantAnimAnchor;

	[SerializeField]
	private KBatchedAnimController animController;

	public float baseCharacterScale = 0.38f;

	private KAnimFile idle_anim;

	[Header("Progress Bars")]
	[SerializeField]
	private ToolTip expectationsTooltip;

	[SerializeField]
	private LocText moraleProgressLabel;

	[SerializeField]
	private GameObject moraleWarning;

	[SerializeField]
	private GameObject moraleNotch;

	[SerializeField]
	private Color moraleNotchColor;

	private List<GameObject> moraleNotches = new List<GameObject>();

	[SerializeField]
	private LocText expectationsProgressLabel;

	[SerializeField]
	private GameObject expectationWarning;

	[SerializeField]
	private GameObject expectationNotch;

	[SerializeField]
	private Color expectationNotchColor;

	[SerializeField]
	private Color expectationNotchProspectColor;

	private List<GameObject> expectationNotches = new List<GameObject>();

	[SerializeField]
	private ToolTip experienceBarTooltip;

	[SerializeField]
	private Image experienceProgressFill;

	[SerializeField]
	private LocText EXPCount;

	[SerializeField]
	private LocText duplicantLevelIndicator;

	[SerializeField]
	private KScrollRect scrollRect;

	[SerializeField]
	private DropDown hatDropDown;

	[SerializeField]
	public Image selectedHat;

	private IAssignableIdentity currentlySelectedMinion;

	private List<SkillMinionWidget> minionWidgets = new List<SkillMinionWidget>();

	private string hoveredSkillID = "";

	private Dictionary<string, GameObject> skillWidgets = new Dictionary<string, GameObject>();

	private Dictionary<string, int> skillGroupRow = new Dictionary<string, int>();

	private List<GameObject> skillColumns = new List<GameObject>();

	private bool dirty;

	private bool linesPending;

	private int layoutRowHeight = 80;

	private Coroutine delayRefreshRoutine;

	public IAssignableIdentity CurrentlySelectedMinion
	{
		get
		{
			if (currentlySelectedMinion == null || currentlySelectedMinion.IsNull())
			{
				return null;
			}
			return currentlySelectedMinion;
		}
		set
		{
			currentlySelectedMinion = value;
			if (IsActive())
			{
				RefreshSelectedMinion();
				RefreshSkillWidgets();
			}
		}
	}

	protected override void OnActivate()
	{
		base.ConsumeMouseScroll = true;
		base.OnActivate();
		BuildMinions();
		RefreshAll();
		Components.LiveMinionIdentities.OnAdd += OnAddMinionIdentity;
		Components.LiveMinionIdentities.OnRemove += OnRemoveMinionIdentity;
		CloseButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		MultiToggle multiToggle = dupeSortingToggle;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			SortByMinon();
		});
		MultiToggle multiToggle2 = moraleSortingToggle;
		multiToggle2.onClick = (System.Action)Delegate.Combine(multiToggle2.onClick, (System.Action)delegate
		{
			SortByMorale();
		});
		MultiToggle multiToggle3 = experienceSortingToggle;
		multiToggle3.onClick = (System.Action)Delegate.Combine(multiToggle3.onClick, (System.Action)delegate
		{
			SortByExperience();
		});
	}

	protected override void OnShow(bool show)
	{
		if (show)
		{
			if (CurrentlySelectedMinion == null && Components.LiveMinionIdentities.Count > 0)
			{
				CurrentlySelectedMinion = Components.LiveMinionIdentities.Items[0];
			}
			RefreshAll();
		}
		base.OnShow(show);
	}

	public void RefreshAll()
	{
		dirty = false;
		RefreshSkillWidgets();
		RefreshSelectedMinion();
		linesPending = true;
	}

	private void RefreshSelectedMinion()
	{
		SetPortraitAnimator(currentlySelectedMinion);
		RefreshProgressBars();
		RefreshHat();
	}

	private void RefreshProgressBars()
	{
		if (currentlySelectedMinion == null || currentlySelectedMinion.IsNull())
		{
			return;
		}
		MinionIdentity minionIdentity = currentlySelectedMinion as MinionIdentity;
		HierarchyReferences component = expectationsTooltip.GetComponent<HierarchyReferences>();
		component.GetReference("Labels").gameObject.SetActive(minionIdentity != null);
		component.GetReference("MoraleBar").gameObject.SetActive(minionIdentity != null);
		component.GetReference("ExpectationBar").gameObject.SetActive(minionIdentity != null);
		component.GetReference("StoredMinion").gameObject.SetActive(minionIdentity == null);
		experienceProgressFill.gameObject.SetActive(minionIdentity != null);
		if (minionIdentity == null)
		{
			expectationsTooltip.SetSimpleTooltip(string.Format(UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, (currentlySelectedMinion as StoredMinionIdentity).GetStorageReason(), currentlySelectedMinion.GetProperName()));
			experienceBarTooltip.SetSimpleTooltip(string.Format(UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, (currentlySelectedMinion as StoredMinionIdentity).GetStorageReason(), currentlySelectedMinion.GetProperName()));
			EXPCount.text = "";
			duplicantLevelIndicator.text = UI.TABLESCREENS.NA;
			return;
		}
		MinionResume component2 = minionIdentity.GetComponent<MinionResume>();
		float num = MinionResume.CalculatePreviousExperienceBar(component2.TotalSkillPointsGained);
		float num2 = MinionResume.CalculateNextExperienceBar(component2.TotalSkillPointsGained);
		float fillAmount = (component2.TotalExperienceGained - num) / (num2 - num);
		EXPCount.text = Mathf.RoundToInt(component2.TotalExperienceGained - num) + " / " + Mathf.RoundToInt(num2 - num);
		duplicantLevelIndicator.text = component2.AvailableSkillpoints.ToString();
		experienceProgressFill.fillAmount = fillAmount;
		experienceBarTooltip.SetSimpleTooltip(string.Format(UI.SKILLS_SCREEN.EXPERIENCE_TOOLTIP, Mathf.RoundToInt(num2 - num) - Mathf.RoundToInt(component2.TotalExperienceGained - num)));
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(component2);
		AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(component2);
		float num3 = 0f;
		float num4 = 0f;
		if (!string.IsNullOrEmpty(hoveredSkillID) && !component2.HasMasteredSkill(hoveredSkillID))
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			list.Add(hoveredSkillID);
			while (list.Count > 0)
			{
				for (int num5 = list.Count - 1; num5 >= 0; num5--)
				{
					if (!component2.HasMasteredSkill(list[num5]))
					{
						num3 += (float)(Db.Get().Skills.Get(list[num5]).tier + 1);
						if (component2.AptitudeBySkillGroup.ContainsKey(Db.Get().Skills.Get(list[num5]).skillGroup) && component2.AptitudeBySkillGroup[Db.Get().Skills.Get(list[num5]).skillGroup] > 0f)
						{
							num4 += 1f;
						}
						foreach (string priorSkill in Db.Get().Skills.Get(list[num5]).priorSkills)
						{
							list2.Add(priorSkill);
						}
					}
				}
				list.Clear();
				list.AddRange(list2);
				list2.Clear();
			}
		}
		float num6 = attributeInstance.GetTotalValue() + num4 / (attributeInstance2.GetTotalValue() + num3);
		float f = Mathf.Max(attributeInstance.GetTotalValue() + num4, attributeInstance2.GetTotalValue() + num3);
		while (moraleNotches.Count < Mathf.RoundToInt(f))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(moraleNotch, moraleNotch.transform.parent);
			gameObject.SetActive(value: true);
			moraleNotches.Add(gameObject);
		}
		while (moraleNotches.Count > Mathf.RoundToInt(f))
		{
			GameObject gameObject2 = moraleNotches[moraleNotches.Count - 1];
			moraleNotches.Remove(gameObject2);
			UnityEngine.Object.Destroy(gameObject2);
		}
		for (int i = 0; i < moraleNotches.Count; i++)
		{
			if ((float)i < attributeInstance.GetTotalValue() + num4)
			{
				moraleNotches[i].GetComponentsInChildren<Image>()[1].color = moraleNotchColor;
			}
			else
			{
				moraleNotches[i].GetComponentsInChildren<Image>()[1].color = Color.clear;
			}
		}
		moraleProgressLabel.text = string.Concat(UI.SKILLS_SCREEN.MORALE, ": ", attributeInstance.GetTotalValue().ToString());
		if (num4 > 0f)
		{
			LocText locText = moraleProgressLabel;
			locText.text = locText.text + " + " + GameUtil.ApplyBoldString(GameUtil.ColourizeString(moraleNotchColor, num4.ToString()));
		}
		while (expectationNotches.Count < Mathf.RoundToInt(f))
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate(expectationNotch, expectationNotch.transform.parent);
			gameObject3.SetActive(value: true);
			expectationNotches.Add(gameObject3);
		}
		while (expectationNotches.Count > Mathf.RoundToInt(f))
		{
			GameObject gameObject4 = expectationNotches[expectationNotches.Count - 1];
			expectationNotches.Remove(gameObject4);
			UnityEngine.Object.Destroy(gameObject4);
		}
		for (int j = 0; j < expectationNotches.Count; j++)
		{
			if ((float)j < attributeInstance2.GetTotalValue() + num3)
			{
				if ((float)j < attributeInstance2.GetTotalValue())
				{
					expectationNotches[j].GetComponentsInChildren<Image>()[1].color = expectationNotchColor;
				}
				else
				{
					expectationNotches[j].GetComponentsInChildren<Image>()[1].color = expectationNotchProspectColor;
				}
			}
			else
			{
				expectationNotches[j].GetComponentsInChildren<Image>()[1].color = Color.clear;
			}
		}
		expectationsProgressLabel.text = string.Concat(UI.SKILLS_SCREEN.MORALE_EXPECTATION, ": ", attributeInstance2.GetTotalValue().ToString());
		if (num3 > 0f)
		{
			LocText locText2 = expectationsProgressLabel;
			locText2.text = locText2.text + " + " + GameUtil.ApplyBoldString(GameUtil.ColourizeString(expectationNotchColor, num3.ToString()));
		}
		if (num6 < 1f)
		{
			expectationWarning.SetActive(value: true);
			moraleWarning.SetActive(value: false);
		}
		else
		{
			expectationWarning.SetActive(value: false);
			moraleWarning.SetActive(value: true);
		}
		string text = "";
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		text = text + GameUtil.ApplyBoldString(UI.SKILLS_SCREEN.MORALE) + ": " + attributeInstance.GetTotalValue() + "\n";
		for (int k = 0; k < attributeInstance.Modifiers.Count; k++)
		{
			dictionary.Add(attributeInstance.Modifiers[k].GetDescription(), attributeInstance.Modifiers[k].Value);
		}
		List<KeyValuePair<string, float>> list3 = dictionary.ToList();
		list3.Sort((KeyValuePair<string, float> pair1, KeyValuePair<string, float> pair2) => pair2.Value.CompareTo(pair1.Value));
		foreach (KeyValuePair<string, float> item in list3)
		{
			text = text + "    • " + item.Key + ": " + ((item.Value > 0f) ? UIConstants.ColorPrefixGreen : UIConstants.ColorPrefixRed) + item.Value + UIConstants.ColorSuffix + "\n";
		}
		text += "\n";
		text = text + GameUtil.ApplyBoldString(UI.SKILLS_SCREEN.MORALE_EXPECTATION) + ": " + attributeInstance2.GetTotalValue() + "\n";
		for (int l = 0; l < attributeInstance2.Modifiers.Count; l++)
		{
			text = text + "    • " + attributeInstance2.Modifiers[l].GetDescription() + ": " + ((attributeInstance2.Modifiers[l].Value > 0f) ? UIConstants.ColorPrefixRed : UIConstants.ColorPrefixGreen) + attributeInstance2.Modifiers[l].GetFormattedString(component2.gameObject) + UIConstants.ColorSuffix + "\n";
		}
		expectationsTooltip.SetSimpleTooltip(text);
	}

	private void RefreshHat()
	{
		if (currentlySelectedMinion == null || currentlySelectedMinion.IsNull())
		{
			return;
		}
		List<IListableOption> list = new List<IListableOption>();
		string text = "";
		MinionIdentity minionIdentity = currentlySelectedMinion as MinionIdentity;
		if (minionIdentity != null)
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			text = (string.IsNullOrEmpty(component.TargetHat) ? component.CurrentHat : component.TargetHat);
			foreach (KeyValuePair<string, bool> item in component.MasteryBySkillID)
			{
				if (item.Value)
				{
					list.Add(new SkillListable(item.Key));
				}
			}
			hatDropDown.Initialize(list, OnHatDropEntryClick, hatDropDownSort, hatDropEntryRefreshAction, displaySelectedValueWhenClosed: false, currentlySelectedMinion);
		}
		else
		{
			StoredMinionIdentity storedMinionIdentity = currentlySelectedMinion as StoredMinionIdentity;
			text = (string.IsNullOrEmpty(storedMinionIdentity.targetHat) ? storedMinionIdentity.currentHat : storedMinionIdentity.targetHat);
		}
		hatDropDown.openButton.enabled = minionIdentity != null;
		selectedHat.transform.Find("Arrow").gameObject.SetActive(minionIdentity != null);
		selectedHat.sprite = Assets.GetSprite(string.IsNullOrEmpty(text) ? "hat_role_none" : text);
	}

	private void OnHatDropEntryClick(IListableOption skill, object data)
	{
		MinionIdentity minionIdentity = currentlySelectedMinion as MinionIdentity;
		if (minionIdentity == null)
		{
			return;
		}
		MinionResume component = minionIdentity.GetComponent<MinionResume>();
		string s = "hat_role_none";
		if (skill != null)
		{
			selectedHat.sprite = Assets.GetSprite((skill as SkillListable).skillHat);
			if (component != null)
			{
				s = (skill as SkillListable).skillHat;
				component.SetHats(component.CurrentHat, s);
				if (component.OwnsHat(s))
				{
					new PutOnHatChore(component, Db.Get().ChoreTypes.SwitchHat);
				}
			}
		}
		else
		{
			selectedHat.sprite = Assets.GetSprite(s);
			if (component != null)
			{
				component.SetHats(component.CurrentHat, null);
				component.ApplyTargetHat();
			}
		}
		foreach (SkillMinionWidget minionWidget in minionWidgets)
		{
			if (minionWidget.minion == currentlySelectedMinion)
			{
				minionWidget.RefreshHat(component.TargetHat);
			}
		}
	}

	private void hatDropEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		if (entry.entryData != null)
		{
			SkillListable skillListable = entry.entryData as SkillListable;
			entry.image.sprite = Assets.GetSprite(skillListable.skillHat);
		}
	}

	private int hatDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		return 0;
	}

	private void Update()
	{
		if (dirty)
		{
			RefreshAll();
		}
		if (!linesPending)
		{
			return;
		}
		foreach (GameObject value in skillWidgets.Values)
		{
			value.GetComponent<SkillWidget>().RefreshLines();
		}
		linesPending = false;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed && !scrollRect.isDragging && e.TryConsume(Action.MouseRight))
		{
			ManagementMenu.Instance.CloseAll();
		}
		else
		{
			base.OnKeyUp(e);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed && e.TryConsume(Action.Escape))
		{
			ManagementMenu.Instance.CloseAll();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private void RefreshSkillWidgets()
	{
		int num = 1;
		foreach (SkillGroup resource in Db.Get().SkillGroups.resources)
		{
			List<Skill> skillsBySkillGroup = GetSkillsBySkillGroup(resource.Id);
			if (skillsBySkillGroup.Count <= 0)
			{
				continue;
			}
			if (!skillGroupRow.ContainsKey(resource.Id))
			{
				skillGroupRow.Add(resource.Id, num++);
			}
			for (int i = 0; i < skillsBySkillGroup.Count; i++)
			{
				Skill skill = skillsBySkillGroup[i];
				if (!skillWidgets.ContainsKey(skill.Id))
				{
					while (skill.tier >= skillColumns.Count)
					{
						GameObject gameObject = Util.KInstantiateUI(Prefab_skillColumn, Prefab_tableLayout, force_active: true);
						skillColumns.Add(gameObject);
						HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
						if (skillColumns.Count % 2 == 0)
						{
							component.GetReference("BG").gameObject.SetActive(value: false);
						}
					}
					GameObject value = Util.KInstantiateUI(Prefab_skillWidget, skillColumns[skill.tier], force_active: true);
					skillWidgets.Add(skill.Id, value);
				}
				skillWidgets[skill.Id].GetComponent<SkillWidget>().Refresh(skill.Id);
			}
		}
		foreach (SkillMinionWidget minionWidget in minionWidgets)
		{
			minionWidget.Refresh();
		}
		RefreshWidgetPositions();
	}

	public void HoverSkill(string skillID)
	{
		hoveredSkillID = skillID;
		if (delayRefreshRoutine != null)
		{
			StopCoroutine(delayRefreshRoutine);
			delayRefreshRoutine = null;
		}
		if (string.IsNullOrEmpty(hoveredSkillID))
		{
			delayRefreshRoutine = StartCoroutine(DelayRefreshProgressBars());
		}
		else
		{
			RefreshProgressBars();
		}
	}

	private IEnumerator DelayRefreshProgressBars()
	{
		yield return new WaitForSecondsRealtime(0.1f);
		RefreshProgressBars();
	}

	public void RefreshWidgetPositions()
	{
		float num = 0f;
		foreach (KeyValuePair<string, GameObject> skillWidget in skillWidgets)
		{
			float rowPosition = GetRowPosition(skillWidget.Key);
			num = Mathf.Max(rowPosition, num);
			skillWidget.Value.rectTransform().anchoredPosition = Vector2.down * rowPosition;
		}
		num = Mathf.Max(num, layoutRowHeight);
		float num2 = layoutRowHeight;
		foreach (GameObject skillColumn in skillColumns)
		{
			skillColumn.GetComponent<LayoutElement>().minHeight = num + num2;
		}
		linesPending = true;
	}

	public float GetRowPosition(string skillID)
	{
		int num = skillGroupRow[Db.Get().Skills.Get(skillID).skillGroup];
		return layoutRowHeight * (num - 1);
	}

	private void OnAddMinionIdentity(MinionIdentity add)
	{
		BuildMinions();
		RefreshAll();
	}

	private void OnRemoveMinionIdentity(MinionIdentity remove)
	{
		if (CurrentlySelectedMinion == remove)
		{
			CurrentlySelectedMinion = null;
		}
		BuildMinions();
		RefreshAll();
	}

	private void BuildMinions()
	{
		for (int num = minionWidgets.Count - 1; num >= 0; num--)
		{
			minionWidgets[num].DeleteObject();
		}
		minionWidgets.Clear();
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			GameObject gameObject = Util.KInstantiateUI(Prefab_minion, Prefab_minionLayout, force_active: true);
			gameObject.GetComponent<SkillMinionWidget>().SetMinon(item);
			minionWidgets.Add(gameObject.GetComponent<SkillMinionWidget>());
		}
		foreach (MinionStorage item2 in Components.MinionStorages.Items)
		{
			foreach (MinionStorage.Info item3 in item2.GetStoredMinionInfo())
			{
				if (item3.serializedMinion != null)
				{
					StoredMinionIdentity minon = item3.serializedMinion.Get<StoredMinionIdentity>();
					GameObject gameObject2 = Util.KInstantiateUI(Prefab_minion, Prefab_minionLayout, force_active: true);
					gameObject2.GetComponent<SkillMinionWidget>().SetMinon(minon);
					minionWidgets.Add(gameObject2.GetComponent<SkillMinionWidget>());
				}
			}
		}
		if (CurrentlySelectedMinion == null && Components.LiveMinionIdentities.Count > 0)
		{
			CurrentlySelectedMinion = Components.LiveMinionIdentities.Items[0];
		}
	}

	public Vector2 GetSkillWidgetLineTargetPosition(string skillID)
	{
		return skillWidgets[skillID].GetComponent<SkillWidget>().lines_right.GetPosition();
	}

	public SkillWidget GetSkillWidget(string skill)
	{
		return skillWidgets[skill].GetComponent<SkillWidget>();
	}

	public List<Skill> GetSkillsBySkillGroup(string skillGrp)
	{
		List<Skill> list = new List<Skill>();
		foreach (Skill resource in Db.Get().Skills.resources)
		{
			if (resource.skillGroup == skillGrp)
			{
				list.Add(resource);
			}
		}
		return list;
	}

	private void SelectSortToggle(MultiToggle toggle)
	{
		dupeSortingToggle.ChangeState(0);
		experienceSortingToggle.ChangeState(0);
		moraleSortingToggle.ChangeState(0);
		if (toggle != null)
		{
			if (activeSortToggle == toggle)
			{
				sortReversed = !sortReversed;
			}
			activeSortToggle = toggle;
		}
		activeSortToggle.ChangeState((!sortReversed) ? 1 : 2);
	}

	private void SortByMorale()
	{
		SelectSortToggle(moraleSortingToggle);
		List<SkillMinionWidget> list = minionWidgets;
		list.Sort(delegate(SkillMinionWidget a, SkillMinionWidget b)
		{
			MinionIdentity minionIdentity = a.minion as MinionIdentity;
			MinionIdentity minionIdentity2 = b.minion as MinionIdentity;
			if (minionIdentity == null && minionIdentity2 == null)
			{
				return 0;
			}
			if (minionIdentity == null)
			{
				return -1;
			}
			if (minionIdentity2 == null)
			{
				return 1;
			}
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			MinionResume component2 = minionIdentity2.GetComponent<MinionResume>();
			AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(component);
			AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(component);
			AttributeInstance attributeInstance3 = Db.Get().Attributes.QualityOfLife.Lookup(component2);
			AttributeInstance attributeInstance4 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(component2);
			float num = attributeInstance.GetTotalValue() / attributeInstance2.GetTotalValue();
			float value = attributeInstance3.GetTotalValue() / attributeInstance4.GetTotalValue();
			return num.CompareTo(value);
		});
		ReorderEntries(list, sortReversed);
	}

	private void SortByMinon()
	{
		SelectSortToggle(dupeSortingToggle);
		List<SkillMinionWidget> list = minionWidgets;
		list.Sort((SkillMinionWidget a, SkillMinionWidget b) => a.minion.GetProperName().CompareTo(b.minion.GetProperName()));
		ReorderEntries(list, sortReversed);
	}

	private void SortByExperience()
	{
		SelectSortToggle(experienceSortingToggle);
		List<SkillMinionWidget> list = minionWidgets;
		list.Sort(delegate(SkillMinionWidget a, SkillMinionWidget b)
		{
			MinionIdentity minionIdentity = a.minion as MinionIdentity;
			MinionIdentity minionIdentity2 = b.minion as MinionIdentity;
			if (minionIdentity == null && minionIdentity2 == null)
			{
				return 0;
			}
			if (minionIdentity == null)
			{
				return -1;
			}
			if (minionIdentity2 == null)
			{
				return 1;
			}
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			MinionResume component2 = minionIdentity2.GetComponent<MinionResume>();
			float num = component.AvailableSkillpoints / (component.TotalSkillPointsGained + 1);
			float value = component2.AvailableSkillpoints / (component2.TotalSkillPointsGained + 1);
			return num.CompareTo(value);
		});
		ReorderEntries(list, sortReversed);
	}

	protected void ReorderEntries(List<SkillMinionWidget> sortedEntries, bool reverse)
	{
		for (int i = 0; i < sortedEntries.Count; i++)
		{
			if (reverse)
			{
				sortedEntries[i].transform.SetSiblingIndex(sortedEntries.Count - 1 - i);
			}
			else
			{
				sortedEntries[i].transform.SetSiblingIndex(i);
			}
		}
	}

	private void SetPortraitAnimator(IAssignableIdentity identity)
	{
		if (identity == null || identity.IsNull())
		{
			return;
		}
		if (animController == null)
		{
			animController = Util.KInstantiateUI(Assets.GetPrefab(new Tag("FullMinionUIPortrait")), duplicantAnimAnchor.gameObject).GetComponent<KBatchedAnimController>();
			animController.gameObject.SetActive(value: true);
			animController.animScale = baseCharacterScale;
		}
		string value = "";
		Accessorizer component = animController.GetComponent<Accessorizer>();
		for (int num = component.GetAccessories().Count - 1; num >= 0; num--)
		{
			component.RemoveAccessory(component.GetAccessories()[num].Get());
		}
		MinionIdentity minionIdentity = identity as MinionIdentity;
		StoredMinionIdentity storedMinionIdentity = identity as StoredMinionIdentity;
		Accessorizer accessorizer = null;
		if (minionIdentity != null)
		{
			accessorizer = minionIdentity.GetComponent<Accessorizer>();
			foreach (ResourceRef<Accessory> accessory in accessorizer.GetAccessories())
			{
				component.AddAccessory(accessory.Get());
			}
			value = minionIdentity.GetComponent<MinionResume>().CurrentHat;
		}
		else if (storedMinionIdentity != null)
		{
			foreach (ResourceRef<Accessory> accessory2 in storedMinionIdentity.accessories)
			{
				component.AddAccessory(accessory2.Get());
			}
			value = storedMinionIdentity.currentHat;
		}
		HashedString name = "anim_idle_healthy_kanim";
		idle_anim = Assets.GetAnim(name);
		if (idle_anim != null)
		{
			animController.AddAnimOverrides(idle_anim);
		}
		animController.Queue("idle_default", KAnim.PlayMode.Loop);
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		animController.SetSymbolVisiblity(hat.targetSymbolId, (!string.IsNullOrEmpty(value)) ? true : false);
		animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, string.IsNullOrEmpty(value) ? true : false);
		animController.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, (!string.IsNullOrEmpty(value)) ? true : false);
		KAnim.Build.Symbol source_symbol = null;
		KAnim.Build.Symbol source_symbol2 = null;
		if ((bool)accessorizer)
		{
			source_symbol = accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
			source_symbol2 = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
		}
		else if (storedMinionIdentity != null)
		{
			source_symbol = storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
			source_symbol2 = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
		}
		animController.GetComponent<SymbolOverrideController>().AddSymbolOverride(Db.Get().AccessorySlots.HairAlways.targetSymbolId, source_symbol, 1);
		animController.GetComponent<SymbolOverrideController>().AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, source_symbol2, 1);
	}
}
