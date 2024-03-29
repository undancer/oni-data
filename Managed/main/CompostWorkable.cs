using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/CompostWorkable")]
public class CompostWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
	}

	protected override void OnStartWork(Worker worker)
	{
	}

	protected override void OnStopWork(Worker worker)
	{
	}
}
