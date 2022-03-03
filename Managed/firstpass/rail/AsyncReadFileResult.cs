namespace rail
{
	public class AsyncReadFileResult : EventBase
	{
		public uint try_read_length;

		public int offset;

		public string data;

		public string filename;
	}
}
