using UnityEngine;

public class JettisonableCargoModule : GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>
{
	public class Def : BaseDef
	{
		public DefComponent<Storage> landerContainer;

		public Tag landerPrefabID;

		public Vector3 cargoDropOffset;
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

		private MinionIdentity chosenDuplicant;

		public bool AutoDeploy
		{
			get;
			set;
		}

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
				MinionStorage component = gameObject.GetComponent<MinionStorage>();
				return component != null;
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
			RocketModuleCluster component = base.master.GetComponent<RocketModuleCluster>();
			Clustercraft component2 = component.CraftInterface.GetComponent<Clustercraft>();
			ClusterGridEntity stableOrbitAsteroid = component2.GetStableOrbitAsteroid();
			if (stableOrbitAsteroid != null)
			{
				int id = stableOrbitAsteroid.GetComponent<WorldContainer>().id;
				GameObject gameObject = landerContainer.FindFirst(base.def.landerPrefabID);
				Placeable component3 = gameObject.GetComponent<Placeable>();
				component3.restrictWorldId = id;
				ClusterManager.Instance.SetActiveWorld(id);
				ManagementMenu.Instance.CloseAll();
				PlaceTool.Instance.Activate(component3, OnLanderPlaced);
			}
		}

		private void OnLanderPlaced(Placeable lander, int cell)
		{
			GameObject gameObject = landerContainer.FindFirst(base.def.landerPrefabID);
			landerContainer.Drop(lander.gameObject);
			TreeFilterable component = GetComponent<TreeFilterable>();
			TreeFilterable component2 = lander.GetComponent<TreeFilterable>();
			if (component2 != null)
			{
				component2.UpdateFilters(component.AcceptedTags);
			}
			Storage component3 = lander.GetComponent<Storage>();
			if (component3 != null)
			{
				Storage[] components = base.gameObject.GetComponents<Storage>();
				foreach (Storage storage in components)
				{
					storage.Transfer(component3, block_events: false, hide_popups: true);
				}
			}
			MinionStorage component4 = lander.GetComponent<MinionStorage>();
			if (component4 != null)
			{
				CraftModuleInterface craftInterface = GetComponent<RocketModuleCluster>().CraftInterface;
				WorldContainer worldContainer = ((craftInterface != null) ? craftInterface.GetComponent<WorldContainer>() : null);
				if (worldContainer != null)
				{
					int id = worldContainer.id;
					foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
					{
						if (item == chosenDuplicant && item.GetMyWorldId() == id)
						{
							Game.Instance.assignmentManager.RemoveFromWorld(item.assignableProxy.Get(), id);
							craftInterface.GetPassengerModule().RemoveRocketPassenger(item);
							component4.SerializeMinion(item.gameObject);
							break;
						}
					}
				}
			}
			Vector3 position = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
			lander.transform.SetPosition(position);
			lander.gameObject.SetActive(value: true);
			lander.Trigger(1792516731, base.gameObject);
			ManagementMenu.Instance.ToggleClusterMap();
			ClusterMapScreen.Instance.SelectEntity(GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<ClusterGridEntity>(), frameDelay: true);
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
			Clustercraft component = GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			return component.GetStableOrbitAsteroid() != null;
		}

		public void EmptyCargo()
		{
			ChooseLanderLocation();
		}

		public bool CanEmptyCargo()
		{
			return base.sm.hasCargo.Get(base.smi) && IsValidDropLocation() && (!ChooseDuplicant || ChosenDuplicant != null);
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
		not_grounded.emptying.PlayAnim("deploying").GoTo(not_grounded.empty);
		not_grounded.empty.PlayAnim("deployed").ParamTransition(hasCargo, not_grounded.loaded, GameStateMachine<JettisonableCargoModule, StatesInstance, IStateMachineTarget, Def>.IsTrue);
	}
}
