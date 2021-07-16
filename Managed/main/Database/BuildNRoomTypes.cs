using STRINGS;

namespace Database
{
	public class BuildNRoomTypes : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private RoomType roomType;

		private int numToCreate;

		public BuildNRoomTypes(RoomType roomType, int numToCreate = 1)
		{
			this.roomType = roomType;
			this.numToCreate = numToCreate;
		}

		public override bool Success()
		{
			int num = 0;
			foreach (Room room in Game.Instance.roomProber.rooms)
			{
				if (room.roomType == roomType)
				{
					num++;
				}
			}
			return num >= numToCreate;
		}

		public void Deserialize(IReader reader)
		{
			string id = reader.ReadKleiString();
			roomType = Db.Get().RoomTypes.Get(id);
			numToCreate = reader.ReadInt32();
		}

		public override string GetProgress(bool complete)
		{
			int num = 0;
			foreach (Room room in Game.Instance.roomProber.rooms)
			{
				if (room.roomType == roomType)
				{
					num++;
				}
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_N_ROOMS, roomType.Name, complete ? numToCreate : num, numToCreate);
		}
	}
}
