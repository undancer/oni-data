using UnityEngine;

public class LaunchPadMaterialDistributor : GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>
{
	public class Def : BaseDef
	{
	}

	public class HasRocketStates : State
	{
		public class TransferringStates : State
		{
			public State actual;

			public State delay;
		}

		public TransferringStates transferring;

		public State transferComplete;
	}

	public class OperationalStates : State
	{
		public State noRocket;

		public State rocketLanding;

		public HasRocketStates hasRocket;

		public State rocketLost;
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public RocketModuleCluster GetLandedRocketFromPad()
		{
			return GetComponent<LaunchPad>().LandedRocket;
		}

		public void EmptyRocket(float dt)
		{
			CraftModuleInterface craftInterface = base.sm.attachedRocket.Get<RocketModuleCluster>(base.smi).CraftInterface;
			DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.PooledDictionary pooledDictionary = DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Solids] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Liquids] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Gasses] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
			foreach (Ref<RocketModuleCluster> clusterModule in craftInterface.ClusterModules)
			{
				CargoBayCluster component = clusterModule.Get().GetComponent<CargoBayCluster>();
				if (component != null && component.storageType != CargoBay.CargoType.Entities && component.storage.MassStored() > 0f)
				{
					pooledDictionary[component.storageType].Add(component);
				}
			}
			bool flag = false;
			HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
			base.smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);
			foreach (ChainedBuilding.StatesInstance item in chain)
			{
				ModularConduitPortController.Instance sMI = item.GetSMI<ModularConduitPortController.Instance>();
				IConduitDispenser component2 = item.GetComponent<IConduitDispenser>();
				Operational component3 = item.GetComponent<Operational>();
				bool unloading = false;
				if (component2 != null && (sMI == null || sMI.SelectedMode == ModularConduitPortController.Mode.Unload || sMI.SelectedMode == ModularConduitPortController.Mode.Both) && (component3 == null || component3.IsOperational))
				{
					sMI.SetRocket(hasRocket: true);
					TreeFilterable component4 = item.GetComponent<TreeFilterable>();
					float num = component2.Storage.RemainingCapacity();
					foreach (CargoBayCluster item2 in pooledDictionary[CargoBayConduit.ElementToCargoMap[component2.ConduitType]])
					{
						if (item2.storage.Count == 0)
						{
							continue;
						}
						for (int num2 = item2.storage.items.Count - 1; num2 >= 0; num2--)
						{
							GameObject gameObject = item2.storage.items[num2];
							if (component4.AcceptedTags.Contains(gameObject.PrefabID()))
							{
								unloading = true;
								flag = true;
								if (num <= 0f)
								{
									break;
								}
								Pickupable pickupable = gameObject.GetComponent<Pickupable>().Take(num);
								if (pickupable != null)
								{
									component2.Storage.Store(pickupable.gameObject);
									num -= pickupable.PrimaryElement.Mass;
								}
							}
						}
					}
				}
				sMI?.SetUnloading(unloading);
			}
			chain.Recycle();
			pooledDictionary[CargoBay.CargoType.Solids].Recycle();
			pooledDictionary[CargoBay.CargoType.Liquids].Recycle();
			pooledDictionary[CargoBay.CargoType.Gasses].Recycle();
			pooledDictionary.Recycle();
			base.sm.emptyComplete.Set(!flag, this);
		}

		public void FillRocket(float dt)
		{
			CraftModuleInterface craftInterface = base.sm.attachedRocket.Get<RocketModuleCluster>(base.smi).CraftInterface;
			DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.PooledDictionary pooledDictionary = DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Solids] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Liquids] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Gasses] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
			foreach (Ref<RocketModuleCluster> clusterModule in craftInterface.ClusterModules)
			{
				CargoBayCluster component = clusterModule.Get().GetComponent<CargoBayCluster>();
				if (component != null && component.storageType != CargoBay.CargoType.Entities && component.RemainingCapacity > 0f)
				{
					pooledDictionary[component.storageType].Add(component);
				}
			}
			bool flag = false;
			HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
			base.smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);
			foreach (ChainedBuilding.StatesInstance item in chain)
			{
				ModularConduitPortController.Instance sMI = item.GetSMI<ModularConduitPortController.Instance>();
				IConduitConsumer component2 = item.GetComponent<IConduitConsumer>();
				bool loading = false;
				if (component2 != null && (sMI == null || sMI.SelectedMode == ModularConduitPortController.Mode.Load || sMI.SelectedMode == ModularConduitPortController.Mode.Both))
				{
					sMI.SetRocket(hasRocket: true);
					for (int num = component2.Storage.items.Count - 1; num >= 0; num--)
					{
						GameObject gameObject = component2.Storage.items[num];
						foreach (CargoBayCluster item2 in pooledDictionary[CargoBayConduit.ElementToCargoMap[component2.ConduitType]])
						{
							float remainingCapacity = item2.RemainingCapacity;
							float num2 = component2.Storage.MassStored();
							if (!(remainingCapacity <= 0f) && !(num2 <= 0f) && item2.GetComponent<TreeFilterable>().AcceptedTags.Contains(gameObject.PrefabID()))
							{
								loading = true;
								flag = true;
								Pickupable pickupable = gameObject.GetComponent<Pickupable>().Take(remainingCapacity);
								if (pickupable != null)
								{
									item2.storage.Store(pickupable.gameObject);
									remainingCapacity -= pickupable.PrimaryElement.Mass;
								}
							}
						}
					}
				}
				sMI?.SetLoading(loading);
			}
			chain.Recycle();
			pooledDictionary[CargoBay.CargoType.Solids].Recycle();
			pooledDictionary[CargoBay.CargoType.Liquids].Recycle();
			pooledDictionary[CargoBay.CargoType.Gasses].Recycle();
			pooledDictionary.Recycle();
			base.sm.fillComplete.Set(!flag, base.smi);
		}
	}

	public State inoperational;

	public OperationalStates operational;

	private TargetParameter attachedRocket;

	private BoolParameter emptyComplete;

	private BoolParameter fillComplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = inoperational;
		base.serializable = SerializeType.ParamsOnly;
		inoperational.EventTransition(GameHashes.OperationalChanged, operational, (Instance smi) => smi.GetComponent<Operational>().IsOperational);
		operational.DefaultState(operational.noRocket).EventTransition(GameHashes.OperationalChanged, inoperational, (Instance smi) => !smi.GetComponent<Operational>().IsOperational).EventHandler(GameHashes.ChainedNetworkChanged, delegate(Instance smi, object data)
		{
			SetAttachedRocket(smi.GetLandedRocketFromPad(), smi);
		});
		operational.noRocket.Enter(delegate(Instance smi)
		{
			SetAttachedRocket(smi.GetLandedRocketFromPad(), smi);
		}).EventHandler(GameHashes.RocketLanded, delegate(Instance smi, object data)
		{
			SetAttachedRocket(smi.GetLandedRocketFromPad(), smi);
		}).EventHandler(GameHashes.RocketCreated, delegate(Instance smi, object data)
		{
			SetAttachedRocket(smi.GetLandedRocketFromPad(), smi);
		})
			.ParamTransition(attachedRocket, operational.rocketLanding, (Instance smi, GameObject p) => p != null);
		operational.rocketLanding.EventTransition(GameHashes.RocketLaunched, operational.rocketLost).OnTargetLost(attachedRocket, operational.rocketLost).Target(attachedRocket)
			.TagTransition(GameTags.RocketOnGround, operational.hasRocket)
			.Target(masterTarget);
		operational.hasRocket.DefaultState(operational.hasRocket.transferring).Update(delegate(Instance smi, float dt)
		{
			smi.EmptyRocket(dt);
		}, UpdateRate.SIM_1000ms).Update(delegate(Instance smi, float dt)
		{
			smi.FillRocket(dt);
		}, UpdateRate.SIM_1000ms)
			.EventTransition(GameHashes.RocketLaunched, operational.rocketLost)
			.OnTargetLost(attachedRocket, operational.rocketLost)
			.Target(attachedRocket)
			.EventTransition(GameHashes.DoLaunchRocket, operational.rocketLost)
			.Target(masterTarget);
		operational.hasRocket.transferring.DefaultState(operational.hasRocket.transferring.actual).ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoEmptying).ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoFilling);
		operational.hasRocket.transferring.actual.ParamTransition(emptyComplete, operational.hasRocket.transferring.delay, (Instance smi, bool p) => emptyComplete.Get(smi) && fillComplete.Get(smi)).ParamTransition(fillComplete, operational.hasRocket.transferring.delay, (Instance smi, bool p) => emptyComplete.Get(smi) && fillComplete.Get(smi));
		operational.hasRocket.transferring.delay.ParamTransition(fillComplete, operational.hasRocket.transferring.actual, GameStateMachine<LaunchPadMaterialDistributor, Instance, IStateMachineTarget, Def>.IsFalse).ParamTransition(emptyComplete, operational.hasRocket.transferring.actual, GameStateMachine<LaunchPadMaterialDistributor, Instance, IStateMachineTarget, Def>.IsFalse).ScheduleGoTo(4f, operational.hasRocket.transferComplete);
		operational.hasRocket.transferComplete.ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoFull).ToggleTag(GameTags.TransferringCargoComplete).ParamTransition(fillComplete, operational.hasRocket.transferring, GameStateMachine<LaunchPadMaterialDistributor, Instance, IStateMachineTarget, Def>.IsFalse)
			.ParamTransition(emptyComplete, operational.hasRocket.transferring, GameStateMachine<LaunchPadMaterialDistributor, Instance, IStateMachineTarget, Def>.IsFalse);
		operational.rocketLost.Enter(delegate(Instance smi)
		{
			emptyComplete.Set(value: false, smi);
			fillComplete.Set(value: false, smi);
			SetAttachedRocket(null, smi);
		}).GoTo(operational.noRocket);
	}

	private void SetAttachedRocket(RocketModuleCluster attached, Instance smi)
	{
		HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
		smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);
		foreach (ChainedBuilding.StatesInstance item in chain)
		{
			item.GetSMI<ModularConduitPortController.Instance>()?.SetRocket(attached != null);
		}
		attachedRocket.Set(attached, smi);
		chain.Recycle();
	}
}
