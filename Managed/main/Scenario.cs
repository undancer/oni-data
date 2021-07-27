using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using ProcGenGame;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Scenario")]
public class Scenario : KMonoBehaviour
{
	public class RowLayout
	{
		public int Left;

		public int Bot;

		public Builder Builder;

		public RowLayout(int left, int bot)
		{
			Left = left;
			Bot = bot;
		}

		public Builder NextRow()
		{
			if (Builder != null)
			{
				Bot = Builder.Max.y + 1;
			}
			Builder = new Builder(Left, Bot);
			return Builder;
		}
	}

	public class Builder
	{
		public bool PlaceUtilities;

		public int Left;

		public int Bot;

		public Vector2I Min;

		public Vector2I Max;

		public SimHashes Element;

		private Scenario Scenario;

		public Builder(int left, int bot, SimHashes element = SimHashes.Copper)
		{
			Left = left;
			Bot = bot;
			Element = element;
			Scenario = Instance;
			PlaceUtilities = true;
			Min = new Vector2I(left, bot);
			Max = new Vector2I(left, bot);
		}

		private void UpdateMinMax(int x, int y)
		{
			Min.x = Math.Min(x, Min.x);
			Min.y = Math.Min(y, Min.y);
			Max.x = Math.Max(x + 1, Max.x);
			Max.y = Math.Max(y + 1, Max.y);
		}

		public void Utilities(int count)
		{
			for (int i = 0; i < count; i++)
			{
				Scenario.PlaceUtilities(Left, Bot);
				Left++;
			}
		}

		public void BuildingOffsets(string prefab_id, params int[] offsets)
		{
			int left = Left;
			int bot = Bot;
			for (int i = 0; i < offsets.Length / 2; i++)
			{
				Jump(offsets[i * 2], offsets[i * 2 + 1]);
				Building(prefab_id);
				JumpTo(left, bot);
			}
		}

		public void Placer(string prefab_id, Element element)
		{
			BuildingDef buildingDef = Assets.GetBuildingDef(prefab_id);
			int buildingCell = buildingDef.GetBuildingCell(Grid.OffsetCell(Instance.RootCell, Left, Bot));
			Vector3 pos = Grid.CellToPosCBC(buildingCell, Grid.SceneLayer.Building);
			UpdateMinMax(Left, Bot);
			UpdateMinMax(Left + buildingDef.WidthInCells - 1, Bot + buildingDef.HeightInCells - 1);
			Left += buildingDef.WidthInCells;
			Scenario.RunAfterNextUpdate(delegate
			{
				Assets.GetBuildingDef(prefab_id).TryPlace(null, pos, Orientation.Neutral, new Tag[2]
				{
					element.tag,
					ElementLoader.FindElementByHash(SimHashes.SedimentaryRock).tag
				});
			});
		}

		public GameObject Building(string prefab_id)
		{
			GameObject result = Scenario.PlaceBuilding(Left, Bot, prefab_id, Element);
			BuildingDef buildingDef = Assets.GetBuildingDef(prefab_id);
			UpdateMinMax(Left, Bot);
			UpdateMinMax(Left + buildingDef.WidthInCells - 1, Bot + buildingDef.HeightInCells - 1);
			if (PlaceUtilities)
			{
				for (int i = 0; i < buildingDef.WidthInCells; i++)
				{
					UpdateMinMax(Left + i, Bot);
					Scenario.PlaceUtilities(Left + i, Bot);
				}
			}
			Left += buildingDef.WidthInCells;
			return result;
		}

		public void Minion(Action<GameObject> on_spawn = null)
		{
			UpdateMinMax(Left, Bot);
			int left = Left;
			int bot = Bot;
			Scenario.RunAfterNextUpdate(delegate
			{
				GameObject obj = Scenario.SpawnMinion(left, bot);
				if (on_spawn != null)
				{
					on_spawn(obj);
				}
			});
		}

		private GameObject Hexaped()
		{
			return Scenario.SpawnPrefab(Left, Bot, "Hexaped", Grid.SceneLayer.Front);
		}

