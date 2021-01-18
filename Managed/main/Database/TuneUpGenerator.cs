using System;
using System.IO;
using STRINGS;

namespace Database
{
	public class TuneUpGenerator : ColonyAchievementRequirement
	{
		private float numChoreseToComplete;

		private float choresCompleted;

		public TuneUpGenerator(float numChoreseToComplete)
		{
			this.numChoreseToComplete = numChoreseToComplete;
		}

		public override bool Success()
		{
			float num = 0f;
			ReportManager.ReportEntry entry = ReportManager.Instance.TodaysReport.GetEntry(ReportManager.ReportType.ChoreStatus);
			for (int i = 0; i < entry.contextEntries.Count; i++)
			{
				ReportManager.ReportEntry reportEntry = entry.contextEntries[i];
				if (reportEntry.context == Db.Get().ChoreTypes.PowerTinker.Name)
				{
					num += reportEntry.Negative;
				}
			}
			string name = Db.Get().ChoreTypes.PowerTinker.Name;
			int count = ReportManager.Instance.reports.Count;
			for (int j = 0; j < count; j++)
			{
				ReportManager.ReportEntry entry2 = ReportManager.Instance.reports[j].GetEntry(ReportManager.ReportType.ChoreStatus);
				int count2 = entry2.contextEntries.Count;
				for (int k = 0; k < count2; k++)
				{
					ReportManager.ReportEntry reportEntry2 = entry2.contextEntries[k];
					if (reportEntry2.context == name)
					{
						num += reportEntry2.Negative;
					}
				}
			}
			choresCompleted = Math.Abs(num);
			return Math.Abs(num) >= numChoreseToComplete;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(numChoreseToComplete);
		}

		public override void Deserialize(IReader reader)
		{
			numChoreseToComplete = reader.ReadSingle();
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CHORES_OF_TYPE, complete ? numChoreseToComplete : choresCompleted, numChoreseToComplete, Db.Get().ChoreTypes.PowerTinker.Name);
		}
	}
}
