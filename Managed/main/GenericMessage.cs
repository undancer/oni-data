using KSerialization;
using UnityEngine;

public class GenericMessage : Message
{
	[Serialize]
	private string title;

	[Serialize]
	private string tooltip;

	[Serialize]
	private string body;

	[Serialize]
	private Ref<KMonoBehaviour> clickFocus = new Ref<KMonoBehaviour>();

	public GenericMessage(string _title, string _body, string _tooltip, KMonoBehaviour click_focus = null)
	{
		title = _title;
		body = _body;
		tooltip = _tooltip;
		clickFocus.Set(click_focus);
	}

	public GenericMessage()
	{
	}

	public override string GetSound()
	{
		return null;
	}

	public override string GetMessageBody()
	{
		return body;
	}

	public override string GetTooltip()
	{
		return tooltip;
	}

	public override string GetTitle()
	{
		return title;
	}

	public override void OnClick()
	{
		KMonoBehaviour kMonoBehaviour = clickFocus.Get();
		if (kMonoBehaviour == null)
		{
			return;
		}
		Transform transform = kMonoBehaviour.transform;
		if (!(transform == null))
		{
			Vector3 position = transform.GetPosition();
			position.z = -40f;
			CameraController.Instance.SetTargetPos(position, 8f, playSound: true);
			if (transform.GetComponent<KSelectable>() != null)
			{
				SelectTool.Instance.Select(transform.GetComponent<KSelectable>());
			}
		}
	}
}
