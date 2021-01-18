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
		public State full;
	}

	public class StatesInstance : GameInstance
	{
		private float velocity;

		private Vector3 previousPosition;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void Launch(int dest)
		{
			base.sm.destinationWorld.Set(dest, this);
		}

		public void StartLaunch()
		{
			velocity = 35f;
			if (GameComps.Fallers.Has(base.gameObject))
			{
				GameComps.Fallers.Remove(base.gameObject);
			}
		}

		public void StartLand()
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(base.sm.destinationWorld.Get(this));
			int num = (int)Random.Range(world.minimumBounds.x, world.maximumBounds.x);
			TransformExtensions.SetPosition(position: new Vector2(num, world.Height), transform: base.transform);
			if (GameComps.Fallers.Has(base.gameObject))
			{
				GameComps.Fallers.Remove(base.gameObject);
			}
			GameComps.Fallers.Add(base.gameObject, new Vector2(0f, 10f));
			base.sm.destinationWorld.Set(-1, this);
		}

		public void UpdateLaunch(float dt)
		{
			WorldContainer myWorld = base.gameObject.GetMyWorld();
			if (myWorld != null)
			{
				Vector3 position = base.transform.GetPosition();
				Vector3 position2 = position + new Vector3(0f, velocity * dt, 0f);
				base.transform.SetPosition(position2);
			}
			else
			{
				base.sm.beginTravelling.Trigger(this);
			}
		}

		public void UpdateLanding(float dt)
		{
			WorldContainer myWorld = base.gameObject.GetMyWorld();
			if (myWorld != null)
			{
				Vector3 position = base.transform.GetPosition();
				position.y -= 0.5f;
				int cell = Grid.PosToCell(position);
				if (Grid.IsSolidCell(cell))
				{
					base.sm.onSurface.Set(value: true, this);
				}
			}
		}

		public void OnDroppedAll()
		{
			base.gameObject.DeleteObject();
		}
	}

	public IntParameter destinationWorld = new IntParameter(-1);

	public BoolParameter onSurface = new BoolParameter(default_value: false);

	public Signal beginTravelling;

	public TakeoffStates takeoff;

	public TravelStates travel;

	public LandingStates landing;

	public GroundedStates grounded;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = takeoff;
		base.serializable = SerializeType.Both_DEPRECATED;
		takeoff.DefaultState(takeoff.launch).ParamTransition(destinationWorld, landing, (StatesInstance smi, int p) => p == -1).OnSignal(beginTravelling, travel);
		takeoff.launch.Enter(delegate(StatesInstance smi)
		{
			smi.StartLaunch();
		}).GoTo(takeoff.airborne);
		takeoff.airborne.Update("Launch", delegate(StatesInstance smi, float dt)
		{
			smi.UpdateLaunch(dt);
		});
		travel.DefaultState(travel.travelling);
		travel.travelling.GoTo(travel.transferWorlds);
		travel.transferWorlds.Enter(delegate(StatesInstance smi)
		{
			smi.StartLand();
		}).GoTo(landing.landing);
		landing.DefaultState(landing.landing).ParamTransition(onSurface, grounded, GameStateMachine<RailGunPayload, StatesInstance, IStateMachineTarget, Def>.IsTrue).ParamTransition(destinationWorld, takeoff, (StatesInstance smi, int p) => p != -1);
		landing.landing.Update("Landing", delegate(StatesInstance smi, float dt)
		{
			smi.UpdateLanding(dt);
		}).ToggleGravity(landing.impact);
		landing.impact.Enter(delegate(StatesInstance smi)
		{
			onSurface.Set(value: true, smi);
		});
		grounded.DefaultState(grounded.full).EventHandler(GameHashes.DroppedAll, delegate(StatesInstance smi)
		{
			smi.OnDroppedAll();
		});
		grounded.full.ToggleMainStatusItem(Db.Get().BuildingStatusItems.RailgunpayloadNeedsEmptying).ToggleTag(GameTags.RailGunPayloadEmptyable);
	}
}
