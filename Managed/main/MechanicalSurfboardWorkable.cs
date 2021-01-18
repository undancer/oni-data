using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/MechanicalSurfboardWorkable")]
public class MechanicalSurfboardWorkable : Workable, IWorkerPrioritizable
{
	[MyCmpReq]
	private Operational operational;

	public int basePriority;

	private MechanicalSurfboard surfboard;

	private MechanicalSurfboardWorkable()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		showProgressBar = true;
		resetProgressOnStop = true;
		synchronizeAnims = true;
		SetWorkTime(30f);
		surfboard = GetComponent<MechanicalSurfboard>();
	}

	protected override void OnStartWork(Worker worker)
	{
		operational.SetActive(value: true);
		worker.GetComponent<Effects>().Add("MechanicalSurfing", should_save: false);
	}

	public override AnimInfo GetAnim(Worker worker)
	{
		AnimInfo result = default(AnimInfo);
		AttributeInstance attributeInstance = worker.GetAttributes().Get(Db.Get().Attributes.Athletics);
		if (attributeInstance.GetTotalValue() <= 7f)
		{
			result.overrideAnims = new KAnimFile[1]
			{
				Assets.GetAnim(surfboard.interactAnims[0])
			};
		}
		else if (attributeInstance.GetTotalValue() <= 15f)
		{
			result.overrideAnims = new KAnimFile[1]
			{
				Assets.GetAnim(surfboard.interactAnims[1])
			};
		}
		else
		{
			result.overrideAnims = new KAnimFile[1]
			{
				Assets.GetAnim(surfboard.interactAnims[2])
			};
		}
		return result;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		Building component = GetComponent<Building>();
		MechanicalSurfboard component2 = GetComponent<MechanicalSurfboard>();
		int widthInCells = component.Def.WidthInCells;
		int min = -(widthInCells - 1) / 2;
		int max = widthInCells / 2;
		int x = Random.Range(min, max);
		float num = component2.waterSpillRateKG * dt;
		GetComponent<Storage>().ConsumeAndGetDisease(SimHashes.Water.CreateTag(), num, out var disease_info, out var aggregate_temperature);
		int cell = Grid.OffsetCell(Grid.PosToCell(base.gameObject), new CellOffset(x, 0));
		int elementIndex = ElementLoader.GetElementIndex(SimHashes.Water);
		FallingWater.instance.AddParticle(cell, (byte)elementIndex, num, aggregate_temperature, disease_info.idx, disease_info.count, skip_sound: true);
		return false;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(surfboard.specificEffect))
		{
			component.Add(surfboard.specificEffect, should_save: true);
		}
		if (!string.IsNullOrEmpty(surfboard.trackingEffect))
		{
			component.Add(surfboard.trackingEffect, should_save: true);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		operational.SetActive(value: false);
		worker.GetComponent<Effects>().Remove("MechanicalSurfing");
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(surfboard.trackingEffect) && component.HasEffect(surfboard.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(surfboard.specificEffect) && component.HasEffect(surfboard.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}
}
