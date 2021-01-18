using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeterScreen : KScreen, IRender1000ms
{
	private struct DisplayInfo
	{
		public int selectedIndex;
	}

	[SerializeField]
	private LocText currentMinions;

	public ToolTip MinionsTooltip;

	public LocText StressText;

	public ToolTip StressTooltip;

	public LocText RationsText;

	public ToolTip RationsTooltip;

	public LocText SickText;

	public ToolTip SickTooltip;

	public TextStyleSetting ToolTipStyle_Header;

	public TextStyleSetting ToolTipStyle_Property;

	private bool startValuesSet;

	[SerializeField]
	private KToggle RedAlertButton;

	public ToolTip RedAlertTooltip;

	private DisplayInfo stressDisplayInfo;

	private DisplayInfo immunityDisplayInfo;

	private int cachedMinionCount;

	private long cachedCalories;

	private Dictionary<string, float> rationsDict;

	public static MeterScreen Instance
	{
		get;
		private set;
	}

	public bool StartValuesSet => startValuesSet;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	protected override void OnSpawn()
	{
		StressTooltip.OnToolTip = OnStressTooltip;
		SickTooltip.OnToolTip = OnSickTooltip;
		RationsTooltip.OnToolTip = OnRationsTooltip;
		RedAlertTooltip.OnToolTip = OnRedAlertTooltip;
		RedAlertButton.onClick += delegate
		{
			OnRedAlertClick();
		};
	}

	private void OnRedAlertClick()
	{
		bool flag = !VignetteManager.Instance.Get().IsRedAlertToggledOn();
		VignetteManager.Instance.Get().ToggleRedAlert(flag);
		if (flag)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open"));
		}
		else
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
		}
	}

	public void Render1000ms(float dt)
	{
		Refresh();
	}

	public void InitializeValues()
	{
		if (!startValuesSet)
		{
			startValuesSet = true;
			Refresh();
		}
	}

	private void Refresh()
	{
		RefreshMinions();
		RefreshRations();
		RefreshStress();
		RefreshSick();
	}

	private void RefreshMinions()
	{
		int count = Components.LiveMinionIdentities.Count;
		if (count != cachedMinionCount)
		{
			cachedMinionCount = count;
			currentMinions.text = count.ToString("0");
			MinionsTooltip.ClearMultiStringTooltip();
			MinionsTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_POPULATION, count.ToString("0")), ToolTipStyle_Header);
		}
	}

	private void RefreshSick()
	{
		int num = CountSickDupes();
		SickText.text = num.ToString();
	}

	private void RefreshRations()
	{
		if (RationsText != null && RationTracker.Get() != null)
		{
			long num = (long)RationTracker.Get().CountRations(null);
			if (cachedCalories != num)
			{
				RationsText.text = GameUtil.GetFormattedCalories(num);
				cachedCalories = num;
			}
		}
	}

	private IList<MinionIdentity> GetStressedMinions()
	{
		Amount stress_amount = Db.Get().Amounts.Stress;
		return new List<MinionIdentity>(new List<MinionIdentity>(Components.LiveMinionIdentities.Items).OrderByDescending((MinionIdentity x) => stress_amount.Lookup(x).value));
	}

	private string OnStressTooltip()
	{
		float maxStress = GameUtil.GetMaxStress();
		StressTooltip.ClearMultiStringTooltip();
		StressTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_AVGSTRESS, Mathf.Round(maxStress) + "%"), ToolTipStyle_Header);
		Amount stress = Db.Get().Amounts.Stress;
		IList<MinionIdentity> stressedMinions = GetStressedMinions();
		for (int i = 0; i < stressedMinions.Count; i++)
		{
			MinionIdentity minionIdentity = stressedMinions[i];
			AmountInstance amount = stress.Lookup(minionIdentity);
			AddToolTipAmountPercentLine(StressTooltip, amount, minionIdentity, i == stressDisplayInfo.selectedIndex);
		}
		return "";
	}

	private string OnSickTooltip()
	{
		int num = CountSickDupes();
		SickTooltip.ClearMultiStringTooltip();
		SickTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_SICK_DUPES, num.ToString()), ToolTipStyle_Header);
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = Components.LiveMinionIdentities[i];
			string text = minionIdentity.GetComponent<KSelectable>().GetName();
			Sicknesses sicknesses = minionIdentity.GetComponent<MinionModifiers>().sicknesses;
			if (sicknesses.IsInfected())
			{
				text += " (";
				int num2 = 0;
				foreach (SicknessInstance item in sicknesses)
				{
					text = text + ((num2 > 0) ? ", " : "") + item.modifier.Name;
					num2++;
				}
				text += ")";
			}
			bool selected = i == immunityDisplayInfo.selectedIndex;
			AddToolTipLine(SickTooltip, text, selected);
		}
		return "";
	}

	private static int CountSickDupes()
	{
		int num = 0;
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			if (item.GetComponent<MinionModifiers>().sicknesses.IsInfected())
			{
				num++;
			}
		}
		return num;
	}

	private void AddToolTipLine(ToolTip tooltip, string str, bool selected)
	{
		if (selected)
		{
			tooltip.AddMultiStringTooltip("<color=#F0B310FF>" + str + "</color>", ToolTipStyle_Property);
		}
		else
		{
			tooltip.AddMultiStringTooltip(str, ToolTipStyle_Property);
		}
	}

	private void AddToolTipAmountPercentLine(ToolTip tooltip, AmountInstance amount, MinionIdentity id, bool selected)
	{
		string str = id.GetComponent<KSelectable>().GetName() + ":  " + Mathf.Round(amount.value) + "%";
		AddToolTipLine(tooltip, str, selected);
	}

	private string OnRationsTooltip()
	{
		rationsDict.Clear();
		float calories = RationTracker.Get().CountRations(rationsDict);
		RationsText.text = GameUtil.GetFormattedCalories(calories);
		RationsTooltip.ClearMultiStringTooltip();
		RationsTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_MEALHISTORY, GameUtil.GetFormattedCalories(calories)), ToolTipStyle_Header);
		RationsTooltip.AddMultiStringTooltip("", ToolTipStyle_Property);
		foreach (KeyValuePair<string, float> item in rationsDict.OrderByDescending(delegate(KeyValuePair<string, float> x)
		{
			EdiblesManager.FoodInfo foodInfo2 = EdiblesManager.GetFoodInfo(x.Key);
			return x.Value * (foodInfo2?.CaloriesPerUnit ?? (-1f));
		}).ToDictionary((KeyValuePair<string, float> t) => t.Key, (KeyValuePair<string, float> t) => t.Value))
		{
			EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(item.Key);
			RationsTooltip.AddMultiStringTooltip((foodInfo != null) ? $"{foodInfo.Name}: {GameUtil.GetFormattedCalories(item.Value * foodInfo.CaloriesPerUnit)}" : string.Format(UI.TOOLTIPS.METERSCREEN_INVALID_FOOD_TYPE, item.Key), ToolTipStyle_Property);
		}
		return "";
	}

	private string OnRedAlertTooltip()
	{
		RedAlertTooltip.ClearMultiStringTooltip();
		RedAlertTooltip.AddMultiStringTooltip(UI.TOOLTIPS.RED_ALERT_TITLE, ToolTipStyle_Header);
		RedAlertTooltip.AddMultiStringTooltip(UI.TOOLTIPS.RED_ALERT_CONTENT, ToolTipStyle_Property);
		return "";
	}

	private void RefreshStress()
	{
		float maxStress = GameUtil.GetMaxStress();
		StressText.text = Mathf.Round(maxStress).ToString();
	}

	public void OnClickStress(BaseEventData base_ev_data)
	{
		IList<MinionIdentity> stressedMinions = GetStressedMinions();
		UpdateDisplayInfo(base_ev_data, ref stressDisplayInfo, stressedMinions);
		OnStressTooltip();
		StressTooltip.forceRefresh = true;
	}

	private IList<MinionIdentity> GetSickMinions()
	{
		return Components.LiveMinionIdentities.Items;
	}

	public void OnClickImmunity(BaseEventData base_ev_data)
	{
		IList<MinionIdentity> sickMinions = GetSickMinions();
		UpdateDisplayInfo(base_ev_data, ref immunityDisplayInfo, sickMinions);
		OnSickTooltip();
		SickTooltip.forceRefresh = true;
	}

	private void UpdateDisplayInfo(BaseEventData base_ev_data, ref DisplayInfo display_info, IList<MinionIdentity> minions)
	{
		PointerEventData pointerEventData = base_ev_data as PointerEventData;
		if (pointerEventData == null)
		{
			return;
		}
		switch (pointerEventData.button)
		{
		case PointerEventData.InputButton.Left:
			if (Components.LiveMinionIdentities.Count < display_info.selectedIndex)
			{
				display_info.selectedIndex = -1;
			}
			if (Components.LiveMinionIdentities.Count > 0)
			{
				display_info.selectedIndex = (display_info.selectedIndex + 1) % Components.LiveMinionIdentities.Count;
				MinionIdentity minionIdentity = minions[display_info.selectedIndex];
				SelectTool.Instance.SelectAndFocus(minionIdentity.transform.GetPosition(), minionIdentity.GetComponent<KSelectable>(), new Vector3(5f, 0f, 0f));
			}
			break;
		case PointerEventData.InputButton.Right:
			display_info.selectedIndex = -1;
			break;
		}
	}

	public MeterScreen()
	{
		DisplayInfo displayInfo = new DisplayInfo
		{
			selectedIndex = -1
		};
		stressDisplayInfo = displayInfo;
		displayInfo = new DisplayInfo
		{
			selectedIndex = -1
		};
		immunityDisplayInfo = displayInfo;
		cachedMinionCount = -1;
		cachedCalories = -1L;
		rationsDict = new Dictionary<string, float>();
		base._002Ector();
	}
}
