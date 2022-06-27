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
	private List<Ref<RocketModuleCluster>> clusterModules = new List<Ref<RocketModuleCluster>>();

	private Ref<RocketModuleCluster> bottomModule;

	[Serialize]
	private Dictionary<int, Ref<LaunchPad>> preferredLaunchPad = new Dictionary<int, Ref<LaunchPad>>();

	[MyCmpReq]
	private Clustercraft m_clustercraft;

	private List<ProcessCondition.ProcessConditionType> conditionsToCheck = new List<ProcessCondition.ProcessConditionType>
	{
		ProcessCondition.ProcessConditionType.RocketPrep,
		ProcessCondition.ProcessConditionType.RocketStorage,
		ProcessCondition.ProcessConditionType.RocketBoard,
		ProcessCondition.ProcessConditionType.RocketFlight
	};

	private int lastConditionTypeSucceeded = -1;

	private List<ProcessCondition> returnConditions = new List<ProcessCondition>();

	public IList<Ref<RocketModuleCluster>> ClusterModules => clusterModules.AsReadOnly();

	public LaunchPad CurrentPad
	{
		get
		{
			if (m_clustercraft != null && m_clustercraft.Status != Clustercraft.CraftStatus.InFlight && clusterModules.Count > 0)
			{
				if (bottomModule == null)
				{
					SetBottomModule();
				}
				Debug.Assert(bottomModule != null && bottomModule.Get() != null, "More than one cluster module but no bottom module found.");
				int num = Grid.CellBelow(Grid.PosToCell(bottomModule.Get().transform.position));
				if (Grid.IsValidCell(num))
				{
					GameObject value = null;
					Grid.ObjectLayers[1].TryGetValue(num, out value);
					if (value != null)
					{
						return value.GetComponent<LaunchPad>();
					}
				}
			}
			return null;
		}
	}

	public float Speed => m_clustercraft.Speed;

	public float Range
	{
		get
		{
			foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
			{
				RocketEngineCluster component = clusterModule.Get().GetComponent<RocketEngineCluster>();
				if (component != null)
				{
					return BurnableMassRemaining / component.GetComponent<RocketModuleCluster>().performanceStats.FuelKilogramPerDistance;
				}
			}
			return 0f;
		}
	}

	public float FuelPerHex
	{
		get
		{
			foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
			{
				RocketEngineCluster component = clusterModule.Get().GetComponent<RocketEngineCluster>();
				if (component != null)
				{
					return component.GetComponent<RocketModuleCluster>().performanceStats.FuelKilogramPerDistance * 600f;
				}
			}
			return float.PositiveInfinity;
		}
	}

	public float BurnableMassRemaining
	{
		get
		{
			RocketEngineCluster rocketEngineCluster = null;
			foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
			{
				rocketEngineCluster = clusterModule.Get().GetComponent<RocketEngineCluster>();
				if (rocketEngineCluster != null)
				{
					break;
				}
			}
			if (rocketEngineCluster == null)
			{
				return 0f;
			}
			if (!rocketEngineCluster.requireOxidizer)
			{
				return FuelRemaining;
			}
			return Mathf.Min(FuelRemaining, OxidizerPowerRemaining);
		}
	}

	public float FuelRemaining
	{
		get
		{
			RocketEngineCluster engine = GetEngine();
			if (engine == null)
			{
				return 0f;
			}
			float num = 0f;
			foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
			{
				IFuelTank component = clusterModule.Get().GetComponent<IFuelTank>();
				if (!component.IsNullOrDestroyed())
				{
					num += component.Storage.GetAmountAvailable(engine.fuelTag);
				}
			}
			return Mathf.CeilToInt(num);
		}
	}

	public float OxidizerPowerRemaining
	{
		get
		{
			float num = 0f;
			foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
			{
				OxidizerTank component = clusterModule.Get().GetComponent<OxidizerTank>();
				if (component != null)
				{
					num += component.TotalOxidizerPower;
				}
			}
			return Mathf.CeilToInt(num);
		}
	}

	public int MaxHeight
	{
		get
		{
			RocketEngineCluster engine = GetEngine();
			if (engine != null)
			{
				return engine.maxHeight;
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
			foreach (Ref<RocketModuleCluster> clusterModule in ClusterModules)
			{
				num += clusterModule.Get().GetComponent<Building>().Def.HeightInCells;
			}
			return num;
		}
	}

	public bool HasCargoModule
	{
		get
		{
			foreach (Ref<RocketModuleCluster> clusterModule in ClusterModules)
			{
				if ((object)clusterModule.Get().GetComponent<CargoBayCluster>() != null)
				{
					return true;
				}
			}
			return false;
		}
	}

	public LaunchPad GetPreferredLaunchPadForWorld(int world_id)
	{
		if (preferredLaunchPad.ContainsKey(world_id))
		{
			return preferredLaunchPad[world_id].Get();
		}
		return null;
	}

	private void SetPreferredLaunchPadForWorld(LaunchPad pad)
	{
		if (!preferredLaunchPad.ContainsKey(pad.GetMyWorldId()))
		{
			preferredLaunchPad.Add(CurrentPad.GetMyWorldId(), new Ref<LaunchPad>());
		}
		preferredLaunchPad[CurrentPad.GetMyWorldId()].Set(CurrentPad);
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
		SetBottomModule();
		Subscribe(-1311384361, CompleteSelfDestruct);
	}

	private void OnLoad(Game.GameSaveData data)
	{
		foreach (Ref<RocketModule> module in modules)
		{
			clusterModules.Add(new Ref<RocketModuleCluster>(module.Get().GetComponent<RocketModuleCluster>()));
		}
		modules.Clear();
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			if (!(clusterModule.Get() == null))
			{
				clusterModule.Get().CraftInterface = this;
			}
		}
		bool flag = false;
		for (int num = clusterModules.Count - 1; num >= 0; num--)
		{
			if (clusterModules[num] == null || clusterModules[num].Get() == null)
			{
				Debug.LogWarning($"Rocket {base.name} had a null module at index {num} on load! Why????", this);
				clusterModules.RemoveAt(num);
				flag = true;
			}
		}
		SetBottomModule();
		if (!flag || m_clustercraft.Status != 0)
		{
			return;
		}
		Debug.LogWarning("The module stack was broken. Collapsing " + base.name + "...", this);
		SortModuleListByPosition();
		LaunchPad currentPad = CurrentPad;
		if (currentPad != null)
		{
			int num2 = currentPad.RocketBottomPosition;
			for (int i = 0; i < clusterModules.Count; i++)
			{
				RocketModuleCluster rocketModuleCluster = clusterModules[i].Get();
				if (num2 != Grid.PosToCell(rocketModuleCluster.transform.GetPosition()))
				{
					Debug.LogWarning($"Collapsing space under module {i}:{rocketModuleCluster.name}");
					rocketModuleCluster.transform.SetPosition(Grid.CellToPos(num2, CellAlignment.Bottom, Grid.SceneLayer.Building));
				}
				num2 = Grid.OffsetCell(num2, 0, clusterModules[i].Get().GetComponent<Building>().Def.HeightInCells);
			}
		}
		for (int j = 0; j < clusterModules.Count - 1; j++)
		{
			BuildingAttachPoint component = clusterModules[j].Get().GetComponent<BuildingAttachPoint>();
			if (component != null)
			{
				AttachableBuilding component2 = clusterModules[j + 1].Get().GetComponent<AttachableBuilding>();
				if (component.points[0].attachedBuilding != component2)
				{
					Debug.LogWarning("Reattaching " + component.name + " & " + component2.name);
					component.points[0].attachedBuilding = component2;
				}
			}
		}
	}

	public void AddModule(RocketModuleCluster newModule)
	{
		for (int i = 0; i < clusterModules.Count; i++)
		{
			if (clusterModules[i].Get() == newModule)
			{
				Debug.LogError("Adding module " + newModule?.ToString() + " to the same rocket (" + m_clustercraft.Name + ") twice");
			}
		}
		clusterModules.Add(new Ref<RocketModuleCluster>(newModule));
		newModule.CraftInterface = this;
		Trigger(1512695988, newModule);
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			RocketModuleCluster rocketModuleCluster = clusterModule.Get();
			if (rocketModuleCluster != null && rocketModuleCluster != newModule)
			{
				rocketModuleCluster.Trigger(1512695988, newModule);
			}
		}
		newModule.Trigger(1512695988, newModule);
		SetBottomModule();
	}

	public void RemoveModule(RocketModuleCluster module)
	{
		for (int num = clusterModules.Count - 1; num >= 0; num--)
		{
			if (clusterModules[num].Get() == module)
			{
				clusterModules.RemoveAt(num);
				break;
			}
		}
		Trigger(1512695988);
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			clusterModule.Get().Trigger(1512695988);
		}
		SetBottomModule();
		if (clusterModules.Count == 0)
		{
			base.gameObject.DeleteObject();
		}
	}

	private void SortModuleListByPosition()
	{
		clusterModules.Sort((Ref<RocketModuleCluster> a, Ref<RocketModuleCluster> b) => (!(Grid.CellToPos(Grid.PosToCell(a.Get())).y < Grid.CellToPos(Grid.PosToCell(b.Get())).y)) ? 1 : (-1));
	}

	private void SetBottomModule()
	{
		if (clusterModules.Count > 0)
		{
			bottomModule = clusterModules[0];
			Vector3 vector = bottomModule.Get().transform.position;
			{
				foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
				{
					Vector3 position = clusterModule.Get().transform.position;
					if (position.y < vector.y)
					{
						bottomModule = clusterModule;
						vector = position;
					}
				}
				return;
			}
		}
		bottomModule = null;
	}

	public int GetHeightOfModuleTop(GameObject module)
	{
		int num = 0;
		for (int i = 0; i < ClusterModules.Count; i++)
		{
			num += clusterModules[i].Get().GetComponent<Building>().Def.HeightInCells;
			if (clusterModules[i].Get().gameObject == module)
			{
				return num;
			}
		}
		Debug.LogError("Could not find module " + module.GetProperName() + " in CraftModuleInterface craft " + m_clustercraft.Name);
		return 0;
	}

	public int GetModuleRelativeVerticalPosition(GameObject module)
	{
		int num = 0;
		for (int i = 0; i < ClusterModules.Count; i++)
		{
			if (clusterModules[i].Get().gameObject == module)
			{
				return num;
			}
			num += clusterModules[i].Get().GetComponent<Building>().Def.HeightInCells;
		}
		Debug.LogError("Could not find module " + module.GetProperName() + " in CraftModuleInterface craft " + m_clustercraft.Name);
		return 0;
	}

	public void Sim4000ms(float dt)
	{
		int num = 0;
		foreach (ProcessCondition.ProcessConditionType item in conditionsToCheck)
		{
			if (EvaluateConditionSet(item) != 0)
			{
				num++;
			}
		}
		if (num != lastConditionTypeSucceeded)
		{
			lastConditionTypeSucceeded = num;
			TriggerEventOnCraftAndRocket(GameHashes.LaunchConditionChanged, null);
		}
	}

	public bool IsLaunchRequested()
	{
		return m_clustercraft.LaunchRequested;
	}

	public bool CheckPreppedForLaunch()
	{
		if (EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep) != 0 && EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage) != 0)
		{
			return EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketFlight) != ProcessCondition.Status.Failure;
		}
		return false;
	}

	public bool CheckReadyToLaunch()
	{
		if (EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep) != 0 && EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage) != 0 && EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketFlight) != 0)
		{
			return EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketBoard) != ProcessCondition.Status.Failure;
		}
		return false;
	}

	public bool HasLaunchWarnings()
	{
		if (EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep) != ProcessCondition.Status.Warning && EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage) != ProcessCondition.Status.Warning)
		{
			return EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketBoard) == ProcessCondition.Status.Warning;
		}
		return true;
	}

	public bool CheckReadyForAutomatedLaunchCommand()
	{
		if (EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep) == ProcessCondition.Status.Ready)
		{
			return EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage) == ProcessCondition.Status.Ready;
		}
		return false;
	}

	public bool CheckReadyForAutomatedLaunch()
	{
		if (EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep) == ProcessCondition.Status.Ready && EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage) == ProcessCondition.Status.Ready)
		{
			return EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketBoard) == ProcessCondition.Status.Ready;
		}
		return false;
	}

	public void TriggerEventOnCraftAndRocket(GameHashes evt, object data)
	{
		Trigger((int)evt, data);
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			clusterModule.Get().Trigger((int)evt, data);
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
		CurrentPad.Trigger(705820818, this);
		SetPreferredLaunchPadForWorld(CurrentPad);
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			clusterModule.Get().Trigger(705820818, this);
		}
	}

	public void DoLand(LaunchPad pad)
	{
		int num = pad.RocketBottomPosition;
		for (int i = 0; i < clusterModules.Count; i++)
		{
			clusterModules[i].Get().MoveToPad(num);
			num = Grid.OffsetCell(num, 0, clusterModules[i].Get().GetComponent<Building>().Def.HeightInCells);
		}
		SetBottomModule();
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			clusterModule.Get().Trigger(-1165815793, pad);
		}
		pad.Trigger(-1165815793, this);
	}

	public LaunchConditionManager FindLaunchConditionManager()
	{
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			LaunchConditionManager component = clusterModule.Get().GetComponent<LaunchConditionManager>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public LaunchableRocketCluster FindLaunchableRocket()
	{
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			RocketModuleCluster rocketModuleCluster = clusterModule.Get();
			LaunchableRocketCluster component = rocketModuleCluster.GetComponent<LaunchableRocketCluster>();
			if (component != null && rocketModuleCluster.CraftInterface != null && rocketModuleCluster.CraftInterface.GetComponent<Clustercraft>().Status == Clustercraft.CraftStatus.Grounded)
			{
				return component;
			}
		}
		return null;
	}

	public List<GameObject> GetParts()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			list.Add(clusterModule.Get().gameObject);
		}
		return list;
	}

	public RocketEngineCluster GetEngine()
	{
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			RocketEngineCluster component = clusterModule.Get().GetComponent<RocketEngineCluster>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public PassengerRocketModule GetPassengerModule()
	{
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			PassengerRocketModule component = clusterModule.Get().GetComponent<PassengerRocketModule>();
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
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			List<ProcessCondition> conditionSet = clusterModule.Get().GetConditionSet(conditionType);
			if (conditionSet != null)
			{
				returnConditions.AddRange(conditionSet);
			}
		}
		if (CurrentPad != null)
		{
			List<ProcessCondition> conditionSet2 = CurrentPad.GetComponent<LaunchPadConditions>().GetConditionSet(conditionType);
			if (conditionSet2 != null)
			{
				returnConditions.AddRange(conditionSet2);
			}
		}
		return returnConditions;
	}

	private ProcessCondition.Status EvaluateConditionSet(ProcessCondition.ProcessConditionType conditionType)
	{
		ProcessCondition.Status status = ProcessCondition.Status.Ready;
		foreach (ProcessCondition item in GetConditionSet(conditionType))
		{
			ProcessCondition.Status status2 = item.EvaluateCondition();
			if (status2 < status)
			{
				status = status2;
			}
			if (status == ProcessCondition.Status.Failure)
			{
				return status;
			}
		}
		return status;
	}

	private void ForceAttachmentNetwork()
	{
		RocketModuleCluster rocketModuleCluster = null;
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			RocketModuleCluster rocketModuleCluster2 = clusterModule.Get();
			if (rocketModuleCluster != null)
			{
				BuildingAttachPoint component = rocketModuleCluster.GetComponent<BuildingAttachPoint>();
				AttachableBuilding component2 = rocketModuleCluster2.GetComponent<AttachableBuilding>();
				component.points[0].attachedBuilding = component2;
			}
			rocketModuleCluster = rocketModuleCluster2;
		}
	}

	public static Storage SpawnRocketDebris(string nameSuffix, SimHashes element)
	{
		GameObject obj = Util.KInstantiate(position: new Vector3(-1f, -1f, 0f), original: Assets.GetPrefab("DebrisPayload"));
		obj.GetComponent<PrimaryElement>().SetElement(element);
		obj.name += nameSuffix;
		obj.SetActive(value: true);
		return obj.GetComponent<Storage>();
	}

	public void CompleteSelfDestruct(object data = null)
	{
		Debug.Assert(this.HasTag(GameTags.RocketInSpace), "Self Destruct is only valid for in-space rockets!");
		SimHashes elementID = GetPassengerModule().GetComponent<PrimaryElement>().ElementID;
		List<RocketModule> list = new List<RocketModule>();
		foreach (Ref<RocketModuleCluster> clusterModule in clusterModules)
		{
			list.Add(clusterModule.Get());
		}
		List<GameObject> list2 = new List<GameObject>();
		foreach (RocketModule item in list)
		{
			Storage[] components = item.GetComponents<Storage>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].DropAll(vent_gas: false, dump_liquid: false, default(Vector3), do_disease_transfer: true, list2);
			}
			Deconstructable component = item.GetComponent<Deconstructable>();
			list2.AddRange(component.ForceDestroyAndGetMaterials());
		}
		List<Storage> list3 = new List<Storage>();
		foreach (GameObject item2 in list2)
		{
			Pickupable component2 = item2.GetComponent<Pickupable>();
			if (component2 != null)
			{
				component2.PrimaryElement.Units = Mathf.Max(1, Mathf.RoundToInt(component2.PrimaryElement.Units * 0.5f));
				if ((list3.Count == 0 || list3[list3.Count - 1].RemainingCapacity() == 0f) && component2.PrimaryElement.Mass > 0f)
				{
					list3.Add(SpawnRocketDebris(" from CMI", elementID));
				}
				Storage storage = list3[list3.Count - 1];
				while (component2.PrimaryElement.Mass > storage.RemainingCapacity())
				{
					Pickupable pickupable = component2.Take(storage.RemainingCapacity());
					storage.Store(pickupable.gameObject);
					storage = SpawnRocketDebris(" from CMI", elementID);
					list3.Add(storage);
				}
				if (component2.PrimaryElement.Mass > 0f)
				{
					storage.Store(component2.gameObject);
				}
			}
		}
		foreach (Storage item3 in list3)
		{
			RailGunPayload.StatesInstance sMI = item3.GetSMI<RailGunPayload.StatesInstance>();
			sMI.StartSM();
			sMI.Travel(m_clustercraft.Location, ClusterUtil.ClosestVisibleAsteroidToLocation(m_clustercraft.Location).Location);
		}
		m_clustercraft.SetExploding();
	}
}
