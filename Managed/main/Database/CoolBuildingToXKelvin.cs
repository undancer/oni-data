using System.IO;
using STRINGS;

namespace Database
{
	public class CoolBuildingToXKelvin : ColonyAchievementRequirement
	{
		private int kelvinToCoolTo;

		public CoolBuildingToXKelvin(int kelvinToCoolTo)
		{
			this.kelvinToCoolTo = kelvinToCoolTo;
		}

		public override bool Success()
		{
			return BuildingComplete.MinKelvinSeen <= (float)kelvinToCoolTo;
		}

		public override void Deserialize(IReader reader)
		{
			kelvinToCoolTo = reader.ReadInt32();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(kelvinToCoolTo);
		}

		public override string GetProgress(bool complete)
		{
			float minKelvinSeen = BuildingComplete.MinKelvinSeen;
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.KELVIN_COOLING, minKelvinSeen);
		}
	}
}
