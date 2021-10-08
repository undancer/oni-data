using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CodexConversionPanel : CodexWidget<CodexConversionPanel>
{
	private LocText label;

	private GameObject materialPrefab;

	private GameObject fabricatorPrefab;

	private GameObject ingredientsContainer;

	private GameObject resultsContainer;

	private GameObject fabricatorContainer;

	private GameObject arrow1;

	private GameObject arrow2;

	private string title;

	private ElementUsage[] ins;

	private ElementUsage[] outs;

	private GameObject Converter;

	public CodexConversionPanel(string title, Tag ctag, float inputAmount, bool inputContinuous, Tag ptag, float outputAmount, bool outputContinuous, GameObject converter)
	{
		this.title = title;
		ins = new ElementUsage[1]
		{
			new ElementUsage(ctag, inputAmount, inputContinuous)
		};
		outs = new ElementUsage[1]
		{
			new ElementUsage(ptag, outputAmount, outputContinuous)
		};
		Converter = converter;
	}

	public CodexConversionPanel(string title, ElementUsage[] ins, ElementUsage[] outs, GameObject converter)
	{
		this.title = title;
		this.ins = ((ins != null) ? ins : new ElementUsage[0]);
		this.outs = ((outs != null) ? outs : new ElementUsage[0]);
		Converter = converter;
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		HierarchyReferences component = contentGameObject.GetComponent<HierarchyReferences>();
		label = component.GetReference<LocText>("Title");
		materialPrefab = component.GetReference<RectTransform>("MaterialPrefab").gameObject;
		fabricatorPrefab = component.GetReference<RectTransform>("FabricatorPrefab").gameObject;
		ingredientsContainer = component.GetReference<RectTransform>("IngredientsContainer").gameObject;
		resultsContainer = component.GetReference<RectTransform>("ResultsContainer").gameObject;
		fabricatorContainer = component.GetReference<RectTransform>("FabricatorContainer").gameObject;
		arrow1 = component.GetReference<RectTransform>("Arrow1").gameObject;
		arrow2 = component.GetReference<RectTransform>("Arrow2").gameObject;
		ClearPanel();
		ConfigureConversion();
	}

	private Tuple<Sprite, Color> GetUISprite(Tag tag)
	{
		if (ElementLoader.GetElement(tag) != null)
		{
			return Def.GetUISprite(ElementLoader.GetElement(tag));
		}
		if (Assets.GetPrefab(tag) != null)
		{
			return Def.GetUISprite(Assets.GetPrefab(tag));
		}
		if (Assets.GetSprite(tag.Name) != null)
		{
			return new Tuple<Sprite, Color>(Assets.GetSprite(tag.Name), Color.white);
		}
		return null;
	}

	private void ConfigureConversion()
	{
		label.text = title;
		bool active = false;
		ElementUsage[] array = ins;
		foreach (ElementUsage elementUsage in array)
		{
			Tag tag2 = elementUsage.tag;
			if (!(tag2 == Tag.Invalid))
			{
				float amount = elementUsage.amount;
				active = true;
				HierarchyReferences component = Util.KInstantiateUI(materialPrefab, ingredientsContainer, force_active: true).GetComponent<HierarchyReferences>();
				Tuple<Sprite, Color> uISprite = GetUISprite(tag2);
				if (uISprite != null)
				{
					component.GetReference<Image>("Icon").sprite = uISprite.first;
					component.GetReference<Image>("Icon").color = uISprite.second;
				}
				GameUtil.TimeSlice timeSlice = (elementUsage.continuous ? GameUtil.TimeSlice.PerCycle : GameUtil.TimeSlice.None);
				component.GetReference<LocText>("Amount").text = GameUtil.GetFormattedByTag(tag2, amount, timeSlice);
				component.GetReference<LocText>("Amount").color = Color.black;
				string text = tag2.ProperName();
				GameObject prefab = Assets.GetPrefab(tag2);
				if ((bool)prefab && prefab.GetComponent<Edible>() != null)
				{
					text = text + "\n    • " + string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(prefab.GetComponent<Edible>().GetQuality()));
				}
				component.GetReference<ToolTip>("Tooltip").toolTip = text;
				component.GetReference<KButton>("Button").onClick += delegate
				{
					ManagementMenu.Instance.codexScreen.ChangeArticle(UI.ExtractLinkID(tag2.ProperName()));
				};
			}
		}
		arrow1.SetActive(active);
		string name = Converter.PrefabID().Name;
		HierarchyReferences component2 = Util.KInstantiateUI(fabricatorPrefab, fabricatorContainer, force_active: true).GetComponent<HierarchyReferences>();
		Tuple<Sprite, Color> uISprite2 = Def.GetUISprite(name);
		component2.GetReference<Image>("Icon").sprite = uISprite2.first;
		component2.GetReference<Image>("Icon").color = uISprite2.second;
		component2.GetReference<ToolTip>("Tooltip").toolTip = Converter.GetProperName();
		component2.GetReference<KButton>("Button").onClick += delegate
		{
			ManagementMenu.Instance.codexScreen.ChangeArticle(UI.ExtractLinkID(Converter.GetProperName()));
		};
		bool active2 = false;
		array = outs;
		foreach (ElementUsage elementUsage2 in array)
		{
			Tag tag = elementUsage2.tag;
			if (!(tag == Tag.Invalid))
			{
				float amount2 = elementUsage2.amount;
				active2 = true;
				HierarchyReferences component3 = Util.KInstantiateUI(materialPrefab, resultsContainer, force_active: true).GetComponent<HierarchyReferences>();
				Tuple<Sprite, Color> uISprite3 = GetUISprite(tag);
				if (uISprite3 != null)
				{
					component3.GetReference<Image>("Icon").sprite = uISprite3.first;
					component3.GetReference<Image>("Icon").color = uISprite3.second;
				}
				GameUtil.TimeSlice timeSlice2 = (elementUsage2.continuous ? GameUtil.TimeSlice.PerCycle : GameUtil.TimeSlice.None);
				component3.GetReference<LocText>("Amount").text = GameUtil.GetFormattedByTag(tag, amount2, timeSlice2);
				component3.GetReference<LocText>("Amount").color = Color.black;
				string text2 = tag.ProperName();
				GameObject prefab2 = Assets.GetPrefab(tag);
				if ((bool)prefab2 && prefab2.GetComponent<Edible>() != null)
				{
					text2 = text2 + "\n    • " + string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(prefab2.GetComponent<Edible>().GetQuality()));
				}
				component3.GetReference<ToolTip>("Tooltip").toolTip = text2;
				component3.GetReference<KButton>("Button").onClick += delegate
				{
					ManagementMenu.Instance.codexScreen.ChangeArticle(UI.ExtractLinkID(tag.ProperName()));
				};
			}
		}
		arrow2.SetActive(active2);
	}

	private void ClearPanel()
	{
		foreach (Transform item in ingredientsContainer.transform)
		{
			Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in resultsContainer.transform)
		{
			Object.Destroy(item2.gameObject);
		}
		foreach (Transform item3 in fabricatorContainer.transform)
		{
			Object.Destroy(item3.gameObject);
		}
	}
}
