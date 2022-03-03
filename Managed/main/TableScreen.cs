using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TableScreen : KScreen
{
	public enum ResultValues
	{
		False,
		Partial,
		True,
		ConditionalGroup
	}

	protected string title;

	protected bool has_default_duplicant_row = true;

	protected bool useWorldDividers = true;

	private bool rows_dirty;

	protected Comparison<IAssignableIdentity> active_sort_method;

	protected TableColumn active_sort_column;

	protected bool sort_is_reversed;

	private int active_cascade_coroutine_count;

	private HandleVector<int>.Handle current_looping_sound = HandleVector<int>.InvalidHandle;

	private bool incubating;

	private int removeWorldHandle = -1;

	protected Dictionary<string, TableColumn> columns = new Dictionary<string, TableColumn>();

	public List<TableRow> rows = new List<TableRow>();

	public List<TableRow> all_sortable_rows = new List<TableRow>();

	public List<string> column_scrollers = new List<string>();

	private Dictionary<GameObject, TableRow> known_widget_rows = new Dictionary<GameObject, TableRow>();

	private Dictionary<GameObject, TableColumn> known_widget_columns = new Dictionary<GameObject, TableColumn>();

	public GameObject prefab_row_empty;

	public GameObject prefab_row_header;

	public GameObject prefab_world_divider;

	public GameObject prefab_scroller_border;

	private string cascade_sound_path = GlobalAssets.GetSound("Placers_Unfurl_LP");

	public KButton CloseButton;

	[MyCmpGet]
	private VerticalLayoutGroup VLG;

	protected GameObject header_row;

	protected GameObject default_row;

	public LocText title_bar;

	public Transform header_content_transform;

	public Transform scroll_content_transform;

	public Transform scroller_borders_transform;

	public Dictionary<int, GameObject> worldDividers = new Dictionary<int, GameObject>();

	private bool scrollersDirty;

	private float targetScrollerPosition;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		removeWorldHandle = ClusterManager.Instance.Subscribe(-1078710002, RemoveWorldDivider);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		title_bar.text = title;
		base.ConsumeMouseScroll = true;
		CloseButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		incubating = true;
		base.transform.rectTransform().localScale = Vector3.zero;
		Components.LiveMinionIdentities.OnAdd += delegate
		{
			MarkRowsDirty();
		};
		Components.LiveMinionIdentities.OnRemove += delegate
		{
			MarkRowsDirty();
		};
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (removeWorldHandle != -1)
		{
			ClusterManager.Instance.Unsubscribe(removeWorldHandle);
		}
	}

	protected override void OnShow(bool show)
	{
		if (!show)
		{
			active_cascade_coroutine_count = 0;
			StopAllCoroutines();
			StopLoopingCascadeSound();
		}
		ZeroScrollers();
		base.OnShow(show);
		rows_dirty = true;
	}

	private void ZeroScrollers()
	{
		if (rows.Count <= 0)
		{
			return;
		}
		foreach (string column_scroller in column_scrollers)
		{
			foreach (TableRow row in rows)
			{
				row.GetScroller(column_scroller).transform.parent.GetComponent<ScrollRect>().horizontalNormalizedPosition = 0f;
			}
		}
		foreach (KeyValuePair<int, GameObject> worldDivider in worldDividers)
		{
			ScrollRect componentInChildren = worldDivider.Value.GetComponentInChildren<ScrollRect>();
			if (componentInChildren != null)
			{
				componentInChildren.horizontalNormalizedPosition = 0f;
			}
		}
	}

	public bool CheckScrollersDirty()
	{
		return scrollersDirty;
	}

	public void SetScrollersDirty(float position)
	{
		targetScrollerPosition = position;
		scrollersDirty = true;
		PositionScrollers();
	}

	public void PositionScrollers()
	{
		foreach (TableRow row in rows)
		{
			ScrollRect componentInChildren = row.GetComponentInChildren<ScrollRect>();
			if (componentInChildren != null)
			{
				componentInChildren.horizontalNormalizedPosition = targetScrollerPosition;
			}
		}
		foreach (KeyValuePair<int, GameObject> worldDivider in worldDividers)
		{
			if (worldDivider.Value.activeInHierarchy)
			{
				ScrollRect componentInChildren2 = worldDivider.Value.GetComponentInChildren<ScrollRect>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.horizontalNormalizedPosition = targetScrollerPosition;
				}
			}
		}
		scrollersDirty = false;
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		if (incubating)
		{
			ZeroScrollers();
			base.transform.rectTransform().localScale = Vector3.one;
			incubating = false;
		}
		if (rows_dirty)
		{
			RefreshRows();
		}
		foreach (TableRow row in rows)
		{
			row.RefreshScrollers();
		}
		foreach (TableColumn value in columns.Values)
		{
			if (!value.isDirty)
			{
				continue;
			}
			foreach (KeyValuePair<TableRow, GameObject> item in value.widgets_by_row)
			{
				value.on_load_action(item.Key.GetIdentity(), item.Value);
				value.MarkClean();
			}
		}
	}

	protected void MarkRowsDirty()
	{
		rows_dirty = true;
	}

	protected virtual void RefreshRows()
	{
		ClearRows();
		AddRow(null);
		if (has_default_duplicant_row)
		{
			AddDefaultRow();
		}
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			if (Components.LiveMinionIdentities[i] != null)
			{
				AddRow(Components.LiveMinionIdentities[i]);
			}
		}
		foreach (MinionStorage item in Components.MinionStorages.Items)
		{
			foreach (MinionStorage.Info item2 in item.GetStoredMinionInfo())
			{
				if (item2.serializedMinion != null)
				{
					StoredMinionIdentity minion = item2.serializedMinion.Get<StoredMinionIdentity>();
					AddRow(minion);
				}
			}
		}
		foreach (int item3 in ClusterManager.Instance.GetWorldIDsSorted())
		{
			AddWorldDivider(item3);
		}
		foreach (KeyValuePair<int, GameObject> worldDivider in worldDividers)
		{
			Component reference = worldDivider.Value.GetComponent<HierarchyReferences>().GetReference("NobodyRow");
			reference.gameObject.SetActive(value: true);
			foreach (MinionAssignablesProxy item4 in Components.MinionAssignablesProxy)
			{
				if (item4 != null && item4.GetTargetGameObject() != null && item4.GetTargetGameObject().GetMyWorld().id == worldDivider.Key)
				{
					reference.gameObject.SetActive(value: false);
					break;
				}
			}
			worldDivider.Value.SetActive(ClusterManager.Instance.GetWorld(worldDivider.Key).IsDiscovered && DlcManager.FeatureClusterSpaceEnabled());
		}
		SortRows();
		rows_dirty = false;
	}

	public virtual void SetSortComparison(Comparison<IAssignableIdentity> comparison, TableColumn sort_column)
	{
		if (comparison == null)
		{
			return;
		}
		if (active_sort_column == sort_column)
		{
			if (sort_is_reversed)
			{
				sort_is_reversed = false;
				active_sort_method = null;
				active_sort_column = null;
			}
			else
			{
				sort_is_reversed = true;
			}
		}
		else
		{
			active_sort_column = sort_column;
			active_sort_method = comparison;
			sort_is_reversed = false;
		}
	}

	public void SortRows()
	{
		foreach (TableColumn value in columns.Values)
		{
			if (value.column_sort_toggle == null)
			{
				continue;
			}
			if (value == active_sort_column)
			{
				if (sort_is_reversed)
				{
					value.column_sort_toggle.ChangeState(2);
				}
				else
				{
					value.column_sort_toggle.ChangeState(1);
				}
			}
			else
			{
				value.column_sort_toggle.ChangeState(0);
			}
		}
		Dictionary<IAssignableIdentity, TableRow> dictionary = new Dictionary<IAssignableIdentity, TableRow>();
		foreach (TableRow all_sortable_row in all_sortable_rows)
		{
			dictionary.Add(all_sortable_row.GetIdentity(), all_sortable_row);
		}
		Dictionary<int, List<IAssignableIdentity>> dictionary2 = new Dictionary<int, List<IAssignableIdentity>>();
		foreach (KeyValuePair<IAssignableIdentity, TableRow> item in dictionary)
		{
			int id = item.Key.GetSoleOwner().GetComponent<MinionAssignablesProxy>().GetTargetGameObject()
				.GetComponent<KMonoBehaviour>()
				.GetMyWorld()
				.id;
			if (!dictionary2.ContainsKey(id))
			{
				dictionary2.Add(id, new List<IAssignableIdentity>());
			}
			dictionary2[id].Add(item.Key);
		}
		all_sortable_rows.Clear();
		Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
		int num = 0;
		int num2 = 0;
		foreach (KeyValuePair<int, List<IAssignableIdentity>> item2 in dictionary2)
		{
			dictionary3.Add(item2.Key, num);
			num++;
			List<IAssignableIdentity> list = new List<IAssignableIdentity>();
			foreach (IAssignableIdentity item3 in item2.Value)
			{
				list.Add(item3);
			}
			if (active_sort_method != null)
			{
				list.Sort(active_sort_method);
				if (sort_is_reversed)
				{
					list.Reverse();
				}
			}
			num += list.Count;
			num2 += list.Count;
			for (int i = 0; i < list.Count; i++)
			{
				all_sortable_rows.Add(dictionary[list[i]]);
			}
		}
		for (int j = 0; j < all_sortable_rows.Count; j++)
		{
			all_sortable_rows[j].gameObject.transform.SetSiblingIndex(j);
		}
		foreach (KeyValuePair<int, int> item4 in dictionary3)
		{
			worldDividers[item4.Key].transform.SetSiblingIndex(item4.Value);
		}
		if (has_default_duplicant_row)
		{
			default_row.transform.SetAsFirstSibling();
		}
	}

	protected int compare_rows_alphabetical(IAssignableIdentity a, IAssignableIdentity b)
	{
		if (a == null && b == null)
		{
			return 0;
		}
		if (a == null)
		{
			return -1;
		}
		if (b == null)
		{
			return 1;
		}
		return a.GetProperName().CompareTo(b.GetProperName());
	}

	protected int default_sort(TableRow a, TableRow b)
	{
		return 0;
	}

	protected void ClearRows()
	{
		for (int num = rows.Count - 1; num >= 0; num--)
		{
			rows[num].Clear();
		}
		rows.Clear();
		all_sortable_rows.Clear();
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, GameObject> worldDivider in worldDividers)
		{
			list.Add(worldDivider.Key);
		}
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			RemoveWorldDivider(list[num2]);
		}
		worldDividers.Clear();
	}

	protected void AddRow(IAssignableIdentity minion)
	{
		bool flag = minion == null;
		GameObject gameObject = Util.KInstantiateUI(flag ? prefab_row_header : prefab_row_empty, (minion == null) ? header_content_transform.gameObject : scroll_content_transform.gameObject, force_active: true);
		TableRow component = gameObject.GetComponent<TableRow>();
		component.rowType = ((!flag) ? ((minion as MinionIdentity != null) ? TableRow.RowType.Minion : TableRow.RowType.StoredMinon) : TableRow.RowType.Header);
		rows.Add(component);
		component.ConfigureContent(minion, columns, this);
		if (!flag)
		{
			all_sortable_rows.Add(component);
		}
		else
		{
			header_row = gameObject;
		}
	}

	protected void AddDefaultRow()
	{
		TableRow component = (default_row = Util.KInstantiateUI(prefab_row_empty, scroll_content_transform.gameObject, force_active: true)).GetComponent<TableRow>();
		component.rowType = TableRow.RowType.Default;
		component.isDefault = true;
		rows.Add(component);
		component.ConfigureContent(null, columns, this);
	}

	protected void AddWorldDivider(int worldId)
	{
		if (!worldDividers.ContainsKey(worldId))
		{
			GameObject gameObject = Util.KInstantiateUI(prefab_world_divider, scroll_content_transform.gameObject, force_active: true);
			gameObject.GetComponentInChildren<Image>().color = ClusterManager.worldColors[worldId % ClusterManager.worldColors.Length];
			RectTransform component = gameObject.GetComponentInChildren<LocText>().GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(150f, component.sizeDelta.y);
			ClusterGridEntity component2 = ClusterManager.Instance.GetWorld(worldId).GetComponent<ClusterGridEntity>();
			string text = ((component2 is Clustercraft) ? NAMEGEN.WORLD.SPACECRAFT_PREFIX : NAMEGEN.WORLD.PLANETOID_PREFIX);
			gameObject.GetComponentInChildren<LocText>().SetText(text + component2.Name);
			gameObject.GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(NAMEGEN.WORLD.WORLDDIVIDER_TOOLTIP, component2.Name));
			gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = component2.GetUISprite();
			worldDividers.Add(worldId, gameObject);
			gameObject.GetComponent<TableRow>().ConfigureAsWorldDivider(columns, this);
		}
	}

	protected void RemoveWorldDivider(object worldId)
	{
		if (worldDividers.ContainsKey((int)worldId))
		{
			rows.Remove(worldDividers[(int)worldId].GetComponent<TableRow>());
			Util.KDestroyGameObject(worldDividers[(int)worldId]);
			worldDividers.Remove((int)worldId);
		}
	}

	protected TableRow GetWidgetRow(GameObject widget_go)
	{
		if (widget_go == null)
		{
			Debug.LogWarning("Widget is null");
			return null;
		}
		if (known_widget_rows.ContainsKey(widget_go))
		{
			return known_widget_rows[widget_go];
		}
		foreach (TableRow row in rows)
		{
			if (row.rowType != TableRow.RowType.WorldDivider && row.ContainsWidget(widget_go))
			{
				known_widget_rows.Add(widget_go, row);
				return row;
			}
		}
		Debug.LogWarning("Row is null for widget: " + widget_go.name + " parent is " + widget_go.transform.parent.name);
		return null;
	}

	protected void StartScrollableContent(string scrollablePanelID)
	{
		if (!column_scrollers.Contains(scrollablePanelID))
		{
			DividerColumn new_column = new DividerColumn(() => true);
			RegisterColumn("scroller_spacer_" + scrollablePanelID, new_column);
			column_scrollers.Add(scrollablePanelID);
		}
	}

	protected PortraitTableColumn AddPortraitColumn(string id, Action<IAssignableIdentity, GameObject> on_load_action, Comparison<IAssignableIdentity> sort_comparison, bool double_click_to_target = true)
	{
		PortraitTableColumn portraitTableColumn = new PortraitTableColumn(on_load_action, sort_comparison, double_click_to_target);
		if (RegisterColumn(id, portraitTableColumn))
		{
			return portraitTableColumn;
		}
		return null;
	}

	protected ButtonLabelColumn AddButtonLabelColumn(string id, Action<IAssignableIdentity, GameObject> on_load_action, Func<IAssignableIdentity, GameObject, string> get_value_action, Action<GameObject> on_click_action, Action<GameObject> on_double_click_action, Comparison<IAssignableIdentity> sort_comparison, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip, Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip, bool whiteText = false)
	{
		ButtonLabelColumn buttonLabelColumn = new ButtonLabelColumn(on_load_action, get_value_action, on_click_action, on_double_click_action, sort_comparison, on_tooltip, on_sort_tooltip, whiteText);
		if (RegisterColumn(id, buttonLabelColumn))
		{
			return buttonLabelColumn;
		}
		return null;
	}

	protected LabelTableColumn AddLabelColumn(string id, Action<IAssignableIdentity, GameObject> on_load_action, Func<IAssignableIdentity, GameObject, string> get_value_action, Comparison<IAssignableIdentity> sort_comparison, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip, Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip, int widget_width = 128, bool should_refresh_columns = false)
	{
		LabelTableColumn labelTableColumn = new LabelTableColumn(on_load_action, get_value_action, sort_comparison, on_tooltip, on_sort_tooltip, widget_width, should_refresh_columns);
		if (RegisterColumn(id, labelTableColumn))
		{
			return labelTableColumn;
		}
		return null;
	}

	protected CheckboxTableColumn AddCheckboxColumn(string id, Action<IAssignableIdentity, GameObject> on_load_action, Func<IAssignableIdentity, GameObject, ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, ResultValues> set_value_function, Comparison<IAssignableIdentity> sort_comparison, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip, Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip)
	{
		CheckboxTableColumn checkboxTableColumn = new CheckboxTableColumn(on_load_action, get_value_action, on_press_action, set_value_function, sort_comparison, on_tooltip, on_sort_tooltip);
		if (RegisterColumn(id, checkboxTableColumn))
		{
			return checkboxTableColumn;
		}
		return null;
	}

	protected SuperCheckboxTableColumn AddSuperCheckboxColumn(string id, CheckboxTableColumn[] columns_affected, Action<IAssignableIdentity, GameObject> on_load_action, Func<IAssignableIdentity, GameObject, ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, ResultValues> set_value_action, Comparison<IAssignableIdentity> sort_comparison, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = new SuperCheckboxTableColumn(columns_affected, on_load_action, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip);
		if (RegisterColumn(id, superCheckboxTableColumn))
		{
			foreach (CheckboxTableColumn obj in columns_affected)
			{
				obj.on_set_action = (Action<GameObject, ResultValues>)Delegate.Combine(obj.on_set_action, new Action<GameObject, ResultValues>(superCheckboxTableColumn.MarkDirty));
			}
			superCheckboxTableColumn.MarkDirty();
			return superCheckboxTableColumn;
		}
		Debug.LogWarning("SuperCheckbox column registration failed");
		return null;
	}

	protected NumericDropDownTableColumn AddNumericDropDownColumn(string id, object user_data, List<TMP_Dropdown.OptionData> options, Action<IAssignableIdentity, GameObject> on_load_action, Action<GameObject, int> set_value_action, Comparison<IAssignableIdentity> sort_comparison, NumericDropDownTableColumn.ToolTipCallbacks tooltip_callbacks)
	{
		NumericDropDownTableColumn numericDropDownTableColumn = new NumericDropDownTableColumn(user_data, options, on_load_action, set_value_action, sort_comparison, tooltip_callbacks);
		if (RegisterColumn(id, numericDropDownTableColumn))
		{
			return numericDropDownTableColumn;
		}
		return null;
	}

	protected bool RegisterColumn(string id, TableColumn new_column)
	{
		if (columns.ContainsKey(id))
		{
			Debug.LogWarning($"Column with id {id} already in dictionary");
			return false;
		}
		new_column.screen = this;
		columns.Add(id, new_column);
		MarkRowsDirty();
		return true;
	}

	protected TableColumn GetWidgetColumn(GameObject widget_go)
	{
		if (known_widget_columns.ContainsKey(widget_go))
		{
			return known_widget_columns[widget_go];
		}
		foreach (KeyValuePair<string, TableColumn> column in columns)
		{
			if (column.Value.ContainsWidget(widget_go))
			{
				known_widget_columns.Add(widget_go, column.Value);
				return column.Value;
			}
		}
		Debug.LogWarning("No column found for widget gameobject " + widget_go.name);
		return null;
	}

	protected void on_load_portrait(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		CrewPortrait component = widget_go.GetComponent<CrewPortrait>();
		if (minion != null)
		{
			component.SetIdentityObject(minion, jobEnabled: false);
		}
		else
		{
			component.targetImage.enabled = widgetRow.rowType == TableRow.RowType.Default;
		}
	}

	protected void on_load_name_label(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText locText = null;
		LocText locText2 = null;
		HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
		locText = component.GetReference("Label") as LocText;
		if (component.HasReference("SubLabel"))
		{
			locText2 = component.GetReference("SubLabel") as LocText;
		}
		if (minion != null)
		{
			locText.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
			if (locText2 != null)
			{
				MinionIdentity minionIdentity = minion as MinionIdentity;
				if (minionIdentity != null)
				{
					locText2.text = minionIdentity.gameObject.GetComponent<MinionResume>().GetSkillsSubtitle();
				}
				else
				{
					locText2.text = "";
				}
				locText2.enableWordWrapping = false;
			}
			return;
		}
		if (widgetRow.isDefault)
		{
			locText.text = UI.JOBSCREEN_DEFAULT;
			if (locText2 != null)
			{
				locText2.gameObject.SetActive(value: false);
			}
		}
		else
		{
			locText.text = UI.JOBSCREEN_EVERYONE;
		}
		if (locText2 != null)
		{
			locText2.text = "";
		}
	}

	protected string get_value_name_label(IAssignableIdentity minion, GameObject widget_go)
	{
		return minion.GetProperName();
	}

	protected void on_load_value_checkbox_column_super(IAssignableIdentity minion, GameObject widget_go)
	{
		MultiToggle multiToggle = null;
		multiToggle = widget_go.GetComponent<MultiToggle>();
		TableRow.RowType rowType = GetWidgetRow(widget_go).rowType;
		if ((uint)rowType <= 2u)
		{
			multiToggle.ChangeState((int)get_value_checkbox_column_super(minion, widget_go));
		}
	}

	public virtual ResultValues get_value_checkbox_column_super(IAssignableIdentity minion, GameObject widget_go)
	{
		SuperCheckboxTableColumn obj = GetWidgetColumn(widget_go) as SuperCheckboxTableColumn;
		TableRow widgetRow = GetWidgetRow(widget_go);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		CheckboxTableColumn[] columns_affected = obj.columns_affected;
		foreach (CheckboxTableColumn checkboxTableColumn in columns_affected)
		{
			if (!checkboxTableColumn.isRevealed)
			{
				continue;
			}
			switch (checkboxTableColumn.get_value_action(widgetRow.GetIdentity(), widgetRow.GetWidget(checkboxTableColumn)))
			{
			case ResultValues.False:
				flag2 = false;
				if (!flag)
				{
					flag5 = true;
				}
				break;
			case ResultValues.Partial:
				flag4 = true;
				flag5 = true;
				break;
			case ResultValues.True:
				flag4 = true;
				flag = false;
				if (!flag2)
				{
					flag5 = true;
				}
				break;
			case ResultValues.ConditionalGroup:
				flag3 = true;
				flag2 = false;
				flag = false;
				break;
			}
		}
		ResultValues result = ResultValues.Partial;
		if (flag3 && !flag4 && !flag2 && !flag)
		{
			result = ResultValues.ConditionalGroup;
		}
		else if (flag2)
		{
			result = ResultValues.True;
		}
		else if (flag)
		{
			result = ResultValues.False;
		}
		else if (flag4)
		{
			result = ResultValues.Partial;
		}
		return result;
	}

	protected void set_value_checkbox_column_super(GameObject widget_go, ResultValues new_value)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = GetWidgetColumn(widget_go) as SuperCheckboxTableColumn;
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			StartCoroutine(CascadeSetRowCheckBoxes(superCheckboxTableColumn.columns_affected, default_row.GetComponent<TableRow>(), new_value, widget_go));
			StartCoroutine(CascadeSetColumnCheckBoxes(all_sortable_rows, superCheckboxTableColumn, new_value, widget_go));
			break;
		case TableRow.RowType.Default:
			StartCoroutine(CascadeSetRowCheckBoxes(superCheckboxTableColumn.columns_affected, widgetRow, new_value, widget_go));
			break;
		case TableRow.RowType.Minion:
			StartCoroutine(CascadeSetRowCheckBoxes(superCheckboxTableColumn.columns_affected, widgetRow, new_value, widget_go));
			break;
		}
	}

	protected IEnumerator CascadeSetRowCheckBoxes(CheckboxTableColumn[] checkBoxToggleColumns, TableRow row, ResultValues state, GameObject ignore_widget = null)
	{
		if (active_cascade_coroutine_count == 0)
		{
			current_looping_sound = LoopingSoundManager.StartSound(cascade_sound_path, Vector3.zero, pause_on_game_pause: false, enable_culling: false);
		}
		active_cascade_coroutine_count++;
		for (int i = 0; i < checkBoxToggleColumns.Length; i++)
		{
			if (!checkBoxToggleColumns[i].widgets_by_row.ContainsKey(row))
			{
				continue;
			}
			GameObject gameObject = checkBoxToggleColumns[i].widgets_by_row[row];
			if (!(gameObject == ignore_widget) && checkBoxToggleColumns[i].isRevealed)
			{
				bool flag = false;
				switch ((GetWidgetColumn(gameObject) as CheckboxTableColumn).get_value_action(row.GetIdentity(), gameObject))
				{
				case ResultValues.False:
					flag = ((state != 0) ? true : false);
					break;
				case ResultValues.Partial:
				case ResultValues.ConditionalGroup:
					flag = true;
					break;
				case ResultValues.True:
					flag = ((state != ResultValues.True) ? true : false);
					break;
				}
				if (flag)
				{
					(GetWidgetColumn(gameObject) as CheckboxTableColumn).on_set_action(gameObject, state);
					yield return null;
				}
			}
		}
		active_cascade_coroutine_count--;
		if (active_cascade_coroutine_count <= 0)
		{
			StopLoopingCascadeSound();
		}
	}

	protected IEnumerator CascadeSetColumnCheckBoxes(List<TableRow> rows, CheckboxTableColumn checkBoxToggleColumn, ResultValues state, GameObject header_widget_go = null)
	{
		if (active_cascade_coroutine_count == 0)
		{
			current_looping_sound = LoopingSoundManager.StartSound(cascade_sound_path, Vector3.zero, pause_on_game_pause: false);
		}
		active_cascade_coroutine_count++;
		for (int i = 0; i < rows.Count; i++)
		{
			GameObject widget = rows[i].GetWidget(checkBoxToggleColumn);
			if (!(widget == header_widget_go))
			{
				bool flag = false;
				switch ((GetWidgetColumn(widget) as CheckboxTableColumn).get_value_action(rows[i].GetIdentity(), widget))
				{
				case ResultValues.False:
					flag = ((state != 0) ? true : false);
					break;
				case ResultValues.Partial:
				case ResultValues.ConditionalGroup:
					flag = true;
					break;
				case ResultValues.True:
					flag = ((state != ResultValues.True) ? true : false);
					break;
				}
				if (flag)
				{
					(GetWidgetColumn(widget) as CheckboxTableColumn).on_set_action(widget, state);
					yield return null;
				}
			}
		}
		if (header_widget_go != null)
		{
			(GetWidgetColumn(header_widget_go) as CheckboxTableColumn).on_load_action(null, header_widget_go);
		}
		active_cascade_coroutine_count--;
		if (active_cascade_coroutine_count <= 0)
		{
			StopLoopingCascadeSound();
		}
	}

	private void StopLoopingCascadeSound()
	{
		if (current_looping_sound.IsValid())
		{
			LoopingSoundManager.StopSound(current_looping_sound);
			current_looping_sound.Clear();
		}
	}

	protected void on_press_checkbox_column_super(GameObject widget_go)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = GetWidgetColumn(widget_go) as SuperCheckboxTableColumn;
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (get_value_checkbox_column_super(widgetRow.GetIdentity(), widget_go))
		{
		case ResultValues.True:
			superCheckboxTableColumn.on_set_action(widget_go, ResultValues.False);
			break;
		case ResultValues.Partial:
		case ResultValues.ConditionalGroup:
			superCheckboxTableColumn.on_set_action(widget_go, ResultValues.True);
			break;
		case ResultValues.False:
			superCheckboxTableColumn.on_set_action(widget_go, ResultValues.True);
			break;
		}
		superCheckboxTableColumn.on_load_action(widgetRow.GetIdentity(), widget_go);
	}

	protected void on_tooltip_sort_alphabetically(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_NAME, null);
			break;
		case TableRow.RowType.Default:
		case TableRow.RowType.Minion:
			break;
		}
	}
}
