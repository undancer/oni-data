using System;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/NextUpdateTimer")]
public class NextUpdateTimer : KMonoBehaviour
{
	public LocText TimerText;

	public KBatchedAnimController UpdateAnimController;

	public KBatchedAnimController UpdateAnimMeterController;

	public float initialAnimScale;

	public System.DateTime nextReleaseDate;

	public System.DateTime currentReleaseDate;

	private string m_releaseTextOverride;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		initialAnimScale = UpdateAnimController.animScale;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		RefreshReleaseTimes();
	}

	public void UpdateReleaseTimes(string lastUpdateTime, string nextUpdateTime, string textOverride)
	{
		if (!System.DateTime.TryParse(lastUpdateTime, out currentReleaseDate))
		{
			Debug.LogWarning("Failed to parse last_update_time: " + lastUpdateTime);
		}
		if (!System.DateTime.TryParse(nextUpdateTime, out nextReleaseDate))
		{
			Debug.LogWarning("Failed to parse next_update_time: " + nextUpdateTime);
		}
		m_releaseTextOverride = textOverride;
		RefreshReleaseTimes();
	}

	private void RefreshReleaseTimes()
	{
		TimeSpan timeSpan = nextReleaseDate - currentReleaseDate;
		TimeSpan timeSpan2 = nextReleaseDate - System.DateTime.UtcNow;
		TimeSpan timeSpan3 = System.DateTime.UtcNow - currentReleaseDate;
		string text = "";
		string text2 = "4";
		if (!string.IsNullOrEmpty(m_releaseTextOverride))
		{
			text = m_releaseTextOverride;
		}
		else if (timeSpan2.TotalHours < 8.0)
		{
			text = UI.DEVELOPMENTBUILDS.UPDATES.TWENTY_FOUR_HOURS;
			text2 = "4";
		}
		else if (timeSpan2.TotalDays < 1.0)
		{
			text = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.FINAL_WEEK, 1);
			text2 = "3";
		}
		else
		{
			int num = timeSpan2.Days % 7;
			int num2 = (timeSpan2.Days - num) / 7;
			if (num2 <= 0)
			{
				text = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.FINAL_WEEK, num);
				text2 = "2";
			}
			else
			{
				text = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.BIGGER_TIMES, num, num2);
				text2 = "1";
			}
		}
		TimerText.text = text;
		UpdateAnimController.Play(text2, KAnim.PlayMode.Loop);
		float positionPercent = Mathf.Clamp01((float)(timeSpan3.TotalSeconds / timeSpan.TotalSeconds));
		UpdateAnimMeterController.SetPositionPercent(positionPercent);
	}
}
