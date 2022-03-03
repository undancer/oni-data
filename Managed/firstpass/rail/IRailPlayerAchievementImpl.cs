using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailPlayerAchievementImpl : RailObject, IRailPlayerAchievement, IRailComponent
	{
		internal IRailPlayerAchievementImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailPlayerAchievementImpl()
		{
		}

		public virtual RailID GetRailID()
		{
			IntPtr ptr = RAIL_API_PINVOKE.IRailPlayerAchievement_GetRailID(swigCPtr_);
			RailID railID = new RailID();
			RailConverter.Cpp2Csharp(ptr, railID);
			return railID;
		}

		public virtual RailResult AsyncRequestAchievement(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_AsyncRequestAchievement(swigCPtr_, user_data);
		}

		public virtual RailResult HasAchieved(string name, out bool achieved)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_HasAchieved(swigCPtr_, name, out achieved);
		}

		public virtual RailResult GetAchievementInfo(string name, out string achievement_info)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_GetAchievementInfo__SWIG_0(swigCPtr_, name, intPtr);
			}
			finally
			{
				achievement_info = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult AsyncTriggerAchievementProgress(string name, uint current_value, uint max_value, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_AsyncTriggerAchievementProgress__SWIG_0(swigCPtr_, name, current_value, max_value, user_data);
		}

		public virtual RailResult AsyncTriggerAchievementProgress(string name, uint current_value, uint max_value)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_AsyncTriggerAchievementProgress__SWIG_1(swigCPtr_, name, current_value, max_value);
		}

		public virtual RailResult AsyncTriggerAchievementProgress(string name, uint current_value)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_AsyncTriggerAchievementProgress__SWIG_2(swigCPtr_, name, current_value);
		}

		public virtual RailResult MakeAchievement(string name)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_MakeAchievement(swigCPtr_, name);
		}

		public virtual RailResult CancelAchievement(string name)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_CancelAchievement(swigCPtr_, name);
		}

		public virtual RailResult AsyncStoreAchievement(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_AsyncStoreAchievement(swigCPtr_, user_data);
		}

		public virtual RailResult ResetAllAchievements()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_ResetAllAchievements(swigCPtr_);
		}

		public virtual RailResult GetAllAchievementsName(List<string> names)
		{
			IntPtr intPtr = ((names == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_GetAllAchievementsName(swigCPtr_, intPtr);
			}
			finally
			{
				if (names != null)
				{
					RailConverter.Cpp2Csharp(intPtr, names);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult GetAchievementInfo(string name, RailPlayerAchievementInfo achievement_info)
		{
			IntPtr intPtr = ((achievement_info == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailPlayerAchievementInfo__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_GetAchievementInfo__SWIG_1(swigCPtr_, name, intPtr);
			}
			finally
			{
				if (achievement_info != null)
				{
					RailConverter.Cpp2Csharp(intPtr, achievement_info);
				}
				RAIL_API_PINVOKE.delete_RailPlayerAchievementInfo(intPtr);
			}
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
