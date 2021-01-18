using System.IO;
using STRINGS;

namespace Database
{
	public class ActivateLorePOI : ColonyAchievementRequirement
	{
		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override bool Success()
		{
			foreach (BuildingComplete item in Components.TemplateBuildings.Items)
			{
				if (!(item == null))
				{
					Unsealable component = item.GetComponent<Unsealable>();
					if (component != null && component.unsealed)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.INVESTIGATE_A_POI;
		}
	}
}
