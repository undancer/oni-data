using UnityEngine;

public class WidgetTransition : MonoBehaviour
{
	public enum TransitionType
	{
		SlideFromRight,
		SlideFromLeft,
		FadeOnly,
		SlideFromTop
	}

	private const float OFFSETX = 50f;

	private const float SLIDE_SPEED = 7f;

	private const float FADEIN_SPEED = 6f;

	private bool fadingIn;

	private CanvasGroup canvasGroup;

	private CanvasGroup CanvasGroup
	{
		get
		{
			if (!(canvasGroup == null))
			{
				return canvasGroup;
			}
			return canvasGroup = base.gameObject.FindOrAddUnityComponent<CanvasGroup>();
		}
	}

	public void SetTransitionType(TransitionType transitionType)
	{
	}

	public void StartTransition()
	{
		if (!fadingIn)
		{
			CanvasGroup.alpha = 0f;
			fadingIn = true;
			base.enabled = true;
		}
	}

	public void StopTransition()
	{
		if (fadingIn)
		{
			fadingIn = false;
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (fadingIn)
		{
			float alpha = CanvasGroup.alpha;
			alpha += 6f * Time.unscaledDeltaTime;
			if (alpha >= 1f)
			{
				alpha = 1f;
			}
			if (alpha == 1f)
			{
				fadingIn = false;
				base.enabled = false;
			}
			CanvasGroup.alpha = alpha;
		}
	}

	private void OnDisable()
	{
		StopTransition();
	}
}
