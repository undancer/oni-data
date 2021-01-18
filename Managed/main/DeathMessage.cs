using KSerialization;
using STRINGS;
using UnityEngine;

public class DeathMessage : TargetMessage
{
	[Serialize]
	private ResourceRef<Death> death = new ResourceRef<Death>();

	public DeathMessage()
	{
	}

	public DeathMessage(GameObject go, Death death)
		: base(go.GetComponent<KPrefabID>())
	{
		this.death.Set(death);
	}

	public override string GetSound()
	{
		return "";
	}

	public override bool PlayNotificationSound()
	{
		return false;
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.DUPLICANTDIED.NAME;
	}

	public override string GetTooltip()
	{
		return GetMessageBody();
	}

	public override string GetMessageBody()
	{
		return death.Get().description.Replace("{Target}", GetTarget().GetName());
	}
}
