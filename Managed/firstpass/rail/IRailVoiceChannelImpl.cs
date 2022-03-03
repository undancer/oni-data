using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailVoiceChannelImpl : RailObject, IRailVoiceChannel, IRailComponent
	{
		internal IRailVoiceChannelImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailVoiceChannelImpl()
		{
		}

		public virtual RailVoiceChannelID GetVoiceChannelID()
		{
			IntPtr ptr = RAIL_API_PINVOKE.IRailVoiceChannel_GetVoiceChannelID(swigCPtr_);
			RailVoiceChannelID railVoiceChannelID = new RailVoiceChannelID();
			RailConverter.Cpp2Csharp(ptr, railVoiceChannelID);
			return railVoiceChannelID;
		}

		public virtual string GetVoiceChannelName()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.IRailVoiceChannel_GetVoiceChannelName(swigCPtr_));
		}

		public virtual EnumRailVoiceChannelJoinState GetJoinState()
		{
			return (EnumRailVoiceChannelJoinState)RAIL_API_PINVOKE.IRailVoiceChannel_GetJoinState(swigCPtr_);
		}

		public virtual RailResult AsyncJoinVoiceChannel(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceChannel_AsyncJoinVoiceChannel(swigCPtr_, user_data);
		}

		public virtual RailResult AsyncLeaveVoiceChannel(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceChannel_AsyncLeaveVoiceChannel(swigCPtr_, user_data);
		}

		public virtual RailResult GetUsers(List<RailID> user_list)
		{
			IntPtr intPtr = ((user_list == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailVoiceChannel_GetUsers(swigCPtr_, intPtr);
			}
			finally
			{
				if (user_list != null)
				{
					RailConverter.Cpp2Csharp(intPtr, user_list);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
		}

		public virtual RailResult AsyncAddUsers(List<RailID> user_list, string user_data)
		{
			IntPtr intPtr = ((user_list == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			if (user_list != null)
			{
				RailConverter.Csharp2Cpp(user_list, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailVoiceChannel_AsyncAddUsers(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
		}

		public virtual RailResult AsyncRemoveUsers(List<RailID> user_list, string user_data)
		{
			IntPtr intPtr = ((user_list == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			if (user_list != null)
			{
				RailConverter.Csharp2Cpp(user_list, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailVoiceChannel_AsyncRemoveUsers(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
		}

		public virtual RailResult CloseChannel()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceChannel_CloseChannel(swigCPtr_);
		}

		public virtual RailResult SetSelfSpeaking(bool speaking)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceChannel_SetSelfSpeaking(swigCPtr_, speaking);
		}

		public virtual bool IsSelfSpeaking()
		{
			return RAIL_API_PINVOKE.IRailVoiceChannel_IsSelfSpeaking(swigCPtr_);
		}

		public virtual RailResult AsyncSetUsersSpeakingState(List<RailVoiceChannelUserSpeakingState> users_speaking_state, string user_data)
		{
			IntPtr intPtr = ((users_speaking_state == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailVoiceChannelUserSpeakingState__SWIG_0());
			if (users_speaking_state != null)
			{
				RailConverter.Csharp2Cpp(users_speaking_state, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailVoiceChannel_AsyncSetUsersSpeakingState(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailVoiceChannelUserSpeakingState(intPtr);
			}
		}

		public virtual RailResult GetUsersSpeakingState(List<RailVoiceChannelUserSpeakingState> users_speaking_state)
		{
			IntPtr intPtr = ((users_speaking_state == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailVoiceChannelUserSpeakingState__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailVoiceChannel_GetUsersSpeakingState(swigCPtr_, intPtr);
			}
			finally
			{
				if (users_speaking_state != null)
				{
					RailConverter.Cpp2Csharp(intPtr, users_speaking_state);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailVoiceChannelUserSpeakingState(intPtr);
			}
		}

		public virtual RailResult GetSpeakingUsers(List<RailID> speaking_users, List<RailID> not_speaking_users)
		{
			IntPtr intPtr = ((speaking_users == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			IntPtr intPtr2 = ((not_speaking_users == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailVoiceChannel_GetSpeakingUsers(swigCPtr_, intPtr, intPtr2);
			}
			finally
			{
				if (speaking_users != null)
				{
					RailConverter.Cpp2Csharp(intPtr, speaking_users);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
				if (not_speaking_users != null)
				{
					RailConverter.Cpp2Csharp(intPtr2, not_speaking_users);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr2);
			}
		}

		public virtual bool IsOwner()
		{
			return RAIL_API_PINVOKE.IRailVoiceChannel_IsOwner(swigCPtr_);
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
