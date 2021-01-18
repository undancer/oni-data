using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

namespace ProcGenGame
{
	public static class MobSpawning
	{
		public static Dictionary<TerrainCell, List<HashSet<int>>> NaturalCavities = new Dictionary<TerrainCell, List<HashSet<int>>>();

		public static HashSet<int> allNaturalCavityCells = new HashSet<int>();

		public static Dictionary<int, string> PlaceFeatureAmbientMobs(WorldGenSettings settings, TerrainCell tc, SeededRandom rnd, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> avoidCells, bool isDebug)
		{
			Dictionary<int, string> spawnedMobs = new Dictionary<int, string>();
			Node node = tc.node;
			HashSet<int> alreadyOccupiedCells = new HashSet<int>();
			FeatureSettings featureSettings = null;
			foreach (Tag featureSpecificTag in node.featureSpecificTags)
			{
				if (settings.HasFeature(featureSpecificTag.Name))
				{
					featureSettings = settings.GetFeature(featureSpecificTag.Name);
					break;
				}
			}
			if (featureSettings == null)
			{
				return spawnedMobs;
			}
			if (featureSettings.internalMobs == null || featureSettings.internalMobs.Count == 0)
			{
				return spawnedMobs;
			}
			List<int> availableSpawnCellsFeature = tc.GetAvailableSpawnCellsFeature();
			tc.LogInfo("PlaceFeatureAmbientMobs", "possibleSpawnPoints", availableSpawnCellsFeature.Count);
			for (int num = availableSpawnCellsFeature.Count - 1; num > 0; num--)
			{
				int num2 = availableSpawnCellsFeature[num];
				if (ElementLoader.elements[cells[num2].elementIdx].id == SimHashes.Katairite || ElementLoader.elements[cells[num2].elementIdx].id == SimHashes.Unobtanium || avoidCells.Contains(num2))
				{
					availableSpawnCellsFeature.RemoveAt(num);
				}
			}
			tc.LogInfo("mob spawns", "Id:" + node.node.Id + " possible cells", availableSpawnCellsFeature.Count);
			if (availableSpawnCellsFeature.Count == 0)
			{
				if (isDebug)
				{
					Debug.LogWarning("No where to put mobs possibleSpawnPoints [" + tc.node.node.Id + "]");
				}
				return null;
			}
			foreach (MobReference internalMob in featureSettings.internalMobs)
			{
				Mob mob = settings.GetMob(internalMob.type);
				if (mob == null)
				{
					Debug.LogError("Missing mob description for internal mob [" + internalMob.type + "]");
					continue;
				}
				List<int> mobPossibleSpawnPoints = GetMobPossibleSpawnPoints(mob, availableSpawnCellsFeature, cells, alreadyOccupiedCells, rnd);
				if (mobPossibleSpawnPoints.Count == 0)
				{
					if (!isDebug)
					{
					}
					continue;
				}
				tc.LogInfo("\t\tpossible", internalMob.type + " mps: " + mobPossibleSpawnPoints.Count + " ps:", availableSpawnCellsFeature.Count);
				int num3 = Mathf.RoundToInt(internalMob.count.GetRandomValueWithinRange(rnd));
				tc.LogInfo("\t\tcount", internalMob.type, num3);
				Tag mobPrefab = ((mob.prefabName == null) ? new Tag(internalMob.type) : new Tag(mob.prefabName));
				SpawnCountMobs(mob, mobPrefab, num3, mobPossibleSpawnPoints, tc, ref spawnedMobs, ref alreadyOccupiedCells);
			}
			return spawnedMobs;
		}

