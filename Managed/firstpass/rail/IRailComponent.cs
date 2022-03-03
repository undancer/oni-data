namespace rail
{
	public interface IRailComponent
	{
		ulong GetComponentVersion();

		void Release();
	}
}
