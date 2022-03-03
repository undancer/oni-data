namespace rail
{
	public class RailUserSpaceDownloadResult
	{
		public string err_msg;

		public ulong finished_bytes;

		public uint finished_files;

		public ulong total_bytes;

		public uint total_files;

		public SpaceWorkID id = new SpaceWorkID();

		public uint err_code;
	}
}
