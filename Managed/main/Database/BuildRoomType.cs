using STRINGS;

namespace Database
{
	public class BuildRoomType : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private RoomType roomType;

		public BuildRoomType(RoomType roomType)
		{
			this.roomType = roomType;
		}

		public override bool Success()
		{
			foreach (Room room in Game.Instance.roomProber.rooms)
			{
				if (room.roomType == roomType)
				{
					return true;
				}
			}
			return false;
		}

		public void Deserialize(IReader reader)
		{
			string id = reader.ReadKleiString();
			roomType = Db.Get().RoomTypes.Get(id);
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_A_ROOM, roomType.Name);
		}
	}
}
