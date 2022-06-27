using System;
using System.Collections.Generic;
using UnityEngine;

public class GameNavGrids
{
	public class SwimValidator : NavTableValidator
	{
		public SwimValidator()
		{
			World instance = World.Instance;
			instance.OnLiquidChanged = (Action<int>)Delegate.Combine(instance.OnLiquidChanged, new Action<int>(OnLiquidChanged));
			GameScenePartitioner.Instance.AddGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[9], OnFoundationTileChanged);
		}

		private void OnFoundationTileChanged(int cell, object unused)
		{
			if (onDirty != null)
			{
				onDirty(cell);
			}
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = Grid.IsSubstantialLiquid(cell);
			if (!flag)
			{
				flag = Grid.IsSubstantialLiquid(Grid.CellAbove(cell));
			}
			bool is_valid = Grid.IsWorldValidCell(cell) && flag && IsClear(cell, bounding_offsets, is_dupe: false);
			nav_table.SetValid(cell, NavType.Swim, is_valid);
		}

		private void OnLiquidChanged(int cell)
		{
			if (onDirty != null)
			{
				onDirty(cell);
			}
		}
	}

	public class FloorValidator : NavTableValidator
	{
		private bool isDupe;

		public FloorValidator(bool is_dupe)
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
			Components.Ladders.Register(OnAddLadder, OnRemoveLadder);
			isDupe = is_dupe;
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = IsWalkableCell(cell, Grid.CellBelow(cell), isDupe);
			nav_table.SetValid(cell, NavType.Floor, flag && IsClear(cell, bounding_offsets, isDupe));
		}

		public static bool IsWalkableCell(int cell, int anchor_cell, bool is_dupe)
		{
			if (!Grid.IsWorldValidCell(cell))
			{
				return false;
			}
			if (!Grid.IsWorldValidCell(anchor_cell))
			{
				return false;
			}
			if (!NavTableValidator.IsCellPassable(cell, is_dupe))
			{
				return false;
			}
			if (Grid.FakeFloor[anchor_cell])
			{
				return true;
			}
			if (Grid.Solid[anchor_cell])
			{
				if (Grid.DupePassable[anchor_cell])
				{
					return false;
				}
				return true;
			}
			if (is_dupe)
			{
				if ((Grid.NavValidatorMasks[cell] & (Grid.NavValidatorFlags.Ladder | Grid.NavValidatorFlags.Pole)) == 0)
				{
					return (Grid.NavValidatorMasks[anchor_cell] & (Grid.NavValidatorFlags.Ladder | Grid.NavValidatorFlags.Pole)) != 0;
				}
				return false;
			}
			return false;
		}

		private void OnAddLadder(Ladder ladder)
		{
			int obj = Grid.PosToCell(ladder);
			if (onDirty != null)
			{
				onDirty(obj);
			}
		}

		private void OnRemoveLadder(Ladder ladder)
		{
			int obj = Grid.PosToCell(ladder);
			if (onDirty != null)
			{
				onDirty(obj);
			}
		}

		private void OnSolidChanged(int cell)
		{
			if (onDirty != null)
			{
				onDirty(cell);
			}
		}

		public override void Clear()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
			Components.Ladders.Unregister(OnAddLadder, OnRemoveLadder);
		}
	}

	public class WallValidator : NavTableValidator
	{
		public WallValidator()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = IsWalkableCell(cell, Grid.CellRight(cell));
			bool flag2 = IsWalkableCell(cell, Grid.CellLeft(cell));
			nav_table.SetValid(cell, NavType.RightWall, flag && IsClear(cell, bounding_offsets, is_dupe: false));
			nav_table.SetValid(cell, NavType.LeftWall, flag2 && IsClear(cell, bounding_offsets, is_dupe: false));
		}

		private static bool IsWalkableCell(int cell, int anchor_cell)
		{
			if (Grid.IsWorldValidCell(cell) && Grid.IsWorldValidCell(anchor_cell))
			{
				if (!NavTableValidator.IsCellPassable(cell, is_dupe: false))
				{
					return false;
				}
				if (Grid.Solid[anchor_cell])
				{
					return true;
				}
				if (Grid.CritterImpassable[anchor_cell])
				{
					return true;
				}
			}
			return false;
		}

		private void OnSolidChanged(int cell)
		{
			if (onDirty != null)
			{
				onDirty(cell);
			}
		}

		public override void Clear()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
		}
	}

	public class CeilingValidator : NavTableValidator
	{
		public CeilingValidator()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = IsWalkableCell(cell, Grid.CellAbove(cell));
			nav_table.SetValid(cell, NavType.Ceiling, flag && IsClear(cell, bounding_offsets, is_dupe: false));
		}

		private static bool IsWalkableCell(int cell, int anchor_cell)
		{
			if (Grid.IsWorldValidCell(cell) && Grid.IsWorldValidCell(anchor_cell))
			{
				if (!NavTableValidator.IsCellPassable(cell, is_dupe: false))
				{
					return false;
				}
				if (Grid.Solid[anchor_cell])
				{
					return true;
				}
				if (Grid.HasDoor[cell] && !Grid.FakeFloor[cell])
				{
					return false;
				}
				if (Grid.FakeFloor[anchor_cell])
				{
					return true;
				}
				if (Grid.HasDoor[anchor_cell])
				{
					return true;
				}
			}
			return false;
		}

		private void OnSolidChanged(int cell)
		{
			if (onDirty != null)
			{
				onDirty(cell);
			}
		}

		public override void Clear()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
		}
	}

	public class LadderValidator : NavTableValidator
	{
		public LadderValidator()
		{
			Components.Ladders.Register(OnAddLadder, OnRemoveLadder);
		}

		private void OnAddLadder(Ladder ladder)
		{
			int obj = Grid.PosToCell(ladder);
			if (onDirty != null)
			{
				onDirty(obj);
			}
		}

		private void OnRemoveLadder(Ladder ladder)
		{
			int obj = Grid.PosToCell(ladder);
			if (onDirty != null)
			{
				onDirty(obj);
			}
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			nav_table.SetValid(cell, NavType.Ladder, IsClear(cell, bounding_offsets, is_dupe: true) && Grid.HasLadder[cell]);
		}

		public override void Clear()
		{
			Components.Ladders.Unregister(OnAddLadder, OnRemoveLadder);
		}
	}

	public class PoleValidator : LadderValidator
	{
		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			nav_table.SetValid(cell, NavType.Pole, IsClear(cell, bounding_offsets, is_dupe: true) && Grid.HasPole[cell]);
		}
	}

	public class TubeValidator : NavTableValidator
	{
		public TubeValidator()
		{
			Components.ITravelTubePieces.Register(OnAddLadder, OnRemoveLadder);
		}

		private void OnAddLadder(ITravelTubePiece tube)
		{
			int obj = Grid.PosToCell(tube.Position);
			if (onDirty != null)
			{
				onDirty(obj);
			}
		}

		private void OnRemoveLadder(ITravelTubePiece tube)
		{
			int obj = Grid.PosToCell(tube.Position);
			if (onDirty != null)
			{
				onDirty(obj);
			}
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			nav_table.SetValid(cell, NavType.Tube, Grid.HasTube[cell]);
		}

		public override void Clear()
		{
			Components.ITravelTubePieces.Unregister(OnAddLadder, OnRemoveLadder);
		}
	}

	public class TeleporterValidator : NavTableValidator
	{
		public TeleporterValidator()
		{
			Components.NavTeleporters.Register(OnAddTeleporter, OnRemoveTeleporter);
		}

		private void OnAddTeleporter(NavTeleporter teleporter)
		{
			int obj = Grid.PosToCell(teleporter);
			if (onDirty != null)
			{
				onDirty(obj);
			}
		}

		private void OnRemoveTeleporter(NavTeleporter teleporter)
		{
			int obj = Grid.PosToCell(teleporter);
			if (onDirty != null)
			{
				onDirty(obj);
			}
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool is_valid = Grid.IsWorldValidCell(cell) && Grid.HasNavTeleporter[cell];
			nav_table.SetValid(cell, NavType.Teleport, is_valid);
		}

		public override void Clear()
		{
			Components.NavTeleporters.Unregister(OnAddTeleporter, OnRemoveTeleporter);
		}
	}

	public class FlyingValidator : NavTableValidator
	{
		private bool exclude_floor;

		private bool exclude_jet_suit_blockers;

		private bool allow_door_traversal;

		private HandleVector<int>.Handle buildingParititonerEntry;

		public FlyingValidator(bool exclude_floor = false, bool exclude_jet_suit_blockers = false, bool allow_door_traversal = false)
		{
			this.exclude_floor = exclude_floor;
			this.exclude_jet_suit_blockers = exclude_jet_suit_blockers;
			this.allow_door_traversal = allow_door_traversal;
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(MarkCellDirty));
			World instance2 = World.Instance;
			instance2.OnLiquidChanged = (Action<int>)Delegate.Combine(instance2.OnLiquidChanged, new Action<int>(MarkCellDirty));
			GameScenePartitioner.Instance.AddGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[1], OnBuildingChange);
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool flag = false;
			if (Grid.IsWorldValidCell(Grid.CellAbove(cell)))
			{
				flag = !Grid.IsSubstantialLiquid(cell) && IsClear(cell, bounding_offsets, allow_door_traversal);
				if (flag && exclude_floor)
				{
					int cell2 = Grid.CellBelow(cell);
					if (Grid.IsWorldValidCell(cell2))
					{
						flag = IsClear(cell2, bounding_offsets, allow_door_traversal);
					}
				}
				if (flag && exclude_jet_suit_blockers)
				{
					GameObject gameObject = Grid.Objects[cell, 1];
					flag = gameObject == null || !gameObject.HasTag(GameTags.JetSuitBlocker);
				}
			}
			nav_table.SetValid(cell, NavType.Hover, flag);
		}

		private void OnBuildingChange(int cell, object data)
		{
			MarkCellDirty(cell);
		}

		private void MarkCellDirty(int cell)
		{
			if (onDirty != null)
			{
				onDirty(cell);
			}
		}

		public override void Clear()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(MarkCellDirty));
			World instance2 = World.Instance;
			instance2.OnLiquidChanged = (Action<int>)Delegate.Remove(instance2.OnLiquidChanged, new Action<int>(MarkCellDirty));
			GameScenePartitioner.Instance.RemoveGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[1], OnBuildingChange);
		}
	}

	public class HoverValidator : NavTableValidator
	{
		public HoverValidator()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(MarkCellDirty));
			World instance2 = World.Instance;
			instance2.OnLiquidChanged = (Action<int>)Delegate.Combine(instance2.OnLiquidChanged, new Action<int>(MarkCellDirty));
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			int num = Grid.CellBelow(cell);
			if (Grid.IsWorldValidCell(num))
			{
				bool flag = Grid.Solid[num] || Grid.FakeFloor[num] || Grid.IsSubstantialLiquid(num);
				nav_table.SetValid(cell, NavType.Hover, !Grid.IsSubstantialLiquid(cell) && flag && IsClear(cell, bounding_offsets, is_dupe: false));
			}
		}

		private void MarkCellDirty(int cell)
		{
			if (onDirty != null)
			{
				onDirty(cell);
			}
		}

		public override void Clear()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(MarkCellDirty));
			World instance2 = World.Instance;
			instance2.OnLiquidChanged = (Action<int>)Delegate.Remove(instance2.OnLiquidChanged, new Action<int>(MarkCellDirty));
		}
	}

	public class SolidValidator : NavTableValidator
	{
		public SolidValidator()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
		}

		public override void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
		{
			bool is_valid = IsDiggable(cell, Grid.CellBelow(cell));
			nav_table.SetValid(cell, NavType.Solid, is_valid);
		}

		public static bool IsDiggable(int cell, int anchor_cell)
		{
			if (Grid.IsWorldValidCell(cell) && Grid.Solid[cell])
			{
				if (!Grid.HasDoor[cell] && !Grid.Foundation[cell])
				{
					byte index = Grid.ElementIdx[cell];
					Element element = ElementLoader.elements[index];
					if (Grid.Element[cell].hardness < 150)
					{
						return !element.HasTag(GameTags.RefinedMetal);
					}
					return false;
				}
				GameObject gameObject = Grid.Objects[cell, 1];
				if (gameObject != null)
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					if (Grid.Element[cell].hardness < 150)
					{
						return !component.Element.HasTag(GameTags.RefinedMetal);
					}
					return false;
				}
			}
			return false;
		}

		private void OnSolidChanged(int cell)
		{
			if (onDirty != null)
			{
				onDirty(cell);
			}
		}

		public override void Clear()
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
		}
	}

	public NavGrid DuplicantGrid;

	public NavGrid WalkerGrid1x1;

	public NavGrid WalkerBabyGrid1x1;

	public NavGrid WalkerGrid1x2;

	public NavGrid DreckoGrid;

	public NavGrid DreckoBabyGrid;

	public NavGrid FloaterGrid;

	public NavGrid FlyerGrid1x2;

	public NavGrid FlyerGrid1x1;

	public NavGrid FlyerGrid2x2;

	public NavGrid SwimmerGrid;

	public NavGrid DiggerGrid;

	public NavGrid SquirrelGrid;

	public NavGrid RobotGrid;

	public GameNavGrids(Pathfinding pathfinding)
	{
		CreateDuplicantNavigation(pathfinding);
		WalkerGrid1x1 = CreateWalkerNavigation(pathfinding, "WalkerNavGrid1x1", new CellOffset[1]
		{
			new CellOffset(0, 0)
		});
		WalkerBabyGrid1x1 = CreateWalkerBabyNavigation(pathfinding, "WalkerBabyNavGrid", new CellOffset[1]
		{
			new CellOffset(0, 0)
		});
		WalkerGrid1x2 = CreateWalkerNavigation(pathfinding, "WalkerNavGrid1x2", new CellOffset[2]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		});
		CreateDreckoNavigation(pathfinding);
		CreateDreckoBabyNavigation(pathfinding);
		CreateFloaterNavigation(pathfinding);
		FlyerGrid1x1 = CreateFlyerNavigation(pathfinding, "FlyerNavGrid1x1", new CellOffset[1]
		{
			new CellOffset(0, 0)
		});
		FlyerGrid1x2 = CreateFlyerNavigation(pathfinding, "FlyerNavGrid1x2", new CellOffset[2]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		});
		FlyerGrid2x2 = CreateFlyerNavigation(pathfinding, "FlyerNavGrid2x2", new CellOffset[4]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1),
			new CellOffset(1, 0),
			new CellOffset(1, 1)
		});
		CreateSwimmerNavigation(pathfinding);
		CreateDiggerNavigation(pathfinding);
		CreateSquirrelNavigation(pathfinding);
	}

	private void CreateDuplicantNavigation(Pathfinding pathfinding)
	{
		NavOffset[] invalid_nav_offsets = new NavOffset[3]
		{
			new NavOffset(NavType.Floor, 1, 0),
			new NavOffset(NavType.Ladder, 1, 0),
			new NavOffset(NavType.Pole, 1, 0)
		};
		CellOffset[] bounding_offsets = new CellOffset[2]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		};
		NavGrid.Transition[] setA = new NavGrid.Transition[110]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 14, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 20, "", new CellOffset[3]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1),
				new CellOffset(1, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[6]
			{
				new NavOffset(NavType.Floor, 1, 0),
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, 1),
				new NavOffset(NavType.Ladder, 1, 1),
				new NavOffset(NavType.Pole, 1, 1)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 20, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], invalid_nav_offsets),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 20, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[6]
			{
				new NavOffset(NavType.Floor, 1, 0),
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, -1),
				new NavOffset(NavType.Ladder, 1, -1),
				new NavOffset(NavType.Pole, 1, -1)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 20, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 14, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 20, "", new CellOffset[2]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Teleport, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 14, "fall_pre", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Teleport, NavType.Floor, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 1, "fall_pst", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 14, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[2]
			{
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 14, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[2]
			{
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Ladder, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 20, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], invalid_nav_offsets),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 0, 0)
			}),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 14, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[2]
			{
				new NavOffset(NavType.Ladder, 0, 1),
				new NavOffset(NavType.Floor, 0, 1)
			}),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 14, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[2]
			{
				new NavOffset(NavType.Floor, 0, -1),
				new NavOffset(NavType.Ladder, 0, -1)
			}),
			new NavGrid.Transition(NavType.Ladder, NavType.Floor, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 20, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], invalid_nav_offsets),
			new NavGrid.Transition(NavType.Ladder, NavType.Ladder, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Ladder, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Ladder, 0, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Ladder, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 25, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], invalid_nav_offsets),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 50, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 50, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[2]
			{
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[2]
			{
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Pole, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 50, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], invalid_nav_offsets),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 0, 0)
			}),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[2]
			{
				new NavOffset(NavType.Pole, 0, 1),
				new NavOffset(NavType.Floor, 0, 1)
			}),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[2]
			{
				new NavOffset(NavType.Floor, 0, -1),
				new NavOffset(NavType.Pole, 0, -1)
			}),
			new NavGrid.Transition(NavType.Pole, NavType.Floor, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 20, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], invalid_nav_offsets),
			new NavGrid.Transition(NavType.Pole, NavType.Ladder, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Pole, NavType.Ladder, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Pole, NavType.Ladder, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Pole, NavType.Ladder, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 20, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], invalid_nav_offsets),
			new NavGrid.Transition(NavType.Ladder, NavType.Pole, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Pole, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 50, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Pole, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ladder, NavType.Pole, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 20, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], invalid_nav_offsets),
			new NavGrid.Transition(NavType.Pole, NavType.Pole, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Pole, NavType.Pole, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 50, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Pole, NavType.Pole, 0, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: false, 6, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Pole, NavType.Pole, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 50, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], invalid_nav_offsets),
			new NavGrid.Transition(NavType.Floor, NavType.Tube, 0, 2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 40, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 7, "", new CellOffset[1]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 2, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 13, "", new CellOffset[2]
			{
				new CellOffset(0, 1),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, 1)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 1, 2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 13, "", new CellOffset[2]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 7, "", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 1, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 13, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 2, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 13, "", new CellOffset[3]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, -1)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 2, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 17, "", new CellOffset[4]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0),
				new CellOffset(1, -1),
				new CellOffset(2, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, -2)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Floor, 0, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[1]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 0, 2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[1]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Ladder, 0, 1)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 7, "", new CellOffset[1]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 2, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 13, "", new CellOffset[2]
			{
				new CellOffset(0, 1),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, 1)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 1, 2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 13, "", new CellOffset[2]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 7, "", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 1, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 13, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 2, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 13, "", new CellOffset[3]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, -1)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 2, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 17, "", new CellOffset[4]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0),
				new CellOffset(1, -1),
				new CellOffset(2, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, -2)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Ladder, 0, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[1]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 0, 2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[1]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Pole, 0, 1)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 7, "", new CellOffset[1]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 2, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 13, "", new CellOffset[2]
			{
				new CellOffset(0, 1),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, 1)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 1, 2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 13, "", new CellOffset[2]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 7, "", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 1, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 13, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 2, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 13, "", new CellOffset[3]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, -1)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 2, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 17, "", new CellOffset[4]
			{
				new CellOffset(1, 0),
				new CellOffset(2, 0),
				new CellOffset(1, -1),
				new CellOffset(2, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, -2)
			}),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Pole, 0, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[1]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: false, is_escape: false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: false, is_escape: false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 0, -1, NavAxis.NA, is_looping: true, loop_has_pre: false, is_escape: false, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 1, 1, NavAxis.Y, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Tube, 0, 1)
			}, new NavOffset[0], critter: false, 2.2f),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 1, 1, NavAxis.X, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Tube, 1, 0)
			}, new NavOffset[0], critter: false, 2.2f),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 1, -1, NavAxis.Y, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Tube, 0, -1)
			}, new NavOffset[0], critter: false, 2.2f),
			new NavGrid.Transition(NavType.Tube, NavType.Tube, 1, -1, NavAxis.X, is_looping: false, loop_has_pre: false, is_escape: false, 10, "", new CellOffset[0], new CellOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Tube, 1, 0)
			}, new NavOffset[0], critter: false, 2.2f),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: false, is_escape: false, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: false, is_escape: false, 15, "hover_hover_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, -1, NavAxis.NA, is_looping: true, loop_has_pre: false, is_escape: false, 15, "hover_hover_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 25, "", new CellOffset[0], new CellOffset[0], new NavOffset[2]
			{
				new NavOffset(NavType.Hover, 1, 0),
				new NavOffset(NavType.Hover, 0, 1)
			}, new NavOffset[0]),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 25, "", new CellOffset[0], new CellOffset[0], new NavOffset[2]
			{
				new NavOffset(NavType.Hover, 1, 0),
				new NavOffset(NavType.Hover, 0, -1)
			}, new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Hover, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Hover, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 20, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Hover, NavType.Floor, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Hover, NavType.Floor, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0])
		};
		NavGrid.Transition[] setB = new NavGrid.Transition[2]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 30, "climb_down_2_-1", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[1]
			{
				new CellOffset(1, 1)
			}, new NavOffset[0], new NavOffset[6]
			{
				new NavOffset(NavType.Floor, 1, 0),
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, -1),
				new NavOffset(NavType.Ladder, 1, -1),
				new NavOffset(NavType.Pole, 1, -1)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 30, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[1]
			{
				new CellOffset(1, 2)
			}, new NavOffset[0], new NavOffset[6]
			{
				new NavOffset(NavType.Floor, 1, 0),
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, 1),
				new NavOffset(NavType.Ladder, 1, 1),
				new NavOffset(NavType.Pole, 1, 1)
			})
		};
		NavGrid.Transition[] transitions = MirrorTransitions(CombineTransitions(setA, setB));
		NavGrid.NavTypeData[] nav_type_data = new NavGrid.NavTypeData[6]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_default"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Ladder,
				idleAnim = "ladder_idle"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Pole,
				idleAnim = "pole_idle"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Tube,
				idleAnim = "tube_idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Hover,
				idleAnim = "hover_hover_1_0_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Teleport,
				idleAnim = "idle_default"
			}
		};
		DuplicantGrid = new NavGrid("MinionNavGrid", transitions, nav_type_data, bounding_offsets, new NavTableValidator[6]
		{
			new FloorValidator(is_dupe: true),
			new LadderValidator(),
			new PoleValidator(),
			new TubeValidator(),
			new TeleporterValidator(),
			new FlyingValidator(exclude_floor: true, exclude_jet_suit_blockers: true, allow_door_traversal: true)
		}, 2, 3, 32);
		DuplicantGrid.updateEveryFrame = true;
		pathfinding.AddNavGrid(DuplicantGrid);
		NavGrid.Transition[] setB2 = new NavGrid.Transition[2]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 30, "climb_down_2_-1", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[1]
			{
				new CellOffset(1, 1)
			}, new NavOffset[0], new NavOffset[6]
			{
				new NavOffset(NavType.Floor, 1, 0),
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, -1),
				new NavOffset(NavType.Ladder, 1, -1),
				new NavOffset(NavType.Pole, 1, -1)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: false, 30, "climb_up_2_1", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1)
			}, new CellOffset[1]
			{
				new CellOffset(1, 2)
			}, new NavOffset[0], new NavOffset[6]
			{
				new NavOffset(NavType.Floor, 1, 0),
				new NavOffset(NavType.Ladder, 1, 0),
				new NavOffset(NavType.Pole, 1, 0),
				new NavOffset(NavType.Floor, 1, 1),
				new NavOffset(NavType.Ladder, 1, 1),
				new NavOffset(NavType.Pole, 1, 1)
			})
		};
		NavGrid.Transition[] transitions2 = MirrorTransitions(CombineTransitions(setA, setB2));
		RobotGrid = new NavGrid("RobotNavGrid", transitions2, nav_type_data, bounding_offsets, new NavTableValidator[2]
		{
			new FloorValidator(is_dupe: true),
			new LadderValidator()
		}, 2, 3, 22);
		RobotGrid.updateEveryFrame = true;
		pathfinding.AddNavGrid(RobotGrid);
	}

	private NavGrid CreateWalkerNavigation(Pathfinding pathfinding, string id, CellOffset[] bounding_offsets)
	{
		NavGrid.Transition[] transitions = new NavGrid.Transition[6]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[1]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[2]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true)
		};
		NavGrid.Transition[] array = MirrorTransitions(transitions);
		NavGrid.NavTypeData[] nav_type_data = new NavGrid.NavTypeData[1]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_loop"
			}
		};
		NavGrid navGrid = new NavGrid(id, array, nav_type_data, bounding_offsets, new NavTableValidator[1]
		{
			new FloorValidator(is_dupe: false)
		}, 2, 3, array.Length);
		pathfinding.AddNavGrid(navGrid);
		return navGrid;
	}

	private NavGrid CreateWalkerBabyNavigation(Pathfinding pathfinding, string id, CellOffset[] bounding_offsets)
	{
		NavGrid.Transition[] transitions = new NavGrid.Transition[1]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0])
		};
		NavGrid.Transition[] array = MirrorTransitions(transitions);
		NavGrid.NavTypeData[] nav_type_data = new NavGrid.NavTypeData[1]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_loop"
			}
		};
		NavGrid navGrid = new NavGrid(id, array, nav_type_data, bounding_offsets, new NavTableValidator[1]
		{
			new FloorValidator(is_dupe: false)
		}, 2, 3, array.Length);
		pathfinding.AddNavGrid(navGrid);
		return navGrid;
	}

	private void CreateDreckoNavigation(Pathfinding pathfinding)
	{
		CellOffset[] bounding_offsets = new CellOffset[1]
		{
			new CellOffset(0, 0)
		};
		NavGrid.Transition[] transitions = new NavGrid.Transition[24]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 3, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 4, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.LeftWall, 1, -2)
			}, critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 5, "", new CellOffset[2]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, 2)
			}, new NavOffset[1]
			{
				new NavOffset(NavType.RightWall, 0, 0)
			}, critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 4, "", new CellOffset[2]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[2]
			{
				new NavOffset(NavType.RightWall, 0, 0),
				new NavOffset(NavType.Floor, 2, 2)
			}, critter: true),
			new NavGrid.Transition(NavType.RightWall, NavType.RightWall, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.LeftWall, NavType.LeftWall, 0, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ceiling, NavType.Ceiling, -1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.RightWall, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_1", new CellOffset[0], new CellOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.RightWall, 0, 0)
			}, new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.RightWall, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.RightWall, 0, 1)
			}),
			new NavGrid.Transition(NavType.RightWall, NavType.Ceiling, -1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_1", new CellOffset[0], new CellOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Ceiling, 0, 0)
			}, new NavOffset[0]),
			new NavGrid.Transition(NavType.RightWall, NavType.Ceiling, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Ceiling, -1, 0)
			}),
			new NavGrid.Transition(NavType.Ceiling, NavType.LeftWall, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_1", new CellOffset[0], new CellOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.LeftWall, 0, 0)
			}, new NavOffset[0]),
			new NavGrid.Transition(NavType.Ceiling, NavType.LeftWall, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.LeftWall, 0, -1)
			}),
			new NavGrid.Transition(NavType.LeftWall, NavType.Floor, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_1", new CellOffset[0], new CellOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 0, 0)
			}, new NavOffset[0]),
			new NavGrid.Transition(NavType.LeftWall, NavType.Floor, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, 0)
			}),
			new NavGrid.Transition(NavType.Floor, NavType.LeftWall, 1, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 2, "floor_wall_1_-2", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.LeftWall, 1, -1)
			}, new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.LeftWall, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.LeftWall, 1, -2)
			}, critter: true),
			new NavGrid.Transition(NavType.LeftWall, NavType.Ceiling, -2, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 2, "floor_wall_1_-2", new CellOffset[2]
			{
				new CellOffset(0, -1),
				new CellOffset(-1, -1)
			}, new CellOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Ceiling, -1, -1)
			}, new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.LeftWall, NavType.Ceiling, -1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Ceiling, -2, -1)
			}, critter: true),
			new NavGrid.Transition(NavType.Ceiling, NavType.RightWall, -1, 2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 2, "floor_wall_1_-2", new CellOffset[2]
			{
				new CellOffset(-1, 0),
				new CellOffset(-1, 1)
			}, new CellOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.RightWall, -1, 1)
			}, new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Ceiling, NavType.RightWall, -1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(-1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.RightWall, -1, 2)
			}, critter: true),
			new NavGrid.Transition(NavType.RightWall, NavType.Floor, 2, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 2, "floor_wall_1_-2", new CellOffset[2]
			{
				new CellOffset(0, 1),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 1, 1)
			}, new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.RightWall, NavType.Floor, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Floor, 2, 1)
			}, critter: true)
		};
		NavGrid.Transition[] transitions2 = MirrorTransitions(transitions);
		NavGrid.NavTypeData[] nav_type_data = new NavGrid.NavTypeData[4]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.RightWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0.5f, -0.5f, 0f),
				rotation = -(float)Math.PI / 2f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Ceiling,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0f, -1f, 0f),
				rotation = -(float)Math.PI
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.LeftWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(-0.5f, -0.5f, 0f),
				rotation = -4.712389f
			}
		};
		DreckoGrid = new NavGrid("DreckoNavGrid", transitions2, nav_type_data, bounding_offsets, new NavTableValidator[3]
		{
			new FloorValidator(is_dupe: false),
			new WallValidator(),
			new CeilingValidator()
		}, 2, 3, 16);
		pathfinding.AddNavGrid(DreckoGrid);
	}

	private void CreateDreckoBabyNavigation(Pathfinding pathfinding)
	{
		CellOffset[] bounding_offsets = new CellOffset[1]
		{
			new CellOffset(0, 0)
		};
		NavGrid.Transition[] transitions = new NavGrid.Transition[12]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.RightWall, NavType.RightWall, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.LeftWall, NavType.LeftWall, 0, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ceiling, NavType.Ceiling, -1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.RightWall, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.RightWall, NavType.Ceiling, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ceiling, NavType.LeftWall, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.LeftWall, NavType.Floor, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.LeftWall, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.LeftWall, NavType.Ceiling, -1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Ceiling, NavType.RightWall, -1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(-1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.RightWall, NavType.Floor, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true)
		};
		NavGrid.Transition[] transitions2 = MirrorTransitions(transitions);
		NavGrid.NavTypeData[] nav_type_data = new NavGrid.NavTypeData[4]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.RightWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0.5f, -0.5f, 0f),
				rotation = -(float)Math.PI / 2f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Ceiling,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0f, -1f, 0f),
				rotation = -(float)Math.PI
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.LeftWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(-0.5f, -0.5f, 0f),
				rotation = -4.712389f
			}
		};
		DreckoBabyGrid = new NavGrid("DreckoBabyNavGrid", transitions2, nav_type_data, bounding_offsets, new NavTableValidator[3]
		{
			new FloorValidator(is_dupe: false),
			new WallValidator(),
			new CeilingValidator()
		}, 2, 3, 16);
		pathfinding.AddNavGrid(DreckoBabyGrid);
	}

	private void CreateFloaterNavigation(Pathfinding pathfinding)
	{
		CellOffset[] bounding_offsets = new CellOffset[1]
		{
			new CellOffset(0, 0)
		};
		NavGrid.Transition[] transitions = new NavGrid.Transition[17]
		{
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Hover, 1, -1)
			}),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[1]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Hover, 1, 0)
			}, critter: true),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Hover, 1, -2)
			}, critter: true),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Hover, 0, 0)
			}),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Hover, 0, -2)
			}),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 2, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 3, "", new CellOffset[3]
			{
				new CellOffset(1, 0),
				new CellOffset(1, 1),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Hover, 2, 0)
			}, critter: true),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 3, "", new CellOffset[3]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1),
				new CellOffset(1, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Hover, 2, -1)
			}, critter: true),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 2, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 3, "", new CellOffset[3]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1),
				new CellOffset(1, -2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Hover, 2, -2)
			}, critter: true),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 3, "", new CellOffset[2]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Hover, 1, 1)
			}, critter: true),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 3, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[1]
			{
				new NavOffset(NavType.Hover, 1, -3)
			}, critter: true),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 2, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 2, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 10, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 10, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Hover, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Hover, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0])
		};
		NavGrid.Transition[] transitions2 = MirrorTransitions(transitions);
		NavGrid.NavTypeData[] nav_type_data = new NavGrid.NavTypeData[2]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Hover,
				idleAnim = "idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Swim,
				idleAnim = "swim_idle_loop"
			}
		};
		FloaterGrid = new NavGrid("FloaterNavGrid", transitions2, nav_type_data, bounding_offsets, new NavTableValidator[2]
		{
			new HoverValidator(),
			new SwimValidator()
		}, 2, 2, 22);
		pathfinding.AddNavGrid(FloaterGrid);
	}

	private NavGrid CreateFlyerNavigation(Pathfinding pathfinding, string id, CellOffset[] bounding_offsets)
	{
		NavGrid.Transition[] transitions = new NavGrid.Transition[12]
		{
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 2, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 2, "hover_hover_1_0", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 1, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 2, "hover_hover_1_0", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 3, "hover_hover_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Hover, NavType.Hover, 0, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 3, "hover_hover_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 5, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 2, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 2, "swim_swim_1_0", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 10, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 10, "swim_swim_1_0", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Hover, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Hover, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0])
		};
		NavGrid.Transition[] transitions2 = MirrorTransitions(transitions);
		NavGrid.NavTypeData[] nav_type_data = new NavGrid.NavTypeData[2]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Hover,
				idleAnim = "idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Swim,
				idleAnim = "idle_loop"
			}
		};
		NavGrid navGrid = new NavGrid(id, transitions2, nav_type_data, bounding_offsets, new NavTableValidator[2]
		{
			new FlyingValidator(),
			new SwimValidator()
		}, 2, 2, 16);
		pathfinding.AddNavGrid(navGrid);
		return navGrid;
	}

	private void CreateSwimmerNavigation(Pathfinding pathfinding)
	{
		CellOffset[] bounding_offsets = new CellOffset[1]
		{
			new CellOffset(0, 0)
		};
		NavGrid.Transition[] transitions = new NavGrid.Transition[5]
		{
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 2, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 2, "swim_swim_1_0", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 1, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 2, "swim_swim_1_0", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 3, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Swim, NavType.Swim, 0, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 3, "swim_swim_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0])
		};
		NavGrid.Transition[] array = MirrorTransitions(transitions);
		NavGrid.NavTypeData[] nav_type_data = new NavGrid.NavTypeData[1]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Swim,
				idleAnim = "idle_loop"
			}
		};
		SwimmerGrid = new NavGrid("SwimmerNavGrid", array, nav_type_data, bounding_offsets, new NavTableValidator[1]
		{
			new SwimValidator()
		}, 1, 1, array.Length);
		pathfinding.AddNavGrid(SwimmerGrid);
	}

	private void CreateDiggerNavigation(Pathfinding pathfinding)
	{
		CellOffset[] bounding_offsets = new CellOffset[1]
		{
			new CellOffset(0, 0)
		};
		NavGrid.Transition[] transitions = new NavGrid.Transition[24]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.RightWall, NavType.RightWall, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.LeftWall, NavType.LeftWall, 0, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ceiling, NavType.Ceiling, -1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.RightWall, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.RightWall, NavType.Ceiling, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ceiling, NavType.LeftWall, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.LeftWall, NavType.Floor, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.LeftWall, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.LeftWall, NavType.Ceiling, -1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Ceiling, NavType.RightWall, -1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(-1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.RightWall, NavType.Floor, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Solid, NavType.Solid, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "idle1", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Solid, NavType.Solid, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "idle2", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Solid, NavType.Solid, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "idle3", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Solid, NavType.Solid, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "idle4", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.Solid, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: true, is_escape: true, 1, "drill_in", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Solid, NavType.Floor, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "drill_out", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ceiling, NavType.Solid, 0, 1, NavAxis.NA, is_looping: false, loop_has_pre: true, is_escape: true, 1, "drill_in", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Solid, NavType.Ceiling, 0, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "drill_out_ceiling", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Solid, NavType.LeftWall, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "drill_out_left_wall", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.LeftWall, NavType.Solid, -1, 0, NavAxis.NA, is_looping: false, loop_has_pre: true, is_escape: true, 1, "drill_in", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Solid, NavType.RightWall, -1, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "drill_out_right_wall", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.RightWall, NavType.Solid, 1, 0, NavAxis.NA, is_looping: false, loop_has_pre: true, is_escape: true, 1, "drill_in", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0])
		};
		NavGrid.Transition[] transitions2 = MirrorTransitions(transitions);
		NavGrid.NavTypeData[] nav_type_data = new NavGrid.NavTypeData[5]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Ceiling,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0f, -1f, 0f),
				rotation = -(float)Math.PI
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.RightWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0.5f, -0.5f, 0f),
				rotation = -(float)Math.PI / 2f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.LeftWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(-0.5f, -0.5f, 0f),
				rotation = -4.712389f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Solid,
				idleAnim = "idle1"
			}
		};
		DiggerGrid = new NavGrid("DiggerNavGrid", transitions2, nav_type_data, bounding_offsets, new NavTableValidator[4]
		{
			new SolidValidator(),
			new FloorValidator(is_dupe: false),
			new WallValidator(),
			new CeilingValidator()
		}, 2, 3, 22);
		pathfinding.AddNavGrid(DiggerGrid);
	}

	private void CreateSquirrelNavigation(Pathfinding pathfinding)
	{
		CellOffset[] bounding_offsets = new CellOffset[1]
		{
			new CellOffset(0, 0)
		};
		NavGrid.Transition[] transitions = new NavGrid.Transition[17]
		{
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 2, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[1]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, 2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[2]
			{
				new CellOffset(0, 1),
				new CellOffset(0, 2)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Floor, NavType.Floor, 1, -2, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "", new CellOffset[2]
			{
				new CellOffset(1, 0),
				new CellOffset(1, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.RightWall, NavType.RightWall, 0, 1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.LeftWall, NavType.LeftWall, 0, -1, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ceiling, NavType.Ceiling, -1, 0, NavAxis.NA, is_looping: true, loop_has_pre: true, is_escape: true, 1, "floor_floor_1_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.RightWall, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.RightWall, NavType.Ceiling, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Ceiling, NavType.LeftWall, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.LeftWall, NavType.Floor, 0, 0, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_0_0", new CellOffset[0], new CellOffset[0], new NavOffset[0], new NavOffset[0]),
			new NavGrid.Transition(NavType.Floor, NavType.LeftWall, 1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.LeftWall, NavType.Ceiling, -1, -1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(0, -1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.Ceiling, NavType.RightWall, -1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(-1, 0)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true),
			new NavGrid.Transition(NavType.RightWall, NavType.Floor, 1, 1, NavAxis.NA, is_looping: false, loop_has_pre: false, is_escape: true, 1, "floor_wall_1_-1", new CellOffset[1]
			{
				new CellOffset(0, 1)
			}, new CellOffset[0], new NavOffset[0], new NavOffset[0], critter: true)
		};
		NavGrid.Transition[] transitions2 = MirrorTransitions(transitions);
		NavGrid.NavTypeData[] nav_type_data = new NavGrid.NavTypeData[4]
		{
			new NavGrid.NavTypeData
			{
				navType = NavType.Floor,
				idleAnim = "idle_loop"
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.Ceiling,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0f, -1f, 0f),
				rotation = -(float)Math.PI
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.RightWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(0.5f, -0.5f, 0f),
				rotation = -(float)Math.PI / 2f
			},
			new NavGrid.NavTypeData
			{
				navType = NavType.LeftWall,
				idleAnim = "idle_loop",
				animControllerOffset = new Vector3(-0.5f, -0.5f, 0f),
				rotation = -4.712389f
			}
		};
		SquirrelGrid = new NavGrid("SquirrelNavGrid", transitions2, nav_type_data, bounding_offsets, new NavTableValidator[3]
		{
			new FloorValidator(is_dupe: false),
			new WallValidator(),
			new CeilingValidator()
		}, 2, 3, 20);
		pathfinding.AddNavGrid(SquirrelGrid);
	}

	private CellOffset[] MirrorOffsets(CellOffset[] offsets)
	{
		List<CellOffset> list = new List<CellOffset>();
		for (int i = 0; i < offsets.Length; i++)
		{
			CellOffset item = offsets[i];
			item.x = -item.x;
			list.Add(item);
		}
		return list.ToArray();
	}

	private NavOffset[] MirrorNavOffsets(NavOffset[] offsets)
	{
		List<NavOffset> list = new List<NavOffset>();
		for (int i = 0; i < offsets.Length; i++)
		{
			NavOffset item = offsets[i];
			item.navType = NavGrid.MirrorNavType(item.navType);
			item.offset.x = -item.offset.x;
			list.Add(item);
		}
		return list.ToArray();
	}

	private NavGrid.Transition[] MirrorTransitions(NavGrid.Transition[] transitions)
	{
		List<NavGrid.Transition> list = new List<NavGrid.Transition>();
		for (int i = 0; i < transitions.Length; i++)
		{
			NavGrid.Transition transition = transitions[i];
			list.Add(transition);
			if (transition.x != 0 || transition.start == NavType.RightWall || transition.end == NavType.RightWall || transition.start == NavType.LeftWall || transition.end == NavType.LeftWall)
			{
				NavGrid.Transition item = transition;
				item.x = -item.x;
				item.voidOffsets = MirrorOffsets(transition.voidOffsets);
				item.solidOffsets = MirrorOffsets(transition.solidOffsets);
				item.validNavOffsets = MirrorNavOffsets(transition.validNavOffsets);
				item.invalidNavOffsets = MirrorNavOffsets(transition.invalidNavOffsets);
				item.start = NavGrid.MirrorNavType(item.start);
				item.end = NavGrid.MirrorNavType(item.end);
				list.Add(item);
			}
		}
		list.Sort((NavGrid.Transition x, NavGrid.Transition y) => x.cost.CompareTo(y.cost));
		return list.ToArray();
	}

	private NavGrid.Transition[] CombineTransitions(NavGrid.Transition[] setA, NavGrid.Transition[] setB)
	{
		NavGrid.Transition[] array = new NavGrid.Transition[setA.Length + setB.Length];
		Array.Copy(setA, array, setA.Length);
		Array.Copy(setB, 0, array, setA.Length, setB.Length);
		Array.Sort(array, (NavGrid.Transition x, NavGrid.Transition y) => x.cost.CompareTo(y.cost));
		return array;
	}
}
