using UnityEngine;
using UnityEngine.UI;

public class NotificationAnimator : MonoBehaviour
{
	private const float START_SPEED = 1f;

	private const float ACCELERATION = 0.5f;

	private const float BOUNCE_DAMPEN = 2f;

	private const int BOUNCE_COUNT = 2;

	private const float OFFSETX = 100f;

	private float speed = 1f;

	private int bounceCount = 2;

	private LayoutElement layoutElement;

	[SerializeField]
	private bool animating = true;

	public void Begin(bool startOffset = true)
	{
		Reset();
		animating = true;
		if (startOffset)
		{
			layoutElement.minWidth = 100f;
			return;
		}
		layoutElement.minWidth = 1f;
		speed = -10f;
	}

	private void Reset()
	{
		bounceCount = 2;
		layoutElement = GetComponent<LayoutElement>();
		layoutElement.minWidth = 0f;
		speed = 1f;
	}

	public void Stop()
	{
		Reset();
		animating = false;
	}

	private void LateUpdate()
	{
		if (!animating)
		{
			return;
		}
		layoutElement.minWidth -= speed;
		speed += 0.5f;
		if (layoutElement.minWidth <= 0f)
		{
			if (bounceCount > 0)
			{
				bounceCount--;
				speed = (0f - speed) / Mathf.Pow(2f, 2 - bounceCount);
				layoutElement.minWidth = 0f - speed;
			}
			else
			{
				layoutElement.minWidth = 0f;
				Stop();
			}
		}
	}
}
