using KSerialization;
using TUNING;
using UnityEngine;

public class NuclearResearchCenterWorkable : Workable
{
	[MyCmpReq]
	private Operational operational;

	[Serialize]
	private float pointsProduced = 0f;

	private NuclearResearchCenter nrc;

	private HighEnergyParticleStorage radiationStorage;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Researching;
		attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
		skillExperienceMultiplier = SKILLS.ALL_DAY_EXPERIENCE;
		radiationStorage = GetComponent<HighEnergyParticleStorage>();
		nrc = GetComponent<NuclearResearchCenter>();
		lightEfficiencyBonus = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SetWorkTime(float.PositiveInfinity);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		float num = dt / nrc.timePerPoint;
		if (Game.Instance.FastWorkersModeActive)
		{
			num *= 2f;
		}
		radiationStorage.ConsumeAndGet(num * nrc.materialPerPoint);
		pointsProduced += num;
		if (pointsProduced >= 1f)
		{
			int num2 = Mathf.FloorToInt(pointsProduced);
			pointsProduced -= num2;
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Research, Research.Instance.GetResearchType("delta").name, base.transform);
			Research.Instance.AddResearchPoints("delta", num2);
		}
		return false;
	}

	public override float GetPercentComplete()
	{
		if (Research.Instance.GetActiveResearch() == null)
		{
			return 0f;
		}
		float num = Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID["delta"];
		float value = 0f;
		if (!Research.Instance.GetActiveResearch().tech.costsByResearchTypeID.TryGetValue("delta", out value))
		{
			return 1f;
		}
		return num / value;
	}

	public override bool InstantlyFinish(Worker worker)
	{
		return false;
	}
}
