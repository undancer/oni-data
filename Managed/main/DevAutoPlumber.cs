using System;
using System.Collections.Generic;
using UnityEngine;

public class DevAutoPlumber
{
	private enum PortSelection
	{
		UtilityInput,
		UtilityOutput,
		PowerInput
	}

	public static void AutoPlumbBuilding(Building building)
	{
		DoElectricalPlumbing(building);
		DoLiquidAndGasPlumbing(building);
		SetupSolidOreDelivery(building);
	}

	public static void DoElectricalPlumbing(Building building)
	{
		if (building.Def.RequiresPowerInput)
		{
			int cell = Grid.OffsetCell(Grid.PosToCell(building), building.Def.PowerInputOffset);
			GameObject gameObject = Grid.Objects[cell, 26];
			if (gameObject != null)
			{
				gameObject.Trigger(-790448070);
			}
			PlaceSourceAndUtilityConduit(building, Assets.GetBuildingDef("DevGenerator"), Assets.GetBuildingDef("WireRefined"), Game.Instance.electricalConduitSystem, new int[2] { 26, 29 }, PortSelection.PowerInput);
		}
	}

	public static void DoLiquidAndGasPlumbing(Building building)
	{
		SetupPlumbingInput(building);
		SetupPlumbingOutput(building);
	}

	public static void SetupSolidOreDelivery(Building building)
	{
		ManualDeliveryKG component = building.GetComponent<ManualDeliveryKG>();
		if (component != null)
		{
			_ = TrySpawnElementOreFromTag(component.RequestedItemTag, Grid.PosToCell(building), component.Capacity) == null;
			return;
		}
		foreach (ComplexRecipe recipe in ComplexRecipeManager.Get().recipes)
		{
			foreach (Tag fabricator in recipe.fabricators)
			{
				if (fabricator == building.Def.PrefabID)
				{
					ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
					foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
					{
						_ = TrySpawnElementOreFromTag(recipeElement.material, Grid.PosToCell(building), recipeElement.amount * 10f) == null;
					}
				}
			}
		}
	}

	private static GameObject TrySpawnElementOreFromTag(Tag t, int cell, float amount)
	{
		Element element = ElementLoader.GetElement(t);
		if (element == null)
		{
			element = ElementLoader.elements.Find((Element match) => match.HasTag(t));
		}
		return element?.substance.SpawnResource(Grid.CellToPos(cell), amount, element.defaultValues.temperature, byte.MaxValue, 0);
	}

	private static void SetupPlumbingInput(Building building)
	{
		ConduitConsumer component = building.GetComponent<ConduitConsumer>();
		if (!(component == null))
		{
			BuildingDef sourceDef = null;
			BuildingDef conduitDef = null;
			int[] conduitTypeLayers = null;
			UtilityNetworkManager<FlowUtilityNetwork, Vent> utlityNetworkManager = null;
			switch (component.ConduitType)
			{
			case ConduitType.Liquid:
				conduitDef = Assets.GetBuildingDef("InsulatedLiquidConduit");
				sourceDef = Assets.GetBuildingDef("DevPumpLiquid");
				utlityNetworkManager = Game.Instance.liquidConduitSystem;
				conduitTypeLayers = new int[2] { 16, 19 };
				break;
			case ConduitType.Gas:
				conduitDef = Assets.GetBuildingDef("InsulatedGasConduit");
				sourceDef = Assets.GetBuildingDef("DevPumpGas");
				utlityNetworkManager = Game.Instance.gasConduitSystem;
				conduitTypeLayers = new int[2] { 12, 15 };
				break;
			}
			GameObject gameObject = PlaceSourceAndUtilityConduit(building, sourceDef, conduitDef, utlityNetworkManager, conduitTypeLayers, PortSelection.UtilityInput);
			Element element = GuessMostRelevantElementForPump(building);
			if (element != null)
			{
				gameObject.GetComponent<DevPump>().SelectedTag = element.tag;
			}
			else
			{
				gameObject.GetComponent<DevPump>().SelectedTag = ElementLoader.FindElementByHash(SimHashes.Vacuum).tag;
			}
		}
	}

	private static void SetupPlumbingOutput(Building building)
	{
		ConduitDispenser component = building.GetComponent<ConduitDispenser>();
		if (!(component == null))
		{
			BuildingDef sourceDef = null;
			BuildingDef conduitDef = null;
			int[] conduitTypeLayers = null;
			UtilityNetworkManager<FlowUtilityNetwork, Vent> utlityNetworkManager = null;
			switch (component.ConduitType)
			{
			case ConduitType.Liquid:
				conduitDef = Assets.GetBuildingDef("InsulatedLiquidConduit");
				sourceDef = Assets.GetBuildingDef("LiquidVent");
				utlityNetworkManager = Game.Instance.liquidConduitSystem;
				conduitTypeLayers = new int[2] { 16, 19 };
				break;
			case ConduitType.Gas:
				conduitDef = Assets.GetBuildingDef("InsulatedGasConduit");
				sourceDef = Assets.GetBuildingDef("GasVent");
				utlityNetworkManager = Game.Instance.gasConduitSystem;
				conduitTypeLayers = new int[2] { 12, 15 };
				break;
			}
			PlaceSourceAndUtilityConduit(building, sourceDef, conduitDef, utlityNetworkManager, conduitTypeLayers, PortSelection.UtilityOutput);
		}
	}

