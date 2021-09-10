using UnityEngine;

public class AutoStorageDropper : GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>
{
	public class Def : BaseDef
	{
		public CellOffset dropOffset;

		public bool asOre;

		public SimHashes[] elementFilter;

		public bool invertElementFilter;

		public bool blockedBySubstantialLiquid;
	}

	public new class Instance : GameInstance
	{
		[MyCmpGet]
		private Storage m_storage;

		[MyCmpGet]
		private Rotatable m_rotatable;

		private HandleVector<int>.Handle partitionerEntrySolid;

		private HandleVector<int>.Handle partitionerEntryLiquid;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			ScheduleNextFrame(RegisterListeners);
		}

		private void RegisterListeners(object obj)
		{
			int cell = Grid.PosToCell(base.smi.GetDropPosition());
			if (Grid.IsValidCell(cell))
			{
				Extents extents = new Extents(cell, new CellOffset[1]
				{
					new CellOffset(0, 0)
				});
				partitionerEntrySolid = GameScenePartitioner.Instance.Add("AutoStorageDropper.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, OnOutpuTileChanged);
				if (base.def.blockedBySubstantialLiquid)
				{
					partitionerEntryLiquid = GameScenePartitioner.Instance.Add("AutoStorageDropper.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.liquidChangedLayer, OnOutpuTileChanged);
				}
				OnOutpuTileChanged(null);
			}
		}

		protected override void OnCleanUp()
		{
			GameScenePartitioner.Instance.Free(ref partitionerEntrySolid);
			GameScenePartitioner.Instance.Free(ref partitionerEntryLiquid);
		}

		private void OnOutpuTileChanged(object data)
		{
			int cell = Grid.PosToCell(base.smi.GetDropPosition());
			bool flag = false;
			flag = ((Grid.IsSolidCell(cell) || (base.def.blockedBySubstantialLiquid && Grid.IsLiquid(cell))) ? true : false);
			base.sm.isBlocked.Set(flag, base.smi);
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
					}
					else
					{
						Dumpable component2 = gameObject.GetComponent<Dumpable>();
						if (!component2.IsNullOrDestroyed())
						{
							component2.Dump(GetDropPosition());
						}
					}
				}
			}
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

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.EventTransition(GameHashes.OnStorageChange, pre_drop).ParamTransition(isBlocked, blocked, GameStateMachine<AutoStorageDropper, Instance, IStateMachineTarget, Def>.IsTrue);
		pre_drop.ScheduleGoTo(0f, dropping);
		dropping.Enter(delegate(Instance smi)
		{
			smi.Drop();
		}).GoTo(idle);
		blocked.ParamTransition(isBlocked, pre_drop, GameStateMachine<AutoStorageDropper, Instance, IStateMachineTarget, Def>.IsFalse).ToggleStatusItem(Db.Get().BuildingStatusItems.OutputTileBlocked);
	}
}
