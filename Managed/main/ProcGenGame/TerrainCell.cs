using System.Collections.Generic;
using Delaunay.Geo;
using KSerialization;
using ProcGen;
using ProcGen.Map;
using UnityEngine;
using VoronoiTree;

namespace ProcGenGame
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class TerrainCell
	{
		public delegate void SetValuesFunction(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc);

		public struct ElementOverride
		{
			public Element element;

			public Sim.PhysicsData pdelement;

			public Sim.DiseaseCell dc;

			public float mass;

			public float temperature;

			public byte diseaseIdx;

			public int diseaseAmount;

			public bool overrideMass;

			public bool overrideTemperature;

			public bool overrideDiseaseIdx;

			public bool overrideDiseaseAmount;
		}

		private const float MASS_VARIATION = 0.2f;

		public List<KeyValuePair<int, Tag>> terrainPositions;

		public List<KeyValuePair<int, Tag>> poi;

		public List<int> neighbourTerrainCells = new List<int>();

		private float finalSize;

		private bool debugMode;

		private List<int> allCells;

		private HashSet<int> availableTerrainPoints;

		private HashSet<int> featureSpawnPoints;

		private HashSet<int> availableSpawnPoints;

		public const int DONT_SET_TEMPERATURE_DEFAULTS = -1;

		private static readonly Tag[] noFeatureSpawnTags = new Tag[5]
		{
			WorldGenTags.StartLocation,
			WorldGenTags.AtStart,
			WorldGenTags.NearStartLocation,
			WorldGenTags.POI,
			WorldGenTags.Feature
		};

		private static readonly TagSet noFeatureSpawnTagSet = new TagSet(noFeatureSpawnTags);

		private static readonly Tag[] noPOISpawnTags = new Tag[5]
		{
			WorldGenTags.StartLocation,
			WorldGenTags.AtStart,
			WorldGenTags.NearStartLocation,
			WorldGenTags.POI,
			WorldGenTags.Feature
		};

		private static readonly TagSet noPOISpawnTagSet = new TagSet(noPOISpawnTags);

		private static readonly Tag[] relaxedNoPOISpawnTags = new Tag[4]
		{
			WorldGenTags.StartLocation,
			WorldGenTags.AtStart,
			WorldGenTags.NearStartLocation,
			WorldGenTags.POI
		};

		private static readonly TagSet relaxedNoPOISpawnTagSet = new TagSet(relaxedNoPOISpawnTags);

		public Polygon poly => site.poly;

		public Cell node { get; private set; }

		public Diagram.Site site { get; private set; }

		public Dictionary<Tag, int> distancesToTags { get; private set; }

		public bool HasMobs
		{
			get
			{
				if (mobs != null)
				{
					return mobs.Count > 0;
				}
				return false;
			}
		}

		public List<KeyValuePair<int, Tag>> mobs { get; private set; }

		protected TerrainCell()
		{
		}

		protected TerrainCell(Cell node, Diagram.Site site, Dictionary<Tag, int> distancesToTags)
		{
			this.node = node;
			this.site = site;
			this.distancesToTags = distancesToTags;
		}

		public virtual void LogInfo(string evt, string param, float value)
		{
			Debug.Log(evt + ":" + param + "=" + value);
		}

		public void InitializeCells(HashSet<int> claimedCells)
		{
			if (allCells != null)
			{
				return;
			}
			allCells = new List<int>();
			availableTerrainPoints = new HashSet<int>();
			availableSpawnPoints = new HashSet<int>();
			for (int i = 0; i < Grid.HeightInCells; i++)
			{
				for (int j = 0; j < Grid.WidthInCells; j++)
				{
					if (poly.Contains(new Vector2(j, i)))
					{
						int item = Grid.XYToCell(j, i);
						availableTerrainPoints.Add(item);
						availableSpawnPoints.Add(item);
						if (claimedCells.Add(item))
						{
							allCells.Add(item);
						}
					}
				}
			}
			LogInfo("Initialise cells", "", allCells.Count);
		}

		public List<int> GetAllCells()
		{
			return new List<int>(allCells);
		}

		public List<int> GetAvailableSpawnCellsAll()
		{
			List<int> list = new List<int>();
			foreach (int availableSpawnPoint in availableSpawnPoints)
			{
				list.Add(availableSpawnPoint);
			}
			return list;
		}

		public List<int> GetAvailableSpawnCellsFeature()
		{
			List<int> list = new List<int>();
			HashSet<int> hashSet = new HashSet<int>(availableSpawnPoints);
			hashSet.ExceptWith(availableTerrainPoints);
			foreach (int item in hashSet)
			{
				list.Add(item);
			}
			return list;
		}

		public List<int> GetAvailableSpawnCellsBiome()
		{
			List<int> list = new List<int>();
			HashSet<int> hashSet = new HashSet<int>(availableSpawnPoints);
			hashSet.ExceptWith(featureSpawnPoints);
			foreach (int item in hashSet)
			{
				list.Add(item);
			}
			return list;
		}

		private bool RemoveFromAvailableSpawnCells(int cell)
		{
			return availableSpawnPoints.Remove(cell);
		}

		public void AddMobs(IEnumerable<KeyValuePair<int, Tag>> newMobs)
		{
			foreach (KeyValuePair<int, Tag> newMob in newMobs)
			{
				AddMob(newMob);
			}
		}

		private void AddMob(int cellIdx, string tag)
		{
			AddMob(new KeyValuePair<int, Tag>(cellIdx, new Tag(tag)));
		}

		public void AddMob(KeyValuePair<int, Tag> mob)
		{
			if (mobs == null)
			{
				mobs = new List<KeyValuePair<int, Tag>>();
			}
			mobs.Add(mob);
			bool flag = RemoveFromAvailableSpawnCells(mob.Key);
			LogInfo("\t\t\tRemoveFromAvailableCells", mob.Value.Name + ": " + (flag ? "success" : "failed"), mob.Key);
			if (!flag)
			{
				if (!allCells.Contains(mob.Key))
				{
					Debug.Assert(condition: false, "Couldnt find cell [" + mob.Key + "] we dont own, to remove for mob [" + mob.Value.Name + "]");
				}
				else
				{
					Debug.Assert(condition: false, "Couldnt find cell [" + mob.Key + "] to remove for mob [" + mob.Value.Name + "]");
				}
			}
		}

		protected string GetSubWorldType(WorldGen worldGen)
		{
			Vector2I pos = new Vector2I((int)site.poly.Centroid().x, (int)site.poly.Centroid().y);
			return worldGen.GetSubWorldType(pos);
		}

		protected Temperature.Range GetTemperatureRange(WorldGen worldGen)
		{
			string subWorldType = GetSubWorldType(worldGen);
			if (subWorldType == null)
			{
				return Temperature.Range.Mild;
			}
			if (!worldGen.Settings.HasSubworld(subWorldType))
			{
				return Temperature.Range.Mild;
			}
			return worldGen.Settings.GetSubWorld(subWorldType).temperatureRange;
		}

		protected void GetTemperatureRange(WorldGen worldGen, ref float min, ref float range)
		{
			Temperature.Range temperatureRange = GetTemperatureRange(worldGen);
			min = SettingsCache.temperatures[temperatureRange].min;
			range = SettingsCache.temperatures[temperatureRange].max - min;
		}

		protected float GetDensityMassForCell(Chunk world, int cellIdx, float mass)
		{
			if (!Grid.IsValidCell(cellIdx))
			{
				return 0f;
			}
			Debug.Assert(world.density[cellIdx] >= 0f && world.density[cellIdx] <= 1f, "Density [" + world.density[cellIdx] + "] out of range [0-1]");
			float num = 0.2f * (world.density[cellIdx] - 0.5f);
			float num2 = mass + mass * num;
			if (num2 > 10000f)
			{
				num2 = 10000f;
			}
			return num2;
		}

		private void HandleSprinkleOfElement(WorldGenSettings settings, Tag targetTag, Chunk world, SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			Element element = ElementLoader.FindElementByName(settings.GetFeature(targetTag.Name).GetOneWeightedSimHash("SprinkleOfElementChoices", rnd).element);
			ProcGen.Room value = null;
			SettingsCache.rooms.TryGetValue(targetTag.Name, out value);
			SampleDescriber sampleDescriber = value;
			Sim.PhysicsData defaultValues = element.defaultValues;
			Sim.DiseaseCell invalid = Sim.DiseaseCell.Invalid;
			for (int i = 0; i < terrainPositions.Count; i++)
			{
				if (terrainPositions[i].Value != targetTag)
				{
					continue;
				}
				float radius = rnd.RandomRange(sampleDescriber.blobSize.min, sampleDescriber.blobSize.max);
				List<Vector2I> filledCircle = ProcGen.Util.GetFilledCircle(Grid.CellToPos2D(terrainPositions[i].Key), radius);
				for (int j = 0; j < filledCircle.Count; j++)
				{
					int num = Grid.XYToCell(filledCircle[j].x, filledCircle[j].y);
					if (Grid.IsValidCell(num))
					{
						defaultValues.mass = GetDensityMassForCell(world, num, element.defaultValues.mass);
						defaultValues.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
						SetValues(num, element, defaultValues, invalid);
					}
				}
			}
		}

		private HashSet<Vector2I> DigFeature(ProcGen.Room.Shape shape, float size, List<int> bordersWidths, SeededRandom rnd, out List<Vector2I> featureCenterPoints, out List<List<Vector2I>> featureBorders)
		{
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
			featureCenterPoints = new List<Vector2I>();
			featureBorders = new List<List<Vector2I>>();
			if (size < 1f)
			{
				return hashSet;
			}
			Vector2 center = site.poly.Centroid();
			finalSize = size;
			switch (shape)
			{
			case ProcGen.Room.Shape.Blob:
				featureCenterPoints = ProcGen.Util.GetBlob(center, finalSize, rnd.RandomSource());
				break;
			case ProcGen.Room.Shape.Circle:
				featureCenterPoints = ProcGen.Util.GetFilledCircle(center, finalSize);
				break;
			case ProcGen.Room.Shape.Square:
				featureCenterPoints = ProcGen.Util.GetFilledRectangle(center, finalSize, finalSize, rnd);
				break;
			case ProcGen.Room.Shape.TallThin:
				featureCenterPoints = ProcGen.Util.GetFilledRectangle(center, finalSize / 4f, finalSize, rnd);
				break;
			case ProcGen.Room.Shape.ShortWide:
				featureCenterPoints = ProcGen.Util.GetFilledRectangle(center, finalSize, finalSize / 4f, rnd);
				break;
			case ProcGen.Room.Shape.Splat:
				featureCenterPoints = ProcGen.Util.GetSplat(center, finalSize, rnd.RandomSource());
				break;
			}
			hashSet.UnionWith(featureCenterPoints);
			if (featureCenterPoints.Count == 0)
			{
				Debug.LogWarning("Room has no centerpoints. Terrain Cell [ shape: " + shape.ToString() + " size: " + finalSize + "] [" + node.NodeId + " " + node.type + " " + node.position.ToString() + "]");
			}
			else if (bordersWidths != null && bordersWidths.Count > 0 && bordersWidths[0] > 0)
			{
				for (int i = 0; i < bordersWidths.Count && bordersWidths[i] > 0; i++)
				{
					featureBorders.Add(ProcGen.Util.GetBorder(hashSet, bordersWidths[i]));
					hashSet.UnionWith(featureBorders[i]);
				}
			}
			return hashSet;
		}

		public static ElementOverride GetElementOverride(string element, SampleDescriber.Override overrides)
		{
			Debug.Assert(element != null && element.Length > 0);
			ElementOverride result = default(ElementOverride);
			result.element = ElementLoader.FindElementByName(element);
			Debug.Assert(result.element != null, "Couldn't find an element called " + element);
			result.pdelement = result.element.defaultValues;
			result.dc = Sim.DiseaseCell.Invalid;
			result.mass = result.element.defaultValues.mass;
			result.temperature = result.element.defaultValues.temperature;
			if (overrides == null)
			{
				return result;
			}
			result.overrideMass = false;
			result.overrideTemperature = false;
			result.overrideDiseaseIdx = false;
			result.overrideDiseaseAmount = false;
			if (overrides.massOverride.HasValue)
			{
				result.mass = overrides.massOverride.Value;
				result.overrideMass = true;
			}
			if (overrides.massMultiplier.HasValue)
			{
				result.mass *= overrides.massMultiplier.Value;
				result.overrideMass = true;
			}
			if (overrides.temperatureOverride.HasValue)
			{
				result.temperature = overrides.temperatureOverride.Value;
				result.overrideTemperature = true;
			}
			if (overrides.temperatureMultiplier.HasValue)
			{
				result.temperature *= overrides.temperatureMultiplier.Value;
				result.overrideTemperature = true;
			}
			if (overrides.diseaseOverride != null)
			{
				result.diseaseIdx = WorldGen.diseaseStats.GetIndex(overrides.diseaseOverride);
				result.overrideDiseaseIdx = true;
			}
			if (overrides.diseaseAmountOverride.HasValue)
			{
				result.diseaseAmount = overrides.diseaseAmountOverride.Value;
				result.overrideDiseaseAmount = true;
			}
			if (result.overrideTemperature)
			{
				result.pdelement.temperature = result.temperature;
			}
			if (result.overrideMass)
			{
				result.pdelement.mass = result.mass;
			}
			if (result.overrideDiseaseIdx)
			{
				result.dc.diseaseIdx = result.diseaseIdx;
			}
			if (result.overrideDiseaseAmount)
			{
				result.dc.elementCount = result.diseaseAmount;
			}
			return result;
		}

		private bool IsFeaturePointContainedInBorder(Vector2 point, WorldGen worldGen)
		{
			if (!node.tags.Contains(WorldGenTags.AllowExceedNodeBorders))
			{
				return true;
			}
			if (!poly.Contains(point))
			{
				TerrainCell terrainCell = worldGen.TerrainCells.Find((TerrainCell x) => x.poly.Contains(point));
				if (terrainCell != null)
				{
					SubWorld subWorld = worldGen.Settings.GetSubWorld(node.GetSubworld());
					SubWorld subWorld2 = worldGen.Settings.GetSubWorld(terrainCell.node.GetSubworld());
					if (subWorld.zoneType != subWorld2.zoneType)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void ApplyPlaceElementForRoom(FeatureSettings feature, string group, List<Vector2I> cells, WorldGen worldGen, Chunk world, SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd, HashSet<int> highPriorityClaims)
		{
			if (cells == null || cells.Count == 0 || !feature.HasGroup(group))
			{
				return;
			}
			switch (feature.ElementChoiceGroups[group].selectionMethod)
			{
			case ProcGen.Room.Selection.Weighted:
			case ProcGen.Room.Selection.WeightedResample:
			{
				for (int i = 0; i < cells.Count; i++)
				{
					int num = Grid.XYToCell(cells[i].x, cells[i].y);
					if (Grid.IsValidCell(num) && !highPriorityClaims.Contains(num) && IsFeaturePointContainedInBorder(cells[i], worldGen))
					{
						WeightedSimHash oneWeightedSimHash = feature.GetOneWeightedSimHash(group, rnd);
						ElementOverride elementOverride = GetElementOverride(oneWeightedSimHash.element, oneWeightedSimHash.overrides);
						if (!elementOverride.overrideTemperature)
						{
							elementOverride.pdelement.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
						}
						if (!elementOverride.overrideMass)
						{
							elementOverride.pdelement.mass = GetDensityMassForCell(world, num, elementOverride.mass);
						}
						SetValues(num, elementOverride.element, elementOverride.pdelement, elementOverride.dc);
					}
				}
				return;
			}
			case ProcGen.Room.Selection.HorizontalSlice:
			{
				int num2 = int.MaxValue;
				int num3 = int.MinValue;
				for (int j = 0; j < cells.Count; j++)
				{
					num2 = Mathf.Min(cells[j].y, num2);
					num3 = Mathf.Max(cells[j].y, num3);
				}
				int num4 = num3 - num2;
				for (int k = 0; k < cells.Count; k++)
				{
					int num5 = Grid.XYToCell(cells[k].x, cells[k].y);
					if (Grid.IsValidCell(num5) && !highPriorityClaims.Contains(num5) && IsFeaturePointContainedInBorder(cells[k], worldGen))
					{
						float percentage = 1f - (float)(cells[k].y - num2) / (float)num4;
						WeightedSimHash weightedSimHashAtChoice = feature.GetWeightedSimHashAtChoice(group, percentage);
						ElementOverride elementOverride2 = GetElementOverride(weightedSimHashAtChoice.element, weightedSimHashAtChoice.overrides);
						if (!elementOverride2.overrideTemperature)
						{
							elementOverride2.pdelement.temperature = temperatureMin + world.heatOffset[num5] * temperatureRange;
						}
						if (!elementOverride2.overrideMass)
						{
							elementOverride2.pdelement.mass = GetDensityMassForCell(world, num5, elementOverride2.mass);
						}
						SetValues(num5, elementOverride2.element, elementOverride2.pdelement, elementOverride2.dc);
					}
				}
				return;
			}
			}
			WeightedSimHash oneWeightedSimHash2 = feature.GetOneWeightedSimHash(group, rnd);
			DebugUtil.LogArgs("Picked one: ", oneWeightedSimHash2.element);
			for (int l = 0; l < cells.Count; l++)
			{
				int num6 = Grid.XYToCell(cells[l].x, cells[l].y);
				if (Grid.IsValidCell(num6) && !highPriorityClaims.Contains(num6) && IsFeaturePointContainedInBorder(cells[l], worldGen))
				{
					ElementOverride elementOverride3 = GetElementOverride(oneWeightedSimHash2.element, oneWeightedSimHash2.overrides);
					if (!elementOverride3.overrideTemperature)
					{
						elementOverride3.pdelement.temperature = temperatureMin + world.heatOffset[num6] * temperatureRange;
					}
					if (!elementOverride3.overrideMass)
					{
						elementOverride3.pdelement.mass = GetDensityMassForCell(world, num6, elementOverride3.mass);
					}
					SetValues(num6, elementOverride3.element, elementOverride3.pdelement, elementOverride3.dc);
				}
			}
		}

		private int GetIndexForLocation(List<Vector2I> points, Mob.Location location, SeededRandom rnd)
		{
			int num = -1;
			if (points == null || points.Count == 0)
			{
				return num;
			}
			if (location == Mob.Location.Air || location == Mob.Location.Solid)
			{
				return rnd.RandomRange(0, points.Count);
			}
			for (int i = 0; i < points.Count; i++)
			{
				if (!Grid.IsValidCell(Grid.XYToCell(points[i].x, points[i].y)))
				{
					continue;
				}
				if (num == -1)
				{
					num = i;
					continue;
				}
				switch (location)
				{
				case Mob.Location.Ceiling:
					if (points[i].y > points[num].y)
					{
						num = i;
					}
					break;
				case Mob.Location.Floor:
					if (points[i].y < points[num].y)
					{
						num = i;
					}
					break;
				}
			}
			return num;
		}

		private void PlaceMobsInRoom(WorldGenSettings settings, List<MobReference> mobTags, List<Vector2I> points, SeededRandom rnd)
		{
			if (points == null)
			{
				return;
			}
			if (mobs == null)
			{
				mobs = new List<KeyValuePair<int, Tag>>();
			}
			for (int i = 0; i < mobTags.Count; i++)
			{
				if (!settings.HasMob(mobTags[i].type))
				{
					Debug.LogError("Missing sample description for tag [" + mobTags[i].type + "]");
					continue;
				}
				Mob mob = settings.GetMob(mobTags[i].type);
				int num = Mathf.RoundToInt(mobTags[i].count.GetRandomValueWithinRange(rnd));
				for (int j = 0; j < num; j++)
				{
					int indexForLocation = GetIndexForLocation(points, mob.location, rnd);
					if (indexForLocation == -1)
					{
						break;
					}
					if (points.Count <= indexForLocation)
					{
						return;
					}
					int cellIdx = Grid.XYToCell(points[indexForLocation].x, points[indexForLocation].y);
					points.RemoveAt(indexForLocation);
					AddMob(cellIdx, mobTags[i].type);
				}
			}
		}

		private int[] ConvertNoiseToPoints(float[] basenoise, float minThreshold = 0.9f, float maxThreshold = 1f)
		{
			if (basenoise == null)
			{
				return null;
			}
			List<int> list = new List<int>();
			float width = site.poly.bounds.width;
			float height = site.poly.bounds.height;
			for (float num = site.position.y - height / 2f; num < site.position.y + height / 2f; num += 1f)
			{
				for (float num2 = site.position.x - width / 2f; num2 < site.position.x + width / 2f; num2 += 1f)
				{
					int num3 = Grid.PosToCell(new Vector2(num2, num));
					if (site.poly.Contains(new Vector2(num2, num)))
					{
						float num4 = (int)basenoise[num3];
						if (!(num4 < minThreshold) && !(num4 > maxThreshold) && !list.Contains(num3))
						{
							list.Add(Grid.PosToCell(new Vector2(num2, num)));
						}
					}
				}
			}
			return list.ToArray();
		}

		private void ApplyForeground(WorldGen worldGen, Chunk world, SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			LogInfo("Apply foregreound", (node.tags != null).ToString(), (node.tags != null) ? node.tags.Count : 0);
			if (node.tags == null)
			{
				return;
			}
			FeatureSettings featureSettings = worldGen.Settings.TryGetFeature(node.type);
			LogInfo("\tFeature?", (featureSettings != null).ToString(), 0f);
			if (featureSettings == null && node.tags != null)
			{
				List<Tag> list = new List<Tag>();
				foreach (Tag tag2 in node.tags)
				{
					if (worldGen.Settings.HasFeature(tag2.Name))
					{
						list.Add(tag2);
					}
				}
				LogInfo("\tNo feature, checking possible feature tags, found", "", list.Count);
				if (list.Count > 0)
				{
					Tag tag = list[rnd.RandomSource().Next(list.Count)];
					featureSettings = worldGen.Settings.GetFeature(tag.Name);
					LogInfo("\tPicked feature", tag.Name, 0f);
				}
			}
			if (featureSettings == null)
			{
				return;
			}
			LogInfo("APPLY FOREGROUND", node.type, 0f);
			float num = featureSettings.blobSize.GetRandomValueWithinRange(rnd);
			float num2 = poly.DistanceToClosestEdge();
			if (!node.tags.Contains(WorldGenTags.AllowExceedNodeBorders) && num2 < num)
			{
				if (debugMode)
				{
					Debug.LogWarning(node.type + " " + featureSettings.shape.ToString() + "  blob size too large to fit in node. Size reduced. " + num + "->" + (num2 - 6f));
				}
				num = num2 - 6f;
			}
			if (num <= 0f)
			{
				return;
			}
			List<Vector2I> featureCenterPoints;
			List<List<Vector2I>> featureBorders;
			HashSet<Vector2I> hashSet = DigFeature(featureSettings.shape, num, featureSettings.borders, rnd, out featureCenterPoints, out featureBorders);
			featureSpawnPoints = new HashSet<int>();
			foreach (Vector2I item in hashSet)
			{
				featureSpawnPoints.Add(Grid.XYToCell(item.x, item.y));
			}
			LogInfo("\t\t", "claimed points", featureSpawnPoints.Count);
			availableTerrainPoints.ExceptWith(featureSpawnPoints);
			ApplyPlaceElementForRoom(featureSettings, "RoomCenterElements", featureCenterPoints, worldGen, world, SetValues, temperatureMin, temperatureRange, rnd, worldGen.HighPriorityClaimedCells);
			if (featureBorders != null)
			{
				for (int i = 0; i < featureBorders.Count; i++)
				{
					ApplyPlaceElementForRoom(featureSettings, "RoomBorderChoices" + i, featureBorders[i], worldGen, world, SetValues, temperatureMin, temperatureRange, rnd, worldGen.HighPriorityClaimedCells);
				}
			}
			if (featureSettings.tags.Contains(WorldGenTags.HighPriorityFeature.Name))
			{
				worldGen.AddHighPriorityCells(featureSpawnPoints);
			}
		}

		private void ApplyBackground(WorldGen worldGen, Chunk world, SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			LogInfo("Apply Background", node.type, 0f);
			float floatSetting = worldGen.Settings.GetFloatSetting("CaveOverrideMaxValue");
			float floatSetting2 = worldGen.Settings.GetFloatSetting("CaveOverrideSliverValue");
			Leaf leafForTerrainCell = worldGen.GetLeafForTerrainCell(this);
			bool flag = leafForTerrainCell.tags.Contains(WorldGenTags.IgnoreCaveOverride);
			bool flag2 = leafForTerrainCell.tags.Contains(WorldGenTags.CaveVoidSliver);
			bool flag3 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToCentroid);
			bool flag4 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToCentroidInv);
			bool flag5 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToEdge);
			bool flag6 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToEdgeInv);
			bool flag7 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToBorder);
			bool flag8 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToBorderWeak);
			bool flag9 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToBorderInv);
			bool flag10 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToWorldTop);
			bool flag11 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToWorldTopOrSide);
			bool flag12 = leafForTerrainCell.tags.Contains(WorldGenTags.DistFunctionPointCentroid);
			bool flag13 = leafForTerrainCell.tags.Contains(WorldGenTags.DistFunctionPointEdge);
			LogInfo("Getting Element Bands", node.type, 0f);
			ElementBandConfiguration elementBandConfiguration = worldGen.Settings.GetElementBandForBiome(node.type);
			if (elementBandConfiguration == null && node.biomeSpecificTags != null)
			{
				LogInfo("\tType is not a biome, checking tags", "", node.tags.Count);
				List<ElementBandConfiguration> list = new List<ElementBandConfiguration>();
				foreach (Tag biomeSpecificTag in node.biomeSpecificTags)
				{
					ElementBandConfiguration elementBandForBiome = worldGen.Settings.GetElementBandForBiome(biomeSpecificTag.Name);
					if (elementBandForBiome != null)
					{
						list.Add(elementBandForBiome);
						LogInfo("\tFound biome", biomeSpecificTag.Name, 0f);
					}
				}
				if (list.Count > 0)
				{
					int num = rnd.RandomSource().Next(list.Count);
					elementBandConfiguration = list[num];
					LogInfo("\tPicked biome", "", num);
				}
			}
			DebugUtil.Assert(elementBandConfiguration != null, "A node didn't get assigned a biome! ", node.type);
			foreach (int availableTerrainPoint in availableTerrainPoints)
			{
				Vector2I pos = Grid.CellToXY(availableTerrainPoint);
				if (worldGen.HighPriorityClaimedCells.Contains(availableTerrainPoint))
				{
					continue;
				}
				float num2 = world.overrides[availableTerrainPoint];
				if (!flag && num2 >= 100f)
				{
					if (num2 >= 300f)
					{
						SetValues(availableTerrainPoint, WorldGen.voidElement, WorldGen.voidElement.defaultValues, Sim.DiseaseCell.Invalid);
					}
					else if (num2 >= 200f)
					{
						SetValues(availableTerrainPoint, WorldGen.unobtaniumElement, WorldGen.unobtaniumElement.defaultValues, Sim.DiseaseCell.Invalid);
					}
					else
					{
						SetValues(availableTerrainPoint, WorldGen.katairiteElement, WorldGen.katairiteElement.defaultValues, Sim.DiseaseCell.Invalid);
					}
					continue;
				}
				float num3 = 1f;
				Vector2 vector = new Vector2(pos.x, pos.y);
				if (flag3 || flag4)
				{
					float num4 = 15f;
					if (flag13)
					{
						float timeOnEdge = 0f;
						MathUtil.Pair<Vector2, Vector2> closestEdge = poly.GetClosestEdge(vector, ref timeOnEdge);
						num4 = Vector2.Distance(closestEdge.First + (closestEdge.Second - closestEdge.First) * timeOnEdge, vector);
					}
					num3 = Vector2.Distance(poly.Centroid(), vector) / num4;
					num3 = Mathf.Max(0f, Mathf.Min(1f, num3));
					if (flag4)
					{
						num3 = 1f - num3;
					}
				}
				if (flag6 || flag5)
				{
					float timeOnEdge2 = 0f;
					MathUtil.Pair<Vector2, Vector2> closestEdge2 = poly.GetClosestEdge(vector, ref timeOnEdge2);
					Vector2 a = closestEdge2.First + (closestEdge2.Second - closestEdge2.First) * timeOnEdge2;
					float num5 = 15f;
					if (flag12)
					{
						num5 = Vector2.Distance(poly.Centroid(), vector);
					}
					num3 = Vector2.Distance(a, vector) / num5;
					num3 = Mathf.Max(0f, Mathf.Min(1f, num3));
					if (flag6)
					{
						num3 = 1f - num3;
					}
				}
				if (flag9 || flag7)
				{
					List<Edge> edgesWithTag = worldGen.WorldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeClosed);
					float num6 = float.MaxValue;
					foreach (Edge item in edgesWithTag)
					{
						MathUtil.Pair<Vector2, Vector2> segment = new MathUtil.Pair<Vector2, Vector2>(item.corner0.position, item.corner1.position);
						float closest_point = 0f;
						num6 = Mathf.Min(Mathf.Abs(MathUtil.GetClosestPointBetweenPointAndLineSegment(segment, vector, ref closest_point)), num6);
					}
					float num7 = (flag8 ? 7f : 20f);
					if (flag12)
					{
						num7 = Vector2.Distance(poly.Centroid(), vector);
					}
					num3 = num6 / num7;
					num3 = Mathf.Max(0f, Mathf.Min(1f, num3));
					if (flag9)
					{
						num3 = 1f - num3;
					}
				}
				if (flag10)
				{
					int y = worldGen.WorldSize.y;
					float num8 = 38f;
					float num9 = 58f;
					float num10 = (float)y - vector.y;
					num3 = ((num10 < num8) ? 0f : ((!(num10 < num9)) ? 1f : Mathf.Clamp01((num10 - num8) / (num9 - num8))));
				}
				if (flag11)
				{
					int y2 = worldGen.WorldSize.y;
					int x = worldGen.WorldSize.x;
					float num11 = 2f;
					float num12 = 10f;
					float num13 = (float)y2 - vector.y;
					float x2 = vector.x;
					float num14 = (float)x - vector.x;
					float num15 = Mathf.Min(num13, x2, num14);
					num3 = ((num15 < num11) ? 0f : ((!(num15 < num12)) ? 1f : Mathf.Clamp01((num15 - num11) / (num12 - num11))));
				}
				worldGen.GetElementForBiomePoint(world, elementBandConfiguration, pos, out var element, out var pd, out var dc, num3);
				pd.mass += pd.mass * 0.2f * (world.density[pos.x + world.size.x * pos.y] - 0.5f);
				if (!element.IsVacuum && element.id != SimHashes.Katairite && element.id != SimHashes.Unobtanium)
				{
					float num16 = temperatureMin;
					if (element.lowTempTransition != null && temperatureMin < element.lowTemp)
					{
						num16 = element.lowTemp;
					}
					pd.temperature = num16 + world.heatOffset[availableTerrainPoint] * temperatureRange;
				}
				if (element.IsSolid && !flag && num2 > floatSetting && num2 < 100f)
				{
					element = ((!flag2 || !(num2 > floatSetting2)) ? WorldGen.vacuumElement : WorldGen.voidElement);
					pd = element.defaultValues;
				}
				SetValues(availableTerrainPoint, element, pd, dc);
			}
			if (node.tags.Contains(WorldGenTags.SprinkleOfOxyRock))
			{
				HandleSprinkleOfElement(worldGen.Settings, WorldGenTags.SprinkleOfOxyRock, world, SetValues, temperatureMin, temperatureRange, rnd);
			}
			if (node.tags.Contains(WorldGenTags.SprinkleOfMetal))
			{
				HandleSprinkleOfElement(worldGen.Settings, WorldGenTags.SprinkleOfMetal, world, SetValues, temperatureMin, temperatureRange, rnd);
			}
		}

		private void GenerateActionCells(WorldGenSettings settings, Tag tag, HashSet<int> possiblePoints, SeededRandom rnd)
		{
			ProcGen.Room value = null;
			SettingsCache.rooms.TryGetValue(tag.Name, out value);
			SampleDescriber sampleDescriber = value;
			if (sampleDescriber == null && settings.HasMob(tag.Name))
			{
				sampleDescriber = settings.GetMob(tag.Name);
			}
			if (sampleDescriber == null)
			{
				return;
			}
			HashSet<int> hashSet = new HashSet<int>();
			float randomValueWithinRange = sampleDescriber.density.GetRandomValueWithinRange(rnd);
			List<Vector2> list;
			switch (sampleDescriber.selectMethod)
			{
			case SampleDescriber.PointSelectionMethod.RandomPoints:
				list = PointGenerator.GetRandomPoints(poly, randomValueWithinRange, 0f, null, sampleDescriber.sampleBehaviour, testInsideBounds: true, rnd);
				break;
			default:
				list = new List<Vector2>();
				list.Add(node.position);
				break;
			}
			foreach (Vector2 item2 in list)
			{
				int item = Grid.XYToCell((int)item2.x, (int)item2.y);
				if (possiblePoints.Contains(item))
				{
					hashSet.Add(item);
				}
			}
			if (value == null || value.mobselection != 0)
			{
				return;
			}
			if (terrainPositions == null)
			{
				terrainPositions = new List<KeyValuePair<int, Tag>>();
			}
			foreach (int item3 in hashSet)
			{
				if (Grid.IsValidCell(item3))
				{
					terrainPositions.Add(new KeyValuePair<int, Tag>(item3, tag));
				}
			}
		}

		private void DoProcess(WorldGen worldGen, Chunk world, SetValuesFunction SetValues, SeededRandom rnd)
		{
			float min = 265f;
			float range = 30f;
			InitializeCells(worldGen.ClaimedCells);
			GetTemperatureRange(worldGen, ref min, ref range);
			ApplyForeground(worldGen, world, SetValues, min, range, rnd);
			for (int i = 0; i < node.tags.Count; i++)
			{
				GenerateActionCells(worldGen.Settings, node.tags[i], availableTerrainPoints, rnd);
			}
			ApplyBackground(worldGen, world, SetValues, min, range, rnd);
		}

		public void Process(WorldGen worldGen, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dcs, Chunk world, SeededRandom rnd)
		{
			SetValuesFunction setValues = delegate(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc)
			{
				if (Grid.IsValidCell(index))
				{
					if (pd.temperature == 0f || (elem as Element).HasTag(GameTags.Special))
					{
						bgTemp[index] = -1f;
					}
					cells[index].SetValues(elem as Element, pd, ElementLoader.elements);
					dcs[index] = dc;
				}
				else
				{
					Debug.LogError("Process::SetValuesFunction Index [" + index + "] is not valid. cells.Length [" + cells.Length + "]");
				}
			};
			DoProcess(worldGen, world, setValues, rnd);
		}

		public void Process(WorldGen worldGen, Chunk world, SeededRandom rnd)
		{
			SetValuesFunction setValues = delegate(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc)
			{
				SimMessages.ModifyCell(index, ElementLoader.GetElementIndex((elem as Element).id), pd.temperature, pd.mass, dc.diseaseIdx, dc.elementCount, SimMessages.ReplaceType.Replace);
			};
			DoProcess(worldGen, world, setValues, rnd);
		}

		public int DistanceToTag(Tag tag)
		{
			return distancesToTags[tag];
		}

		public bool IsSafeToSpawnFeatureTemplate(Tag additionalTag)
		{
			if (!node.tags.Contains(additionalTag))
			{
				return !node.tags.ContainsOne(noFeatureSpawnTagSet);
			}
			return false;
		}

		public bool IsSafeToSpawnFeatureTemplate(bool log = true)
		{
			return !node.tags.ContainsOne(noFeatureSpawnTagSet);
		}

		public bool IsSafeToSpawnPOI(List<TerrainCell> allCells, bool log = true)
		{
			return IsSafeToSpawnPOI(allCells, noPOISpawnTags, noPOISpawnTagSet, log);
		}

		public bool IsSafeToSpawnPOIRelaxed(List<TerrainCell> allCells, bool log = true)
		{
			return IsSafeToSpawnPOI(allCells, relaxedNoPOISpawnTags, relaxedNoPOISpawnTagSet, log);
		}

		private bool IsSafeToSpawnPOI(List<TerrainCell> allCells, Tag[] noSpawnTags, TagSet noSpawnTagSet, bool log)
		{
			return !node.tags.ContainsOne(noSpawnTagSet);
		}
	}
}
