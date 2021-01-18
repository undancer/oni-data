using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CraftModuleInterface : KMonoBehaviour, ISim4000ms
{
	[Serialize]
	private List<Ref<RocketModule>> modules = new List<Ref<RocketModule>>();

	[Serialize]
	public Ref<LaunchPad> currentPad = new Ref<LaunchPad>();

	[MyCmpReq]
	private Clustercraft m_clustercraft;

	private List<ProcessCondition.ProcessConditionType> conditionsToCheck = new List<ProcessCondition.ProcessConditionType>
	{
		ProcessCondition.ProcessConditionType.RocketPrep,
		ProcessCondition.ProcessConditionType.RocketBoard
	};

	private int lastConditionTypeSucceeded = -1;

	private List<ProcessCondition> returnConditions = new List<ProcessCondition>();

	public IList<Ref<RocketModule>> Modules => modules.AsReadOnly();

	public LaunchPad CurrentPad => currentPad.Get();

	public float Speed => m_clustercraft.Speed;

	public float Range
	{
		get
		{
			foreach (Ref<RocketModule> module in modules)
			{
				RocketEngine component = module.Get().GetComponent<RocketEngine>();
				if (component != null)
				{
					float burnableMassRemaining = BurnableMassRemaining;
					return burnableMassRemaining / component.GetComponent<RocketModule>().performanceStats.FuelKilogramPerDistance;
				}
			}
			return 0f;
		}
	}

	public float FuelPerHex
	{
		get
		{
			foreach (Ref<RocketModule> module in modules)
			{
				RocketEngine component = module.Get().GetComponent<RocketEngine>();
				if (component != null)
				{
					return component.GetComponent<RocketModule>().performanceStats.FuelKilogramPerDistance;
				}
			}
			return float.PositiveInfinity;
		}
	}

	public float BurnableMassRemaining
	{
		get
		{
			RocketEngine rocketEngine = null;
			foreach (Ref<RocketModule> module in modules)
			{
				rocketEngine = module.Get().GetComponent<RocketEngine>();
				if (rocketEngine != null)
				{
					break;
				}
			}
			if (rocketEngine == null)
			{
				return 0f;
			}
			return rocketEngine.requireOxidizer ? Mathf.Min(FuelRemaining, OxidizerPowerRemaining) : FuelRemaining;
		}
	}

	public float FuelRemaining
	{
		get
		{
			float num = 0f;
			foreach (Ref<RocketModule> module in modules)
			{
				FuelTank component = module.Get().GetComponent<FuelTank>();
				if (component != null)
				{
					num += component.AmountStored;
				}
			}
			return num;
		}
	}

	public float OxidizerPowerRemaining
	{
		get
		{
			float num = 0f;
			foreach (Ref<RocketModule> module in modules)
			{
				OxidizerTank component = module.Get().GetComponent<OxidizerTank>();
				if (component != null)
				{
					num += component.TotalOxidizerPower;
				}
			}
			return num;
		}
	}

	public int MaxModules
	{
		get
		{
			RocketEngine engine = GetEngine();
			if (engine != null)
			{
				return engine.maxModules;
			}
			return -1;
		}
	}

	public float TotalBurden => m_clustercraft.TotalBurden;

	public float EnginePower => m_clustercraft.EnginePower;

	public int RocketHeight
	{
		get
		{
			int num = 0;
			foreach (Ref<RocketModule> module in Modules)
			{
				num += module.Get().GetComponent<Building>().Def.HeightInCells;
			}
			return num;
		}
	}

	protected override void OnPrefabInit()
	{
		Game instance = Game.Instance;
		instance.OnLoad = (Action<Game.GameSaveData>)Delegate.Combine(instance.OnLoad, new Action<Game.GameSaveData>(OnLoad));
	}

	protected override void OnSpawn()
	{
		Game instance = Game.Instance;
		instance.OnLoad = (Action<Game.GameSaveData>)Delegate.Remove(instance.OnLoad, new Action<Game.GameSaveData>(OnLoad));
		if (m_clustercraft.Status != 0)
		{
			ForceAttachmentNetwork();
		}
	}

	private void OnLoad(Game.GameSaveData data)
	{
		foreach (Ref<RocketModule> module in modules)
		{
			if (!(module.Get() == null))
			{
				module.Get().CraftInterface = this;
			}
		}
		for (int num = modules.Count - 1; num >= 0; num--)
		{
			if (modules[num] == null || modules[num].Get() == null)
			{
				Debug.LogWarning($"Rocket {base.name} had a null module at index {num} on load! Why????", this);
				modules.RemoveAt(num);
			}
		}
	}

	public void AddModule(RocketModule newModule)
	{
		for (int i = 0; i < modules.Count; i++)
		{
			if (modules[i].Get() == newModule)
			{
				Debug.LogError(string.Concat("Adding module ", newModule, " to the same rocket (", m_clustercraft.Name, ") twice"));
			}
		}
		modules.Add(new Ref<RocketModule>(newModule));
		newModule.CraftInterface = this;
		Trigger(826910016, newModule);
		foreach (Ref<RocketModule> module in modules)
		{
			RocketModule rocketModule = module.Get();
			if (rocketModule != null && rocketModule != newModule)
			{
				rocketModule.Trigger(826910016, newModule);
			}
		}
	}

	public void RemoveModule(RocketModule module)
	{
		for (int num = modules.Count - 1; num >= 0; num--)
		{
			if (modules[num].Get() == module)
			{
				modules.RemoveAt(num);
				break;
			}
		}
		if (modules.Count == 0)
		{
			base.gameObject.DeleteObject();
		}
	}

	private void SortModuleListByPosition()
	{
		modules.Sort((Ref<RocketModule> a, Ref<RocketModule> b) => (!(Grid.CellToPos(Grid.PosToCell(a.Get())).y < Grid.CellToPos(Grid.PosToCell(b.Get())).y)) ? 1 : (-1));
	}

	public void Sim4000ms(float dt)
	{
		int num = 0;
		foreach (ProcessCondition.ProcessConditionType item in conditionsToCheck)
		{
			if (EvaluateConditionSet(item))
			{
				num++;
				continue;
			}
			break;
		}
		if (num != lastConditionTypeSucceeded)
		{
			lastConditionTypeSucceeded = num;
			TriggerEventOnCraftAndRocket(GameHashes.LaunchConditionChanged, null);
		}
		if (num != conditionsToCheck.Count && IsLaunchRequested())
		{
			m_clustercraft.CancelLaunch();
		}
	}

	public bool IsLaunchRequested()
	{
		return m_clustercraft.LaunchRequested;
	}

	public bool CheckCrewShouldBoard()
	{
		return EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep);
	}

	public bool CheckReadyToLaunch()
	{
		return EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep) && EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketBoard);
	}

	public bool CheckAbleToFly()
	{
		return EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketFlight);
	}

	public void TriggerEventOnCraftAndRocket(GameHashes evt, object data)
	{
		Trigger((int)evt, data);
		foreach (Ref<RocketModule> module in modules)
		{
			module.Get().Trigger((int)evt, data);
		}
	}

	public void CancelLaunch()
	{
		m_clustercraft.CancelLaunch();
	}

	public void TriggerLaunch(bool automated = false)
	{
		m_clustercraft.RequestLaunch(automated);
	}

	public void DoLaunch()
	{
		SortModuleListByPosition();
		currentPad.Get().Trigger(705820818, this);
		foreach (Ref<RocketModule> module in modules)
		{
			module.Get().Trigger(705820818, this);
		}
		currentPad.Set(null);
	}

	public void DoLand(LaunchPad pad)
	{
		SetCurrentPad(pad);
		int num = pad.PadPosition;
		for (int i = 0; i < modules.Count; i++)
		{
			modules[i].Get().MoveToPad(num);
			num = Grid.OffsetCell(num, 0, modules[i].Get().GetComponent<Building>().Def.HeightInCells);
		}
		foreach (Ref<RocketModule> module in modules)
		{
			if (module.Get().GetComponent<LaunchableRocket>() != null)
			{
				module.Get().Trigger(-1165815793, pad);
				break;
			}
		}
		pad.Trigger(-1165815793, this);
	}

	public void SetCurrentPad(LaunchPad pad)
	{
		currentPad.Set(pad);
	}

	public LaunchConditionManager FindLaunchConditionManager()
	{
		foreach (Ref<RocketModule> module in modules)
		{
			LaunchConditionManager component = module.Get().GetComponent<LaunchConditionManager>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public LaunchableRocket FindLaunchableRocket()
	{
		foreach (Ref<RocketModule> module in modules)
		{
			LaunchableRocket component = module.Get().GetComponent<LaunchableRocket>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public List<GameObject> GetParts()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Ref<RocketModule> module in modules)
		{
			list.Add(module.Get().gameObject);
		}
		return list;
	}

	public RocketEngine GetEngine()
	{
		foreach (Ref<RocketModule> module in modules)
		{
			RocketEngine component = module.Get().GetComponent<RocketEngine>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public PassengerRocketModule GetPassengerModule()
	{
		foreach (Ref<RocketModule> module in modules)
		{
			PassengerRocketModule component = module.Get().GetComponent<PassengerRocketModule>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public WorldContainer GetInteriorWorld()
	{
		PassengerRocketModule passengerModule = GetPassengerModule();
		if (passengerModule == null)
		{
			return null;
		}
		ClustercraftInteriorDoor interiorDoor = passengerModule.GetComponent<ClustercraftExteriorDoor>().GetInteriorDoor();
		if (interiorDoor == null)
		{
			return null;
		}
		return interiorDoor.GetMyWorld();
	}

	public RocketClusterDestinationSelector GetClusterDestinationSelector()
	{
		return GetComponent<RocketClusterDestinationSelector>();
	}

	public bool HasClusterDestinationSelector()
	{
		return GetComponent<RocketClusterDestinationSelector>() != null;
	}

	public List<ProcessCondition> GetConditionSet(ProcessCondition.ProcessConditionType conditionType)
	{
		returnConditions.Clear();
		foreach (Ref<RocketModule> module in modules)
		{
			List<ProcessCondition> conditionSet = module.Get().GetConditionSet(conditionType);
			if (conditionSet != null)
			{
				returnConditions.AddRange(conditionSet);
			}
		}
		if (currentPad.Get() != null)
		{
			List<ProcessCondition> conditionSet2 = currentPad.Get().GetComponent<LaunchPadConditions>().GetConditionSet(conditionType);
			if (conditionSet2 != null)
			{
				returnConditions.AddRange(conditionSet2);
			}
		}
		return returnConditions;
	}

	public bool HasLaunchWarnings()
	{
		foreach (ProcessCondition item in GetConditionSet(ProcessCondition.ProcessConditionType.RocketPrep))
		{
			if (item.EvaluateCondition() == ProcessCondition.Status.Warning)
			{
				return true;
			}
		}
		foreach (ProcessCondition item2 in GetConditionSet(ProcessCondition.ProcessConditionType.RocketBoard))
		{
			if (item2.EvaluateCondition() == ProcessCondition.Status.Warning)
			{
				return true;
			}
		}
		return false;
	}

	private bool EvaluateConditionSet(ProcessCondition.ProcessConditionType conditionType)
	{
		foreach (ProcessCondition item in GetConditionSet(conditionType))
		{
			if (item.EvaluateCondition() == ProcessCondition.Status.Failure)
			{
				return false;
			}
		}
		return true;
	}

	private void ForceAttachmentNetwork()
	{
		RocketModule rocketModule = null;
		foreach (Ref<RocketModule> module in modules)
		{
			RocketModule rocketModule2 = module.Get();
			if (rocketModule != null)
			{
				BuildingAttachPoint component = rocketModule.GetComponent<BuildingAttachPoint>();
				AttachableBuilding component2 = rocketModule2.GetComponent<AttachableBuilding>();
				component.points[0].attachedBuilding = component2;
			}
			rocketModule = rocketModule2;
		}
	}
}
