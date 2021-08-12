using STRINGS;

namespace Database
{
	public class NumberOfDupes : VictoryColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private int numDupes;

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_DUPLICANTS, numDupes);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_DUPLICANTS_DESCRIPTION, numDupes);
		}

		public NumberOfDupes(int num)
		{
			numDupes = num;
		}

		public override bool Success()
		{
			return Components.LiveMinionIdentities.Items.Count >= numDupes;
		}

		public void Deserialize(IReader reader)
		{
			numDupes = reader.ReadInt32();
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.POPULATION, complete ? numDupes : Components.LiveMinionIdentities.Items.Count, numDupes);
		}
	}
}
