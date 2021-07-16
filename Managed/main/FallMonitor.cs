using System.Collections.Generic;
using UnityEngine;

public class FallMonitor : GameStateMachine<FallMonitor, FallMonitor.Instance>
{
	public class EntombedStates : State
	{
		public State recovering;

		public State stuck;
	}

	public new class Instance : GameInstance
	{
		private CellOffset[] entombedEscapeOffsets = new CellOffset[9]
		{
			new CellOffset(0, 1),
			new CellOffset(0, -1),
			new CellOffset(1, 0),
			new CellOffset(-1, 0),
			new CellOffset(1, 1),
			new CellOffset(-1, 1),
			new CellOffset(1, -1),
			new CellOffset(-1, -1),
			new CellOffset(0, 2)
		};

		private Navigator navigator;

		private bool shouldPlayEmotes;

		public string entombedAnimOverride;

		private List<int> safeCells = new List<int>();

		private int MAX_CELLS_TRACKED = 5;

		private bool flipRecoverEmote;

		public Instance(IStateMachineTarget master, bool shouldPlayEmotes, string entombedAnimOverride = null)
			: base(master)
		{
			navigator = GetComponent<Navigator>();
			this.shouldPlayEmotes = shouldPlayEmotes;
			this.entombedAnimOverride = entombedAnimOverride;
			Pathfinding.Instance.FlushNavGridsOnLoad();
			Subscribe(915392638, OnCellChanged);
			Subscribe(1027377649, OnMovementStateChanged);
			Subscribe(387220196, OnDestinationReached);
		}

		private void OnDestinationReached(object data)
		{
			int item = Grid.PosToCell(base.transform.GetPosition());
			if (!safeCells.Contains(item))
			{
				safeCells.Add(item);
				if (safeCells.Count > MAX_CELLS_TRACKED)
				{
					safeCells.RemoveAt(0);
				}
			}
		}

		private void OnMovementStateChanged(object data)
		{
			if ((GameHashes)data != GameHashes.ObjectMovementWakeUp)
			{
				return;
			}
			int item = Grid.PosToCell(base.transform.GetPosition());
			if (!safeCells.Contains(item))
			{
				safeCells.Add(item);
				if (safeCells.Count > MAX_CELLS_TRACKED)
				{
					safeCells.RemoveAt(0);
				}
			}
		}

		private void OnCellChanged(object data)
		{
			int item = (int)data;
			if (!safeCells.Contains(item))
			{
				safeCells.Add(item);
				if (safeCells.Count > MAX_CELLS_TRACKED)
				{
					safeCells.RemoveAt(0);
				}
			}
		}

		public void Recover()
		{
			int cell = Grid.PosToCell(navigator);
			NavGrid.Transition[] transitions = navigator.NavGrid.transitions;
			for (int i = 0; i < transitions.Length; i++)
			{
				NavGrid.Transition transition = transitions[i];
				if (transition.isEscape && navigator.CurrentNavType == transition.start)
				{
					int num = transition.IsValid(cell, navigator.NavGrid.NavTable);
					if (Grid.InvalidCell != num)
					{
						Vector2I vector2I = Grid.CellToXY(cell);
						flipRecoverEmote = Grid.CellToXY(num).x < vector2I.x;
						navigator.BeginTransition(transition);
						break;
					}
				}
			}
		}

		public void RecoverEmote()
		{
			if (shouldPlayEmotes && Random.Range(0, 9) == 8)
			{
				new EmoteChore(base.master.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_react_floor_missing_kanim", new HashedString[1]
				{
					"react"
				}, KAnim.PlayMode.Once, flipRecoverEmote);
			}
		}

		public void LandFloor()
		{
			navigator.SetCurrentNavType(NavType.Floor);
			GetComponent<Transform>().SetPosition(Grid.CellToPosCBC(Grid.PosToCell(GetComponent<Transform>().GetPosition()), Grid.SceneLayer.Move));
		}

		public void AttemptInitialRecovery()
		{
			if (base.gameObject.HasTag(GameTags.Incapacitated))
			{
				return;
			}
			int cell = Grid.PosToCell(navigator);
			NavGrid.Transition[] transitions = navigator.NavGrid.transitions;
			for (int i = 0; i < transitions.Length; i++)
			{
				NavGrid.Transition transition = transitions[i];
				if (transition.isEscape && navigator.CurrentNavType == transition.start)
				{
					int num = transition.IsValid(cell, navigator.NavGrid.NavTable);
					if (Grid.InvalidCell != num)
					{
						base.smi.GoTo(base.smi.sm.recoverinitialfall);
						break;
					}
				}
			}
		}

		public bool CanRecoverToLadder()
		{
			int cell = Grid.PosToCell(base.master.transform.GetPosition());
			if (navigator.NavGrid.NavTable.IsValid(cell, NavType.Ladder))
			{
				return !base.gameObject.HasTag(GameTags.Incapacitated);
			}
			return false;
		}

		public void MountLadder()
		{
			navigator.SetCurrentNavType(NavType.Ladder);
			GetComponent<Transform>().SetPosition(Grid.CellToPosCBC(Grid.PosToCell(GetComponent<Transform>().GetPosition()), Grid.SceneLayer.Move));
		}

		public bool CanRecoverToPole()
		{
			int cell = Grid.PosToCell(base.master.transform.GetPosition());
			if (navigator.NavGrid.NavTable.IsValid(cell, NavType.Pole))
			{
				return !base.gameObject.HasTag(GameTags.Incapacitated);
			}
			return false;
		}

		public void MountPole()
		{
			navigator.SetCurrentNavType(NavType.Pole);
			GetComponent<Transform>().SetPosition(Grid.CellToPosCBC(Grid.PosToCell(GetComponent<Transform>().GetPosition()), Grid.SceneLayer.Move));
		}

		public void UpdateFalling()
		{
			bool value = false;
			bool flag = false;
			if (!navigator.IsMoving() && navigator.CurrentNavType != NavType.Tube)
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				int num2 = Grid.CellAbove(num);
				bool flag2 = Grid.IsValidCell(num);
				bool flag3 = Grid.IsValidCell(num2);
				bool num3 = IsValidNavCell(num) && (!base.gameObject.HasTag(GameTags.Incapacitated) || (navigator.CurrentNavType != NavType.Ladder && navigator.CurrentNavType != NavType.Pole));
				flag = (!num3 && flag2 && Grid.Solid[num]) || (flag3 && Grid.Solid[num2]) || (flag2 && Grid.DupeImpassable[num]) || (flag3 && Grid.DupeImpassable[num2]);
				value = !num3 && !flag;
				if ((!flag2 && flag3) || Grid.WorldIdx[num] != Grid.WorldIdx[num2])
				{
					TeleportInWorld(num);
				}
			}
			base.sm.isFalling.Set(value, base.smi);
			base.sm.isEntombed.Set(flag, base.smi);
		}

