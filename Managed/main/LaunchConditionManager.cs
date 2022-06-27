using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LaunchConditionManager")]
public class LaunchConditionManager : KMonoBehaviour, ISim4000ms, ISim1000ms
{
	public enum ConditionType
	{
		Launch,
		Flight
	}

	public HashedString triggerPort;

	public HashedString statusPort;

	private ILaunchableRocket launchable;

	[Serialize]
	private List<Tuple<string, string, string>> DEBUG_ModuleDestructions;

	private Dictionary<ProcessCondition, Guid> conditionStatuses = new Dictionary<ProcessCondition, Guid>();

	public List<RocketModule> rocketModules { get; private set; }

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
		launchable = GetComponent<ILaunchableRocket>();
		FindModules();
		GetComponent<AttachableBuilding>().onAttachmentNetworkChanged = delegate
		{
			FindModules();
		};
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
			Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(spacecraftFromLaunchConditionManager.id);
			if (base.gameObject.GetComponent<LogicPorts>().GetInputValue(triggerPort) == 1 && spacecraftDestination != null && spacecraftDestination.id != -1)
			{
				Launch(spacecraftDestination);
			}
		}
	}

	public void FindModules()
	{
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>()))
		{
			RocketModule component = item.GetComponent<RocketModule>();
			if (component != null && component.conditionManager == null)
			{
				component.conditionManager = this;
				component.RegisterWithConditionManager();
			}
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

	public List<ProcessCondition> GetLaunchConditionList()
	{
		List<ProcessCondition> list = new List<ProcessCondition>();
		foreach (RocketModule rocketModule in rocketModules)
		{
			foreach (ProcessCondition item in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketPrep))
			{
				list.Add(item);
			}
			foreach (ProcessCondition item2 in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketStorage))
			{
				list.Add(item2);
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
			launchable.LaunchableGameObject.Trigger(705820818);
			SpacecraftManager.instance.SetSpacecraftDestination(this, destination);
			SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this).BeginMission(destination);
		}
	}

	public bool CheckReadyToLaunch()
	{
		foreach (RocketModule rocketModule in rocketModules)
		{
			foreach (ProcessCondition item in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketPrep))
			{
				if (item.EvaluateCondition() == ProcessCondition.Status.Failure)
				{
					return false;
				}
			}
			foreach (ProcessCondition item2 in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketStorage))
			{
				if (item2.EvaluateCondition() == ProcessCondition.Status.Failure)
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
			foreach (ProcessCondition item in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketFlight))
			{
				if (item.EvaluateCondition() == ProcessCondition.Status.Failure)
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
		foreach (KeyValuePair<ProcessCondition, Guid> conditionStatus in conditionStatuses)
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
			{
				foreach (RocketModule rocketModule in rocketModules)
				{
					foreach (ProcessCondition item in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketFlight))
					{
						if (item.EvaluateCondition() == ProcessCondition.Status.Failure)
						{
							if (!conditionStatuses.ContainsKey(item))
							{
								StatusItem statusItem = item.GetStatusItem(ProcessCondition.Status.Failure);
								conditionStatuses[item] = component2.AddStatusItem(statusItem, item);
							}
						}
						else if (conditionStatuses.ContainsKey(item))
						{
							component2.RemoveStatusItem(conditionStatuses[item]);
							conditionStatuses.Remove(item);
						}
					}
				}
				return;
			}
		}
		ClearFlightStatuses();
		component.SendSignal(statusPort, 0);
	}
}
