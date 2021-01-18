namespace Epic.OnlineServices
{
	public static class HelperExtensions
	{
		public static bool IsOperationComplete(this Result result)
		{
			return Helper.IsOperationComplete(result);
		}

		public static string ToHexString(this byte[] byteArray)
		{
			return Helper.ToHexString(byteArray);
		}
	}
}
