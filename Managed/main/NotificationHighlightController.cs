using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationHighlightController : KMonoBehaviour
{
	public RectTransform highlightBoxPrefab;

	private RectTransform highlightBox;

	private List<NotificationHighlightTarget> targets = new List<NotificationHighlightTarget>();

	private ManagementMenuNotification activeTargetNotification;

	protected override void OnSpawn()
	{
		highlightBox = Util.KInstantiateUI<RectTransform>(highlightBoxPrefab.gameObject, base.gameObject);
		HideBox();
	}

	[ContextMenu("Force Update")]
	protected void LateUpdate()
	{
		bool flag = false;
		if (activeTargetNotification != null)
		{
			foreach (NotificationHighlightTarget target in targets)
			{
				if (target.targetKey == activeTargetNotification.highlightTarget)
				{
					SnapBoxToTarget(target);
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			HideBox();
		}
	}

	public void AddTarget(NotificationHighlightTarget target)
	{
		targets.Add(target);
	}

	public void RemoveTarget(NotificationHighlightTarget target)
	{
		targets.Remove(target);
	}

	public void SetActiveTarget(ManagementMenuNotification notification)
	{
		activeTargetNotification = notification;
	}

	public void ClearActiveTarget(ManagementMenuNotification checkNotification)
	{
		if (checkNotification == activeTargetNotification)
		{
			activeTargetNotification = null;
		}
	}

	public void ClearActiveTarget()
	{
		activeTargetNotification = null;
	}

	public void TargetViewed(NotificationHighlightTarget target)
	{
		if (activeTargetNotification != null && activeTargetNotification.highlightTarget == target.targetKey)
		{
			activeTargetNotification.View();
		}
	}

	private void SnapBoxToTarget(NotificationHighlightTarget target)
	{
		RectTransform rectTransform = target.rectTransform();
		Vector3 position = rectTransform.GetPosition();
		highlightBox.sizeDelta = rectTransform.rect.size;
		highlightBox.SetPosition(position + new Vector3(rectTransform.rect.position.x, rectTransform.rect.position.y, 0f));
		RectMask2D componentInParent = rectTransform.GetComponentInParent<RectMask2D>();
		if (componentInParent != null)
		{
			RectTransform rectTransform2 = componentInParent.rectTransform();
			Vector3 a = rectTransform2.TransformPoint(rectTransform2.rect.min);
			Vector3 a2 = rectTransform2.TransformPoint(rectTransform2.rect.max);
			Vector3 b = highlightBox.TransformPoint(highlightBox.rect.min);
			Vector3 b2 = highlightBox.TransformPoint(highlightBox.rect.max);
			Vector3 vector = a - b;
			Vector3 vector2 = a2 - b2;
			if (vector.x > 0f)
			{
				highlightBox.anchoredPosition += new Vector2(vector.x, 0f);
				highlightBox.sizeDelta -= new Vector2(vector.x, 0f);
			}
			else if (vector.y > 0f)
			{
				highlightBox.anchoredPosition += new Vector2(0f, vector.y);
				highlightBox.sizeDelta -= new Vector2(0f, vector.y);
			}
			if (vector2.x < 0f)
			{
				highlightBox.sizeDelta += new Vector2(vector2.x, 0f);
			}
			if (vector2.y < 0f)
			{
				highlightBox.sizeDelta += new Vector2(0f, vector2.y);
			}
		}
		highlightBox.gameObject.SetActive(highlightBox.sizeDelta.x > 0f && highlightBox.sizeDelta.y > 0f);
	}

	private void HideBox()
	{
		highlightBox.gameObject.SetActive(value: false);
	}
}
