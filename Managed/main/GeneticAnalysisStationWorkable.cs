using TUNING;
using UnityEngine;

public class GeneticAnalysisStationWorkable : Workable
{
	[MyCmpAdd]
	public Notifier notifier;

	[MyCmpReq]
	public Storage storage;

	[SerializeField]
	public Vector3 finishedSeedDropOffset;

	private Notification notification;

	public GeneticAnalysisStation.StatesInstance statesInstance;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		requiredSkillPerk = Db.Get().SkillPerks.CanIdentifyMutantSeeds.Id;
		workerStatusItem = Db.Get().DuplicantStatusItems.AnalyzingGenes;
		attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_genetic_analysisstation_kanim") };
		SetWorkTime(150f);
		showProgressBar = true;
		lightEfficiencyBonus = true;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		IdentifyMutant();
	}

	public void IdentifyMutant()
	{
		GameObject gameObject = storage.FindFirst(GameTags.UnidentifiedSeed);
		DebugUtil.DevAssertArgs(gameObject != null, "AAACCCCKKK!! GeneticAnalysisStation finished studying a seed but we don't have one in storage??");
		if (gameObject != null)
		{
			Pickupable component = gameObject.GetComponent<Pickupable>();
			Pickupable pickupable = ((!(component.PrimaryElement.Units > 1f)) ? storage.Drop(gameObject).GetComponent<Pickupable>() : component.Take(1f));
			pickupable.transform.SetPosition(base.transform.GetPosition() + finishedSeedDropOffset);
			MutantPlant component2 = pickupable.GetComponent<MutantPlant>();
			PlantSubSpeciesCatalog.Instance.IdentifySubSpecies(component2.SubSpeciesID);
			component2.Analyze();
		}
	}
}
