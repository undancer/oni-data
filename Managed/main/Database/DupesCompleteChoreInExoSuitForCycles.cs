using System;
using System.Collections.Generic;
using System.Linq;

namespace Database
{
	public class DupesCompleteChoreInExoSuitForCycles : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public int currentCycleStreak;

		public int numCycles;

		public DupesCompleteChoreInExoSuitForCycles(int numCycles)
		{
			this.numCycles = numCycles;
		}

		public override bool Success()
		{
			Dictionary<int, List<int>> dupesCompleteChoresInSuits = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().dupesCompleteChoresInSuits;
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			foreach (MinionIdentity item in Components.MinionIdentities.Items)
			{
				dictionary.Add(item.GetComponent<KPrefabID>().InstanceID, item.arrivalTime);
			}
			int num = 0;
			int num2 = Math.Min(dupesCompleteChoresInSuits.Count, numCycles);
			for (int i = GameClock.Instance.GetCycle() - num2; i <= GameClock.Instance.GetCycle(); i++)
			{
				if (dupesCompleteChoresInSuits.ContainsKey(i))
				{
					List<int> list = dictionary.Keys.Except(dupesCompleteChoresInSuits[i]).ToList();
					bool flag = true;
					foreach (int item2 in list)
					{
						if (dictionary[item2] < (float)i)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						num++;
					}
					else if (i != GameClock.Instance.GetCycle())
					{
						num = 0;
					}
					currentCycleStreak = num;
					if (num >= numCycles)
					{
						currentCycleStreak = numCycles;
						return true;
					}
				}
				else
				{
					currentCycleStreak = Math.Max(currentCycleStreak, num);
					num = 0;
				}
			}
			return false;
		}

		public void Deserialize(IReader reader)
		{
			numCycles = reader.ReadInt32();
		}

		public int GetNumberOfDupesForCycle(int cycle)
		{
			int result = 0;
			Dictionary<int, List<int>> dupesCompleteChoresInSuits = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().dupesCompleteChoresInSuits;
			if (dupesCompleteChoresInSuits.ContainsKey(GameClock.Instance.GetCycle()))
			{
				result = dupesCompleteChoresInSuits[GameClock.Instance.GetCycle()].Count;
			}
			return result;
		}
	}
}
