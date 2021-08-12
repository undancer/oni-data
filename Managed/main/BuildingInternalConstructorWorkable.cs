using TUNING;

public class BuildingInternalConstructorWorkable : Workable
{
	private BuildingInternalConstructor.Instance constructorInstance;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		minimumAttributeMultiplier = 0.75f;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
		skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		resetProgressOnStop = false;
		multitoolContext = "build";
		multitoolHitEffectTag = EffectConfigs.BuildSplashId;
		workingPstComplete = null;
		workingPstFailed = null;
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		constructorInstance = this.GetSMI<BuildingInternalConstructor.Instance>();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		constructorInstance.ConstructionComplete();
	}
}
