namespace rail
{
	public interface IRailGroupChat : IRailComponent
	{
		RailResult GetGroupInfo(RailGroupInfo group_info);

		RailResult OpenGroupWindow();
	}
}
