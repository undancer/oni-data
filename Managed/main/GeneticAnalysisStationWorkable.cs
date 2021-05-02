using TUNING;
using UnityEngine;

public class GeneticAnalysisStationWorkable : Workable
{
	[MyCmpAdd]
	public Notifier notifier;

	[MyCmpReq]
	public Storage storage;

	private Notification notification;

	public GeneticAnalysisStation.StatesInstance statesInstance;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		requiredSkillPerk = Db.Get().SkillPerks.AllowNuclearResearch.Id;
		workerStatusItem = Db.Get().DuplicantStatusItems.AnalyzingGenes;
		attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_genetic_analysisstation_kanim")
		};
		SetWorkTime(150f);
		showProgressBar = true;
		lightEfficiencyBonus = true;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return !statesInstance.GetTargetPlant().IsValid;
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		IdentifyMutant();
	}

	public void IdentifyMutant()
	{
		Tag targetPlant = statesInstance.GetTargetPlant();
		if (!targetPlant.IsValid)
		{
			return;
		}
		Tag targetAsSeed = statesInstance.GetTargetAsSeed();
		GameObject x = storage.FindFirst(targetAsSeed);
		DebugUtil.DevAssertArgs(x != null, "AAACCCCKKK!! GeneticAnalysisStation finished studying a ", targetAsSeed, " but we don't have one in storage??");
		if (x != null)
		{
			if (PlantSubSpeciesCatalog.instance.PartiallyIdentifySpecies(targetPlant, 0.05f))
			{
				statesInstance.SetTargetPlant(Tag.Invalid);
			}
			storage.ConsumeIgnoringDisease(targetAsSeed, 1f);
		}
	}
}
