using UnityEngine;

public class JettisonableCargoModule : GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>
{
	public class Def : BaseDef
	{
		public DefComponent<Storage> landerContainer;

		public Tag landerPrefabID;

		public Vector3 cargoDropOffset;

		public string clusterMapFXPrefabID;
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
		private Storage landerContainer;

		private bool landerPlaced;

		private MinionIdentity chosenDuplicant;

		private int landerPlacementCell;

		public GameObject clusterMapFX;

		public bool AutoDeploy { get; set; }

		public bool CanAutoDeploy => false;

		public bool ChooseDuplicant
		{
			get
			{
				GameObject gameObject = landerContainer.FindFirst(base.def.landerPrefabID);
				if (gameObject == null)
				{
					return false;
				}
				return gameObject.GetComponent<MinionStorage>() != null;
			}
		}

		public MinionIdentity ChosenDuplicant
		{
			get
			{
				return chosenDuplicant;
			}
			set
			{
				chosenDuplicant = value;
			}
		}

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			landerContainer = def.landerContainer.Get(this);
		}

		private void ChooseLanderLocation()
		{
			ClusterGridEntity stableOrbitAsteroid = base.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().GetStableOrbitAsteroid();
			if (stableOrbitAsteroid != null)
			{
				WorldContainer component = stableOrbitAsteroid.GetComponent<WorldContainer>();
				Placeable component2 = landerContainer.FindFirst(base.def.landerPrefabID).GetComponent<Placeable>();
				component2.restrictWorldId = component.id;
				component.LookAtSurface();
				ClusterManager.Instance.SetActiveWorld(component.id);
				ManagementMenu.Instance.CloseAll();
				PlaceTool.Instance.Activate(component2, OnLanderPlaced);
			}
		}

		private void OnLanderPlaced(Placeable lander, int cell)
		{
			landerPlaced = true;
			if (lander.GetComponent<MinionStorage>() != null)
			{
				OpenMoveChoreForChosenDuplicant();
			}
			landerPlacementCell = cell;
			ManagementMenu.Instance.ToggleClusterMap();
			base.sm.emptyCargo.Trigger(base.smi);
			ClusterMapScreen.Instance.SelectEntity(GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<ClusterGridEntity>(), frameDelay: true);
		}

		private void OpenMoveChoreForChosenDuplicant()
		{
			RocketModuleCluster component = base.master.GetComponent<RocketModuleCluster>();
			Clustercraft craft = component.CraftInterface.GetComponent<Clustercraft>();
			ClustercraftInteriorDoor interiorDoor = craft.ModuleInterface.GetPassengerModule().GetComponent<ClustercraftExteriorDoor>().GetInteriorDoor();
			int cell = Grid.OffsetCell(Grid.PosToCell(interiorDoor), interiorDoor.GetComponent<NavTeleporter>().offset);
			MinionStorage storage = landerContainer.FindFirst(base.def.landerPrefabID).GetComponent<MinionStorage>();
			ChosenDuplicant.GetSMI<RocketPassengerMonitor.Instance>().SetModuleDeployChore(cell, delegate
			{
				Game.Instance.assignmentManager.RemoveFromWorld(ChosenDuplicant.assignableProxy.Get(), craft.ModuleInterface.GetInteriorWorld().id);
				craft.ModuleInterface.GetPassengerModule().RemoveRocketPassenger(ChosenDuplicant);
				storage.SerializeMinion(ChosenDuplicant.gameObject);
			});
		}

