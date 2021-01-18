using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Operational")]
public class Operational : KMonoBehaviour
{
	public class Flag
	{
		public enum Type
		{
			Requirement,
			Functional
		}

		public string Name;

		public Type FlagType;

		public Flag(string name, Type type)
		{
			Name = name;
			FlagType = type;
		}
	}

	[Serialize]
	public float inactiveStartTime = 0f;

	[Serialize]
	public float activeStartTime = 0f;

	[Serialize]
	private List<float> uptimeData = new List<float>();

	[Serialize]
	private float activeTime = 0f;

	[Serialize]
	private float inactiveTime = 0f;

	private int MAX_DATA_POINTS = 5;

	public Dictionary<Flag, bool> Flags = new Dictionary<Flag, bool>();

	private static readonly EventSystem.IntraObjectHandler<Operational> OnNewBuildingDelegate = new EventSystem.IntraObjectHandler<Operational>(delegate(Operational component, object data)
	{
		component.OnNewBuilding(data);
	});

	public bool IsFunctional
	{
		get;
		private set;
	}

	public bool IsOperational
	{
		get;
		private set;
	}

	public bool IsActive
	{
		get;
		private set;
	}

	[OnSerializing]
	private void OnSerializing()
	{
		AddTimeData(IsActive);
		activeStartTime = GameClock.Instance.GetTime();
		inactiveStartTime = GameClock.Instance.GetTime();
	}

	protected override void OnPrefabInit()
	{
		UpdateFunctional();
		UpdateOperational();
		Subscribe(-1661515756, OnNewBuildingDelegate);
		GameClock.Instance.Subscribe(631075836, OnNewDay);
	}

	public void OnNewBuilding(object data)
	{
		BuildingComplete component = GetComponent<BuildingComplete>();
		if (component.creationTime > 0f)
		{
			inactiveStartTime = component.creationTime;
			activeStartTime = component.creationTime;
		}
	}

	public bool IsOperationalType(Flag.Type type)
	{
		if (type == Flag.Type.Functional)
		{
			return IsFunctional;
		}
		return IsOperational;
	}

	public void SetFlag(Flag flag, bool value)
	{
		bool value2 = false;
		if (Flags.TryGetValue(flag, out value2))
		{
			if (value2 != value)
			{
				Flags[flag] = value;
				Trigger(187661686, flag);
			}
		}
		else
		{
			Flags[flag] = value;
			Trigger(187661686, flag);
		}
		if (flag.FlagType == Flag.Type.Functional && value != IsFunctional)
		{
			UpdateFunctional();
		}
		if (value != IsOperational)
		{
			UpdateOperational();
		}
	}

	public bool GetFlag(Flag flag)
	{
		bool value = false;
		Flags.TryGetValue(flag, out value);
		return value;
	}

	private void UpdateFunctional()
	{
		bool isFunctional = true;
		foreach (KeyValuePair<Flag, bool> flag in Flags)
		{
			if (flag.Key.FlagType == Flag.Type.Functional && !flag.Value)
			{
				isFunctional = false;
				break;
			}
		}
		IsFunctional = isFunctional;
		Trigger(-1852328367, IsFunctional);
	}

	private void UpdateOperational()
	{
		Dictionary<Flag, bool>.Enumerator enumerator = Flags.GetEnumerator();
		bool flag = true;
		while (enumerator.MoveNext())
		{
			if (!enumerator.Current.Value)
			{
				flag = false;
				break;
			}
		}
		if (flag != IsOperational)
		{
			IsOperational = flag;
			if (!IsOperational)
			{
				SetActive(value: false);
			}
			if (IsOperational)
			{
				GetComponent<KPrefabID>().AddTag(GameTags.Operational);
			}
			else
			{
				GetComponent<KPrefabID>().RemoveTag(GameTags.Operational);
			}
			Trigger(-592767678, IsOperational);
			Game.Instance.Trigger(-809948329, base.gameObject);
		}
	}

	public void SetActive(bool value, bool force_ignore = false)
	{
		if (IsActive != value)
		{
			AddTimeData(value);
			Trigger(824508782, this);
			Game.Instance.Trigger(-809948329, base.gameObject);
		}
	}

	private void AddTimeData(bool value)
	{
		float num = (IsActive ? activeStartTime : inactiveStartTime);
		float time = GameClock.Instance.GetTime();
		float num2 = time - num;
		if (IsActive)
		{
			activeTime += num2;
		}
		else
		{
			inactiveTime += num2;
		}
		IsActive = value;
		if (IsActive)
		{
			activeStartTime = time;
		}
		else
		{
			inactiveStartTime = time;
		}
	}

	public void OnNewDay(object data)
	{
		AddTimeData(IsActive);
		uptimeData.Add(activeTime / 600f);
		while (uptimeData.Count > MAX_DATA_POINTS)
		{
			uptimeData.RemoveAt(0);
		}
		activeTime = 0f;
		inactiveTime = 0f;
	}

	public float GetCurrentCycleUptime()
	{
		if (IsActive)
		{
			float num = (IsActive ? activeStartTime : inactiveStartTime);
			float time = GameClock.Instance.GetTime();
			float num2 = time - num;
			return (activeTime + num2) / GameClock.Instance.GetTimeSinceStartOfCycle();
		}
		return activeTime / GameClock.Instance.GetTimeSinceStartOfCycle();
	}

	public float GetLastCycleUptime()
	{
		if (uptimeData.Count > 0)
		{
			return uptimeData[uptimeData.Count - 1];
		}
		return 0f;
	}

	public float GetUptimeOverCycles(int num_cycles)
	{
		if (uptimeData.Count > 0)
		{
			int num = Mathf.Min(uptimeData.Count, num_cycles);
			float num2 = 0f;
			for (int num3 = num - 1; num3 >= 0; num3--)
			{
				num2 += uptimeData[num3];
			}
			return num2 / (float)num;
		}
		return 0f;
	}
}