		public static Dictionary<int, string> PlaceBiomeAmbientMobs(WorldGenSettings settings, TerrainCell tc, SeededRandom rnd, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> avoidCells, bool isDebug)
		{
			Dictionary<int, string> spawnedMobs = new Dictionary<int, string>();
			Node node = tc.node;
			HashSet<int> alreadyOccupiedCells = new HashSet<int>();
			List<Tag> list = new List<Tag>();
			if (node.biomeSpecificTags == null)
			{
				tc.LogInfo("PlaceBiomeAmbientMobs", "No tags", node.node.Id);
				return null;
			}
			foreach (Tag biomeSpecificTag in node.biomeSpecificTags)
			{
				if (settings.HasMob(biomeSpecificTag.Name) && settings.GetMob(biomeSpecificTag.Name) != null)
				{
					list.Add(biomeSpecificTag);
				}
			}
			if (list.Count <= 0)
			{
				tc.LogInfo("PlaceBiomeAmbientMobs", "No biome MOBS", node.node.Id);
				return null;
			}
			List<int> list2 = (node.tags.Contains(WorldGenTags.PreventAmbientMobsInFeature) ? tc.GetAvailableSpawnCellsBiome() : tc.GetAvailableSpawnCellsAll());
			tc.LogInfo("PlaceBiomAmbientMobs", "possibleSpawnPoints", list2.Count);
			for (int num = list2.Count - 1; num > 0; num--)
			{
				int num2 = list2[num];
				if (ElementLoader.elements[cells[num2].elementIdx].id == SimHashes.Katairite || ElementLoader.elements[cells[num2].elementIdx].id == SimHashes.Unobtanium || avoidCells.Contains(num2))
				{
					list2.RemoveAt(num);
				}
			}
			tc.LogInfo("mob spawns", "Id:" + node.node.Id + " possible cells", list2.Count);
			if (list2.Count == 0)
			{
				if (isDebug)
				{
					Debug.LogWarning("No where to put mobs possibleSpawnPoints [" + tc.node.node.Id + "]");
				}
				return null;
			}
			list.ShuffleSeeded(rnd.RandomSource());
			for (int i = 0; i < list.Count; i++)
			{
				Mob mob = settings.GetMob(list[i].Name);
				if (mob == null)
				{
					Debug.LogError("Missing sample description for tag [" + list[i].Name + "]");
					continue;
				}
				List<int> mobPossibleSpawnPoints = GetMobPossibleSpawnPoints(mob, list2, cells, alreadyOccupiedCells, rnd);
				if (mobPossibleSpawnPoints.Count == 0)
				{
					if (!isDebug)
					{
					}
					continue;
				}
				tc.LogInfo("\t\tpossible", list[i].ToString() + " mps: " + mobPossibleSpawnPoints.Count + " ps:", list2.Count);
				float num3 = mob.density.GetRandomValueWithinRange(rnd) * MobSettings.AmbientMobDensity;
				if (num3 > 1f)
				{
					if (isDebug)
					{
						Debug.LogWarning("Got a mob density greater than 1.0 for " + list[i].Name + ". Probably using density as spacing!");
					}
					num3 = 1f;
				}
				tc.LogInfo("\t\tdensity:", "", num3);
				int num4 = Mathf.RoundToInt((float)mobPossibleSpawnPoints.Count * num3);
				tc.LogInfo("\t\tcount", list[i].ToString(), num4);
				Tag mobPrefab = ((mob.prefabName == null) ? list[i] : new Tag(mob.prefabName));
				SpawnCountMobs(mob, mobPrefab, num4, mobPossibleSpawnPoints, tc, ref spawnedMobs, ref alreadyOccupiedCells);
			}
			return spawnedMobs;
		}

		private static List<int> GetMobPossibleSpawnPoints(Mob mob, List<int> possibleSpawnPoints, Sim.Cell[] cells, HashSet<int> alreadyOccupiedCells, SeededRandom rnd)
		{
			List<int> list = possibleSpawnPoints.FindAll((int cell) => IsSuitableMobSpawnPoint(cell, mob, cells, ref alreadyOccupiedCells));
			list.ShuffleSeeded(rnd.RandomSource());
			return list;
		}

		public static void SpawnCountMobs(Mob mobData, Tag mobPrefab, int count, List<int> mobPossibleSpawnPoints, TerrainCell tc, ref Dictionary<int, string> spawnedMobs, ref HashSet<int> alreadyOccupiedCells)
		{
			for (int i = 0; i < count && i < mobPossibleSpawnPoints.Count; i++)
			{
				int num = mobPossibleSpawnPoints[i];
				for (int j = 0; j < mobData.width; j++)
				{
					for (int k = 0; k < mobData.height; k++)
					{
						int item = MobWidthOffset(num, j);
						alreadyOccupiedCells.Add(item);
					}
				}
				tc.AddMob(new KeyValuePair<int, Tag>(num, mobPrefab));
				spawnedMobs.Add(num, mobPrefab.Name);
			}
		}

		public static int MobWidthOffset(int occupiedCell, int widthIterator)
		{
			return Grid.OffsetCell(occupiedCell, (widthIterator % 2 == 0) ? (-(widthIterator / 2)) : (widthIterator / 2 + widthIterator % 2), 0);
		}

