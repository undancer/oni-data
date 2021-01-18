using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ElectricalUtilityNetwork : UtilityNetwork
{
	private Notification overloadedNotification;

	private List<Wire>[] wireGroups = new List<Wire>[5];

	public List<Wire> allWires = new List<Wire>();

	private const float MIN_OVERLOAD_TIME_FOR_DAMAGE = 6f;

	private const float MIN_OVERLOAD_NOTIFICATION_DISPLAY_TIME = 5f;

	private GameObject targetOverloadedWire = null;

	private float timeOverloaded = 0f;

	private float timeOverloadNotificationDisplayed = 0f;

	public override void AddItem(int cell, object item)
	{
		Wire wire = (Wire)item;
		Wire.WattageRating maxWattageRating = wire.MaxWattageRating;
		List<Wire> list = wireGroups[(int)maxWattageRating];
		if (list == null)
		{
			list = new List<Wire>();
			wireGroups[(int)maxWattageRating] = list;
		}
		list.Add(wire);
		allWires.Add(wire);
		timeOverloaded = Mathf.Max(timeOverloaded, wire.circuitOverloadTime);
	}

	public override void Reset(UtilityNetworkGridNode[] grid)
	{
		for (int i = 0; i < 5; i++)
		{
			List<Wire> list = wireGroups[i];
			if (list == null)
			{
				continue;
			}
			for (int j = 0; j < list.Count; j++)
			{
				Wire wire = list[j];
				if (wire != null)
				{
					wire.circuitOverloadTime = timeOverloaded;
					int num = Grid.PosToCell(wire.transform.GetPosition());
					UtilityNetworkGridNode utilityNetworkGridNode = grid[num];
					utilityNetworkGridNode.networkIdx = -1;
					grid[num] = utilityNetworkGridNode;
				}
			}
			list.Clear();
		}
		allWires.Clear();
		RemoveOverloadedNotification();
	}

	public void UpdateOverloadTime(float dt, float watts_used, List<WireUtilityNetworkLink>[] bridgeGroups)
	{
		bool flag = false;
		List<Wire> list = null;
		List<WireUtilityNetworkLink> list2 = null;
		for (int i = 0; i < 5; i++)
		{
			List<Wire> list3 = wireGroups[i];
			List<WireUtilityNetworkLink> list4 = bridgeGroups[i];
			Wire.WattageRating rating = (Wire.WattageRating)i;
			float maxWattageAsFloat = Wire.GetMaxWattageAsFloat(rating);
			maxWattageAsFloat += POWER.FLOAT_FUDGE_FACTOR;
			if (watts_used > maxWattageAsFloat && ((list4 != null && list4.Count > 0) || (list3 != null && list3.Count > 0)))
			{
				flag = true;
				list = list3;
				list2 = list4;
				break;
			}
		}
		list?.RemoveAll((Wire x) => x == null);
		list2?.RemoveAll((WireUtilityNetworkLink x) => x == null);
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
					source = STRINGS.BUILDINGS.DAMAGESOURCES.CIRCUIT_OVERLOADED,
					popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CIRCUIT_OVERLOADED,
					takeDamageEffect = SpawnFXHashes.BuildingSpark,
					fullDamageEffectName = "spark_damage_kanim",
					statusItemID = Db.Get().BuildingStatusItems.Overloaded.Id
				});
			}
			if (overloadedNotification == null)
			{
				timeOverloadNotificationDisplayed = 0f;
				overloadedNotification = new Notification(MISC.NOTIFICATIONS.CIRCUIT_OVERLOADED.NAME, NotificationType.BadMinor, null, null, expires: true, 0f, null, null, targetOverloadedWire.transform);
				GameScheduler.Instance.Schedule("Power Tutorial", 2f, delegate
				{
					Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Power);
				});
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

	public float GetMaxSafeWattage()
	{
		for (int i = 0; i < wireGroups.Length; i++)
		{
			List<Wire> list = wireGroups[i];
			if (list != null && list.Count > 0)
			{
				Wire.WattageRating rating = (Wire.WattageRating)i;
				return Wire.GetMaxWattageAsFloat(rating);
			}
		}
		return 0f;
	}

	public override void RemoveItem(int cell, object item)
	{
		if (item.GetType() == typeof(Wire))
		{
			Wire wire = (Wire)item;
			wire.circuitOverloadTime = 0f;
			allWires.Remove(wire);
		}
	}
}
