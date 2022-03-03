namespace rail
{
	public interface IRailGroupChatHelper
	{
		RailResult AsyncQueryGroupsInfo(string user_data);

		IRailGroupChat AsyncOpenGroupChat(string group_id, string user_data);
	}
}
