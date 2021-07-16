using KSerialization;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ToiletWorkableClean")]
public class ToiletWorkableClean : Workable
{
	[Serialize]
	public int timesCleaned;

	private static readonly HashedString[] CLEAN_ANIMS = new HashedString[2]
	{
		"unclog_pre",
		"unclog_loop"
	};

	private static readonly HashedString PST_ANIM = new HashedString("unclog_pst");

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
		workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
		attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		workAnims = CLEAN_ANIMS;
		workingPstComplete = new HashedString[1]
		{
			PST_ANIM
		};
		workingPstFailed = new HashedString[1]
		{
			PST_ANIM
		};
	}

	protected override void OnCompleteWork(Worker worker)
	{
		timesCleaned++;
		base.OnCompleteWork(worker);
	}
}
