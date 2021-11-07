using System;
using System.Collections.Generic;

namespace Database
{
	public class DupesVsSolidTransferArmFetch : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public float percentage;

		public int numCycles;

		public int currentCycleCount;

		public bool armsOutPerformingDupesThisCycle;

		public DupesVsSolidTransferArmFetch(float percentage, int numCycles)
		{
			this.percentage = percentage;
			this.numCycles = numCycles;
		}

		public override bool Success()
		{
			Dictionary<int, int> fetchDupeChoreDeliveries = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().fetchDupeChoreDeliveries;
			Dictionary<int, int> fetchAutomatedChoreDeliveries = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().fetchAutomatedChoreDeliveries;
			int num = 0;
			currentCycleCount = 0;
			for (int num2 = GameClock.Instance.GetCycle() - 1; num2 >= GameClock.Instance.GetCycle() - numCycles; num2--)
			{
				if (fetchAutomatedChoreDeliveries.ContainsKey(num2))
				{
					if (fetchDupeChoreDeliveries.ContainsKey(num2) && !((float)fetchDupeChoreDeliveries[num2] < (float)fetchAutomatedChoreDeliveries[num2] * percentage))
					{
						break;
					}
					num++;
				}
				else if (fetchDupeChoreDeliveries.ContainsKey(num2))
				{
					num = 0;
					break;
				}
			}
			currentCycleCount = Math.Max(currentCycleCount, num);
			return num >= numCycles;
		}

		public void Deserialize(IReader reader)
		{
			numCycles = reader.ReadInt32();
			percentage = reader.ReadSingle();
		}
	}
}
