public class NotificationHighlightTarget : KMonoBehaviour
{
	public string targetKey;

	private NotificationHighlightController controller;

	protected void OnEnable()
	{
		controller = GetComponentInParent<NotificationHighlightController>();
		if (controller != null)
		{
			controller.AddTarget(this);
		}
	}

	protected void OnDisable()
	{
		if (controller != null)
		{
			controller.RemoveTarget(this);
		}
	}

	public void View()
	{
		NotificationHighlightController componentInParent = GetComponentInParent<NotificationHighlightController>();
		componentInParent.TargetViewed(this);
	}
}
