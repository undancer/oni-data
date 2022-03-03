namespace rail
{
	public interface IRailFile : IRailComponent
	{
		string GetFilename();

		uint Read(byte[] buff, uint bytes_to_read, out RailResult result);

		uint Read(byte[] buff, uint bytes_to_read);

		uint Write(byte[] buff, uint bytes_to_write, out RailResult result);

		uint Write(byte[] buff, uint bytes_to_write);

		RailResult AsyncRead(uint bytes_to_read, string user_data);

		RailResult AsyncWrite(byte[] buffer, uint bytes_to_write, string user_data);

		uint GetSize();

		void Close();
	}
}
