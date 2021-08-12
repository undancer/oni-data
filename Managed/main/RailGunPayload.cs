using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RailGunPayload : GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>
{
	public class Def : BaseDef
	{
	}

	public class TakeoffStates : State
	{
		public State launch;

		public State airborne;
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
		public State crater;

		public State idle;
	}

	public class StatesInstance : GameInstance
	{
		[Serialize]
		public float takeoffVelocity;

		public KAnimControllerBase animController;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			animController = GetComponent<KAnimControllerBase>();
		}

		public void Launch(AxialI source, AxialI destination)
		{
			GetComponent<RailgunPayloadClusterGridEntity>().Configure(source, destination);
			int asteroidWorldIdAtLocation = ClusterUtil.GetAsteroidWorldIdAtLocation(destination);
			base.sm.destinationWorld.Set(asteroidWorldIdAtLocation, this);
			GoTo(base.sm.takeoff);
		}

		public void Land(AxialI source, AxialI destination)
		{
			GetComponent<RailgunPayloadClusterGridEntity>().Configure(source, destination);
			int asteroidWorldIdAtLocation = ClusterUtil.GetAsteroidWorldIdAtLocation(destination);
			base.sm.destinationWorld.Set(asteroidWorldIdAtLocation, this);
			GoTo(base.sm.travel);
		}

		public void StartTakeoff()
		{
			if (GameComps.Fallers.Has(base.gameObject))
			{
				GameComps.Fallers.Remove(base.gameObject);
			}
		}

		public void StartLand()
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(base.sm.destinationWorld.Get(this));
			int num = 0;
			int landingBeaconLocation = ClusterManager.Instance.GetLandingBeaconLocation(world.id);
			Grid.CellToXY(landingBeaconLocation, out var x, out var _);
			if (landingBeaconLocation != Grid.InvalidCell)
			{
				int minInclusive = Mathf.Max(x - 3, (int)world.minimumBounds.x);
				int maxExclusive = Mathf.Min(x + 3, (int)world.maximumBounds.x);
				num = Mathf.RoundToInt(Random.Range(minInclusive, maxExclusive));
			}
			else
			{
				num = Mathf.RoundToInt(Random.Range(world.minimumBounds.x + 3f, world.maximumBounds.x - 3f));
			}
			TransformExtensions.SetPosition(position: new Vector3((float)num + 0.5f, world.maximumBounds.y - 1f, Grid.GetLayerZ(Grid.SceneLayer.Front)), transform: base.transform);
			if (GameComps.Fallers.Has(base.gameObject))
			{
				GameComps.Fallers.Remove(base.gameObject);
			}
			GameComps.Fallers.Add(base.gameObject, new Vector2(0f, -10f));
			base.sm.destinationWorld.Set(-1, this);
		}

		public void UpdateLaunch(float dt)
		{
			if (base.gameObject.GetMyWorld() != null)
			{
				Vector3 position = base.transform.GetPosition() + new Vector3(0f, takeoffVelocity * dt, 0f);
				base.transform.SetPosition(position);
			}
			else
			{
				base.sm.beginTravelling.Trigger(this);
				GetComponent<ClusterTraveler>().AdvancePathOneStep();
			}
		}

		public void UpdateLanding(float dt)
		{
			if (base.gameObject.GetMyWorld() != null)
			{
				Vector3 position = base.transform.GetPosition();
				position.y -= 0.5f;
				int cell = Grid.PosToCell(position);
				if (Grid.IsWorldValidCell(cell) && Grid.IsSolidCell(cell))
				{
					base.sm.onSurface.Set(value: true, this);
				}
			}
		}

		public void OnDroppedAll()
		{
			base.gameObject.DeleteObject();
		}

		public bool IsTraveling()
		{
			return IsInsideState(base.sm.travel.travelling);
		}

		public void MoveToSpace()
		{
			base.gameObject.transform.SetPosition(new Vector3(-1f, -1f, 0f));
		}

		public void MoveToWorld()
		{
			GetComponent<Pickupable>().deleteOffGrid = true;
		}
	}

	public IntParameter destinationWorld = new IntParameter(-1);

	public BoolParameter onSurface = new BoolParameter(default_value: false);

	public Signal beginTravelling;

	public Signal launch;

	public TakeoffStates takeoff;

	public TravelStates travel;

	public LandingStates landing;

	public GroundedStates grounded;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = grounded.idle;
		base.serializable = SerializeType.Both_DEPRECATED;
		grounded.DefaultState(grounded.idle).Enter(delegate(StatesInstance smi)
		{
			onSurface.Set(value: true, smi);
		}).ToggleMainStatusItem(Db.Get().BuildingStatusItems.RailgunpayloadNeedsEmptying)
			.ToggleTag(GameTags.RailGunPayloadEmptyable)
			.EventHandler(GameHashes.DroppedAll, delegate(StatesInstance smi)
			{
				smi.OnDroppedAll();
			})
			.OnSignal(launch, takeoff);
		grounded.idle.PlayAnim("idle");
		grounded.crater.Enter(delegate(StatesInstance smi)
		{
			smi.animController.randomiseLoopedOffset = true;
		}).Exit(delegate(StatesInstance smi)
		{
			smi.animController.randomiseLoopedOffset = false;
		}).PlayAnim("landed", KAnim.PlayMode.Loop)
			.EventTransition(GameHashes.OnStore, grounded.idle);
		takeoff.DefaultState(takeoff.launch).Enter(delegate(StatesInstance smi)
		{
			onSurface.Set(value: false, smi);
		}).PlayAnim("launching")
			.OnSignal(beginTravelling, travel)
			.Enter(delegate(StatesInstance smi)
			{
				smi.GetComponent<Pickupable>().deleteOffGrid = false;
			});
		takeoff.launch.Enter(delegate(StatesInstance smi)
		{
			smi.StartTakeoff();
		}).GoTo(takeoff.airborne);
		takeoff.airborne.Update("Launch", delegate(StatesInstance smi, float dt)
		{
			smi.UpdateLaunch(dt);
		}, UpdateRate.SIM_EVERY_TICK);
		travel.DefaultState(travel.travelling).Enter(delegate(StatesInstance smi)
		{
			onSurface.Set(value: false, smi);
		}).PlayAnim("idle")
			.ToggleTag(GameTags.EntityInSpace)
			.ToggleMainStatusItem(Db.Get().BuildingStatusItems.InFlight, (StatesInstance smi) => smi.GetComponent<ClusterTraveler>());
		travel.travelling.EventTransition(GameHashes.ClusterDestinationReached, travel.transferWorlds).Enter(delegate(StatesInstance smi)
		{
			smi.MoveToSpace();
		});
		travel.transferWorlds.Enter(delegate(StatesInstance smi)
		{
			smi.StartLand();
		}).GoTo(landing.landing);
		landing.DefaultState(landing.landing).ParamTransition(onSurface, grounded.crater, GameStateMachine<RailGunPayload, StatesInstance, IStateMachineTarget, Def>.IsTrue).ParamTransition(destinationWorld, takeoff, (StatesInstance smi, int p) => p != -1);
		landing.landing.PlayAnim("falling", KAnim.PlayMode.Loop).Update("Landing", delegate(StatesInstance smi, float dt)
		{
			smi.UpdateLanding(dt);
		}).ToggleGravity(landing.impact);
		landing.impact.PlayAnim("land").OnAnimQueueComplete(grounded.crater);
	}
}
