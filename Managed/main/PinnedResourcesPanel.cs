using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class PinnedResourcesPanel : KScreen, IRender1000ms
{
	public class PinnedResourceRow
	{
		public GameObject gameObject;

		public Image icon;

		public LocText nameLabel;

		public LocText valueLabel;

		public MultiToggle pinToggle;

		public MultiToggle notifyToggle;

		public MultiToggle newLabel;

		private float oldResourceAmount = -1f;

		public Tag Tag { get; private set; }

		public string SortableNameWithoutLink { get; private set; }

		public PinnedResourceRow(Tag tag)
		{
			Tag = tag;
			SortableNameWithoutLink = tag.ProperNameStripLink();
		}

		public bool CheckAmountChanged(float newResourceAmount, bool updateIfTrue)
		{
			bool num = newResourceAmount != oldResourceAmount;
			if (num && updateIfTrue)
			{
				oldResourceAmount = newResourceAmount;
			}
			return num;
		}
	}

	public GameObject linePrefab;

	public GameObject rowContainer;

	public MultiToggle headerButton;

	public MultiToggle clearNewButton;

	public KButton clearAllButton;

	public MultiToggle seeAllButton;

	private LocText seeAllLabel;

	private QuickLayout rowContainerLayout;

	private Dictionary<Tag, PinnedResourceRow> rows = new Dictionary<Tag, PinnedResourceRow>();

	public static PinnedResourcesPanel Instance;

	private int clickIdx;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		rowContainerLayout = rowContainer.GetComponent<QuickLayout>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Instance = this;
		Populate();
		Game.Instance.Subscribe(1983128072, Populate);
		MultiToggle component = headerButton.GetComponent<MultiToggle>();
		component.onClick = (System.Action)Delegate.Combine(component.onClick, (System.Action)delegate
		{
			Refresh();
		});
		MultiToggle component2 = seeAllButton.GetComponent<MultiToggle>();
		component2.onClick = (System.Action)Delegate.Combine(component2.onClick, (System.Action)delegate
		{
			AllResourcesScreen.Instance.Show(!AllResourcesScreen.Instance.gameObject.activeSelf);
		});
		seeAllLabel = seeAllButton.GetComponentInChildren<LocText>();
		MultiToggle component3 = clearNewButton.GetComponent<MultiToggle>();
		component3.onClick = (System.Action)Delegate.Combine(component3.onClick, (System.Action)delegate
		{
			ClearAllNew();
		});
		clearAllButton.onClick += delegate
		{
			ClearAllNew();
			UnPinAll();
			Refresh();
		};
		AllResourcesScreen.Instance.Init();
		Refresh();
	}

	protected override void OnForcedCleanUp()
	{
		Instance = null;
		base.OnForcedCleanUp();
	}

	public void ClearExcessiveNewItems()
	{
		if (DiscoveredResources.Instance.CheckAllDiscoveredAreNew())
		{
			DiscoveredResources.Instance.newDiscoveries.Clear();
		}
	}

	private void ClearAllNew()
	{
		foreach (KeyValuePair<Tag, PinnedResourceRow> row in rows)
		{
			if (row.Value.gameObject.activeSelf && DiscoveredResources.Instance.newDiscoveries.ContainsKey(row.Key))
			{
				DiscoveredResources.Instance.newDiscoveries.Remove(row.Key);
			}
		}
	}

	private void UnPinAll()
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		foreach (KeyValuePair<Tag, PinnedResourceRow> row in rows)
		{
			worldInventory.pinnedResources.Remove(row.Key);
		}
	}

	private PinnedResourceRow CreateRow(Tag tag)
	{
		PinnedResourceRow pinnedResourceRow = new PinnedResourceRow(tag);
		HierarchyReferences component = (pinnedResourceRow.gameObject = Util.KInstantiateUI(linePrefab, rowContainer)).GetComponent<HierarchyReferences>();
		pinnedResourceRow.icon = component.GetReference<Image>("Icon");
		pinnedResourceRow.nameLabel = component.GetReference<LocText>("NameLabel");
		pinnedResourceRow.valueLabel = component.GetReference<LocText>("ValueLabel");
		pinnedResourceRow.pinToggle = component.GetReference<MultiToggle>("PinToggle");
		pinnedResourceRow.notifyToggle = component.GetReference<MultiToggle>("NotifyToggle");
		pinnedResourceRow.newLabel = component.GetReference<MultiToggle>("NewLabel");
		Tuple<Sprite, Color> uISprite = Def.GetUISprite(tag);
		pinnedResourceRow.icon.sprite = uISprite.first;
		pinnedResourceRow.icon.color = uISprite.second;
		pinnedResourceRow.nameLabel.SetText(tag.ProperNameStripLink());
		MultiToggle component2 = pinnedResourceRow.gameObject.GetComponent<MultiToggle>();
		component2.onClick = (System.Action)Delegate.Combine(component2.onClick, (System.Action)delegate
		{
			List<Pickupable> list = ClusterManager.Instance.activeWorld.worldInventory.CreatePickupablesList(tag);
			if (list != null && list.Count > 0)
			{
				SelectTool.Instance.SelectAndFocus(list[clickIdx % list.Count].transform.position, list[clickIdx % list.Count].GetComponent<KSelectable>());
				clickIdx++;
			}
			else
			{
				clickIdx = 0;
			}
		});
		return pinnedResourceRow;
	}

	public void Populate(object data = null)
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		foreach (KeyValuePair<Tag, float> newDiscovery in DiscoveredResources.Instance.newDiscoveries)
		{
			if (!rows.ContainsKey(newDiscovery.Key) && IsDisplayedTag(newDiscovery.Key))
			{
				rows.Add(newDiscovery.Key, CreateRow(newDiscovery.Key));
			}
		}
		foreach (Tag pinnedResource in worldInventory.pinnedResources)
		{
			if (!rows.ContainsKey(pinnedResource))
			{
				rows.Add(pinnedResource, CreateRow(pinnedResource));
			}
		}
		foreach (Tag notifyResource in worldInventory.notifyResources)
		{
			if (!rows.ContainsKey(notifyResource))
			{
				rows.Add(notifyResource, CreateRow(notifyResource));
			}
		}
		foreach (KeyValuePair<Tag, PinnedResourceRow> row in rows)
		{
			if (false || worldInventory.pinnedResources.Contains(row.Key) || worldInventory.notifyResources.Contains(row.Key) || (DiscoveredResources.Instance.newDiscoveries.ContainsKey(row.Key) && worldInventory.GetAmount(row.Key, includeRelatedWorlds: false) > 0f))
			{
				if (!row.Value.gameObject.activeSelf)
				{
					row.Value.gameObject.SetActive(value: true);
				}
			}
			else if (row.Value.gameObject.activeSelf)
			{
				row.Value.gameObject.SetActive(value: false);
			}
		}
		foreach (KeyValuePair<Tag, PinnedResourceRow> row2 in rows)
		{
			row2.Value.pinToggle.gameObject.SetActive(worldInventory.pinnedResources.Contains(row2.Key));
		}
		SortRows();
		rowContainerLayout.ForceUpdate();
	}

	private void SortRows()
	{
		List<PinnedResourceRow> list = new List<PinnedResourceRow>();
		foreach (KeyValuePair<Tag, PinnedResourceRow> row in rows)
		{
			list.Add(row.Value);
		}
		list.Sort((PinnedResourceRow a, PinnedResourceRow b) => a.SortableNameWithoutLink.CompareTo(b.SortableNameWithoutLink));
		foreach (PinnedResourceRow item in list)
		{
			rows[item.Tag].gameObject.transform.SetAsLastSibling();
		}
		clearNewButton.transform.SetAsLastSibling();
		seeAllButton.transform.SetAsLastSibling();
	}

	private bool IsDisplayedTag(Tag tag)
	{
		foreach (TagSet allowDisplayCategory in AllResourcesScreen.Instance.allowDisplayCategories)
		{
			foreach (KeyValuePair<Tag, HashSet<Tag>> item in DiscoveredResources.Instance.GetDiscoveredResourcesFromTagSet(allowDisplayCategory))
			{
				if (item.Value.Contains(tag))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void SyncRows()
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		bool flag = false;
		foreach (Tag pinnedResource in worldInventory.pinnedResources)
		{
			if (!rows.ContainsKey(pinnedResource))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			foreach (KeyValuePair<Tag, float> newDiscovery in DiscoveredResources.Instance.newDiscoveries)
			{
				if (!rows.ContainsKey(newDiscovery.Key) && IsDisplayedTag(newDiscovery.Key))
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			foreach (Tag notifyResource in worldInventory.notifyResources)
			{
				if (!rows.ContainsKey(notifyResource))
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			foreach (KeyValuePair<Tag, PinnedResourceRow> row in rows)
			{
				if ((worldInventory.pinnedResources.Contains(row.Key) || worldInventory.notifyResources.Contains(row.Key) || (DiscoveredResources.Instance.newDiscoveries.ContainsKey(row.Key) && worldInventory.GetAmount(row.Key, includeRelatedWorlds: false) > 0f)) != row.Value.gameObject.activeSelf)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			Populate();
		}
	}

	public void Refresh()
	{
		SyncRows();
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		bool flag = false;
		foreach (KeyValuePair<Tag, PinnedResourceRow> row in rows)
		{
			if (row.Value.gameObject.activeSelf)
			{
				RefreshLine(row.Key, worldInventory);
				flag = flag || DiscoveredResources.Instance.newDiscoveries.ContainsKey(row.Key);
			}
		}
		clearNewButton.gameObject.SetActive(flag);
		seeAllLabel.SetText(string.Format(UI.RESOURCESCREEN.SEE_ALL, AllResourcesScreen.Instance.UniqueResourceRowCount()));
	}

	private void RefreshLine(Tag tag, WorldInventory inventory, bool initialConfig = false)
	{
		Tag tag2 = tag;
		if (!AllResourcesScreen.Instance.units.ContainsKey(tag))
		{
			AllResourcesScreen.Instance.units.Add(tag, GameUtil.MeasureUnit.quantity);
		}
		if (!inventory.HasValidCount)
		{
			rows[tag].valueLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
		}
		else
		{
			switch (AllResourcesScreen.Instance.units[tag])
			{
			case GameUtil.MeasureUnit.mass:
			{
				float amount = inventory.GetAmount(tag2, includeRelatedWorlds: false);
				if (rows[tag].CheckAmountChanged(amount, updateIfTrue: true))
				{
					rows[tag].valueLabel.SetText(GameUtil.GetFormattedMass(amount));
				}
				break;
			}
			case GameUtil.MeasureUnit.quantity:
			{
				float amount2 = inventory.GetAmount(tag2, includeRelatedWorlds: false);
				if (rows[tag].CheckAmountChanged(amount2, updateIfTrue: true))
				{
					rows[tag].valueLabel.SetText(GameUtil.GetFormattedUnits(amount2));
				}
				break;
			}
			case GameUtil.MeasureUnit.kcal:
			{
				float num = RationTracker.Get().CountRationsByFoodType(tag.Name, ClusterManager.Instance.activeWorld.worldInventory);
				if (rows[tag].CheckAmountChanged(num, updateIfTrue: true))
				{
					rows[tag].valueLabel.SetText(GameUtil.GetFormattedCalories(num));
				}
				break;
			}
			}
		}
		rows[tag].pinToggle.onClick = delegate
		{
			inventory.pinnedResources.Remove(tag);
			SyncRows();
		};
		rows[tag].notifyToggle.onClick = delegate
		{
			inventory.notifyResources.Remove(tag);
			SyncRows();
		};
		rows[tag].newLabel.gameObject.SetActive(DiscoveredResources.Instance.newDiscoveries.ContainsKey(tag));
		rows[tag].newLabel.onClick = delegate
		{
			AllResourcesScreen.Instance.Show(!AllResourcesScreen.Instance.gameObject.activeSelf);
		};
	}

	public void Render1000ms(float dt)
	{
		if (!(headerButton != null) || headerButton.CurrentState != 0)
		{
			Refresh();
		}
	}
}
