using System;
using System.Collections.Generic;
using Database;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/SpacecraftManager")]
public class SpacecraftManager : KMonoBehaviour, ISim1000ms
{
	public enum DestinationAnalysisState
	{
		Hidden,
		Discovered,
		Complete
	}

	public static SpacecraftManager instance;

	[Serialize]
	private List<Spacecraft> spacecraft = new List<Spacecraft>();

	[Serialize]
	private int nextSpacecraftID;

	public const int INVALID_DESTINATION_ID = -1;

	[Serialize]
	private int analyzeDestinationID = -1;

	[Serialize]
	public bool hasVisitedWormHole;

	[Serialize]
	public List<SpaceDestination> destinations;

	[Serialize]
	public Dictionary<int, int> savedSpacecraftDestinations;

	[Serialize]
	public bool destinationsGenerated;

	[Serialize]
	public Dictionary<int, float> destinationAnalysisScores = new Dictionary<int, float>();

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
		if (savedSpacecraftDestinations == null)
		{
			savedSpacecraftDestinations = new Dictionary<int, int>();
		}
	}

	private void GenerateFixedDestinations()
	{
		SpaceDestinationTypes spaceDestinationTypes = Db.Get().SpaceDestinationTypes;
		if (destinations == null)
		{
			destinations = new List<SpaceDestination>
			{
				new SpaceDestination(0, spaceDestinationTypes.CarbonaceousAsteroid.Id, 0),
				new SpaceDestination(1, spaceDestinationTypes.CarbonaceousAsteroid.Id, 0),
				new SpaceDestination(2, spaceDestinationTypes.MetallicAsteroid.Id, 1),
				new SpaceDestination(3, spaceDestinationTypes.RockyAsteroid.Id, 2),
				new SpaceDestination(4, spaceDestinationTypes.IcyDwarf.Id, 3),
				new SpaceDestination(5, spaceDestinationTypes.OrganicDwarf.Id, 4)
			};
		}
	}

	private void GenerateRandomDestinations()
	{
		System.Random random = new System.Random(SaveLoader.Instance.clusterDetailSave.globalWorldSeed);
		SpaceDestinationTypes spaceDestinationTypes = Db.Get().SpaceDestinationTypes;
		List<List<string>> list = new List<List<string>>
		{
			new List<string>(),
			new List<string> { spaceDestinationTypes.OilyAsteroid.Id },
			new List<string> { spaceDestinationTypes.Satellite.Id },
			new List<string>
			{
				spaceDestinationTypes.Satellite.Id,
				spaceDestinationTypes.RockyAsteroid.Id,
				spaceDestinationTypes.CarbonaceousAsteroid.Id,
				spaceDestinationTypes.ForestPlanet.Id
			},
			new List<string>
			{
				spaceDestinationTypes.MetallicAsteroid.Id,
				spaceDestinationTypes.RockyAsteroid.Id,
				spaceDestinationTypes.CarbonaceousAsteroid.Id,
				spaceDestinationTypes.SaltDwarf.Id
			},
			new List<string>
			{
				spaceDestinationTypes.MetallicAsteroid.Id,
				spaceDestinationTypes.RockyAsteroid.Id,
				spaceDestinationTypes.CarbonaceousAsteroid.Id,
				spaceDestinationTypes.IcyDwarf.Id,
				spaceDestinationTypes.OrganicDwarf.Id
			},
			new List<string>
			{
				spaceDestinationTypes.IcyDwarf.Id,
				spaceDestinationTypes.OrganicDwarf.Id,
				spaceDestinationTypes.DustyMoon.Id,
				spaceDestinationTypes.ChlorinePlanet.Id,
				spaceDestinationTypes.RedDwarf.Id
			},
			new List<string>
			{
				spaceDestinationTypes.DustyMoon.Id,
				spaceDestinationTypes.TerraPlanet.Id,
				spaceDestinationTypes.VolcanoPlanet.Id
			},
			new List<string>
			{
				spaceDestinationTypes.TerraPlanet.Id,
				spaceDestinationTypes.GasGiant.Id,
				spaceDestinationTypes.IceGiant.Id,
				spaceDestinationTypes.RustPlanet.Id
			},
			new List<string>
			{
				spaceDestinationTypes.GasGiant.Id,
				spaceDestinationTypes.IceGiant.Id,
				spaceDestinationTypes.HydrogenGiant.Id
			},
			new List<string>
			{
				spaceDestinationTypes.RustPlanet.Id,
				spaceDestinationTypes.VolcanoPlanet.Id,
				spaceDestinationTypes.RockyAsteroid.Id,
				spaceDestinationTypes.TerraPlanet.Id,
				spaceDestinationTypes.MetallicAsteroid.Id
			},
			new List<string>
			{
				spaceDestinationTypes.ShinyPlanet.Id,
				spaceDestinationTypes.MetallicAsteroid.Id,
				spaceDestinationTypes.RockyAsteroid.Id
			},
			new List<string>
			{
				spaceDestinationTypes.GoldAsteroid.Id,
				spaceDestinationTypes.OrganicDwarf.Id,
				spaceDestinationTypes.ForestPlanet.Id,
				spaceDestinationTypes.ChlorinePlanet.Id
			},
			new List<string>
			{
				spaceDestinationTypes.IcyDwarf.Id,
				spaceDestinationTypes.MetallicAsteroid.Id,
				spaceDestinationTypes.DustyMoon.Id,
				spaceDestinationTypes.VolcanoPlanet.Id,
				spaceDestinationTypes.IceGiant.Id
			},
			new List<string>
			{
				spaceDestinationTypes.ShinyPlanet.Id,
				spaceDestinationTypes.RedDwarf.Id,
				spaceDestinationTypes.RockyAsteroid.Id,
				spaceDestinationTypes.GasGiant.Id
			},
			new List<string>
			{
				spaceDestinationTypes.HydrogenGiant.Id,
				spaceDestinationTypes.ForestPlanet.Id,
				spaceDestinationTypes.OilyAsteroid.Id
			},
			new List<string>
			{
				spaceDestinationTypes.GoldAsteroid.Id,
				spaceDestinationTypes.SaltDwarf.Id,
				spaceDestinationTypes.TerraPlanet.Id,
				spaceDestinationTypes.VolcanoPlanet.Id
			}
		};
		List<int> list2 = new List<int>();
		int num = 3;
		int minValue = 15;
		int maxValue = 25;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Count != 0)
			{
				for (int j = 0; j < num; j++)
				{
					list2.Add(i);
				}
			}
		}
		int num2 = random.Next(minValue, maxValue);
		for (int k = 0; k < num2; k++)
		{
			int index = random.Next(0, list2.Count - 1);
			int num3 = list2[index];
			list2.RemoveAt(index);
			List<string> list3 = list[num3];
			string type = list3[random.Next(0, list3.Count)];
			SpaceDestination item = new SpaceDestination(destinations.Count, type, num3);
			destinations.Add(item);
		}
		destinations.Add(new SpaceDestination(destinations.Count, Db.Get().SpaceDestinationTypes.Earth.Id, 4));
		destinations.Add(new SpaceDestination(destinations.Count, Db.Get().SpaceDestinationTypes.Wormhole.Id, list.Count));
	}

	private void RestoreDestinations()
	{
		if (destinationsGenerated)
		{
			return;
		}
		GenerateFixedDestinations();
		GenerateRandomDestinations();
		destinations.Sort((SpaceDestination a, SpaceDestination b) => a.distance.CompareTo(b.distance));
		List<float> list = new List<float>();
		for (int i = 0; i < 10; i++)
		{
			list.Add((float)i / 10f);
		}
		for (int j = 0; j < 20; j++)
		{
			list.Shuffle();
			int num = 0;
			foreach (SpaceDestination destination in destinations)
			{
				if (destination.distance == j)
				{
					num++;
					destination.startingOrbitPercentage = list[num];
				}
			}
		}
		destinationsGenerated = true;
	}

	public SpaceDestination GetSpacecraftDestination(LaunchConditionManager lcm)
	{
		Spacecraft spacecraftFromLaunchConditionManager = GetSpacecraftFromLaunchConditionManager(lcm);
		return GetSpacecraftDestination(spacecraftFromLaunchConditionManager.id);
	}

	public SpaceDestination GetSpacecraftDestination(int spacecraftID)
	{
		CleanSavedSpacecraftDestinations();
		if (savedSpacecraftDestinations.ContainsKey(spacecraftID))
		{
			return GetDestination(savedSpacecraftDestinations[spacecraftID]);
		}
		return null;
	}

	public List<int> GetSpacecraftsForDestination(SpaceDestination destination)
	{
		CleanSavedSpacecraftDestinations();
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, int> savedSpacecraftDestination in savedSpacecraftDestinations)
		{
			if (savedSpacecraftDestination.Value == destination.id)
			{
				list.Add(savedSpacecraftDestination.Key);
			}
		}
		return list;
	}

	private void CleanSavedSpacecraftDestinations()
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, int> savedSpacecraftDestination in savedSpacecraftDestinations)
		{
			bool flag = false;
			foreach (Spacecraft item in spacecraft)
			{
				if (item.id == savedSpacecraftDestination.Key)
				{
					flag = true;
					break;
				}
			}
			bool flag2 = false;
			foreach (SpaceDestination destination in destinations)
			{
				if (destination.id == savedSpacecraftDestination.Value)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag || !flag2)
			{
				list.Add(savedSpacecraftDestination.Key);
			}
		}
		foreach (int item2 in list)
		{
			savedSpacecraftDestinations.Remove(item2);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.spacecraftManager = this;
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			Debug.Assert(spacecraft == null || spacecraft.Count == 0);
		}
		else
		{
			RestoreDestinations();
		}
	}

	public void SetSpacecraftDestination(LaunchConditionManager lcm, SpaceDestination destination)
	{
		Spacecraft spacecraftFromLaunchConditionManager = GetSpacecraftFromLaunchConditionManager(lcm);
		savedSpacecraftDestinations[spacecraftFromLaunchConditionManager.id] = destination.id;
	}

	public int GetSpacecraftID(ILaunchableRocket rocket)
	{
		foreach (Spacecraft item in spacecraft)
		{
			if (item.launchConditions.gameObject == rocket.LaunchableGameObject)
			{
				return item.id;
			}
		}
		return -1;
	}

	public SpaceDestination GetDestination(int destinationID)
	{
		foreach (SpaceDestination destination in destinations)
		{
			if (destination.id == destinationID)
			{
				return destination;
			}
		}
		Debug.LogErrorFormat("No space destination with ID {0}", destinationID);
		return null;
	}

	public void RegisterSpacecraft(Spacecraft craft)
	{
		if (!spacecraft.Contains(craft))
		{
			if (craft.HasInvalidID())
			{
				craft.SetID(nextSpacecraftID);
				nextSpacecraftID++;
			}
			spacecraft.Add(craft);
		}
	}

	public void UnregisterSpacecraft(LaunchConditionManager conditionManager)
	{
		Spacecraft spacecraftFromLaunchConditionManager = GetSpacecraftFromLaunchConditionManager(conditionManager);
		spacecraftFromLaunchConditionManager.SetState(Spacecraft.MissionState.Destroyed);
		spacecraft.Remove(spacecraftFromLaunchConditionManager);
	}

	public List<Spacecraft> GetSpacecraft()
	{
		return spacecraft;
	}

	public Spacecraft GetSpacecraftFromLaunchConditionManager(LaunchConditionManager lcm)
	{
		foreach (Spacecraft item in spacecraft)
		{
			if (item.launchConditions == lcm)
			{
				return item;
			}
		}
		return null;
	}

	public void Sim1000ms(float dt)
	{
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			return;
		}
		foreach (Spacecraft item in spacecraft)
		{
			item.ProgressMission(dt);
		}
		foreach (SpaceDestination destination in destinations)
		{
			destination.Replenish(dt);
		}
	}

	public void PushReadyToLandNotification(Spacecraft spacecraft)
	{
		Notification notification = new Notification(BUILDING.STATUSITEMS.SPACECRAFTREADYTOLAND.NOTIFICATION, NotificationType.Good, delegate(List<Notification> notificationList, object data)
		{
			string text = BUILDING.STATUSITEMS.SPACECRAFTREADYTOLAND.NOTIFICATION_TOOLTIP;
			foreach (Notification notification2 in notificationList)
			{
				text = text + "\n" + (string)notification2.tooltipData;
			}
			return text;
		}, "• " + spacecraft.rocketName);
		spacecraft.launchConditions.gameObject.AddOrGet<Notifier>().Add(notification);
	}

	private void SpawnMissionResults(Dictionary<SimHashes, float> results)
	{
		foreach (KeyValuePair<SimHashes, float> result in results)
		{
			ElementLoader.FindElementByHash(result.Key).substance.SpawnResource(PlayerController.GetCursorPos(KInputManager.GetMousePos()), result.Value, 300f, 0, 0);
		}
	}

	public float GetDestinationAnalysisScore(SpaceDestination destination)
	{
		return GetDestinationAnalysisScore(destination.id);
	}

	public float GetDestinationAnalysisScore(int destinationID)
	{
		if (destinationAnalysisScores.ContainsKey(destinationID))
		{
			return destinationAnalysisScores[destinationID];
		}
		return 0f;
	}

	public void EarnDestinationAnalysisPoints(int destinationID, float points)
	{
		if (!destinationAnalysisScores.ContainsKey(destinationID))
		{
			destinationAnalysisScores.Add(destinationID, 0f);
		}
		SpaceDestination destination = GetDestination(destinationID);
		DestinationAnalysisState destinationAnalysisState = GetDestinationAnalysisState(destination);
		destinationAnalysisScores[destinationID] += points;
		DestinationAnalysisState destinationAnalysisState2 = GetDestinationAnalysisState(destination);
		if (destinationAnalysisState == destinationAnalysisState2)
		{
			return;
		}
		int starmapAnalysisDestinationID = instance.GetStarmapAnalysisDestinationID();
		if (starmapAnalysisDestinationID != destinationID)
		{
			return;
		}
		if (destinationAnalysisState2 == DestinationAnalysisState.Complete)
		{
			if (instance.GetDestination(starmapAnalysisDestinationID).type == Db.Get().SpaceDestinationTypes.Earth.Id)
			{
				Game.Instance.unlocks.Unlock("earth");
			}
			if (instance.GetDestination(starmapAnalysisDestinationID).type == Db.Get().SpaceDestinationTypes.Wormhole.Id)
			{
				Game.Instance.unlocks.Unlock("wormhole");
			}
			instance.SetStarmapAnalysisDestinationID(-1);
		}
		Trigger(532901469);
	}

	public DestinationAnalysisState GetDestinationAnalysisState(SpaceDestination destination)
	{
		if (destination.startAnalyzed)
		{
			return DestinationAnalysisState.Complete;
		}
		float destinationAnalysisScore = GetDestinationAnalysisScore(destination);
		if (destinationAnalysisScore >= (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE)
		{
			return DestinationAnalysisState.Complete;
		}
		if (destinationAnalysisScore >= (float)ROCKETRY.DESTINATION_ANALYSIS.DISCOVERED)
		{
			return DestinationAnalysisState.Discovered;
		}
		return DestinationAnalysisState.Hidden;
	}

	public bool AreAllDestinationsAnalyzed()
	{
		foreach (SpaceDestination destination in destinations)
		{
			if (GetDestinationAnalysisState(destination) != DestinationAnalysisState.Complete)
			{
				return false;
			}
		}
		return true;
	}

	public void SetStarmapAnalysisDestinationID(int id)
	{
		analyzeDestinationID = id;
		Trigger(532901469, id);
	}

	public int GetStarmapAnalysisDestinationID()
	{
		return analyzeDestinationID;
	}

	public bool HasAnalysisTarget()
	{
		return analyzeDestinationID != -1;
	}
}
