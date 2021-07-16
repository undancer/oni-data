using KSerialization;
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

	private KBatchedAnimController animController;

	[Serialize]
	private float nextYeildRoll = -1f;

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
			Assets.GetAnim("anim_interacts_artifact_analysis_kanim")
		};
		SetWorkTime(150f);
		showProgressBar = true;
		lightEfficiencyBonus = true;
		Components.ArtifactAnalysisStations.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		animController = GetComponent<KBatchedAnimController>();
		animController.SetSymbolVisiblity("snapTo_artifact", is_visible: false);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.ArtifactAnalysisStations.Remove(this);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		InitialDisplayStoredArtifact();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		PositionArtifact();
		return base.OnWorkTick(worker, dt);
	}

	private void InitialDisplayStoredArtifact()
	{
		GameObject gameObject = GetComponent<Storage>().GetItems()[0];
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.GetBatchInstanceData().ClearOverrideTransformMatrix();
		}
		gameObject.transform.SetPosition(new Vector3(base.transform.position.x, base.transform.position.y, Grid.GetLayerZ(Grid.SceneLayer.BuildingBack)));
		gameObject.SetActive(value: true);
		component.enabled = false;
		component.enabled = true;
		PositionArtifact();
	}

	private void ReleaseStoredArtifact()
	{
		Storage component = GetComponent<Storage>();
		GameObject gameObject = component.GetItems()[0];
		KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
		gameObject.transform.SetPosition(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, Grid.GetLayerZ(Grid.SceneLayer.Ore)));
		component2.enabled = false;
		component2.enabled = true;
		component.Drop(gameObject);
	}

	private void PositionArtifact()
	{
		GameObject gameObject = GetComponent<Storage>().GetItems()[0];
		bool symbolVisible;
		Vector3 position = animController.GetSymbolTransform("snapTo_artifact", out symbolVisible).GetColumn(3);
		position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingBack);
		gameObject.transform.SetPosition(position);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		ConsumeCharm();
		ReleaseStoredArtifact();
	}

	private void ConsumeCharm()
	{
		GameObject gameObject = storage.FindFirst(GameTags.CharmedArtifact);
		DebugUtil.DevAssertArgs(gameObject != null, "ArtifactAnalysisStation finished studying a charmed artifact but there is not one in its storage");
		if (gameObject != null)
		{
			YieldPayload(gameObject.GetComponent<SpaceArtifact>());
			gameObject.GetComponent<SpaceArtifact>().RemoveCharm();
		}
		if (ArtifactSelector.Instance.RecordArtifactAnalyzed(gameObject.GetComponent<KPrefabID>().PrefabID().ToString()))
		{
			if (gameObject.HasTag(GameTags.TerrestrialArtifact))
			{
				ArtifactSelector.Instance.IncrementAnalyzedTerrestrialArtifacts();
			}
			else
			{
				ArtifactSelector.Instance.IncrementAnalyzedSpaceArtifacts();
			}
		}
	}

	private void YieldPayload(SpaceArtifact artifact)
	{
		if (nextYeildRoll == -1f)
		{
			nextYeildRoll = Random.Range(0f, 1f);
		}
		if (nextYeildRoll <= artifact.GetArtifactTier().payloadDropChance)
		{
			GameUtil.KInstantiate(Assets.GetPrefab("GeneShufflerRecharge"), statesInstance.master.transform.position + finishedArtifactDropOffset, Grid.SceneLayer.Ore).SetActive(value: true);
		}
		nextYeildRoll = Random.Range(0f, 1f);
	}
}
