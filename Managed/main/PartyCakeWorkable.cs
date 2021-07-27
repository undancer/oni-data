using TUNING;

public class PartyCakeWorkable : Workable
{
	private static readonly HashedString[] WORK_ANIMS = new HashedString[2] { "salt_pre", "salt_loop" };

	private static readonly HashedString PST_ANIM = new HashedString("salt_pst");

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Cooking;
		alwaysShowProgressBar = true;
		resetProgressOnStop = false;
		attributeConverter = Db.Get().AttributeConverters.CookingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_desalinator_kanim") };
		workAnims = WORK_ANIMS;
		workingPstComplete = new HashedString[1] { PST_ANIM };
		workingPstFailed = new HashedString[1] { PST_ANIM };
		synchronizeAnims = false;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		base.OnWorkTick(worker, dt);
		GetComponent<KBatchedAnimController>().SetPositionPercent(GetPercentComplete());
		return false;
	}
}
