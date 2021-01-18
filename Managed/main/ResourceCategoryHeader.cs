using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ResourceCategoryHeader")]
public class ResourceCategoryHeader : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, ISim4000ms
{
	[Serializable]
	public struct ElementReferences
	{
		public LocText LabelText;

		public LocText QuantityText;
	}

	public GameObject Prefab_ResourceEntry;

	public Transform EntryContainer;

	public Tag ResourceCategoryTag;

	public GameUtil.MeasureUnit Measure;

	public bool IsOpen = false;

	public ImageToggleState expandArrow;

	private Button mButton;

	public Dictionary<Tag, ResourceEntry> ResourcesDiscovered = new Dictionary<Tag, ResourceEntry>();

	public ElementReferences elements;

	public Color TextColor_Interactable;

	public Color TextColor_NonInteractable;

	private string quantityString = null;

	private float currentQuantity = 0f;

	private bool anyDiscovered = false;

	public const float chartHistoryLength = 3000f;

	[MyCmpGet]
	private ToolTip tooltip;

	[SerializeField]
	private int minimizedFontSize;

	[SerializeField]
	private int maximizedFontSize;

	[SerializeField]
	private Color highlightColour;

	[SerializeField]
	private Color BackgroundHoverColor;

	[SerializeField]
	private Image Background;

	public GameObject sparkChart;

	private float cachedAvailable = float.MinValue;

	private float cachedTotal = float.MinValue;

	private float cachedReserved = float.MinValue;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		EntryContainer.SetParent(base.transform.parent);
		EntryContainer.SetSiblingIndex(base.transform.GetSiblingIndex() + 1);
		EntryContainer.localScale = Vector3.one;
		mButton = GetComponent<Button>();
		mButton.onClick.AddListener(delegate
		{
			ToggleOpen(play_sound: true);
		});
		SetInteractable(anyDiscovered);
		SetActiveColor(state: false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		tooltip.OnToolTip = OnTooltip;
		UpdateContents();
		RefreshChart();
	}

	private void SetInteractable(bool state)
	{
		if (state)
		{
			if (!IsOpen)
			{
				expandArrow.SetInactive();
			}
			else
			{
				expandArrow.SetActive();
			}
		}
		else
		{
			SetOpen(open: false);
			expandArrow.SetDisabled();
		}
	}

	private void SetActiveColor(bool state)
	{
		if (state)
		{
			elements.QuantityText.color = TextColor_Interactable;
			elements.LabelText.color = TextColor_Interactable;
			expandArrow.ActiveColour = TextColor_Interactable;
			expandArrow.InactiveColour = TextColor_Interactable;
			expandArrow.TargetImage.color = TextColor_Interactable;
		}
		else
		{
			elements.LabelText.color = TextColor_NonInteractable;
			elements.QuantityText.color = TextColor_NonInteractable;
			expandArrow.ActiveColour = TextColor_NonInteractable;
			expandArrow.InactiveColour = TextColor_NonInteractable;
			expandArrow.TargetImage.color = TextColor_NonInteractable;
		}
	}

	public void SetTag(Tag t, GameUtil.MeasureUnit measure)
	{
		ResourceCategoryTag = t;
		Measure = measure;
		elements.LabelText.text = t.ProperName();
		if (SaveGame.Instance.expandedResourceTags.Contains(ResourceCategoryTag))
		{
			anyDiscovered = true;
			ToggleOpen(play_sound: false);
		}
	}

