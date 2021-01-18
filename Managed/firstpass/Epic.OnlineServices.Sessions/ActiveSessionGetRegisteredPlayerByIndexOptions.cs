namespace Epic.OnlineServices.Sessions
{
	public class ActiveSessionGetRegisteredPlayerByIndexOptions
	{
		public int ApiVersion => 1;

		public uint PlayerIndex
		{
			get;
			set;
		}
	}
}
