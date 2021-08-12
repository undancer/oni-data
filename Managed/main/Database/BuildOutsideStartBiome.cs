using Klei;
using ProcGen;
using STRINGS;

namespace Database
{
	public class BuildOutsideStartBiome : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public override bool Success()
		{
			WorldDetailSave clusterDetailSave = SaveLoader.Instance.clusterDetailSave;
			foreach (BuildingComplete item in Components.BuildingCompletes.Items)
			{
				if (item.GetComponent<KPrefabID>().HasTag(GameTags.TemplateBuilding))
				{
					continue;
				}
				for (int i = 0; i < clusterDetailSave.overworldCells.Count; i++)
				{
					WorldDetailSave.OverworldCell overworldCell = clusterDetailSave.overworldCells[i];
					if (overworldCell.tags != null && !overworldCell.tags.Contains(WorldGenTags.StartWorld) && overworldCell.poly.PointInPolygon(item.transform.GetPosition()))
					{
						Game.Instance.unlocks.Unlock("buildoutsidestartingbiome");
						return true;
					}
				}
			}
			return false;
		}

		public void Deserialize(IReader reader)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_OUTSIDE_START;
		}
	}
}
