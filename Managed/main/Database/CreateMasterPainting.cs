using System.IO;
using STRINGS;

namespace Database
{
	public class CreateMasterPainting : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			foreach (Painting item in Components.Paintings.Items)
			{
				if (item != null && item.CurrentStatus == Artable.Status.Great)
				{
					return true;
				}
			}
			return false;
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CREATE_A_PAINTING;
		}
	}
}
