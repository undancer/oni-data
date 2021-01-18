using System;
using System.Collections.Generic;
using UnityEngine;

public class TableColumn : IRender1000ms
{
	public Action<IAssignableIdentity, GameObject> on_load_action;

	public Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip;

	public Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip;

	public Comparison<IAssignableIdentity> sort_comparer;

	public Dictionary<TableRow, GameObject> widgets_by_row = new Dictionary<TableRow, GameObject>();

	public string scrollerID;

	public TableScreen screen;

	public MultiToggle column_sort_toggle;

	private Func<bool> revealed;

	protected bool dirty = false;

	public bool isRevealed => revealed == null || revealed();

	public bool isDirty => dirty;

	public TableColumn(Action<IAssignableIdentity, GameObject> on_load_action, Comparison<IAssignableIdentity> sort_comparison, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip = null, Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip = null, Func<bool> revealed = null, bool should_refresh_columns = false, string scrollerID = "")
	{
		this.on_load_action = on_load_action;
		sort_comparer = sort_comparison;
		this.on_tooltip = on_tooltip;
		this.on_sort_tooltip = on_sort_tooltip;
		this.revealed = revealed;
		this.scrollerID = scrollerID;
		if (should_refresh_columns)
		{
			SimAndRenderScheduler.instance.Add(this);
		}
	}

	protected string GetTooltip(ToolTip tool_tip_instance)
	{
		GameObject gameObject = tool_tip_instance.gameObject;
		HierarchyReferences component = tool_tip_instance.GetComponent<HierarchyReferences>();
		if (component != null && component.HasReference("Widget"))
		{
			gameObject = component.GetReference("Widget").gameObject;
		}
		TableRow tableRow = null;
		foreach (KeyValuePair<TableRow, GameObject> item in widgets_by_row)
		{
			if (item.Value == gameObject)
			{
				tableRow = item.Key;
				break;
			}
		}
		if (tableRow != null && on_tooltip != null)
		{
			on_tooltip(tableRow.GetIdentity(), gameObject, tool_tip_instance);
		}
		return "";
	}

	protected string GetSortTooltip(ToolTip sort_tooltip_instance)
	{
		GameObject gameObject = sort_tooltip_instance.transform.parent.gameObject;
		TableRow tableRow = null;
		foreach (KeyValuePair<TableRow, GameObject> item in widgets_by_row)
		{
			if (item.Value == gameObject)
			{
				tableRow = item.Key;
				break;
			}
		}
		if (tableRow != null && on_sort_tooltip != null)
		{
			on_sort_tooltip(tableRow.GetIdentity(), gameObject, sort_tooltip_instance);
		}
		return "";
	}

	public bool ContainsWidget(GameObject widget)
	{
		return widgets_by_row.ContainsValue(widget);
	}

	public virtual GameObject GetMinionWidget(GameObject parent)
	{
		Debug.LogError("Table Column has no Widget prefab");
		return null;
	}

	public virtual GameObject GetHeaderWidget(GameObject parent)
	{
		Debug.LogError("Table Column has no Widget prefab");
		return null;
	}

	public virtual GameObject GetDefaultWidget(GameObject parent)
	{
		Debug.LogError("Table Column has no Widget prefab");
		return null;
	}

	public void Render1000ms(float dt)
	{
		MarkDirty();
	}

	public void MarkDirty(GameObject triggering_obj = null, TableScreen.ResultValues triggering_object_state = TableScreen.ResultValues.False)
	{
		dirty = true;
	}

	public void MarkClean()
	{
		dirty = false;
	}
}
