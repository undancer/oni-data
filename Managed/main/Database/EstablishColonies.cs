using STRINGS;

namespace Database
{
	public class EstablishColonies : VictoryColonyAchievementRequirement
	{
		public static int BASE_COUNT = 5;

		public override string GetProgress(bool complete)
		{
			return ((string)COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ESTABLISH_COLONIES).Replace("{goalBaseCount}", BASE_COUNT.ToString()).Replace("{baseCount}", GetColonyCount().ToString()).Replace("{neededCount}", BASE_COUNT.ToString());
		}

		public override string Description()
		{
			return GetProgress(Success());
		}

		public override bool Success()
		{
			return GetColonyCount() >= BASE_COUNT;
		}

		public override string Name()
		{
			return COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.REQUIREMENTS.SEVERAL_COLONIES;
		}

		private int GetColonyCount()
		{
			int num = 0;
			for (int i = 0; i < Components.Telepads.Count; i++)
			{
				Activatable component = Components.Telepads[i].GetComponent<Activatable>();
				if (component == null || component.IsActivated)
				{
					num++;
				}
			}
			return num;
		}
	}
}
