using System.Collections;
using System.Collections.Generic;
using Klei;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ResourceEntry")]
public class ResourceEntry : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Tag Resource;

	public GameUtil.MeasureUnit Measure;

	public LocText NameLabel;

	public LocText QuantityLabel;

	public Image image;

	[SerializeField]
	private Color AvailableColor;

	[SerializeField]
	private Color UnavailableColor;

	[SerializeField]
	private Color OverdrawnColor;

	[SerializeField]
	private Color HighlightColor;

	[SerializeField]
	private Color BackgroundHoverColor;

	[SerializeField]
	private Image Background;

	[MyCmpGet]
	private ToolTip tooltip;

	[MyCmpReq]
	private Button button;

	private const float CLICK_RESET_TIME_THRESHOLD = 10f;

	private int selectionIdx;

	private float lastClickTime;

	private List<Pickupable> cachedPickupables;

	private float currentQuantity = float.MinValue;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		QuantityLabel.color = AvailableColor;
		NameLabel.color = AvailableColor;
		button.onClick.AddListener(OnClick);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		tooltip.OnToolTip = OnToolTip;
	}

	private void OnClick()
	{
		lastClickTime = Time.unscaledTime;
		if (cachedPickupables == null)
		{
			cachedPickupables = WorldInventory.Instance.CreatePickupablesList(Resource);
			StartCoroutine(ClearCachedPickupablesAfterThreshold());
		}
		if (cachedPickupables == null)
		{
			return;
		}
		Pickupable pickupable = null;
		for (int i = 0; i < cachedPickupables.Count; i++)
		{
			selectionIdx++;
			int index = selectionIdx % cachedPickupables.Count;
			pickupable = cachedPickupables[index];
			if (pickupable != null && !pickupable.HasTag(GameTags.StoredPrivate))
			{
				break;
			}
		}
		if (!(pickupable != null))
		{
			return;
		}
		Transform transform = pickupable.transform;
		if (pickupable.storage != null)
		{
			transform = pickupable.storage.transform;
		}
		SelectTool.Instance.SelectAndFocus(transform.transform.GetPosition(), transform.GetComponent<KSelectable>(), Vector3.zero);
		for (int j = 0; j < cachedPickupables.Count; j++)
		{
			Pickupable pickupable2 = cachedPickupables[j];
			if (pickupable2 != null)
			{
				KAnimControllerBase component = pickupable2.GetComponent<KAnimControllerBase>();
				if (component != null)
				{
					component.HighlightColour = HighlightColor;
				}
			}
		}
	}

	private IEnumerator ClearCachedPickupablesAfterThreshold()
	{
		while (cachedPickupables != null && lastClickTime != 0f && Time.unscaledTime - lastClickTime < 10f)
		{
			yield return new WaitForSeconds(1f);
		}
		cachedPickupables = null;
	}

	public void GetAmounts(EdiblesManager.FoodInfo food_info, bool doExtras, out float available, out float total, out float reserved)
	{
		available = WorldInventory.Instance.GetAmount(Resource);
		total = (doExtras ? WorldInventory.Instance.GetTotalAmount(Resource) : 0f);
		reserved = (doExtras ? MaterialNeeds.Instance.GetAmount(Resource) : 0f);
		if (food_info != null)
		{
			available *= food_info.CaloriesPerUnit;
			total *= food_info.CaloriesPerUnit;
			reserved *= food_info.CaloriesPerUnit;
		}
	}

	private void GetAmounts(bool doExtras, out float available, out float total, out float reserved)
	{
		EdiblesManager.FoodInfo food_info = ((Measure == GameUtil.MeasureUnit.kcal) ? EdiblesManager.GetFoodInfo(Resource.Name) : null);
		GetAmounts(food_info, doExtras, out available, out total, out reserved);
	}

	public void UpdateValue()
	{
		SetName(Resource.ProperName());
		bool allowInsufficientMaterialBuild = GenericGameSettings.instance.allowInsufficientMaterialBuild;
		GetAmounts(allowInsufficientMaterialBuild, out var available, out var total, out var reserved);
		if (currentQuantity != available)
		{
			currentQuantity = available;
			QuantityLabel.text = ResourceCategoryScreen.QuantityTextForMeasure(available, Measure);
		}
		Color color = AvailableColor;
		if (reserved > total)
		{
			color = OverdrawnColor;
		}
		else if (available == 0f)
		{
			color = UnavailableColor;
		}
		if (QuantityLabel.color != color)
		{
			QuantityLabel.color = color;
		}
		if (NameLabel.color != color)
		{
			NameLabel.color = color;
		}
	}

	private string OnToolTip()
	{
		GetAmounts(doExtras: true, out var available, out var total, out var reserved);
		return string.Concat(NameLabel.text + "\n", string.Format(UI.RESOURCESCREEN.AVAILABLE_TOOLTIP, ResourceCategoryScreen.QuantityTextForMeasure(available, Measure), ResourceCategoryScreen.QuantityTextForMeasure(reserved, Measure), ResourceCategoryScreen.QuantityTextForMeasure(total, Measure)));
	}

	public void SetName(string name)
	{
		NameLabel.text = name;
	}

	public void SetTag(Tag t, GameUtil.MeasureUnit measure)
	{
		Resource = t;
		Measure = measure;
		cachedPickupables = null;
	}

	private void Hover(bool is_hovering)
	{
		if (WorldInventory.Instance == null)
		{
			return;
		}
		if (is_hovering)
		{
			Background.color = BackgroundHoverColor;
		}
		else
		{
			Background.color = new Color(0f, 0f, 0f, 0f);
		}
		ICollection<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(Resource);
		if (pickupables == null)
		{
			return;
		}
		foreach (Pickupable item in pickupables)
		{
			if (item == null)
			{
				continue;
			}
			KAnimControllerBase component = item.GetComponent<KAnimControllerBase>();
			if (!(component == null))
			{
				if (is_hovering)
				{
					component.HighlightColour = HighlightColor;
				}
				else
				{
					component.HighlightColour = Color.black;
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

	public void SetSprite(Tag t)
	{
		Element element = ElementLoader.FindElementByName(Resource.Name);
		if (element != null)
		{
			Sprite uISpriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(element.substance.anim);
			if (uISpriteFromMultiObjectAnim != null)
			{
				image.sprite = uISpriteFromMultiObjectAnim;
			}
		}
	}

	public void SetSprite(Sprite sprite)
	{
		image.sprite = sprite;
	}
}
