using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class AllResourcesScreen : KScreen, ISim4000ms, ISim1000ms
{
	private class ScreenRowBase
	{
		public LocText availableLabel;

		public LocText totalLabel;

		public LocText reservedLabel;

		public SparkLayer sparkLayer;

		private float oldAvailableResourceAmount = -1f;

		private float oldTotalResourceAmount = -1f;

		private float oldReserverResourceAmount = -1f;

		public Tag Tag { get; private set; }

		public GameObject GameObject { get; private set; }

		public ScreenRowBase(Tag tag, GameObject gameObject)
		{
			Tag = tag;
			GameObject = gameObject;
			HierarchyReferences component = GameObject.GetComponent<HierarchyReferences>();
			availableLabel = component.GetReference<LocText>("AvailableLabel");
			totalLabel = component.GetReference<LocText>("TotalLabel");
			reservedLabel = component.GetReference<LocText>("ReservedLabel");
			sparkLayer = component.GetReference<SparkLayer>("Chart");
		}

		public bool CheckAvailableAmountChanged(float newAvailableResourceAmount, bool updateIfTrue)
		{
			bool num = newAvailableResourceAmount != oldAvailableResourceAmount;
			if (num && updateIfTrue)
			{
				oldAvailableResourceAmount = newAvailableResourceAmount;
			}
			return num;
		}

		public bool CheckTotalResourceAmountChanged(float newTotalResourceAmount, bool updateIfTrue)
		{
			bool num = newTotalResourceAmount != oldTotalResourceAmount;
			if (num && updateIfTrue)
			{
				oldTotalResourceAmount = newTotalResourceAmount;
			}
			return num;
		}

		public bool CheckReservedResourceAmountChanged(float newReservedResourceAmount, bool updateIfTrue)
		{
			bool num = newReservedResourceAmount != oldReserverResourceAmount;
			if (num && updateIfTrue)
			{
				oldReserverResourceAmount = newReservedResourceAmount;
			}
			return num;
		}
	}

	private class CategoryRow : ScreenRowBase
	{
		public FoldOutPanel FoldOutPanel { get; private set; }

		public CategoryRow(Tag tag, GameObject gameObject)
			: base(tag, gameObject)
		{
			FoldOutPanel = base.GameObject.GetComponent<FoldOutPanel>();
		}
	}

	private class ResourceRow : ScreenRowBase
	{
		public MultiToggle notificiationToggle;

		public MultiToggle pinToggle;

		public ResourceRow(Tag tag, GameObject gameObject)
			: base(tag, gameObject)
		{
			HierarchyReferences component = base.GameObject.GetComponent<HierarchyReferences>();
			notificiationToggle = component.GetReference<MultiToggle>("NotificationToggle");
			pinToggle = component.GetReference<MultiToggle>("PinToggle");
		}
	}

	private Dictionary<Tag, ResourceRow> resourceRows = new Dictionary<Tag, ResourceRow>();

	private Dictionary<Tag, CategoryRow> categoryRows = new Dictionary<Tag, CategoryRow>();

	public Dictionary<Tag, GameUtil.MeasureUnit> units = new Dictionary<Tag, GameUtil.MeasureUnit>();

	public GameObject rootListContainer;

	public GameObject resourceLinePrefab;

	public GameObject categoryLinePrefab;

	public KButton closeButton;

	public bool allowRefresh = true;

	private bool currentlyShown;

	[SerializeField]
	private KInputTextField searchInputField;

	[SerializeField]
	private KButton clearSearchButton;

	public static AllResourcesScreen Instance;

	public Dictionary<Tag, bool> currentlyDisplayedRows = new Dictionary<Tag, bool>();

	public List<TagSet> allowDisplayCategories = new List<TagSet>();

	private bool initialized;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.ConsumeMouseScroll = true;
		Init();
	}

	protected override void OnForcedCleanUp()
	{
		Instance = null;
		base.OnForcedCleanUp();
	}

	public void Init()
	{
		if (!initialized)
		{
			initialized = true;
			Populate();
			Game.Instance.Subscribe(1983128072, Populate);
			DiscoveredResources.Instance.OnDiscover += delegate
			{
				Populate();
			};
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
			KInputTextField kInputTextField = searchInputField;
			kInputTextField.onFocus = (System.Action)Delegate.Combine(kInputTextField.onFocus, (System.Action)delegate
			{
				base.isEditing = true;
			});
			searchInputField.onEndEdit.AddListener(delegate
			{
				base.isEditing = false;
			});
			Show(show: false);
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		currentlyShown = show;
		if (show)
		{
			ManagementMenu.Instance.CloseAll();
			AllDiagnosticsScreen.Instance.Show(show: false);
			RefreshRows();
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
	}

	private void SpawnRows()
	{
		_ = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		allowDisplayCategories.Add(GameTags.MaterialCategories);
		allowDisplayCategories.Add(GameTags.CalorieCategories);
		allowDisplayCategories.Add(GameTags.UnitCategories);
		foreach (Tag materialCategory in GameTags.MaterialCategories)
		{
			SpawnCategoryRow(materialCategory, GameUtil.MeasureUnit.mass);
		}
		foreach (Tag calorieCategory in GameTags.CalorieCategories)
		{
			SpawnCategoryRow(calorieCategory, GameUtil.MeasureUnit.kcal);
		}
		foreach (Tag unitCategory in GameTags.UnitCategories)
		{
			SpawnCategoryRow(unitCategory, GameUtil.MeasureUnit.quantity);
		}
		List<Tag> list = new List<Tag>();
		foreach (KeyValuePair<Tag, CategoryRow> categoryRow in categoryRows)
		{
			list.Add(categoryRow.Key);
		}
		list.Sort((Tag a, Tag b) => a.ProperNameStripLink().CompareTo(b.ProperNameStripLink()));
		foreach (Tag item in list)
		{
			categoryRows[item].GameObject.transform.SetAsLastSibling();
		}
	}

	private void SpawnCategoryRow(Tag categoryTag, GameUtil.MeasureUnit unit)
	{
		GameObject gameObject = null;
		if (!categoryRows.ContainsKey(categoryTag))
		{
			GameObject gameObject2 = Util.KInstantiateUI(categoryLinePrefab, rootListContainer, force_active: true);
			CategoryRow categoryRow = new CategoryRow(categoryTag, gameObject2);
			gameObject2.GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel").SetText(categoryTag.ProperNameStripLink());
			categoryRows.Add(categoryTag, categoryRow);
			currentlyDisplayedRows.Add(categoryTag, value: true);
			units.Add(categoryTag, unit);
			GraphBase component = categoryRow.sparkLayer.GetComponent<GraphBase>();
			component.axis_x.min_value = 0f;
			component.axis_x.max_value = 600f;
			component.axis_x.guide_frequency = 120f;
			component.RefreshGuides();
		}
		gameObject = categoryRows[categoryTag].FoldOutPanel.container;
		foreach (Tag item in DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(categoryTag))
		{
			if (resourceRows.ContainsKey(item))
			{
				continue;
			}
			GameObject gameObject3 = Util.KInstantiateUI(resourceLinePrefab, gameObject, force_active: true);
			HierarchyReferences component2 = gameObject3.GetComponent<HierarchyReferences>();
			Tuple<Sprite, Color> uISprite = Def.GetUISprite(item);
			component2.GetReference<Image>("Icon").sprite = uISprite.first;
			component2.GetReference<Image>("Icon").color = uISprite.second;
			component2.GetReference<LocText>("NameLabel").SetText(item.ProperNameStripLink());
			Tag targetTag = item;
			MultiToggle pinToggle = component2.GetReference<MultiToggle>("PinToggle");
			MultiToggle multiToggle = pinToggle;
			multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
			{
				if (ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Contains(targetTag))
				{
					ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Remove(targetTag);
				}
				else
				{
					ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Add(targetTag);
					if (DiscoveredResources.Instance.newDiscoveries.ContainsKey(targetTag))
					{
						DiscoveredResources.Instance.newDiscoveries.Remove(targetTag);
					}
				}
				RefreshPinnedState(targetTag);
				pinToggle.ChangeState(ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Contains(targetTag) ? 1 : 0);
			});
			gameObject3.GetComponent<MultiToggle>().onClick = pinToggle.onClick;
			MultiToggle notifyToggle = component2.GetReference<MultiToggle>("NotificationToggle");
			MultiToggle multiToggle2 = notifyToggle;
			multiToggle2.onClick = (System.Action)Delegate.Combine(multiToggle2.onClick, (System.Action)delegate
			{
				if (ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Contains(targetTag))
				{
					ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Remove(targetTag);
				}
				else
				{
					ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Add(targetTag);
				}
				RefreshPinnedState(targetTag);
				notifyToggle.ChangeState(ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Contains(targetTag) ? 1 : 0);
			});
			component2.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().axis_x.min_value = 0f;
			component2.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().axis_x.max_value = 600f;
			component2.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().axis_x.guide_frequency = 120f;
			component2.GetReference<SparkLayer>("Chart").GetComponent<GraphBase>().RefreshGuides();
			ResourceRow value = new ResourceRow(item, gameObject3);
			resourceRows.Add(item, value);
			currentlyDisplayedRows.Add(item, value: true);
			if (units.ContainsKey(item))
			{
				Debug.LogError("Trying to add " + item.ToString() + ":UnitType " + units[item].ToString() + " but units dictionary already has key " + item.ToString() + " with unit type:" + unit);
			}
			else
			{
				units.Add(item, unit);
			}
		}
	}

	private void FilterRowBySearch(Tag tag, string filter)
	{
		currentlyDisplayedRows[tag] = PassesSearchFilter(tag, filter);
	}

	private void SearchFilter(string search)
	{
		foreach (KeyValuePair<Tag, ResourceRow> resourceRow in resourceRows)
		{
			FilterRowBySearch(resourceRow.Key, search);
		}
		foreach (KeyValuePair<Tag, CategoryRow> categoryRow in categoryRows)
		{
			if (PassesSearchFilter(categoryRow.Key, search))
			{
				currentlyDisplayedRows[categoryRow.Key] = true;
				foreach (Tag item in DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(categoryRow.Key))
				{
					if (currentlyDisplayedRows.ContainsKey(item))
					{
						currentlyDisplayedRows[item] = true;
					}
				}
			}
			else
			{
				currentlyDisplayedRows[categoryRow.Key] = false;
			}
		}
		EnableCategoriesByActiveChildren();
		SetRowsActive();
	}

	private bool PassesSearchFilter(Tag tag, string filter)
	{
		filter = filter.ToUpper();
		string text = tag.ProperName().ToUpper();
		if (filter != "" && !text.Contains(filter) && !tag.Name.ToUpper().Contains(filter))
		{
			return false;
		}
		return true;
	}

	private void EnableCategoriesByActiveChildren()
	{
		foreach (KeyValuePair<Tag, CategoryRow> categoryRow in categoryRows)
		{
			if (DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(categoryRow.Key).Count == 0)
			{
				currentlyDisplayedRows[categoryRow.Key] = false;
				continue;
			}
			GameObject container = categoryRow.Value.GameObject.GetComponent<FoldOutPanel>().container;
			foreach (KeyValuePair<Tag, ResourceRow> resourceRow in resourceRows)
			{
				if (!(resourceRow.Value.GameObject.transform.parent.gameObject != container))
				{
					currentlyDisplayedRows[categoryRow.Key] = currentlyDisplayedRows[categoryRow.Key] || currentlyDisplayedRows[resourceRow.Key];
				}
			}
		}
	}

	private void RefreshPinnedState(Tag tag)
	{
		resourceRows[tag].notificiationToggle.ChangeState(ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Contains(tag) ? 1 : 0);
		resourceRows[tag].pinToggle.ChangeState(ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Contains(tag) ? 1 : 0);
	}

	public void RefreshRows()
	{
		WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
		EnableCategoriesByActiveChildren();
		SetRowsActive();
		if (!allowRefresh)
		{
			return;
		}
		foreach (KeyValuePair<Tag, CategoryRow> categoryRow in categoryRows)
		{
			if (!categoryRow.Value.GameObject.activeInHierarchy)
			{
				continue;
			}
			float amount = worldInventory.GetAmount(categoryRow.Key, includeRelatedWorlds: false);
			float totalAmount = worldInventory.GetTotalAmount(categoryRow.Key, includeRelatedWorlds: false);
			if (!worldInventory.HasValidCount)
			{
				categoryRow.Value.availableLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
				categoryRow.Value.totalLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
				categoryRow.Value.reservedLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
				continue;
			}
			switch (units[categoryRow.Key])
			{
			case GameUtil.MeasureUnit.mass:
				if (categoryRow.Value.CheckAvailableAmountChanged(amount, updateIfTrue: true))
				{
					categoryRow.Value.availableLabel.SetText(GameUtil.GetFormattedMass(amount));
				}
				if (categoryRow.Value.CheckTotalResourceAmountChanged(totalAmount, updateIfTrue: true))
				{
					categoryRow.Value.totalLabel.SetText(GameUtil.GetFormattedMass(totalAmount));
				}
				if (categoryRow.Value.CheckReservedResourceAmountChanged(totalAmount - amount, updateIfTrue: true))
				{
					categoryRow.Value.reservedLabel.SetText(GameUtil.GetFormattedMass(totalAmount - amount));
				}
				break;
			case GameUtil.MeasureUnit.kcal:
			{
				float calories = RationTracker.Get().CountRations(null, ClusterManager.Instance.activeWorld.worldInventory);
				if (categoryRow.Value.CheckAvailableAmountChanged(amount, updateIfTrue: true))
				{
					categoryRow.Value.availableLabel.SetText(GameUtil.GetFormattedCalories(calories));
				}
				if (categoryRow.Value.CheckTotalResourceAmountChanged(totalAmount, updateIfTrue: true))
				{
					categoryRow.Value.totalLabel.SetText(GameUtil.GetFormattedCalories(totalAmount));
				}
				if (categoryRow.Value.CheckReservedResourceAmountChanged(totalAmount - amount, updateIfTrue: true))
				{
					categoryRow.Value.reservedLabel.SetText(GameUtil.GetFormattedCalories(totalAmount - amount));
				}
				break;
			}
			case GameUtil.MeasureUnit.quantity:
				if (categoryRow.Value.CheckAvailableAmountChanged(amount, updateIfTrue: true))
				{
					categoryRow.Value.availableLabel.SetText(GameUtil.GetFormattedUnits(amount));
				}
				if (categoryRow.Value.CheckTotalResourceAmountChanged(totalAmount, updateIfTrue: true))
				{
					categoryRow.Value.totalLabel.SetText(GameUtil.GetFormattedUnits(totalAmount));
				}
				if (categoryRow.Value.CheckReservedResourceAmountChanged(totalAmount - amount, updateIfTrue: true))
				{
					categoryRow.Value.reservedLabel.SetText(GameUtil.GetFormattedUnits(totalAmount - amount));
				}
				break;
			}
		}
		foreach (KeyValuePair<Tag, ResourceRow> resourceRow in resourceRows)
		{
			if (!resourceRow.Value.GameObject.activeInHierarchy)
			{
				continue;
			}
			float amount2 = worldInventory.GetAmount(resourceRow.Key, includeRelatedWorlds: false);
			float totalAmount2 = worldInventory.GetTotalAmount(resourceRow.Key, includeRelatedWorlds: false);
			if (!worldInventory.HasValidCount)
			{
				resourceRow.Value.availableLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
				resourceRow.Value.totalLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
				resourceRow.Value.reservedLabel.SetText(UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
			}
			else
			{
				switch (units[resourceRow.Key])
				{
				case GameUtil.MeasureUnit.mass:
					if (resourceRow.Value.CheckAvailableAmountChanged(amount2, updateIfTrue: true))
					{
						resourceRow.Value.availableLabel.SetText(GameUtil.GetFormattedMass(amount2));
					}
					if (resourceRow.Value.CheckTotalResourceAmountChanged(totalAmount2, updateIfTrue: true))
					{
						resourceRow.Value.totalLabel.SetText(GameUtil.GetFormattedMass(totalAmount2));
					}
					if (resourceRow.Value.CheckReservedResourceAmountChanged(totalAmount2 - amount2, updateIfTrue: true))
					{
						resourceRow.Value.reservedLabel.SetText(GameUtil.GetFormattedMass(totalAmount2 - amount2));
					}
					break;
				case GameUtil.MeasureUnit.kcal:
				{
					float num = RationTracker.Get().CountRationsByFoodType(resourceRow.Key.Name, ClusterManager.Instance.activeWorld.worldInventory);
					if (resourceRow.Value.CheckAvailableAmountChanged(num, updateIfTrue: true))
					{
						resourceRow.Value.availableLabel.SetText(GameUtil.GetFormattedCalories(num));
					}
					if (resourceRow.Value.CheckTotalResourceAmountChanged(totalAmount2, updateIfTrue: true))
					{
						resourceRow.Value.totalLabel.SetText(GameUtil.GetFormattedCalories(totalAmount2));
					}
					if (resourceRow.Value.CheckReservedResourceAmountChanged(totalAmount2 - amount2, updateIfTrue: true))
					{
						resourceRow.Value.reservedLabel.SetText(GameUtil.GetFormattedCalories(totalAmount2 - amount2));
					}
					break;
				}
				case GameUtil.MeasureUnit.quantity:
					if (resourceRow.Value.CheckAvailableAmountChanged(amount2, updateIfTrue: true))
					{
						resourceRow.Value.availableLabel.SetText(GameUtil.GetFormattedUnits(amount2));
					}
					if (resourceRow.Value.CheckTotalResourceAmountChanged(totalAmount2, updateIfTrue: true))
					{
						resourceRow.Value.totalLabel.SetText(GameUtil.GetFormattedUnits(totalAmount2));
					}
					if (resourceRow.Value.CheckReservedResourceAmountChanged(totalAmount2 - amount2, updateIfTrue: true))
					{
						resourceRow.Value.reservedLabel.SetText(GameUtil.GetFormattedUnits(totalAmount2 - amount2));
					}
					break;
				}
			}
			RefreshPinnedState(resourceRow.Key);
		}
	}

	public int UniqueResourceRowCount()
	{
		return resourceRows.Count;
	}

	private void RefreshCharts()
	{
		float time = GameClock.Instance.GetTime();
		float num = 3000f;
		foreach (KeyValuePair<Tag, CategoryRow> categoryRow in categoryRows)
		{
			ResourceTracker resourceStatistic = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, categoryRow.Key);
			if (resourceStatistic != null)
			{
				SparkLayer sparkLayer = categoryRow.Value.sparkLayer;
				Tuple<float, float>[] array = resourceStatistic.ChartableData(num);
				if (array.Length != 0)
				{
					sparkLayer.graph.axis_x.max_value = array[array.Length - 1].first;
				}
				else
				{
					sparkLayer.graph.axis_x.max_value = 0f;
				}
				sparkLayer.graph.axis_x.min_value = time - num;
				sparkLayer.RefreshLine(array, "resourceAmount");
			}
			else
			{
				DebugUtil.DevLogError("DevError: No tracker found for resource category " + categoryRow.Key.ToString());
			}
		}
		foreach (KeyValuePair<Tag, ResourceRow> resourceRow in resourceRows)
		{
			if (!resourceRow.Value.GameObject.activeInHierarchy)
			{
				continue;
			}
			ResourceTracker resourceStatistic2 = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, resourceRow.Key);
			if (resourceStatistic2 != null)
			{
				SparkLayer sparkLayer2 = resourceRow.Value.sparkLayer;
				Tuple<float, float>[] array2 = resourceStatistic2.ChartableData(num);
				if (array2.Length != 0)
				{
					sparkLayer2.graph.axis_x.max_value = array2[array2.Length - 1].first;
				}
				else
				{
					sparkLayer2.graph.axis_x.max_value = 0f;
				}
				sparkLayer2.graph.axis_x.min_value = time - num;
				sparkLayer2.RefreshLine(array2, "resourceAmount");
			}
			else
			{
				DebugUtil.DevLogError("DevError: No tracker found for resource " + resourceRow.Key.ToString());
			}
		}
	}

	private void SetRowsActive()
	{
		foreach (KeyValuePair<Tag, CategoryRow> categoryRow in categoryRows)
		{
			if (categoryRow.Value.GameObject.activeSelf != currentlyDisplayedRows[categoryRow.Key])
			{
				categoryRow.Value.GameObject.SetActive(currentlyDisplayedRows[categoryRow.Key]);
			}
		}
		foreach (KeyValuePair<Tag, ResourceRow> resourceRow in resourceRows)
		{
			if (resourceRow.Value.GameObject.activeSelf != currentlyDisplayedRows[resourceRow.Key])
			{
				resourceRow.Value.GameObject.SetActive(currentlyDisplayedRows[resourceRow.Key]);
			}
		}
	}

	public void Sim4000ms(float dt)
	{
		if (currentlyShown)
		{
			RefreshCharts();
		}
	}

	public void Sim1000ms(float dt)
	{
		if (currentlyShown)
		{
			RefreshRows();
		}
	}
}
