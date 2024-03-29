using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using STRINGS;
using TUNING;
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

	public GameObject stressSpark;

	public LocText RationsText;

	public ToolTip RationsTooltip;

	public GameObject rationsSpark;

	public LocText SickText;

	public ToolTip SickTooltip;

	public TextStyleSetting ToolTipStyle_Header;

	public TextStyleSetting ToolTipStyle_Property;

	private bool startValuesSet;

	public MultiToggle RedAlertButton;

	public ToolTip RedAlertTooltip;

	private DisplayInfo stressDisplayInfo = new DisplayInfo
	{
		selectedIndex = -1
	};

	private DisplayInfo immunityDisplayInfo = new DisplayInfo
	{
		selectedIndex = -1
	};

	private List<MinionIdentity> worldLiveMinionIdentities;

	private int cachedMinionCount = -1;

	private long cachedCalories = -1L;

	private Dictionary<string, float> rationsDict = new Dictionary<string, float>();

	public static MeterScreen Instance { get; private set; }

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
		MultiToggle redAlertButton = RedAlertButton;
		redAlertButton.onClick = (System.Action)Delegate.Combine(redAlertButton.onClick, (System.Action)delegate
		{
			OnRedAlertClick();
		});
		Game.Instance.Subscribe(1983128072, delegate
		{
			Refresh();
		});
		Game.Instance.Subscribe(1585324898, delegate
		{
			RefreshRedAlertButtonState();
		});
		Game.Instance.Subscribe(-1393151672, delegate
		{
			RefreshRedAlertButtonState();
		});
	}

	private void OnRedAlertClick()
	{
		bool flag = !ClusterManager.Instance.activeWorld.AlertManager.IsRedAlertToggledOn();
		ClusterManager.Instance.activeWorld.AlertManager.ToggleRedAlert(flag);
		if (flag)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open"));
		}
		else
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
		}
	}

	private void RefreshRedAlertButtonState()
	{
		RedAlertButton.ChangeState(ClusterManager.Instance.activeWorld.IsRedAlert() ? 1 : 0);
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
		RefreshWorldMinionIdentities();
		RefreshMinions();
		RefreshRations();
		RefreshStress();
		RefreshSick();
		RefreshRedAlertButtonState();
	}

	private void RefreshWorldMinionIdentities()
	{
		worldLiveMinionIdentities = new List<MinionIdentity>(from x in Components.LiveMinionIdentities.GetWorldItems(ClusterManager.Instance.activeWorldId)
			where !x.IsNullOrDestroyed()
			select x);
	}

	private List<MinionIdentity> GetWorldMinionIdentities()
	{
		if (worldLiveMinionIdentities == null)
		{
			RefreshWorldMinionIdentities();
		}
		return worldLiveMinionIdentities;
	}

	private void RefreshMinions()
	{
		int count = Components.LiveMinionIdentities.Count;
		int count2 = GetWorldMinionIdentities().Count;
		if (count2 != cachedMinionCount)
		{
			cachedMinionCount = count2;
			string text = "";
			if (DlcManager.FeatureClusterSpaceEnabled())
			{
				ClusterGridEntity component = ClusterManager.Instance.activeWorld.GetComponent<ClusterGridEntity>();
				text = string.Format(UI.TOOLTIPS.METERSCREEN_POPULATION_CLUSTER, component.Name, count2, count);
				currentMinions.text = $"{count2}/{count}";
			}
			else
			{
				currentMinions.text = $"{count}";
				text = string.Format(UI.TOOLTIPS.METERSCREEN_POPULATION, count.ToString("0"));
			}
			MinionsTooltip.ClearMultiStringTooltip();
			MinionsTooltip.AddMultiStringTooltip(text, ToolTipStyle_Header);
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
			long num = (long)RationTracker.Get().CountRations(null, ClusterManager.Instance.activeWorld.worldInventory);
			if (cachedCalories != num)
			{
				RationsText.text = GameUtil.GetFormattedCalories(num);
				cachedCalories = num;
			}
		}
		rationsSpark.GetComponentInChildren<SparkLayer>().SetColor(((float)cachedCalories > (float)GetWorldMinionIdentities().Count * 1000000f) ? Constants.NEUTRAL_COLOR : Constants.NEGATIVE_COLOR);
		rationsSpark.GetComponentInChildren<LineLayer>().RefreshLine(TrackerTool.Instance.GetWorldTracker<KCalTracker>(ClusterManager.Instance.activeWorldId).ChartableData(600f), "kcal");
	}

	private IList<MinionIdentity> GetStressedMinions()
	{
		Amount stress_amount = Db.Get().Amounts.Stress;
		return (from x in new List<MinionIdentity>(GetWorldMinionIdentities())
			where !x.IsNullOrDestroyed()
			orderby stress_amount.Lookup(x).value descending
			select x).ToList();
	}

	private string OnStressTooltip()
	{
		float maxStressInActiveWorld = GameUtil.GetMaxStressInActiveWorld();
		StressTooltip.ClearMultiStringTooltip();
		StressTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_AVGSTRESS, Mathf.Round(maxStressInActiveWorld) + "%"), ToolTipStyle_Header);
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
		List<MinionIdentity> worldMinionIdentities = GetWorldMinionIdentities();
		SickTooltip.ClearMultiStringTooltip();
		SickTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_SICK_DUPES, num.ToString()), ToolTipStyle_Header);
		for (int i = 0; i < worldMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = worldMinionIdentities[i];
			if (minionIdentity.IsNullOrDestroyed())
			{
				continue;
			}
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

	private int CountSickDupes()
	{
		int num = 0;
		foreach (MinionIdentity worldMinionIdentity in GetWorldMinionIdentities())
		{
			if (!worldMinionIdentity.IsNullOrDestroyed() && worldMinionIdentity.GetComponent<MinionModifiers>().sicknesses.IsInfected())
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
		float calories = RationTracker.Get().CountRations(rationsDict, ClusterManager.Instance.activeWorld.worldInventory);
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
		float maxStressInActiveWorld = GameUtil.GetMaxStressInActiveWorld();
		StressText.text = Mathf.Round(maxStressInActiveWorld).ToString();
		WorldTracker worldTracker = TrackerTool.Instance.GetWorldTracker<StressTracker>(ClusterManager.Instance.activeWorldId);
		stressSpark.GetComponentInChildren<SparkLayer>().SetColor((worldTracker.GetCurrentValue() >= STRESS.ACTING_OUT_RESET) ? Constants.NEGATIVE_COLOR : Constants.NEUTRAL_COLOR);
		stressSpark.GetComponentInChildren<LineLayer>().RefreshLine(worldTracker.ChartableData(600f), "stressData");
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
		return GetWorldMinionIdentities();
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
		if (!(base_ev_data is PointerEventData pointerEventData))
		{
			return;
		}
		List<MinionIdentity> worldMinionIdentities = GetWorldMinionIdentities();
		switch (pointerEventData.button)
		{
		case PointerEventData.InputButton.Left:
			if (worldMinionIdentities.Count < display_info.selectedIndex)
			{
				display_info.selectedIndex = -1;
			}
			if (worldMinionIdentities.Count > 0)
			{
				display_info.selectedIndex = (display_info.selectedIndex + 1) % worldMinionIdentities.Count;
				MinionIdentity minionIdentity = minions[display_info.selectedIndex];
				SelectTool.Instance.SelectAndFocus(minionIdentity.transform.GetPosition(), minionIdentity.GetComponent<KSelectable>(), new Vector3(5f, 0f, 0f));
			}
			break;
		case PointerEventData.InputButton.Right:
			display_info.selectedIndex = -1;
			break;
		}
	}
}
