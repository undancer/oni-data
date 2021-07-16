using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class TreeFilterableSideScreen : SideScreenContent
{
	private struct TagOrderInfo
	{
		public Tag tag;

		public string strippedName;
	}

	[SerializeField]
	private MultiToggle allCheckBox;

	[SerializeField]
	private MultiToggle onlyAllowTransportItemsCheckBox;

	[SerializeField]
	private GameObject onlyallowTransportItemsRow;

	[SerializeField]
	private TreeFilterableSideScreenRow rowPrefab;

	[SerializeField]
	private GameObject rowGroup;

	[SerializeField]
	private TreeFilterableSideScreenElement elementPrefab;

	private GameObject target;

	private bool visualDirty;

	private KImage onlyAllowTransportItemsImg;

	public UIPool<TreeFilterableSideScreenElement> elementPool;

	private UIPool<TreeFilterableSideScreenRow> rowPool;

	private TreeFilterable targetFilterable;

	private Dictionary<Tag, TreeFilterableSideScreenRow> tagRowMap = new Dictionary<Tag, TreeFilterableSideScreenRow>();

	private Storage storage;

	public bool IsStorage => storage != null;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		rowPool = new UIPool<TreeFilterableSideScreenRow>(rowPrefab);
		elementPool = new UIPool<TreeFilterableSideScreenElement>(elementPrefab);
		MultiToggle multiToggle = allCheckBox;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			switch (GetAllCheckboxState())
			{
			case TreeFilterableSideScreenRow.State.On:
				SetAllCheckboxState(TreeFilterableSideScreenRow.State.Off);
				break;
			case TreeFilterableSideScreenRow.State.Off:
			case TreeFilterableSideScreenRow.State.Mixed:
				SetAllCheckboxState(TreeFilterableSideScreenRow.State.On);
				break;
			}
		});
		onlyAllowTransportItemsCheckBox.onClick = OnlyAllowTransportItemsClicked;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		allCheckBox.transform.parent.parent.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ALLBUTTONTOOLTIP);
		onlyAllowTransportItemsCheckBox.transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ONLYALLOWTRANSPORTITEMSBUTTONTOOLTIP);
	}

	private void UpdateAllCheckBoxVisualState()
	{
		switch (GetAllCheckboxState())
		{
		case TreeFilterableSideScreenRow.State.Off:
			allCheckBox.ChangeState(0);
			break;
		case TreeFilterableSideScreenRow.State.Mixed:
			allCheckBox.ChangeState(1);
			break;
		case TreeFilterableSideScreenRow.State.On:
			allCheckBox.ChangeState(2);
			break;
		}
		visualDirty = false;
	}

	public void Update()
	{
		foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> item in tagRowMap)
		{
			if (item.Value.visualDirty)
			{
				item.Value.UpdateCheckBoxVisualState();
				visualDirty = true;
			}
		}
		if (visualDirty)
		{
			UpdateAllCheckBoxVisualState();
		}
	}

	private void OnlyAllowTransportItemsClicked()
	{
		storage.SetOnlyFetchMarkedItems(!storage.GetOnlyFetchMarkedItems());
	}

	private TreeFilterableSideScreenRow.State GetAllCheckboxState()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		using (Dictionary<Tag, TreeFilterableSideScreenRow>.Enumerator enumerator = tagRowMap.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current.Value.GetState())
				{
				case TreeFilterableSideScreenRow.State.Mixed:
					flag3 = true;
					break;
				case TreeFilterableSideScreenRow.State.On:
					flag = true;
					break;
				case TreeFilterableSideScreenRow.State.Off:
					flag2 = true;
					break;
				}
			}
		}
		if (flag3)
		{
			return TreeFilterableSideScreenRow.State.Mixed;
		}
		if (flag && !flag2)
		{
			return TreeFilterableSideScreenRow.State.On;
		}
		if (!flag && flag2)
		{
			return TreeFilterableSideScreenRow.State.Off;
		}
		if (flag && flag2)
		{
			return TreeFilterableSideScreenRow.State.Mixed;
		}
		return TreeFilterableSideScreenRow.State.Off;
	}

	private void SetAllCheckboxState(TreeFilterableSideScreenRow.State newState)
	{
		switch (newState)
		{
		case TreeFilterableSideScreenRow.State.Off:
			foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> item in tagRowMap)
			{
				item.Value.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.Off);
			}
			break;
		case TreeFilterableSideScreenRow.State.On:
			foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> item2 in tagRowMap)
			{
				item2.Value.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.On);
			}
			break;
		}
		visualDirty = true;
	}

	public bool GetElementTagAcceptedState(Tag t)
	{
		return targetFilterable.ContainsTag(t);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		if (target.GetComponent<TreeFilterable>() != null)
		{
			return target.GetComponent<FlatTagFilterable>() == null;
		}
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		this.target = target;
		if (target == null)
		{
			Debug.LogError("The target object provided was null");
			return;
		}
		targetFilterable = target.GetComponent<TreeFilterable>();
		if (targetFilterable == null)
		{
			Debug.LogError("The target provided does not have a Tree Filterable component");
			return;
		}
		if (!targetFilterable.showUserMenu)
		{
			DetailsScreen.Instance.DeactivateSideContent();
			return;
		}
		if (IsStorage && !storage.showInUI)
		{
			DetailsScreen.Instance.DeactivateSideContent();
			return;
		}
		storage = targetFilterable.GetComponent<Storage>();
		storage.Subscribe(644822890, OnOnlyFetchMarkedItemsSettingChanged);
		OnOnlyFetchMarkedItemsSettingChanged(null);
		CreateCategories();
	}

	private void OnOnlyFetchMarkedItemsSettingChanged(object data)
	{
		onlyAllowTransportItemsCheckBox.ChangeState(storage.GetOnlyFetchMarkedItems() ? 1 : 0);
		if (storage.allowSettingOnlyFetchMarkedItems)
		{
			onlyallowTransportItemsRow.SetActive(value: true);
		}
		else
		{
			onlyallowTransportItemsRow.SetActive(value: false);
		}
	}

	public bool IsTagAllowed(Tag tag)
	{
		return targetFilterable.AcceptedTags.Contains(tag);
	}

	public void AddTag(Tag tag)
	{
		if (!(targetFilterable == null))
		{
			targetFilterable.AddTagToFilter(tag);
		}
	}

	public void RemoveTag(Tag tag)
	{
		if (!(targetFilterable == null))
		{
			targetFilterable.RemoveTagFromFilter(tag);
		}
	}

	private List<TagOrderInfo> GetTagsSortedAlphabetically(ICollection<Tag> tags)
	{
		List<TagOrderInfo> list = new List<TagOrderInfo>();
		foreach (Tag tag in tags)
		{
			list.Add(new TagOrderInfo
			{
				tag = tag,
				strippedName = tag.ProperNameStripLink()
			});
		}
		list.Sort((TagOrderInfo a, TagOrderInfo b) => a.strippedName.CompareTo(b.strippedName));
		return list;
	}

	private TreeFilterableSideScreenRow AddRow(Tag rowTag)
	{
		TreeFilterableSideScreenRow freeElement = rowPool.GetFreeElement(rowGroup, forceActive: true);
		freeElement.Parent = this;
		tagRowMap.Add(rowTag, freeElement);
		Dictionary<Tag, bool> dictionary = new Dictionary<Tag, bool>();
		foreach (TagOrderInfo item in GetTagsSortedAlphabetically(DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(rowTag)))
		{
			dictionary.Add(item.tag, targetFilterable.ContainsTag(item.tag) || targetFilterable.ContainsTag(rowTag));
		}
		freeElement.SetElement(rowTag, targetFilterable.ContainsTag(rowTag), dictionary);
		freeElement.transform.SetAsLastSibling();
		return freeElement;
	}

	public float GetAmountInStorage(Tag tag)
	{
		if (!IsStorage)
		{
			return 0f;
		}
		return storage.GetMassAvailable(tag);
	}

	private void CreateCategories()
	{
		if (storage.storageFilters != null && storage.storageFilters.Count >= 1)
		{
			bool flag = target.GetComponent<CreatureDeliveryPoint>() != null;
			foreach (TagOrderInfo item in GetTagsSortedAlphabetically(storage.storageFilters))
			{
				Tag tag = item.tag;
				if (flag || DiscoveredResources.Instance.IsDiscovered(tag))
				{
					AddRow(tag);
				}
			}
			visualDirty = true;
		}
		else
		{
			Debug.LogError("If you're filtering, your storage filter should have the filters set on it");
		}
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (storage != null)
		{
			storage.Unsubscribe(644822890, OnOnlyFetchMarkedItemsSettingChanged);
		}
		rowPool.ClearAll();
		elementPool.ClearAll();
		tagRowMap.Clear();
	}
}
