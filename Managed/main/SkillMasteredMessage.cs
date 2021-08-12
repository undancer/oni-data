using KSerialization;
using STRINGS;

public class SkillMasteredMessage : Message
{
	[Serialize]
	private string minionName;

	public SkillMasteredMessage()
	{
	}

	public SkillMasteredMessage(MinionResume resume)
	{
		minionName = resume.GetProperName();
	}

	public override string GetSound()
	{
		return "AI_Notification_ResearchComplete";
	}

	public override string GetMessageBody()
	{
		Debug.Assert(minionName != null);
		string arg = string.Format(MISC.NOTIFICATIONS.SKILL_POINT_EARNED.LINE, minionName);
		return string.Format(MISC.NOTIFICATIONS.SKILL_POINT_EARNED.MESSAGEBODY, arg);
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.SKILL_POINT_EARNED.NAME.Replace("{Duplicant}", minionName);
	}

	public override string GetTooltip()
	{
		return MISC.NOTIFICATIONS.SKILL_POINT_EARNED.TOOLTIP.Replace("{Duplicant}", minionName);
	}

	public override bool IsValid()
	{
		return minionName != null;
	}
}
