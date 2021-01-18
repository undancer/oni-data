using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Database;

public class ColonyAchievementStatus
{
	public bool success = false;

	public bool failed = false;

	private ColonyAchievement m_achievement;

	public List<ColonyAchievementRequirement> Requirements => m_achievement.requirementChecklist;

	public ColonyAchievementStatus(string achievementId)
	{
		m_achievement = Db.Get().ColonyAchievements.Get(achievementId);
	}

	public void UpdateAchievement()
	{
		if (Requirements.Count <= 0)
		{
			return;
		}
		success = true;
		foreach (ColonyAchievementRequirement requirement in Requirements)
		{
			success &= requirement.Success();
			failed |= requirement.Fail();
		}
	}

	public static ColonyAchievementStatus Deserialize(IReader reader, string achievementId)
	{
		bool flag = reader.ReadByte() != 0;
		bool flag2 = reader.ReadByte() != 0;
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 22))
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string typeName = reader.ReadKleiString();
				Type type = Type.GetType(typeName);
				if (type != null)
				{
					AchievementRequirementSerialization_Deprecated achievementRequirementSerialization_Deprecated = FormatterServices.GetUninitializedObject(type) as AchievementRequirementSerialization_Deprecated;
					Debug.Assert(achievementRequirementSerialization_Deprecated != null, $"Cannot deserialize old data for type {type}");
					achievementRequirementSerialization_Deprecated.Deserialize(reader);
				}
			}
		}
		ColonyAchievementStatus colonyAchievementStatus = new ColonyAchievementStatus(achievementId);
		colonyAchievementStatus.success = flag;
		colonyAchievementStatus.failed = flag2;
		return colonyAchievementStatus;
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write((byte)(success ? 1u : 0u));
		writer.Write((byte)(failed ? 1u : 0u));
	}
}
