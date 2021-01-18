using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LaunchConditionManager")]
public class LaunchConditionManager : KMonoBehaviour, ISim4000ms, ISim1000ms
{
	public HashedString triggerPort;

	public HashedString statusPort;

	private LaunchableRocket launchable;

	[Serialize]
	private List<Tuple<string, string, string>> DEBUG_ModuleDestructions;

	private Dictionary<RocketFlightCondition, Guid> conditionStatuses = new Dictionary<RocketFlightCondition, Guid>();

	public List<RocketModule> rocketModules
	{
		get;
		private set;
	}

	public void DEBUG_TraceModuleDestruction(string moduleName, string state, string stackTrace)
	{
		if (DEBUG_ModuleDestructions == null)
		{
			DEBUG_ModuleDestructions = new List<Tuple<string, string, string>>();
		}
		DEBUG_ModuleDestructions.Add(new Tuple<string, string, string>(moduleName, state, stackTrace));
	}

	[ContextMenu("Dump Module Destructions")]
	private void DEBUG_DumpModuleDestructions()
	{
		if (DEBUG_ModuleDestructions == null || DEBUG_ModuleDestructions.Count == 0)
		{
			DebugUtil.LogArgs("Sorry, no logged module destructions. :(");
			return;
		}
		foreach (Tuple<string, string, string> dEBUG_ModuleDestruction in DEBUG_ModuleDestructions)
		{
			DebugUtil.LogArgs(dEBUG_ModuleDestruction.first, ">", dEBUG_ModuleDestruction.second, "\n", dEBUG_ModuleDestruction.third, "\nEND MODULE DUMP\n\n");
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		rocketModules = new List<RocketModule>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		launchable = GetComponent<LaunchableRocket>();
		FindModules();
		GetComponent<AttachableBuilding>().onAttachmentNetworkChanged = delegate
		{
			FindModules();
		};
		Subscribe(-1582839653, OnTagsChanged);
	}

	private void OnTagsChanged(object data)
	{
		foreach (RocketModule rocketModule in rocketModules)
		{
			rocketModule.OnConditionManagerTagsChanged(data);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public void Sim1000ms(float dt)
	{
		Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
		if (spacecraftFromLaunchConditionManager != null)
		{
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(spacecraftFromLaunchConditionManager.id);
			if (base.gameObject.GetComponent<LogicPorts>().GetInputValue(triggerPort) == 1 && spacecraftDestination != null && spacecraftDestination.id != -1)
			{
				Launch(spacecraftDestination);
			}
		}
	}

	public void FindModules()
	{
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			RocketModule component = item.GetComponent<RocketModule>();
			if (component != null && component.conditionManager == null)
			{
				component.conditionManager = this;
				component.RegisterWithConditionManager();
			}
		}
		Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
		if (spacecraftFromLaunchConditionManager != null)
		{
			spacecraftFromLaunchConditionManager.moduleCount = attachedNetwork.Count;
		}
	}

	public void RegisterRocketModule(RocketModule module)
	{
		if (!rocketModules.Contains(module))
		{
			rocketModules.Add(module);
		}
	}

	public void UnregisterRocketModule(RocketModule module)
	{
		rocketModules.Remove(module);
	}

	public List<RocketLaunchCondition> GetLaunchConditionList()
	{
		List<RocketLaunchCondition> list = new List<RocketLaunchCondition>();
		foreach (RocketModule rocketModule in rocketModules)
		{
			foreach (RocketLaunchCondition launchCondition in rocketModule.launchConditions)
			{
				list.Add(launchCondition);
			}
		}
		return list;
	}

	public void Launch(SpaceDestination destination)
	{
		if (destination == null)
		{
			Debug.LogError("Null destination passed to launch");
		}
		if (SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this).state == Spacecraft.MissionState.Grounded && (DebugHandler.InstantBuildMode || (CheckReadyToLaunch() && CheckAbleToFly())))
		{
			launchable.Trigger(-1056989049);
			SpacecraftManager.instance.SetSpacecraftDestination(this, destination);
			SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this).BeginMission(destination);
		}
	}

	public bool CheckReadyToLaunch()
	{
		foreach (RocketModule rocketModule in rocketModules)
		{
			foreach (RocketLaunchCondition launchCondition in rocketModule.launchConditions)
			{
				if (launchCondition.EvaluateLaunchCondition() == RocketLaunchCondition.LaunchStatus.Failure)
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool CheckAbleToFly()
	{
		foreach (RocketModule rocketModule in rocketModules)
		{
			foreach (RocketFlightCondition flightCondition in rocketModule.flightConditions)
			{
				if (!flightCondition.EvaluateFlightCondition())
				{
					return false;
				}
			}
		}
		return true;
	}

	private void ClearFlightStatuses()
	{
		KSelectable component = GetComponent<KSelectable>();
		foreach (KeyValuePair<RocketFlightCondition, Guid> conditionStatus in conditionStatuses)
		{
			component.RemoveStatusItem(conditionStatus.Value);
		}
		conditionStatuses.Clear();
	}

	public void Sim4000ms(float dt)
	{
		bool num = CheckReadyToLaunch();
		LogicPorts component = base.gameObject.GetComponent<LogicPorts>();
		if (num)
		{
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
			if (spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Grounded || spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Launching)
			{
				component.SendSignal(statusPort, 1);
			}
			else
			{
				component.SendSignal(statusPort, 0);
			}
			KSelectable component2 = GetComponent<KSelectable>();
			foreach (RocketModule rocketModule in rocketModules)
			{
				foreach (RocketFlightCondition flightCondition in rocketModule.flightConditions)
				{
					if (!flightCondition.EvaluateFlightCondition())
					{
						if (!conditionStatuses.ContainsKey(flightCondition))
						{
							StatusItem failureStatusItem = flightCondition.GetFailureStatusItem();
							conditionStatuses[flightCondition] = component2.AddStatusItem(failureStatusItem, flightCondition);
						}
					}
					else if (conditionStatuses.ContainsKey(flightCondition))
					{
						component2.RemoveStatusItem(conditionStatuses[flightCondition]);
						conditionStatuses.Remove(flightCondition);
					}
				}
			}
		}
		else
		{
			ClearFlightStatuses();
			component.SendSignal(statusPort, 0);
		}
	}
}
