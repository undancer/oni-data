namespace rail
{
	public interface IRailStreamFile : IRailComponent
	{
		string GetFilename();

		RailResult AsyncRead(int offset, uint bytes_to_read, string user_data);

		RailResult AsyncWrite(byte[] buff, uint bytes_to_write, string user_data);

		ulong GetSize();

		RailResult Close();

		void Cancel();
	}
}
