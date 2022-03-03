namespace rail
{
	public class AsyncReadStreamFileResult : EventBase
	{
		public uint try_read_length;

		public int offset;

		public string data;

		public string filename;
	}
}
