using System.Collections;
using UnityEngine;

public class FrontEndBackground : UIDupeRandomizer
{
	public class Tuning : TuningData<Tuning>
	{
		public float minDreckoInterval;

		public float maxDreckoInterval;

		public float minFirstDreckoInterval;

		public float maxFirstDreckoInterval;
	}

	private KBatchedAnimController dreckoController;

	private float nextDreckoTime = 0f;

	private Tuning tuning;

	protected override void Start()
	{
		tuning = TuningData<Tuning>.Get();
		base.Start();
		for (int i = 0; i < anims.Length; i++)
		{
			int minionIndex = i;
			KBatchedAnimController kBatchedAnimController = anims[i].minions[0];
			if (kBatchedAnimController.gameObject.activeInHierarchy)
			{
				kBatchedAnimController.onAnimComplete += delegate(HashedString name)
				{
					WaitForABit(minionIndex, name);
				};
				WaitForABit(i, HashedString.Invalid);
			}
		}
		dreckoController = base.transform.GetChild(0).Find("startmenu_drecko").GetComponent<KBatchedAnimController>();
		if (dreckoController.gameObject.activeInHierarchy)
		{
			dreckoController.enabled = false;
			nextDreckoTime = Random.Range(tuning.minFirstDreckoInterval, tuning.maxFirstDreckoInterval) + Time.unscaledTime;
		}
	}

	protected override void Update()
	{
		base.Update();
		UpdateDrecko();
	}

	private void UpdateDrecko()
	{
		if (dreckoController.gameObject.activeInHierarchy && Time.unscaledTime > nextDreckoTime)
		{
			dreckoController.enabled = true;
			dreckoController.Play("idle");
			nextDreckoTime = Random.Range(tuning.minDreckoInterval, tuning.maxDreckoInterval) + Time.unscaledTime;
		}
	}

	private void WaitForABit(int minion_idx, HashedString name)
	{
		StartCoroutine(WaitForTime(minion_idx));
	}

	private IEnumerator WaitForTime(int minion_idx)
	{
		anims[minion_idx].lastWaitTime = Random.Range(anims[minion_idx].minSecondsBetweenAction, anims[minion_idx].maxSecondsBetweenAction);
		yield return new WaitForSecondsRealtime(anims[minion_idx].lastWaitTime);
		GetNewBody(minion_idx);
		foreach (KBatchedAnimController kbac in anims[minion_idx].minions)
		{
			kbac.ClearQueue();
			kbac.Play(anims[minion_idx].anim_name);
		}
	}
}
