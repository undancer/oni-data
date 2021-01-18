using System;
using System.Collections;
using UnityEngine;

public class ExpandRevealUIContent : MonoBehaviour
{
	private Coroutine activeRoutine;

	private Action<object> activeRoutineCompleteCallback;

	public AnimationCurve expandAnimation;

	public AnimationCurve collapseAnimation;

	public KRectStretcher MaskRectStretcher;

	public KRectStretcher BGRectStretcher;

	public KChildFitter MaskChildFitter;

	public KChildFitter BGChildFitter;

	public float speedScale = 1f;

	public bool Collapsing;

	public bool Expanding;

	private void OnDisable()
	{
		if ((bool)BGChildFitter)
		{
			BGChildFitter.WidthScale = (BGChildFitter.HeightScale = 0f);
		}
		if ((bool)MaskChildFitter)
		{
			if (MaskChildFitter.fitWidth)
			{
				MaskChildFitter.WidthScale = 0f;
			}
			if (MaskChildFitter.fitHeight)
			{
				MaskChildFitter.HeightScale = 0f;
			}
		}
		if ((bool)BGRectStretcher)
		{
			BGRectStretcher.XStretchFactor = (BGRectStretcher.YStretchFactor = 0f);
			BGRectStretcher.UpdateStretching();
		}
		if ((bool)MaskRectStretcher)
		{
			MaskRectStretcher.XStretchFactor = (MaskRectStretcher.YStretchFactor = 0f);
			MaskRectStretcher.UpdateStretching();
		}
	}

	public void Expand(Action<object> completeCallback)
	{
		if ((bool)MaskChildFitter && (bool)MaskRectStretcher)
		{
			Debug.LogWarning("ExpandRevealUIContent has references to both a MaskChildFitter and a MaskRectStretcher. It should have only one or the other. ChildFitter to match child size, RectStretcher to match parent size.");
		}
		if ((bool)BGChildFitter && (bool)BGRectStretcher)
		{
			Debug.LogWarning("ExpandRevealUIContent has references to both a BGChildFitter and a BGRectStretcher . It should have only one or the other.  ChildFitter to match child size, RectStretcher to match parent size.");
		}
		if (activeRoutine != null)
		{
			StopCoroutine(activeRoutine);
		}
		CollapsedImmediate();
		activeRoutineCompleteCallback = completeCallback;
		activeRoutine = StartCoroutine(expand(null));
	}

	public void Collapse(Action<object> completeCallback)
	{
		if (activeRoutine != null)
		{
			if (activeRoutineCompleteCallback != null)
			{
				activeRoutineCompleteCallback(null);
			}
			StopCoroutine(activeRoutine);
		}
		activeRoutineCompleteCallback = completeCallback;
		if (base.gameObject.activeInHierarchy)
		{
			activeRoutine = StartCoroutine(collapse(completeCallback));
			return;
		}
		activeRoutine = null;
		completeCallback?.Invoke(null);
	}

	private IEnumerator expand(Action<object> completeCallback)
	{
		Collapsing = false;
		Expanding = true;
		float num = 0f;
		Keyframe[] keys = expandAnimation.keys;
		for (int i = 0; i < keys.Length; i++)
		{
			Keyframe keyframe = keys[i];
			if (keyframe.time > num)
			{
				num = keyframe.time;
			}
		}
		float duration = num / speedScale;
		for (float remaining = duration; remaining >= 0f; remaining -= Time.unscaledDeltaTime * speedScale)
		{
			SetStretch(expandAnimation.Evaluate(duration - remaining));
			yield return null;
		}
		SetStretch(expandAnimation.Evaluate(duration));
		completeCallback?.Invoke(null);
		activeRoutine = null;
		Expanding = false;
	}

	private void SetStretch(float value)
	{
		if ((bool)BGRectStretcher)
		{
			if (BGRectStretcher.StretchX)
			{
				BGRectStretcher.XStretchFactor = value;
			}
			if (BGRectStretcher.StretchY)
			{
				BGRectStretcher.YStretchFactor = value;
			}
		}
		if ((bool)MaskRectStretcher)
		{
			if (MaskRectStretcher.StretchX)
			{
				MaskRectStretcher.XStretchFactor = value;
			}
			if (MaskRectStretcher.StretchY)
			{
				MaskRectStretcher.YStretchFactor = value;
			}
		}
		if ((bool)BGChildFitter)
		{
			if (BGChildFitter.fitWidth)
			{
				BGChildFitter.WidthScale = value;
			}
			if (BGChildFitter.fitHeight)
			{
				BGChildFitter.HeightScale = value;
			}
		}
		if ((bool)MaskChildFitter)
		{
			if (MaskChildFitter.fitWidth)
			{
				MaskChildFitter.WidthScale = value;
			}
			if (MaskChildFitter.fitHeight)
			{
				MaskChildFitter.HeightScale = value;
			}
		}
	}

	private IEnumerator collapse(Action<object> completeCallback)
	{
		Expanding = false;
		Collapsing = true;
		float num = 0f;
		Keyframe[] keys = collapseAnimation.keys;
		for (int i = 0; i < keys.Length; i++)
		{
			Keyframe keyframe = keys[i];
			if (keyframe.time > num)
			{
				num = keyframe.time;
			}
		}
		float duration = num;
		for (float remaining = duration; remaining >= 0f; remaining -= Time.unscaledDeltaTime)
		{
			SetStretch(collapseAnimation.Evaluate(duration - remaining));
			yield return null;
		}
		SetStretch(collapseAnimation.Evaluate(duration));
		completeCallback?.Invoke(null);
		activeRoutine = null;
		Collapsing = false;
		base.gameObject.SetActive(value: false);
	}

	public void CollapsedImmediate()
	{
		float time = collapseAnimation.length;
		SetStretch(collapseAnimation.Evaluate(time));
	}
}
