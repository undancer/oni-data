using UnityEngine;

public class AutoStorageDropper : GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>
{
	public class DropperFxConfig
	{
		public string animFile;

		public string animName;

		public Grid.SceneLayer layer = Grid.SceneLayer.FXFront;

		public bool useElementTint = true;

		public bool flipX;

		public bool flipY;
	}

	public class Def : BaseDef
	{
		public CellOffset dropOffset;

		public bool asOre;

		public SimHashes[] elementFilter;

		public bool invertElementFilter;

		public bool blockedBySubstantialLiquid;

		public DropperFxConfig neutralFx;

		public DropperFxConfig leftFx;

		public DropperFxConfig rightFx;

		public DropperFxConfig upFx;

		public DropperFxConfig downFx;

		public Vector3 fxOffset = Vector3.zero;

		public float cooldown = 2f;

		public float delay;
	}

	public new class Instance : GameInstance
	{
		[MyCmpGet]
		private Storage m_storage;

		[MyCmpGet]
		private Rotatable m_rotatable;

		private float m_timeSinceLastDrop;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void SetInvertElementFilter(bool value)
		{
			base.def.invertElementFilter = value;
			base.smi.sm.checkCanDrop.Trigger(base.smi);
		}

		public void UpdateBlockedStatus()
		{
			int cell = Grid.PosToCell(base.smi.GetDropPosition());
			bool value = Grid.IsSolidCell(cell) || (base.def.blockedBySubstantialLiquid && Grid.IsSubstantialLiquid(cell));
			base.sm.isBlocked.Set(value, base.smi);
		}

		private bool IsFilteredElement(SimHashes element)
		{
			for (int i = 0; i != base.def.elementFilter.Length; i++)
			{
				if (base.def.elementFilter[i] == element)
				{
					return true;
				}
			}
			return false;
		}

		private bool AllowedToDrop(SimHashes element)
		{
			if (base.def.elementFilter != null && base.def.elementFilter.Length != 0 && (base.def.invertElementFilter || !IsFilteredElement(element)))
			{
				if (base.def.invertElementFilter)
				{
					return !IsFilteredElement(element);
				}
				return false;
			}
			return true;
		}

		public void Drop()
		{
			bool flag = false;
			Element element = null;
			for (int num = m_storage.Count - 1; num >= 0; num--)
			{
				GameObject gameObject = m_storage.items[num];
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (AllowedToDrop(component.ElementID))
				{
					if (base.def.asOre)
					{
						m_storage.Drop(gameObject);
						gameObject.transform.SetPosition(GetDropPosition());
						element = component.Element;
						flag = true;
					}
					else
					{
						Dumpable component2 = gameObject.GetComponent<Dumpable>();
						if (!component2.IsNullOrDestroyed())
						{
							component2.Dump(GetDropPosition());
							element = component.Element;
							flag = true;
						}
					}
				}
			}
			DropperFxConfig dropperAnim = GetDropperAnim();
			if (flag && dropperAnim != null && GameClock.Instance.GetTime() > m_timeSinceLastDrop + base.def.cooldown)
			{
				m_timeSinceLastDrop = GameClock.Instance.GetTime();
				Vector3 position = Grid.CellToPosCCC(Grid.PosToCell(GetDropPosition()), dropperAnim.layer);
				position += ((m_rotatable != null) ? m_rotatable.GetRotatedOffset(base.def.fxOffset) : base.def.fxOffset);
				KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect(dropperAnim.animFile, position, null, update_looping_sounds_position: false, dropperAnim.layer);
				kBatchedAnimController.destroyOnAnimComplete = false;
				kBatchedAnimController.FlipX = dropperAnim.flipX;
				kBatchedAnimController.FlipY = dropperAnim.flipY;
				if (dropperAnim.useElementTint)
				{
					kBatchedAnimController.TintColour = element.substance.colour;
				}
				kBatchedAnimController.Play(dropperAnim.animName);
			}
		}

		public DropperFxConfig GetDropperAnim()
		{
			CellOffset cellOffset = ((m_rotatable != null) ? m_rotatable.GetRotatedCellOffset(base.def.dropOffset) : base.def.dropOffset);
			if (cellOffset.x < 0)
			{
				return base.def.leftFx;
			}
			if (cellOffset.x > 0)
			{
				return base.def.rightFx;
			}
			if (cellOffset.y < 0)
			{
				return base.def.downFx;
			}
			if (cellOffset.y > 0)
			{
				return base.def.upFx;
			}
			return base.def.neutralFx;
		}

		public Vector3 GetDropPosition()
		{
			if (!(m_rotatable != null))
			{
				return base.transform.GetPosition() + base.def.dropOffset.ToVector3();
			}
			return base.transform.GetPosition() + m_rotatable.GetRotatedCellOffset(base.def.dropOffset).ToVector3();
		}
	}

	private State idle;

	private State pre_drop;

	private State dropping;

	private State blocked;

	private BoolParameter isBlocked;

	public Signal checkCanDrop;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		root.Update(delegate(Instance smi, float dt)
		{
			smi.UpdateBlockedStatus();
		}, UpdateRate.SIM_200ms, load_balance: true);
		idle.EventTransition(GameHashes.OnStorageChange, pre_drop).OnSignal(checkCanDrop, pre_drop, (Instance smi) => !smi.GetComponent<Storage>().IsEmpty()).ParamTransition(isBlocked, blocked, GameStateMachine<AutoStorageDropper, Instance, IStateMachineTarget, Def>.IsTrue);
		pre_drop.ScheduleGoTo((Instance smi) => smi.def.delay, dropping);
		dropping.Enter(delegate(Instance smi)
		{
			smi.Drop();
		}).GoTo(idle);
		blocked.ParamTransition(isBlocked, pre_drop, GameStateMachine<AutoStorageDropper, Instance, IStateMachineTarget, Def>.IsFalse).ToggleStatusItem(Db.Get().BuildingStatusItems.OutputTileBlocked);
	}
}
