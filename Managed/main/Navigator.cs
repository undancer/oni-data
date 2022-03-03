using System;
using System.Collections.Generic;
using System.IO;
using STRINGS;
using UnityEngine;

public class Navigator : StateMachineComponent<Navigator.StatesInstance>, ISaveLoadableDetails
{
	public class ActiveTransition
	{
		public int x;

		public int y;

		public bool isLooping;

		public NavType start;

		public NavType end;

		public HashedString preAnim;

		public HashedString anim;

		public float speed;

		public float animSpeed = 1f;

		public Func<bool> isCompleteCB;

		public NavGrid.Transition navGridTransition;

		public ActiveTransition(NavGrid.Transition transition, float default_speed)
		{
			x = transition.x;
			y = transition.y;
			isLooping = transition.isLooping;
			start = transition.start;
			end = transition.end;
			preAnim = transition.preAnim;
			anim = transition.anim;
			speed = default_speed;
			animSpeed = transition.animSpeed;
			navGridTransition = transition;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, Navigator, object>.GameInstance
	{
		public StatesInstance(Navigator master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Navigator>
	{
		public class NormalStates : State
		{
			public State moving;

			public State arrived;

			public State failed;

			public State stopped;
		}

		public TargetParameter moveTarget;

		public BoolParameter isPaused = new BoolParameter(default_value: false);

		public NormalStates normal;

		public State paused;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = normal.stopped;
			saveHistory = true;
			normal.ParamTransition(isPaused, paused, GameStateMachine<States, StatesInstance, Navigator, object>.IsTrue).Update("NavigatorProber", delegate(StatesInstance smi, float dt)
			{
				smi.master.Sim4000ms(dt);
			}, UpdateRate.SIM_4000ms);
			normal.moving.Enter(delegate(StatesInstance smi)
			{
				smi.Trigger(1027377649, GameHashes.ObjectMovementWakeUp);
			}).Update("UpdateNavigator", delegate(StatesInstance smi, float dt)
			{
				smi.master.SimEveryTick(dt);
			}, UpdateRate.SIM_EVERY_TICK, load_balance: true).Exit(delegate(StatesInstance smi)
			{
				smi.Trigger(1027377649, GameHashes.ObjectMovementSleep);
			});
			normal.arrived.TriggerOnEnter(GameHashes.DestinationReached).GoTo(normal.stopped);
			normal.failed.TriggerOnEnter(GameHashes.NavigationFailed).GoTo(normal.stopped);
			normal.stopped.DoNothing();
			paused.ParamTransition(isPaused, normal, GameStateMachine<States, StatesInstance, Navigator, object>.IsFalse);
		}
	}

	public struct PathProbeTask : IWorkItem<object>
	{
		private int cell;

		private Navigator navigator;

		public PathProbeTask(Navigator navigator)
		{
			this.navigator = navigator;
			cell = -1;
		}

		public void Update()
		{
			cell = Grid.PosToCell(navigator);
			navigator.abilities.Refresh();
		}

		public void Run(object sharedData)
		{
			navigator.PathProber.UpdateProbe(navigator.NavGrid, cell, navigator.CurrentNavType, navigator.abilities, navigator.flags);
		}
	}

	public bool DebugDrawPath;

	[MyCmpAdd]
	public PathProber PathProber;

	[MyCmpAdd]
	private Facing facing;

	[MyCmpGet]
	public AnimEventHandler animEventHandler;

	public float defaultSpeed = 1f;

	public TransitionDriver transitionDriver;

	public string NavGridName;

	public bool updateProber;

	public int maxProbingRadius;

	public PathFinder.PotentialPath.Flags flags;

	private LoggerFSS log;

	public Dictionary<NavType, int> distanceTravelledByNavType;

	public Grid.SceneLayer sceneLayer = Grid.SceneLayer.Move;

	private PathFinderAbilities abilities;

	[MyCmpReq]
	private KSelectable selectable;

	[NonSerialized]
	public PathFinder.Path path;

	public NavType CurrentNavType;

	private int AnchorCell;

	private KPrefabID targetLocator;

	private int reservedCell = NavigationReservations.InvalidReservation;

	private NavTactic tactic;

	public PathProbeTask pathProbeTask;

	private static readonly EventSystem.IntraObjectHandler<Navigator> OnDefeatedDelegate = new EventSystem.IntraObjectHandler<Navigator>(delegate(Navigator component, object data)
	{
		component.OnDefeated(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Navigator> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Navigator>(delegate(Navigator component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Navigator> OnSelectObjectDelegate = new EventSystem.IntraObjectHandler<Navigator>(delegate(Navigator component, object data)
	{
		component.OnSelectObject(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Navigator> OnStoreDelegate = new EventSystem.IntraObjectHandler<Navigator>(delegate(Navigator component, object data)
	{
		component.OnStore(data);
	});

	public bool executePathProbeTaskAsync;

	public KMonoBehaviour target { get; set; }

	public CellOffset[] targetOffsets { get; private set; }

	public NavGrid NavGrid { get; private set; }

	public void Serialize(BinaryWriter writer)
	{
		byte currentNavType = (byte)CurrentNavType;
		writer.Write(currentNavType);
		writer.Write(distanceTravelledByNavType.Count);
		foreach (KeyValuePair<NavType, int> item in distanceTravelledByNavType)
		{
			byte key = (byte)item.Key;
			writer.Write(key);
			writer.Write(item.Value);
		}
	}

	public void Deserialize(IReader reader)
	{
		NavType navType = (NavType)reader.ReadByte();
		if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 11))
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				NavType key = (NavType)reader.ReadByte();
				int value = reader.ReadInt32();
				if (distanceTravelledByNavType.ContainsKey(key))
				{
					distanceTravelledByNavType[key] = value;
				}
			}
		}
		bool flag = false;
		NavType[] validNavTypes = NavGrid.ValidNavTypes;
		for (int j = 0; j < validNavTypes.Length; j++)
		{
			if (validNavTypes[j] == navType)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			CurrentNavType = navType;
		}
	}

	protected override void OnPrefabInit()
	{
		transitionDriver = new TransitionDriver(this);
		targetLocator = Util.KInstantiate(Assets.GetPrefab(TargetLocator.ID)).GetComponent<KPrefabID>();
		targetLocator.gameObject.SetActive(value: true);
		log = new LoggerFSS("Navigator");
		simRenderLoadBalance = true;
		autoRegisterSimRender = false;
		NavGrid = Pathfinding.Instance.GetNavGrid(NavGridName);
		GetComponent<PathProber>().SetValidNavTypes(NavGrid.ValidNavTypes, maxProbingRadius);
		distanceTravelledByNavType = new Dictionary<NavType, int>();
		for (int i = 0; i < 11; i++)
		{
			distanceTravelledByNavType.Add((NavType)i, 0);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(1623392196, OnDefeatedDelegate);
		Subscribe(-1506500077, OnDefeatedDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-1503271301, OnSelectObjectDelegate);
		Subscribe(856640610, OnStoreDelegate);
		if (updateProber)
		{
			SimAndRenderScheduler.instance.Add(this);
		}
		pathProbeTask = new PathProbeTask(this);
		SetCurrentNavType(CurrentNavType);
	}

	public bool IsMoving()
	{
		return base.smi.IsInsideState(base.smi.sm.normal.moving);
	}

	public bool GoTo(int cell, CellOffset[] offsets = null)
	{
		if (offsets == null)
		{
			offsets = new CellOffset[1];
		}
		targetLocator.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
		return GoTo(targetLocator, offsets, NavigationTactics.ReduceTravelDistance);
	}

	public bool GoTo(int cell, CellOffset[] offsets, NavTactic tactic)
	{
		if (offsets == null)
		{
			offsets = new CellOffset[1];
		}
		targetLocator.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
		return GoTo(targetLocator, offsets, tactic);
	}

	public void UpdateTarget(int cell)
	{
		targetLocator.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
	}

	public bool GoTo(KMonoBehaviour target, CellOffset[] offsets, NavTactic tactic)
	{
		if (tactic == null)
		{
			tactic = NavigationTactics.ReduceTravelDistance;
		}
		base.smi.GoTo(base.smi.sm.normal.moving);
		base.smi.sm.moveTarget.Set(target.gameObject, base.smi);
		this.tactic = tactic;
		this.target = target;
		targetOffsets = offsets;
		ClearReservedCell();
		AdvancePath();
		return IsMoving();
	}

	public void BeginTransition(NavGrid.Transition transition)
	{
		transitionDriver.EndTransition();
		base.smi.GoTo(base.smi.sm.normal.moving);
		ActiveTransition transition2 = new ActiveTransition(transition, defaultSpeed);
		transitionDriver.BeginTransition(this, transition2);
	}

	private bool ValidatePath(ref PathFinder.Path path)
	{
		PathFinderAbilities currentAbilities = GetCurrentAbilities();
		return PathFinder.ValidatePath(NavGrid, currentAbilities, ref path);
	}

	public void AdvancePath(bool trigger_advance = true)
	{
		int num = Grid.PosToCell(this);
		if (target == null)
		{
			Trigger(-766531887);
			Stop();
		}
		else if (num == reservedCell && CurrentNavType != NavType.Tube)
		{
			Stop(arrived_at_destination: true);
		}
		else
		{
			bool flag = false;
			int num2 = Grid.PosToCell(target);
			if (reservedCell == NavigationReservations.InvalidReservation)
			{
				flag = true;
			}
			else if (!CanReach(reservedCell))
			{
				flag = true;
			}
			else if (!Grid.IsCellOffsetOf(reservedCell, num2, targetOffsets))
			{
				flag = true;
			}
			else if (path.IsValid())
			{
				if (num == path.nodes[0].cell && CurrentNavType == path.nodes[0].navType)
				{
					flag = !ValidatePath(ref path);
				}
				else if (num == path.nodes[1].cell && CurrentNavType == path.nodes[1].navType)
				{
					path.nodes.RemoveAt(0);
					flag = !ValidatePath(ref path);
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				int cellPreferences = tactic.GetCellPreferences(num2, targetOffsets, this);
				SetReservedCell(cellPreferences);
				if (reservedCell != NavigationReservations.InvalidReservation)
				{
					PathFinder.UpdatePath(potential_path: new PathFinder.PotentialPath(num, CurrentNavType, flags), nav_grid: NavGrid, abilities: GetCurrentAbilities(), query: PathFinderQueries.cellQuery.Reset(reservedCell), path: ref path);
				}
				else
				{
					Stop();
				}
			}
			if (path.IsValid())
			{
				BeginTransition(NavGrid.transitions[path.nodes[1].transitionId]);
				distanceTravelledByNavType[CurrentNavType] = Mathf.Max(distanceTravelledByNavType[CurrentNavType] + 1, distanceTravelledByNavType[CurrentNavType]);
			}
			else if (path.HasArrived())
			{
				Stop(arrived_at_destination: true);
			}
			else
			{
				ClearReservedCell();
				Stop();
			}
		}
		if (trigger_advance)
		{
			Trigger(1347184327);
		}
	}

	public NavGrid.Transition GetNextTransition()
	{
		return NavGrid.transitions[path.nodes[1].transitionId];
	}

	public void Stop(bool arrived_at_destination = false, bool play_idle = true)
	{
		target = null;
		targetOffsets = null;
		path.Clear();
		base.smi.sm.moveTarget.Set(null, base.smi);
		transitionDriver.EndTransition();
		if (play_idle)
		{
			HashedString idleAnim = NavGrid.GetIdleAnim(CurrentNavType);
			GetComponent<KAnimControllerBase>().Play(idleAnim, KAnim.PlayMode.Loop);
		}
		if (arrived_at_destination)
		{
			base.smi.GoTo(base.smi.sm.normal.arrived);
		}
		else if (base.smi.GetCurrentState() == base.smi.sm.normal.moving)
		{
			ClearReservedCell();
			base.smi.GoTo(base.smi.sm.normal.failed);
		}
	}

	private void SimEveryTick(float dt)
	{
		if (IsMoving())
		{
			transitionDriver.UpdateTransition(dt);
		}
	}

	public void Sim4000ms(float dt)
	{
		UpdateProbe(forceUpdate: true);
	}

	public void UpdateProbe(bool forceUpdate = false)
	{
		if (forceUpdate || !executePathProbeTaskAsync)
		{
			pathProbeTask.Update();
			pathProbeTask.Run(null);
		}
	}

	public void DrawPath()
	{
		if (base.gameObject.activeInHierarchy && IsMoving())
		{
			NavPathDrawer.Instance.DrawPath(GetComponent<KAnimControllerBase>().GetPivotSymbolPosition(), path);
		}
	}

	public void Pause(string reason)
	{
		base.smi.sm.isPaused.Set(value: true, base.smi);
	}

	public void Unpause(string reason)
	{
		base.smi.sm.isPaused.Set(value: false, base.smi);
	}

	private void OnDefeated(object data)
	{
		ClearReservedCell();
		Stop(arrived_at_destination: false, play_idle: false);
	}

	private void ClearReservedCell()
	{
		if (reservedCell != NavigationReservations.InvalidReservation)
		{
			NavigationReservations.Instance.RemoveOccupancy(reservedCell);
			reservedCell = NavigationReservations.InvalidReservation;
		}
	}

	private void SetReservedCell(int cell)
	{
		ClearReservedCell();
		reservedCell = cell;
		NavigationReservations.Instance.AddOccupancy(cell);
	}

	public int GetReservedCell()
	{
		return reservedCell;
	}

	public int GetAnchorCell()
	{
		return AnchorCell;
	}

	public bool IsValidNavType(NavType nav_type)
	{
		return NavGrid.HasNavTypeData(nav_type);
	}

	public void SetCurrentNavType(NavType nav_type)
	{
		CurrentNavType = nav_type;
		AnchorCell = NavTypeHelper.GetAnchorCell(nav_type, Grid.PosToCell(this));
		NavGrid.NavTypeData navTypeData = NavGrid.GetNavTypeData(CurrentNavType);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		Vector2 one = Vector2.one;
		if (navTypeData.flipX)
		{
			one.x = -1f;
		}
		if (navTypeData.flipY)
		{
			one.y = -1f;
		}
		component.navMatrix = Matrix2x3.Translate(navTypeData.animControllerOffset * 200f) * Matrix2x3.Rotate(navTypeData.rotation) * Matrix2x3.Scale(one);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!base.gameObject.HasTag(GameTags.Dead))
		{
			KIconButtonMenu.ButtonInfo button = ((NavPathDrawer.Instance.GetNavigator() != this) ? new KIconButtonMenu.ButtonInfo("action_navigable_regions", UI.USERMENUACTIONS.DRAWPATHS.NAME, OnDrawPaths, Action.NumActions, null, null, null, UI.USERMENUACTIONS.DRAWPATHS.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_navigable_regions", UI.USERMENUACTIONS.DRAWPATHS.NAME_OFF, OnDrawPaths, Action.NumActions, null, null, null, UI.USERMENUACTIONS.DRAWPATHS.TOOLTIP_OFF));
			Game.Instance.userMenu.AddButton(base.gameObject, button, 0.1f);
			Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_follow_cam", UI.USERMENUACTIONS.FOLLOWCAM.NAME, OnFollowCam, Action.NumActions, null, null, null, UI.USERMENUACTIONS.FOLLOWCAM.TOOLTIP), 0.3f);
		}
	}

	private void OnFollowCam()
	{
		if (CameraController.Instance.followTarget == base.transform)
		{
			CameraController.Instance.ClearFollowTarget();
		}
		else
		{
			CameraController.Instance.SetFollowTarget(base.transform);
		}
	}

	private void OnDrawPaths()
	{
		if (NavPathDrawer.Instance.GetNavigator() != this)
		{
			NavPathDrawer.Instance.SetNavigator(this);
		}
		else
		{
			NavPathDrawer.Instance.ClearNavigator();
		}
	}

	private void OnSelectObject(object data)
	{
		NavPathDrawer.Instance.ClearNavigator();
	}

	public void OnStore(object data)
	{
		if (data is Storage || (data != null && (bool)data))
		{
			Stop();
		}
	}

	public PathFinderAbilities GetCurrentAbilities()
	{
		abilities.Refresh();
		return abilities;
	}

	public void SetAbilities(PathFinderAbilities abilities)
	{
		this.abilities = abilities;
	}

	public bool CanReach(IApproachable approachable)
	{
		return CanReach(approachable.GetCell(), approachable.GetOffsets());
	}

	public bool CanReach(int cell, CellOffset[] offsets)
	{
		foreach (CellOffset offset in offsets)
		{
			int cell2 = Grid.OffsetCell(cell, offset);
			if (CanReach(cell2))
			{
				return true;
			}
		}
		return false;
	}

	public bool CanReach(int cell)
	{
		return GetNavigationCost(cell) != -1;
	}

	public int GetNavigationCost(int cell)
	{
		if (Grid.IsValidCell(cell))
		{
			return PathProber.GetCost(cell);
		}
		return -1;
	}

	public int GetNavigationCostIgnoreProberOffset(int cell, CellOffset[] offsets)
	{
		return PathProber.GetNavigationCostIgnoreProberOffset(cell, offsets);
	}

	public int GetNavigationCost(int cell, CellOffset[] offsets)
	{
		int num = -1;
		int num2 = offsets.Length;
		for (int i = 0; i < num2; i++)
		{
			int cell2 = Grid.OffsetCell(cell, offsets[i]);
			int navigationCost = GetNavigationCost(cell2);
			if (navigationCost != -1 && (num == -1 || navigationCost < num))
			{
				num = navigationCost;
			}
		}
		return num;
	}

	public int GetNavigationCost(IApproachable approachable)
	{
		return GetNavigationCost(approachable.GetCell(), approachable.GetOffsets());
	}

	public void RunQuery(PathFinderQuery query)
	{
		int cell = Grid.PosToCell(this);
		PathFinder.Run(potential_path: new PathFinder.PotentialPath(cell, CurrentNavType, flags), nav_grid: NavGrid, abilities: GetCurrentAbilities(), query: query);
	}

	public void SetFlags(PathFinder.PotentialPath.Flags new_flags)
	{
		flags |= new_flags;
	}

	public void ClearFlags(PathFinder.PotentialPath.Flags new_flags)
	{
		flags &= (PathFinder.PotentialPath.Flags)(byte)(~(int)new_flags);
	}
}