		public void FinalDeploy()
		{
			landerPlaced = false;
			Placeable component = landerContainer.FindFirst(base.def.landerPrefabID).GetComponent<Placeable>();
			landerContainer.FindFirst(base.def.landerPrefabID);
			landerContainer.Drop(component.gameObject);
			TreeFilterable component2 = GetComponent<TreeFilterable>();
			TreeFilterable component3 = component.GetComponent<TreeFilterable>();
			if (component3 != null)
			{
				component3.UpdateFilters(component2.AcceptedTags);
			}
			Storage component4 = component.GetComponent<Storage>();
			if (component4 != null)
			{
				Storage[] components = base.gameObject.GetComponents<Storage>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].Transfer(component4, block_events: false, hide_popups: true);
				}
			}
			Vector3 position = Grid.CellToPosCBC(landerPlacementCell, Grid.SceneLayer.Building);
			component.transform.SetPosition(position);
			component.gameObject.SetActive(value: true);
			Clustercraft component5 = base.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			component5.gameObject.Trigger(1792516731, component);
			component.Trigger(1792516731, base.gameObject);
			GameObject gameObject = Assets.TryGetPrefab(base.smi.def.clusterMapFXPrefabID);
			if (!(gameObject != null))
			{
				return;
			}
			clusterMapFX = GameUtil.KInstantiate(gameObject, Grid.SceneLayer.Background);
			clusterMapFX.SetActive(value: true);
			ClusterFXEntity component6 = clusterMapFX.GetComponent<ClusterFXEntity>();
			AxialI location = component5.Location;
			AxialI myWorldLocation = component.GetMyWorldLocation();
			component6.Init(animOffset: Vector3.Normalize(AxialUtil.AxialToWorld(myWorldLocation.r, myWorldLocation.q) - AxialUtil.AxialToWorld(location.r, location.q)) * 100f, location: component5.Location);
			component.Subscribe(1969584890, delegate
			{
				if (!clusterMapFX.IsNullOrDestroyed())
				{
					Util.KDestroyGameObject(clusterMapFX);
				}
			});
			component.Subscribe(1591811118, delegate
			{
				if (!clusterMapFX.IsNullOrDestroyed())
				{
					Util.KDestroyGameObject(clusterMapFX);
				}
			});
		}

		public bool CheckReadyForFinalDeploy()
		{
			MinionStorage component = landerContainer.FindFirst(base.def.landerPrefabID).GetComponent<MinionStorage>();
			if (component != null)
			{
				return component.GetStoredMinionInfo().Count > 0;
			}
			return true;
		}

		public void CancelPendingDeploy()
		{
			landerPlaced = false;
			if (ChosenDuplicant != null && CheckIfLoaded())
			{
				ChosenDuplicant.GetSMI<RocketPassengerMonitor.Instance>().CancelModuleDeployChore();
			}
		}

		public bool CheckIfLoaded()
		{
			bool flag = false;
			foreach (GameObject item in landerContainer.items)
			{
				if (item.PrefabID() == base.def.landerPrefabID)
				{
					flag = true;
					break;
				}
			}
			if (flag != base.sm.hasCargo.Get(this))
			{
				base.sm.hasCargo.Set(flag, this);
			}
			return flag;
		}

		public bool IsValidDropLocation()
		{
			return GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().GetStableOrbitAsteroid() != null;
		}

		public void EmptyCargo()
		{
			ChooseLanderLocation();
		}

		public bool CanEmptyCargo()
		{
			if (base.sm.hasCargo.Get(base.smi) && IsValidDropLocation() && (!ChooseDuplicant || ChosenDuplicant != null))
			{
				return !landerPlaced;
			}
			return false;
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
		});
		grounded.DefaultState(grounded.loaded).TagTransition(GameTags.RocketNotOnGround, not_grounded);
		grounded.loaded.PlayAnim("loaded").ParamTransition(hasCargo, grounded.empty, GameStateMachine<JettisonableCargoModule, StatesInstance, IStateMachineTarget, Def>.IsFalse);
		grounded.empty.PlayAnim("deployed").ParamTransition(hasCargo, grounded.loaded, GameStateMachine<JettisonableCargoModule, StatesInstance, IStateMachineTarget, Def>.IsTrue);
		not_grounded.DefaultState(not_grounded.loaded).TagTransition(GameTags.RocketNotOnGround, grounded, on_remove: true);
		not_grounded.loaded.PlayAnim("loaded").ParamTransition(hasCargo, not_grounded.empty, GameStateMachine<JettisonableCargoModule, StatesInstance, IStateMachineTarget, Def>.IsFalse).OnSignal(emptyCargo, not_grounded.emptying);
		not_grounded.emptying.PlayAnim("deploying").Update(delegate(StatesInstance smi, float dt)
		{
			if (smi.CheckReadyForFinalDeploy())
			{
				smi.FinalDeploy();
				smi.GoTo(smi.sm.not_grounded.empty);
			}
		}).EventTransition(GameHashes.ClusterLocationChanged, (StatesInstance smi) => Game.Instance, not_grounded)
			.Exit(delegate(StatesInstance smi)
			{
				smi.CancelPendingDeploy();
			});
		not_grounded.empty.PlayAnim("deployed").ParamTransition(hasCargo, not_grounded.loaded, GameStateMachine<JettisonableCargoModule, StatesInstance, IStateMachineTarget, Def>.IsTrue);
	}
}
