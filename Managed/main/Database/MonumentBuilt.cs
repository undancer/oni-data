using STRINGS;

namespace Database
{
	public class MonumentBuilt : VictoryColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public override string Name()
		{
			return COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.BUILT_MONUMENT;
		}

		public override string Description()
		{
			return COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.BUILT_MONUMENT_DESCRIPTION;
		}

		public override bool Success()
		{
			foreach (MonumentPart monumentPart in Components.MonumentParts)
			{
				if (monumentPart.IsMonumentCompleted())
				{
					Game.Instance.unlocks.Unlock("thriving");
					return true;
				}
			}
			return false;
		}

		public void Deserialize(IReader reader)
		{
		}

		public override string GetProgress(bool complete)
		{
			return Name();
		}
	}
}
