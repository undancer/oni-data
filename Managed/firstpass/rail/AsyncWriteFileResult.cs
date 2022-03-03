namespace rail
{
	public class AsyncWriteFileResult : EventBase
	{
		public uint written_length;

		public int offset;

		public uint try_write_length;

		public string filename;
	}
}
