using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Database;

public class ColonyAchievementStatus
{
	public bool success;

	public bool failed;

	private ColonyAchievement m_achievement;

	public List<ColonyAchievementRequirement> Requirements => m_achievement.requirementChecklist;

	public ColonyAchievementStatus(string achievementId)
	{
		m_achievement = Db.Get().ColonyAchievements.TryGet(achievementId);
	}

	public void UpdateAchievement()
	{
		if (Requirements.Count <= 0 || m_achievement.Disabled)
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
				Type type = Type.GetType(reader.ReadKleiString());
				if (type != null)
				{
					AchievementRequirementSerialization_Deprecated obj = FormatterServices.GetUninitializedObject(type) as AchievementRequirementSerialization_Deprecated;
					Debug.Assert(obj != null, $"Cannot deserialize old data for type {type}");
					obj.Deserialize(reader);
				}
			}
		}
		return new ColonyAchievementStatus(achievementId)
		{
			success = flag,
			failed = flag2
		};
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write((byte)(success ? 1u : 0u));
		writer.Write((byte)(failed ? 1u : 0u));
	}
}
