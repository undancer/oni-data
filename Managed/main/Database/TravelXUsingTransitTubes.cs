using System.IO;
using STRINGS;

namespace Database
{
	public class TravelXUsingTransitTubes : ColonyAchievementRequirement
	{
		private int distanceToTravel;

		private NavType navType;

		public TravelXUsingTransitTubes(NavType navType, int distanceToTravel)
		{
			this.navType = navType;
			this.distanceToTravel = distanceToTravel;
		}

		public override bool Success()
		{
			int num = 0;
			foreach (MinionIdentity item in Components.MinionIdentities.Items)
			{
				Navigator component = item.GetComponent<Navigator>();
				if (component != null && component.distanceTravelledByNavType.ContainsKey(navType))
				{
					num += component.distanceTravelledByNavType[navType];
				}
			}
			return num >= distanceToTravel;
		}

		public override void Deserialize(IReader reader)
		{
			byte b = (byte)(navType = (NavType)reader.ReadByte());
			distanceToTravel = reader.ReadInt32();
		}

		public override void Serialize(BinaryWriter writer)
		{
			byte value = (byte)navType;
			writer.Write(value);
			writer.Write(distanceToTravel);
		}

		public override string GetProgress(bool complete)
		{
			int num = 0;
			foreach (MinionIdentity item in Components.MinionIdentities.Items)
			{
				Navigator component = item.GetComponent<Navigator>();
				if (component != null && component.distanceTravelledByNavType.ContainsKey(navType))
				{
					num += component.distanceTravelledByNavType[navType];
				}
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.TRAVELED_IN_TUBES, complete ? distanceToTravel : num, distanceToTravel);
		}
	}
}
