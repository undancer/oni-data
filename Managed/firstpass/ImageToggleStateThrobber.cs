using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Plugins/ImageToggleStateThrobber")]
public class ImageToggleStateThrobber : KMonoBehaviour
{
	public ImageToggleState[] targetImageToggleStates;

	public ImageToggleState.State state1;

	public ImageToggleState.State state2;

	public float period = 2f;

	public bool useScaledTime = false;

	private float t = 0f;

	public void OnEnable()
	{
		t = 0f;
	}

	public void OnDisable()
	{
		ImageToggleState[] array = targetImageToggleStates;
		foreach (ImageToggleState imageToggleState in array)
		{
			imageToggleState.ResetColor();
		}
	}

	public void Update()
	{
		float num = (useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime);
		t = (t + num) % period;
		float num2 = Mathf.Cos(t / period * 2f * (float)Math.PI) * 0.5f + 0.5f;
		ImageToggleState[] array = targetImageToggleStates;
		foreach (ImageToggleState imageToggleState in array)
		{
			Color a = ColorForState(imageToggleState, state1);
			Color b = ColorForState(imageToggleState, state2);
			Color color = Color.Lerp(a, b, num2);
			imageToggleState.TargetImage.color = color;
		}
	}

	private Color ColorForState(ImageToggleState its, ImageToggleState.State state)
	{
		return state switch
		{
			ImageToggleState.State.Inactive => its.InactiveColour, 
			ImageToggleState.State.Disabled => its.DisabledColour, 
			ImageToggleState.State.DisabledActive => its.DisabledActiveColour, 
			_ => its.ActiveColour, 
		};
	}
}
