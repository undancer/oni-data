using System.Collections.Generic;
using UnityEngine;

public class LaunchPadMaterialDistributor : GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>
{
	public class Def : BaseDef
	{
	}

	public class HasRocketStates : State
	{
		public State emptying;

		public State filling;

		public State full;
	}

	public class OperationalStates : State
	{
		public State noRocket;

		public HasRocketStates hasRocket;

		public State rocketLost;
	}

	public new class Instance : GameInstance
	{
		private Dictionary<CargoBay.CargoType, bool> rocketCanTransfer = new Dictionary<CargoBay.CargoType, bool>
		{
			{
				CargoBay.CargoType.Solids,
				false
			},
			{
				CargoBay.CargoType.Liquids,
				false
			},
			{
				CargoBay.CargoType.Gasses,
				false
			}
		};

		private Dictionary<CargoBay.CargoType, bool> modulesCanPump = new Dictionary<CargoBay.CargoType, bool>
		{
			{
				CargoBay.CargoType.Solids,
				false
			},
			{
				CargoBay.CargoType.Liquids,
				false
			},
			{
				CargoBay.CargoType.Gasses,
				false
			}
		};

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public RocketModule GetLandedRocketFromPad()
		{
			return GetComponent<LaunchPad>().LandedRocket;
		}

		public void EmptyRocket(float dt)
		{
			CraftModuleInterface craftInterface = base.sm.attachedRocket.Get<RocketModule>(base.smi).CraftInterface;
			DictionaryPool<CargoBay.CargoType, ListPool<CargoBay, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.PooledDictionary pooledDictionary = DictionaryPool<CargoBay.CargoType, ListPool<CargoBay, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Solids] = ListPool<CargoBay, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Liquids] = ListPool<CargoBay, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Gasses] = ListPool<CargoBay, LaunchPadMaterialDistributor>.Allocate();
			rocketCanTransfer[CargoBay.CargoType.Solids] = false;
			rocketCanTransfer[CargoBay.CargoType.Liquids] = false;
			rocketCanTransfer[CargoBay.CargoType.Gasses] = false;
			modulesCanPump[CargoBay.CargoType.Solids] = false;
			modulesCanPump[CargoBay.CargoType.Liquids] = false;
			modulesCanPump[CargoBay.CargoType.Gasses] = false;
			foreach (Ref<RocketModule> module in craftInterface.Modules)
			{
				RocketModule rocketModule = module.Get();
				CargoBay component = rocketModule.GetComponent<CargoBay>();
				if (component != null && component.storageType != CargoBay.CargoType.Entities && component.storage.MassStored() > 0f)
				{
					pooledDictionary[component.storageType].Add(component);
					rocketCanTransfer[component.storageType] = true;
				}
			}
			foreach (GameObject linkedBuilding in base.gameObject.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings())
			{
				ModularConduitPortController.Instance sMI = linkedBuilding.GetSMI<ModularConduitPortController.Instance>();
				IConduitDispenser component2 = linkedBuilding.GetComponent<IConduitDispenser>();
				if (component2 != null && (sMI == null || sMI.SelectedMode == ModularConduitPortController.Mode.Unload || sMI.SelectedMode == ModularConduitPortController.Mode.Both))
				{
					modulesCanPump[CargoBay.ElementToCargoMap[component2.ConduitType]] = true;
					float num = component2.Storage.RemainingCapacity();
					foreach (CargoBay item in pooledDictionary[CargoBay.ElementToCargoMap[component2.ConduitType]])
					{
						if (num <= 0f)
						{
							break;
						}
						if (item.storage.Count == 0)
						{
							continue;
						}
						for (int num2 = item.storage.items.Count - 1; num2 >= 0; num2--)
						{
							GameObject gameObject = item.storage.items[num2];
							Pickupable pickupable = gameObject.GetComponent<Pickupable>().Take(num);
							if (pickupable != null)
							{
								component2.Storage.Store(pickupable.gameObject);
								num -= pickupable.PrimaryElement.Mass;
							}
						}
					}
				}
				if (sMI != null && component2 != null)
				{
					sMI.SetUnloading(rocketCanTransfer[CargoBay.ElementToCargoMap[component2.ConduitType]]);
				}
			}
			bool flag = true;
			foreach (KeyValuePair<CargoBay.CargoType, bool> item2 in rocketCanTransfer)
			{
				if (item2.Value && modulesCanPump.ContainsKey(item2.Key))
				{
					flag = false;
				}
			}
			pooledDictionary[CargoBay.CargoType.Solids].Recycle();
			pooledDictionary[CargoBay.CargoType.Liquids].Recycle();
			pooledDictionary[CargoBay.CargoType.Gasses].Recycle();
			pooledDictionary.Recycle();
			if (!flag)
			{
				return;
			}
			foreach (GameObject linkedBuilding2 in base.gameObject.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings())
			{
				linkedBuilding2.GetSMI<ModularConduitPortController.Instance>()?.SetUnloading(isUnloading: false);
			}
			base.sm.emptyComplete.Set(value: true, this);
		}

		public void FillRocket(float dt)
		{
			CraftModuleInterface craftInterface = base.sm.attachedRocket.Get<RocketModule>(base.smi).CraftInterface;
			DictionaryPool<CargoBay.CargoType, ListPool<CargoBay, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.PooledDictionary pooledDictionary = DictionaryPool<CargoBay.CargoType, ListPool<CargoBay, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Solids] = ListPool<CargoBay, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Liquids] = ListPool<CargoBay, LaunchPadMaterialDistributor>.Allocate();
			pooledDictionary[CargoBay.CargoType.Gasses] = ListPool<CargoBay, LaunchPadMaterialDistributor>.Allocate();
			rocketCanTransfer[CargoBay.CargoType.Solids] = false;
			rocketCanTransfer[CargoBay.CargoType.Liquids] = false;
			rocketCanTransfer[CargoBay.CargoType.Gasses] = false;
			modulesCanPump[CargoBay.CargoType.Solids] = false;
			modulesCanPump[CargoBay.CargoType.Liquids] = false;
			modulesCanPump[CargoBay.CargoType.Gasses] = false;
			foreach (Ref<RocketModule> module in craftInterface.Modules)
			{
				RocketModule rocketModule = module.Get();
				CargoBay component = rocketModule.GetComponent<CargoBay>();
				if (component != null && component.storageType != CargoBay.CargoType.Entities && component.storage.RemainingCapacity() > 0f)
				{
					pooledDictionary[component.storageType].Add(component);
					rocketCanTransfer[component.storageType] = true;
				}
			}
			foreach (GameObject linkedBuilding in base.gameObject.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings())
			{
				ModularConduitPortController.Instance sMI = linkedBuilding.GetSMI<ModularConduitPortController.Instance>();
				IConduitConsumer component2 = linkedBuilding.GetComponent<IConduitConsumer>();
				if (component2 != null && (sMI == null || sMI.SelectedMode == ModularConduitPortController.Mode.Load || sMI.SelectedMode == ModularConduitPortController.Mode.Both))
				{
					modulesCanPump[CargoBay.ElementToCargoMap[component2.ConduitType]] = true;
					foreach (CargoBay item in pooledDictionary[CargoBay.ElementToCargoMap[component2.ConduitType]])
					{
						float num = item.storage.RemainingCapacity();
						float num2 = component2.Storage.MassStored();
						if (num <= 0f || num2 <= 0f)
						{
							continue;
						}
						for (int num3 = component2.Storage.items.Count - 1; num3 >= 0; num3--)
						{
							GameObject gameObject = component2.Storage.items[num3];
							Pickupable pickupable = gameObject.GetComponent<Pickupable>().Take(num);
							if (pickupable != null)
							{
								item.storage.Store(pickupable.gameObject);
								num -= pickupable.PrimaryElement.Mass;
							}
						}
					}
				}
				if (sMI != null && component2 != null)
				{
					sMI.SetLoading(rocketCanTransfer[CargoBay.ElementToCargoMap[component2.ConduitType]]);
				}
			}
			bool flag = true;
			foreach (KeyValuePair<CargoBay.CargoType, bool> item2 in rocketCanTransfer)
			{
				if (item2.Value && modulesCanPump.ContainsKey(item2.Key))
				{
					flag = false;
				}
			}
			pooledDictionary[CargoBay.CargoType.Solids].Recycle();
			pooledDictionary[CargoBay.CargoType.Liquids].Recycle();
			pooledDictionary[CargoBay.CargoType.Gasses].Recycle();
			pooledDictionary.Recycle();
			if (flag == base.sm.fillComplete.Get(base.smi))
			{
				return;
			}
			foreach (GameObject linkedBuilding2 in base.gameObject.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings())
			{
				linkedBuilding2.GetSMI<ModularConduitPortController.Instance>()?.SetLoading(isLoading: false);
			}
			base.sm.fillComplete.Set(flag, base.smi);
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
		operational.DefaultState(operational.noRocket).Enter(delegate(Instance smi)
		{
			SetAttachedRocket(smi.GetLandedRocketFromPad(), smi);
		}).EventTransition(GameHashes.OperationalChanged, inoperational, (Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		operational.noRocket.EventHandler(GameHashes.RocketLanded, delegate(Instance smi, object data)
		{
			SetAttachedRocket(smi.GetLandedRocketFromPad(), smi);
		}).ParamTransition(attachedRocket, operational.hasRocket, (Instance smi, GameObject p) => p != null);
		operational.hasRocket.DefaultState(operational.hasRocket.emptying).OnTargetLost(attachedRocket, operational.rocketLost).Target(attachedRocket)
			.EventTransition(GameHashes.DoLaunchRocket, operational.rocketLost)
			.Target(masterTarget);
		operational.hasRocket.emptying.ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoEmptying).Update(delegate(Instance smi, float dt)
		{
			smi.EmptyRocket(dt);
		}, UpdateRate.SIM_1000ms).ParamTransition(emptyComplete, operational.hasRocket.filling, GameStateMachine<LaunchPadMaterialDistributor, Instance, IStateMachineTarget, Def>.IsTrue);
		operational.hasRocket.filling.ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoFilling).ParamTransition(fillComplete, operational.hasRocket.full, GameStateMachine<LaunchPadMaterialDistributor, Instance, IStateMachineTarget, Def>.IsTrue).Update(delegate(Instance smi, float dt)
		{
			smi.FillRocket(dt);
		}, UpdateRate.SIM_1000ms);
		operational.hasRocket.full.ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoFull).ToggleTag(GameTags.TransferringCargoComplete).ParamTransition(fillComplete, operational.hasRocket.filling, GameStateMachine<LaunchPadMaterialDistributor, Instance, IStateMachineTarget, Def>.IsFalse)
			.Update(delegate(Instance smi, float dt)
			{
				smi.FillRocket(dt);
			}, UpdateRate.SIM_1000ms);
		operational.rocketLost.Enter(delegate(Instance smi)
		{
			emptyComplete.Set(value: false, smi);
			fillComplete.Set(value: false, smi);
			SetAttachedRocket(null, smi);
		}).GoTo(operational.noRocket);
	}

	private void SetAttachedRocket(RocketModule attached, Instance smi)
	{
		foreach (GameObject linkedBuilding in smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings())
		{
			linkedBuilding.GetSMI<ModularConduitPortController.Instance>()?.SetRocket(attached != null);
		}
		attachedRocket.Set(attached, smi);
	}
}
