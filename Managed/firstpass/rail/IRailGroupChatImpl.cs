using System;

namespace rail
{
	public class IRailGroupChatImpl : RailObject, IRailGroupChat, IRailComponent
	{
		internal IRailGroupChatImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailGroupChatImpl()
		{
		}

		public virtual RailResult GetGroupInfo(RailGroupInfo group_info)
		{
			IntPtr intPtr = ((group_info == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailGroupInfo__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGroupChat_GetGroupInfo(swigCPtr_, intPtr);
			}
			finally
			{
				if (group_info != null)
				{
					RailConverter.Cpp2Csharp(intPtr, group_info);
				}
				RAIL_API_PINVOKE.delete_RailGroupInfo(intPtr);
			}
		}

		public virtual RailResult OpenGroupWindow()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGroupChat_OpenGroupWindow(swigCPtr_);
		}

		public virtual ulong GetComponentVersion()
		{
			return RAIL_API_PINVOKE.IRailComponent_GetComponentVersion(swigCPtr_);
		}

		public virtual void Release()
		{
			RAIL_API_PINVOKE.IRailComponent_Release(swigCPtr_);
		}
	}
}
