using System;
using System.Collections.Generic;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllDiagnosticsScreen : KScreen, ISim4000ms, ISim1000ms
{
	private Dictionary<string, GameObject> diagnosticRows = new Dictionary<string, GameObject>();

	public GameObject rootListContainer;

	public GameObject resourceLinePrefab;

	public KButton closeButton;

	public bool allowRefresh = true;

	[SerializeField]
	private TMP_InputField searchInputField;

	[SerializeField]
	private KButton clearSearchButton;

	public static AllDiagnosticsScreen Instance;

	public Dictionary<Tag, bool> currentlyDisplayedRows = new Dictionary<Tag, bool>();

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
		GameObject gameObject = Util.KInstantiateUI(resourceLinePrefab, container, force_active: true);
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
				int activeWorldId = ClusterManager.Instance.activeWorldId;
				int num = (int)ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[activeWorldId][id];
				int num2 = num - 1;
				if (num2 < 0)
				{
					num2 = 2;
				}
				ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[activeWorldId][id] = (ColonyDiagnosticUtility.DisplaySetting)num2;
			}
			RefreshRows();
			ColonyDiagnosticScreen.Instance.RefreshAll();
		});
		gameObject.GetComponent<MultiToggle>().onClick = reference.onClick;
		component.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().axis_x.min_value = 0f;
		component.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().axis_x.max_value = 600f;
		component.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().axis_x.guide_frequency = 120f;
		component.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().RefreshGuides();
		diagnosticRows.Add(id2, gameObject);
		currentlyDisplayedRows.Add(id2, value: true);
		component.GetReference<Image>("Icon").sprite = Assets.GetSprite(diagnostic.icon);
		RefreshPinnedState(id2);
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
		filter = filter.ToUpper();
		string text = ColonyDiagnosticUtility.Instance.GetDiagnosticName(tag.ToString()).ToUpper();
		if (filter != "" && !text.Contains(filter) && !tag.Name.ToUpper().Contains(filter))
		{
			return false;
		}
		return true;
	}

	private void RefreshPinnedState(string diagnosticID)
	{
		if (!ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[ClusterManager.Instance.activeWorldId].ContainsKey(diagnosticID))
		{
			return;
		}
		if (ColonyDiagnosticUtility.Instance.IsDiagnosticTutorialDisabled(diagnosticID))
		{
			diagnosticRows[diagnosticID].GetComponent<HierarchyReferences>().GetReference<MultiToggle>("PinToggle").ChangeState(3);
			return;
		}
		switch (ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[ClusterManager.Instance.activeWorldId][diagnosticID])
		{
		case ColonyDiagnosticUtility.DisplaySetting.Never:
			diagnosticRows[diagnosticID].GetComponent<HierarchyReferences>().GetReference<MultiToggle>("PinToggle").ChangeState(0);
			break;
		case ColonyDiagnosticUtility.DisplaySetting.AlertOnly:
			diagnosticRows[diagnosticID].GetComponent<HierarchyReferences>().GetReference<MultiToggle>("PinToggle").ChangeState(1);
			break;
		case ColonyDiagnosticUtility.DisplaySetting.Always:
			diagnosticRows[diagnosticID].GetComponent<HierarchyReferences>().GetReference<MultiToggle>("PinToggle").ChangeState(2);
			break;
		}
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
				ColonyDiagnostic diagnostic = ColonyDiagnosticUtility.Instance.GetDiagnostic(diagnosticRow.Key, ClusterManager.Instance.activeWorldId);
				if (diagnostic != null)
				{
					component.GetReference<LocText>("AvailableLabel").SetText(diagnostic.GetAverageValueString());
					component.GetReference<Image>("Indicator").color = diagnostic.colors[diagnostic.LatestResult.opinion];
					string str = "";
					switch (ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[ClusterManager.Instance.activeWorldId][diagnostic.id])
					{
					case ColonyDiagnosticUtility.DisplaySetting.Always:
						str = UI.DIAGNOSTICS_SCREEN.CLICK_TOGGLE_MESSAGE.NEVER;
						break;
					case ColonyDiagnosticUtility.DisplaySetting.AlertOnly:
						str = UI.DIAGNOSTICS_SCREEN.CLICK_TOGGLE_MESSAGE.ALWAYS;
						break;
					case ColonyDiagnosticUtility.DisplaySetting.Never:
						str = UI.DIAGNOSTICS_SCREEN.CLICK_TOGGLE_MESSAGE.ALERT_ONLY;
						break;
					}
					diagnosticRow.Value.GetComponent<ToolTip>().refreshWhileHovering = true;
					diagnosticRow.Value.GetComponent<ToolTip>().SetSimpleTooltip(string.Concat(Strings.Get(new StringKey("STRINGS.UI.COLONY_DIAGNOSTICS." + diagnostic.id.ToUpper() + ".TOOLTIP_NAME")), "\n", diagnostic.LatestResult.Message, str));
				}
				RefreshPinnedState(diagnosticRow.Key);
			}
		}
		SetRowsActive();
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
