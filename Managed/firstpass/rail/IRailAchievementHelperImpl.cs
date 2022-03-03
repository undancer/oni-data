using System;

namespace rail
{
	public class IRailAchievementHelperImpl : RailObject, IRailAchievementHelper
	{
		internal IRailAchievementHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailAchievementHelperImpl()
		{
		}

		public virtual IRailPlayerAchievement CreatePlayerAchievement(RailID player)
		{
			IntPtr intPtr = ((player == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (player != null)
			{
				RailConverter.Csharp2Cpp(player, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailAchievementHelper_CreatePlayerAchievement(swigCPtr_, intPtr);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailPlayerAchievementImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual IRailGlobalAchievement GetGlobalAchievement()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailAchievementHelper_GetGlobalAchievement(swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailGlobalAchievementImpl(intPtr);
			}
			return null;
		}
	}
}
