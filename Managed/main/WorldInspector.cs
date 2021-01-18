using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class WorldInspector : MonoBehaviour
{
	[Serializable]
	public struct StateSetting
	{
		public Color StateColor;

		public Color StateColor_Dark;

		public Sprite TemperatureBarBG;

		public Sprite StateIcon;
	}

	[Serializable]
	public struct PropertyIcons
	{
		public Sprite Mass;

		public Sprite Rations;

		public Sprite Quality;

		public Sprite Resource;
	}

	public Text PropertyLeftText;

	public Text PropertyRightText;

	public Image PropertyIcon_Left;

	public Image PropertyIcon_Right;

	public PropertyIcons propertySprites;

	public GameObject TemperatureNotch;

	public Image TemperatureNotchBG;

	public Image TemperatureNotchSymbol;

	public Text TemperatureTextDisplay;

	public Image TemperatureBarImage;

	public Image TransitionStateIcon_Low;

	public Image TransitionStateIcon_High;

	public ToolTip Tooltip_CurrentTemperature;

	public StateSetting SolidState;

	public StateSetting LiquidState;

	public StateSetting GasState;

	private float temperaturePositionWidgetX_Min = 30f;

	private float temperaturePositionWidgetX_Max = 172f;

	private static readonly string[] massStrings = new string[4];

	private static readonly string[] invalidCellMassStrings = new string[4]
	{
		"",
		"",
		"",
		""
	};

	private static float cachedMass = -1f;

	private static Element cachedElement;

	private void Update()
	{
		Refresh();
	}

	private void Refresh()
	{
		if (!(SelectTool.Instance.selected == null))
		{
			CellSelectionObject component = SelectTool.Instance.selected.GetComponent<CellSelectionObject>();
			if (component != null)
			{
				UpdateAsSimCell(component);
			}
			ElementChunk component2 = SelectTool.Instance.selected.GetComponent<ElementChunk>();
			if (component2 != null)
			{
				UpdateAsElementChunk(component2);
			}
			Edible component3 = SelectTool.Instance.selected.GetComponent<Edible>();
			if (component3 != null)
			{
				UpdateAsEdible(component3);
			}
		}
	}

	private void UpdateAsSimCell(CellSelectionObject cellObject)
	{
		string[] array = MassStringsReadOnly(cellObject.mouseCell);
		PropertyLeftText.text = array[0] + array[1] + " " + array[2];
		PropertyIcon_Left.sprite = propertySprites.Mass;
		PropertyRightText.text = cellObject.tags.ProperName();
		PropertyIcon_Right.sprite = propertySprites.Resource;
		TemperatureTextDisplay.text = GameUtil.GetFormattedTemperature(Mathf.RoundToInt(cellObject.temperature));
		Tooltip_CurrentTemperature.toolTip = "Current Temperature: " + GameUtil.GetFormattedTemperature(Mathf.RoundToInt(cellObject.temperature));
		int state = GetState(cellObject);
		SetStateColorScheme(state);
		Tooltip_CurrentTemperature.toolTip = SetCurrentTemperatureTooltip(cellObject.element, state);
		if ((cellObject.state & Element.State.TemperatureInsulated) == Element.State.TemperatureInsulated)
		{
			TemperatureNotch.SetActive(value: false);
		}
		else
		{
			TemperatureNotch.SetActive(value: true);
		}
		TemperatureNotch.rectTransform().anchoredPosition = new Vector2(GetTemperaturePosition(cellObject), TemperatureNotch.rectTransform().anchoredPosition.y);
	}

	private void UpdateAsElementChunk(ElementChunk _chunkObject)
	{
		PrimaryElement component = _chunkObject.GetComponent<PrimaryElement>();
		string text = "";
		PropertyLeftText.text = $"{component.Mass:0.00}" + " kg";
		PropertyIcon_Left.sprite = propertySprites.Mass;
		PropertyRightText.text = ElementLoader.FindElementByHash(component.ElementID).GetMaterialCategoryTag().ProperName();
		PropertyIcon_Right.sprite = propertySprites.Resource;
		TemperatureTextDisplay.text = GameUtil.GetFormattedTemperature(Mathf.RoundToInt(component.Temperature));
		text = "Current Temperature: " + GameUtil.GetFormattedTemperature(Mathf.RoundToInt(component.Temperature));
		SetStateColorScheme(0);
		text += SetCurrentTemperatureTooltip(ElementLoader.FindElementByHash(component.ElementID), 0);
		text = text + "\nMelts at: <color=yellow>" + GameUtil.GetFormattedTemperature(Mathf.RoundToInt(ElementLoader.FindElementByHash(component.ElementID).highTemp)) + "</color>";
		Tooltip_CurrentTemperature.toolTip = text;
		TemperatureNotch.SetActive(value: true);
		TemperatureNotch.rectTransform().anchoredPosition = new Vector2(GetTemperaturePosition(component), TemperatureNotch.rectTransform().anchoredPosition.y);
	}

	private void UpdateAsEdible(Edible edibleObject)
	{
		string text = "";
		PropertyLeftText.text = edibleObject.Units + " Rations";
		PropertyIcon_Left.sprite = propertySprites.Rations;
		PropertyRightText.text = edibleObject.GetQuality().ToString();
		PropertyIcon_Right.sprite = propertySprites.Quality;
		float f = Grid.Temperature[Grid.PosToCell(edibleObject)];
		TemperatureTextDisplay.text = GameUtil.GetFormattedTemperature(Mathf.RoundToInt(f));
		text = "Current Temperature: " + GameUtil.GetFormattedTemperature(Mathf.RoundToInt(f));
		SetStateColorScheme(0);
		text = text + "\nRots at temperatures above: <color=yellow>" + GameUtil.GetFormattedTemperature(Mathf.RoundToInt(edibleObject.FoodInfo.RotTemperature)) + "</color>";
		Tooltip_CurrentTemperature.toolTip = text;
		TemperatureNotch.SetActive(value: true);
		TemperatureNotch.rectTransform().anchoredPosition = new Vector2(GetTemperaturePosition(edibleObject), TemperatureNotch.rectTransform().anchoredPosition.y);
	}

	private int GetState(CellSelectionObject cellObject)
	{
		int result = 0;
		if (cellObject.element.IsGas)
		{
			result = 2;
		}
		else if (cellObject.element.IsLiquid)
		{
			result = 1;
		}
		else if (cellObject.element.IsSolid)
		{
			result = 0;
		}
		return result;
	}

	private void SetStateColorScheme(int state)
	{
		switch (state)
		{
		case 0:
			TemperatureBarImage.sprite = SolidState.TemperatureBarBG;
			TemperatureNotchSymbol.sprite = SolidState.StateIcon;
			TemperatureNotchBG.color = SolidState.StateColor;
			TemperatureTextDisplay.color = SolidState.StateColor;
			TransitionStateIcon_Low.sprite = SolidState.StateIcon;
			TransitionStateIcon_Low.color = SolidState.StateColor;
			TransitionStateIcon_High.sprite = LiquidState.StateIcon;
			TransitionStateIcon_High.color = LiquidState.StateColor;
			break;
		case 1:
			TemperatureBarImage.sprite = LiquidState.TemperatureBarBG;
			TemperatureNotchSymbol.sprite = LiquidState.StateIcon;
			TemperatureNotchBG.color = LiquidState.StateColor;
			TemperatureTextDisplay.color = LiquidState.StateColor;
			TransitionStateIcon_Low.sprite = SolidState.StateIcon;
			TransitionStateIcon_Low.color = SolidState.StateColor;
			TransitionStateIcon_High.sprite = GasState.StateIcon;
			TransitionStateIcon_High.color = GasState.StateColor;
			break;
		case 2:
			TemperatureBarImage.sprite = GasState.TemperatureBarBG;
			TemperatureNotchSymbol.sprite = GasState.StateIcon;
			TemperatureNotchBG.color = GasState.StateColor;
			TemperatureTextDisplay.color = GasState.StateColor;
			TransitionStateIcon_Low.sprite = LiquidState.StateIcon;
			TransitionStateIcon_Low.color = LiquidState.StateColor;
			TransitionStateIcon_High.sprite = GasState.StateIcon;
			TransitionStateIcon_High.color = GasState.StateColor;
			break;
		}
	}

	private string SetCurrentTemperatureTooltip(Element element, int state)
	{
		string text = "";
		switch (state)
		{
		case 0:
			text = text + "\nMelts at: <color=yellow>" + GameUtil.GetFormattedTemperature(Mathf.RoundToInt(element.highTemp)) + "</color>";
			break;
		case 1:
			text = text + "\nFreezes at: <color=cyan>" + GameUtil.GetFormattedTemperature(Mathf.RoundToInt(element.lowTemp)) + "</color>";
			text = text + "\nEvaporates at: <color=red>" + GameUtil.GetFormattedTemperature(Mathf.RoundToInt(element.highTemp)) + "</color>";
			break;
		case 2:
			text = text + "\nCondenses at: <color=yellow>" + GameUtil.GetFormattedTemperature(Mathf.RoundToInt(element.lowTemp)) + "</color>";
			break;
		}
		return text;
	}

	private float GetTemperaturePosition(CellSelectionObject cellObject)
	{
		float num = 1f;
		float num2 = 2000f;
		switch (GetState(cellObject))
		{
		case 0:
			num2 = cellObject.element.highTemp;
			break;
		case 1:
			num = cellObject.element.lowTemp;
			num2 = cellObject.element.highTemp;
			break;
		case 2:
			num = cellObject.element.lowTemp;
			num2 = num + 300f;
			break;
		}
		float num3 = num2 - num;
		float num4 = temperaturePositionWidgetX_Min;
		float num5 = temperaturePositionWidgetX_Max;
		float num6 = num5 - num4;
		return Mathf.Clamp(num4 + (cellObject.temperature - num) * num6 / num3, num4, num5);
	}

	private float GetTemperaturePosition(PrimaryElement chunkObject)
	{
		float num = 1f;
		float num2 = ElementLoader.FindElementByHash(chunkObject.ElementID).highTemp - num;
		float num3 = temperaturePositionWidgetX_Min;
		float num4 = temperaturePositionWidgetX_Max;
		float num5 = num4 - num3;
		return Mathf.Clamp(num3 + (chunkObject.Temperature - num) * num5 / num2, num3, num4);
	}

	private float GetTemperaturePosition(Edible edibleObject)
	{
		float num = 1f;
		float num2 = edibleObject.FoodInfo.RotTemperature - num;
		float num3 = temperaturePositionWidgetX_Min;
		float num4 = temperaturePositionWidgetX_Max;
		float num5 = num4 - num3;
		float num6 = Grid.Temperature[Grid.PosToCell(edibleObject)];
		return Mathf.Clamp(num3 + num6 * num5 / num2, num3, num4);
	}

	public static void DestroyStatics()
	{
		cachedElement = null;
	}

	public static string[] MassStringsReadOnly(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return invalidCellMassStrings;
		}
		Element element = Grid.Element[cell];
		float num = Grid.Mass[cell];
		if (element == cachedElement && num == cachedMass)
		{
			return massStrings;
		}
		cachedElement = element;
		cachedMass = num;
		massStrings[3] = " " + GameUtil.GetBreathableString(element, num);
		if (element.id == SimHashes.Vacuum)
		{
			massStrings[0] = "N/A";
			massStrings[1] = "";
			massStrings[2] = "";
		}
		else if (element.id == SimHashes.Unobtanium)
		{
			massStrings[0] = UI.NEUTRONIUMMASS;
			massStrings[1] = "";
			massStrings[2] = "";
		}
		else
		{
			massStrings[2] = UI.UNITSUFFIXES.MASS.KILOGRAM;
			if (num < 5f)
			{
				num *= 1000f;
				massStrings[2] = UI.UNITSUFFIXES.MASS.GRAM;
			}
			if (num < 5f)
			{
				num *= 1000f;
				massStrings[2] = UI.UNITSUFFIXES.MASS.MILLIGRAM;
			}
			if (num < 5f)
			{
				num *= 1000f;
				massStrings[2] = UI.UNITSUFFIXES.MASS.MICROGRAM;
				num = Mathf.Floor(num);
			}
			int num2 = Mathf.FloorToInt(num);
			massStrings[0] = num2.ToString();
			float num3 = Mathf.FloorToInt(10f * (num - (float)num2));
			massStrings[1] = "." + num3;
		}
		return massStrings;
	}
}
