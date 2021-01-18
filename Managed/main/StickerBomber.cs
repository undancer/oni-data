using System;
using System.Collections.Generic;
using KSerialization;
using TUNING;
using UnityEngine;

public class StickerBomber : GameStateMachine<StickerBomber, StickerBomber.Instance>
{
	public class OverjoyedStates : State
	{
		public State idle;

		public State place_stickers;
	}

	public new class Instance : GameInstance
	{
		private class StickerBombReactable : Reactable
		{
			private int stickersToPlace = 0;

			private int stickersPlaced = 0;

			private int placementCell;

			private float tile_random_range = 1f;

			private float tile_random_rotation = 90f;

			private float TIME_PER_STICKER_PLACED = 0.66f;

			private float STICKER_PLACE_TIMER;

			private KBatchedAnimController kbac;

			private KAnimFile animset = Assets.GetAnim("anim_stickers_kanim");

			private HashedString pre_anim = "working_pre";

			private HashedString loop_anim = "working_loop";

			private HashedString pst_anim = "working_pst";

			private Instance stickerBomber;

			private Func<int, bool> canPlaceStickerCb = delegate(int cell)
			{
				if (Grid.Solid[cell])
				{
					return false;
				}
				if (Grid.IsValidCell(Grid.CellLeft(cell)) && Grid.Solid[Grid.CellLeft(cell)])
				{
					return false;
				}
				if (Grid.IsValidCell(Grid.CellRight(cell)) && Grid.Solid[Grid.CellRight(cell)])
				{
					return false;
				}
				if (Grid.IsValidCell(Grid.OffsetCell(cell, 0, 1)) && Grid.Solid[Grid.OffsetCell(cell, 0, 1)])
				{
					return false;
				}
				if (Grid.IsValidCell(Grid.OffsetCell(cell, 0, -1)) && Grid.Solid[Grid.OffsetCell(cell, 0, -1)])
				{
					return false;
				}
				return (!Grid.IsCellOpenToSpace(cell)) ? true : false;
			};

			public StickerBombReactable(GameObject gameObject, Instance stickerBomber)
				: base(gameObject, "StickerBombReactable", Db.Get().ChoreTypes.Build, 2, 1)
			{
				preventChoreInterruption = true;
				this.stickerBomber = stickerBomber;
			}

			public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
			{
				if (reactor != null)
				{
					return false;
				}
				if (new_reactor == null)
				{
					return false;
				}
				if (gameObject != new_reactor)
				{
					return false;
				}
				Navigator component = new_reactor.GetComponent<Navigator>();
				if (component == null)
				{
					return false;
				}
				if (component.CurrentNavType == NavType.Tube || component.CurrentNavType == NavType.Ladder || component.CurrentNavType == NavType.Pole)
				{
					return false;
				}
				return true;
			}

			protected override void InternalBegin()
			{
				stickersToPlace = UnityEngine.Random.Range(4, 6);
				STICKER_PLACE_TIMER = TIME_PER_STICKER_PLACED;
				placementCell = FindPlacementCell();
				if (placementCell == 0)
				{
					End();
					return;
				}
				kbac = reactor.GetComponent<KBatchedAnimController>();
				kbac.AddAnimOverrides(animset);
				kbac.Play(pre_anim);
				kbac.Queue(loop_anim, KAnim.PlayMode.Loop);
			}

			public override void Update(float dt)
			{
				STICKER_PLACE_TIMER -= dt;
				if (STICKER_PLACE_TIMER <= 0f)
				{
					PlaceSticker();
					STICKER_PLACE_TIMER = TIME_PER_STICKER_PLACED;
				}
				if (stickersPlaced >= stickersToPlace)
				{
					kbac.Play(pst_anim);
					End();
				}
			}

			protected override void InternalEnd()
			{
				if (kbac != null)
				{
					kbac.RemoveAnimOverrides(animset);
					kbac = null;
				}
				stickerBomber.sm.doneStickerBomb.Trigger(stickerBomber);
				stickersPlaced = 0;
			}

			private int FindPlacementCell()
			{
				int cell = Grid.PosToCell(reactor.transform.GetPosition() + Vector3.up);
				ListPool<int, PathFinder>.PooledList pooledList = ListPool<int, PathFinder>.Allocate();
				ListPool<int, PathFinder>.PooledList pooledList2 = ListPool<int, PathFinder>.Allocate();
				QueuePool<GameUtil.FloodFillInfo, Comet>.PooledQueue pooledQueue = QueuePool<GameUtil.FloodFillInfo, Comet>.Allocate();
				pooledQueue.Enqueue(new GameUtil.FloodFillInfo
				{
					cell = cell,
					depth = 0
				});
				GameUtil.FloodFillConditional(pooledQueue, canPlaceStickerCb, pooledList, pooledList2, 2);
				if (pooledList2.Count > 0)
				{
					int random = pooledList2.GetRandom();
					pooledList.Recycle();
					pooledList2.Recycle();
					pooledQueue.Recycle();
					return random;
				}
				return 0;
			}

			private void PlaceSticker()
			{
				stickersPlaced++;
				Vector3 a = Grid.CellToPos(placementCell);
				int num = 10;
				while (num > 0)
				{
					num--;
					Vector3 position = a + new Vector3(UnityEngine.Random.Range(0f - tile_random_range, tile_random_range), UnityEngine.Random.Range(0f - tile_random_range, tile_random_range), -2.5f);
					List<int> offsets = StickerBomb.BuildCellOffsets(position);
					if (StickerBomb.CanPlaceSticker(offsets))
					{
						GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("StickerBomb".ToTag()), position, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f - tile_random_rotation, tile_random_rotation)));
						StickerBomb component = gameObject.GetComponent<StickerBomb>();
						string stickerType = reactor.GetComponent<MinionIdentity>().stickerType;
						component.SetStickerType(stickerType);
						gameObject.SetActive(value: true);
						num = 0;
					}
				}
			}

			protected override void InternalCleanup()
			{
			}
		}

		[Serialize]
		public float nextStickerBomb = 0f;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public Reactable CreateReactable()
		{
			return new StickerBombReactable(base.master.gameObject, base.smi);
		}
	}

	public Signal doneStickerBomb;

	public State neutral;

	public OverjoyedStates overjoyed;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = neutral;
		root.TagTransition(GameTags.Dead, null);
		neutral.TagTransition(GameTags.Overjoyed, overjoyed).Exit(delegate(Instance smi)
		{
			smi.nextStickerBomb = GameClock.Instance.GetTime() + TRAITS.JOY_REACTIONS.STICKER_BOMBER.TIME_PER_STICKER_BOMB;
		});
		overjoyed.TagTransition(GameTags.Overjoyed, neutral, on_remove: true).DefaultState(overjoyed.idle);
		overjoyed.idle.Transition(overjoyed.place_stickers, (Instance smi) => GameClock.Instance.GetTime() >= smi.nextStickerBomb);
		overjoyed.place_stickers.Exit(delegate(Instance smi)
		{
			smi.nextStickerBomb = GameClock.Instance.GetTime() + TRAITS.JOY_REACTIONS.STICKER_BOMBER.TIME_PER_STICKER_BOMB;
		}).ToggleReactable((Instance smi) => smi.CreateReactable()).OnSignal(doneStickerBomb, overjoyed.idle);
	}
}
