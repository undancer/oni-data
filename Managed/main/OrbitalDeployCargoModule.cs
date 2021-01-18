using KSerialization;
using UnityEngine;

public class OrbitalDeployCargoModule : GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>
{
	public class Def : BaseDef
	{
		public float launchMass;
	}

	public class GroundedStates : State
	{
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
		private bool autoDeploy = false;

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
			RocketModule component = GetComponent<RocketModule>();
			component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new LoadingCompleteCondition(storage));
		}

		public void DeployCargoPods()
		{
			RocketModule component = base.master.GetComponent<RocketModule>();
			Clustercraft component2 = component.CraftInterface.GetComponent<Clustercraft>();
			ClusterGridEntity orbitAsteroid = component2.GetOrbitAsteroid();
			if (orbitAsteroid != null)
			{
				WorldContainer component3 = orbitAsteroid.GetComponent<WorldContainer>();
				int id = component3.id;
				Vector3 position = new Vector3(component3.minimumBounds.x, component3.maximumBounds.y, 0f);
				while (storage.MassStored() > 0f)
				{
					GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("RailGunPayload"), position);
					for (float num = 0f; num < base.def.launchMass; num += storage.Transfer(gameObject.GetComponent<Storage>(), GameTags.Stored, base.def.launchMass - num, block_events: false, hide_popups: true))
					{
						if (!(storage.MassStored() > 0f))
						{
							break;
						}
					}
					gameObject.SetActive(value: true);
					gameObject.GetSMI<RailGunPayload.StatesInstance>().Launch(id);
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
			Clustercraft component = GetComponent<RocketModule>().CraftInterface.GetComponent<Clustercraft>();
			return component.GetOrbitAsteroid() != null;
		}

		public void EmptyCargo()
		{
			DeployCargoPods();
		}

		public bool CanEmptyCargo()
		{
			return base.sm.hasCargo.Get(base.smi) && IsValidDropLocation();
		}
	}

	public BoolParameter hasCargo;

	public Signal emptyCargo;

	public GroundedStates grounded;

	public NotGroundedStates not_grounded;

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
		grounded.loaded.PlayAnim("loaded").ParamTransition(hasCargo, grounded.empty, GameStateMachine<OrbitalDeployCargoModule, StatesInstance, IStateMachineTarget, Def>.IsFalse);
		grounded.empty.PlayAnim("deployed").ParamTransition(hasCargo, grounded.loaded, GameStateMachine<OrbitalDeployCargoModule, StatesInstance, IStateMachineTarget, Def>.IsTrue);
		not_grounded.DefaultState(not_grounded.loaded).TagTransition(GameTags.RocketNotOnGround, grounded, on_remove: true);
		not_grounded.loaded.PlayAnim("loaded").ParamTransition(hasCargo, not_grounded.empty, GameStateMachine<OrbitalDeployCargoModule, StatesInstance, IStateMachineTarget, Def>.IsFalse).OnSignal(emptyCargo, not_grounded.emptying);
		not_grounded.emptying.PlayAnim("deploying").GoTo(not_grounded.empty);
		not_grounded.empty.PlayAnim("deployed").ParamTransition(hasCargo, not_grounded.loaded, GameStateMachine<OrbitalDeployCargoModule, StatesInstance, IStateMachineTarget, Def>.IsTrue);
	}
}
