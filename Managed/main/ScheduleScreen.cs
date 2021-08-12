using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScheduleScreen : KScreen
{
	[SerializeField]
	private SchedulePaintButton paintButtonPrefab;

	[SerializeField]
	private GameObject paintButtonContainer;

	[SerializeField]
	private ScheduleScreenEntry scheduleEntryPrefab;

	[SerializeField]
	private GameObject scheduleEntryContainer;

	[SerializeField]
	private KButton addScheduleButton;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private ColorStyleSetting hygene_color;

	[SerializeField]
	private ColorStyleSetting work_color;

	[SerializeField]
	private ColorStyleSetting recreation_color;

	[SerializeField]
	private ColorStyleSetting sleep_color;

	private Dictionary<string, ColorStyleSetting> paintStyles;

	private List<ScheduleScreenEntry> entries;

	private List<SchedulePaintButton> paintButtons;

	private SchedulePaintButton selectedPaint;

	public override float GetSortKey()
	{
		return 50f;
	}

	protected override void OnPrefabInit()
	{
		base.ConsumeMouseScroll = true;
		entries = new List<ScheduleScreenEntry>();
		paintStyles = new Dictionary<string, ColorStyleSetting>();
		paintStyles["Hygene"] = hygene_color;
		paintStyles["Worktime"] = work_color;
		paintStyles["Recreation"] = recreation_color;
		paintStyles["Sleep"] = sleep_color;
	}

	protected override void OnSpawn()
	{
		paintButtons = new List<SchedulePaintButton>();
		foreach (ScheduleGroup allGroup in Db.Get().ScheduleGroups.allGroups)
		{
			AddPaintButton(allGroup);
		}
		foreach (Schedule schedule in ScheduleManager.Instance.GetSchedules())
		{
			AddScheduleEntry(schedule);
		}
		addScheduleButton.onClick += OnAddScheduleClick;
		closeButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		ScheduleManager.Instance.onSchedulesChanged += OnSchedulesChanged;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		ScheduleManager.Instance.onSchedulesChanged -= OnSchedulesChanged;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			Activate();
		}
	}

	private void AddPaintButton(ScheduleGroup group)
	{
		SchedulePaintButton schedulePaintButton = Util.KInstantiateUI<SchedulePaintButton>(paintButtonPrefab.gameObject, paintButtonContainer, force_active: true);
		schedulePaintButton.SetGroup(group, paintStyles, OnPaintButtonClick);
		schedulePaintButton.SetToggle(on: false);
		paintButtons.Add(schedulePaintButton);
	}

	private void OnAddScheduleClick()
	{
		ScheduleManager.Instance.AddDefaultSchedule(alarmOn: false);
	}

	private void OnPaintButtonClick(SchedulePaintButton clicked)
	{
		if (selectedPaint != clicked)
		{
			foreach (SchedulePaintButton paintButton in paintButtons)
			{
				paintButton.SetToggle(paintButton == clicked);
			}
			selectedPaint = clicked;
		}
		else
		{
			clicked.SetToggle(on: false);
			selectedPaint = null;
		}
	}

	private void OnPaintDragged(ScheduleScreenEntry entry, float ratio)
	{
		if (!(selectedPaint == null))
		{
			int idx = Mathf.FloorToInt(ratio * (float)entry.schedule.GetBlocks().Count);
			entry.schedule.SetGroup(idx, selectedPaint.group);
		}
	}

	private void AddScheduleEntry(Schedule schedule)
	{
		ScheduleScreenEntry scheduleScreenEntry = Util.KInstantiateUI<ScheduleScreenEntry>(scheduleEntryPrefab.gameObject, scheduleEntryContainer, force_active: true);
		scheduleScreenEntry.Setup(schedule, paintStyles, OnPaintDragged);
		entries.Add(scheduleScreenEntry);
	}

	private void OnSchedulesChanged(List<Schedule> schedules)
	{
		foreach (ScheduleScreenEntry entry in entries)
		{
			Util.KDestroyGameObject(entry);
		}
		entries.Clear();
		foreach (Schedule schedule in schedules)
		{
			AddScheduleEntry(schedule);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (CheckBlockedInput())
		{
			if (!e.Consumed)
			{
				e.Consumed = true;
			}
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private bool CheckBlockedInput()
	{
		bool result = false;
		if (UnityEngine.EventSystems.EventSystem.current != null)
		{
			GameObject currentSelectedGameObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != null)
			{
				foreach (ScheduleScreenEntry entry in entries)
				{
					if (currentSelectedGameObject == entry.GetNameInputField())
					{
						return true;
					}
				}
				return result;
			}
		}
		return result;
	}
}
