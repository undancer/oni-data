using KSerialization;
using UnityEngine;

public class OrbitalDeployCargoModule : GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>
{
	public class Def : BaseDef
	{
		public float numCapsules;
	}

	public class GroundedStates : State
	{
		public State loading;

		public State loaded;

		public State empty;
	}

	public class NotGroundedStates : State
	{
		public State loaded;

		public State emptying;

		public State empty;
	}

	public class StatesInstance : GameInstance, IEmptyableCargo
	{
		private Storage storage;

		[Serialize]
		private bool autoDeploy;

		public bool AutoDeploy
		{
			get
			{
				return autoDeploy;
			}
			set
			{
				autoDeploy = value;
			}
		}

		public bool CanAutoDeploy => true;

		public bool ChooseDuplicant => false;

		public MinionIdentity ChosenDuplicant
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			storage = GetComponent<Storage>();
			GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, new LoadingCompleteCondition(storage));
		}

		public bool NeedsVisualUpdate()
		{
			int num = base.sm.numVisualCapsules.Get(this);
			int num2 = Mathf.FloorToInt(storage.MassStored() / 200f);
			if (num < num2)
			{
				base.sm.numVisualCapsules.Delta(1, this);
				return true;
			}
			return false;
		}

		public string GetLoadingAnimName()
		{
			int num = base.sm.numVisualCapsules.Get(this);
			int num2 = Mathf.RoundToInt(storage.capacityKg / 200f);
			if (num == num2)
			{
				return "loading6_full";
			}
			if (num == num2 - 1)
			{
				return "loading5";
			}
			if (num == num2 - 2)
			{
				return "loading4";
			}
			if (num == num2 - 3 || num > 2)
			{
				return "loading3_repeat";
			}
			return num switch
			{
				2 => "loading2", 
				1 => "loading1", 
				_ => "deployed", 
			};
		}

		public void DeployCargoPods()
		{
			Clustercraft component = base.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			ClusterGridEntity orbitAsteroid = component.GetOrbitAsteroid();
			if (orbitAsteroid != null)
			{
				WorldContainer component2 = orbitAsteroid.GetComponent<WorldContainer>();
				_ = component2.id;
				Vector3 position = new Vector3(component2.minimumBounds.x + 1f, component2.maximumBounds.y, Grid.GetLayerZ(Grid.SceneLayer.Front));
				while (storage.MassStored() > 0f)
				{
					GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("RailGunPayload"), position);
					gameObject.GetComponent<Pickupable>().deleteOffGrid = false;
					for (float num = 0f; num < 200f; num += storage.Transfer(gameObject.GetComponent<Storage>(), GameTags.Stored, 200f - num, block_events: false, hide_popups: true))
					{
						if (!(storage.MassStored() > 0f))
						{
							break;
						}
					}
					gameObject.SetActive(value: true);
					gameObject.GetSMI<RailGunPayload.StatesInstance>().Travel(component.Location, component2.GetMyWorldLocation());
				}
			}
			CheckIfLoaded();
		}

		public bool CheckIfLoaded()
		{
			bool flag = storage.MassStored() > 0f;
			if (flag != base.sm.hasCargo.Get(this))
			{
				base.sm.hasCargo.Set(flag, this);
			}
			return flag;
		}

		public bool IsValidDropLocation()
		{
			return GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().GetOrbitAsteroid() != null;
		}

		public void EmptyCargo()
		{
			DeployCargoPods();
		}

		public bool CanEmptyCargo()
		{
			if (base.sm.hasCargo.Get(base.smi))
			{
				return IsValidDropLocation();
			}
			return false;
		}
	}

	public BoolParameter hasCargo;

	public Signal emptyCargo;

	public GroundedStates grounded;

	public NotGroundedStates not_grounded;

	public IntParameter numVisualCapsules;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = grounded;
		root.Enter(delegate(StatesInstance smi)
		{
			smi.CheckIfLoaded();
		}).EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
		{
			smi.CheckIfLoaded();
		}).EventHandler(GameHashes.ClusterDestinationReached, delegate(StatesInstance smi)
		{
			if (smi.AutoDeploy && smi.IsValidDropLocation())
			{
				smi.DeployCargoPods();
			}
		});
		grounded.DefaultState(grounded.loaded).TagTransition(GameTags.RocketNotOnGround, not_grounded);
		grounded.loading.PlayAnim((StatesInstance smi) => smi.GetLoadingAnimName()).ParamTransition(hasCargo, grounded.empty, GameStateMachine<OrbitalDeployCargoModule, StatesInstance, IStateMachineTarget, Def>.IsFalse).OnAnimQueueComplete(grounded.loaded);
		grounded.loaded.ParamTransition(hasCargo, grounded.empty, GameStateMachine<OrbitalDeployCargoModule, StatesInstance, IStateMachineTarget, Def>.IsFalse).EventTransition(GameHashes.OnStorageChange, grounded.loading, (StatesInstance smi) => smi.NeedsVisualUpdate());
		grounded.empty.Enter(delegate(StatesInstance smi)
		{
			numVisualCapsules.Set(0, smi);
		}).PlayAnim("deployed").ParamTransition(hasCargo, grounded.loaded, GameStateMachine<OrbitalDeployCargoModule, StatesInstance, IStateMachineTarget, Def>.IsTrue);
		not_grounded.DefaultState(not_grounded.loaded).TagTransition(GameTags.RocketNotOnGround, grounded, on_remove: true);
		not_grounded.loaded.PlayAnim("loaded").ParamTransition(hasCargo, not_grounded.empty, GameStateMachine<OrbitalDeployCargoModule, StatesInstance, IStateMachineTarget, Def>.IsFalse).OnSignal(emptyCargo, not_grounded.emptying);
		not_grounded.emptying.PlayAnim("deploying").GoTo(not_grounded.empty);
		not_grounded.empty.PlayAnim("deployed").ParamTransition(hasCargo, not_grounded.loaded, GameStateMachine<OrbitalDeployCargoModule, StatesInstance, IStateMachineTarget, Def>.IsTrue);
	}
}
