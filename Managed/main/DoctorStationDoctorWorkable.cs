using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/DoctorStationDoctorWorkable")]
public class DoctorStationDoctorWorkable : Workable
{
	[MyCmpReq]
	private DoctorStation station;

	private DoctorStationDoctorWorkable()
	{
		synchronizeAnims = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		attributeConverter = Db.Get().AttributeConverters.DoctorSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.MedicalAid.Id;
		skillExperienceMultiplier = SKILLS.BARELY_EVER_EXPERIENCE;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		station.SetHasDoctor(has: true);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		station.SetHasDoctor(has: false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		station.CompleteDoctoring();
	}
}
