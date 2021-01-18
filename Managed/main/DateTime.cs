using UnityEngine;

public class DateTime : KScreen
{
	public static DateTime Instance;

	public LocText day;

	private int displayedDayCount = -1;

	[SerializeField]
	private LocText text;

	[SerializeField]
	private ToolTip tooltip;

	[SerializeField]
	private TextStyleSetting tooltipstyle_Days;

	[SerializeField]
	private TextStyleSetting tooltipstyle_Playtime;

	[SerializeField]
	public KToggle scheduleToggle;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		tooltip.OnComplexToolTip = SaveGame.Instance.GetColonyToolTip;
	}

	private void Update()
	{
		if (GameClock.Instance != null && displayedDayCount != GameUtil.GetCurrentCycle())
		{
			text.text = Days();
			displayedDayCount = GameUtil.GetCurrentCycle();
		}
	}

	private string Days()
	{
		return GameUtil.GetCurrentCycle().ToString();
	}
}
