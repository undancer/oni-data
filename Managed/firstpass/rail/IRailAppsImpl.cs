using System;

namespace rail
{
	public class IRailAppsImpl : RailObject, IRailApps
	{
		internal IRailAppsImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailAppsImpl()
		{
		}

		public virtual bool IsGameInstalled(RailGameID game_id)
		{
			IntPtr intPtr = ((game_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailGameID__SWIG_0());
			if (game_id != null)
			{
				RailConverter.Csharp2Cpp(game_id, intPtr);
			}
			try
			{
				return RAIL_API_PINVOKE.IRailApps_IsGameInstalled(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailGameID(intPtr);
			}
		}

		public virtual RailResult AsyncQuerySubscribeWishPlayState(RailGameID game_id, string user_data)
		{
			IntPtr intPtr = ((game_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailGameID__SWIG_0());
			if (game_id != null)
			{
				RailConverter.Csharp2Cpp(game_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailApps_AsyncQuerySubscribeWishPlayState(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailGameID(intPtr);
			}
		}
	}
}