	private static Element GuessMostRelevantElementForPump(Building destinationBuilding)
	{
		ConduitConsumer consumer = destinationBuilding.GetComponent<ConduitConsumer>();
		Tag targetTag = destinationBuilding.GetComponent<ConduitConsumer>().capacityTag;
		ElementConverter elementConverter = destinationBuilding.GetComponent<ElementConverter>();
		ElementConsumer elementConsumer = destinationBuilding.GetComponent<ElementConsumer>();
		RocketEngineCluster rocketEngineCluster = destinationBuilding.GetComponent<RocketEngineCluster>();
		return ElementLoader.elements.Find(delegate(Element match)
		{
			if (elementConverter != null)
			{
				bool flag = false;
				for (int i = 0; i < elementConverter.consumedElements.Length; i++)
				{
					if (elementConverter.consumedElements[i].tag == match.tag)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			else if (elementConsumer != null)
			{
				bool flag2 = false;
				if (ElementLoader.FindElementByHash(elementConsumer.elementToConsume).tag == match.tag)
				{
					flag2 = true;
				}
				if (!flag2)
				{
					return false;
				}
			}
			else if (rocketEngineCluster != null)
			{
				bool flag3 = false;
				if (rocketEngineCluster.fuelTag == match.tag)
				{
					flag3 = true;
				}
				if (!flag3)
				{
					return false;
				}
			}
			if ((consumer.ConduitType == ConduitType.Liquid && !match.IsLiquid) || (consumer.ConduitType == ConduitType.Gas && !match.IsGas))
			{
				return false;
			}
			return (match.HasTag(targetTag) || !(targetTag != GameTags.Any)) ? true : false;
		});
	}

	private static GameObject PlaceSourceAndUtilityConduit(Building destinationBuilding, BuildingDef sourceDef, BuildingDef conduitDef, IUtilityNetworkMgr utlityNetworkManager, int[] conduitTypeLayers, PortSelection portSelection)
	{
		Building building = null;
		List<int> list = new List<int>();
		int cell = FindClearPlacementLocation(Grid.PosToCell(destinationBuilding), new List<int>(conduitTypeLayers) { 1 }.ToArray(), list);
		bool flag = false;
		int num = 10;
		while (!flag)
		{
			num--;
			building = PlaceConduitSourceBuilding(cell, sourceDef);
			if (building == null)
			{
				return null;
			}
			List<int> list2 = GenerateClearConduitPath(building, destinationBuilding, conduitTypeLayers, portSelection);
			if (list2 == null)
			{
				list.Add(Grid.PosToCell(building));
				building.Trigger(-790448070);
			}
			else
			{
				flag = true;
				BuildConduits(list2, conduitDef, utlityNetworkManager);
			}
		}
		return building.gameObject;
	}

	private static int FindClearPlacementLocation(int nearStartingCell, int[] placementBlockingObjectLayers, List<int> rejectLocations)
	{
		Func<int, object, bool> fn = delegate(int test, object unusedData)
		{
			int[] array = new int[6]
			{
				test,
				Grid.OffsetCell(test, 1, 0),
				Grid.OffsetCell(test, 1, -1),
				Grid.OffsetCell(test, 0, -1),
				Grid.OffsetCell(test, 0, 1),
				Grid.OffsetCell(test, 1, 1)
			};
			foreach (int num in array)
			{
				if (!Grid.IsValidCell(num))
				{
					return false;
				}
				if (Grid.Solid[num])
				{
					return false;
				}
				if (Grid.ObjectLayers[1].ContainsKey(num))
				{
					return false;
				}
				int[] array2 = placementBlockingObjectLayers;
				foreach (int num2 in array2)
				{
					if (Grid.ObjectLayers[num2].ContainsKey(num))
					{
						return false;
					}
				}
				if (rejectLocations.Contains(test))
				{
					return false;
				}
			}
			return true;
		};
		int max_depth = 20;
		return GameUtil.FloodFillFind(fn, null, nearStartingCell, max_depth, stop_at_solid: false, stop_at_liquid: false);
	}

	private static List<int> GenerateClearConduitPath(Building sourceBuilding, Building destinationBuilding, int[] conduitTypeLayers, PortSelection portSelection)
	{
		new List<int>();
		if (sourceBuilding == null)
		{
			return null;
		}
		int conduitStart = -1;
		int conduitEnd = -1;
		switch (portSelection)
		{
		case PortSelection.UtilityInput:
			conduitStart = Grid.OffsetCell(Grid.PosToCell(sourceBuilding), sourceBuilding.Def.UtilityOutputOffset);
			conduitEnd = Grid.OffsetCell(Grid.PosToCell(destinationBuilding), destinationBuilding.Def.UtilityInputOffset);
			break;
		case PortSelection.UtilityOutput:
			conduitStart = Grid.OffsetCell(Grid.PosToCell(destinationBuilding), destinationBuilding.Def.UtilityOutputOffset);
			conduitEnd = Grid.OffsetCell(Grid.PosToCell(sourceBuilding), sourceBuilding.Def.UtilityInputOffset);
			break;
		case PortSelection.PowerInput:
			conduitStart = Grid.OffsetCell(Grid.PosToCell(sourceBuilding), sourceBuilding.Def.PowerOutputOffset);
			conduitEnd = Grid.OffsetCell(Grid.PosToCell(destinationBuilding), destinationBuilding.Def.PowerInputOffset);
			break;
		}
		return GetGridPath(conduitStart, conduitEnd, delegate(int cell)
		{
			if (!Grid.IsValidCell(cell))
			{
				return false;
			}
			int[] array = conduitTypeLayers;
			foreach (int layer in array)
			{
				GameObject gameObject = Grid.Objects[cell, layer];
				bool flag = gameObject == sourceBuilding.gameObject || gameObject == destinationBuilding.gameObject;
				bool flag2 = cell == conduitEnd || cell == conduitStart;
				if (gameObject != null && (!flag || (flag && !flag2)))
				{
					return false;
				}
			}
			return true;
		});
	}

	private static Building PlaceConduitSourceBuilding(int cell, BuildingDef def)
	{
		List<Tag> selected_elements = new List<Tag> { SimHashes.Cuprite.CreateTag() };
		return def.Build(cell, Orientation.Neutral, null, selected_elements, 273.15f, playsound: true, GameClock.Instance.GetTime()).GetComponent<Building>();
	}

	private static void BuildConduits(List<int> path, BuildingDef conduitDef, object utilityNetwork)
	{
		List<Tag> selected_elements = new List<Tag> { SimHashes.Cuprite.CreateTag() };
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < path.Count; i++)
		{
			list.Add(conduitDef.Build(path[i], Orientation.Neutral, null, selected_elements, 273.15f, playsound: true, GameClock.Instance.GetTime()));
		}
		if (list.Count >= 2)
		{
			IUtilityNetworkMgr utilityNetworkMgr = (IUtilityNetworkMgr)utilityNetwork;
			for (int j = 1; j < list.Count; j++)
			{
				UtilityConnections utilityConnections = UtilityConnectionsExtensions.DirectionFromToCell(Grid.PosToCell(list[j - 1]), Grid.PosToCell(list[j]));
				utilityNetworkMgr.AddConnection(utilityConnections, Grid.PosToCell(list[j - 1]), is_physical_building: true);
				utilityNetworkMgr.AddConnection(utilityConnections.InverseDirection(), Grid.PosToCell(list[j]), is_physical_building: true);
				((IUtilityItem)list[j].GetComponent<KAnimGraphTileVisualizer>())?.UpdateConnections(utilityNetworkMgr.GetConnections(Grid.PosToCell(list[j]), is_physical_building: true));
			}
		}
	}

	private static List<int> GetGridPath(int startCell, int endCell, Func<int, bool> testFunction, int maxDepth = 20)
	{
		List<int> list = new List<int>();
		List<int> frontier = new List<int>();
		List<int> touched = new List<int>();
		Dictionary<int, int> crumbs = new Dictionary<int, int>();
		frontier.Add(startCell);
		List<int> newFrontier = new List<int>();
		int num = 0;
		while (!touched.Contains(endCell))
		{
			num++;
			if (num > maxDepth || frontier.Count == 0)
			{
				break;
			}
			foreach (int item in frontier)
			{
				_ExpandFrontier(item);
			}
			frontier.Clear();
			foreach (int item2 in newFrontier)
			{
				frontier.Add(item2);
			}
			newFrontier.Clear();
		}
		int num2 = endCell;
		list.Add(num2);
		while (crumbs.ContainsKey(num2))
		{
			num2 = crumbs[num2];
			list.Add(num2);
		}
		list.Reverse();
		return list;
		void _ExpandFrontier(int fromCell)
		{
			int[] array = new int[4]
			{
				Grid.CellAbove(fromCell),
				Grid.CellBelow(fromCell),
				Grid.CellLeft(fromCell),
				Grid.CellRight(fromCell)
			};
			foreach (int num3 in array)
			{
				if (!newFrontier.Contains(num3) && !frontier.Contains(num3) && !touched.Contains(num3) && testFunction(num3))
				{
					newFrontier.Add(num3);
					crumbs.Add(num3, fromCell);
				}
				touched.Add(num3);
				if (num3 == endCell)
				{
					break;
				}
			}
			touched.Add(fromCell);
		}
	}
}
