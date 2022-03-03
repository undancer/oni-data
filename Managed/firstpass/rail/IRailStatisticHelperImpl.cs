using System;

namespace rail
{
	public class IRailStatisticHelperImpl : RailObject, IRailStatisticHelper
	{
		internal IRailStatisticHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailStatisticHelperImpl()
		{
		}

		public virtual IRailPlayerStats CreatePlayerStats(RailID player)
		{
			IntPtr intPtr = ((player == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (player != null)
			{
				RailConverter.Csharp2Cpp(player, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailStatisticHelper_CreatePlayerStats(swigCPtr_, intPtr);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailPlayerStatsImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual IRailGlobalStats GetGlobalStats()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailStatisticHelper_GetGlobalStats(swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailGlobalStatsImpl(intPtr);
			}
			return null;
		}

		public virtual RailResult AsyncGetNumberOfPlayer(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStatisticHelper_AsyncGetNumberOfPlayer(swigCPtr_, user_data);
		}
	}
}
