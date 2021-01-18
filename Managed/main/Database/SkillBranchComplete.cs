using System.Collections.Generic;
using System.IO;
using KSerialization;
using STRINGS;

namespace Database
{
	public class SkillBranchComplete : ColonyAchievementRequirement
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
					if (item.HasMasteredSkill(item2.Id))
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(skillsToMaster.Count);
			foreach (Skill item in skillsToMaster)
			{
				writer.WriteKleiString(item.Id);
			}
		}

		public override void Deserialize(IReader reader)
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
