using System.Collections.Generic;
using ProcGen.Map;
using ProcGenGame;
using VoronoiTree;

namespace Klei
{
	public class TerrainCellLogged : TerrainCell
	{
		public TerrainCellLogged()
		{
		}

		public TerrainCellLogged(Cell node, Diagram.Site site, Dictionary<Tag, int> distancesToTags)
			: base(node, site, distancesToTags)
		{
		}

		public override void LogInfo(string evt, string param, float value)
		{
		}
	}
}
