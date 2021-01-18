using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupSelectorWidget : MonoBehaviour
{
	[Serializable]
	public struct ItemData
	{
		public Sprite sprite;

		public object userData;

		public ItemData(Sprite sprite, object user_data)
		{
			this.sprite = sprite;
			userData = user_data;
		}
	}

	public struct ItemCallbacks
	{
		public Func<object, IList<int>> getSubPanelDisplayIndices;

		public Action<object, object> onItemAdded;

		public Action<object, object> onItemRemoved;

		public Func<object, object, bool, string> getItemHoverText;
	}

	[SerializeField]
	private GameObject itemTemplate;

	[SerializeField]
	private RectTransform selectedItemsPanel;

	[SerializeField]
	private RectTransform unselectedItemsPanel;

	[SerializeField]
	private KButton addItemButton;

	[SerializeField]
	private int numExpectedPanelColumns = 3;

	private object widgetID;

	private ItemCallbacks itemCallbacks;

	private IList<ItemData> options;

	private List<int> selectedOptionIndices = new List<int>();

	private List<GameObject> selectedVisualizers = new List<GameObject>();

	public void Initialize(object widget_id, IList<ItemData> options, ItemCallbacks item_callbacks)
	{
		widgetID = widget_id;
		this.options = options;
		itemCallbacks = item_callbacks;
		addItemButton.onClick += OnAddItemClicked;
	}

	public void Reconfigure(IList<int> selected_option_indices)
	{
		selectedOptionIndices.Clear();
		selectedOptionIndices.AddRange(selected_option_indices);
		selectedOptionIndices.Sort();
		addItemButton.isInteractable = selectedOptionIndices.Count < options.Count;
		RebuildSelectedVisualizers();
	}

	private void OnAddItemClicked()
	{
		if (!IsSubPanelOpen())
		{
			if (RebuildSubPanelOptions() > 0)
			{
				unselectedItemsPanel.GetComponent<GridLayoutGroup>().constraintCount = Mathf.Min(numExpectedPanelColumns, unselectedItemsPanel.childCount);
				unselectedItemsPanel.gameObject.SetActive(value: true);
				unselectedItemsPanel.GetComponent<Selectable>().Select();
			}
		}
		else
		{
			CloseSubPanel();
		}
	}

	private void OnItemAdded(int option_idx)
	{
		if (itemCallbacks.onItemAdded != null)
		{
			itemCallbacks.onItemAdded(widgetID, options[option_idx].userData);
			RebuildSubPanelOptions();
		}
	}

	private void OnItemRemoved(int option_idx)
	{
		if (itemCallbacks.onItemRemoved != null)
		{
			itemCallbacks.onItemRemoved(widgetID, options[option_idx].userData);
		}
	}

	private void RebuildSelectedVisualizers()
	{
		foreach (GameObject selectedVisualizer in selectedVisualizers)
		{
			Util.KDestroyGameObject(selectedVisualizer);
		}
		selectedVisualizers.Clear();
		foreach (int selectedOptionIndex in selectedOptionIndices)
		{
			GameObject item = CreateItem(selectedOptionIndex, OnItemRemoved, selectedItemsPanel.gameObject, is_selected_item: true);
			selectedVisualizers.Add(item);
		}
	}

	private GameObject CreateItem(int idx, Action<int> on_click, GameObject parent, bool is_selected_item)
	{
		GameObject gameObject = Util.KInstantiateUI(itemTemplate, parent, force_active: true);
		KButton component = gameObject.GetComponent<KButton>();
		component.onClick += delegate
		{
			on_click(idx);
		};
		component.fgImage.sprite = options[idx].sprite;
		if (parent == selectedItemsPanel.gameObject)
		{
			HierarchyReferences component2 = component.GetComponent<HierarchyReferences>();
			if (component2 != null)
			{
				Component reference = component2.GetReference("CancelImg");
				if (reference != null)
				{
					reference.gameObject.SetActive(value: true);
				}
			}
		}
		gameObject.GetComponent<ToolTip>().OnToolTip = () => itemCallbacks.getItemHoverText(widgetID, options[idx].userData, is_selected_item);
		return gameObject;
	}

	public bool IsSubPanelOpen()
	{
		return unselectedItemsPanel.gameObject.activeSelf;
	}

	public void CloseSubPanel()
	{
		ClearSubPanelOptions();
		unselectedItemsPanel.gameObject.SetActive(value: false);
	}

	private void ClearSubPanelOptions()
	{
		foreach (Transform item in unselectedItemsPanel.transform)
		{
			Util.KDestroyGameObject(item.gameObject);
		}
	}

	private int RebuildSubPanelOptions()
	{
		IList<int> list = itemCallbacks.getSubPanelDisplayIndices(widgetID);
		if (list.Count > 0)
		{
			ClearSubPanelOptions();
			foreach (int item in list)
			{
				if (!selectedOptionIndices.Contains(item))
				{
					CreateItem(item, OnItemAdded, unselectedItemsPanel.gameObject, is_selected_item: false);
				}
			}
		}
		else
		{
			CloseSubPanel();
		}
		return list.Count;
	}
}
