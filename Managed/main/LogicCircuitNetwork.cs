using System.Collections.Generic;
using System.Collections.ObjectModel;
using FMOD.Studio;
using STRINGS;
using UnityEngine;

public class LogicCircuitNetwork : UtilityNetwork
{
	public class LogicSoundPair
	{
		public int playedIndex = 0;

		public float lastPlayed = 0f;
	}

	private List<LogicWire>[] wireGroups = new List<LogicWire>[2];

	private List<LogicUtilityNetworkLink>[] relevantBridges = new List<LogicUtilityNetworkLink>[2];

	private List<ILogicEventReceiver> receivers = new List<ILogicEventReceiver>();

	private List<ILogicEventSender> senders = new List<ILogicEventSender>();

	private int previousValue = -1;

	private int outputValue;

	private bool resetting = false;

	public static float logicSoundLastPlayedTime = 0f;

	private const float MIN_OVERLOAD_TIME_FOR_DAMAGE = 6f;

	private const float MIN_OVERLOAD_NOTIFICATION_DISPLAY_TIME = 5f;

	private GameObject targetOverloadedWire = null;

	private float timeOverloaded = 0f;

	private float timeOverloadNotificationDisplayed = 0f;

	private Notification overloadedNotification;

	public static Dictionary<int, LogicSoundPair> logicSoundRegister = new Dictionary<int, LogicSoundPair>();

	public int OutputValue => outputValue;

