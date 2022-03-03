namespace rail
{
	public interface RailCrashBuffer
	{
		string GetData();

		uint GetBufferLength();

		uint GetValidLength();

		uint SetData(string data, uint length, uint offset);

		uint SetData(string data, uint length);

		uint AppendData(string data, uint length);
	}
}