	private void ToggleOpen(bool play_sound)
	{
		if (!anyDiscovered)
		{
			if (play_sound)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
			}
		}
		else if (!IsOpen)
		{
			if (play_sound)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open"));
			}
			SetOpen(open: true);
			elements.LabelText.fontSize = maximizedFontSize;
			elements.QuantityText.fontSize = maximizedFontSize;
		}
		else
		{
			if (play_sound)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
			}
			SetOpen(open: false);
			elements.LabelText.fontSize = minimizedFontSize;
			elements.QuantityText.fontSize = minimizedFontSize;
		}
	}

	private void Hover(bool is_hovering)
	{
		Background.color = (is_hovering ? BackgroundHoverColor : new Color(0f, 0f, 0f, 0f));
		ICollection<Pickupable> collection = null;
		if (ClusterManager.Instance.activeWorld.worldInventory != null)
		{
			collection = ClusterManager.Instance.activeWorld.worldInventory.GetPickupables(ResourceCategoryTag);
		}
		if (collection == null)
		{
			return;
		}
		foreach (Pickupable item in collection)
		{
			if (!(item == null))
			{
				KAnimControllerBase component = item.GetComponent<KAnimControllerBase>();
				if (!(component == null))
				{
					component.HighlightColour = (is_hovering ? highlightColour : Color.black);
				}
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Hover(is_hovering: true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Hover(is_hovering: false);
	}

	public void SetOpen(bool open)
	{
		IsOpen = open;
		if (open)
		{
			expandArrow.SetActive();
			if (!SaveGame.Instance.expandedResourceTags.Contains(ResourceCategoryTag))
			{
				SaveGame.Instance.expandedResourceTags.Add(ResourceCategoryTag);
			}
		}
		else
		{
			expandArrow.SetInactive();
			SaveGame.Instance.expandedResourceTags.Remove(ResourceCategoryTag);
		}
		EntryContainer.gameObject.SetActive(IsOpen);
	}

	private void GetAmounts(bool doExtras, out float available, out float total, out float reserved)
	{
		available = 0f;
		total = 0f;
		reserved = 0f;
		HashSet<Tag> resources = null;
		if (!DiscoveredResources.Instance.TryGetDiscoveredResourcesFromTag(ResourceCategoryTag, out resources))
		{
			return;
		}
		ListPool<Tag, ResourceCategoryHeader>.PooledList pooledList = ListPool<Tag, ResourceCategoryHeader>.Allocate();
		foreach (Tag item in resources)
		{
			EdiblesManager.FoodInfo foodInfo = null;
			if (Measure == GameUtil.MeasureUnit.kcal)
			{
				foodInfo = EdiblesManager.GetFoodInfo(item.Name);
				if (foodInfo == null)
				{
					pooledList.Add(item);
					continue;
				}
			}
			anyDiscovered = true;
			ResourceEntry value = null;
			if (!ResourcesDiscovered.TryGetValue(item, out value))
			{
				value = NewResourceEntry(item, Measure);
				ResourcesDiscovered.Add(item, value);
			}
			value.GetAmounts(foodInfo, doExtras, out var available2, out var total2, out var reserved2);
			available += available2;
			total += total2;
			reserved += reserved2;
		}
		foreach (Tag item2 in pooledList)
		{
			resources.Remove(item2);
		}
		pooledList.Recycle();
	}

	public void UpdateContents()
	{
		GetAmounts(doExtras: false, out var available, out var total, out var reserved);
		if (available != cachedAvailable || total != cachedTotal || reserved != cachedReserved)
		{
			if (quantityString == null || currentQuantity != available)
			{
				switch (Measure)
				{
				case GameUtil.MeasureUnit.mass:
					quantityString = GameUtil.GetFormattedMass(available);
					break;
				case GameUtil.MeasureUnit.quantity:
					quantityString = available.ToString();
					break;
				case GameUtil.MeasureUnit.kcal:
					quantityString = GameUtil.GetFormattedCalories(available);
					break;
				}
				elements.QuantityText.text = quantityString;
				currentQuantity = available;
			}
			cachedAvailable = available;
			cachedTotal = total;
			cachedReserved = reserved;
		}
		foreach (KeyValuePair<Tag, ResourceEntry> item in ResourcesDiscovered)
		{
			item.Value.UpdateValue();
		}
		SetActiveColor(available > 0f);
		SetInteractable(anyDiscovered);
	}

	private string OnTooltip()
	{
		GetAmounts(doExtras: true, out var available, out var total, out var reserved);
		string str = elements.LabelText.text + "\n";
		str += string.Format(UI.RESOURCESCREEN.AVAILABLE_TOOLTIP, ResourceCategoryScreen.QuantityTextForMeasure(available, Measure), ResourceCategoryScreen.QuantityTextForMeasure(reserved, Measure), ResourceCategoryScreen.QuantityTextForMeasure(total, Measure));
		float delta = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, ResourceCategoryTag).GetDelta(150f);
		if (delta != 0f)
		{
			return str + "\n\n" + string.Format(UI.RESOURCESCREEN.TREND_TOOLTIP, (delta > 0f) ? UI.RESOURCESCREEN.INCREASING_STR : UI.RESOURCESCREEN.DECREASING_STR, GameUtil.GetFormattedMass(Mathf.Abs(delta)));
		}
		return str + "\n\n" + UI.RESOURCESCREEN.TREND_TOOLTIP_NO_CHANGE;
	}

	private ResourceEntry NewResourceEntry(Tag resourceTag, GameUtil.MeasureUnit measure)
	{
		GameObject gameObject = Util.KInstantiateUI(Prefab_ResourceEntry, EntryContainer.gameObject, force_active: true);
		ResourceEntry component = gameObject.GetComponent<ResourceEntry>();
		component.SetTag(resourceTag, measure);
		return component;
	}

	public void Sim4000ms(float dt)
	{
		RefreshChart();
	}

	private void RefreshChart()
	{
		if (sparkChart != null)
		{
			ResourceTracker resourceStatistic = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, ResourceCategoryTag);
			sparkChart.GetComponentInChildren<LineLayer>().RefreshLine(resourceStatistic.ChartableData(3000f), "resourceAmount");
			sparkChart.GetComponentInChildren<SparkLayer>().SetColor(Constants.NEUTRAL_COLOR);
		}
	}
}
