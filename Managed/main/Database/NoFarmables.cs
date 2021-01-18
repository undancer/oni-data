using System.IO;
using STRINGS;

namespace Database
{
	public class NoFarmables : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			foreach (PlantablePlot item in Components.PlantablePlots.Items)
			{
				if (!(item.Occupant != null))
				{
					continue;
				}
				Tag[] possibleDepositObjectTags = item.possibleDepositObjectTags;
				for (int i = 0; i < possibleDepositObjectTags.Length; i++)
				{
					if (possibleDepositObjectTags[i] != GameTags.DecorSeed)
					{
						return false;
					}
				}
			}
			return true;
		}

		public override bool Fail()
		{
			return !Success();
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.NO_FARM_TILES;
		}
	}
}
