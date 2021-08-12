using STRINGS;
using UnityEngine;

namespace Database
{
	public class RevealAsteriod : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private float percentToReveal;

		private float amountRevealed;

		public RevealAsteriod(float percentToReveal)
		{
			this.percentToReveal = percentToReveal;
		}

		public override bool Success()
		{
			amountRevealed = 0f;
			float num = 0f;
			WorldContainer startWorld = ClusterManager.Instance.GetStartWorld();
			Vector2 minimumBounds = startWorld.minimumBounds;
			Vector2 maximumBounds = startWorld.maximumBounds;
			for (int i = (int)minimumBounds.x; (float)i <= maximumBounds.x; i++)
			{
				for (int j = (int)minimumBounds.y; (float)j <= maximumBounds.y; j++)
				{
					if (Grid.Visible[Grid.PosToCell(new Vector2(i, j))] > 0)
					{
						num += 1f;
					}
				}
			}
			amountRevealed = num / (float)(startWorld.Width * startWorld.Height);
			return amountRevealed > percentToReveal;
		}

		public void Deserialize(IReader reader)
		{
			percentToReveal = reader.ReadSingle();
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.REVEALED, amountRevealed * 100f, percentToReveal * 100f);
		}
	}
}