		public void OreOffsets(int count, SimHashes element, params int[] offsets)
		{
			int left = Left;
			int bot = Bot;
			for (int i = 0; i < offsets.Length / 2; i++)
			{
				Jump(offsets[i * 2], offsets[i * 2 + 1]);
				Ore(count, element);
				JumpTo(left, bot);
			}
		}

		public void Ore(int count = 1, SimHashes element = SimHashes.Cuprite)
		{
			UpdateMinMax(Left, Bot);
			for (int i = 0; i < count; i++)
			{
				Scenario.SpawnOre(Left, Bot, element);
			}
		}

		public void PrefabOffsets(string prefab_id, params int[] offsets)
		{
			int left = Left;
			int bot = Bot;
			for (int i = 0; i < offsets.Length / 2; i++)
			{
				Jump(offsets[i * 2], offsets[i * 2 + 1]);
				Prefab(prefab_id);
				JumpTo(left, bot);
			}
		}

		public void Prefab(string prefab_id, Action<GameObject> on_spawn = null)
		{
			UpdateMinMax(Left, Bot);
			int left = Left;
			int bot = Bot;
			Scenario.RunAfterNextUpdate(delegate
			{
				GameObject obj = Scenario.SpawnPrefab(left, bot, prefab_id);
				if (on_spawn != null)
				{
					on_spawn(obj);
				}
			});
		}

		public void Wall(int height)
		{
			for (int i = 0; i < height; i++)
			{
				Scenario.PlaceBuilding(Left, Bot + i, "Tile");
				UpdateMinMax(Left, Bot + i);
				if (PlaceUtilities)
				{
					Scenario.PlaceUtilities(Left, Bot + i);
				}
			}
			Left++;
		}

		public void Jump(int x = 0, int y = 0)
		{
			Left += x;
			Bot += y;
		}

		public void JumpTo(int left, int bot)
		{
			Left = left;
			Bot = bot;
		}

