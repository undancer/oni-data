using System;

public class VisibleAreaUpdater
{
	private GridVisibleArea VisibleArea;

	private Action<int> OutsideViewFirstTimeCallback;

	private Action<int> InsideViewFirstTimeCallback;

	private Action<int> UpdateCallback;

	private string Name;

	public VisibleAreaUpdater(Action<int> outside_view_first_time_cb, Action<int> inside_view_first_time_cb, string name)
	{
		OutsideViewFirstTimeCallback = outside_view_first_time_cb;
		InsideViewFirstTimeCallback = inside_view_first_time_cb;
		UpdateCallback = InternalUpdateCell;
		Name = name;
	}

	public void Update()
	{
		if (CameraController.Instance != null && VisibleArea == null)
		{
			VisibleArea = CameraController.Instance.VisibleArea;
			VisibleArea.Run(InsideViewFirstTimeCallback);
		}
	}

	private void InternalUpdateCell(int cell)
	{
		OutsideViewFirstTimeCallback(cell);
		InsideViewFirstTimeCallback(cell);
	}

	public void UpdateCell(int cell)
	{
		if (VisibleArea != null)
		{
			VisibleArea.RunIfVisible(cell, UpdateCallback);
		}
	}
}
