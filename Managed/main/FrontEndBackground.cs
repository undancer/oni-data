using System;
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

	private float nextDreckoTime;

	private Tuning tuning;

	[NonSerialized]
	public Camera baseCamera;

	protected override void Start()
	{
		tuning = TuningData<Tuning>.Get();
		SetupCameras();
		base.Start();
		for (int i = 0; i < anims.Length; i++)
		{
			int minionIndex = i;
			anims[i].minions[0].onAnimComplete += delegate(HashedString name)
			{
				WaitForABit(minionIndex, name);
			};
			WaitForABit(i, HashedString.Invalid);
		}
		dreckoController = base.transform.GetChild(0).Find("startmenu_drecko").GetComponent<KBatchedAnimController>();
		dreckoController.enabled = false;
		nextDreckoTime = UnityEngine.Random.Range(tuning.minFirstDreckoInterval, tuning.maxFirstDreckoInterval) + Time.unscaledTime;
	}

	protected override void Update()
	{
		base.Update();
		UpdateDrecko();
	}

	private void UpdateDrecko()
	{
		if (Time.unscaledTime > nextDreckoTime)
		{
			dreckoController.enabled = true;
			dreckoController.Play("idle");
			nextDreckoTime = UnityEngine.Random.Range(tuning.minDreckoInterval, tuning.maxDreckoInterval) + Time.unscaledTime;
		}
	}

	private void WaitForABit(int minion_idx, HashedString name)
	{
		StartCoroutine(WaitForTime(minion_idx));
	}

	private IEnumerator WaitForTime(int minion_idx)
	{
		anims[minion_idx].lastWaitTime = UnityEngine.Random.Range(anims[minion_idx].minSecondsBetweenAction, anims[minion_idx].maxSecondsBetweenAction);
		yield return new WaitForSecondsRealtime(anims[minion_idx].lastWaitTime);
		GetNewBody(minion_idx);
		foreach (KBatchedAnimController minion in anims[minion_idx].minions)
		{
			minion.ClearQueue();
			minion.Play(anims[minion_idx].anim_name);
		}
	}

	private void SetupCameras()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "Cameras";
		gameObject.transform.parent = base.transform.parent;
		Util.Reset(gameObject.transform);
		baseCamera = GetComponentInChildren<Camera>();
		baseCamera.name = "BaseCamera";
		baseCamera.transform.SetParent(gameObject.transform);
		baseCamera.transparencySortMode = TransparencySortMode.Orthographic;
		baseCamera.tag = "Untagged";
	}
}
