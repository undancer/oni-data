public struct DataPoint
{
	public float periodStart;

	public float periodEnd;

	public float periodValue;

	public DataPoint(float start, float end, float value)
	{
		periodStart = start;
		periodEnd = end;
		periodValue = value;
	}
}
