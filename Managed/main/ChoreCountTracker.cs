public class ChoreCountTracker : WorldTracker
{
	public ChoreGroup choreGroup;

	public ChoreCountTracker(int worldID, ChoreGroup group)
		: base(worldID)
	{
		choreGroup = group;
	}

	public override void UpdateData()
	{
		float num = 0f;
		foreach (Chore chore in GlobalChoreProvider.Instance.chores)
		{
			if (chore == null || chore.target.Equals(null) || chore.gameObject == null || chore.gameObject.GetMyWorldId() != base.WorldID)
			{
				continue;
			}
			ChoreGroup[] groups = chore.choreType.groups;
			foreach (ChoreGroup choreGroup in groups)
			{
				if (choreGroup == this.choreGroup)
				{
					num += 1f;
					break;
				}
			}
		}
		AddPoint(num);
	}

	public override string FormatValueString(float value)
	{
		return value.ToString();
	}
}
