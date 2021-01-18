using System.Collections;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
	private bool m_bouncing = false;

	public float durationSecs = 0.3f;

	public Vector3 bounceTarget;

	public int numBounces = 1;

	public void Bounce()
	{
		if (base.gameObject.activeInHierarchy && !m_bouncing)
		{
			StartCoroutine(DoBounce());
		}
	}

	public bool IsBouncing()
	{
		return m_bouncing;
	}

	private IEnumerator DoBounce()
	{
		m_bouncing = true;
		int bouncesCompleted = 0;
		Vector3 startPos = base.gameObject.transform.position;
		yield return new WaitForEndOfFrame();
		for (; bouncesCompleted < numBounces; bouncesCompleted++)
		{
			float iterationTargetFrac = 1f / Mathf.Pow(2f, bouncesCompleted);
			Vector3 iterationTarget = bounceTarget * iterationTargetFrac;
			float iterationDurationFrac = 1f / (float)(bouncesCompleted + 1);
			float iterationDuration = durationSecs * iterationDurationFrac;
			float completion = 0f;
			while (completion < 1f)
			{
				Vector3 position = base.gameObject.transform.position;
				float t = Mathf.Min(Time.unscaledDeltaTime, 0.3f);
				completion = Mathf.Min(completion + t / iterationDuration, 1f);
				Vector3 bounceOffset = BounceSpline(completion) * iterationTarget;
				if (bounceTarget.x != 0f)
				{
					position.x = startPos.x + bounceOffset.x;
				}
				if (bounceTarget.y != 0f)
				{
					position.y = startPos.y + bounceOffset.y;
				}
				base.gameObject.transform.SetPosition(position);
				yield return new WaitForEndOfFrame();
				position = default(Vector3);
			}
		}
		Vector3 finalPosition = base.gameObject.transform.position;
		if (bounceTarget.x != 0f)
		{
			finalPosition.x = startPos.x;
		}
		if (bounceTarget.y != 0f)
		{
			finalPosition.y = startPos.y;
		}
		base.gameObject.transform.SetPosition(finalPosition);
		m_bouncing = false;
	}

	private static float BounceSpline(float k)
	{
		if (k < 0.5f)
		{
			return QuadOut(k * 2f);
		}
		return 1f - QuadIn(k * 2f - 1f);
	}

	private static float QuadOut(float k)
	{
		return k * (2f - k);
	}

	private static float QuadIn(float k)
	{
		return k * k;
	}
}
