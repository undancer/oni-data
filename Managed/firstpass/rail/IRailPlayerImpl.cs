using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailPlayerImpl : RailObject, IRailPlayer
	{
		internal IRailPlayerImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailPlayerImpl()
		{
		}

		public virtual bool AlreadyLoggedIn()
		{
			return RAIL_API_PINVOKE.IRailPlayer_AlreadyLoggedIn(swigCPtr_);
		}

		public virtual RailID GetRailID()
		{
			IntPtr ptr = RAIL_API_PINVOKE.IRailPlayer_GetRailID(swigCPtr_);
			RailID railID = new RailID();
			RailConverter.Cpp2Csharp(ptr, railID);
			return railID;
		}

		public virtual RailResult GetPlayerDataPath(out string path)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailPlayer_GetPlayerDataPath(swigCPtr_, intPtr);
			}
			finally
			{
				path = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult AsyncAcquireSessionTicket(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayer_AsyncAcquireSessionTicket(swigCPtr_, user_data);
		}

		public virtual RailResult AsyncStartSessionWithPlayer(RailSessionTicket player_ticket, RailID player_rail_id, string user_data)
		{
			IntPtr intPtr = ((player_ticket == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSessionTicket());
			if (player_ticket != null)
			{
				RailConverter.Csharp2Cpp(player_ticket, intPtr);
			}
			IntPtr intPtr2 = ((player_rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (player_rail_id != null)
			{
				RailConverter.Csharp2Cpp(player_rail_id, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailPlayer_AsyncStartSessionWithPlayer(swigCPtr_, intPtr, intPtr2, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSessionTicket(intPtr);
				RAIL_API_PINVOKE.delete_RailID(intPtr2);
			}
		}

		public virtual void TerminateSessionOfPlayer(RailID player_rail_id)
		{
			IntPtr intPtr = ((player_rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (player_rail_id != null)
			{
				RailConverter.Csharp2Cpp(player_rail_id, intPtr);
			}
			try
			{
				RAIL_API_PINVOKE.IRailPlayer_TerminateSessionOfPlayer(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual void AbandonSessionTicket(RailSessionTicket session_ticket)
		{
			IntPtr intPtr = ((session_ticket == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSessionTicket());
			if (session_ticket != null)
			{
				RailConverter.Csharp2Cpp(session_ticket, intPtr);
			}
			try
			{
				RAIL_API_PINVOKE.IRailPlayer_AbandonSessionTicket(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSessionTicket(intPtr);
			}
		}

		public virtual RailResult GetPlayerName(out string name)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailPlayer_GetPlayerName(swigCPtr_, intPtr);
			}
			finally
			{
				name = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual EnumRailPlayerOwnershipType GetPlayerOwnershipType()
		{
			return (EnumRailPlayerOwnershipType)RAIL_API_PINVOKE.IRailPlayer_GetPlayerOwnershipType(swigCPtr_);
		}

		public virtual RailResult AsyncGetGamePurchaseKey(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayer_AsyncGetGamePurchaseKey(swigCPtr_, user_data);
		}

		public virtual bool IsGameRevenueLimited()
		{
			return RAIL_API_PINVOKE.IRailPlayer_IsGameRevenueLimited(swigCPtr_);
		}

		public virtual float GetRateOfGameRevenue()
		{
			return RAIL_API_PINVOKE.IRailPlayer_GetRateOfGameRevenue(swigCPtr_);
		}

		public virtual RailResult AsyncQueryPlayerBannedStatus(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayer_AsyncQueryPlayerBannedStatus(swigCPtr_, user_data);
		}

		public virtual RailResult AsyncGetAuthenticateURL(RailGetAuthenticateURLOptions options, string user_data)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailGetAuthenticateURLOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailPlayer_AsyncGetAuthenticateURL(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailGetAuthenticateURLOptions(intPtr);
			}
		}

		public virtual RailResult AsyncGetPlayerMetadata(List<string> keys, string user_data)
		{
			IntPtr intPtr = ((keys == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (keys != null)
			{
				RailConverter.Csharp2Cpp(keys, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailPlayer_AsyncGetPlayerMetadata(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult AsyncGetEncryptedGameTicket(string set_metadata, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayer_AsyncGetEncryptedGameTicket(swigCPtr_, set_metadata, user_data);
		}

		public virtual RailPlayerAccountType GetPlayerAccountType()
		{
			return (RailPlayerAccountType)RAIL_API_PINVOKE.IRailPlayer_GetPlayerAccountType(swigCPtr_);
		}
	}
}
