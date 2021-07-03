using STRINGS;

namespace Database
{
	public class EstablishColonies : VictoryColonyAchievementRequirement
	{
		private const int BASE_COUNT = 5;

		public override string GetProgress(bool complete)
		{
			string text = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ESTABLISH_COLONIES;
			text = text.Replace("{goalBaseCount}", 5.ToString());
			text = text.Replace("{baseCount}", GetColonyCount().ToString());
			return text.Replace("{neededCount}", 5.ToString());
		}

		public override string Description()
		{
			return GetProgress(Success());
		}

		public override bool Success()
		{
			return GetColonyCount() >= 5;
		}

		public override string Name()
		{
			return COLONY_ACHIEVEMENTS.CONQUER_CLUSTER.REQUIREMENTS.SEVERAL_COLONIES;
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
