using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TimeOfDayPositioner")]
public class TimeOfDayPositioner : KMonoBehaviour
{
	[SerializeField]
	private RectTransform targetRect;

	private void Update()
	{
		float f = GameClock.Instance.GetCurrentCycleAsPercentage() * targetRect.rect.width;
		(base.transform as RectTransform).anchoredPosition = targetRect.anchoredPosition + new Vector2(Mathf.Round(f), 0f);
	}
}
