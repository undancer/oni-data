using System;

public class ToiletTracker : WorldTracker
{
	public ToiletTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		throw new NotImplementedException();
	}

	public override string FormatValueString(float value)
	{
		return value.ToString();
	}
}
