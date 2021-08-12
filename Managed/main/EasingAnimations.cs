using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasingAnimations : MonoBehaviour
{
	[Serializable]
	public struct AnimationScales
	{
		public enum AnimationType
		{
			EaseInOutBack,
			EaseOutBack,
			EaseInBack
		}

		public string name;

		public float startScale;

		public float endScale;

		public AnimationType type;

		public float easingMultiplier;

		[HideInInspector]
		public float currentScale;
	}

	public AnimationScales[] scales;

	private AnimationScales currentAnimation;

	private Coroutine animationCoroutine;

	private Dictionary<string, AnimationScales> animationMap;

	public Action<string> OnAnimationDone;

	private void Start()
	{
		if (animationMap == null || animationMap.Count == 0)
		{
			Initialize();
		}
	}

	private void Initialize()
	{
		animationMap = new Dictionary<string, AnimationScales>();
		AnimationScales[] array = scales;
		for (int i = 0; i < array.Length; i++)
		{
			AnimationScales value = array[i];
			animationMap.Add(value.name, value);
		}
	}

	public void PlayAnimation(string animationName, float delay = 0f)
	{
		if (animationMap == null || animationMap.Count == 0)
		{
			Initialize();
		}
		if (animationMap.ContainsKey(animationName))
		{
			if (animationCoroutine != null)
			{
				StopCoroutine(animationCoroutine);
			}
			currentAnimation = animationMap[animationName];
			currentAnimation.currentScale = currentAnimation.startScale;
			base.transform.localScale = Vector3.one * currentAnimation.currentScale;
			animationCoroutine = StartCoroutine(ExecuteAnimation(delay));
		}
	}

	private IEnumerator ExecuteAnimation(float delay)
	{
		float startTime2 = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < startTime2 + delay)
		{
			yield return null;
		}
		startTime2 = Time.realtimeSinceStartup;
		bool keepAnimating = true;
		while (keepAnimating)
		{
			float num = Time.realtimeSinceStartup - startTime2;
			currentAnimation.currentScale = GetEasing(num * currentAnimation.easingMultiplier);
			keepAnimating = ((!(currentAnimation.endScale > currentAnimation.startScale)) ? (currentAnimation.currentScale > currentAnimation.endScale + 0.025f) : (currentAnimation.currentScale < currentAnimation.endScale - 0.025f));
			if (!keepAnimating)
			{
				currentAnimation.currentScale = currentAnimation.endScale;
			}
			base.transform.localScale = Vector3.one * currentAnimation.currentScale;
			yield return new WaitForEndOfFrame();
		}
		if (OnAnimationDone != null)
		{
			OnAnimationDone(currentAnimation.name);
		}
	}

	private float GetEasing(float t)
	{
		return currentAnimation.type switch
		{
			AnimationScales.AnimationType.EaseInBack => EaseInBack(currentAnimation.currentScale, currentAnimation.endScale, t), 
			AnimationScales.AnimationType.EaseOutBack => EaseOutBack(currentAnimation.currentScale, currentAnimation.endScale, t), 
			_ => EaseInOutBack(currentAnimation.currentScale, currentAnimation.endScale, t), 
		};
	}

	public float EaseInOutBack(float start, float end, float value)
	{
		float num = 1.70158f;
		end -= start;
		value /= 0.5f;
		if (value < 1f)
		{
			num *= 1.525f;
			return end * 0.5f * (value * value * ((num + 1f) * value - num)) + start;
		}
		value -= 2f;
		num *= 1.525f;
		return end * 0.5f * (value * value * ((num + 1f) * value + num) + 2f) + start;
	}

	public float EaseInBack(float start, float end, float value)
	{
		end -= start;
		value /= 1f;
		float num = 1.70158f;
		return end * value * value * ((num + 1f) * value - num) + start;
	}

	public float EaseOutBack(float start, float end, float value)
	{
		float num = 1.70158f;
		end -= start;
		value -= 1f;
		return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
	}
}
