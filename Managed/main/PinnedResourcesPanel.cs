using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class PinnedResourcesPanel : KScreen, IRender1000ms
{
	public GameObject linePrefab;

	public GameObject rowContainer;

	public MultiToggle headerButton;

	public MultiToggle clearNewButton;

	public MultiToggle seeAllButton;

	private Dictionary<Tag, GameObject> rows = new Dictionary<Tag, GameObject>();

	public static PinnedResourcesPanel Instance;

	private int clickIdx;

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
		MultiToggle component3 = clearNewButton.GetComponent<MultiToggle>();
		component3.onClick = (System.Action)Delegate.Combine(component3.onClick, (System.Action)delegate
		{
			foreach (KeyValuePair<Tag, GameObject> row in rows)
			{
				if (row.Value.activeSelf && DiscoveredResources.Instance.newDiscoveries.ContainsKey(row.Key))
				{
					DiscoveredResources.Instance.newDiscoveries.Remove(row.Key);
				}
			}
		});
		AllResourcesScreen.Instance.Init();
		Refresh();
	}

	public void Populate(object data = null)
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		foreach (KeyValuePair<Tag, float> resource in DiscoveredResources.Instance.newDiscoveries)
		{
			if (rows.ContainsKey(resource.Key) || !IsDisplayedTag(resource.Key))
			{
				continue;
			}
			GameObject gameObject = Util.KInstantiateUI(linePrefab, rowContainer);
			rows.Add(resource.Key, gameObject);
			MultiToggle component = gameObject.GetComponent<MultiToggle>();
			component.onClick = (System.Action)Delegate.Combine(component.onClick, (System.Action)delegate
			{
				List<Pickupable> list2 = ClusterManager.Instance.activeWorld.worldInventory.CreatePickupablesList(resource.Key);
				if (list2 != null && list2.Count > 0)
				{
					SelectTool.Instance.SelectAndFocus(list2[clickIdx % list2.Count].transform.position, list2[clickIdx % list2.Count].GetComponent<KSelectable>());
					clickIdx++;
				}
				else
				{
					clickIdx = 0;
				}
			});
		}
		foreach (Tag tag in worldInventory.pinnedResources)
		{
			if (rows.ContainsKey(tag))
			{
				continue;
			}
			GameObject gameObject2 = Util.KInstantiateUI(linePrefab, rowContainer);
			MultiToggle component2 = gameObject2.GetComponent<MultiToggle>();
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
			rows.Add(tag, gameObject2);
		}
		foreach (Tag notifyResource in worldInventory.notifyResources)
		{
			if (!rows.ContainsKey(notifyResource))
			{
				GameObject value = Util.KInstantiateUI(linePrefab, rowContainer);
				rows.Add(notifyResource, value);
			}
		}
		foreach (KeyValuePair<Tag, GameObject> row in rows)
		{
			if (false || worldInventory.pinnedResources.Contains(row.Key) || worldInventory.notifyResources.Contains(row.Key) || (DiscoveredResources.Instance.newDiscoveries.ContainsKey(row.Key) && worldInventory.GetAmount(row.Key, includeRelatedWorlds: false) > 0f))
			{
				if (!row.Value.activeSelf)
				{
					row.Value.SetActive(value: true);
				}
			}
			else if (row.Value.activeSelf)
			{
				row.Value.SetActive(value: false);
			}
		}
		foreach (KeyValuePair<Tag, GameObject> row2 in rows)
		{
			row2.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("PinToggle").gameObject.SetActive(worldInventory.pinnedResources.Contains(row2.Key));
		}
		SortRows();
	}

	private void SortRows()
	{
		List<Tag> list = new List<Tag>();
		foreach (KeyValuePair<Tag, GameObject> row in rows)
		{
			list.Add(row.Key);
		}
		list.Sort((Tag a, Tag b) => a.ProperNameStripLink().CompareTo(b.ProperNameStripLink()));
		foreach (Tag item in list)
		{
			rows[item].transform.SetAsLastSibling();
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
			foreach (KeyValuePair<Tag, GameObject> row in rows)
			{
				if (worldInventory.pinnedResources.Contains(row.Key) != row.Value.activeSelf)
				{
					flag = true;
					break;
				}
				if (worldInventory.notifyResources.Contains(row.Key) != row.Value.activeSelf)
				{
					flag = true;
					break;
				}
				if (DiscoveredResources.Instance.newDiscoveries.ContainsKey(row.Key) != row.Value.activeSelf)
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
		foreach (KeyValuePair<Tag, GameObject> row in rows)
		{
			if (row.Value.activeSelf)
			{
				RefreshLine(row.Key, worldInventory);
				flag = flag || DiscoveredResources.Instance.newDiscoveries.ContainsKey(row.Key);
			}
		}
		clearNewButton.gameObject.SetActive(flag);
		seeAllButton.GetComponentInChildren<LocText>().SetText(string.Format(UI.RESOURCESCREEN.SEE_ALL, AllResourcesScreen.Instance.UniqueResourceRowCount()));
	}

	private void RefreshLine(Tag tag, WorldInventory inventory)
	{
		Tag tag2 = tag;
		HierarchyReferences component = rows[tag2].GetComponent<HierarchyReferences>();
		Tuple<Sprite, Color> uISprite = Def.GetUISprite(tag2);
		component.GetReference<Image>("Icon").sprite = uISprite.first;
		component.GetReference<Image>("Icon").color = uISprite.second;
		component.GetReference<LocText>("NameLabel").SetText(tag2.ProperNameStripLink());
		if (!AllResourcesScreen.Instance.units.ContainsKey(tag))
		{
			AllResourcesScreen.Instance.units.Add(tag, GameUtil.MeasureUnit.quantity);
		}
		if (!inventory.HasValidCount)
		{
			component.GetReference<LocText>("ValueLabel").SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
		}
		else
		{
			switch (AllResourcesScreen.Instance.units[tag])
			{
			case GameUtil.MeasureUnit.mass:
				component.GetReference<LocText>("ValueLabel").SetText(GameUtil.GetFormattedMass(inventory.GetAmount(tag2, includeRelatedWorlds: false)));
				break;
			case GameUtil.MeasureUnit.quantity:
				component.GetReference<LocText>("ValueLabel").SetText(GameUtil.GetFormattedUnits(inventory.GetAmount(tag2, includeRelatedWorlds: false)));
				break;
			case GameUtil.MeasureUnit.kcal:
			{
				float calories = RationTracker.Get().CountRationsByFoodType(tag.Name, ClusterManager.Instance.activeWorld.worldInventory);
				component.GetReference<LocText>("ValueLabel").SetText(GameUtil.GetFormattedCalories(calories));
				break;
			}
			}
		}
		component.GetReference<MultiToggle>("PinToggle").onClick = delegate
		{
			inventory.pinnedResources.Remove(tag);
			SyncRows();
		};
		component.GetReference<MultiToggle>("NotifyToggle").onClick = delegate
		{
			inventory.notifyResources.Remove(tag);
			SyncRows();
		};
		component.GetReference("NewLabel").gameObject.SetActive(DiscoveredResources.Instance.newDiscoveries.ContainsKey(tag));
		component.GetReference("NewLabel").GetComponent<MultiToggle>().onClick = delegate
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