	public int WireCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < 2; i++)
			{
				if (wireGroups[i] != null)
				{
					num += wireGroups[i].Count;
				}
			}
			return num;
		}
	}

	public ReadOnlyCollection<ILogicEventSender> Senders => senders.AsReadOnly();

	public ReadOnlyCollection<ILogicEventReceiver> Receivers => receivers.AsReadOnly();

	public override void AddItem(int cell, object item)
	{
		if (item is LogicWire)
		{
			LogicWire logicWire = (LogicWire)item;
			LogicWire.BitDepth maxBitDepth = logicWire.MaxBitDepth;
			List<LogicWire> list = wireGroups[(int)maxBitDepth];
			if (list == null)
			{
				list = new List<LogicWire>();
				wireGroups[(int)maxBitDepth] = list;
			}
			list.Add(logicWire);
		}
		else if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver item2 = (ILogicEventReceiver)item;
			receivers.Add(item2);
		}
		else if (item is ILogicEventSender)
		{
			ILogicEventSender item3 = (ILogicEventSender)item;
			senders.Add(item3);
		}
	}

	public override void RemoveItem(int cell, object item)
	{
		if (item is LogicWire)
		{
			LogicWire logicWire = (LogicWire)item;
			wireGroups[(int)logicWire.MaxBitDepth].Remove(logicWire);
		}
		else if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver item2 = item as ILogicEventReceiver;
			receivers.Remove(item2);
		}
		else if (item is ILogicEventSender)
		{
			ILogicEventSender item3 = (ILogicEventSender)item;
			senders.Remove(item3);
		}
	}

	public override void ConnectItem(int cell, object item)
	{
		if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = (ILogicEventReceiver)item;
			logicEventReceiver.OnLogicNetworkConnectionChanged(connected: true);
		}
		else if (item is ILogicEventSender)
		{
			ILogicEventSender logicEventSender = (ILogicEventSender)item;
			logicEventSender.OnLogicNetworkConnectionChanged(connected: true);
		}
	}

	public override void DisconnectItem(int cell, object item)
	{
		if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = item as ILogicEventReceiver;
			logicEventReceiver.ReceiveLogicEvent(0);
			logicEventReceiver.OnLogicNetworkConnectionChanged(connected: false);
		}
		else if (item is ILogicEventSender)
		{
			ILogicEventSender logicEventSender = item as ILogicEventSender;
			logicEventSender.OnLogicNetworkConnectionChanged(connected: false);
		}
	}

	public override void Reset(UtilityNetworkGridNode[] grid)
	{
		resetting = true;
		previousValue = -1;
		outputValue = 0;
		for (int i = 0; i < 2; i++)
		{
			List<LogicWire> list = wireGroups[i];
			if (list == null)
			{
				continue;
			}
			for (int j = 0; j < list.Count; j++)
			{
				LogicWire logicWire = list[j];
				if (logicWire != null)
				{
					int num = Grid.PosToCell(logicWire.transform.GetPosition());
					UtilityNetworkGridNode utilityNetworkGridNode = grid[num];
					utilityNetworkGridNode.networkIdx = -1;
					grid[num] = utilityNetworkGridNode;
				}
			}
			list.Clear();
		}
		senders.Clear();
		receivers.Clear();
		resetting = false;
		RemoveOverloadedNotification();
	}

	public void UpdateLogicValue()
	{
		if (resetting)
		{
			return;
		}
		previousValue = outputValue;
		outputValue = 0;
		foreach (ILogicEventSender sender in senders)
		{
			sender.LogicTick();
		}
		foreach (ILogicEventSender sender2 in senders)
		{
			int logicValue = sender2.GetLogicValue();
			outputValue |= logicValue;
		}
	}

	public int GetBitsUsed()
	{
		int num = 0;
		if (outputValue > 1)
		{
			return 4;
		}
		return 1;
	}

	public bool IsBitActive(int bit)
	{
		return (OutputValue & (1 << bit)) > 0;
	}

	public static bool IsBitActive(int bit, int value)
	{
		return (value & (1 << bit)) > 0;
	}

	public static int GetBitValue(int bit, int value)
	{
		return value & (1 << bit);
	}

	public void SendLogicEvents(bool force_send, int id)
	{
		if (resetting || !(outputValue != previousValue || force_send))
		{
			return;
		}
		foreach (ILogicEventReceiver receiver in receivers)
		{
			receiver.ReceiveLogicEvent(outputValue);
		}
		if (!force_send)
		{
			TriggerAudio((previousValue >= 0) ? previousValue : 0, id);
		}
	}

	private void TriggerAudio(int old_value, int id)
	{
		SpeedControlScreen instance = SpeedControlScreen.Instance;
		if (old_value == outputValue || !(instance != null) || instance.IsPaused)
		{
			return;
		}
		int num = 0;
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		List<LogicWire> list = new List<LogicWire>();
		for (int i = 0; i < 2; i++)
		{
			List<LogicWire> list2 = wireGroups[i];
			if (list2 == null)
			{
				continue;
			}
			for (int j = 0; j < list2.Count; j++)
			{
				num++;
				if (visibleArea.Min <= list2[j].transform.GetPosition() && list2[j].transform.GetPosition() <= visibleArea.Max)
				{
					list.Add(list2[j]);
				}
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		int index = Mathf.CeilToInt(list.Count / 2);
		if (list[index] != null)
		{
			Vector3 position = list[index].transform.GetPosition();
			position.z = 0f;
			string name = "Logic_Circuit_Toggle";
			LogicSoundPair logicSoundPair = new LogicSoundPair();
			if (!logicSoundRegister.ContainsKey(id))
			{
				logicSoundRegister.Add(id, logicSoundPair);
			}
			else
			{
				logicSoundPair.playedIndex = logicSoundRegister[id].playedIndex;
				logicSoundPair.lastPlayed = logicSoundRegister[id].lastPlayed;
			}
			if (logicSoundPair.playedIndex < 2)
			{
				logicSoundRegister[id].playedIndex = logicSoundPair.playedIndex + 1;
			}
			else
			{
				logicSoundRegister[id].playedIndex = 0;
				logicSoundRegister[id].lastPlayed = Time.time;
			}
			float value = (Time.time - logicSoundPair.lastPlayed) / 3f;
			EventInstance instance2 = KFMOD.BeginOneShot(GlobalAssets.GetSound(name), position);
			instance2.setParameterByName("logic_volumeModifer", value);
			instance2.setParameterByName("wireCount", num % 24);
			instance2.setParameterByName("enabled", outputValue);
			KFMOD.EndOneShot(instance2);
		}
	}

	public void UpdateOverloadTime(float dt, int bits_used)
	{
		bool flag = false;
		List<LogicWire> list = null;
		List<LogicUtilityNetworkLink> list2 = null;
		for (int i = 0; i < 2; i++)
		{
			List<LogicWire> list3 = wireGroups[i];
			List<LogicUtilityNetworkLink> list4 = relevantBridges[i];
			LogicWire.BitDepth rating = (LogicWire.BitDepth)i;
			float num = LogicWire.GetBitDepthAsInt(rating);
			if ((float)bits_used > num && ((list4 != null && list4.Count > 0) || (list3 != null && list3.Count > 0)))
			{
				flag = true;
				list = list3;
				list2 = list4;
				break;
			}
		}
		list?.RemoveAll((LogicWire x) => x == null);
		list2?.RemoveAll((LogicUtilityNetworkLink x) => x == null);
		if (flag)
		{
			timeOverloaded += dt;
			if (!(timeOverloaded > 6f))
			{
				return;
			}
			timeOverloaded = 0f;
			if (targetOverloadedWire == null)
			{
				if (list2 != null && list2.Count > 0)
				{
					int index = Random.Range(0, list2.Count);
					targetOverloadedWire = list2[index].gameObject;
				}
				else if (list != null && list.Count > 0)
				{
					int index2 = Random.Range(0, list.Count);
					targetOverloadedWire = list[index2].gameObject;
				}
			}
			if (targetOverloadedWire != null)
			{
				targetOverloadedWire.Trigger(-794517298, new BuildingHP.DamageSourceInfo
				{
					damage = 1,
					source = BUILDINGS.DAMAGESOURCES.LOGIC_CIRCUIT_OVERLOADED,
					popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.LOGIC_CIRCUIT_OVERLOADED,
					takeDamageEffect = SpawnFXHashes.BuildingLogicOverload,
					fullDamageEffectName = "logic_ribbon_damage_kanim",
					statusItemID = Db.Get().BuildingStatusItems.LogicOverloaded.Id
				});
			}
			if (overloadedNotification == null)
			{
				timeOverloadNotificationDisplayed = 0f;
				overloadedNotification = new Notification(MISC.NOTIFICATIONS.LOGIC_CIRCUIT_OVERLOADED.NAME, NotificationType.BadMinor, null, null, expires: true, 0f, null, null, targetOverloadedWire.transform);
				Notifier notifier = Game.Instance.FindOrAdd<Notifier>();
				notifier.Add(overloadedNotification);
			}
		}
		else
		{
			timeOverloaded = Mathf.Max(0f, timeOverloaded - dt * 0.95f);
			timeOverloadNotificationDisplayed += dt;
			if (timeOverloadNotificationDisplayed > 5f)
			{
				RemoveOverloadedNotification();
			}
		}
	}

	private void RemoveOverloadedNotification()
	{
		if (overloadedNotification != null)
		{
			Notifier notifier = Game.Instance.FindOrAdd<Notifier>();
			notifier.Remove(overloadedNotification);
			overloadedNotification = null;
		}
	}

	public void UpdateRelevantBridges(List<LogicUtilityNetworkLink>[] bridgeGroups)
	{
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		for (int i = 0; i < bridgeGroups.Length; i++)
		{
			if (relevantBridges[i] != null)
			{
				relevantBridges[i].Clear();
			}
			for (int j = 0; j < bridgeGroups[i].Count; j++)
			{
				if (logicCircuitManager.GetNetworkForCell(bridgeGroups[i][j].cell_one) == this || logicCircuitManager.GetNetworkForCell(bridgeGroups[i][j].cell_two) == this)
				{
					if (relevantBridges[i] == null)
					{
						relevantBridges[i] = new List<LogicUtilityNetworkLink>();
					}
					relevantBridges[i].Add(bridgeGroups[i][j]);
				}
			}
		}
	}
}
