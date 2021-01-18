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

		private bool flipRecoverEmote;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			navigator = GetComponent<Navigator>();
			Pathfinding.Instance.FlushNavGridsOnLoad();
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
			if (Random.Range(0, 9) == 8)
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

		public bool IsFalling()
		{
			if (navigator.IsMoving())
			{
				return false;
			}
			int cell = Grid.PosToCell(base.master.transform.GetPosition());
			if (!Grid.IsValidCell(cell))
			{
				return false;
			}
			if (!Grid.IsValidCell(Grid.CellBelow(cell)))
			{
				return false;
			}
			return !navigator.NavGrid.NavTable.IsValid(cell, navigator.CurrentNavType);
		}

		public void UpdateFalling()
		{
			bool value = false;
			bool flag = false;
			if (!navigator.IsMoving())
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				int num2 = Grid.CellAbove(num);
				bool num3 = navigator.NavGrid.NavTable.IsValid(num, navigator.CurrentNavType) && (!base.gameObject.HasTag(GameTags.Incapacitated) || (navigator.CurrentNavType != NavType.Ladder && navigator.CurrentNavType != NavType.Pole));
				flag = !num3 && ((Grid.IsValidCell(num) && Grid.Solid[num]) || (Grid.IsValidCell(num2) && Grid.Solid[num2]));
				value = !num3 && !flag;
			}
			base.sm.isFalling.Set(value, base.smi);
			base.sm.isEntombed.Set(flag, base.smi);
		}

		private bool IsValidNavCell(int cell)
		{
			return navigator.NavGrid.NavTable.IsValid(cell, navigator.CurrentNavType);
		}

		public void TryEntombedEscape()
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			CellOffset[] array = entombedEscapeOffsets;
			foreach (CellOffset offset in array)
			{
				if (!Grid.IsCellOffsetValid(cell, offset))
				{
					continue;
				}
				int cell2 = Grid.OffsetCell(cell, offset);
				if (IsValidNavCell(cell2))
				{
					base.transform.SetPosition(Grid.CellToPosCBC(cell2, Grid.SceneLayer.Move));
					base.transform.GetComponent<Navigator>().Stop();
					if (base.gameObject.HasTag(GameTags.Incapacitated))
					{
						base.transform.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
					}
					UpdateFalling();
					GoTo(base.sm.standing);
					return;
				}
			}
			array = entombedEscapeOffsets;
			foreach (CellOffset offset2 in array)
			{
				if (Grid.IsCellOffsetValid(cell, offset2))
				{
					int num = Grid.OffsetCell(cell, offset2);
					int num2 = Grid.CellAbove(num);
					if (Grid.IsValidCell(num2) && !Grid.Solid[num] && !Grid.Solid[num2])
					{
						base.transform.SetPosition(Grid.CellToPosCBC(num, Grid.SceneLayer.Move));
						base.transform.GetComponent<Navigator>().Stop();
						base.transform.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
						UpdateFalling();
						GoTo(base.sm.standing);
						return;
					}
				}
			}
			GoTo(base.sm.entombed.stuck);
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
		root.EventTransition(GameHashes.OnStore, instorage).Update("CheckLanded", delegate(Instance smi, float dt)
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
		instorage.EventTransition(GameHashes.OnStore, standing);
		entombed.DefaultState(entombed.recovering);
		entombed.recovering.Enter("TryEntombedEscape", delegate(Instance smi)
		{
			smi.TryEntombedEscape();
		});
		entombed.stuck.Enter("StopNavigator", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().Stop();
		}).ToggleChore((Instance smi) => new EntombedChore(smi.master), standing).ParamTransition(isEntombed, standing, GameStateMachine<FallMonitor, Instance, IStateMachineTarget, object>.IsFalse);
	}
}
