using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TemplateSelectionInfoPanel")]
public class TemplateSelectionInfoPanel : KMonoBehaviour, IRender1000ms
{
	[SerializeField]
	private GameObject prefab_detail_label;

	[SerializeField]
	private GameObject current_detail_container;

	[SerializeField]
	private LocText saved_detail_label;

	[SerializeField]
	private KButton save_button;

	private Func<List<int>, string>[] details = new Func<List<int>, string>[8] { TotalMass, AverageMass, AverageTemperature, TotalJoules, JoulesPerKilogram, MassPerElement, TotalRadiation, AverageRadiation };

	private static List<Tuple<Element, float>> mass_per_element = new List<Tuple<Element, float>>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		for (int i = 0; i < details.Length; i++)
		{
			Util.KInstantiateUI(prefab_detail_label, current_detail_container, force_active: true);
		}
		RefreshDetails();
		save_button.onClick += SaveCurrentDetails;
	}

	public void SaveCurrentDetails()
	{
		string text = "";
		for (int i = 0; i < details.Length; i++)
		{
			text = text + details[i](DebugBaseTemplateButton.Instance.SelectedCells) + "\n";
		}
		text += UI.HORIZONTAL_BR_RULE;
		text += saved_detail_label.text;
		saved_detail_label.text = text;
	}

	public void Render1000ms(float dt)
	{
		RefreshDetails();
	}

	public void RefreshDetails()
	{
		for (int i = 0; i < details.Length; i++)
		{
			current_detail_container.transform.GetChild(i).GetComponent<LocText>().text = details[i](DebugBaseTemplateButton.Instance.SelectedCells);
		}
	}

	private static string TotalMass(List<int> cells)
	{
		float num = 0f;
		foreach (int cell in cells)
		{
			num += Grid.Mass[cell];
		}
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.TOTAL_MASS, GameUtil.GetFormattedMass(num));
	}

	private static string AverageMass(List<int> cells)
	{
		float num = 0f;
		foreach (int cell in cells)
		{
			num += Grid.Mass[cell];
		}
		num /= (float)cells.Count;
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.AVERAGE_MASS, GameUtil.GetFormattedMass(num));
	}

	private static string AverageTemperature(List<int> cells)
	{
		float num = 0f;
		foreach (int cell in cells)
		{
			num += Grid.Temperature[cell];
		}
		num /= (float)cells.Count;
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.AVERAGE_TEMPERATURE, GameUtil.GetFormattedTemperature(num));
	}

	private static string TotalJoules(List<int> cells)
	{
		float num = 0f;
		foreach (int cell in cells)
		{
			num += Grid.Element[cell].specificHeatCapacity * Grid.Temperature[cell] * (Grid.Mass[cell] * 1000f);
		}
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.TOTAL_JOULES, GameUtil.GetFormattedJoules(num));
	}

	private static string JoulesPerKilogram(List<int> cells)
	{
		float num = 0f;
		float num2 = 0f;
		foreach (int cell in cells)
		{
			num += Grid.Element[cell].specificHeatCapacity * Grid.Temperature[cell] * (Grid.Mass[cell] * 1000f);
			num2 += Grid.Mass[cell];
		}
		num /= num2;
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.JOULES_PER_KILOGRAM, GameUtil.GetFormattedJoules(num));
	}

	private static string TotalRadiation(List<int> cells)
	{
		float num = 0f;
		foreach (int cell in cells)
		{
			num += Grid.Radiation[cell];
		}
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.TOTAL_RADS, GameUtil.GetFormattedRads(num));
	}

	private static string AverageRadiation(List<int> cells)
	{
		float num = 0f;
		foreach (int cell in cells)
		{
			num += Grid.Radiation[cell];
		}
		num /= (float)cells.Count;
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.AVERAGE_RADS, GameUtil.GetFormattedRads(num));
	}

	private static string MassPerElement(List<int> cells)
	{
		mass_per_element.Clear();
		foreach (int cell in cells)
		{
			bool flag = false;
			for (int i = 0; i < mass_per_element.Count; i++)
			{
				if (mass_per_element[i].first == Grid.Element[cell])
				{
					mass_per_element[i].second += Grid.Mass[cell];
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				mass_per_element.Add(new Tuple<Element, float>(Grid.Element[cell], Grid.Mass[cell]));
			}
		}
		mass_per_element.Sort(delegate(Tuple<Element, float> a, Tuple<Element, float> b)
		{
			if (a.second > b.second)
			{
				return -1;
			}
			return (b.second > a.second) ? 1 : 0;
		});
		string text = "";
		foreach (Tuple<Element, float> item in mass_per_element)
		{
			text = text + item.first.name + ": " + GameUtil.GetFormattedMass(item.second) + "\n";
		}
		return text;
	}
}
