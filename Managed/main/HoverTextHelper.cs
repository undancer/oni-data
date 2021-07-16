using STRINGS;
using UnityEngine;

public class HoverTextHelper
{
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

	public static void DestroyStatics()
	{
		cachedElement = null;
		cachedMass = -1f;
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
			massStrings[0] = UI.NA;
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
			float num3 = Mathf.RoundToInt(10f * (num - (float)num2));
			massStrings[1] = "." + num3;
		}
		return massStrings;
	}
}
