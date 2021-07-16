using System;
using System.Collections.Generic;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllDiagnosticsScreen : KScreen, ISim4000ms, ISim1000ms
{
	private Dictionary<string, GameObject> diagnosticRows = new Dictionary<string, GameObject>();

	private Dictionary<string, Dictionary<string, GameObject>> criteriaRows = new Dictionary<string, Dictionary<string, GameObject>>();

	public GameObject rootListContainer;

	public GameObject diagnosticLinePrefab;

	public GameObject subDiagnosticLinePrefab;

	public KButton closeButton;

	public bool allowRefresh = true;

	[SerializeField]
	private TMP_InputField searchInputField;

	[SerializeField]
	private KButton clearSearchButton;

	public static AllDiagnosticsScreen Instance;

	public Dictionary<Tag, bool> currentlyDisplayedRows = new Dictionary<Tag, bool>();

	public Dictionary<Tag, bool> subrowContainerOpen = new Dictionary<Tag, bool>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.ConsumeMouseScroll = true;
		Populate();
		Game.Instance.Subscribe(1983128072, Populate);
		Game.Instance.Subscribe(-1280433810, Populate);
		closeButton.onClick += delegate
		{
			Show(show: false);
		};
		clearSearchButton.onClick += delegate
		{
			searchInputField.text = "";
		};
		searchInputField.onValueChanged.AddListener(delegate(string value)
		{
			SearchFilter(value);
		});
		TMP_InputField tMP_InputField = searchInputField;
		tMP_InputField.onFocus = (System.Action)Delegate.Combine(tMP_InputField.onFocus, (System.Action)delegate
		{
			base.isEditing = true;
		});
		searchInputField.onEndEdit.AddListener(delegate
		{
			base.isEditing = false;
		});
		Show(show: false);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			ManagementMenu.Instance.CloseAll();
			AllResourcesScreen.Instance.Show(show: false);
			RefreshSubrows();
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
			Show(show: false);
			e.Consumed = true;
		}
		if (base.isEditing)
		{
			e.Consumed = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public int GetRowCount()
	{
		return diagnosticRows.Count;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
			Show(show: false);
			e.Consumed = true;
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
		}
	}

	public override float GetSortKey()
	{
		return 50f;
	}

	public void Populate(object data = null)
	{
		SpawnRows();
		foreach (string key2 in diagnosticRows.Keys)
		{
			Tag key = key2;
			currentlyDisplayedRows[key] = true;
		}
		SearchFilter(searchInputField.text);
		RefreshRows();
	}

	private void SpawnRows()
	{
		foreach (KeyValuePair<int, Dictionary<string, ColonyDiagnosticUtility.DisplaySetting>> diagnosticDisplaySetting in ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings)
		{
			foreach (KeyValuePair<string, ColonyDiagnosticUtility.DisplaySetting> item in ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[diagnosticDisplaySetting.Key])
			{
				if (!diagnosticRows.ContainsKey(item.Key))
				{
					ColonyDiagnostic diagnostic = ColonyDiagnosticUtility.Instance.GetDiagnostic(item.Key, diagnosticDisplaySetting.Key);
					if (!(diagnostic is WorkTimeDiagnostic) && !(diagnostic is ChoreGroupDiagnostic))
					{
						SpawnRow(diagnostic, rootListContainer);
					}
				}
			}
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, GameObject> diagnosticRow in diagnosticRows)
		{
			list.Add(diagnosticRow.Key);
		}
		list.Sort((string a, string b) => ColonyDiagnosticUtility.Instance.GetDiagnosticName(a).CompareTo(ColonyDiagnosticUtility.Instance.GetDiagnosticName(b)));
		foreach (string item2 in list)
		{
			diagnosticRows[item2].transform.SetAsLastSibling();
		}
	}

	private void SpawnRow(ColonyDiagnostic diagnostic, GameObject container)
	{
		if (diagnostic == null || diagnosticRows.ContainsKey(diagnostic.id))
		{
			return;
		}
		GameObject gameObject = Util.KInstantiateUI(diagnosticLinePrefab, container, force_active: true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<LocText>("NameLabel").SetText(diagnostic.name);
		string id2 = diagnostic.id;
		MultiToggle reference = component.GetReference<MultiToggle>("PinToggle");
		string id = diagnostic.id;
		reference.onClick = (System.Action)Delegate.Combine(reference.onClick, (System.Action)delegate
		{
			if (ColonyDiagnosticUtility.Instance.IsDiagnosticTutorialDisabled(diagnostic.id))
			{
				ColonyDiagnosticUtility.Instance.ClearDiagnosticTutorialSetting(diagnostic.id);
			}
			else
			{
				int activeWorldId2 = ClusterManager.Instance.activeWorldId;
				int num = (int)ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[activeWorldId2][id];
				int num2 = num - 1;
				if (num2 < 0)
				{
					num2 = 2;
				}
				ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[activeWorldId2][id] = (ColonyDiagnosticUtility.DisplaySetting)num2;
			}
			RefreshRows();
			ColonyDiagnosticScreen.Instance.RefreshAll();
		});
		GraphBase component2 = component.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>();
		component2.axis_x.min_value = 0f;
		component2.axis_x.max_value = 600f;
		component2.axis_x.guide_frequency = 120f;
		component2.RefreshGuides();
		diagnosticRows.Add(id2, gameObject);
		criteriaRows.Add(id2, new Dictionary<string, GameObject>());
		currentlyDisplayedRows.Add(id2, value: true);
		component.GetReference<Image>("Icon").sprite = Assets.GetSprite(diagnostic.icon);
		RefreshPinnedState(id2);
		RectTransform reference2 = component.GetReference<RectTransform>("SubRows");
		DiagnosticCriterion[] criteria = diagnostic.GetCriteria();
		foreach (DiagnosticCriterion sub in criteria)
		{
			GameObject gameObject2 = Util.KInstantiateUI(subDiagnosticLinePrefab, reference2.gameObject, force_active: true);
			gameObject2.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.DIAGNOSTICS_SCREEN.CRITERIA_TOOLTIP, diagnostic.name, sub.name));
			HierarchyReferences component3 = gameObject2.GetComponent<HierarchyReferences>();
			LocText reference3 = component3.GetReference<LocText>("Label");
			reference3.SetText(sub.name);
			criteriaRows[diagnostic.id].Add(sub.id, gameObject2);
			MultiToggle reference4 = component3.GetReference<MultiToggle>("PinToggle");
			reference4.onClick = (System.Action)Delegate.Combine(reference4.onClick, (System.Action)delegate
			{
				int activeWorldId = ClusterManager.Instance.activeWorldId;
				bool flag = ColonyDiagnosticUtility.Instance.IsCriteriaEnabled(activeWorldId, diagnostic.id, sub.id);
				ColonyDiagnosticUtility.Instance.SetCriteriaEnabled(activeWorldId, diagnostic.id, sub.id, !flag);
				RefreshSubrows();
			});
		}
		subrowContainerOpen.Add(diagnostic.id, value: false);
		MultiToggle reference5 = component.GetReference<MultiToggle>("SubrowToggle");
		reference5.onClick = (System.Action)Delegate.Combine(reference5.onClick, (System.Action)delegate
		{
			subrowContainerOpen[diagnostic.id] = !subrowContainerOpen[diagnostic.id];
			RefreshSubrows();
		});
		component.GetReference<MultiToggle>("MainToggle").onClick = reference5.onClick;
	}

	private void FilterRowBySearch(Tag tag, string filter)
	{
		currentlyDisplayedRows[tag] = PassesSearchFilter(tag, filter);
	}

	private void SearchFilter(string search)
	{
		foreach (KeyValuePair<string, GameObject> diagnosticRow in diagnosticRows)
		{
			FilterRowBySearch(diagnosticRow.Key, search);
		}
		foreach (KeyValuePair<string, GameObject> diagnosticRow2 in diagnosticRows)
		{
			currentlyDisplayedRows[diagnosticRow2.Key] = PassesSearchFilter(diagnosticRow2.Key, search);
		}
		SetRowsActive();
	}

	private bool PassesSearchFilter(Tag tag, string filter)
	{
		if (string.IsNullOrEmpty(filter))
		{
			return true;
		}
		filter = filter.ToUpper();
		string id = tag.ToString();
		string text = ColonyDiagnosticUtility.Instance.GetDiagnosticName(id).ToUpper();
		if (text.Contains(filter) || tag.Name.ToUpper().Contains(filter))
		{
			return true;
		}
		ColonyDiagnostic diagnostic = ColonyDiagnosticUtility.Instance.GetDiagnostic(id, ClusterManager.Instance.activeWorldId);
		if (diagnostic == null)
		{
			return false;
		}
		DiagnosticCriterion[] criteria = diagnostic.GetCriteria();
		if (criteria == null)
		{
			return false;
		}
		DiagnosticCriterion[] array = criteria;
		foreach (DiagnosticCriterion diagnosticCriterion in array)
		{
			if (diagnosticCriterion.name.ToUpper().Contains(filter))
			{
				return true;
			}
		}
		return false;
	}

	private void RefreshPinnedState(string diagnosticID)
	{
		if (!ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[ClusterManager.Instance.activeWorldId].ContainsKey(diagnosticID))
		{
			return;
		}
		MultiToggle reference = diagnosticRows[diagnosticID].GetComponent<HierarchyReferences>().GetReference<MultiToggle>("PinToggle");
		if (ColonyDiagnosticUtility.Instance.IsDiagnosticTutorialDisabled(diagnosticID))
		{
			reference.ChangeState(3);
		}
		else
		{
			switch (ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[ClusterManager.Instance.activeWorldId][diagnosticID])
			{
			case ColonyDiagnosticUtility.DisplaySetting.Never:
				reference.ChangeState(0);
				break;
			case ColonyDiagnosticUtility.DisplaySetting.AlertOnly:
				reference.ChangeState(1);
				break;
			case ColonyDiagnosticUtility.DisplaySetting.Always:
				reference.ChangeState(2);
				break;
			}
		}
		string simpleTooltip = "";
		if (ColonyDiagnosticUtility.Instance.IsDiagnosticTutorialDisabled(diagnosticID))
		{
			simpleTooltip = UI.DIAGNOSTICS_SCREEN.CLICK_TOGGLE_MESSAGE.TUTORIAL_DISABLED;
		}
		else
		{
			switch (ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[ClusterManager.Instance.activeWorldId][diagnosticID])
			{
			case ColonyDiagnosticUtility.DisplaySetting.Always:
				simpleTooltip = UI.DIAGNOSTICS_SCREEN.CLICK_TOGGLE_MESSAGE.NEVER;
				break;
			case ColonyDiagnosticUtility.DisplaySetting.AlertOnly:
				simpleTooltip = UI.DIAGNOSTICS_SCREEN.CLICK_TOGGLE_MESSAGE.ALWAYS;
				break;
			case ColonyDiagnosticUtility.DisplaySetting.Never:
				simpleTooltip = UI.DIAGNOSTICS_SCREEN.CLICK_TOGGLE_MESSAGE.ALERT_ONLY;
				break;
			}
		}
		reference.GetComponent<ToolTip>().SetSimpleTooltip(simpleTooltip);
	}

	public void RefreshRows()
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		if (allowRefresh)
		{
			foreach (KeyValuePair<string, GameObject> diagnosticRow in diagnosticRows)
			{
				HierarchyReferences component = diagnosticRow.Value.GetComponent<HierarchyReferences>();
				component.GetReference<LocText>("AvailableLabel").SetText(diagnosticRow.Key);
				component.GetReference<RectTransform>("SubRows").gameObject.SetActive(value: false);
				ColonyDiagnostic diagnostic = ColonyDiagnosticUtility.Instance.GetDiagnostic(diagnosticRow.Key, ClusterManager.Instance.activeWorldId);
				if (diagnostic != null)
				{
					component.GetReference<LocText>("AvailableLabel").SetText(diagnostic.GetAverageValueString());
					component.GetReference<Image>("Indicator").color = diagnostic.colors[diagnostic.LatestResult.opinion];
					ToolTip reference = component.GetReference<ToolTip>("Tooltip");
					reference.refreshWhileHovering = true;
					reference.SetSimpleTooltip(string.Concat(Strings.Get(new StringKey("STRINGS.UI.COLONY_DIAGNOSTICS." + diagnostic.id.ToUpper() + ".TOOLTIP_NAME")), "\n", diagnostic.LatestResult.Message));
				}
				RefreshPinnedState(diagnosticRow.Key);
			}
		}
		SetRowsActive();
		RefreshSubrows();
	}

	private void RefreshSubrows()
	{
		foreach (KeyValuePair<string, GameObject> diagnosticRow in diagnosticRows)
		{
			DebugUtil.DevAssert(subrowContainerOpen.ContainsKey(diagnosticRow.Key), "AllDiagnosticsScreen subrowContainerOpen does not contain key " + diagnosticRow.Key + " - it should have been added in SpawnRows");
			HierarchyReferences component = diagnosticRow.Value.GetComponent<HierarchyReferences>();
			component.GetReference<MultiToggle>("SubrowToggle").ChangeState(subrowContainerOpen[diagnosticRow.Key] ? 1 : 0);
			RectTransform reference = component.GetReference<RectTransform>("SubRows");
			reference.gameObject.SetActive(subrowContainerOpen[diagnosticRow.Key]);
			int num = 0;
			foreach (KeyValuePair<string, GameObject> item in criteriaRows[diagnosticRow.Key])
			{
				GameObject value = item.Value;
				HierarchyReferences component2 = value.GetComponent<HierarchyReferences>();
				MultiToggle reference2 = component2.GetReference<MultiToggle>("PinToggle");
				int activeWorldId = ClusterManager.Instance.activeWorldId;
				string key = diagnosticRow.Key;
				string key2 = item.Key;
				bool flag = ColonyDiagnosticUtility.Instance.IsCriteriaEnabled(activeWorldId, key, key2);
				reference2.ChangeState(flag ? 1 : 0);
				if (flag)
				{
					num++;
				}
			}
			component.GetReference<LocText>("SubrowHeaderLabel").SetText(string.Format(UI.DIAGNOSTICS_SCREEN.CRITERIA_ENABLED_COUNT, num, criteriaRows[diagnosticRow.Key].Count));
		}
	}

	private void RefreshCharts()
	{
		foreach (KeyValuePair<string, GameObject> diagnosticRow in diagnosticRows)
		{
			HierarchyReferences component = diagnosticRow.Value.GetComponent<HierarchyReferences>();
			ColonyDiagnostic diagnostic = ColonyDiagnosticUtility.Instance.GetDiagnostic(diagnosticRow.Key, ClusterManager.Instance.activeWorldId);
			if (diagnostic != null)
			{
				SparkLayer reference = component.GetReference<SparkLayer>("Chart");
				Tracker tracker = diagnostic.tracker;
				if (tracker != null)
				{
					float num = 3000f;
					Tuple<float, float>[] array = tracker.ChartableData(num);
					reference.graph.axis_x.max_value = array[array.Length - 1].first;
					reference.graph.axis_x.min_value = reference.graph.axis_x.max_value - num;
					reference.RefreshLine(array, "resourceAmount");
				}
			}
		}
	}

	private void SetRowsActive()
	{
		foreach (KeyValuePair<string, GameObject> diagnosticRow in diagnosticRows)
		{
			if (ColonyDiagnosticUtility.Instance.GetDiagnostic(diagnosticRow.Key, ClusterManager.Instance.activeWorldId) == null)
			{
				currentlyDisplayedRows[diagnosticRow.Key] = false;
			}
		}
		foreach (KeyValuePair<string, GameObject> diagnosticRow2 in diagnosticRows)
		{
			if (diagnosticRow2.Value.activeSelf != currentlyDisplayedRows[diagnosticRow2.Key])
			{
				diagnosticRow2.Value.SetActive(currentlyDisplayedRows[diagnosticRow2.Key]);
			}
		}
	}

	public void Sim4000ms(float dt)
	{
		RefreshCharts();
	}

	public void Sim1000ms(float dt)
	{
		RefreshRows();
	}
}