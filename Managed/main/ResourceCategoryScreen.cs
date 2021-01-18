using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceCategoryScreen : KScreen
{
	public static ResourceCategoryScreen Instance;

	public GameObject Prefab_CategoryBar;

	public Transform CategoryContainer;

	public MultiToggle HiderButton;

	public KLayoutElement HideTarget;

	private float HideSpeedFactor = 12f;

	private float maxHeightPadding = 280f;

	private float targetContentHideHeight;

	public Dictionary<Tag, ResourceCategoryHeader> DisplayedCategories = new Dictionary<Tag, ResourceCategoryHeader>();

	private Tag[] DisplayedCategoryKeys;

	private int categoryUpdatePacer;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Instance = this;
		base.ConsumeMouseScroll = true;
		MultiToggle hiderButton = HiderButton;
		hiderButton.onClick = (System.Action)Delegate.Combine(hiderButton.onClick, new System.Action(OnHiderClick));
		OnHiderClick();
		CreateTagSetHeaders(GameTags.MaterialCategories, GameUtil.MeasureUnit.mass);
		CreateTagSetHeaders(GameTags.CalorieCategories, GameUtil.MeasureUnit.kcal);
		CreateTagSetHeaders(GameTags.UnitCategories, GameUtil.MeasureUnit.quantity);
		if (!DisplayedCategories.ContainsKey(GameTags.Miscellaneous))
		{
			ResourceCategoryHeader value = NewCategoryHeader(GameTags.Miscellaneous, GameUtil.MeasureUnit.mass);
			DisplayedCategories.Add(GameTags.Miscellaneous, value);
		}
		DisplayedCategoryKeys = DisplayedCategories.Keys.ToArray();
	}

	private void CreateTagSetHeaders(IEnumerable<Tag> set, GameUtil.MeasureUnit measure)
	{
		foreach (Tag item in set)
		{
			ResourceCategoryHeader value = NewCategoryHeader(item, measure);
			DisplayedCategories.Add(item, value);
		}
	}

	private void OnHiderClick()
	{
		HiderButton.NextState();
		if (HiderButton.CurrentState == 0)
		{
			targetContentHideHeight = 0f;
		}
		else
		{
			targetContentHideHeight = Mathf.Min(((float)Screen.height - maxHeightPadding) / GameScreenManager.Instance.ssOverlayCanvas.GetComponent<KCanvasScaler>().GetCanvasScale(), CategoryContainer.rectTransform().rect.height);
		}
	}

	private void Update()
	{
		if (WorldInventory.Instance == null)
		{
			return;
		}
		if (HideTarget.minHeight != targetContentHideHeight)
		{
			float minHeight = HideTarget.minHeight;
			float num = targetContentHideHeight - minHeight;
			num = Mathf.Clamp(num * HideSpeedFactor * Time.unscaledDeltaTime, (num > 0f) ? (0f - num) : num, (num > 0f) ? num : (0f - num));
			minHeight += num;
			HideTarget.minHeight = minHeight;
		}
		for (int i = 0; i < 1; i++)
		{
			Tag tag = DisplayedCategoryKeys[categoryUpdatePacer];
			ResourceCategoryHeader resourceCategoryHeader = DisplayedCategories[tag];
			if (WorldInventory.Instance.IsDiscovered(tag) && !resourceCategoryHeader.gameObject.activeInHierarchy)
			{
				resourceCategoryHeader.gameObject.SetActive(value: true);
			}
			resourceCategoryHeader.UpdateContents();
			categoryUpdatePacer = (categoryUpdatePacer + 1) % DisplayedCategoryKeys.Length;
		}
		if (HiderButton.CurrentState != 0)
		{
			targetContentHideHeight = Mathf.Min(((float)Screen.height - maxHeightPadding) / GameScreenManager.Instance.ssOverlayCanvas.GetComponent<KCanvasScaler>().GetCanvasScale(), CategoryContainer.rectTransform().rect.height);
		}
		if (MeterScreen.Instance != null && !MeterScreen.Instance.StartValuesSet)
		{
			MeterScreen.Instance.InitializeValues();
		}
	}

	private ResourceCategoryHeader NewCategoryHeader(Tag categoryTag, GameUtil.MeasureUnit measure)
	{
		GameObject gameObject = Util.KInstantiateUI(Prefab_CategoryBar, CategoryContainer.gameObject);
		gameObject.name = "CategoryHeader_" + categoryTag.Name;
		ResourceCategoryHeader component = gameObject.GetComponent<ResourceCategoryHeader>();
		component.SetTag(categoryTag, measure);
		return component;
	}

	public static string QuantityTextForMeasure(float quantity, GameUtil.MeasureUnit measure)
	{
		return measure switch
		{
			GameUtil.MeasureUnit.mass => GameUtil.GetFormattedMass(quantity), 
			GameUtil.MeasureUnit.kcal => GameUtil.GetFormattedCalories(quantity), 
			_ => quantity.ToString(), 
		};
	}
}
