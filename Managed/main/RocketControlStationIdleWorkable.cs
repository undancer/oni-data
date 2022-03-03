using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/RocketControlStationIdleWorkable")]
public class RocketControlStationIdleWorkable : Workable
{
	[MyCmpReq]
	private Operational operational;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_rocket_control_station_kanim") };
		showProgressBar = true;
		resetProgressOnStop = true;
		synchronizeAnims = true;
		attributeConverter = Db.Get().AttributeConverters.PilotingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Rocketry.Id;
		skillExperienceMultiplier = SKILLS.BARELY_EVER_EXPERIENCE;
		SetWorkTime(30f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.GetSMI<RocketControlStation.StatesInstance>()?.SetPilotSpeedMult(worker);
	}
}