		private void TeleportInWorld(int cell)
		{
			WorldContainer worldContainer = null;
			do
			{
				int num = Grid.CellAbove(cell);
				worldContainer = ClusterManager.Instance.GetWorld(Grid.WorldIdx[num]);
			}
			while (worldContainer == null);
			int safeCell = worldContainer.GetSafeCell();
			Debug.Log($"Teleporting {navigator.name} to {safeCell}");
			MoveToCell(safeCell);
		}

		private bool IsValidNavCell(int cell)
		{
			if (navigator.NavGrid.NavTable.IsValid(cell, navigator.CurrentNavType))
			{
				return !Grid.DupeImpassable[cell];
			}
			return false;
		}

		public void TryEntombedEscape()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			int backCell = GetComponent<Facing>().GetBackCell();
			int num2 = Grid.CellAbove(backCell);
			int num3 = Grid.CellBelow(backCell);
			int[] array = new int[3]
			{
				backCell,
				num2,
				num3
			};
			foreach (int cell in array)
			{
				if (IsValidNavCell(cell))
				{
					MoveToCell(cell);
					return;
				}
			}
			int cell2 = Grid.PosToCell(base.transform.GetPosition());
			for (int num4 = safeCells.Count - 1; num4 >= 0; num4--)
			{
				int num5 = safeCells[num4];
				if (num5 != num && IsValidNavCell(num5))
				{
					MoveToCell(num5);
					return;
				}
			}
			CellOffset[] array2 = entombedEscapeOffsets;
			foreach (CellOffset offset in array2)
			{
				if (Grid.IsCellOffsetValid(cell2, offset))
				{
					int cell3 = Grid.OffsetCell(cell2, offset);
					if (IsValidNavCell(cell3))
					{
						MoveToCell(cell3);
						return;
					}
				}
			}
			array2 = entombedEscapeOffsets;
			foreach (CellOffset offset2 in array2)
			{
				if (Grid.IsCellOffsetValid(cell2, offset2))
				{
					int num6 = Grid.OffsetCell(cell2, offset2);
					int num7 = Grid.CellAbove(num6);
					if (Grid.IsValidCell(num7) && !Grid.Solid[num6] && !Grid.Solid[num7] && !Grid.DupeImpassable[num6] && !Grid.DupeImpassable[num7])
					{
						MoveToCell(num6, forceFloorNav: true);
						return;
					}
				}
			}
			GoTo(base.sm.entombed.stuck);
		}

		private void MoveToCell(int cell, bool forceFloorNav = false)
		{
			base.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
			base.transform.GetComponent<Navigator>().Stop();
			if (base.gameObject.HasTag(GameTags.Incapacitated) || forceFloorNav)
			{
				base.transform.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
			}
			UpdateFalling();
			if (base.sm.isEntombed.Get(base.smi))
			{
				GoTo(base.sm.entombed.stuck);
			}
			else
			{
				GoTo(base.sm.standing);
			}
		}
	}

	public State standing;

	public State falling_pre;

	public State falling;

	public EntombedStates entombed;

	public State recoverladder;

	public State recoverpole;

	public State recoverinitialfall;

	public State landfloor;

	public State instorage;

	public BoolParameter isEntombed;

	public BoolParameter isFalling;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = standing;
		root.TagTransition(GameTags.Stored, instorage).Update("CheckLanded", delegate(Instance smi, float dt)
		{
			smi.UpdateFalling();
		}, UpdateRate.SIM_33ms, load_balance: true);
		standing.ParamTransition(isEntombed, entombed, GameStateMachine<FallMonitor, Instance, IStateMachineTarget, object>.IsTrue).ParamTransition(isFalling, falling_pre, GameStateMachine<FallMonitor, Instance, IStateMachineTarget, object>.IsTrue);
		falling_pre.Enter("StopNavigator", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().Stop();
		}).Enter("AttemptInitialRecovery", delegate(Instance smi)
		{
			smi.AttemptInitialRecovery();
		}).GoTo(falling)
			.ToggleBrain("falling_pre");
		falling.ToggleBrain("falling").PlayAnim("fall_pre").QueueAnim("fall_loop", loop: true)
			.ParamTransition(isEntombed, entombed, GameStateMachine<FallMonitor, Instance, IStateMachineTarget, object>.IsTrue)
			.Transition(recoverladder, (Instance smi) => smi.CanRecoverToLadder(), UpdateRate.SIM_33ms)
			.Transition(recoverpole, (Instance smi) => smi.CanRecoverToPole(), UpdateRate.SIM_33ms)
			.ToggleGravity(landfloor);
		recoverinitialfall.ToggleBrain("recoverinitialfall").Enter("Recover", delegate(Instance smi)
		{
			smi.Recover();
		}).EventTransition(GameHashes.DestinationReached, standing)
			.EventTransition(GameHashes.NavigationFailed, standing)
			.Exit(delegate(Instance smi)
			{
				smi.RecoverEmote();
			});
		landfloor.Enter("Land", delegate(Instance smi)
		{
			smi.LandFloor();
		}).GoTo(standing);
		recoverladder.ToggleBrain("recoverladder").PlayAnim("floor_ladder_0_0").Enter("MountLadder", delegate(Instance smi)
		{
			smi.MountLadder();
		})
			.OnAnimQueueComplete(standing);
		recoverpole.ToggleBrain("recoverpole").PlayAnim("floor_pole_0_0").Enter("MountPole", delegate(Instance smi)
		{
			smi.MountPole();
		})
			.OnAnimQueueComplete(standing);
		instorage.TagTransition(GameTags.Stored, instorage, on_remove: true);
		entombed.DefaultState(entombed.recovering);
		entombed.recovering.Enter("TryEntombedEscape", delegate(Instance smi)
		{
			smi.TryEntombedEscape();
		});
		entombed.stuck.Enter("StopNavigator", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().Stop();
		}).ToggleChore((Instance smi) => new EntombedChore(smi.master, smi.entombedAnimOverride), standing).ParamTransition(isEntombed, standing, GameStateMachine<FallMonitor, Instance, IStateMachineTarget, object>.IsFalse);
	}
}
