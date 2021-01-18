using KSerialization;
using STRINGS;

public class WorldDetectedMessage : Message
{
	[Serialize]
	private int worldID;

	public WorldDetectedMessage()
	{
	}

	public WorldDetectedMessage(WorldContainer world)
	{
		worldID = world.id;
	}

	public override string GetSound()
	{
		return "AI_Notification_ResearchComplete";
	}

	public override string GetMessageBody()
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(worldID);
		return string.Format(MISC.NOTIFICATIONS.WORLDDETECTED.MESSAGEBODY, world.GetProperName());
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.WORLDDETECTED.NAME;
	}

	public override string GetTooltip()
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(worldID);
		return string.Format(MISC.NOTIFICATIONS.WORLDDETECTED.TOOLTIP, world.GetProperName());
	}

	public override bool IsValid()
	{
		return worldID != ClusterManager.INVALID_WORLD_IDX;
	}
}
