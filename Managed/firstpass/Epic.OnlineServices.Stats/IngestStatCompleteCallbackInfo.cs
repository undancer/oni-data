namespace Epic.OnlineServices.Stats
{
	public class IngestStatCompleteCallbackInfo
	{
		public Result ResultCode
		{
			get;
			set;
		}

		public object ClientData
		{
			get;
			set;
		}

		public ProductUserId LocalUserId
		{
			get;
			set;
		}

		public ProductUserId TargetUserId
		{
			get;
			set;
		}
	}
}
