using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Klei;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/GameClock")]
public class GameClock : KMonoBehaviour, ISaveLoadable, ISim33ms, IRender1000ms
{
	public static GameClock Instance;

	[Serialize]
	private int frame;

	[Serialize]
	private float time;

	[Serialize]
	private float timeSinceStartOfCycle;

	[Serialize]
	private int cycle;

	[Serialize]
	private float timePlayed;

	[Serialize]
	private bool isNight;

	public static readonly string NewCycleKey = "NewCycle";

	private Dictionary<string, object> newDayMetric = new Dictionary<string, object>
	{
		{
			NewCycleKey,
			null
		}
	};

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		timeSinceStartOfCycle = 50f;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (time != 0f)
		{
			cycle = (int)(time / 600f);
			timeSinceStartOfCycle = Mathf.Max(time - (float)cycle * 600f, 0f);
			time = 0f;
		}
	}

	public void Sim33ms(float dt)
	{
		AddTime(dt);
	}

	public void Render1000ms(float dt)
	{
		timePlayed += dt;
	}

	private void LateUpdate()
	{
		frame++;
	}

	private void AddTime(float dt)
	{
		timeSinceStartOfCycle += dt;
		bool flag = false;
		while (timeSinceStartOfCycle >= 600f)
		{
			cycle++;
			timeSinceStartOfCycle -= 600f;
			Trigger(631075836);
			flag = true;
		}
		if (!isNight && IsNighttime())
		{
			isNight = true;
			Trigger(-722330267);
		}
		if (isNight && !IsNighttime())
		{
			isNight = false;
		}
		if (flag && SaveGame.Instance.AutoSaveCycleInterval > 0 && cycle % SaveGame.Instance.AutoSaveCycleInterval == 0)
		{
			DoAutoSave(cycle);
		}
	}

	public float GetTimeSinceStartOfReport()
	{
		if (IsNighttime())
		{
			return 525f - GetTimeSinceStartOfCycle();
		}
		return GetTimeSinceStartOfCycle() + 75f;
	}

	public float GetTimeSinceStartOfCycle()
	{
		return timeSinceStartOfCycle;
	}

	public float GetCurrentCycleAsPercentage()
	{
		return timeSinceStartOfCycle / 600f;
	}

	public float GetTime()
	{
		return timeSinceStartOfCycle + (float)cycle * 600f;
	}

	public int GetFrame()
	{
		return frame;
	}

	public int GetCycle()
	{
		return cycle;
	}

	public bool IsNighttime()
	{
		return !(Instance.GetCurrentCycleAsPercentage() < 0.875f);
	}

	public void SetTime(float new_time)
	{
		float dt = Mathf.Max(new_time - GetTime(), 0f);
		AddTime(dt);
	}

	public float GetTimePlayedInSeconds()
	{
		return timePlayed;
	}

	private void DoAutoSave(int day)
	{
		if (GenericGameSettings.instance.disableAutosave)
		{
			return;
		}
		day++;
		newDayMetric[NewCycleKey] = day;
		ThreadedHttps<KleiMetrics>.Instance.SendEvent(newDayMetric, "DoAutoSave");
		string text = SaveLoader.GetActiveSaveFilePath();
		if (text == null)
		{
			text = SaveLoader.GetAutosaveFilePath();
		}
		int num = text.LastIndexOf("\\");
		if (num > 0)
		{
			int num2 = text.IndexOf(" Cycle ", num);
			if (num2 > 0)
			{
				text = text.Substring(0, num2);
			}
		}
		text = Path.ChangeExtension(text, null);
		text = text + " Cycle " + day;
		text = SaveScreen.GetValidSaveFilename(text);
		text = Path.Combine(SaveLoader.GetActiveAutoSavePath(), Path.GetFileName(text));
		string text2 = text;
		int num3 = 1;
		while (File.Exists(text))
		{
			text = text2.Replace(".sav", "");
			text = SaveScreen.GetValidSaveFilename(text2 + " (" + num3 + ")");
			num3++;
		}
		Game.Instance.StartDelayedSave(text, isAutoSave: true, updateSavePointer: false);
	}
}
