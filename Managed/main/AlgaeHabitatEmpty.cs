using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/AlgaeHabitatEmpty")]
public class AlgaeHabitatEmpty : Workable
{
	private static readonly HashedString[] CLEAN_ANIMS = new HashedString[2]
	{
		"sponge_pre",
		"sponge_loop"
	};

	private static readonly HashedString PST_ANIM = new HashedString("sponge_pst");

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
		workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
		attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		workAnims = CLEAN_ANIMS;
		workingPstComplete = new HashedString[1]
		{
			PST_ANIM
		};
		workingPstFailed = new HashedString[1]
		{
			PST_ANIM
		};
		synchronizeAnims = false;
	}
}
