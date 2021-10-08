using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/HiveWorkableEmpty")]
public class HiveWorkableEmpty : Workable
{
	private static readonly HashedString[] WORK_ANIMS = new HashedString[2] { "working_pre", "working_loop" };

	private static readonly HashedString PST_ANIM = new HashedString("working_pst");

	public bool wasStung;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
		attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		workAnims = WORK_ANIMS;
		workingPstComplete = new HashedString[1] { PST_ANIM };
		workingPstFailed = new HashedString[1] { PST_ANIM };
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		if (!wasStung)
		{
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().harvestAHiveWithoutGettingStung = true;
		}
	}
}
