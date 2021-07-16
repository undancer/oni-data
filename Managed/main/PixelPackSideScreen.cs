using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelPackSideScreen : SideScreenContent
{
	public PixelPack targetPixelPack;

	public KButton copyActiveToStandbyButton;

	public KButton copyStandbyToActiveButton;

	public KButton swapColorsButton;

	public GameObject colorSwatchContainer;

	public GameObject swatchEntry;

	public GameObject activeColorsContainer;

	public GameObject standbyColorsContainer;

	public List<GameObject> activeColors = new List<GameObject>();

	public List<GameObject> standbyColors = new List<GameObject>();

	public Color paintingColor;

	public GameObject selectedSwatchEntry;

	private Dictionary<Color, GameObject> swatch_object_by_color = new Dictionary<Color, GameObject>();

	private List<GameObject> highlightedSwatchGameObjects = new List<GameObject>();

	private List<Color> colorSwatch = new List<Color>
	{
		new Color(124f / 255f, 124f / 255f, 124f / 255f),
		new Color(0f, 0f, 84f / 85f),
		new Color(0f, 0f, 188f / 255f),
		new Color(4f / 15f, 8f / 51f, 188f / 255f),
		new Color(148f / 255f, 0f, 44f / 85f),
		new Color(56f / 85f, 0f, 32f / 255f),
		new Color(56f / 85f, 16f / 255f, 0f),
		new Color(8f / 15f, 4f / 51f, 0f),
		new Color(16f / 51f, 16f / 85f, 0f),
		new Color(0f, 0.47058824f, 0f),
		new Color(0f, 104f / 255f, 0f),
		new Color(0f, 88f / 255f, 0f),
		new Color(0f, 64f / 255f, 88f / 255f),
		new Color(0f, 0f, 0f),
		new Color(188f / 255f, 188f / 255f, 188f / 255f),
		new Color(0f, 0.47058824f, 248f / 255f),
		new Color(0f, 88f / 255f, 248f / 255f),
		new Color(104f / 255f, 4f / 15f, 84f / 85f),
		new Color(72f / 85f, 0f, 0.8f),
		new Color(76f / 85f, 0f, 88f / 255f),
		new Color(248f / 255f, 56f / 255f, 0f),
		new Color(76f / 85f, 92f / 255f, 16f / 255f),
		new Color(172f / 255f, 124f / 255f, 0f),
		new Color(0f, 184f / 255f, 0f),
		new Color(0f, 56f / 85f, 0f),
		new Color(0f, 56f / 85f, 4f / 15f),
		new Color(0f, 8f / 15f, 8f / 15f),
		new Color(0f, 0f, 0f),
		new Color(248f / 255f, 248f / 255f, 248f / 255f),
		new Color(0.23529412f, 188f / 255f, 84f / 85f),
		new Color(104f / 255f, 8f / 15f, 84f / 85f),
		new Color(152f / 255f, 0.47058824f, 248f / 255f),
		new Color(248f / 255f, 0.47058824f, 248f / 255f),
		new Color(248f / 255f, 88f / 255f, 152f / 255f),
		new Color(248f / 255f, 0.47058824f, 88f / 255f),
		new Color(84f / 85f, 32f / 51f, 4f / 15f),
		new Color(248f / 255f, 184f / 255f, 0f),
		new Color(184f / 255f, 248f / 255f, 8f / 85f),
		new Color(88f / 255f, 72f / 85f, 28f / 85f),
		new Color(88f / 255f, 248f / 255f, 152f / 255f),
		new Color(0f, 232f / 255f, 72f / 85f),
		new Color(0.47058824f, 0.47058824f, 0.47058824f),
		new Color(84f / 85f, 84f / 85f, 84f / 85f),
		new Color(164f / 255f, 76f / 85f, 84f / 85f),
		new Color(184f / 255f, 184f / 255f, 248f / 255f),
		new Color(72f / 85f, 184f / 255f, 248f / 255f),
		new Color(248f / 255f, 184f / 255f, 248f / 255f),
		new Color(248f / 255f, 184f / 255f, 64f / 85f),
		new Color(0.9411765f, 208f / 255f, 176f / 255f),
		new Color(84f / 85f, 224f / 255f, 56f / 85f),
		new Color(248f / 255f, 72f / 85f, 0.47058824f),
		new Color(72f / 85f, 248f / 255f, 0.47058824f),
		new Color(184f / 255f, 248f / 255f, 184f / 255f),
		new Color(184f / 255f, 248f / 255f, 72f / 85f),
		new Color(0f, 84f / 85f, 84f / 85f),
		new Color(72f / 85f, 72f / 85f, 72f / 85f)
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (swatch_object_by_color.Count == 0)
		{
			InitializeColorSwatch();
		}
		PopulateColorSelections();
		copyActiveToStandbyButton.onClick += CopyActiveToStandby;
		copyStandbyToActiveButton.onClick += CopyStandbyToActive;
		swapColorsButton.onClick += SwapColors;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<PixelPack>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		targetPixelPack = target.GetComponent<PixelPack>();
		PopulateColorSelections();
		HighlightUsedColors();
	}

	private void HighlightUsedColors()
	{
		if (swatch_object_by_color.Count == 0)
		{
			InitializeColorSwatch();
		}
		for (int i = 0; i < highlightedSwatchGameObjects.Count; i++)
		{
			highlightedSwatchGameObjects[i].GetComponent<HierarchyReferences>().GetReference("used").GetComponentInChildren<Image>()
				.gameObject.SetActive(value: false);
		}
		highlightedSwatchGameObjects.Clear();
		for (int j = 0; j < targetPixelPack.colorSettings.Count; j++)
		{
			swatch_object_by_color[targetPixelPack.colorSettings[j].activeColor].GetComponent<HierarchyReferences>().GetReference("used").gameObject.SetActive(value: true);
			swatch_object_by_color[targetPixelPack.colorSettings[j].standbyColor].GetComponent<HierarchyReferences>().GetReference("used").gameObject.SetActive(value: true);
			highlightedSwatchGameObjects.Add(swatch_object_by_color[targetPixelPack.colorSettings[j].activeColor]);
			highlightedSwatchGameObjects.Add(swatch_object_by_color[targetPixelPack.colorSettings[j].standbyColor]);
		}
	}

	private void PopulateColorSelections()
	{
		for (int i = 0; i < targetPixelPack.colorSettings.Count; i++)
		{
			int current_id = i;
			activeColors[i].GetComponent<Image>().color = targetPixelPack.colorSettings[i].activeColor;
			activeColors[i].GetComponent<KButton>().onClick += delegate
			{
				PixelPack.ColorPair value2 = targetPixelPack.colorSettings[current_id];
				activeColors[current_id].GetComponent<Image>().color = paintingColor;
				value2.activeColor = paintingColor;
				targetPixelPack.colorSettings[current_id] = value2;
				targetPixelPack.UpdateColors();
				HighlightUsedColors();
			};
			standbyColors[i].GetComponent<Image>().color = targetPixelPack.colorSettings[i].standbyColor;
			standbyColors[i].GetComponent<KButton>().onClick += delegate
			{
				PixelPack.ColorPair value = targetPixelPack.colorSettings[current_id];
				standbyColors[current_id].GetComponent<Image>().color = paintingColor;
				value.standbyColor = paintingColor;
				targetPixelPack.colorSettings[current_id] = value;
				targetPixelPack.UpdateColors();
				HighlightUsedColors();
			};
		}
	}

	private void InitializeColorSwatch()
	{
		bool flag = false;
		for (int i = 0; i < colorSwatch.Count; i++)
		{
			GameObject swatch = Util.KInstantiateUI(swatchEntry, colorSwatchContainer, force_active: true);
			Image component = swatch.GetComponent<Image>();
			component.color = colorSwatch[i];
			KButton component2 = swatch.GetComponent<KButton>();
			Color color = colorSwatch[i];
			if (component.color == Color.black)
			{
				swatch.GetComponent<HierarchyReferences>().GetReference("selected").GetComponentInChildren<Image>()
					.color = Color.white;
			}
			if (!flag)
			{
				SelectColor(color, swatch);
				flag = true;
			}
			component2.onClick += delegate
			{
				SelectColor(color, swatch);
			};
			swatch_object_by_color[color] = swatch;
		}
	}

	private void SelectColor(Color color, GameObject swatchEntry)
	{
		paintingColor = color;
		swatchEntry.GetComponent<HierarchyReferences>().GetReference("selected").gameObject.SetActive(value: true);
		if (selectedSwatchEntry != null && selectedSwatchEntry != swatchEntry)
		{
			selectedSwatchEntry.GetComponent<HierarchyReferences>().GetReference("selected").gameObject.SetActive(value: false);
		}
		selectedSwatchEntry = swatchEntry;
	}

	private void CopyActiveToStandby()
	{
		for (int i = 0; i < targetPixelPack.colorSettings.Count; i++)
		{
			PixelPack.ColorPair value = targetPixelPack.colorSettings[i];
			value.standbyColor = value.activeColor;
			targetPixelPack.colorSettings[i] = value;
			standbyColors[i].GetComponent<Image>().color = value.activeColor;
		}
		HighlightUsedColors();
		targetPixelPack.UpdateColors();
	}

	private void CopyStandbyToActive()
	{
		for (int i = 0; i < targetPixelPack.colorSettings.Count; i++)
		{
			PixelPack.ColorPair value = targetPixelPack.colorSettings[i];
			value.activeColor = value.standbyColor;
			targetPixelPack.colorSettings[i] = value;
			activeColors[i].GetComponent<Image>().color = value.standbyColor;
		}
		HighlightUsedColors();
		targetPixelPack.UpdateColors();
	}

	private void SwapColors()
	{
		for (int i = 0; i < targetPixelPack.colorSettings.Count; i++)
		{
			PixelPack.ColorPair value = default(PixelPack.ColorPair);
			value.activeColor = targetPixelPack.colorSettings[i].standbyColor;
			value.standbyColor = targetPixelPack.colorSettings[i].activeColor;
			targetPixelPack.colorSettings[i] = value;
			activeColors[i].GetComponent<Image>().color = value.activeColor;
			standbyColors[i].GetComponent<Image>().color = value.standbyColor;
		}
		HighlightUsedColors();
		targetPixelPack.UpdateColors();
	}
}
