using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SelectMarker")]
public class SelectMarker : KMonoBehaviour
{
	public float animationOffset = 0.1f;

	private Transform targetTransform;

	public void SetTargetTransform(Transform target_transform)
	{
		targetTransform = target_transform;
		LateUpdate();
	}

	private void LateUpdate()
	{
		if (targetTransform == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		Vector3 position = targetTransform.GetPosition();
		KCollider2D component = targetTransform.GetComponent<KCollider2D>();
		if (component != null)
		{
			position.x = component.bounds.center.x;
			position.y = component.bounds.center.y + component.bounds.size.y / 2f + 0.1f;
		}
		else
		{
			position.y += 2f;
		}
		Vector3 vector = new Vector3(0f, (Mathf.Sin(Time.unscaledTime * 4f) + 1f) * animationOffset, 0f);
		base.transform.SetPosition(position + vector);
	}
}
