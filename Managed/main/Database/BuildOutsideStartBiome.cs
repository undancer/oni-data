using System.IO;
using Delaunay.Geo;
using Klei;
using ProcGen;
using STRINGS;

namespace Database
{
	public class BuildOutsideStartBiome : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			WorldDetailSave worldDetailSave = SaveLoader.Instance.worldDetailSave;
			for (int i = 0; i < worldDetailSave.overworldCells.Count; i++)
			{
				WorldDetailSave.OverworldCell overworldCell = worldDetailSave.overworldCells[i];
				if (overworldCell.tags == null || overworldCell.tags.Contains(WorldGenTags.StartWorld))
				{
					continue;
				}
				Polygon poly = overworldCell.poly;
				foreach (BuildingComplete item in Components.BuildingCompletes.Items)
				{
					if (!item.GetComponent<KPrefabID>().HasTag(GameTags.TemplateBuilding) && poly.PointInPolygon(item.transform.GetPosition()))
					{
						Game.Instance.unlocks.Unlock("buildoutsidestartingbiome");
						return true;
					}
				}
			}
			return false;
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_OUTSIDE_START;
		}
	}
}
