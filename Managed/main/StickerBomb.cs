using System.Collections.Generic;
using KSerialization;
using TUNING;
using UnityEngine;

public class StickerBomb : StateMachineComponent<StickerBomb.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, StickerBomb, object>.GameInstance
	{
		[Serialize]
		public float destroyTime;

		public StatesInstance(StickerBomb master)
			: base(master)
		{
		}

		public string GetStickerAnim(string type)
		{
			return $"{type}_{base.master.stickerType}";
		}
	}

	public class States : GameStateMachine<States, StatesInstance, StickerBomb>
	{
		public State destroy;

		public State sparkle;

		public State idle;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			base.serializable = SerializeType.Both_DEPRECATED;
			root.Transition(destroy, (StatesInstance smi) => GameClock.Instance.GetTime() >= smi.destroyTime).DefaultState(idle);
			idle.PlayAnim((StatesInstance smi) => smi.GetStickerAnim("idle")).ScheduleGoTo((StatesInstance smi) => Random.Range(20, 30), sparkle);
			sparkle.PlayAnim((StatesInstance smi) => smi.GetStickerAnim("sparkle")).OnAnimQueueComplete(idle);
			destroy.Enter(delegate(StatesInstance smi)
			{
				Util.KDestroyGameObject(smi.master);
			});
		}
	}

	[Serialize]
	public string stickerType;

	private HandleVector<int>.Handle partitionerEntry;

	private List<int> cellOffsets;

	protected override void OnSpawn()
	{
		cellOffsets = BuildCellOffsets(base.transform.GetPosition());
		base.smi.destroyTime = GameClock.Instance.GetTime() + TRAITS.JOY_REACTIONS.STICKER_BOMBER.STICKER_DURATION;
		base.smi.StartSM();
		Extents extents = GetComponent<OccupyArea>().GetExtents();
		Extents extents2 = new Extents(extents.x - 1, extents.y - 1, extents.width + 2, extents.height + 2);
		partitionerEntry = GameScenePartitioner.Instance.Add("StickerBomb.OnSpawn", base.gameObject, extents2, GameScenePartitioner.Instance.objectLayers[2], OnFoundationCellChanged);
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	private void OnFoundationCellChanged(object data)
	{
		if (!CanPlaceSticker(cellOffsets))
		{
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	public static List<int> BuildCellOffsets(Vector3 position)
	{
		List<int> list = new List<int>();
		bool num = position.x % 1f < 0.5f;
		bool flag = position.y % 1f > 0.5f;
		int num2 = Grid.PosToCell(position);
		list.Add(num2);
		if (num)
		{
			list.Add(Grid.CellLeft(num2));
			if (flag)
			{
				list.Add(Grid.CellAbove(num2));
				list.Add(Grid.CellUpLeft(num2));
			}
			else
			{
				list.Add(Grid.CellBelow(num2));
				list.Add(Grid.CellDownLeft(num2));
			}
		}
		else
		{
			list.Add(Grid.CellRight(num2));
			if (flag)
			{
				list.Add(Grid.CellAbove(num2));
				list.Add(Grid.CellUpRight(num2));
			}
			else
			{
				list.Add(Grid.CellBelow(num2));
				list.Add(Grid.CellDownRight(num2));
			}
		}
		return list;
	}

	public static bool CanPlaceSticker(List<int> offsets)
	{
		foreach (int offset in offsets)
		{
			if (Grid.IsCellOpenToSpace(offset))
			{
				return false;
			}
		}
		return true;
	}

	public void SetStickerType(string newStickerType)
	{
		if (newStickerType == null)
		{
			newStickerType = "sticker";
		}
		stickerType = $"{newStickerType}_{TRAITS.JOY_REACTIONS.STICKER_BOMBER.STICKER_ANIMS.GetRandom()}";
	}
}
