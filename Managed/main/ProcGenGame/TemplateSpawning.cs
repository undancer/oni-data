using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

namespace ProcGenGame
{
	public class TemplateSpawning
	{
		private static float minProgressPercent;

		private static float maxProgressPercent;

		private static int m_poiPadding;

		private const int TEMPERATURE_PADDING = 3;

		private static float EXTREME_POI_OVERLAP_TEMPERATURE_RANGE = 100f;

		public static List<KeyValuePair<Vector2I, TemplateContainer>> DetermineTemplatesForWorld(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<RectInt> placedPOIBounds, bool isRunningDebugGen, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, 0f, WorldGenProgressStages.Stages.PlaceTemplates);
			List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets = new List<KeyValuePair<Vector2I, TemplateContainer>>();
			m_poiPadding = settings.GetIntSetting("POIPadding");
			minProgressPercent = 0f;
			maxProgressPercent = 0.25f;
			SpawnStartingTemplate(settings, terrainCells, ref templateSpawnTargets, ref placedPOIBounds, successCallbackFn);
			minProgressPercent = maxProgressPercent;
			maxProgressPercent = 0.5f;
			SpawnTemplatesFromTemplateRules(settings, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, isRunningDebugGen, successCallbackFn);
			minProgressPercent = maxProgressPercent;
			maxProgressPercent = 0.75f;
			SpawnPOITemplates(settings, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, successCallbackFn);
			minProgressPercent = maxProgressPercent;
			maxProgressPercent = 1f;
			SpawnFeatureTemplates(settings, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, successCallbackFn);
			successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, 1f, WorldGenProgressStages.Stages.PlaceTemplates);
			return templateSpawnTargets;
		}

		private static float ProgressPercent(float stagePercent)
		{
			return MathUtil.ReRange(stagePercent, 0f, 1f, minProgressPercent, maxProgressPercent);
		}

		private static void SpawnStartingTemplate(WorldGenSettings settings, List<TerrainCell> terrainCells, ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, ref List<RectInt> placedPOIBounds, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			List<TerrainCell> list = terrainCells.FindAll((TerrainCell tc) => tc.node.tags.Contains(WorldGenTags.StartLocation));
			foreach (TerrainCell item2 in list)
			{
				TemplateContainer template = TemplateCache.GetTemplate(settings.world.startingBaseTemplate);
				KeyValuePair<Vector2I, TemplateContainer> item = new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)item2.poly.Centroid().x, (int)item2.poly.Centroid().y), template);
				templateSpawnTargets.Add(item);
				placedPOIBounds.Add(template.GetTemplateBounds(item.Key, m_poiPadding));
			}
		}

		private static void SpawnPOITemplates(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, ref List<RectInt> placedPOIBounds, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			int num = 0;
			float num2 = settings.world.subworldFiles.Count;
			foreach (WeightedSubworldName subworldFile in settings.world.subworldFiles)
			{
				successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, ProgressPercent((float)num++ / num2), WorldGenProgressStages.Stages.PlaceTemplates);
				HashSet<string> hashSet = new HashSet<string>();
				SubWorld subWorld = settings.GetSubWorld(subworldFile.name);
				if (subWorld.pointsOfInterest == null)
				{
					continue;
				}
				List<TerrainCell> list = terrainCells.FindAll((TerrainCell tc) => tc.node.tags.Contains(subWorld.name.ToTag()));
				for (int num3 = list.Count - 1; num3 >= 0; num3--)
				{
					if (!list[num3].IsSafeToSpawnPOI(terrainCells))
					{
						list.Remove(list[num3]);
					}
				}
				list.ShuffleSeeded(myRandom.RandomSource());
				foreach (KeyValuePair<string, string[]> item3 in subWorld.pointsOfInterest)
				{
					if (list.Count <= 0)
					{
						continue;
					}
					ListPool<string, TemplateSpawning>.PooledList pooledList = ListPool<string, TemplateSpawning>.Allocate();
					string[] value = item3.Value;
					foreach (string item in value)
					{
						if (!hashSet.Contains(item))
						{
							pooledList.Add(item);
						}
					}
					if (pooledList.Count == 0)
					{
						DebugUtil.LogWarningArgs("Template spawning for subworld " + subWorld.name + ", not enough candidates for poiSet " + item3.Key + ", skipping.");
						continue;
					}
					string text = pooledList[myRandom.RandomRange(0, pooledList.Count - 1)];
					TemplateContainer template = TemplateCache.GetTemplate(text);
					if (template != null)
					{
						TerrainCell terrainCell = null;
						for (int j = 0; j < list.Count; j++)
						{
							TerrainCell terrainCell2 = list[j];
							RectInt templateBounds = template.GetTemplateBounds(terrainCell2.poly.Centroid(), m_poiPadding);
							if (!IsPOIOverlappingBounds(placedPOIBounds, templateBounds) && !IsPOIOverlappingHighTemperatureDelta(templateBounds, subWorld, ref terrainCells, settings))
							{
								KeyValuePair<Vector2I, TemplateContainer> item2 = new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell2.poly.Centroid().x, (int)terrainCell2.poly.Centroid().y), template);
								templateSpawnTargets.Add(item2);
								placedPOIBounds.Add(templateBounds);
								terrainCell2.node.tags.Add(text.ToTag());
								terrainCell2.node.tags.Add(WorldGenTags.POI);
								terrainCell = terrainCell2;
								hashSet.Add(text);
								break;
							}
						}
						if (terrainCell != null)
						{
							list.Remove(terrainCell);
						}
					}
					pooledList.Recycle();
				}
			}
		}

		private static void SpawnFeatureTemplates(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, ref List<RectInt> placedPOIBounds, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			int num = 0;
			float num2 = settings.world.subworldFiles.Count;
			foreach (WeightedSubworldName subworldFile in settings.world.subworldFiles)
			{
				successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, ProgressPercent((float)num++ / num2), WorldGenProgressStages.Stages.PlaceTemplates);
				SubWorld subWorld = settings.GetSubWorld(subworldFile.name);
				if (subWorld.featureTemplates == null || subWorld.featureTemplates.Count <= 0)
				{
					continue;
				}
				List<string> list = new List<string>();
				foreach (KeyValuePair<string, int> featureTemplate in subWorld.featureTemplates)
				{
					for (int i = 0; i < featureTemplate.Value; i++)
					{
						list.Add(featureTemplate.Key);
					}
				}
				list.ShuffleSeeded(myRandom.RandomSource());
				List<TerrainCell> list2 = terrainCells.FindAll((TerrainCell tc) => tc.node.tags.Contains(subWorld.name.ToTag()));
				list2.ShuffleSeeded(myRandom.RandomSource());
				foreach (TerrainCell item2 in list2)
				{
					if (list.Count == 0)
					{
						break;
					}
					if (!item2.IsSafeToSpawnFeatureTemplate())
					{
						continue;
					}
					string text = list[list.Count - 1];
					list.RemoveAt(list.Count - 1);
					TemplateContainer template = TemplateCache.GetTemplate(text);
					if (template != null)
					{
						RectInt templateBounds = template.GetTemplateBounds(item2.poly.Centroid(), m_poiPadding);
						if (IsPOIOverlappingBounds(placedPOIBounds, templateBounds))
						{
							DebugUtil.LogArgs(" -> Cannot place here");
							break;
						}
						if (IsPOIOverlappingHighTemperatureDelta(templateBounds, subWorld, ref terrainCells, settings))
						{
							DebugUtil.LogArgs(" -> Cannot place here");
							break;
						}
						KeyValuePair<Vector2I, TemplateContainer> item = new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)item2.poly.Centroid().x, (int)item2.poly.Centroid().y), template);
						templateSpawnTargets.Add(item);
						placedPOIBounds.Add(template.GetTemplateBounds(item.Key, m_poiPadding));
						item2.node.tags.Add(text.ToTag());
						item2.node.tags.Add(WorldGenTags.POI);
					}
				}
			}
		}

		private static void SpawnTemplatesFromTemplateRules(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, ref List<RectInt> placedPOIBounds, bool isRunningDebugGen, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			List<ProcGen.World.TemplateSpawnRules> list = new List<ProcGen.World.TemplateSpawnRules>();
			if (settings.world.worldTemplateRules != null)
			{
				list.AddRange(settings.world.worldTemplateRules);
			}
			foreach (WeightedSubworldName subworldFile in settings.world.subworldFiles)
			{
				SubWorld subWorld = settings.GetSubWorld(subworldFile.name);
				if (subWorld.subworldTemplateRules != null)
				{
					list.AddRange(subWorld.subworldTemplateRules);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			int num = 0;
			float num2 = list.Count;
			list.Sort((ProcGen.World.TemplateSpawnRules a, ProcGen.World.TemplateSpawnRules b) => b.priority.CompareTo(a.priority));
			HashSet<string> hashSet = new HashSet<string>();
			foreach (ProcGen.World.TemplateSpawnRules item in list)
			{
				successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, ProgressPercent((float)num++ / num2), WorldGenProgressStages.Stages.PlaceTemplates);
				for (int i = 0; i < item.times; i++)
				{
					ListPool<string, TemplateSpawning>.PooledList pooledList = ListPool<string, TemplateSpawning>.Allocate();
					foreach (string name in item.names)
					{
						if (!name.StartsWith("poi/") || !hashSet.Contains(name))
						{
							pooledList.Add(name);
						}
					}
					pooledList.ShuffleSeeded(myRandom.RandomSource());
					if (pooledList.Count == 0)
					{
						pooledList.Recycle();
						continue;
					}
					int num3 = 0;
					switch (item.listRule)
					{
					case ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeAll:
						num3 = pooledList.Count;
						break;
					case ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeSome:
						num3 = item.someCount;
						pooledList.RemoveRange(item.someCount, pooledList.Count - item.someCount);
						break;
					case ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeSomeTryMore:
						num3 = item.someCount;
						pooledList.RemoveRange(item.someCount + item.moreCount, pooledList.Count - item.someCount - item.moreCount);
						break;
					case ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeOne:
						num3 = 1;
						pooledList.RemoveRange(1, pooledList.Count - 1);
						break;
					case ProcGen.World.TemplateSpawnRules.ListRule.TrySome:
						pooledList.RemoveRange(item.someCount, pooledList.Count - item.someCount);
						break;
					case ProcGen.World.TemplateSpawnRules.ListRule.TryOne:
						pooledList.RemoveRange(1, pooledList.Count - 1);
						break;
					}
					foreach (string item2 in pooledList)
					{
						bool flag = num3 > 0;
						bool flag2 = FindTargetForTemplate(item2, item, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, flag, settings);
						if (flag && !flag2)
						{
							string text = "TemplateSpawning: Could not place template " + item2 + ", but it's supposed to be guaranteed on " + settings.world.filePath;
							DebugUtil.LogErrorArgs(text);
							if (!isRunningDebugGen)
							{
								throw new Exception(text);
							}
						}
						else
						{
							num3--;
						}
					}
					pooledList.Recycle();
				}
			}
		}

		private static bool FindTargetForTemplate(string template, ProcGen.World.TemplateSpawnRules rule, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, ref List<RectInt> placedPOIBounds, bool guarantee, WorldGenSettings settings)
		{
			TemplateContainer template2 = TemplateCache.GetTemplate(template);
			if (template2 == null)
			{
				return false;
			}
			List<TerrainCell> filteredTerrainCells = terrainCells.FindAll(delegate(TerrainCell tc)
			{
				tc.LogInfo("Filtering", template, 0f);
				return tc.IsSafeToSpawnPOI(terrainCells) && DoesCellMatchFilters(tc, rule.allowedCellsFilter);
			});
			RemoveOverlappingPOIs(ref filteredTerrainCells, ref terrainCells, ref placedPOIBounds, template2, settings);
			if (filteredTerrainCells.Count == 0)
			{
				if (guarantee)
				{
					DebugUtil.LogWarningArgs("Could not place " + template + " using normal rules, trying relaxed");
					filteredTerrainCells = terrainCells.FindAll(delegate(TerrainCell tc)
					{
						tc.LogInfo("Filtering Relaxed", template, 0f);
						return tc.IsSafeToSpawnPOIRelaxed(terrainCells) && DoesCellMatchFilters(tc, rule.allowedCellsFilter);
					});
					RemoveOverlappingPOIs(ref filteredTerrainCells, ref terrainCells, ref placedPOIBounds, template2, settings);
				}
				if (filteredTerrainCells.Count == 0)
				{
					return false;
				}
			}
			filteredTerrainCells.ShuffleSeeded(myRandom.RandomSource());
			TerrainCell terrainCell = filteredTerrainCells[filteredTerrainCells.Count - 1];
			KeyValuePair<Vector2I, TemplateContainer> item = new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell.poly.Centroid().x, (int)terrainCell.poly.Centroid().y), template2);
			templateSpawnTargets.Add(item);
			placedPOIBounds.Add(template2.GetTemplateBounds(item.Key, m_poiPadding));
			terrainCell.node.tags.Add(template.ToTag());
			terrainCell.node.tags.Add(WorldGenTags.POI);
			return true;
		}

		private static bool IsPOIOverlappingBounds(List<RectInt> placedPOIBounds, RectInt templateBounds)
		{
			foreach (RectInt placedPOIBound in placedPOIBounds)
			{
				if (templateBounds.Overlaps(placedPOIBound))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsPOIOverlappingHighTemperatureDelta(RectInt paddedTemplateBounds, SubWorld subworld, ref List<TerrainCell> allCells, WorldGenSettings settings)
		{
			Vector2 b = 2f * Vector2.one * m_poiPadding;
			Vector2 b2 = 2f * Vector2.one * 3f;
			Rect rect = new Rect(paddedTemplateBounds.position, paddedTemplateBounds.size - b + b2);
			Temperature temperature = SettingsCache.temperatures[subworld.temperatureRange];
			for (int i = 0; i < allCells.Count; i++)
			{
				TerrainCell terrainCell = allCells[i];
				SubWorld subWorld = settings.GetSubWorld(terrainCell.node.GetSubworld());
				Temperature temperature2 = SettingsCache.temperatures[subWorld.temperatureRange];
				if (subWorld.temperatureRange != subworld.temperatureRange)
				{
					float num = Mathf.Min(temperature.min, temperature2.min);
					float num2 = Mathf.Max(temperature.max, temperature2.max);
					float num3 = num2 - num;
					bool flag = rect.Overlaps(terrainCell.poly.bounds);
					bool flag2 = num3 > EXTREME_POI_OVERLAP_TEMPERATURE_RANGE;
					if (flag && flag2)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static void RemoveOverlappingPOIs(ref List<TerrainCell> filteredTerrainCells, ref List<TerrainCell> allCells, ref List<RectInt> placedPOIBounds, TemplateContainer container, WorldGenSettings settings)
		{
			for (int num = filteredTerrainCells.Count - 1; num >= 0; num--)
			{
				TerrainCell terrainCell = filteredTerrainCells[num];
				int index = num;
				SubWorld subWorld = settings.GetSubWorld(terrainCell.node.GetSubworld());
				RectInt templateBounds = container.GetTemplateBounds(terrainCell.poly.Centroid(), m_poiPadding);
				bool flag = false;
				if (IsPOIOverlappingBounds(placedPOIBounds, templateBounds))
				{
					terrainCell.LogInfo("-> Removed due to overlapping POIs", "", 0f);
					flag = true;
				}
				else if (IsPOIOverlappingHighTemperatureDelta(templateBounds, subWorld, ref allCells, settings))
				{
					terrainCell.LogInfo("-> Removed due to overlapping extreme temperature delta", "", 0f);
					flag = true;
				}
				if (flag)
				{
					filteredTerrainCells.RemoveAt(index);
				}
			}
		}

		private static bool DoesCellMatchFilters(TerrainCell cell, List<ProcGen.World.AllowedCellsFilter> filters)
		{
			bool flag = false;
			foreach (ProcGen.World.AllowedCellsFilter filter in filters)
			{
				bool flag2 = DoesCellMatchFilter(cell, filter);
				switch (filter.command)
				{
				case ProcGen.World.AllowedCellsFilter.Command.Clear:
					flag = false;
					break;
				case ProcGen.World.AllowedCellsFilter.Command.Replace:
					flag = flag2;
					break;
				case ProcGen.World.AllowedCellsFilter.Command.ExceptWith:
				case ProcGen.World.AllowedCellsFilter.Command.SymmetricExceptWith:
					if (flag2)
					{
						flag = false;
					}
					break;
				case ProcGen.World.AllowedCellsFilter.Command.UnionWith:
					flag = flag2 || flag;
					break;
				case ProcGen.World.AllowedCellsFilter.Command.IntersectWith:
					flag = flag2 && flag;
					break;
				}
				cell.LogInfo("-> DoesCellMatchFilter " + filter.command, flag2 ? "1" : "0", flag ? 1 : 0);
			}
			cell.LogInfo("> Final match", flag ? "true" : "false", 0f);
			return flag;
		}

		private static bool DoesCellMatchFilter(TerrainCell cell, ProcGen.World.AllowedCellsFilter filter)
		{
			if (!ValidateFilter(filter))
			{
				return false;
			}
			if (filter.tagcommand != 0)
			{
				switch (filter.tagcommand)
				{
				case ProcGen.World.AllowedCellsFilter.TagCommand.Default:
					return true;
				case ProcGen.World.AllowedCellsFilter.TagCommand.AtTag:
					return cell.node.tags.Contains(filter.tag);
				case ProcGen.World.AllowedCellsFilter.TagCommand.NotAtTag:
					return !cell.node.tags.Contains(filter.tag);
				case ProcGen.World.AllowedCellsFilter.TagCommand.DistanceFromTag:
				{
					int num = cell.DistanceToTag(filter.tag);
					return num >= filter.minDistance && num <= filter.maxDistance;
				}
				}
			}
			else
			{
				if (filter.subworldNames != null && filter.subworldNames.Count > 0)
				{
					foreach (string subworldName in filter.subworldNames)
					{
						if (cell.node.tags.Contains(subworldName))
						{
							return true;
						}
					}
					return false;
				}
				if (filter.zoneTypes != null && filter.zoneTypes.Count > 0)
				{
					foreach (SubWorld.ZoneType zoneType in filter.zoneTypes)
					{
						if (cell.node.tags.Contains(zoneType.ToString()))
						{
							return true;
						}
					}
					return false;
				}
				if (filter.temperatureRanges != null && filter.temperatureRanges.Count > 0)
				{
					foreach (Temperature.Range temperatureRange in filter.temperatureRanges)
					{
						if (cell.node.tags.Contains(temperatureRange.ToString()))
						{
							return true;
						}
					}
					return false;
				}
			}
			return true;
		}

		private static bool ValidateFilter(ProcGen.World.AllowedCellsFilter filter)
		{
			int num = 0;
			if (filter.tagcommand != 0)
			{
				num++;
			}
			if (filter.subworldNames != null && filter.subworldNames.Count > 0)
			{
				num++;
			}
			if (filter.zoneTypes != null && filter.zoneTypes.Count > 0)
			{
				num++;
			}
			if (filter.temperatureRanges != null && filter.temperatureRanges.Count > 0)
			{
				num++;
			}
			if (num != 1)
			{
				string str = "BAD ALLOWED CELLS FILTER in FEATURE RULES!";
				str += "\nA filter can only specify one of `tagcommand`, `subworldNames`, `zoneTypes`, or `temperatureRanges`.";
				str += "\nFound a filter with the following:";
				if (filter.tagcommand != 0)
				{
					str += "\ntagcommand:\n\t";
					str += filter.tagcommand;
					str += "\ntag:\n\t";
					str += filter.tag;
				}
				if (filter.subworldNames != null && filter.subworldNames.Count > 0)
				{
					str += "\nsubworldNames:\n\t";
					str += string.Join(", ", filter.subworldNames);
				}
				if (filter.zoneTypes != null && filter.zoneTypes.Count > 0)
				{
					str += "\nzoneTypes:\n";
					str += string.Join(", ", filter.zoneTypes);
				}
				if (filter.temperatureRanges != null && filter.temperatureRanges.Count > 0)
				{
					str += "\ntemperatureRanges:\n";
					str += string.Join(", ", filter.temperatureRanges);
				}
				Debug.LogError(str);
				return false;
			}
			return true;
		}
	}
}
