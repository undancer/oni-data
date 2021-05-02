using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class ColonyDiagnosticUtility : KMonoBehaviour, ISim4000ms
{
	public enum DisplaySetting
	{
		Always,
		AlertOnly,
		Never,
		LENGTH
	}

	public static ColonyDiagnosticUtility Instance;

	private Dictionary<int, List<ColonyDiagnostic>> worldDiagnostics = new Dictionary<int, List<ColonyDiagnostic>>();

	[Serialize]
	public Dictionary<int, Dictionary<string, DisplaySetting>> diagnosticDisplaySettings = new Dictionary<int, Dictionary<string, DisplaySetting>>();

	[Serialize]
	private Dictionary<string, float> diagnosticTutorialStatus = new Dictionary<string, float>
	{
		{
			typeof(ToiletDiagnostic).Name,
			450f
		},
		{
			typeof(BedDiagnostic).Name,
			900f
		},
		{
			typeof(BreathabilityDiagnostic).Name,
			1800f
		},
		{
			typeof(FoodDiagnostic).Name,
			3000f
		},
		{
			typeof(FarmDiagnostic).Name,
			6000f
		},
		{
			typeof(StressDiagnostic).Name,
			9000f
		},
		{
			typeof(PowerUseDiagnostic).Name,
			12000f
		},
		{
			typeof(BatteryDiagnostic).Name,
			12000f
		}
	};

	private bool ignoreFirstUpdate = true;

	public ColonyDiagnostic.DiagnosticResult.Opinion GetWorldDiagnosticResult(int worldID)
	{
		ColonyDiagnostic.DiagnosticResult.Opinion opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Good;
		foreach (ColonyDiagnostic item in worldDiagnostics[worldID])
		{
			DisplaySetting displaySetting = Instance.diagnosticDisplaySettings[worldID][item.id];
			if (displaySetting != DisplaySetting.Never && !Instance.IsDiagnosticTutorialDisabled(item.id))
			{
				switch (diagnosticDisplaySettings[worldID][item.id])
				{
				case DisplaySetting.Always:
				case DisplaySetting.AlertOnly:
				{
					int num = Math.Min((int)opinion, (int)item.LatestResult.opinion);
					opinion = (ColonyDiagnostic.DiagnosticResult.Opinion)num;
					break;
				}
				}
			}
		}
		return opinion;
	}

	public string GetWorldDiagnosticResultTooltip(int worldID)
	{
		string text = "";
		foreach (ColonyDiagnostic item in worldDiagnostics[worldID])
		{
			DisplaySetting displaySetting = Instance.diagnosticDisplaySettings[worldID][item.id];
			if (displaySetting == DisplaySetting.Never || Instance.IsDiagnosticTutorialDisabled(item.id))
			{
				continue;
			}
			switch (diagnosticDisplaySettings[worldID][item.id])
			{
			case DisplaySetting.Always:
			case DisplaySetting.AlertOnly:
				if (item.LatestResult.opinion < ColonyDiagnostic.DiagnosticResult.Opinion.Normal)
				{
					text = text + "\n" + item.LatestResult.Message;
				}
				break;
			}
		}
		return text;
	}

	public bool IsDiagnosticTutorialDisabled(string id)
	{
		if (Instance.diagnosticTutorialStatus.ContainsKey(id) && GameClock.Instance.GetTime() < Instance.diagnosticTutorialStatus[id])
		{
			return true;
		}
		return false;
	}

	public void ClearDiagnosticTutorialSetting(string id)
	{
		if (Instance.diagnosticTutorialStatus.ContainsKey(id))
		{
			Instance.diagnosticTutorialStatus[id] = -1f;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (int item in ClusterManager.Instance.GetWorldIDsSorted())
		{
			AddWorld(item);
		}
		ClusterManager.Instance.Subscribe(-1280433810, Refresh);
		ClusterManager.Instance.Subscribe(-1078710002, RemoveWorld);
	}

	private void Refresh(object data)
	{
		int worldID = (int)data;
		AddWorld(worldID);
	}

	private void RemoveWorld(object data)
	{
		int key = (int)data;
		if (diagnosticDisplaySettings.Remove(key))
		{
			worldDiagnostics.Remove(key);
		}
	}

	public ColonyDiagnostic GetDiagnostic(string id, int worldID)
	{
		return worldDiagnostics[worldID].Find((ColonyDiagnostic match) => match.id == id);
	}

	public T GetDiagnostic<T>(int worldID) where T : ColonyDiagnostic
	{
		return (T)worldDiagnostics[worldID].Find((ColonyDiagnostic match) => match is T);
	}

	public string GetDiagnosticName(string id)
	{
		foreach (KeyValuePair<int, List<ColonyDiagnostic>> worldDiagnostic in worldDiagnostics)
		{
			foreach (ColonyDiagnostic item in worldDiagnostic.Value)
			{
				if (item.id == id)
				{
					return item.name;
				}
			}
		}
		Debug.LogWarning("Cannot locate name of diagnostic " + id + " because no worlds have a diagnostic with that id ");
		return "";
	}

	public ChoreGroupDiagnostic GetChoreGroupDiagnostic(int worldID, ChoreGroup choreGroup)
	{
		return (ChoreGroupDiagnostic)worldDiagnostics[worldID].Find((ColonyDiagnostic match) => match is ChoreGroupDiagnostic && ((ChoreGroupDiagnostic)match).choreGroup == choreGroup);
	}

	public WorkTimeDiagnostic GetWorkTimeDiagnostic(int worldID, ChoreGroup choreGroup)
	{
		return (WorkTimeDiagnostic)worldDiagnostics[worldID].Find((ColonyDiagnostic match) => match is WorkTimeDiagnostic && ((WorkTimeDiagnostic)match).choreGroup == choreGroup);
	}

	public void AddWorld(int worldID)
	{
		bool flag = false;
		if (!diagnosticDisplaySettings.ContainsKey(worldID))
		{
			diagnosticDisplaySettings.Add(worldID, new Dictionary<string, DisplaySetting>());
			flag = true;
		}
		List<ColonyDiagnostic> list = new List<ColonyDiagnostic>();
		list.Add(new BreathabilityDiagnostic(worldID));
		list.Add(new FoodDiagnostic(worldID));
		list.Add(new StressDiagnostic(worldID));
		list.Add(new RadiationDiagnostic(worldID));
		list.Add(new ReactorDiagnostic(worldID));
		if (ClusterManager.Instance.GetWorld(worldID).IsModuleInterior)
		{
			list.Add(new FloatingRocketDiagnostic(worldID));
			list.Add(new RocketFuelDiagnostic(worldID));
			list.Add(new RocketOxidizerDiagnostic(worldID));
		}
		else
		{
			list.Add(new BedDiagnostic(worldID));
			list.Add(new ToiletDiagnostic(worldID));
			list.Add(new PowerUseDiagnostic(worldID));
			list.Add(new BatteryDiagnostic(worldID));
			list.Add(new IdleDiagnostic(worldID));
			list.Add(new TrappedDuplicantDiagnostic(worldID));
			list.Add(new FarmDiagnostic(worldID));
			list.Add(new EntombedDiagnostic(worldID));
			for (int i = 0; i < Db.Get().ChoreGroups.Count; i++)
			{
				list.Add(new ChoreGroupDiagnostic(worldID, Db.Get().ChoreGroups[i]));
				list.Add(new WorkTimeDiagnostic(worldID, Db.Get().ChoreGroups[i]));
			}
			list.Add(new AllChoresDiagnostic(worldID));
			list.Add(new AllWorkTimeDiagnostic(worldID));
		}
		worldDiagnostics.Add(worldID, list);
		foreach (ColonyDiagnostic item in list)
		{
			if (!diagnosticDisplaySettings[worldID].ContainsKey(item.id))
			{
				diagnosticDisplaySettings[worldID].Add(item.id, DisplaySetting.AlertOnly);
			}
		}
		if (flag)
		{
			diagnosticDisplaySettings[worldID][typeof(BreathabilityDiagnostic).Name] = DisplaySetting.Always;
			diagnosticDisplaySettings[worldID][typeof(FoodDiagnostic).Name] = DisplaySetting.Always;
			diagnosticDisplaySettings[worldID][typeof(StressDiagnostic).Name] = DisplaySetting.Always;
			diagnosticDisplaySettings[worldID][typeof(IdleDiagnostic).Name] = DisplaySetting.Never;
			if (ClusterManager.Instance.GetWorld(worldID).IsModuleInterior)
			{
				diagnosticDisplaySettings[worldID][typeof(FloatingRocketDiagnostic).Name] = DisplaySetting.Always;
				diagnosticDisplaySettings[worldID][typeof(RocketFuelDiagnostic).Name] = DisplaySetting.Always;
				diagnosticDisplaySettings[worldID][typeof(RocketOxidizerDiagnostic).Name] = DisplaySetting.Always;
			}
		}
	}

	public void Sim4000ms(float dt)
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = default(ColonyDiagnostic.DiagnosticResult);
		if (ignoreFirstUpdate)
		{
			diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.NO_DATA;
			diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
			diagnosticResult.clickThroughTarget = null;
		}
		foreach (KeyValuePair<int, List<ColonyDiagnostic>> worldDiagnostic in worldDiagnostics)
		{
			foreach (ColonyDiagnostic item in worldDiagnostic.Value)
			{
				item.SetResult(ignoreFirstUpdate ? diagnosticResult : item.Evaluate());
			}
		}
		if (ignoreFirstUpdate)
		{
			ignoreFirstUpdate = false;
		}
	}

	public static bool PastNewBuildingGracePeriod(Transform building)
	{
		BuildingComplete component = building.GetComponent<BuildingComplete>();
		if (component != null && GameClock.Instance.GetTime() - component.creationTime < 600f)
		{
			return false;
		}
		return true;
	}

	public static bool IgnoreRocketsWithNoCrewRequested(int worldID, out ColonyDiagnostic.DiagnosticResult result)
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(worldID);
		result = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.NO_MINIONS);
		if (world.IsModuleInterior)
		{
			for (int i = 0; i < Components.Clustercrafts.Count; i++)
			{
				WorldContainer interiorWorld = Components.Clustercrafts[i].ModuleInterface.GetInteriorWorld();
				if (!(interiorWorld == null) && interiorWorld.id == worldID)
				{
					PassengerRocketModule passengerModule = Components.Clustercrafts[i].ModuleInterface.GetPassengerModule();
					if (passengerModule != null && !passengerModule.ShouldCrewGetIn())
					{
						result = default(ColonyDiagnostic.DiagnosticResult);
						result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
						result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS_REQUESTED;
						return true;
					}
				}
			}
		}
		return false;
	}

	public static DiagnosticCriterion GetWorldHasMinionCriterion(int worldID)
	{
		return new DiagnosticCriterion(delegate
		{
			ColonyDiagnostic.DiagnosticResult result = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, "");
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(worldID);
			if (worldItems.Count == 0)
			{
				result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
			}
			return result;
		});
	}
}
