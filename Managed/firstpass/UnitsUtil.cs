public static class UnitsUtil
{
	public static bool IsTimeUnit(Units unit)
	{
		if ((uint)(unit - 1) <= 1u)
		{
			return true;
		}
		return false;
	}

	public static string GetUnitSuffix(Units unit)
	{
		if (unit == Units.Kelvin)
		{
			return "K";
		}
		return "";
	}
}
