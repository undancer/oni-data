using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ProcGen;
using UnityEngine;

public class Grid
{
	[Flags]
	public enum BuildFlags : byte
	{
		Solid = 0x1,
		Foundation = 0x2,
		Door = 0x4,
		FakeFloor = 0x8,
		DupePassable = 0x10,
		DupeImpassable = 0x20,
		CritterImpassable = 0x40
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsFoundationIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.Foundation) != 0;
			}
			set
			{
				UpdateBuildMask(i, BuildFlags.Foundation, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsSolidIndexer
	{
		public bool this[int i] => (BuildMasks[i] & BuildFlags.Solid) != 0;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsDupeImpassableIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.DupeImpassable) != 0;
			}
			set
			{
				UpdateBuildMask(i, BuildFlags.DupeImpassable, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsFakeFloorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.FakeFloor) != 0;
			}
			set
			{
				UpdateBuildMask(i, BuildFlags.FakeFloor, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsDupePassableIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.DupePassable) != 0;
			}
			set
			{
				UpdateBuildMask(i, BuildFlags.DupePassable, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsImpassableIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.CritterImpassable) != 0;
			}
			set
			{
				UpdateBuildMask(i, BuildFlags.CritterImpassable, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsDoorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.Door) != 0;
			}
			set
			{
				UpdateBuildMask(i, BuildFlags.Door, value);
			}
		}
	}

	[Flags]
	public enum VisFlags : byte
	{
		Revealed = 0x1,
		PreventFogOfWarReveal = 0x2,
		RenderedByWorld = 0x4,
		AllowPathfinding = 0x8
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct VisFlagsRevealedIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (VisMasks[i] & VisFlags.Revealed) != 0;
			}
			set
			{
				UpdateVisMask(i, VisFlags.Revealed, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct VisFlagsPreventFogOfWarRevealIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (VisMasks[i] & VisFlags.PreventFogOfWarReveal) != 0;
			}
			set
			{
				UpdateVisMask(i, VisFlags.PreventFogOfWarReveal, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct VisFlagsRenderedByWorldIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (VisMasks[i] & VisFlags.RenderedByWorld) != 0;
			}
			set
			{
				UpdateVisMask(i, VisFlags.RenderedByWorld, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct VisFlagsAllowPathfindingIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (VisMasks[i] & VisFlags.AllowPathfinding) != 0;
			}
			set
			{
				UpdateVisMask(i, VisFlags.AllowPathfinding, value);
			}
		}
	}

	[Flags]
	public enum NavValidatorFlags : byte
	{
		Ladder = 0x1,
		Pole = 0x2,
		Tube = 0x4,
		NavTeleporter = 0x8,
		UnderConstruction = 0x10
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavValidatorFlagsLadderIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavValidatorMasks[i] & NavValidatorFlags.Ladder) != 0;
			}
			set
			{
				UpdateNavValidatorMask(i, NavValidatorFlags.Ladder, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavValidatorFlagsPoleIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavValidatorMasks[i] & NavValidatorFlags.Pole) != 0;
			}
			set
			{
				UpdateNavValidatorMask(i, NavValidatorFlags.Pole, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavValidatorFlagsTubeIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavValidatorMasks[i] & NavValidatorFlags.Tube) != 0;
			}
			set
			{
				UpdateNavValidatorMask(i, NavValidatorFlags.Tube, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavValidatorFlagsNavTeleporterIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavValidatorMasks[i] & NavValidatorFlags.NavTeleporter) != 0;
			}
			set
			{
				UpdateNavValidatorMask(i, NavValidatorFlags.NavTeleporter, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavValidatorFlagsUnderConstructionIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavValidatorMasks[i] & NavValidatorFlags.UnderConstruction) != 0;
			}
			set
			{
				UpdateNavValidatorMask(i, NavValidatorFlags.UnderConstruction, value);
			}
		}
	}

	[Flags]
	public enum NavFlags : byte
	{
		AccessDoor = 0x1,
		TubeEntrance = 0x2,
		PreventIdleTraversal = 0x4,
		Reserved = 0x8,
		SuitMarker = 0x10
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavFlagsAccessDoorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavMasks[i] & NavFlags.AccessDoor) != 0;
			}
			set
			{
				UpdateNavMask(i, NavFlags.AccessDoor, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavFlagsTubeEntranceIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavMasks[i] & NavFlags.TubeEntrance) != 0;
			}
			set
			{
				UpdateNavMask(i, NavFlags.TubeEntrance, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavFlagsPreventIdleTraversalIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavMasks[i] & NavFlags.PreventIdleTraversal) != 0;
			}
			set
			{
				UpdateNavMask(i, NavFlags.PreventIdleTraversal, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavFlagsReservedIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavMasks[i] & NavFlags.Reserved) != 0;
			}
			set
			{
				UpdateNavMask(i, NavFlags.Reserved, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavFlagsSuitMarkerIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavMasks[i] & NavFlags.SuitMarker) != 0;
			}
			set
			{
				UpdateNavMask(i, NavFlags.SuitMarker, value);
			}
		}
	}

	public struct Restriction
	{
		[Flags]
		public enum Directions : byte
		{
			Left = 0x1,
			Right = 0x2,
			Teleport = 0x4
		}

		public enum Orientation : byte
		{
			Vertical,
			Horizontal,
			SingleCell
		}

		public const int DefaultID = -1;

		public Dictionary<int, Directions> DirectionMasksForMinionInstanceID;

		public Orientation orientation;
	}

	private struct TubeEntrance
	{
		public bool operational;

		public int reservationCapacity;

		public HashSet<int> reservedInstanceIDs;
	}

	public struct SuitMarker
	{
		[Flags]
		public enum Flags : byte
		{
			OnlyTraverseIfUnequipAvailable = 0x1,
			Operational = 0x2,
			Rotated = 0x4
		}

		public int suitCount;

		public int lockerCount;

		public Flags flags;

		public PathFinder.PotentialPath.Flags pathFlags;

		public HashSet<int> minionIDsWithSuitReservations;

		public HashSet<int> minionIDsWithEmptyLockerReservations;

		public int emptyLockerCount => lockerCount - suitCount;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ObjectLayerIndexer
	{
		public GameObject this[int cell, int layer]
		{
			get
			{
				GameObject value = null;
				ObjectLayers[layer].TryGetValue(cell, out value);
				return value;
			}
			set
			{
				if (value == null)
				{
					ObjectLayers[layer].Remove(cell);
				}
				else
				{
					ObjectLayers[layer][cell] = value;
				}
				GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.objectLayers[layer], value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct PressureIndexer
	{
		public unsafe float this[int i] => mass[i] * 101.3f;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct TransparentIndexer
	{
		public unsafe bool this[int i] => (properties[i] & 0x10) != 0;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ElementIdxIndexer
	{
		public unsafe byte this[int i] => elementIdx[i];
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct TemperatureIndexer
	{
		public unsafe float this[int i] => temperature[i];
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct RadiationIndexer
	{
		public unsafe float this[int i] => radiation[i];
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct MassIndexer
	{
		public unsafe float this[int i] => mass[i];
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct PropertiesIndexer
	{
		public unsafe byte this[int i] => properties[i];
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ExposedToSunlightIndexer
	{
		public unsafe byte this[int i] => exposedToSunlight[i];
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct StrengthInfoIndexer
	{
		public unsafe byte this[int i] => strengthInfo[i];
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Insulationndexer
	{
		public unsafe byte this[int i] => insulation[i];
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct DiseaseIdxIndexer
	{
		public unsafe byte this[int i] => diseaseIdx[i];
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct DiseaseCountIndexer
	{
		public unsafe int this[int i] => diseaseCount[i];
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct AccumulatedFlowIndexer
	{
		public unsafe float this[int i] => AccumulatedFlowValues[i];
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct LightIntensityIndexer
	{
		public unsafe int this[int i]
		{
			get
			{
				int num = (int)((float)(int)exposedToSunlight[i] / 255f * Game.Instance.currentSunlightIntensity);
				int num2 = LightCount[i];
				return num + num2;
			}
		}
	}

	public enum SceneLayer
	{
		WorldSelection = -3,
		NoLayer = -2,
		Background = -1,
		Backwall = 1,
		Gas = 2,
		GasConduits = 3,
		GasConduitBridges = 4,
		LiquidConduits = 5,
		LiquidConduitBridges = 6,
		SolidConduits = 7,
		SolidConduitContents = 8,
		SolidConduitBridges = 9,
		Wires = 10,
		WireBridges = 11,
		WireBridgesFront = 12,
		LogicWires = 13,
		LogicGates = 14,
		LogicGatesFront = 0xF,
		InteriorWall = 0x10,
		GasFront = 17,
		BuildingBack = 18,
		Building = 19,
		BuildingUse = 20,
		BuildingFront = 21,
		TransferArm = 22,
		Ore = 23,
		Creatures = 24,
		Move = 25,
		Front = 26,
		GlassTile = 27,
		Liquid = 28,
		Ground = 29,
		TileMain = 30,
		TileFront = 0x1F,
		FXFront = 0x20,
		FXFront2 = 33,
		SceneMAX = 34
	}

	public static readonly CellOffset[] DefaultOffset = new CellOffset[1];

	public static float WidthInMeters;

	public static float HeightInMeters;

	public static int WidthInCells;

	public static int HeightInCells;

	public static float CellSizeInMeters;

	public static float InverseCellSizeInMeters;

	public static float HalfCellSizeInMeters;

	public static int CellCount;

	public static int InvalidCell = -1;

	public static int TopBorderHeight = 2;

	public static Dictionary<int, GameObject>[] ObjectLayers;

	public static Action<int> OnReveal;

	public static BuildFlags[] BuildMasks;

	public static BuildFlagsFoundationIndexer Foundation;

	public static BuildFlagsSolidIndexer Solid;

	public static BuildFlagsDupeImpassableIndexer DupeImpassable;

	public static BuildFlagsFakeFloorIndexer FakeFloor;

	public static BuildFlagsDupePassableIndexer DupePassable;

	public static BuildFlagsImpassableIndexer CritterImpassable;

	public static BuildFlagsDoorIndexer HasDoor;

	public static VisFlags[] VisMasks;

	public static VisFlagsRevealedIndexer Revealed;

	public static VisFlagsPreventFogOfWarRevealIndexer PreventFogOfWarReveal;

	public static VisFlagsRenderedByWorldIndexer RenderedByWorld;

	public static VisFlagsAllowPathfindingIndexer AllowPathfinding;

	public static NavValidatorFlags[] NavValidatorMasks;

	public static NavValidatorFlagsLadderIndexer HasLadder;

	public static NavValidatorFlagsPoleIndexer HasPole;

	public static NavValidatorFlagsTubeIndexer HasTube;

	public static NavValidatorFlagsNavTeleporterIndexer HasNavTeleporter;

	public static NavValidatorFlagsUnderConstructionIndexer IsTileUnderConstruction;

	public static NavFlags[] NavMasks;

	private static NavFlagsAccessDoorIndexer HasAccessDoor;

	public static NavFlagsTubeEntranceIndexer HasTubeEntrance;

	public static NavFlagsPreventIdleTraversalIndexer PreventIdleTraversal;

	public static NavFlagsReservedIndexer Reserved;

	public static NavFlagsSuitMarkerIndexer HasSuitMarker;

	private static Dictionary<int, Restriction> restrictions = new Dictionary<int, Restriction>();

	private static Dictionary<int, TubeEntrance> tubeEntrances = new Dictionary<int, TubeEntrance>();

	private static Dictionary<int, SuitMarker> suitMarkers = new Dictionary<int, SuitMarker>();

	public unsafe static byte* elementIdx;

	public unsafe static float* temperature;

	public unsafe static float* radiation;

	public unsafe static float* mass;

	public unsafe static byte* properties;

	public unsafe static byte* strengthInfo;

	public unsafe static byte* insulation;

	public unsafe static byte* diseaseIdx;

	public unsafe static int* diseaseCount;

	public unsafe static byte* exposedToSunlight;

	public unsafe static float* AccumulatedFlowValues = null;

	public static byte[] Visible;

	public static byte[] Spawnable;

	public static float[] Damage;

	public static float[] Decor;

	public static bool[] GravitasFacility;

	public static byte[] WorldIdx;

	public static float[] Loudness;

	public static Element[] Element;

	public static int[] LightCount;

	public static PressureIndexer Pressure;

	public static TransparentIndexer Transparent;

	public static ElementIdxIndexer ElementIdx;

	public static TemperatureIndexer Temperature;

	public static RadiationIndexer Radiation;

	public static MassIndexer Mass;

	public static PropertiesIndexer Properties;

	public static ExposedToSunlightIndexer ExposedToSunlight;

	public static StrengthInfoIndexer StrengthInfo;

	public static Insulationndexer Insulation;

	public static DiseaseIdxIndexer DiseaseIdx;

	public static DiseaseCountIndexer DiseaseCount;

	public static LightIntensityIndexer LightIntensity;

	public static AccumulatedFlowIndexer AccumulatedFlow;

	public static ObjectLayerIndexer Objects;

	public static float LayerMultiplier = 1f;

	private static readonly Func<int, bool> VisibleBlockingDelegate = (int cell) => VisibleBlockingCB(cell);

	private static readonly Func<int, bool> PhysicalBlockingDelegate = (int cell) => PhysicalBlockingCB(cell);

	private static void UpdateBuildMask(int i, BuildFlags flag, bool state)
	{
		if (state)
		{
			BuildMasks[i] |= flag;
		}
		else
		{
			BuildMasks[i] &= (BuildFlags)(byte)(~(int)flag);
		}
	}

	public static void SetSolid(int cell, bool solid, CellSolidEvent ev)
	{
		UpdateBuildMask(cell, BuildFlags.Solid, solid);
		if (ev == null)
		{
		}
	}

	private static void UpdateVisMask(int i, VisFlags flag, bool state)
	{
		if (state)
		{
			VisMasks[i] |= flag;
		}
		else
		{
			VisMasks[i] &= (VisFlags)(byte)(~(int)flag);
		}
	}

	private static void UpdateNavValidatorMask(int i, NavValidatorFlags flag, bool state)
	{
		if (state)
		{
			NavValidatorMasks[i] |= flag;
		}
		else
		{
			NavValidatorMasks[i] &= (NavValidatorFlags)(byte)(~(int)flag);
		}
	}

	private static void UpdateNavMask(int i, NavFlags flag, bool state)
	{
		if (state)
		{
			NavMasks[i] |= flag;
		}
		else
		{
			NavMasks[i] &= (NavFlags)(byte)(~(int)flag);
		}
	}

	public static void ResetNavMasksAndDetails()
	{
		NavMasks = null;
		tubeEntrances.Clear();
		restrictions.Clear();
		suitMarkers.Clear();
	}

	public static bool DEBUG_GetRestrictions(int cell, out Restriction restriction)
	{
		return restrictions.TryGetValue(cell, out restriction);
	}

	public static void RegisterRestriction(int cell, Restriction.Orientation orientation)
	{
		HasAccessDoor[cell] = true;
		restrictions[cell] = new Restriction
		{
			DirectionMasksForMinionInstanceID = new Dictionary<int, Restriction.Directions>(),
			orientation = orientation
		};
	}

	public static void UnregisterRestriction(int cell)
	{
		restrictions.Remove(cell);
		HasAccessDoor[cell] = false;
	}

	public static void SetRestriction(int cell, int minionInstanceID, Restriction.Directions directions)
	{
		restrictions[cell].DirectionMasksForMinionInstanceID[minionInstanceID] = directions;
	}

	public static void ClearRestriction(int cell, int minionInstanceID)
	{
		restrictions[cell].DirectionMasksForMinionInstanceID.Remove(minionInstanceID);
	}

	public static bool HasPermission(int cell, int minionInstanceID, int fromCell, NavType fromNavType)
	{
		if (!HasAccessDoor[cell])
		{
			return true;
		}
		Restriction restriction = restrictions[cell];
		Vector2I vector2I = CellToXY(cell);
		Vector2I vector2I2 = CellToXY(fromCell);
		Restriction.Directions directions = (Restriction.Directions)0;
		int num = vector2I.x - vector2I2.x;
		int num2 = vector2I.y - vector2I2.y;
		switch (restriction.orientation)
		{
		case Restriction.Orientation.Vertical:
			if (num < 0)
			{
				directions |= Restriction.Directions.Left;
			}
			if (num > 0)
			{
				directions |= Restriction.Directions.Right;
			}
			break;
		case Restriction.Orientation.Horizontal:
			if (num2 > 0)
			{
				directions |= Restriction.Directions.Left;
			}
			if (num2 < 0)
			{
				directions |= Restriction.Directions.Right;
			}
			break;
		case Restriction.Orientation.SingleCell:
			if (Math.Abs(num) != 1 && Math.Abs(num2) != 1 && fromNavType != NavType.Teleport)
			{
				directions |= Restriction.Directions.Teleport;
			}
			break;
		}
		Restriction.Directions value = (Restriction.Directions)0;
		if (restriction.DirectionMasksForMinionInstanceID.TryGetValue(minionInstanceID, out value) || restriction.DirectionMasksForMinionInstanceID.TryGetValue(-1, out value))
		{
			return (value & directions) == 0;
		}
		return true;
	}

	public static void RegisterTubeEntrance(int cell, int reservationCapacity)
	{
		DebugUtil.Assert(!tubeEntrances.ContainsKey(cell));
		HasTubeEntrance[cell] = true;
		tubeEntrances[cell] = new TubeEntrance
		{
			reservationCapacity = reservationCapacity,
			reservedInstanceIDs = new HashSet<int>()
		};
	}

	public static void UnregisterTubeEntrance(int cell)
	{
		DebugUtil.Assert(tubeEntrances.ContainsKey(cell));
		HasTubeEntrance[cell] = false;
		tubeEntrances.Remove(cell);
	}

	public static bool ReserveTubeEntrance(int cell, int minionInstanceID, bool reserve)
	{
		TubeEntrance tubeEntrance = tubeEntrances[cell];
		HashSet<int> reservedInstanceIDs = tubeEntrance.reservedInstanceIDs;
		if (reserve)
		{
			DebugUtil.Assert(HasTubeEntrance[cell]);
			if (reservedInstanceIDs.Count == tubeEntrance.reservationCapacity)
			{
				return false;
			}
			bool test = reservedInstanceIDs.Add(minionInstanceID);
			DebugUtil.Assert(test);
			return true;
		}
		return reservedInstanceIDs.Remove(minionInstanceID);
	}

	public static void SetTubeEntranceReservationCapacity(int cell, int newReservationCapacity)
	{
		DebugUtil.Assert(HasTubeEntrance[cell]);
		TubeEntrance value = tubeEntrances[cell];
		value.reservationCapacity = newReservationCapacity;
		tubeEntrances[cell] = value;
	}

	public static bool HasUsableTubeEntrance(int cell, int minionInstanceID)
	{
		if (!HasTubeEntrance[cell])
		{
			return false;
		}
		TubeEntrance tubeEntrance = tubeEntrances[cell];
		if (!tubeEntrance.operational)
		{
			return false;
		}
		HashSet<int> reservedInstanceIDs = tubeEntrance.reservedInstanceIDs;
		return reservedInstanceIDs.Count < tubeEntrance.reservationCapacity || reservedInstanceIDs.Contains(minionInstanceID);
	}

	public static bool HasReservedTubeEntrance(int cell, int minionInstanceID)
	{
		DebugUtil.Assert(HasTubeEntrance[cell]);
		return tubeEntrances[cell].reservedInstanceIDs.Contains(minionInstanceID);
	}

	public static void SetTubeEntranceOperational(int cell, bool operational)
	{
		DebugUtil.Assert(HasTubeEntrance[cell]);
		TubeEntrance value = tubeEntrances[cell];
		value.operational = operational;
		tubeEntrances[cell] = value;
	}

	public static void RegisterSuitMarker(int cell)
	{
		DebugUtil.Assert(!HasSuitMarker[cell]);
		HasSuitMarker[cell] = true;
		suitMarkers[cell] = new SuitMarker
		{
			suitCount = 0,
			lockerCount = 0,
			flags = SuitMarker.Flags.Operational,
			minionIDsWithSuitReservations = new HashSet<int>(),
			minionIDsWithEmptyLockerReservations = new HashSet<int>()
		};
	}

	public static void UnregisterSuitMarker(int cell)
	{
		DebugUtil.Assert(HasSuitMarker[cell]);
		HasSuitMarker[cell] = false;
		suitMarkers.Remove(cell);
	}

	public static bool ReserveSuit(int cell, int minionInstanceID, bool reserve)
	{
		DebugUtil.Assert(HasSuitMarker[cell]);
		SuitMarker suitMarker = suitMarkers[cell];
		HashSet<int> minionIDsWithSuitReservations = suitMarker.minionIDsWithSuitReservations;
		if (reserve)
		{
			if (minionIDsWithSuitReservations.Count == suitMarker.suitCount)
			{
				return false;
			}
			bool test = minionIDsWithSuitReservations.Add(minionInstanceID);
			DebugUtil.Assert(test);
			return true;
		}
		return minionIDsWithSuitReservations.Remove(minionInstanceID);
	}

	public static bool ReserveEmptyLocker(int cell, int minionInstanceID, bool reserve)
	{
		DebugUtil.Assert(HasSuitMarker[cell]);
		SuitMarker suitMarker = suitMarkers[cell];
		HashSet<int> minionIDsWithEmptyLockerReservations = suitMarker.minionIDsWithEmptyLockerReservations;
		if (reserve)
		{
			if (minionIDsWithEmptyLockerReservations.Count == suitMarker.emptyLockerCount)
			{
				return false;
			}
			bool test = minionIDsWithEmptyLockerReservations.Add(minionInstanceID);
			DebugUtil.Assert(test);
			return true;
		}
		return minionIDsWithEmptyLockerReservations.Remove(minionInstanceID);
	}

	public static void UpdateSuitMarker(int cell, int fullLockerCount, int emptyLockerCount, SuitMarker.Flags flags, PathFinder.PotentialPath.Flags pathFlags)
	{
		DebugUtil.Assert(HasSuitMarker[cell]);
		SuitMarker value = suitMarkers[cell];
		value.suitCount = fullLockerCount;
		value.lockerCount = fullLockerCount + emptyLockerCount;
		value.flags = flags;
		value.pathFlags = pathFlags;
		suitMarkers[cell] = value;
	}

	public static bool TryGetSuitMarkerFlags(int cell, out SuitMarker.Flags flags, out PathFinder.PotentialPath.Flags pathFlags)
	{
		if (HasSuitMarker[cell])
		{
			flags = suitMarkers[cell].flags;
			pathFlags = suitMarkers[cell].pathFlags;
			return true;
		}
		flags = (SuitMarker.Flags)0;
		pathFlags = PathFinder.PotentialPath.Flags.None;
		return false;
	}

	public static bool HasSuit(int cell, int minionInstanceID)
	{
		if (!HasSuitMarker[cell])
		{
			return false;
		}
		SuitMarker suitMarker = suitMarkers[cell];
		HashSet<int> minionIDsWithSuitReservations = suitMarker.minionIDsWithSuitReservations;
		return minionIDsWithSuitReservations.Count < suitMarker.suitCount || minionIDsWithSuitReservations.Contains(minionInstanceID);
	}

	public static bool HasEmptyLocker(int cell, int minionInstanceID)
	{
		if (!HasSuitMarker[cell])
		{
			return false;
		}
		SuitMarker suitMarker = suitMarkers[cell];
		HashSet<int> minionIDsWithEmptyLockerReservations = suitMarker.minionIDsWithEmptyLockerReservations;
		return minionIDsWithEmptyLockerReservations.Count < suitMarker.emptyLockerCount || minionIDsWithEmptyLockerReservations.Contains(minionInstanceID);
	}

	public unsafe static void InitializeCells()
	{
		for (int i = 0; i != WidthInCells * HeightInCells; i++)
		{
			byte index = elementIdx[i];
			Element element = ElementLoader.elements[index];
			Element[i] = element;
			if (element.IsSolid)
			{
				BuildMasks[i] |= BuildFlags.Solid;
			}
			else
			{
				BuildMasks[i] &= ~BuildFlags.Solid;
			}
			RenderedByWorld[i] = element.substance != null && element.substance.renderedByWorld && Objects[i, 9] == null;
		}
	}

	public unsafe static bool IsInitialized()
	{
		return mass != null;
	}

	public static int GetCellInDirection(int cell, Direction d)
	{
		return d switch
		{
			Direction.Up => CellAbove(cell), 
			Direction.Down => CellBelow(cell), 
			Direction.Left => CellLeft(cell), 
			Direction.Right => CellRight(cell), 
			Direction.None => cell, 
			_ => -1, 
		};
	}

	public static int CellAbove(int cell)
	{
		return cell + WidthInCells;
	}

	public static int CellBelow(int cell)
	{
		return cell - WidthInCells;
	}

	public static int CellLeft(int cell)
	{
		return (cell % WidthInCells > 0) ? (cell - 1) : (-1);
	}

	public static int CellRight(int cell)
	{
		return (cell % WidthInCells < WidthInCells - 1) ? (cell + 1) : (-1);
	}

	public static CellOffset GetOffset(int cell)
	{
		int x = 0;
		int y = 0;
		CellToXY(cell, out x, out y);
		return new CellOffset(x, y);
	}

	public static int CellUpLeft(int cell)
	{
		int result = -1;
		if (cell < (HeightInCells - 1) * WidthInCells && cell % WidthInCells > 0)
		{
			result = cell - 1 + WidthInCells;
		}
		return result;
	}

	public static int CellUpRight(int cell)
	{
		int result = -1;
		if (cell < (HeightInCells - 1) * WidthInCells && cell % WidthInCells < WidthInCells - 1)
		{
			result = cell + 1 + WidthInCells;
		}
		return result;
	}

	public static int CellDownLeft(int cell)
	{
		int result = -1;
		if (cell > WidthInCells && cell % WidthInCells > 0)
		{
			result = cell - 1 - WidthInCells;
		}
		return result;
	}

	public static int CellDownRight(int cell)
	{
		int result = -1;
		if (cell >= WidthInCells && cell % WidthInCells < WidthInCells - 1)
		{
			result = cell + 1 - WidthInCells;
		}
		return result;
	}

	public static bool IsCellLeftOf(int cell, int other_cell)
	{
		return CellColumn(cell) < CellColumn(other_cell);
	}

	public static bool IsCellOffsetOf(int cell, int target_cell, CellOffset[] target_offsets)
	{
		int num = target_offsets.Length;
		for (int i = 0; i < num; i++)
		{
			if (cell == OffsetCell(target_cell, target_offsets[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsCellOffsetOf(int cell, GameObject target, CellOffset[] target_offsets)
	{
		int target_cell = PosToCell(target);
		return IsCellOffsetOf(cell, target_cell, target_offsets);
	}

	public static int GetCellDistance(int cell_a, int cell_b)
	{
		CellOffset offset = GetOffset(cell_a, cell_b);
		return Math.Abs(offset.x) + Math.Abs(offset.y);
	}

	public static int GetCellRange(int cell_a, int cell_b)
	{
		CellOffset offset = GetOffset(cell_a, cell_b);
		return Math.Max(Math.Abs(offset.x), Math.Abs(offset.y));
	}

	public static CellOffset GetOffset(int base_cell, int offset_cell)
	{
		CellToXY(base_cell, out var x, out var y);
		CellToXY(offset_cell, out var x2, out var y2);
		return new CellOffset(x2 - x, y2 - y);
	}

	public static int OffsetCell(int cell, CellOffset offset)
	{
		return cell + offset.x + offset.y * WidthInCells;
	}

	public static int OffsetCell(int cell, int x, int y)
	{
		return cell + x + y * WidthInCells;
	}

	public static bool IsCellOffsetValid(int cell, int x, int y)
	{
		CellToXY(cell, out var x2, out var y2);
		return x2 + x >= 0 && x2 + x < WidthInCells && y2 + y >= 0 && y2 + y < HeightInCells;
	}

	public static bool IsCellOffsetValid(int cell, CellOffset offset)
	{
		return IsCellOffsetValid(cell, offset.x, offset.y);
	}

	public static int PosToCell(StateMachine.Instance smi)
	{
		return PosToCell(smi.transform.GetPosition());
	}

	public static int PosToCell(GameObject go)
	{
		return PosToCell(go.transform.GetPosition());
	}

	public static int PosToCell(KMonoBehaviour cmp)
	{
		return PosToCell(cmp.transform.GetPosition());
	}

	public static bool IsValidBuildingCell(int cell)
	{
		return cell >= 0 && cell < CellCount - WidthInCells * TopBorderHeight;
	}

	public static bool IsWorldValidCell(int cell)
	{
		return IsValidCell(cell) && WorldIdx[cell] != ClusterManager.INVALID_WORLD_IDX;
	}

	public static bool IsValidCell(int cell)
	{
		return cell >= 0 && cell < CellCount;
	}

	public static bool IsActiveWorld(int cell)
	{
		return ClusterManager.Instance != null && ClusterManager.Instance.activeWorldId == WorldIdx[cell];
	}

	public static bool IsCellOpenToSpace(int cell)
	{
		if (IsSolidCell(cell))
		{
			return false;
		}
		GameObject x = Objects[cell, 2];
		if (x != null)
		{
			return false;
		}
		SubWorld.ZoneType subWorldZoneType = World.Instance.zoneRenderData.GetSubWorldZoneType(cell);
		return subWorldZoneType == SubWorld.ZoneType.Space;
	}

	public static int PosToCell(Vector2 pos)
	{
		float x = pos.x;
		float num = pos.y + 0.05f;
		int num2 = (int)num;
		int num3 = (int)x;
		return num2 * WidthInCells + num3;
	}

	public static int PosToCell(Vector3 pos)
	{
		float x = pos.x;
		float num = pos.y + 0.05f;
		int num2 = (int)num;
		int num3 = (int)x;
		return num2 * WidthInCells + num3;
	}

	public static void PosToXY(Vector3 pos, out int x, out int y)
	{
		int cell = PosToCell(pos);
		CellToXY(cell, out x, out y);
	}

	public static void PosToXY(Vector3 pos, out Vector2I xy)
	{
		int cell = PosToCell(pos);
		CellToXY(cell, out xy.x, out xy.y);
	}

	public static Vector2I PosToXY(Vector3 pos)
	{
		int cell = PosToCell(pos);
		Vector2I result = default(Vector2I);
		CellToXY(cell, out result.x, out result.y);
		return result;
	}

	public static int XYToCell(int x, int y)
	{
		return x + y * WidthInCells;
	}

	public static void CellToXY(int cell, out int x, out int y)
	{
		x = CellColumn(cell);
		y = CellRow(cell);
	}

	public static Vector2I CellToXY(int cell)
	{
		return new Vector2I(CellColumn(cell), CellRow(cell));
	}

	public static Vector3 CellToPos(int cell, float x_offset, float y_offset, float z_offset)
	{
		int widthInCells = WidthInCells;
		float num = CellSizeInMeters * (float)(cell % widthInCells);
		float num2 = CellSizeInMeters * (float)(cell / widthInCells);
		return new Vector3(num + x_offset, num2 + y_offset, z_offset);
	}

	public static Vector3 CellToPos(int cell)
	{
		int widthInCells = WidthInCells;
		float x = CellSizeInMeters * (float)(cell % widthInCells);
		float y = CellSizeInMeters * (float)(cell / widthInCells);
		return new Vector3(x, y, 0f);
	}

	public static Vector3 CellToPos2D(int cell)
	{
		int widthInCells = WidthInCells;
		float x = CellSizeInMeters * (float)(cell % widthInCells);
		float y = CellSizeInMeters * (float)(cell / widthInCells);
		return new Vector2(x, y);
	}

	public static int CellRow(int cell)
	{
		return cell / WidthInCells;
	}

	public static int CellColumn(int cell)
	{
		return cell % WidthInCells;
	}

	public static int ClampX(int x)
	{
		return Math.Min(Math.Max(x, 0), WidthInCells - 1);
	}

	public static int ClampY(int y)
	{
		return Math.Min(Math.Max(y, 0), HeightInCells - 1);
	}

	public static Vector2I Constrain(Vector2I val)
	{
		val.x = Mathf.Max(0, Mathf.Min(val.x, WidthInCells - 1));
		val.y = Mathf.Max(0, Mathf.Min(val.y, HeightInCells - 1));
		return val;
	}

	public static void Reveal(int cell, byte visibility = byte.MaxValue)
	{
		bool flag = Spawnable[cell] == 0 && visibility > 0;
		Spawnable[cell] = Math.Max(visibility, Visible[cell]);
		if (!PreventFogOfWarReveal[cell])
		{
			Visible[cell] = Math.Max(visibility, Visible[cell]);
		}
		if (flag && OnReveal != null)
		{
			OnReveal(cell);
		}
	}

	public static ObjectLayer GetObjectLayerForConduitType(ConduitType conduit_type)
	{
		return conduit_type switch
		{
			ConduitType.Gas => ObjectLayer.GasConduitConnection, 
			ConduitType.Liquid => ObjectLayer.LiquidConduitConnection, 
			ConduitType.Solid => ObjectLayer.SolidConduitConnection, 
			_ => throw new ArgumentException("Invalid value.", "conduit_type"), 
		};
	}

	public static Vector3 CellToPos(int cell, CellAlignment alignment, SceneLayer layer)
	{
		switch (alignment)
		{
		case CellAlignment.Bottom:
			return CellToPosCBC(cell, layer);
		case CellAlignment.Left:
			return CellToPosLCC(cell, layer);
		case CellAlignment.Right:
			return CellToPosRCC(cell, layer);
		case CellAlignment.Top:
			return CellToPosCTC(cell, layer);
		case CellAlignment.RandomInternal:
		{
			Vector3 b = new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0f, 0f);
			return CellToPosCCC(cell, layer) + b;
		}
		default:
			return CellToPosCCC(cell, layer);
		}
	}

	public static float GetLayerZ(SceneLayer layer)
	{
		return 0f - HalfCellSizeInMeters - CellSizeInMeters * (float)layer * LayerMultiplier;
	}

	public static Vector3 CellToPosCCC(int cell, SceneLayer layer)
	{
		return CellToPos(cell, HalfCellSizeInMeters, HalfCellSizeInMeters, GetLayerZ(layer));
	}

	public static Vector3 CellToPosCBC(int cell, SceneLayer layer)
	{
		return CellToPos(cell, HalfCellSizeInMeters, 0.01f, GetLayerZ(layer));
	}

	public static Vector3 CellToPosCCF(int cell, SceneLayer layer)
	{
		return CellToPos(cell, HalfCellSizeInMeters, HalfCellSizeInMeters, (0f - CellSizeInMeters) * (float)layer * LayerMultiplier);
	}

	public static Vector3 CellToPosLCC(int cell, SceneLayer layer)
	{
		return CellToPos(cell, 0.01f, HalfCellSizeInMeters, GetLayerZ(layer));
	}

	public static Vector3 CellToPosRCC(int cell, SceneLayer layer)
	{
		return CellToPos(cell, CellSizeInMeters - 0.01f, HalfCellSizeInMeters, GetLayerZ(layer));
	}

	public static Vector3 CellToPosCTC(int cell, SceneLayer layer)
	{
		return CellToPos(cell, HalfCellSizeInMeters, CellSizeInMeters - 0.01f, GetLayerZ(layer));
	}

	public static bool IsSolidCell(int cell)
	{
		return IsValidCell(cell) && Solid[cell];
	}

	public unsafe static bool IsSubstantialLiquid(int cell, float threshold = 0.35f)
	{
		if (IsValidCell(cell))
		{
			byte b = elementIdx[cell];
			if (b < ElementLoader.elements.Count)
			{
				Element element = ElementLoader.elements[b];
				if (element.IsLiquid && mass[cell] >= element.defaultValues.mass * threshold)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsVisiblyInLiquid(Vector2 pos)
	{
		int num = PosToCell(pos);
		if (IsValidCell(num) && IsLiquid(num))
		{
			int cell = CellAbove(num);
			if (IsValidCell(cell) && IsLiquid(cell))
			{
				return true;
			}
			float num2 = Mass[num];
			float num3 = (float)(int)pos.y - pos.y;
			if (num2 / 1000f <= num3)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsLiquid(int cell)
	{
		Element element = ElementLoader.elements[ElementIdx[cell]];
		if (element.IsLiquid)
		{
			return true;
		}
		return false;
	}

	public static bool IsGas(int cell)
	{
		Element element = ElementLoader.elements[ElementIdx[cell]];
		if (element.IsGas)
		{
			return true;
		}
		return false;
	}

	public static void GetVisibleExtents(out int min_x, out int min_y, out int max_x, out int max_y)
	{
		Vector3 vector = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.transform.GetPosition().z));
		Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.GetPosition().z));
		min_y = (int)vector2.y;
		max_y = (int)(vector.y + 0.5f);
		min_x = (int)vector2.x;
		max_x = (int)(vector.x + 0.5f);
	}

	public static void GetVisibleExtents(out Vector2I min, out Vector2I max)
	{
		GetVisibleExtents(out min.x, out min.y, out max.x, out max.y);
	}

	public static bool IsVisible(int cell)
	{
		return Visible[cell] > 0 || !PropertyTextures.IsFogOfWarEnabled;
	}

	public static bool VisibleBlockingCB(int cell)
	{
		return !Transparent[cell] && IsSolidCell(cell);
	}

	public static bool VisibilityTest(int x, int y, int x2, int y2, bool blocking_tile_visible = false)
	{
		return TestLineOfSight(x, y, x2, y2, VisibleBlockingDelegate, blocking_tile_visible);
	}

	public static bool VisibilityTest(int cell, int target_cell, bool blocking_tile_visible = false)
	{
		int x = 0;
		int y = 0;
		CellToXY(cell, out x, out y);
		int x2 = 0;
		int y2 = 0;
		CellToXY(target_cell, out x2, out y2);
		return VisibilityTest(x, y, x2, y2, blocking_tile_visible);
	}

	public static bool PhysicalBlockingCB(int cell)
	{
		return Solid[cell];
	}

	public static bool IsPhysicallyAccessible(int x, int y, int x2, int y2, bool blocking_tile_visible = false)
	{
		return TestLineOfSight(x, y, x2, y2, PhysicalBlockingDelegate, blocking_tile_visible);
	}

	public static void CollectCellsInLine(int startCell, int endCell, HashSet<int> outputCells)
	{
		int num = 2;
		int cellDistance = GetCellDistance(startCell, endCell);
		Vector2 a = (CellToPos(endCell) - CellToPos(startCell)).normalized;
		for (float num2 = 0f; num2 < (float)cellDistance; num2 = Mathf.Min(num2 + 1f / (float)num, cellDistance))
		{
			int num3 = PosToCell(CellToPos(startCell) + (Vector3)(a * num2));
			if (GetCellDistance(startCell, num3) <= cellDistance)
			{
				outputCells.Add(num3);
			}
		}
	}

	public static bool TestLineOfSight(int x, int y, int x2, int y2, Func<int, bool> blocking_cb, bool blocking_tile_visible = false)
	{
		int num = x;
		int num2 = y;
		int num3 = x2 - x;
		int num4 = y2 - y;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		if (num3 < 0)
		{
			num5 = -1;
		}
		else if (num3 > 0)
		{
			num5 = 1;
		}
		if (num4 < 0)
		{
			num6 = -1;
		}
		else if (num4 > 0)
		{
			num6 = 1;
		}
		if (num3 < 0)
		{
			num7 = -1;
		}
		else if (num3 > 0)
		{
			num7 = 1;
		}
		int num9 = Math.Abs(num3);
		int num10 = Math.Abs(num4);
		if (num9 <= num10)
		{
			num9 = Math.Abs(num4);
			num10 = Math.Abs(num3);
			if (num4 < 0)
			{
				num8 = -1;
			}
			else if (num4 > 0)
			{
				num8 = 1;
			}
			num7 = 0;
		}
		int num11 = num9 >> 1;
		for (int i = 0; i <= num9; i++)
		{
			int num12 = XYToCell(x, y);
			if (!IsValidCell(num12))
			{
				return false;
			}
			bool flag = blocking_cb(num12);
			if ((x != num || y != num2) && flag)
			{
				if (blocking_tile_visible && x == x2 && y == y2)
				{
					return true;
				}
				return false;
			}
			num11 += num10;
			if (num11 >= num9)
			{
				num11 -= num9;
				x += num5;
				y += num6;
			}
			else
			{
				x += num7;
				y += num8;
			}
		}
		return true;
	}

	public static bool GetFreeGridSpace(Vector2I size, out Vector2I offset)
	{
		Vector2I gridOffset = BestFit.GetGridOffset(ClusterManager.Instance.WorldContainers, size, out offset);
		if (gridOffset.X <= WidthInCells && gridOffset.Y <= HeightInCells)
		{
			SimMessages.SimDataResizeGridAndInitializeVacuumCells(gridOffset, size.x, size.y, offset.x, offset.y);
			Game.Instance.roomProber.Refresh();
			return true;
		}
		return false;
	}

	public static void FreeGridSpace(Vector2I size, Vector2I offset)
	{
		SimMessages.SimDataFreeCells(size.x, size.y, offset.x, offset.y);
		for (int i = offset.y; i < size.y + offset.y + 1; i++)
		{
			for (int j = offset.x - 1; j < size.x + offset.x + 1; j++)
			{
				int num = XYToCell(j, i);
				if (IsValidCell(num))
				{
					Element[num] = ElementLoader.FindElementByHash(SimHashes.Vacuum);
				}
			}
		}
		Game.Instance.roomProber.Refresh();
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawBoxOnCell(int cell, Color color, float offset = 0f)
	{
		Vector3 vector = CellToPos(cell) + new Vector3(0.5f, 0.5f, 0f);
		float num = 0.5f + offset;
	}
}
