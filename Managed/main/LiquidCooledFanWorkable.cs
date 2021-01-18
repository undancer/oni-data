using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/LiquidCooledFanWorkable")]
public class LiquidCooledFanWorkable : Workable
{
	[MyCmpGet]
	private Operational operational;

	private LiquidCooledFanWorkable()
	{
		showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = null;
	}

	protected override void OnSpawn()
	{
		GameScheduler.Instance.Schedule("InsulationTutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Insulation);
		});
		base.OnSpawn();
	}

	protected override void OnStartWork(Worker worker)
	{
		operational.SetActive(value: true);
	}

	protected override void OnStopWork(Worker worker)
	{
		operational.SetActive(value: false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		operational.SetActive(value: false);
	}
}
