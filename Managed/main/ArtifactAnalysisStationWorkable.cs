using TUNING;
using UnityEngine;

public class ArtifactAnalysisStationWorkable : Workable
{
	[MyCmpAdd]
	public Notifier notifier;

	[MyCmpReq]
	public Storage storage;

	[SerializeField]
	public Vector3 finishedArtifactDropOffset;

	private Notification notification;

	public ArtifactAnalysisStation.StatesInstance statesInstance;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		requiredSkillPerk = Db.Get().SkillPerks.CanStudyArtifact.Id;
		workerStatusItem = Db.Get().DuplicantStatusItems.AnalyzingArtifact;
		attributeConverter = Db.Get().AttributeConverters.ArtSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_rockrefinery_kanim")
		};
		SetWorkTime(150f);
		showProgressBar = true;
		lightEfficiencyBonus = true;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		ConsumeCharm();
	}

	private void ConsumeCharm()
	{
		GameObject gameObject = storage.FindFirst(GameTags.CharmedArtifact);
		DebugUtil.DevAssertArgs(gameObject != null, "ArtifactAnalysisStation finished studying a charmed artifact but there is not one in its storage");
		if (gameObject != null)
		{
			YieldPayload();
			gameObject.GetComponent<SpaceArtifact>().RemoveCharm();
			storage.Drop(gameObject);
		}
	}

	private void YieldPayload()
	{
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("GeneShufflerRecharge"), statesInstance.master.transform.position + finishedArtifactDropOffset, Grid.SceneLayer.Ore);
		gameObject.SetActive(value: true);
	}
}
