using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Database;
using KSerialization;

public class ColonyAchievementStatus
{
	public bool success;

	public bool failed;

	private List<ColonyAchievementRequirement> requirements = new List<ColonyAchievementRequirement>();

	public List<ColonyAchievementRequirement> Requirements => requirements;

	public void UpdateAchievement()
	{
		if (requirements == null || requirements.Count <= 0)
		{
			return;
		}
		success = true;
		foreach (ColonyAchievementRequirement requirement in requirements)
		{
			requirement.Update();
			success &= requirement.Success();
			failed |= requirement.Fail();
		}
	}

	public void Deserialize(IReader reader)
	{
		success = reader.ReadByte() != 0;
		failed = reader.ReadByte() != 0;
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Type type = Type.GetType(reader.ReadKleiString());
			if (type != null)
			{
				ColonyAchievementRequirement colonyAchievementRequirement = (ColonyAchievementRequirement)FormatterServices.GetUninitializedObject(type);
				colonyAchievementRequirement.Deserialize(reader);
				requirements.Add(colonyAchievementRequirement);
			}
		}
	}

	public void SetRequirements(List<ColonyAchievementRequirement> requirementChecklist)
	{
		requirements = requirementChecklist;
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write((byte)(success ? 1u : 0u));
		writer.Write((byte)(failed ? 1u : 0u));
		writer.Write((requirements != null) ? requirements.Count : 0);
		if (requirements == null)
		{
			return;
		}
		foreach (ColonyAchievementRequirement requirement in requirements)
		{
			writer.WriteKleiString(requirement.GetType().ToString());
			requirement.Serialize(writer);
		}
	}
}
