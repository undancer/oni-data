using KSerialization;
using TUNING;
using UnityEngine;

public class NuclearResearchCenterWorkable : Workable
{
	[MyCmpReq]
	private Operational operational;

	[Serialize]
	private float pointsProduced;

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
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Research, Research.Instance.GetResearchType("nuclear").name, base.transform);
			Research.Instance.AddResearchPoints("nuclear", num2);
		}
		TechInstance activeResearch = Research.Instance.GetActiveResearch();
		if (radiationStorage.IsEmpty() || activeResearch == null)
		{
			return true;
		}
		if (activeResearch.PercentageCompleteResearchType("nuclear") >= 1f)
		{
			return true;
		}
		return false;
	}

	protected override void OnAbortWork(Worker worker)
	{
		base.OnAbortWork(worker);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
	}

	public override float GetPercentComplete()
	{
		if (Research.Instance.GetActiveResearch() == null)
		{
			return 0f;
		}
		float num = Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID["nuclear"];
		float value = 0f;
		if (!Research.Instance.GetActiveResearch().tech.costsByResearchTypeID.TryGetValue("nuclear", out value))
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
