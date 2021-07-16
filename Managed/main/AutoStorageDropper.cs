using UnityEngine;

public class AutoStorageDropper : GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>
{
	public class Def : BaseDef
	{
		public Tag dropTag;

		public CellOffset dropOffset;

		public bool asOre;

		public float maxRate = float.MaxValue;

		public bool onlyWhenOperational;
	}

	public new class Instance : GameInstance
	{
		private Storage storage;

		private Rotatable rotatable;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			storage = master.GetComponent<Storage>();
			rotatable = master.GetComponent<Rotatable>();
		}

		public void Drop()
		{
			for (int num = storage.Count - 1; num >= 0; num--)
			{
				GameObject gameObject = storage.items[num];
				if (gameObject.HasTag(base.def.dropTag))
				{
					if (base.def.asOre)
					{
						Vector3 position = ((rotatable != null) ? (base.transform.GetPosition() + rotatable.GetRotatedCellOffset(base.def.dropOffset).ToVector3()) : (base.transform.GetPosition() + base.def.dropOffset.ToVector3()));
						storage.Drop(gameObject);
						gameObject.transform.SetPosition(position);
					}
					else
					{
						int num2 = ((rotatable != null) ? Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), rotatable.GetRotatedCellOffset(base.def.dropOffset)) : Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), base.def.dropOffset));
						storage.ConsumeAndGetDisease(base.def.dropTag, float.MaxValue, out var amount_consumed, out var disease_info, out var aggregate_temperature);
						Element element = ElementLoader.GetElement(base.def.dropTag);
						byte idx = element.idx;
						if (element.IsLiquid)
						{
							FallingWater.instance.AddParticle(num2, idx, amount_consumed, aggregate_temperature, disease_info.idx, disease_info.count, skip_sound: true);
						}
						else
						{
							SimMessages.ModifyCell(num2, idx, aggregate_temperature, amount_consumed, disease_info.idx, disease_info.count);
						}
					}
				}
			}
		}
	}

	private State idle;

	private State pre_drop;

	private State dropping;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.EventTransition(GameHashes.OnStorageChange, pre_drop);
		pre_drop.ScheduleGoTo(0f, dropping);
		dropping.Enter(delegate(Instance smi)
		{
			smi.Drop();
		}).GoTo(idle);
	}
}
