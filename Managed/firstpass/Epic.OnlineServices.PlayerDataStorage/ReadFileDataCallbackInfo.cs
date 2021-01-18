namespace Epic.OnlineServices.PlayerDataStorage
{
	public class ReadFileDataCallbackInfo
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

		public uint TotalFileSizeBytes
		{
			get;
			set;
		}

		public bool IsLastChunk
		{
			get;
			set;
		}

		public byte[] DataChunk
		{
			get;
			set;
		}
	}
}
