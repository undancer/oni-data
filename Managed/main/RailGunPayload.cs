using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RailGunPayload : GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>
{
	public class Def : BaseDef
	{
		public bool attractToBeacons;

		public string clusterAnimSymbolSwapTarget;

		public List<string> randomClusterSymbolSwaps;

		public string worldAnimSymbolSwapTarget;

		public List<string> randomWorldSymbolSwaps;
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

		[Serialize]
		private int randomSymbolSwapIndex = -1;

		public KBatchedAnimController animController;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			animController = GetComponent<KBatchedAnimController>();
			DebugUtil.Assert(def.clusterAnimSymbolSwapTarget == null == (def.worldAnimSymbolSwapTarget == null), "Must specify both or neither symbol swap targets!");
			DebugUtil.Assert((def.randomClusterSymbolSwaps == null && def.randomWorldSymbolSwaps == null) || def.randomClusterSymbolSwaps.Count == def.randomWorldSymbolSwaps.Count, "Must specify the same number of swaps for both world and cluster!");
			if (def.clusterAnimSymbolSwapTarget != null && def.worldAnimSymbolSwapTarget != null)
			{
				if (randomSymbolSwapIndex == -1)
				{
					randomSymbolSwapIndex = Random.Range(0, def.randomClusterSymbolSwaps.Count);
					Debug.Log($"Rolling a random symbol: {randomSymbolSwapIndex}", base.gameObject);
				}
				GetComponent<BallisticClusterGridEntity>().SwapSymbolFromSameAnim(def.clusterAnimSymbolSwapTarget, def.randomClusterSymbolSwaps[randomSymbolSwapIndex]);
				KAnim.Build.Symbol symbol = animController.AnimFiles[0].GetData().build.GetSymbol(def.randomWorldSymbolSwaps[randomSymbolSwapIndex]);
				animController.GetComponent<SymbolOverrideController>().AddSymbolOverride(def.worldAnimSymbolSwapTarget, symbol);
			}
		}

		public void Launch(AxialI source, AxialI destination)
		{
			GetComponent<BallisticClusterGridEntity>().Configure(source, destination);
			int asteroidWorldIdAtLocation = ClusterUtil.GetAsteroidWorldIdAtLocation(destination);
			base.sm.destinationWorld.Set(asteroidWorldIdAtLocation, this);
			GoTo(base.sm.takeoff);
		}

		public void Travel(AxialI source, AxialI destination)
		{
			GetComponent<BallisticClusterGridEntity>().Configure(source, destination);
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
			int num2 = Grid.InvalidCell;
			if (base.def.attractToBeacons)
			{
				num2 = ClusterManager.Instance.GetLandingBeaconLocation(world.id);
			}
			if (num2 != Grid.InvalidCell)
			{
				Grid.CellToXY(num2, out var x, out var _);
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
				return;
			}
			base.sm.beginTravelling.Trigger(this);
			ClusterGridEntity component = GetComponent<ClusterGridEntity>();
			if (ClusterGrid.Instance.GetAsteroidAtCell(component.Location) != null)
			{
				GetComponent<ClusterTraveler>().AdvancePathOneStep();
			}
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
			Pickupable component = GetComponent<Pickupable>();
			if (component != null)
			{
				component.deleteOffGrid = false;
			}
			base.gameObject.transform.SetPosition(new Vector3(-1f, -1f, 0f));
		}

		public void MoveToWorld()
		{
			Pickupable component = GetComponent<Pickupable>();
			if (component != null)
			{
				component.deleteOffGrid = true;
			}
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
			.ToggleTag(GameTags.ClusterEntityGrounded)
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
			.OnSignal(beginTravelling, travel);
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
		}).Enter(delegate(StatesInstance smi)
		{
			smi.MoveToSpace();
		})
			.PlayAnim("idle")
			.ToggleTag(GameTags.EntityInSpace)
			.ToggleMainStatusItem(Db.Get().BuildingStatusItems.InFlight, (StatesInstance smi) => smi.GetComponent<ClusterTraveler>());
		travel.travelling.EventTransition(GameHashes.ClusterDestinationReached, travel.transferWorlds);
		travel.transferWorlds.Enter(delegate(StatesInstance smi)
		{
			smi.StartLand();
		}).GoTo(landing.landing);
		landing.DefaultState(landing.landing).ParamTransition(onSurface, grounded.crater, GameStateMachine<RailGunPayload, StatesInstance, IStateMachineTarget, Def>.IsTrue).ParamTransition(destinationWorld, takeoff, (StatesInstance smi, int p) => p != -1)
			.Enter(delegate(StatesInstance smi)
			{
				smi.MoveToWorld();
			});
		landing.landing.PlayAnim("falling", KAnim.PlayMode.Loop).UpdateTransition(landing.impact, (StatesInstance smi, float dt) => smi.UpdateLanding(dt)).ToggleGravity(landing.impact);
		landing.impact.PlayAnim("land").TriggerOnEnter(GameHashes.JettisonCargo).OnAnimQueueComplete(grounded.crater);
	}
}