		public void Hole(int width, int height)
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					int num = Grid.OffsetCell(Scenario.RootCell, Left + i, Bot + j);
					UpdateMinMax(Left + i, Bot + j);
					SimMessages.ReplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.Scenario, 0f);
					Scenario.ReplaceElementMask[num] = true;
				}
			}
		}

		public void FillOffsets(SimHashes element, params int[] offsets)
		{
			int left = Left;
			int bot = Bot;
			for (int i = 0; i < offsets.Length / 2; i++)
			{
				Jump(offsets[i * 2], offsets[i * 2 + 1]);
				Fill(1, 1, element);
				JumpTo(left, bot);
			}
		}

		public void Fill(int width, int height, SimHashes element)
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					int num = Grid.OffsetCell(Scenario.RootCell, Left + i, Bot + j);
					UpdateMinMax(Left + i, Bot + j);
					SimMessages.ReplaceElement(num, element, CellEventLogger.Instance.Scenario, 5000f);
					Scenario.ReplaceElementMask[num] = true;
				}
			}
		}

		public void InAndOuts()
		{
			Wall(3);
			Building("GasVent");
			Hole(3, 3);
			Utilities(2);
			Wall(3);
			Building("LiquidVent");
			Hole(3, 3);
			Utilities(2);
			Wall(3);
			Fill(3, 3, SimHashes.Water);
			Utilities(2);
			GameObject pump = Building("Pump");
			Scenario.RunAfterNextUpdate(delegate
			{
				pump.GetComponent<BuildingEnabledButton>().IsEnabled = true;
			});
		}

		public Builder FinalizeRoom(SimHashes element = SimHashes.Oxygen, SimHashes tileElement = SimHashes.Steel)
		{
			for (int i = Min.x - 1; i <= Max.x; i++)
			{
				if (i == Min.x - 1 || i == Max.x)
				{
					for (int j = Min.y - 1; j <= Max.y; j++)
					{
						Scenario.PlaceBuilding(i, j, "Tile", tileElement);
					}
				}
				else
				{
					int num = 500;
					if (element == SimHashes.Void)
					{
						num = 0;
					}
					for (int k = Min.y; k < Max.y; k++)
					{
						int num2 = Grid.OffsetCell(Scenario.RootCell, i, k);
						if (!Scenario.ReplaceElementMask[num2])
						{
							SimMessages.ReplaceElement(num2, element, CellEventLogger.Instance.Scenario, num);
						}
					}
				}
				Scenario.PlaceBuilding(i, Min.y - 1, "Tile", tileElement);
				Scenario.PlaceBuilding(i, Max.y, "Tile", tileElement);
			}
			return new Builder(Max.x + 1, Min.y);
		}
	}

	private int Bot;

	private int Left;

	public int RootCell;

	public static Scenario Instance;

	public bool PropaneGeneratorTest = true;

	public bool HatchTest = true;

	public bool DoorTest = true;

	public bool AirLockTest = true;

	public bool BedTest = true;

	public bool SuitTest = true;

	public bool SuitRechargeTest = true;

	public bool FabricatorTest = true;

	public bool ElectrolyzerTest = true;

	public bool HexapedTest = true;

	public bool FallTest = true;

	public bool FeedingTest = true;

	public bool OrePerformanceTest = true;

	public bool TwoKelvinsOneSuitTest = true;

	public bool LiquifierTest = true;

	public bool RockCrusherTest = true;

	public bool CementMixerTest = true;

	public bool KilnTest = true;

	public bool ClearExistingScene = true;

	public bool[] ReplaceElementMask { get; set; }

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		SaveLoader instance = SaveLoader.Instance;
		instance.OnWorldGenComplete = (Action<Cluster>)Delegate.Combine(instance.OnWorldGenComplete, new Action<Cluster>(OnWorldGenComplete));
	}

	private void OnWorldGenComplete(Cluster clusterLayout)
	{
		Init();
	}

	private void Init()
	{
		Bot = Grid.HeightInCells / 4;
		Left = 150;
		RootCell = Grid.OffsetCell(0, Left, Bot);
		ReplaceElementMask = new bool[Grid.CellCount];
	}

	private void DigHole(int x, int y, int width, int height)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				SimMessages.ReplaceElement(Grid.OffsetCell(RootCell, x + i, y + j), SimHashes.Oxygen, CellEventLogger.Instance.Scenario, 200f);
				SimMessages.ReplaceElement(Grid.OffsetCell(RootCell, x, y + j), SimHashes.Ice, CellEventLogger.Instance.Scenario, 1000f);
				SimMessages.ReplaceElement(Grid.OffsetCell(RootCell, x + width, y + j), SimHashes.Ice, CellEventLogger.Instance.Scenario, 1000f);
			}
			SimMessages.ReplaceElement(Grid.OffsetCell(RootCell, x + i, y - 1), SimHashes.Ice, CellEventLogger.Instance.Scenario, 1000f);
			SimMessages.ReplaceElement(Grid.OffsetCell(RootCell, x + i, y + height), SimHashes.Ice, CellEventLogger.Instance.Scenario, 1000f);
		}
	}

	private void Fill(int x, int y, SimHashes id = SimHashes.Ice)
	{
		SimMessages.ReplaceElement(Grid.OffsetCell(RootCell, x, y), id, CellEventLogger.Instance.Scenario, 10000f);
	}

	private void PlaceColumn(int x, int y, int height)
	{
		for (int i = 0; i < height; i++)
		{
			SimMessages.ReplaceElement(Grid.OffsetCell(RootCell, x, y + i), SimHashes.Ice, CellEventLogger.Instance.Scenario, 10000f);
		}
	}

	private void PlaceTileX(int left, int bot, int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			PlaceBuilding(left + i, bot, "Tile");
		}
	}

	private void PlaceTileY(int left, int bot, int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			PlaceBuilding(left, bot + i, "Tile");
		}
	}

	private void Clear(int x, int y)
	{
		SimMessages.ReplaceElement(Grid.OffsetCell(RootCell, x, y), SimHashes.Oxygen, CellEventLogger.Instance.Scenario, 10000f);
	}

	private void PlacerLadder(int x, int y, int amount)
	{
		int num = 1;
		if (amount < 0)
		{
			amount = -amount;
			num = -1;
		}
		for (int i = 0; i < amount; i++)
		{
			PlaceBuilding(x, y + i * num, "Ladder");
		}
	}

	private void PlaceBuildings(int left, int bot)
	{
		PlaceBuilding(++left, bot, "ManualGenerator", SimHashes.Iron);
		PlaceBuilding(left += 2, bot, "OxygenMachine", SimHashes.Steel);
		PlaceBuilding(left += 2, bot, "SpaceHeater", SimHashes.Steel);
		PlaceBuilding(++left, bot, "Electrolyzer", SimHashes.Steel);
		PlaceBuilding(++left, bot, "Smelter", SimHashes.Steel);
		SpawnOre(left, bot + 1, SimHashes.Ice);
	}

	private IEnumerator TurnOn(GameObject go)
	{
		yield return null;
		yield return null;
		go.GetComponent<BuildingEnabledButton>().IsEnabled = true;
	}

	private void SetupPlacerTest(Builder b, Element element)
	{
		foreach (BuildingDef buildingDef in Assets.BuildingDefs)
		{
			if (buildingDef.Name != "Excavator")
			{
				b.Placer(buildingDef.PrefabID, element);
			}
		}
		b.FinalizeRoom();
	}

	private void SetupBuildingTest(RowLayout row_layout, bool is_powered, bool break_building)
	{
		Builder builder = null;
		int num = 0;
		foreach (BuildingDef buildingDef in Assets.BuildingDefs)
		{
			if (builder == null)
			{
				builder = row_layout.NextRow();
				num = Left;
				if (is_powered)
				{
					builder.Minion();
					builder.Minion();
				}
			}
			if (buildingDef.Name != "Excavator")
			{
				GameObject gameObject = builder.Building(buildingDef.PrefabID);
				if (break_building)
				{
					BuildingHP component = gameObject.GetComponent<BuildingHP>();
					if (component != null)
					{
						component.DoDamage(int.MaxValue);
					}
				}
			}
			if (builder.Left > num + 100)
			{
				builder.FinalizeRoom();
				builder = null;
			}
		}
		builder.FinalizeRoom();
	}

	private IEnumerator RunAfterNextUpdateRoutine(System.Action action)
	{
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		action();
	}

	private void RunAfterNextUpdate(System.Action action)
	{
		StartCoroutine(RunAfterNextUpdateRoutine(action));
	}

	public void SetupFabricatorTest(Builder b)
	{
		b.Minion();
		b.Building("ManualGenerator");
		b.Ore(3);
		b.Minion();
		b.Building("Masonry");
		b.InAndOuts();
		b.FinalizeRoom();
	}

	public void SetupDoorTest(Builder b)
	{
		b.Minion();
		b.Jump(1);
		b.Building("Door");
		b.Building("ManualGenerator");
		b.FinalizeRoom();
	}

	public void SetupHatchTest(Builder b)
	{
		b.Minion();
		b.Building("Door");
		b.Building("ManualGenerator");
		b.FinalizeRoom();
	}

	public void SetupPropaneGeneratorTest(Builder b)
	{
		b.Building("PropaneGenerator");
		b.Building("OxygenMachine");
		b.FinalizeRoom(SimHashes.Propane);
	}

	public void SetupAirLockTest(Builder b)
	{
		b.Minion();
		b.Jump(1);
		b.Minion();
		b.Jump(1);
		b.Building("PoweredAirlock");
		b.Building("ManualGenerator");
		b.FinalizeRoom();
	}

	public void SetupBedTest(Builder b)
	{
		b.Minion(delegate(GameObject go)
		{
			go.GetAmounts().SetValue("Stamina", 10f);
		});
		b.Building("ManualGenerator");
		b.Minion(delegate(GameObject go)
		{
			go.GetAmounts().SetValue("Stamina", 10f);
		});
		b.Building("ComfyBed");
		b.FinalizeRoom();
	}

	public void SetupHexapedTest(Builder b)
	{
		b.Fill(4, 4, SimHashes.Oxygen);
		b.Prefab("Hexaped");
		b.Jump(2);
		b.Ore(1, SimHashes.Iron);
		b.FinalizeRoom();
	}

	public void SetupElectrolyzerTest(Builder b)
	{
		b.Minion();
		b.Building("ManualGenerator");
		b.Ore(3, SimHashes.Ice);
		b.Minion();
		b.Building("Electrolyzer");
		b.FinalizeRoom();
	}

	public void SetupOrePerformanceTest(Builder b)
	{
		int num = 20;
		int num2 = 20;
		int left = b.Left;
		int bot = b.Bot;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j += 2)
			{
				b.Jump(i, j);
				b.Ore();
				b.JumpTo(left, bot);
			}
		}
		b.FinalizeRoom();
	}

	public void SetupFeedingTest(Builder b)
	{
		b.FillOffsets(SimHashes.IgneousRock, 1, 0, 3, 0, 3, 1, 5, 0, 5, 1, 5, 2, 7, 0, 7, 1, 7, 2, 9, 0, 9, 1, 11, 0);
		b.PrefabOffsets("Hexaped", 0, 0, 2, 0, 4, 0, 7, 3, 9, 2, 11, 1);
		b.OreOffsets(1, SimHashes.IronOre, 1, 1, 3, 2, 5, 3, 8, 0, 10, 0, 12, 0);
		b.FinalizeRoom();
	}

	public void SetupLiquifierTest(Builder b)
	{
		b.Minion();
		b.Minion();
		b.Ore(2, SimHashes.Ice);
		b.Building("ManualGenerator");
		b.Building("Liquifier");
		b.FinalizeRoom();
	}

	public void SetupFallTest(Builder b)
	{
		b.Jump(0, 5);
		b.Minion();
		b.Jump(0, -1);
		b.Building("Tile");
		b.Building("Tile");
		b.Building("Tile");
		b.Jump(-1, 1);
		b.Minion();
		b.Jump(2);
		b.Minion();
		b.Jump(0, -1);
		b.Building("Tile");
		b.Jump(2, 1);
		b.Minion();
		b.Building("Ladder");
		b.Jump(-1, -1);
		b.Building("Tile");
		b.Jump(-1, -3);
		b.Building("Ladder");
		b.FinalizeRoom();
	}

	public void SetupClimbTest(int left, int bot)
	{
		DigHole(left, bot, 13, 5);
		SpawnPrefab(left + 1, bot + 1, "Minion");
		int num = left + 2;
		Clear(num++, bot - 1);
		num++;
		Fill(num++, bot);
		num++;
		Clear(num, bot - 1);
		Clear(num++, bot - 2);
		Fill(num++, bot);
		Clear(num, bot - 1);
		Clear(num++, bot - 2);
		num++;
		Fill(num, bot);
		Fill(num, bot + 1);
	}

	private void SetupSuitRechargeTest(Builder b)
	{
		b.Prefab("PressureSuit", delegate(GameObject go)
		{
			go.GetComponent<SuitTank>().Empty();
		});
		b.Building("ManualGenerator");
		b.Minion();
		b.Building("SuitRecharger");
		b.Minion();
		b.Building("GasVent");
		b.FinalizeRoom();
	}

	private void SetupSuitTest(Builder b)
	{
		b.Minion();
		b.Prefab("PressureSuit");
		b.Jump(1, 2);
		b.Building("Tile");
		b.Jump(-1, -2);
		b.Building("Door");
		b.Building("ManualGenerator");
		b.FinalizeRoom();
	}

	private void SetupTwoKelvinsOneSuitTest(Builder b)
	{
		b.Minion();
		b.Jump(2);
		b.Building("Door");
		b.Jump(2);
		b.Minion();
		b.Prefab("PressureSuit");
		b.FinalizeRoom();
	}

	public void Clear()
	{
		foreach (Brain item in Components.Brains.Items)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (Pickupable item2 in Components.Pickupables.Items)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		foreach (BuildingComplete item3 in Components.BuildingCompletes.Items)
		{
			UnityEngine.Object.Destroy(item3.gameObject);
		}
	}

	public void SetupGameplayTest()
	{
		Init();
		Vector3 pos = Grid.CellToPosCCC(RootCell, Grid.SceneLayer.Background);
		CameraController.Instance.SnapTo(pos);
		if (ClearExistingScene)
		{
			Clear();
		}
		RowLayout rowLayout = new RowLayout(0, 0);
		if (CementMixerTest)
		{
			SetupCementMixerTest(rowLayout.NextRow());
		}
		if (RockCrusherTest)
		{
			SetupRockCrusherTest(rowLayout.NextRow());
		}
		if (PropaneGeneratorTest)
		{
			SetupPropaneGeneratorTest(rowLayout.NextRow());
		}
		if (DoorTest)
		{
			SetupDoorTest(rowLayout.NextRow());
		}
		if (HatchTest)
		{
			SetupHatchTest(rowLayout.NextRow());
		}
		if (AirLockTest)
		{
			SetupAirLockTest(rowLayout.NextRow());
		}
		if (BedTest)
		{
			SetupBedTest(rowLayout.NextRow());
		}
		if (LiquifierTest)
		{
			SetupLiquifierTest(rowLayout.NextRow());
		}
		if (SuitTest)
		{
			SetupSuitTest(rowLayout.NextRow());
		}
		if (SuitRechargeTest)
		{
			SetupSuitRechargeTest(rowLayout.NextRow());
		}
		if (TwoKelvinsOneSuitTest)
		{
			SetupTwoKelvinsOneSuitTest(rowLayout.NextRow());
		}
		if (FabricatorTest)
		{
			SetupFabricatorTest(rowLayout.NextRow());
		}
		if (ElectrolyzerTest)
		{
			SetupElectrolyzerTest(rowLayout.NextRow());
		}
		if (HexapedTest)
		{
			SetupHexapedTest(rowLayout.NextRow());
		}
		if (FallTest)
		{
			SetupFallTest(rowLayout.NextRow());
		}
		if (FeedingTest)
		{
			SetupFeedingTest(rowLayout.NextRow());
		}
		if (OrePerformanceTest)
		{
			SetupOrePerformanceTest(rowLayout.NextRow());
		}
		if (KilnTest)
		{
			SetupKilnTest(rowLayout.NextRow());
		}
	}

	private GameObject SpawnMinion(int x, int y)
	{
		return SpawnPrefab(x, y, "Minion", Grid.SceneLayer.Move);
	}

	private void SetupLadderTest(int left, int bot)
	{
		int num = 5;
		DigHole(left, bot, 13, num);
		SpawnMinion(left + 1, bot);
		int x = left + 1;
		PlacerLadder(x++, bot, num);
		PlaceColumn(x++, bot, num);
		SpawnMinion(x, bot);
		PlacerLadder(x++, bot + 1, num - 1);
		PlaceColumn(x++, bot, num);
		SpawnMinion(x++, bot);
		PlacerLadder(x++, bot, num);
		PlaceColumn(x++, bot, num);
		SpawnMinion(x++, bot);
		PlacerLadder(x++, bot + 1, num - 1);
		PlaceColumn(x++, bot, num);
		SpawnMinion(x++, bot);
		PlacerLadder(x++, bot - 1, -num);
	}

	public void PlaceUtilitiesX(int left, int bot, int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			PlaceUtilities(left + i, bot);
		}
	}

	public void PlaceUtilities(int left, int bot)
	{
		PlaceBuilding(left, bot, "Wire");
		PlaceBuilding(left, bot, "GasConduit");
	}

	public void SetupVisualTest()
	{
		Init();
		RowLayout row_layout = new RowLayout(Left, Bot);
		SetupBuildingTest(row_layout, is_powered: false, break_building: false);
	}

	private void SpawnMaterialTest(Builder b)
	{
		foreach (Element element in ElementLoader.elements)
		{
			if (element.IsSolid)
			{
				b.Element = element.id;
				b.Building("Generator");
			}
		}
		b.FinalizeRoom();
	}

	public GameObject PlaceBuilding(int x, int y, string prefab_id, SimHashes element = SimHashes.Cuprite)
	{
		return PlaceBuilding(RootCell, x, y, prefab_id, element);
	}

	public static GameObject PlaceBuilding(int root_cell, int x, int y, string prefab_id, SimHashes element = SimHashes.Cuprite)
	{
		int cell = Grid.OffsetCell(root_cell, x, y);
		BuildingDef buildingDef = Assets.GetBuildingDef(prefab_id);
		if (buildingDef == null || buildingDef.PlacementOffsets == null)
		{
			DebugUtil.LogErrorArgs("Missing def for", prefab_id);
		}
		Element element2 = ElementLoader.FindElementByHash(element);
		Debug.Assert(element2 != null, "Missing primary element '" + Enum.GetName(typeof(SimHashes), element) + "' in '" + prefab_id + "'");
		GameObject obj = buildingDef.Build(buildingDef.GetBuildingCell(cell), Orientation.Neutral, null, new Tag[2]
		{
			element2.tag,
			ElementLoader.FindElementByHash(SimHashes.SedimentaryRock).tag
		}, 293.15f, playsound: false);
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		component.InternalTemperature = 300f;
		component.Temperature = 300f;
		return obj;
	}

	private void SpawnOre(int x, int y, SimHashes element = SimHashes.Cuprite)
	{
		RunAfterNextUpdate(delegate
		{
			Vector3 position = Grid.CellToPosCCC(Grid.OffsetCell(RootCell, x, y), Grid.SceneLayer.Ore);
			position.x += UnityEngine.Random.Range(-0.1f, 0.1f);
			ElementLoader.FindElementByHash(element).substance.SpawnResource(position, 4000f, 293f, byte.MaxValue, 0);
		});
	}

	public GameObject SpawnPrefab(int x, int y, string name, Grid.SceneLayer scene_layer = Grid.SceneLayer.Ore)
	{
		return SpawnPrefab(RootCell, x, y, name, scene_layer);
	}

	public void SpawnPrefabLate(int x, int y, string name, Grid.SceneLayer scene_layer = Grid.SceneLayer.Ore)
	{
		RunAfterNextUpdate(delegate
		{
			SpawnPrefab(RootCell, x, y, name, scene_layer);
		});
	}

	public static GameObject SpawnPrefab(int RootCell, int x, int y, string name, Grid.SceneLayer scene_layer = Grid.SceneLayer.Ore)
	{
		int cell = Grid.OffsetCell(RootCell, x, y);
		GameObject prefab = Assets.GetPrefab(TagManager.Create(name));
		if (prefab == null)
		{
			return null;
		}
		return GameUtil.KInstantiate(prefab, Grid.CellToPosCBC(cell, scene_layer), scene_layer);
	}

	public void SetupElementTest()
	{
		Init();
		PropertyTextures.FogOfWarScale = 1f;
		Vector3 pos = Grid.CellToPosCCC(RootCell, Grid.SceneLayer.Background);
		CameraController.Instance.SnapTo(pos);
		Clear();
		Builder builder = new RowLayout(0, 0).NextRow();
		HashSet<Element> elements = new HashSet<Element>();
		int bot = builder.Bot;
		foreach (Element item in (from element in ElementLoader.elements
			where element.IsSolid
			orderby element.highTempTransitionTarget
			select element).ToList())
		{
			if (!item.IsSolid)
			{
				continue;
			}
			Element element2 = item;
			int left = builder.Left;
			bool flag = false;
			do
			{
				elements.Add(element2);
				builder.Hole(2, 3);
				builder.Fill(2, 2, element2.id);
				builder.FinalizeRoom(SimHashes.Vacuum, SimHashes.Unobtanium);
				builder = new Builder(left, builder.Bot + 4);
				flag = element2.HasTransitionUp;
				if (flag)
				{
					element2 = element2.highTempTransition;
				}
			}
			while (flag);
			builder = new Builder(left + 3, bot);
		}
		foreach (Element item2 in (from element in ElementLoader.elements
			where element.IsLiquid && !elements.Contains(element)
			orderby element.highTempTransitionTarget
			select element).ToList())
		{
			Element element3 = item2;
			int left2 = builder.Left;
			bool flag2 = false;
			do
			{
				elements.Add(element3);
				builder.Hole(2, 3);
				builder.Fill(2, 2, element3.id);
				builder.FinalizeRoom(SimHashes.Vacuum, SimHashes.Unobtanium);
				builder = new Builder(left2, builder.Bot + 4);
				flag2 = element3.HasTransitionUp;
				if (flag2)
				{
					element3 = element3.highTempTransition;
				}
			}
			while (flag2);
			builder = new Builder(left2 + 3, bot);
		}
		foreach (Element item3 in ElementLoader.elements.Where((Element element) => element.state == Element.State.Gas && !elements.Contains(element)).ToList())
		{
			int left3 = builder.Left;
			builder.Hole(2, 3);
			builder.Fill(2, 2, item3.id);
			builder.FinalizeRoom(SimHashes.Vacuum, SimHashes.Unobtanium);
			builder = new Builder(left3, builder.Bot + 4);
			builder = new Builder(left3 + 3, bot);
		}
	}

	private void InitDebugScenario()
	{
		Init();
		PropertyTextures.FogOfWarScale = 1f;
		Vector3 pos = Grid.CellToPosCCC(RootCell, Grid.SceneLayer.Background);
		CameraController.Instance.SnapTo(pos);
		Clear();
	}

	public void SetupTileTest()
	{
		InitDebugScenario();
		for (int i = 0; i < Grid.HeightInCells; i++)
		{
			for (int j = 0; j < Grid.WidthInCells; j++)
			{
				SimMessages.ReplaceElement(Grid.XYToCell(j, i), SimHashes.Oxygen, CellEventLogger.Instance.Scenario, 100f);
			}
		}
		Builder builder = new RowLayout(0, 0).NextRow();
		for (int k = 0; k < 16; k++)
		{
			builder.Jump();
			builder.Fill(1, 1, (((uint)k & (true ? 1u : 0u)) != 0) ? SimHashes.Copper : SimHashes.Diamond);
			builder.Jump(1);
			builder.Fill(1, 1, (((uint)k & 2u) != 0) ? SimHashes.Copper : SimHashes.Diamond);
			builder.Jump(-1, 1);
			builder.Fill(1, 1, (((uint)k & 4u) != 0) ? SimHashes.Copper : SimHashes.Diamond);
			builder.Jump(1);
			builder.Fill(1, 1, (((uint)k & 8u) != 0) ? SimHashes.Copper : SimHashes.Diamond);
			builder.Jump(2, -1);
		}
	}

	public void SetupRiverTest()
	{
		InitDebugScenario();
		int num = Mathf.Min(64, Grid.WidthInCells);
		int num2 = Mathf.Min(64, Grid.HeightInCells);
		List<Element> list = new List<Element>();
		foreach (Element element in ElementLoader.elements)
		{
			if (element.IsLiquid)
			{
				list.Add(element);
			}
		}
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				SimHashes new_element = ((i == 0) ? SimHashes.Unobtanium : SimHashes.Oxygen);
				SimMessages.ReplaceElement(Grid.XYToCell(j, i), new_element, CellEventLogger.Instance.Scenario, 1000f);
			}
		}
	}

	public void SetupRockCrusherTest(Builder b)
	{
		InitDebugScenario();
		b.Building("ManualGenerator");
		b.Minion();
		b.Building("Crusher");
		b.Minion();
		b.FinalizeRoom();
	}

	public void SetupCementMixerTest(Builder b)
	{
		InitDebugScenario();
		b.Building("Generator");
		b.Minion();
		b.Building("Crusher");
		b.Minion();
		b.Minion();
		b.Building("Mixer");
		b.Ore(20, SimHashes.SedimentaryRock);
		b.FinalizeRoom();
	}

	public void SetupKilnTest(Builder b)
	{
		InitDebugScenario();
		b.Building("ManualGenerator");
		b.Minion();
		b.Building("Kiln");
		b.Minion();
		b.Ore(20, SimHashes.SandCement);
		b.FinalizeRoom();
	}
}
