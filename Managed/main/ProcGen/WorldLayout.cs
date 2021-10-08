using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay.Geo;
using KSerialization;
using ObjectCloner;
using ProcGen.Map;
using ProcGenGame;
using UnityEngine;
using VoronoiTree;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class WorldLayout
	{
		[Flags]
		public enum DebugFlags
		{
			LocalGraph = 0x1,
			OverworldGraph = 0x2,
			VoronoiTree = 0x4,
			PowerDiagram = 0x8
		}

		[SerializationConfig(MemberSerialization.OptOut)]
		private class ExtraIO
		{
			public List<Leaf> leafs = new List<Leaf>();

			public List<Tree> internals = new List<Tree>();

			public List<KeyValuePair<int, int>> leafInternalParent = new List<KeyValuePair<int, int>>();

			public List<KeyValuePair<int, int>> internalInternalParent = new List<KeyValuePair<int, int>>();

			[OnDeserializing]
			internal void OnDeserializingMethod()
			{
				leafs = new List<Leaf>();
				internals = new List<Tree>();
				leafInternalParent = new List<KeyValuePair<int, int>>();
				internalInternalParent = new List<KeyValuePair<int, int>>();
			}
		}

		private Tree voronoiTree;

		[Serialize]
		public MapGraph localGraph;

		[Serialize]
		public MapGraph overworldGraph;

		[EnumFlags]
		public static DebugFlags drawOptions;

		private LineSegment topEdge;

		private LineSegment bottomEdge;

		private LineSegment leftEdge;

		private LineSegment rightEdge;

		private SeededRandom myRandom;

		[Serialize]
		private ExtraIO extra;

		[Serialize]
		public int mapWidth { get; private set; }

		[Serialize]
		public int mapHeight { get; private set; }

		public bool layoutOK { get; private set; }

		public static LevelLayer levelLayerGradient { get; private set; }

		public WorldGen worldGen { get; private set; }

		public WorldLayout(WorldGen worldGen, int seed)
		{
			this.worldGen = worldGen;
			localGraph = new MapGraph(seed);
			overworldGraph = new MapGraph(seed);
			SetSeed(seed);
		}

		public WorldLayout(WorldGen worldGen, int width, int height, int seed)
			: this(worldGen, seed)
		{
			mapWidth = width;
			mapHeight = height;
		}

		public void SetSeed(int seed)
		{
			myRandom = new SeededRandom(seed);
			localGraph.SetSeed(seed);
			overworldGraph.SetSeed(seed);
		}

		public Tree GetVoronoiTree()
		{
			return voronoiTree;
		}

		public static void SetLayerGradient(LevelLayer newGradient)
		{
			levelLayerGradient = newGradient;
		}

		public static string GetNodeTypeFromLayers(Vector2 point, float mapHeight, SeededRandom rnd)
		{
			string name = WorldGenTags.TheVoid.Name;
			int index = rnd.RandomRange(0, levelLayerGradient[levelLayerGradient.Count - 1].content.Count);
			name = levelLayerGradient[levelLayerGradient.Count - 1].content[index];
			for (int i = 0; i < levelLayerGradient.Count; i++)
			{
				if (point.y < levelLayerGradient[i].maxValue * mapHeight)
				{
					int index2 = rnd.RandomRange(0, levelLayerGradient[i].content.Count);
					name = levelLayerGradient[i].content[index2];
					break;
				}
			}
			return name;
		}

		public Tree GenerateOverworld(bool usePD, bool isRunningDebugGen)
		{
			Debug.Assert(mapWidth != 0 && mapHeight != 0, "Map size has not been set");
			Debug.Assert(worldGen.Settings.world != null, "You need to set a world");
			Diagram.Site site = new Diagram.Site(0u, new Vector2(mapWidth / 2, mapHeight / 2));
			topEdge = new LineSegment(new Vector2(0f, mapHeight - 5), new Vector2(mapWidth, mapHeight - 5));
			bottomEdge = new LineSegment(new Vector2(0f, 5f), new Vector2(mapWidth, 5f));
			leftEdge = new LineSegment(new Vector2(5f, 0f), new Vector2(5f, mapHeight));
			rightEdge = new LineSegment(new Vector2(mapWidth - 5, 0f), new Vector2(mapWidth - 5, mapHeight));
			site.poly = new Polygon(new Rect(0f, 0f, mapWidth, mapHeight));
			voronoiTree = new Tree(site, null, myRandom.seed);
			VoronoiTree.Node.maxIndex = 0u;
			float floatSetting = worldGen.Settings.GetFloatSetting("OverworldDensityMin");
			float floatSetting2 = worldGen.Settings.GetFloatSetting("OverworldDensityMax");
			float density = myRandom.RandomRange(floatSetting, floatSetting2);
			float floatSetting3 = worldGen.Settings.GetFloatSetting("OverworldAvoidRadius");
			PointGenerator.SampleBehaviour enumSetting = worldGen.Settings.GetEnumSetting<PointGenerator.SampleBehaviour>("OverworldSampleBehaviour");
			Cell cell = null;
			if (!string.IsNullOrEmpty(worldGen.Settings.world.startSubworldName))
			{
				WeightedSubworldName weightedSubworldName = worldGen.Settings.world.subworldFiles.Find((WeightedSubworldName x) => x.name == worldGen.Settings.world.startSubworldName);
				Debug.Assert(weightedSubworldName != null, "The start subworld must be listed in the subworld files for a world.");
				Vector2 position = new Vector2((float)mapWidth * worldGen.Settings.world.startingBasePositionHorizontal.GetRandomValueWithinRange(myRandom), (float)mapHeight * worldGen.Settings.world.startingBasePositionVertical.GetRandomValueWithinRange(myRandom));
				cell = overworldGraph.AddNode(weightedSubworldName.name, position);
				SubWorld subWorld = worldGen.Settings.GetSubWorld(weightedSubworldName.name);
				float num = ((weightedSubworldName.overridePower > 0f) ? weightedSubworldName.overridePower : subWorld.pdWeight);
				VoronoiTree.Node node = voronoiTree.AddSite(new Diagram.Site((uint)cell.NodeId, cell.position, num), VoronoiTree.Node.NodeType.Internal);
				node.AddTag(WorldGenTags.AtStart);
				ApplySubworldToNode(node, subWorld, num);
			}
			List<Vector2> list = new List<Vector2>();
			if (cell != null)
			{
				list.Add(cell.position);
			}
			List<Vector2> randomPoints = PointGenerator.GetRandomPoints(site.poly, density, floatSetting3, list, enumSetting, testInsideBounds: false, myRandom, doShuffle: false);
			int intSetting = worldGen.Settings.GetIntSetting("OverworldMinNodes");
			int intSetting2 = worldGen.Settings.GetIntSetting("OverworldMaxNodes");
			if (randomPoints.Count > intSetting2)
			{
				randomPoints.ShuffleSeeded(myRandom.RandomSource());
				randomPoints.RemoveRange(intSetting2, randomPoints.Count - intSetting2);
			}
			if (randomPoints.Count < intSetting)
			{
				throw new Exception($"World layout with fewer than {intSetting} points.");
			}
			for (int i = 0; i < randomPoints.Count; i++)
			{
				Cell cell2 = overworldGraph.AddNode(WorldGenTags.UnassignedNode.Name, randomPoints[i]);
				voronoiTree.AddSite(new Diagram.Site((uint)cell2.NodeId, cell2.position), VoronoiTree.Node.NodeType.Internal).tags.Add(WorldGenTags.UnassignedNode);
				cell2.tags.Add(WorldGenTags.UnassignedNode);
			}
			List<Diagram.Site> list2 = new List<Diagram.Site>();
			for (int j = 0; j < voronoiTree.ChildCount(); j++)
			{
				list2.Add(voronoiTree.GetChild(j).site);
			}
			if (usePD)
			{
				voronoiTree.ComputeNode(list2);
				voronoiTree.ComputeNodePD(list2);
			}
			else
			{
				voronoiTree.ComputeChildren(myRandom.seed + 1);
			}
			voronoiTree.VisitAll(delegate(VoronoiTree.Node n)
			{
				Debug.Assert(n.site.poly != null, $"Node {n.site.id} had a null poly after initial overworld compute!!");
			});
			voronoiTree.AddTagToChildren(WorldGenTags.Overworld);
			TagTopAndBottomSites(WorldGenTags.AtSurface, WorldGenTags.AtDepths);
			TagEdgeSites(WorldGenTags.AtEdge, WorldGenTags.AtEdge);
			ResetMapGraphFromVoronoiTree(voronoiTree.ImmediateChildren(), overworldGraph, clear: true);
			PropagateDistanceTags(voronoiTree, WorldGenTags.DistanceTags);
			ConvertUnknownCells(myRandom, isRunningDebugGen);
			if (worldGen.Settings.GetOverworldAddTags() != null)
			{
				foreach (string overworldAddTag in worldGen.Settings.GetOverworldAddTags())
				{
					int childIndex = myRandom.RandomSource().Next(voronoiTree.ChildCount());
					voronoiTree.GetChild(childIndex).AddTag(new Tag(overworldAddTag));
				}
			}
			if (usePD)
			{
				voronoiTree.ComputeNodePD(list2);
			}
			voronoiTree.VisitAll(delegate(VoronoiTree.Node n)
			{
				Debug.Assert(n.site.poly != null, $"Node {n.site.id} had a null poly after final overworld compute!!");
			});
			FlattenOverworld();
			return voronoiTree;
		}

		public static void ResetMapGraphFromVoronoiTree(List<VoronoiTree.Node> nodes, MapGraph graph, bool clear)
		{
			if (clear)
			{
				graph.ClearEdgesAndCorners();
			}
			for (int i = 0; i < nodes.Count; i++)
			{
				VoronoiTree.Node node = nodes[i];
				Cell cell = graph.FindNodeByID(node.site.id);
				cell.tags.Union(node.tags);
				cell.SetPosition(node.site.position);
				foreach (VoronoiTree.Node neighbor in node.GetNeighbors())
				{
					Cell cell2 = graph.FindNodeByID(neighbor.site.id);
					if (graph.GetArc(cell, cell2) == null)
					{
						int edgeIdx = -1;
						if (node.site.poly.SharesEdge(neighbor.site.poly, ref edgeIdx, out var overlapSegment) == Polygon.Commonality.Edge)
						{
							Corner corner = graph.AddOrGetCorner(overlapSegment.p0.Value);
							Corner corner2 = graph.AddOrGetCorner(overlapSegment.p1.Value);
							graph.AddOrGetEdge(cell, cell2, corner, corner2);
						}
					}
				}
			}
		}

		public void PopulateSubworlds()
		{
			AddSubworldChildren();
			GetStartLocation();
			PropagateStartTag();
		}

		private void PropagateDistanceTags(Tree tree, TagSet tags)
		{
			foreach (Tag tag in tags)
			{
				Dictionary<uint, int> distanceToTag = overworldGraph.GetDistanceToTag(tag);
				if (distanceToTag == null)
				{
					continue;
				}
				int num = 0;
				for (int i = 0; i < tree.ChildCount(); i++)
				{
					VoronoiTree.Node child = tree.GetChild(i);
					uint id = child.site.id;
					if (distanceToTag.ContainsKey(id))
					{
						child.minDistanceToTag.Add(tag, distanceToTag[id]);
						num++;
						if (distanceToTag[id] > 0)
						{
							child.AddTag(new Tag(tag.Name + "_Distance" + distanceToTag[id]));
						}
					}
				}
			}
		}

		private HashSet<WeightedSubWorld> GetNameFilterSet(VoronoiTree.Node vn, World.AllowedCellsFilter filter, List<WeightedSubWorld> subworlds)
		{
			HashSet<WeightedSubWorld> hashSet = new HashSet<WeightedSubWorld>();
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
			{
				int i;
				for (i = 0; i < filter.subworldNames.Count; i++)
				{
					hashSet.UnionWith(subworlds.FindAll((WeightedSubWorld f) => f.subWorld.name == filter.subworldNames[i]));
				}
				break;
			}
			case World.AllowedCellsFilter.TagCommand.AtTag:
			{
				if (!vn.tags.Contains(filter.tag))
				{
					break;
				}
				int j;
				for (j = 0; j < filter.subworldNames.Count; j++)
				{
					hashSet.UnionWith(subworlds.FindAll((WeightedSubWorld f) => f.subWorld.name == filter.subworldNames[j]));
				}
				break;
			}
			case World.AllowedCellsFilter.TagCommand.NotAtTag:
			{
				if (vn.tags.Contains(filter.tag))
				{
					break;
				}
				int k;
				for (k = 0; k < filter.subworldNames.Count; k++)
				{
					hashSet.UnionWith(subworlds.FindAll((WeightedSubWorld f) => f.subWorld.name == filter.subworldNames[k]));
				}
				break;
			}
			case World.AllowedCellsFilter.TagCommand.DistanceFromTag:
			{
				Debug.Assert(vn.minDistanceToTag.ContainsKey(filter.tag.ToTag()), filter.tag);
				if (vn.minDistanceToTag[filter.tag.ToTag()] < filter.minDistance || vn.minDistanceToTag[filter.tag.ToTag()] > filter.maxDistance)
				{
					break;
				}
				int l;
				for (l = 0; l < filter.subworldNames.Count; l++)
				{
					hashSet.UnionWith(subworlds.FindAll((WeightedSubWorld f) => f.subWorld.name == filter.subworldNames[l]));
				}
				break;
			}
			}
			return hashSet;
		}

		private HashSet<WeightedSubWorld> GetZoneTypeFilterSet(VoronoiTree.Node vn, World.AllowedCellsFilter filter, Dictionary<string, List<WeightedSubWorld>> subworldsByZoneType)
		{
			HashSet<WeightedSubWorld> hashSet = new HashSet<WeightedSubWorld>();
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
			{
				for (int l = 0; l < filter.zoneTypes.Count; l++)
				{
					hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[l].ToString()]);
				}
				break;
			}
			case World.AllowedCellsFilter.TagCommand.AtTag:
				if (vn.tags.Contains(filter.tag))
				{
					for (int k = 0; k < filter.zoneTypes.Count; k++)
					{
						hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[k].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.NotAtTag:
				if (!vn.tags.Contains(filter.tag))
				{
					for (int j = 0; j < filter.zoneTypes.Count; j++)
					{
						hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[j].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.DistanceFromTag:
				Debug.Assert(vn.minDistanceToTag.ContainsKey(filter.tag.ToTag()), filter.tag);
				if (vn.minDistanceToTag[filter.tag.ToTag()] >= filter.minDistance && vn.minDistanceToTag[filter.tag.ToTag()] <= filter.maxDistance)
				{
					for (int i = 0; i < filter.zoneTypes.Count; i++)
					{
						hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[i].ToString()]);
					}
				}
				break;
			}
			return hashSet;
		}

		private HashSet<WeightedSubWorld> GetTemperatureFilterSet(VoronoiTree.Node vn, World.AllowedCellsFilter filter, Dictionary<string, List<WeightedSubWorld>> subworldsByTemperature)
		{
			HashSet<WeightedSubWorld> hashSet = new HashSet<WeightedSubWorld>();
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
			{
				for (int l = 0; l < filter.temperatureRanges.Count; l++)
				{
					hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[l].ToString()]);
				}
				break;
			}
			case World.AllowedCellsFilter.TagCommand.AtTag:
				if (vn.tags.Contains(filter.tag))
				{
					for (int k = 0; k < filter.temperatureRanges.Count; k++)
					{
						hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[k].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.NotAtTag:
				if (!vn.tags.Contains(filter.tag))
				{
					for (int j = 0; j < filter.temperatureRanges.Count; j++)
					{
						hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[j].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.DistanceFromTag:
				Debug.Assert(vn.minDistanceToTag.ContainsKey(filter.tag.ToTag()), filter.tag);
				if (vn.minDistanceToTag[filter.tag.ToTag()] >= filter.minDistance && vn.minDistanceToTag[filter.tag.ToTag()] <= filter.maxDistance)
				{
					for (int i = 0; i < filter.temperatureRanges.Count; i++)
					{
						hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[i].ToString()]);
					}
				}
				break;
			}
			return hashSet;
		}

		private void RunFilterClearCommand(VoronoiTree.Node vn, World.AllowedCellsFilter filter, HashSet<WeightedSubWorld> allowedSubworldsSet)
		{
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
				allowedSubworldsSet.Clear();
				break;
			case World.AllowedCellsFilter.TagCommand.AtTag:
				if (vn.tags.Contains(filter.tag))
				{
					allowedSubworldsSet.Clear();
				}
				break;
			case World.AllowedCellsFilter.TagCommand.NotAtTag:
				if (!vn.tags.Contains(filter.tag))
				{
					allowedSubworldsSet.Clear();
				}
				break;
			case World.AllowedCellsFilter.TagCommand.DistanceFromTag:
				Debug.Assert(vn.minDistanceToTag.ContainsKey(filter.tag.ToTag()), filter.tag);
				if (vn.minDistanceToTag[filter.tag.ToTag()] >= filter.minDistance && vn.minDistanceToTag[filter.tag.ToTag()] <= filter.maxDistance)
				{
					allowedSubworldsSet.Clear();
				}
				break;
			}
		}

		private HashSet<WeightedSubWorld> Filter(VoronoiTree.Node vn, List<WeightedSubWorld> allSubWorlds, Dictionary<string, List<WeightedSubWorld>> subworldsByTemperature, Dictionary<string, List<WeightedSubWorld>> subworldsByZoneType)
		{
			HashSet<WeightedSubWorld> hashSet = new HashSet<WeightedSubWorld>();
			World world = worldGen.Settings.world;
			string text = "";
			foreach (KeyValuePair<Tag, int> item in vn.minDistanceToTag)
			{
				text = text + item.Key.Name + ":" + item.Value + ", ";
			}
			foreach (World.AllowedCellsFilter unknownCellsAllowedSubworld in world.unknownCellsAllowedSubworlds)
			{
				HashSet<WeightedSubWorld> hashSet2 = new HashSet<WeightedSubWorld>();
				if (unknownCellsAllowedSubworld.subworldNames != null && unknownCellsAllowedSubworld.subworldNames.Count > 0)
				{
					hashSet2.UnionWith(GetNameFilterSet(vn, unknownCellsAllowedSubworld, allSubWorlds));
				}
				if (unknownCellsAllowedSubworld.temperatureRanges != null && unknownCellsAllowedSubworld.temperatureRanges.Count > 0)
				{
					hashSet2.UnionWith(GetTemperatureFilterSet(vn, unknownCellsAllowedSubworld, subworldsByTemperature));
				}
				if (unknownCellsAllowedSubworld.zoneTypes != null && unknownCellsAllowedSubworld.zoneTypes.Count > 0)
				{
					hashSet2.UnionWith(GetZoneTypeFilterSet(vn, unknownCellsAllowedSubworld, subworldsByZoneType));
				}
				switch (unknownCellsAllowedSubworld.command)
				{
				case World.AllowedCellsFilter.Command.All:
					Debug.LogError("Command.All is unsupported for unknownCellsAllowedSubworlds.");
					break;
				case World.AllowedCellsFilter.Command.Clear:
					RunFilterClearCommand(vn, unknownCellsAllowedSubworld, hashSet);
					break;
				case World.AllowedCellsFilter.Command.Replace:
					if (hashSet2.Count > 0)
					{
						hashSet.Clear();
						hashSet.UnionWith(hashSet2);
					}
					break;
				case World.AllowedCellsFilter.Command.UnionWith:
					hashSet.UnionWith(hashSet2);
					break;
				case World.AllowedCellsFilter.Command.ExceptWith:
					hashSet.ExceptWith(hashSet2);
					break;
				case World.AllowedCellsFilter.Command.IntersectWith:
					hashSet.IntersectWith(hashSet2);
					break;
				case World.AllowedCellsFilter.Command.SymmetricExceptWith:
					hashSet.SymmetricExceptWith(hashSet2);
					break;
				}
			}
			return hashSet;
		}

		private void ConvertUnknownCells(SeededRandom myRandom, bool isRunningDebugGen)
		{
			List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
			voronoiTree.GetNodesWithTag(WorldGenTags.UnassignedNode, list);
			list.ShuffleSeeded(myRandom.RandomSource());
			List<WeightedSubworldName> subworldList = new List<WeightedSubworldName>(worldGen.Settings.world.subworldFiles);
			List<WeightedSubWorld> subworldsForWorld = worldGen.Settings.GetSubworldsForWorld(subworldList);
			Dictionary<string, List<WeightedSubWorld>> dictionary = new Dictionary<string, List<WeightedSubWorld>>();
			foreach (Temperature.Range range in Enum.GetValues(typeof(Temperature.Range)))
			{
				dictionary.Add(range.ToString(), subworldsForWorld.FindAll((WeightedSubWorld sw) => sw.subWorld.temperatureRange == range));
			}
			Dictionary<string, List<WeightedSubWorld>> dictionary2 = new Dictionary<string, List<WeightedSubWorld>>();
			foreach (SubWorld.ZoneType zt in Enum.GetValues(typeof(SubWorld.ZoneType)))
			{
				dictionary2.Add(zt.ToString(), subworldsForWorld.FindAll((WeightedSubWorld sw) => sw.subWorld.zoneType == zt));
			}
			foreach (VoronoiTree.Node item in list)
			{
				Node node = overworldGraph.FindNodeByID(item.site.id);
				item.tags.Remove(WorldGenTags.UnassignedNode);
				node.tags.Remove(WorldGenTags.UnassignedNode);
				List<WeightedSubWorld> list2 = new List<WeightedSubWorld>(Filter(item, subworldsForWorld, dictionary, dictionary2));
				List<WeightedSubWorld> list3 = list2.FindAll((WeightedSubWorld x) => x.minCount > 0);
				WeightedSubWorld weightedSubWorld;
				if (list3.Count > 0)
				{
					weightedSubWorld = list3[0];
					int priority = weightedSubWorld.priority;
					foreach (WeightedSubWorld item2 in list3)
					{
						if (item2.priority > priority || (item2.priority == priority && item2.minCount > weightedSubWorld.minCount))
						{
							weightedSubWorld = item2;
							priority = item2.priority;
						}
					}
					weightedSubWorld.minCount--;
				}
				else
				{
					weightedSubWorld = WeightedRandom.Choose(list2, myRandom);
				}
				if (weightedSubWorld != null)
				{
					ApplySubworldToNode(item, weightedSubWorld.subWorld, weightedSubWorld.overridePower);
					weightedSubWorld.maxCount--;
					if (weightedSubWorld.maxCount <= 0)
					{
						subworldsForWorld.Remove(weightedSubWorld);
					}
					continue;
				}
				string text = "";
				foreach (KeyValuePair<Tag, int> item3 in item.minDistanceToTag)
				{
					text = text + item3.Key.Name + ":" + item3.Value + ", ";
				}
				DebugUtil.LogWarningArgs("No allowed Subworld types. Using default. ", node.tags.ToString(), "Distances:", text);
				node.SetType("Default");
			}
			foreach (WeightedSubWorld item4 in subworldsForWorld)
			{
				if (item4.minCount > 0)
				{
					if (!isRunningDebugGen)
					{
						throw new Exception($"Could not guarantee minCount of Subworld {item4.subWorld.name}, {item4.minCount} remaining on world {worldGen.Settings.world.filePath}.");
					}
					DebugUtil.DevLogError($"Could not guarantee minCount of Subworld {item4.subWorld.name}, {item4.minCount} remaining on world {worldGen.Settings.world.filePath}.");
				}
			}
		}

		private Node ApplySubworldToNode(VoronoiTree.Node vn, SubWorld subWorld, float overridePower = -1f)
		{
			Node node = overworldGraph.FindNodeByID(vn.site.id);
			node.SetType(subWorld.name);
			vn.site.weight = ((overridePower > 0f) ? overridePower : subWorld.pdWeight);
			foreach (string tag in subWorld.tags)
			{
				vn.AddTag(new Tag(tag));
			}
			vn.AddTag(subWorld.zoneType.ToString());
			return node;
		}

		private void FlattenOverworld()
		{
			try
			{
				ResetMapGraphFromVoronoiTree(voronoiTree.ImmediateChildren(), overworldGraph, clear: true);
				foreach (Edge arc in overworldGraph.arcs)
				{
					List<Cell> nodes = overworldGraph.GetNodes(arc);
					Cell cell = nodes[0];
					Cell cell2 = nodes[1];
					SubWorld subWorld = worldGen.Settings.GetSubWorld(cell.type);
					Debug.Assert(subWorld != null, "SubWorld is null: " + cell.type);
					SubWorld subWorld2 = worldGen.Settings.GetSubWorld(cell2.type);
					Debug.Assert(subWorld2 != null, "other SubWorld is null: " + cell2.type);
					if (cell.type == cell2.type || subWorld.zoneType == subWorld2.zoneType)
					{
						arc.tags.Add(WorldGenTags.EdgeOpen);
					}
					else if (subWorld.borderOverride == "NONE" || subWorld2.borderOverride == "NONE")
					{
						arc.tags.Add(WorldGenTags.EdgeOpen);
					}
					else
					{
						arc.tags.Add(WorldGenTags.EdgeClosed);
					}
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				Debug.LogError("ex: " + message + " " + stackTrace);
			}
		}

		public static bool TestEdgeConsistency(MapGraph graph, Cell cell, out Edge problemEdge)
		{
			List<Edge> arcs = graph.GetArcs(cell);
			foreach (Edge item in arcs)
			{
				int num = 0;
				int num2 = 0;
				foreach (Edge item2 in arcs)
				{
					if (item2.corner0 == item.corner0 || item2.corner1 == item.corner0)
					{
						num++;
					}
					if (item2.corner1 == item.corner1 || item2.corner1 == item.corner1)
					{
						num2++;
					}
				}
				if (num != 2 || num2 != 2)
				{
					problemEdge = item;
					return false;
				}
			}
			problemEdge = null;
			return true;
		}

		private void AddSubworldChildren()
		{
			new TagSet().Add(WorldGenTags.Overworld);
			List<string> defaultMoveTags = worldGen.Settings.GetDefaultMoveTags();
			if (defaultMoveTags != null)
			{
				new TagSet(defaultMoveTags);
			}
			List<Feature> list = new List<Feature>();
			foreach (KeyValuePair<string, int> globalFeature in worldGen.Settings.world.globalFeatures)
			{
				for (int i = 0; i < globalFeature.Value; i++)
				{
					Feature feature = new Feature();
					feature.type = globalFeature.Key;
					list.Add(feature);
				}
			}
			Dictionary<uint, List<Feature>> dictionary = new Dictionary<uint, List<Feature>>();
			List<VoronoiTree.Node> list2 = new List<VoronoiTree.Node>();
			voronoiTree.GetNodesWithoutTag(WorldGenTags.NoGlobalFeatureSpawning, list2);
			list2.ShuffleSeeded(myRandom.RandomSource());
			foreach (Feature item in list)
			{
				if (list2.Count == 0)
				{
					break;
				}
				VoronoiTree.Node node = list2[0];
				list2.RemoveAt(0);
				if (!dictionary.ContainsKey(node.site.id))
				{
					dictionary[node.site.id] = new List<Feature>();
				}
				dictionary[node.site.id].Add(item);
			}
			localGraph.ClearEdgesAndCorners();
			for (int j = 0; j < voronoiTree.ChildCount(); j++)
			{
				VoronoiTree.Node child2 = voronoiTree.GetChild(j);
				if (child2.type == VoronoiTree.Node.NodeType.Internal)
				{
					Tree child = child2 as Tree;
					Node node2 = overworldGraph.FindNodeByID(child.site.id);
					SubWorld subWorld = SerializingCloner.Copy(worldGen.Settings.GetSubWorld(node2.type));
					child.AddTag(new Tag(node2.type));
					child.AddTag(new Tag(subWorld.temperatureRange.ToString()));
					child.AddTag(new Tag(subWorld.zoneType.ToString()));
					if (dictionary.ContainsKey(child2.site.id))
					{
						subWorld.features.AddRange(dictionary[child2.site.id]);
					}
					GenerateChildren(subWorld, child, localGraph, mapHeight, j + myRandom.seed);
					child.RelaxRecursive(0, 10, 1f, worldGen.Settings.world.layoutMethod == World.LayoutMethod.PowerTree);
					child.VisitAll(delegate(VoronoiTree.Node n)
					{
						Debug.Assert(n.site.poly != null, $"Node {n.site.id}, child of {child.site.id} had a null poly after final subworld relax!!");
					});
				}
			}
			VoronoiTree.Node.maxDepth = voronoiTree.MaxDepth();
		}

		private List<Vector2> GetPoints(string name, LoggerSSF log, int minPointCount, int maxPointCount, Polygon boundingArea, float density, float avoidRadius, List<Vector2> avoidPoints, PointGenerator.SampleBehaviour sampleBehaviour, bool testInsideBounds, SeededRandom rnd, bool doShuffle = true, bool testAvoidPoints = true)
		{
			List<Vector2> list = null;
			int num = 0;
			do
			{
				list = PointGenerator.GetRandomPoints(boundingArea, density, avoidRadius, avoidPoints, sampleBehaviour, testInsideBounds, rnd, doShuffle, testAvoidPoints);
				if (list.Count < minPointCount)
				{
					density *= 0.8f;
					avoidRadius *= 0.8f;
					_ = worldGen.isRunningDebugGen;
				}
				num++;
			}
			while (list.Count < minPointCount && list.Count <= maxPointCount && num < 10);
			if (list.Count > maxPointCount)
			{
				list.RemoveRange(maxPointCount, list.Count - maxPointCount);
			}
			return list;
		}

		public void GenerateChildren(SubWorld sw, Tree node, MapGraph graph, float worldHeight, int seed)
		{
			SeededRandom seededRandom = new SeededRandom(seed);
			List<string> defaultMoveTags = worldGen.Settings.GetDefaultMoveTags();
			TagSet tagSet = ((defaultMoveTags != null) ? new TagSet(defaultMoveTags) : null);
			TagSet tagSet2 = new TagSet();
			if (tagSet != null)
			{
				for (int i = 0; i < tagSet.Count; i++)
				{
					Tag item = tagSet[i];
					if (node.tags.Contains(item))
					{
						node.tags.Remove(item);
						tagSet2.Add(item);
					}
				}
			}
			TagSet tagSet3 = new TagSet(node.tags);
			tagSet3.Remove(WorldGenTags.Overworld);
			for (int j = 0; j < sw.tags.Count; j++)
			{
				tagSet3.Add(new Tag(sw.tags[j]));
			}
			float randomValueWithinRange = sw.density.GetRandomValueWithinRange(seededRandom);
			List<Vector2> list = new List<Vector2>();
			if (sw.centralFeature != null)
			{
				list.Add(node.site.poly.Centroid());
				CreateTreeNodeWithFeatureAndBiome(worldGen.Settings, sw, node, graph, sw.centralFeature, node.site.poly.Centroid(), tagSet3, -1).AddTag(WorldGenTags.CenteralFeature);
			}
			node.dontRelaxChildren = sw.dontRelaxChildren;
			int num = Mathf.Max(sw.features.Count + sw.extraBiomeChildren, sw.minChildCount);
			int maxPointCount = int.MaxValue;
			if (sw.singleChildCount)
			{
				num = 1;
				maxPointCount = 1;
			}
			List<Vector2> points = GetPoints(sw.name, node.log, num, maxPointCount, node.site.poly, randomValueWithinRange, sw.avoidRadius, list, sw.sampleBehaviour, testInsideBounds: true, seededRandom, doShuffle: true, sw.doAvoidPoints);
			Debug.Assert(points.Count >= num, $"Overworld node {node.site.id} of subworld {sw.name} generated {points.Count} points of an expected minimum {num}\nThis probably means that either:\n* sampler density is too large (lower the number for tighter samples)\n* avoid radius is too large (only applies if there is a central feature, especialy if you get 0 points generated)\n* min point count is just plain too large.");
			for (int k = 0; k < sw.samplers.Count; k++)
			{
				list.AddRange(points);
				float randomValueWithinRange2 = sw.samplers[k].density.GetRandomValueWithinRange(seededRandom);
				List<Vector2> randomPoints = PointGenerator.GetRandomPoints(node.site.poly, randomValueWithinRange2, sw.samplers[k].avoidRadius, list, sw.samplers[k].sampleBehaviour, testInsideBounds: true, seededRandom, doShuffle: true, sw.samplers[k].doAvoidPoints);
				points.AddRange(randomPoints);
			}
			if (points.Count > 200)
			{
				points.RemoveRange(200, points.Count - 200);
			}
			if (points.Count < num)
			{
				string text = "";
				for (int l = 0; l < node.site.poly.Vertices.Count; l++)
				{
					text = text + node.site.poly.Vertices[l].ToString() + ", ";
				}
				if (worldGen.isRunningDebugGen)
				{
					Debug.Assert(points.Count >= num, "Error not enough points " + sw.name + " in node " + node.site.id);
				}
				return;
			}
			_ = sw.features.Count;
			_ = points.Count;
			for (int m = 0; m < points.Count; m++)
			{
				Feature feature = null;
				if (m < sw.features.Count)
				{
					feature = sw.features[m];
				}
				CreateTreeNodeWithFeatureAndBiome(worldGen.Settings, sw, node, graph, feature, points[m], tagSet3, m);
			}
			node.ComputeChildren(seededRandom.seed + 1);
			node.VisitAll(delegate(VoronoiTree.Node n)
			{
				Debug.Assert(n.site.poly != null, $"Node {n.site.id}, child of {node.site.id} had a null poly after final subworld compute!!");
			});
			if (node.ChildCount() > 0)
			{
				for (int num2 = 0; num2 < tagSet2.Count; num2++)
				{
					Debug.Log($"Applying Moved Tag {tagSet2[num2].Name} to {node.site.id}");
					node.GetChild(seededRandom.RandomSource().Next(node.ChildCount())).AddTag(tagSet2[num2]);
				}
			}
		}

		private VoronoiTree.Node CreateTreeNodeWithFeatureAndBiome(WorldGenSettings settings, SubWorld sw, Tree node, MapGraph graph, Feature feature, Vector2 pos, TagSet newTags, int i)
		{
			string text = null;
			bool flag = false;
			TagSet tagSet = new TagSet();
			TagSet tagSet2 = new TagSet();
			if (feature != null)
			{
				FeatureSettings feature2 = settings.GetFeature(feature.type);
				text = feature.type;
				tagSet2.Union(new TagSet(feature2.tags));
				if (feature.tags != null && feature.tags.Count > 0)
				{
					tagSet2.Union(new TagSet(feature.tags));
				}
				if (feature.excludesTags != null && feature.excludesTags.Count > 0)
				{
					tagSet2.Remove(new TagSet(feature.excludesTags));
				}
				tagSet2.Add(new Tag(feature.type));
				tagSet2.Add(WorldGenTags.Feature);
				if (feature2.forceBiome != null)
				{
					tagSet.Add(feature2.forceBiome);
					flag = true;
				}
				if (feature2.biomeTags != null)
				{
					tagSet.Union(new TagSet(feature2.biomeTags));
				}
			}
			if (!flag && sw.biomes.Count > 0)
			{
				WeightedBiome weightedBiome = WeightedRandom.Choose(sw.biomes, myRandom);
				if (text == null)
				{
					text = weightedBiome.name;
				}
				tagSet.Add(weightedBiome.name);
				if (weightedBiome.tags != null && weightedBiome.tags.Count > 0)
				{
					tagSet.Union(new TagSet(weightedBiome.tags));
				}
				flag = true;
			}
			if (!flag)
			{
				text = "UNKNOWN";
				Debug.LogError("Couldn't get a biome for a cell in " + sw.name + ". Maybe it doesn't have any biomes configured?");
			}
			Cell cell = graph.AddNode(text, pos);
			cell.biomeSpecificTags = new TagSet(tagSet);
			cell.featureSpecificTags = new TagSet(tagSet2);
			VoronoiTree.Node node2 = node.AddSite(new Diagram.Site((uint)cell.NodeId, cell.position), VoronoiTree.Node.NodeType.Internal);
			node2.tags = new TagSet(newTags);
			node2.tags.Add(text);
			node2.tags.Union(tagSet);
			node2.tags.Union(tagSet2);
			return node2;
		}

		private void TagTopAndBottomSites(Tag topTag, Tag bottomTag)
		{
			List<Diagram.Site> list = new List<Diagram.Site>();
			List<Diagram.Site> list2 = new List<Diagram.Site>();
			voronoiTree.GetIntersectingLeafSites(topEdge, list);
			voronoiTree.GetIntersectingLeafSites(bottomEdge, list2);
			for (int i = 0; i < list.Count; i++)
			{
				voronoiTree.GetNodeForSite(list[i]).AddTag(topTag);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				voronoiTree.GetNodeForSite(list2[j]).AddTag(bottomTag);
			}
		}

		private void TagEdgeSites(Tag leftTag, Tag rightTag)
		{
			List<Diagram.Site> list = new List<Diagram.Site>();
			List<Diagram.Site> list2 = new List<Diagram.Site>();
			voronoiTree.GetIntersectingLeafSites(leftEdge, list);
			voronoiTree.GetIntersectingLeafSites(rightEdge, list2);
			for (int i = 0; i < list.Count; i++)
			{
				voronoiTree.GetNodeForSite(list[i]).AddTag(leftTag);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				voronoiTree.GetNodeForSite(list2[j]).AddTag(rightTag);
			}
		}

		private bool StartAreaTooLarge(VoronoiTree.Node node)
		{
			if (node.tags.Contains(WorldGenTags.AtStart))
			{
				return node.site.poly.Area() > 2000f;
			}
			return false;
		}

		private void PropagateStartTag()
		{
			foreach (VoronoiTree.Node startNode in GetStartNodes())
			{
				startNode.AddTagToNeighbors(WorldGenTags.NearStartLocation);
				startNode.AddTag(WorldGenTags.IgnoreCaveOverride);
			}
		}

		public List<VoronoiTree.Node> GetStartNodes()
		{
			return GetLeafNodesWithTag(WorldGenTags.StartLocation);
		}

		public List<VoronoiTree.Node> GetLeafNodesWithTag(Tag tag)
		{
			List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
			voronoiTree.GetLeafNodes(list, (VoronoiTree.Node node) => node.tags != null && node.tags.Contains(tag));
			return list;
		}

		public List<Node> GetTerrainNodesForTag(Tag tag)
		{
			List<Node> list = new List<Node>();
			foreach (VoronoiTree.Node item in GetLeafNodesWithTag(tag))
			{
				Node node = localGraph.FindNodeByID(item.site.id);
				if (node != null)
				{
					list.Add(node);
				}
			}
			return list;
		}

		private Node FindFirstNode(string nodeType)
		{
			return localGraph.FindNode((Cell node) => node.type == nodeType);
		}

		private Node FindFirstNodeWithTag(Tag tag)
		{
			return localGraph.FindNode((Cell node) => node.tags != null && node.tags.Contains(tag));
		}

		public Vector2I GetStartLocation()
		{
			if (string.IsNullOrEmpty(worldGen.Settings.world.startSubworldName))
			{
				Debug.Log("World (" + worldGen.Settings.world.filePath + ") does not have a starting subworld specified");
				return new Vector2I(mapWidth / 2, mapHeight / 2);
			}
			Node node2 = FindFirstNodeWithTag(WorldGenTags.StartLocation);
			if (node2 == null)
			{
				List<VoronoiTree.Node> nodes = GetStartNodes();
				if (nodes == null || nodes.Count == 0)
				{
					Debug.LogWarning("Couldnt find start node");
					return new Vector2I(mapWidth / 2, mapHeight / 2);
				}
				node2 = localGraph.FindNode((Cell node) => (uint)node.NodeId == nodes[0].site.id);
				node2.tags.Add(WorldGenTags.StartLocation);
			}
			if (node2 == null)
			{
				Debug.LogWarning("Couldnt find start node");
				return new Vector2I(mapWidth / 2, mapHeight / 2);
			}
			return new Vector2I((int)node2.position.x, (int)node2.position.y);
		}

		private List<Diagram.Site> GetIntersectingSites(VoronoiTree.Node intersectingSiteSource, Tree sitesSource)
		{
			List<Diagram.Site> list = new List<Diagram.Site>();
			list = new List<Diagram.Site>();
			LineSegment edge;
			for (int i = 1; i < intersectingSiteSource.site.poly.Vertices.Count - 1; i++)
			{
				edge = new LineSegment(intersectingSiteSource.site.poly.Vertices[i - 1], intersectingSiteSource.site.poly.Vertices[i]);
				sitesSource.GetIntersectingLeafSites(edge, list);
			}
			edge = new LineSegment(intersectingSiteSource.site.poly.Vertices[intersectingSiteSource.site.poly.Vertices.Count - 1], intersectingSiteSource.site.poly.Vertices[0]);
			sitesSource.GetIntersectingLeafSites(edge, list);
			return list;
		}

		public void GetEdgeOfMapSites(Tree vt, List<Diagram.Site> topSites, List<Diagram.Site> bottomSites, List<Diagram.Site> leftSites, List<Diagram.Site> rightSites)
		{
			vt.GetIntersectingLeafSites(topEdge, topSites);
			vt.GetIntersectingLeafSites(bottomEdge, bottomSites);
			vt.GetIntersectingLeafSites(leftEdge, leftSites);
			vt.GetIntersectingLeafSites(rightEdge, rightSites);
		}

		[OnSerializing]
		internal void OnSerializingMethod()
		{
			try
			{
				extra = new ExtraIO();
				if (voronoiTree == null)
				{
					return;
				}
				extra.internals.Add(voronoiTree);
				voronoiTree.GetInternalNodes(extra.internals);
				List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
				voronoiTree.GetLeafNodes(list);
				foreach (Leaf ln in list)
				{
					if (ln != null)
					{
						extra.leafInternalParent.Add(new KeyValuePair<int, int>(extra.leafs.Count, extra.internals.FindIndex(0, (Tree n) => n == ln.parent)));
						extra.leafs.Add(ln);
					}
				}
				for (int i = 0; i < extra.internals.Count; i++)
				{
					Tree vt = extra.internals[i];
					if (vt.parent != null)
					{
						int num = extra.internals.FindIndex(0, (Tree n) => n == vt.parent);
						if (num >= 0)
						{
							extra.internalInternalParent.Add(new KeyValuePair<int, int>(i, num));
						}
					}
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				Debug.Log("Error deserialising " + ex.Message);
			}
		}

		[OnSerialized]
		internal void OnSerializedMethod()
		{
			extra = null;
		}

		[OnDeserializing]
		internal void OnDeserializingMethod()
		{
			extra = new ExtraIO();
		}

		[OnDeserialized]
		internal void OnDeserializedMethod()
		{
			try
			{
				voronoiTree = extra.internals[0];
				for (int i = 0; i < extra.internalInternalParent.Count; i++)
				{
					KeyValuePair<int, int> keyValuePair = extra.internalInternalParent[i];
					Tree child = extra.internals[keyValuePair.Key];
					extra.internals[keyValuePair.Value].AddChild(child);
				}
				for (int j = 0; j < extra.leafInternalParent.Count; j++)
				{
					KeyValuePair<int, int> keyValuePair2 = extra.leafInternalParent[j];
					VoronoiTree.Node child2 = extra.leafs[keyValuePair2.Key];
					extra.internals[keyValuePair2.Value].AddChild(child2);
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				Debug.Log("Error deserialising " + ex.Message);
			}
			extra = null;
		}
	}
}