		private static bool IsSuitableMobSpawnPoint(int cell, Mob mob, Sim.Cell[] cells, ref HashSet<int> alreadyOccupiedCells)
		{
			for (int i = 0; i < mob.width; i++)
			{
				for (int j = 0; j < mob.height; j++)
				{
					int num = MobWidthOffset(cell, i);
					if (!Grid.IsValidCell(num) || !Grid.IsValidCell(Grid.CellAbove(num)) || !Grid.IsValidCell(Grid.CellBelow(num)))
					{
						return false;
					}
					if (alreadyOccupiedCells.Contains(num))
					{
						return false;
					}
				}
			}
			switch (mob.location)
			{
			case Mob.Location.Solid:
				if (!isNaturalCavity(cell))
				{
					return Grid.Solid[cell];
				}
				return false;
			case Mob.Location.Ceiling:
				if (isNaturalCavity(cell) && !Grid.Solid[cell] && Grid.Solid[Grid.CellAbove(cell)] && !Grid.Solid[Grid.CellBelow(cell)])
				{
					return !Grid.IsLiquid(cell);
				}
				return false;
			case Mob.Location.Floor:
				if (isNaturalCavity(cell) && !Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)] && Grid.Solid[Grid.CellBelow(cell)])
				{
					return !Grid.IsLiquid(cell);
				}
				return false;
			case Mob.Location.LiquidFloor:
				if (isNaturalCavity(cell) && !Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)] && Grid.Solid[Grid.CellBelow(cell)])
				{
					return Grid.IsLiquid(cell);
				}
				return false;
			case Mob.Location.AnyFloor:
				if (isNaturalCavity(cell) && !Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)])
				{
					return Grid.Solid[Grid.CellBelow(cell)];
				}
				return false;
			case Mob.Location.Air:
				if (!Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)])
				{
					return !Grid.IsLiquid(cell);
				}
				return false;
			case Mob.Location.Water:
				if (Grid.Element[cell].id == SimHashes.Water || Grid.Element[cell].id == SimHashes.DirtyWater)
				{
					if (Grid.Element[Grid.CellAbove(cell)].id != SimHashes.Water)
					{
						return Grid.Element[Grid.CellAbove(cell)].id == SimHashes.DirtyWater;
					}
					return true;
				}
				return false;
			case Mob.Location.Surface:
			{
				bool flag = true;
				for (int k = 0; k < mob.width; k++)
				{
					int num2 = MobWidthOffset(cell, k);
					flag = flag && Grid.Element[num2].id == SimHashes.Vacuum && Grid.Solid[Grid.CellBelow(num2)];
				}
				return flag;
			}
			default:
				if (isNaturalCavity(cell))
				{
					return !Grid.Solid[cell];
				}
				return false;
			}
		}

		public static bool isNaturalCavity(int cell)
		{
			if (NaturalCavities == null)
			{
				return false;
			}
			if (allNaturalCavityCells.Contains(cell))
			{
				return true;
			}
			return false;
		}

		public static void DetectNaturalCavities(List<TerrainCell> terrainCells, WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.ANALYZINGWORLD.key, 0.8f, WorldGenProgressStages.Stages.DetectNaturalCavities);
			HashSet<int> invalidCells = new HashSet<int>();
			for (int i = 0; i < terrainCells.Count; i++)
			{
				TerrainCell terrainCell = terrainCells[i];
				float completePercent = (float)i / (float)terrainCells.Count * 100f;
				updateProgressFn(UI.WORLDGEN.ANALYZINGWORLDCOMPLETE.key, completePercent, WorldGenProgressStages.Stages.DetectNaturalCavities);
				NaturalCavities.Add(terrainCell, new List<HashSet<int>>());
				invalidCells.Clear();
				List<int> allCells = terrainCell.GetAllCells();
				for (int j = 0; j < allCells.Count; j++)
				{
					int num = allCells[j];
					if (!Grid.Solid[num] && !invalidCells.Contains(num))
					{
						HashSet<int> hashSet = GameUtil.FloodCollectCells(num, (int checkCell) => !invalidCells.Contains(checkCell) && !Grid.Solid[checkCell], 300, invalidCells);
						if (hashSet != null && hashSet.Count > 0)
						{
							NaturalCavities[terrainCell].Add(hashSet);
							allNaturalCavityCells.UnionWith(hashSet);
						}
					}
				}
			}
			updateProgressFn(UI.WORLDGEN.ANALYZINGWORLDCOMPLETE.key, 100f, WorldGenProgressStages.Stages.DetectNaturalCavities);
		}
	}
}
