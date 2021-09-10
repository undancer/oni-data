using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class TravellingCargoLander : GameStateMachine<TravellingCargoLander, TravellingCargoLander.StatesInstance, IStateMachineTarget, TravellingCargoLander.Def>
{
	public class Def : BaseDef
	{
		public int landerWidth = 1;

		public float landingSpeed = 5f;

		public bool deployOnLanding;
	}

	public class TravelStates : State
	{
		public State travelling;

		public State transferWorlds;
	}

	public class LandingStates : State
	{
		public State landing;

		public State impact;
	}

	public class GroundedStates : State
	{
		public State loaded;

		public State emptying;

		public State empty;
	}

	public class StatesInstance : GameInstance
	{
		[Serialize]
		public float flightAnimOffset = 50f;

		public KBatchedAnimController animController;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			animController = GetComponent<KBatchedAnimController>();
		}

		public void Travel(AxialI source, AxialI destination)
		{
			GetComponent<BallisticClusterGridEntity>().Configure(source, destination);
			int asteroidWorldIdAtLocation = ClusterUtil.GetAsteroidWorldIdAtLocation(destination);
			base.sm.destinationWorld.Set(asteroidWorldIdAtLocation, this);
			GoTo(base.sm.travel);
		}

		public void StartLand()
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(base.sm.destinationWorld.Get(this));
			Vector3 position = Grid.CellToPosCBC(ClusterManager.Instance.GetRandomSurfaceCell(world.id, base.def.landerWidth), animController.sceneLayer);
			base.transform.SetPosition(position);
		}

		public bool UpdateLanding(float dt)
		{
			if (base.gameObject.GetMyWorld() != null)
			{
				Vector3 position = base.transform.GetPosition();
				position.y -= 0.5f;
				int cell = Grid.PosToCell(position);
				if (Grid.IsWorldValidCell(cell) && Grid.IsSolidCell(cell))
				{
					return true;
				}
			}
			return false;
		}

		public void MoveToSpace()
		{
			Pickupable component = GetComponent<Pickupable>();
			if (component != null)
			{
				component.deleteOffGrid = false;
			}
			base.gameObject.transform.SetPosition(new Vector3(-1f, -1f, Grid.GetLayerZ(animController.sceneLayer)));
		}

		public void MoveToWorld()
		{
			Pickupable component = GetComponent<Pickupable>();
			if (component != null)
			{
				component.deleteOffGrid = true;
			}
		}

		public void ResetAnimPosition()
		{
			animController.Offset = Vector3.up * flightAnimOffset;
		}

		public void LandingUpdate(float dt)
		{
			flightAnimOffset = Mathf.Max(flightAnimOffset - dt * base.def.landingSpeed, 0f);
			ResetAnimPosition();
		}

		public void DoLand()
		{
			animController.Offset = Vector3.zero;
			OccupyArea component = base.smi.GetComponent<OccupyArea>();
			if (component != null)
			{
				component.ApplyToCells = true;
			}
			if (base.def.deployOnLanding && CheckIfLoaded())
			{
				base.sm.emptyCargo.Trigger(this);
			}
		}

		public bool CheckIfLoaded()
		{
			bool flag = false;
			MinionStorage component = GetComponent<MinionStorage>();
			if (component != null)
			{
				flag |= component.GetStoredMinionInfo().Count > 0;
			}
			Storage component2 = GetComponent<Storage>();
			if (component2 != null && !component2.IsEmpty())
			{
				flag = true;
			}
			if (flag != base.sm.hasCargo.Get(this))
			{
				base.sm.hasCargo.Set(flag, this);
			}
			return flag;
		}
	}

	public IntParameter destinationWorld = new IntParameter(-1);

	public BoolParameter isLanding = new BoolParameter(default_value: false);

	public BoolParameter isLanded = new BoolParameter(default_value: false);

	public BoolParameter hasCargo = new BoolParameter(default_value: false);

	public Signal emptyCargo;

	public State init;

	public TravelStates travel;

	public LandingStates landing;

	public GroundedStates grounded;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = init;
		base.serializable = SerializeType.ParamsOnly;
		root.InitializeOperationalFlag(RocketModule.landedFlag).Enter(delegate(StatesInstance smi)
		{
			smi.CheckIfLoaded();
		}).EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
		{
			smi.CheckIfLoaded();
		});
		init.ParamTransition(isLanding, landing.landing, GameStateMachine<TravellingCargoLander, StatesInstance, IStateMachineTarget, Def>.IsTrue).ParamTransition(isLanded, grounded, GameStateMachine<TravellingCargoLander, StatesInstance, IStateMachineTarget, Def>.IsTrue).GoTo(travel);
		travel.DefaultState(travel.travelling).Enter(delegate(StatesInstance smi)
		{
			smi.MoveToSpace();
		}).PlayAnim("idle")
			.ToggleTag(GameTags.EntityInSpace)
			.ToggleMainStatusItem(Db.Get().BuildingStatusItems.InFlight, (StatesInstance smi) => smi.GetComponent<ClusterTraveler>());
		travel.travelling.EventTransition(GameHashes.ClusterDestinationReached, travel.transferWorlds);
		travel.transferWorlds.Enter(delegate(StatesInstance smi)
		{
			smi.StartLand();
		}).GoTo(landing.landing);
		landing.Enter(delegate(StatesInstance smi)
		{
			isLanding.Set(value: true, smi);
		}).Exit(delegate(StatesInstance smi)
		{
			isLanding.Set(value: false, smi);
		});
		landing.landing.PlayAnim("landing", KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
		{
			smi.ResetAnimPosition();
		}).Update(delegate(StatesInstance smi, float dt)
		{
			smi.LandingUpdate(dt);
		}, UpdateRate.SIM_EVERY_TICK)
			.Transition(landing.impact, (StatesInstance smi) => smi.flightAnimOffset <= 0f)
			.Enter(delegate(StatesInstance smi)
			{
				smi.MoveToWorld();
			});
		landing.impact.PlayAnim("grounded_pre").OnAnimQueueComplete(grounded);
		grounded.DefaultState(grounded.loaded).ToggleOperationalFlag(RocketModule.landedFlag).Enter(delegate(StatesInstance smi)
		{
			smi.CheckIfLoaded();
		})
			.Enter(delegate(StatesInstance smi)
			{
				isLanded.Set(value: true, smi);
			});
		grounded.loaded.PlayAnim("grounded").ParamTransition(hasCargo, grounded.empty, GameStateMachine<TravellingCargoLander, StatesInstance, IStateMachineTarget, Def>.IsFalse).OnSignal(emptyCargo, grounded.emptying)
			.Enter(delegate(StatesInstance smi)
			{
				smi.DoLand();
			});
		grounded.emptying.PlayAnim("deploying").TriggerOnEnter(GameHashes.JettisonCargo).OnAnimQueueComplete(grounded.empty);
		grounded.empty.PlayAnim("deployed").ParamTransition(hasCargo, grounded.loaded, GameStateMachine<TravellingCargoLander, StatesInstance, IStateMachineTarget, Def>.IsTrue);
	}
}
