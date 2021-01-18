using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/BeachChairWorkable")]
public class BeachChairWorkable : Workable, IWorkerPrioritizable
{
	[MyCmpReq]
	private Operational operational;

	private float timeLit;

	public string soundPath = GlobalAssets.GetSound("BeachChair_music_lp");

	public HashedString BEACH_CHAIR_LIT_PARAMETER = "beachChair_lit";

	public int basePriority;

	private BeachChair beachChair;

	private BeachChairWorkable()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_beach_chair_kanim")
		};
		workAnims = null;
		workingPstComplete = null;
		workingPstFailed = null;
		showProgressBar = true;
		resetProgressOnStop = true;
		synchronizeAnims = false;
		lightEfficiencyBonus = false;
		SetWorkTime(150f);
		beachChair = GetComponent<BeachChair>();
	}

	protected override void OnStartWork(Worker worker)
	{
		timeLit = 0f;
		beachChair.SetWorker(worker);
		operational.SetActive(value: true);
		worker.GetComponent<Effects>().Add("BeachChairRelaxing", should_save: false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		int i = Grid.PosToCell(base.gameObject);
		int num = Grid.LightIntensity[i];
		bool flag = (float)num >= 9999f;
		beachChair.SetLit(flag);
		if (flag)
		{
			GetComponent<LoopingSounds>().SetParameter(soundPath, BEACH_CHAIR_LIT_PARAMETER, 1f);
			timeLit += dt;
		}
		else
		{
			GetComponent<LoopingSounds>().SetParameter(soundPath, BEACH_CHAIR_LIT_PARAMETER, 0f);
		}
		return false;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		float num = timeLit / workTime;
		if (num >= 0.75f)
		{
			component.Add(beachChair.specificEffectLit, should_save: true);
			component.Remove(beachChair.specificEffectUnlit);
		}
		else
		{
			component.Add(beachChair.specificEffectUnlit, should_save: true);
			component.Remove(beachChair.specificEffectLit);
		}
		component.Add(beachChair.trackingEffect, should_save: true);
	}

	protected override void OnStopWork(Worker worker)
	{
		operational.SetActive(value: false);
		worker.GetComponent<Effects>().Remove("BeachChairRelaxing");
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (component.HasEffect(beachChair.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (component.HasEffect(beachChair.specificEffectLit) || component.HasEffect(beachChair.specificEffectUnlit))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}
}
