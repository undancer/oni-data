using System;

namespace rail
{
	public class IRailAntiAddictionHelperImpl : RailObject, IRailAntiAddictionHelper
	{
		internal IRailAntiAddictionHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailAntiAddictionHelperImpl()
		{
		}

		public virtual RailResult AsyncQueryGameOnlineTime(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailAntiAddictionHelper_AsyncQueryGameOnlineTime(swigCPtr_, user_data);
		}
	}
}
