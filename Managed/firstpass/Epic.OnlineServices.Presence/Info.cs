namespace Epic.OnlineServices.Presence
{
	public class Info
	{
		public int ApiVersion => 2;

		public Status Status
		{
			get;
			set;
		}

		public EpicAccountId UserId
		{
			get;
			set;
		}

		public string ProductId
		{
			get;
			set;
		}

		public string ProductVersion
		{
			get;
			set;
		}

		public string Platform
		{
			get;
			set;
		}

		public string RichText
		{
			get;
			set;
		}

		public DataRecord[] Records
		{
			get;
			set;
		}

		public string ProductName
		{
			get;
			set;
		}
	}
}
