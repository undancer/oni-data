using System;

namespace rail
{
	public class IRailGroupChatHelperImpl : RailObject, IRailGroupChatHelper
	{
		internal IRailGroupChatHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailGroupChatHelperImpl()
		{
		}

		public virtual RailResult AsyncQueryGroupsInfo(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGroupChatHelper_AsyncQueryGroupsInfo(swigCPtr_, user_data);
		}

		public virtual IRailGroupChat AsyncOpenGroupChat(string group_id, string user_data)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailGroupChatHelper_AsyncOpenGroupChat(swigCPtr_, group_id, user_data);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailGroupChatImpl(intPtr);
			}
			return null;
		}
	}
}
