using TUNING;

public class SplatWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
		workerStatusItem = Db.Get().DuplicantStatusItems.Mopping;
		attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		multitoolContext = "disinfect";
		multitoolHitEffectTag = "fx_disinfect_splash";
		synchronizeAnims = false;
		Prioritizable.AddRef(base.gameObject);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SetWorkTime(5f);
	}
}
