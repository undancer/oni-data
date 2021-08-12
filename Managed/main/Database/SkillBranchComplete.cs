using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class SkillBranchComplete : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private List<Skill> skillsToMaster;

		public SkillBranchComplete(List<Skill> skillsToMaster)
		{
			this.skillsToMaster = skillsToMaster;
		}

		public override bool Success()
		{
			foreach (MinionResume item in Components.MinionResumes.Items)
			{
				foreach (Skill item2 in skillsToMaster)
				{
					if (!item.HasMasteredSkill(item2.Id))
					{
						continue;
					}
					if (item.HasBeenGrantedSkill(item2))
					{
						List<Skill> allPriorSkills = Db.Get().Skills.GetAllPriorSkills(item2);
						bool flag = true;
						foreach (Skill item3 in allPriorSkills)
						{
							flag = flag && item.HasMasteredSkill(item3.Id);
						}
						if (flag)
						{
							return true;
						}
						continue;
					}
					return true;
				}
			}
			return false;
		}

		public void Deserialize(IReader reader)
		{
			skillsToMaster = new List<Skill>();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string id = reader.ReadKleiString();
				skillsToMaster.Add(Db.Get().Skills.Get(id));
			}
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.SKILL_BRANCH;
		}
	}
}
