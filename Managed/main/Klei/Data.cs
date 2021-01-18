using System.Collections.Generic;
using ProcGen;
using ProcGenGame;
using VoronoiTree;

namespace Klei
{
	public class Data
	{
		public int globalWorldSeed = 0;

		public int globalWorldLayoutSeed = 0;

		public int globalTerrainSeed = 0;

		public int globalNoiseSeed = 0;

		public int chunkEdgeSize = 32;

		public WorldLayout worldLayout = null;

		public List<TerrainCell> terrainCells = null;

		public List<TerrainCell> overworldCells = null;

		public List<ProcGen.River> rivers = null;

		public GameSpawnData gameSpawnData = null;

		public Chunk world = null;

		public Tree voronoiTree = null;

		public AxialI clusterLocation;

		public Data()
		{
			worldLayout = new WorldLayout(null, 0);
			terrainCells = new List<TerrainCell>();
			overworldCells = new List<TerrainCell>();
			rivers = new List<ProcGen.River>();
			gameSpawnData = new GameSpawnData();
			world = new Chunk();
			voronoiTree = new Tree(0);
		}
	}
}
