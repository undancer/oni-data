namespace Epic.OnlineServices.PlayerDataStorage
{
	public class WriteFileDataCallbackInfo
	{
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

		public string Filename
		{
			get;
			set;
		}

		public uint DataBufferLengthBytes
		{
			get;
			set;
		}
	}
}
