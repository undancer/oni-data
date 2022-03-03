using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GeneratedBuildings
{
	public static void LoadGeneratedBuildings(List<Type> types)
	{
		Type typeFromHandle = typeof(IBuildingConfig);
		List<Type> list = new List<Type>();
		foreach (Type type in types)
		{
			if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
			{
				list.Add(type);
			}
		}
		foreach (Type item in list)
		{
			object obj = Activator.CreateInstance(item);
			try
			{
				BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
			}
			catch (Exception e)
			{
				DebugUtil.LogException(null, "Exception in RegisterBuilding for type " + item.FullName + " from " + item.Assembly.GetName().Name, e);
			}
		}
		foreach (PlanScreen.PlanInfo item2 in BUILDINGS.PLANORDER)
		{
			List<string> list2 = new List<string>();
			foreach (KeyValuePair<string, string> buildingAndSubcategoryDatum in item2.buildingAndSubcategoryData)
			{
				if (Assets.GetBuildingDef(buildingAndSubcategoryDatum.Key) == null)
				{
					list2.Add(buildingAndSubcategoryDatum.Key);
				}
			}
			foreach (string entry2 in list2)
			{
				item2.buildingAndSubcategoryData.RemoveAll((KeyValuePair<string, string> match) => match.Key == entry2);
			}
			List<string> list3 = new List<string>();
			foreach (string entry in item2.data)
			{
				if (item2.buildingAndSubcategoryData.FindIndex((KeyValuePair<string, string> x) => x.Key == entry) == -1 && Assets.GetBuildingDef(entry) != null)
				{
					Debug.LogWarning("Mod: Building '" + entry + "' was not added properly to PlanInfo, use ModUtil.AddBuildingToPlanScreen instead.");
					list3.Add(entry);
				}
			}
			foreach (string item3 in list3)
			{
				ModUtil.AddBuildingToPlanScreen(item2.category, item3, "uncategorized");
			}
		}
	}

	public static void MakeBuildingAlwaysOperational(GameObject go)
	{
		BuildingDef def = go.GetComponent<BuildingComplete>().Def;
		if (def.LogicInputPorts != null || def.LogicOutputPorts != null)
		{
			Debug.LogWarning("Do not call MakeBuildingAlwaysOperational directly if LogicInputPorts or LogicOutputPorts are defined. Instead set BuildingDef.AlwaysOperational = true");
		}
		MakeBuildingAlwaysOperationalImpl(go);
	}

	public static void RemoveLoopingSounds(GameObject go)
	{
		UnityEngine.Object.DestroyImmediate(go.GetComponent<LoopingSounds>());
	}

	public static void RemoveDefaultLogicPorts(GameObject go)
	{
		UnityEngine.Object.DestroyImmediate(go.GetComponent<LogicPorts>());
	}

	public static void RegisterWithOverlay(HashSet<Tag> overlay_tags, string id)
	{
		overlay_tags.Add(new Tag(id));
		overlay_tags.Add(new Tag(id + "UnderConstruction"));
	}

	public static void RegisterSingleLogicInputPort(GameObject go)
	{
		LogicPorts logicPorts = go.AddOrGet<LogicPorts>();
		logicPorts.inputPortInfo = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0)).ToArray();
		logicPorts.outputPortInfo = null;
	}

	private static void MakeBuildingAlwaysOperationalImpl(GameObject go)
	{
		UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		UnityEngine.Object.DestroyImmediate(go.GetComponent<Operational>());
		UnityEngine.Object.DestroyImmediate(go.GetComponent<LogicPorts>());
	}

	public static void InitializeLogicPorts(GameObject go, BuildingDef def)
	{
		if (def.AlwaysOperational)
		{
			MakeBuildingAlwaysOperationalImpl(go);
		}
		if (def.LogicInputPorts != null)
		{
			go.AddOrGet<LogicPorts>().inputPortInfo = def.LogicInputPorts.ToArray();
		}
		if (def.LogicOutputPorts != null)
		{
			go.AddOrGet<LogicPorts>().outputPortInfo = def.LogicOutputPorts.ToArray();
		}
	}

	public static void InitializeHighEnergyParticlePorts(GameObject go, BuildingDef def)
	{
		if (def.UseHighEnergyParticleInputPort || def.UseHighEnergyParticleOutputPort)
		{
			HighEnergyParticlePort highEnergyParticlePort = go.AddOrGet<HighEnergyParticlePort>();
			highEnergyParticlePort.particleInputOffset = def.HighEnergyParticleInputOffset;
			highEnergyParticlePort.particleOutputOffset = def.HighEnergyParticleOutputOffset;
			highEnergyParticlePort.particleInputEnabled = def.UseHighEnergyParticleInputPort;
			highEnergyParticlePort.particleOutputEnabled = def.UseHighEnergyParticleOutputPort;
		}
	}
}
