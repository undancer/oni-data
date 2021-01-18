using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupSelectorHeaderWidget : MonoBehaviour
{
	public struct ItemCallbacks
	{
		public Func<object, string> getTitleHoverText;

		public Func<object, bool, string> getTitleButtonHoverText;

		public Func<object, bool, IList<int>> getHeaderButtonOptions;

		public Action<object> onItemAdded;

		public Action<object> onItemRemoved;

		public Func<object, bool, object, string> getItemHoverText;

		public Func<object, IList<int>> getValidSortOptionIndices;

		public Func<object, string> getSortHoverText;

		public Action<object, object> onSort;
	}

	public LocText label;

	[SerializeField]
	private GameObject itemTemplate;

	[SerializeField]
	private RectTransform itemsPanel;

	[SerializeField]
	private KButton addItemButton;

	[SerializeField]
	private KButton removeItemButton;

	[SerializeField]
	private KButton sortButton;

	[SerializeField]
	private int numExpectedPanelColumns = 3;

	private object widgetID;

	private ItemCallbacks itemCallbacks;

	private IList<GroupSelectorWidget.ItemData> options;

	public void Initialize(object widget_id, IList<GroupSelectorWidget.ItemData> options, ItemCallbacks item_callbacks)
	{
		widgetID = widget_id;
		this.options = options;
		itemCallbacks = item_callbacks;
		if (itemCallbacks.getTitleHoverText != null)
		{
			label.GetComponent<ToolTip>().OnToolTip = () => itemCallbacks.getTitleHoverText(widget_id);
		}
		bool adding_item = true;
		addItemButton.onClick += delegate
		{
			RebuildSubPanel(addItemButton.transform.GetPosition(), (object widget_go) => itemCallbacks.getHeaderButtonOptions(widget_go, adding_item), itemCallbacks.onItemAdded, (object widget_go, object item_data) => itemCallbacks.getItemHoverText(widget_go, adding_item, item_data));
		};
		bool adding_item2 = false;
		removeItemButton.onClick += delegate
		{
			RebuildSubPanel(removeItemButton.transform.GetPosition(), (object widget_go) => itemCallbacks.getHeaderButtonOptions(widget_go, adding_item2), itemCallbacks.onItemRemoved, (object widget_go, object item_data) => itemCallbacks.getItemHoverText(widget_go, adding_item2, item_data));
		};
		sortButton.onClick += delegate
		{
			RebuildSubPanel(sortButton.transform.GetPosition(), itemCallbacks.getValidSortOptionIndices, delegate(object item_data)
			{
				itemCallbacks.onSort(widgetID, item_data);
			}, (object widget_go, object item_data) => itemCallbacks.getSortHoverText(item_data));
		};
		if (itemCallbacks.getTitleButtonHoverText != null)
		{
			addItemButton.GetComponent<ToolTip>().OnToolTip = () => itemCallbacks.getTitleButtonHoverText(widget_id, arg2: true);
			removeItemButton.GetComponent<ToolTip>().OnToolTip = () => itemCallbacks.getTitleButtonHoverText(widget_id, arg2: false);
		}
	}

	private void RebuildSubPanel(Vector3 pos, Func<object, IList<int>> display_list_query, Action<object> on_item_selected, Func<object, object, string> get_item_hover_text)
	{
		itemsPanel.gameObject.transform.SetPosition(pos + new Vector3(2f, 2f, 0f));
		IList<int> list = display_list_query(widgetID);
		if (list.Count > 0)
		{
			ClearSubPanelOptions();
			foreach (int item in list)
			{
				int idx = item;
				GroupSelectorWidget.ItemData itemData = options[idx];
				GameObject gameObject = Util.KInstantiateUI(itemTemplate, itemsPanel.gameObject, force_active: true);
				KButton component = gameObject.GetComponent<KButton>();
				component.fgImage.sprite = options[idx].sprite;
				component.onClick += delegate
				{
					on_item_selected(options[idx].userData);
					RebuildSubPanel(pos, display_list_query, on_item_selected, get_item_hover_text);
				};
				if (get_item_hover_text != null)
				{
					ToolTip component2 = gameObject.GetComponent<ToolTip>();
					component2.OnToolTip = () => get_item_hover_text(widgetID, options[idx].userData);
				}
			}
			GridLayoutGroup component3 = itemsPanel.GetComponent<GridLayoutGroup>();
			component3.constraintCount = Mathf.Min(numExpectedPanelColumns, itemsPanel.childCount);
			itemsPanel.gameObject.SetActive(value: true);
			itemsPanel.GetComponent<Selectable>().Select();
		}
		else
		{
			CloseSubPanel();
		}
	}

	public void CloseSubPanel()
	{
		ClearSubPanelOptions();
		itemsPanel.gameObject.SetActive(value: false);
	}

	private void ClearSubPanelOptions()
	{
		foreach (Transform item in itemsPanel.transform)
		{
			Util.KDestroyGameObject(item.gameObject);
		}
	}
}
