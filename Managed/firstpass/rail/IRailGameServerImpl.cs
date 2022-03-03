using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailGameServerImpl : RailObject, IRailGameServer, IRailComponent
	{
		internal IRailGameServerImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailGameServerImpl()
		{
		}

		public virtual RailID GetGameServerRailID()
		{
			IntPtr ptr = RAIL_API_PINVOKE.IRailGameServer_GetGameServerRailID(swigCPtr_);
			RailID railID = new RailID();
			RailConverter.Cpp2Csharp(ptr, railID);
			return railID;
		}

		public virtual RailResult GetGameServerName(out string name)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServer_GetGameServerName(swigCPtr_, intPtr);
			}
			finally
			{
				name = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult GetGameServerFullName(out string full_name)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServer_GetGameServerFullName(swigCPtr_, intPtr);
			}
			finally
			{
				full_name = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailID GetOwnerRailID()
		{
			IntPtr ptr = RAIL_API_PINVOKE.IRailGameServer_GetOwnerRailID(swigCPtr_);
			RailID railID = new RailID();
			RailConverter.Cpp2Csharp(ptr, railID);
			return railID;
		}

		public virtual bool SetHost(string game_server_host)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetHost(swigCPtr_, game_server_host);
		}

		public virtual bool GetHost(out string game_server_host)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return RAIL_API_PINVOKE.IRailGameServer_GetHost(swigCPtr_, intPtr);
			}
			finally
			{
				game_server_host = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual bool SetMapName(string game_server_map)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetMapName(swigCPtr_, game_server_map);
		}

		public virtual bool GetMapName(out string game_server_map)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return RAIL_API_PINVOKE.IRailGameServer_GetMapName(swigCPtr_, intPtr);
			}
			finally
			{
				game_server_map = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual bool SetPasswordProtect(bool has_password)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetPasswordProtect(swigCPtr_, has_password);
		}

		public virtual bool GetPasswordProtect()
		{
			return RAIL_API_PINVOKE.IRailGameServer_GetPasswordProtect(swigCPtr_);
		}

		public virtual bool SetMaxPlayers(uint max_player_count)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetMaxPlayers(swigCPtr_, max_player_count);
		}

		public virtual uint GetMaxPlayers()
		{
			return RAIL_API_PINVOKE.IRailGameServer_GetMaxPlayers(swigCPtr_);
		}

		public virtual bool SetBotPlayers(uint bot_player_count)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetBotPlayers(swigCPtr_, bot_player_count);
		}

		public virtual uint GetBotPlayers()
		{
			return RAIL_API_PINVOKE.IRailGameServer_GetBotPlayers(swigCPtr_);
		}

		public virtual bool SetGameServerDescription(string game_server_description)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetGameServerDescription(swigCPtr_, game_server_description);
		}

		public virtual bool GetGameServerDescription(out string game_server_description)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return RAIL_API_PINVOKE.IRailGameServer_GetGameServerDescription(swigCPtr_, intPtr);
			}
			finally
			{
				game_server_description = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual bool SetGameServerTags(string game_server_tags)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetGameServerTags(swigCPtr_, game_server_tags);
		}

		public virtual bool GetGameServerTags(out string game_server_tags)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return RAIL_API_PINVOKE.IRailGameServer_GetGameServerTags(swigCPtr_, intPtr);
			}
			finally
			{
				game_server_tags = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual bool SetMods(List<string> server_mods)
		{
			IntPtr intPtr = ((server_mods == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (server_mods != null)
			{
				RailConverter.Csharp2Cpp(server_mods, intPtr);
			}
			try
			{
				return RAIL_API_PINVOKE.IRailGameServer_SetMods(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual bool GetMods(List<string> server_mods)
		{
			IntPtr intPtr = ((server_mods == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			try
			{
				return RAIL_API_PINVOKE.IRailGameServer_GetMods(swigCPtr_, intPtr);
			}
			finally
			{
				if (server_mods != null)
				{
					RailConverter.Cpp2Csharp(intPtr, server_mods);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual bool SetSpectatorHost(string spectator_host)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetSpectatorHost(swigCPtr_, spectator_host);
		}

		public virtual bool GetSpectatorHost(out string spectator_host)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return RAIL_API_PINVOKE.IRailGameServer_GetSpectatorHost(swigCPtr_, intPtr);
			}
			finally
			{
				spectator_host = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual bool SetGameServerVersion(string version)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetGameServerVersion(swigCPtr_, version);
		}

		public virtual bool GetGameServerVersion(out string version)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return RAIL_API_PINVOKE.IRailGameServer_GetGameServerVersion(swigCPtr_, intPtr);
			}
			finally
			{
				version = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual bool SetIsFriendOnly(bool is_friend_only)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetIsFriendOnly(swigCPtr_, is_friend_only);
		}

		public virtual bool GetIsFriendOnly()
		{
			return RAIL_API_PINVOKE.IRailGameServer_GetIsFriendOnly(swigCPtr_);
		}

		public virtual bool ClearAllMetadata()
		{
			return RAIL_API_PINVOKE.IRailGameServer_ClearAllMetadata(swigCPtr_);
		}

		public virtual RailResult GetMetadata(string key, out string value)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServer_GetMetadata(swigCPtr_, key, intPtr);
			}
			finally
			{
				value = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult SetMetadata(string key, string value)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_SetMetadata(swigCPtr_, key, value);
		}

		public virtual RailResult AsyncSetMetadata(List<RailKeyValue> key_values, string user_data)
		{
			IntPtr intPtr = ((key_values == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailKeyValue__SWIG_0());
			if (key_values != null)
			{
				RailConverter.Csharp2Cpp(key_values, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServer_AsyncSetMetadata(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailKeyValue(intPtr);
			}
		}

		public virtual RailResult AsyncGetMetadata(List<string> keys, string user_data)
		{
			IntPtr intPtr = ((keys == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (keys != null)
			{
				RailConverter.Csharp2Cpp(keys, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServer_AsyncGetMetadata(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult AsyncGetAllMetadata(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_AsyncGetAllMetadata(swigCPtr_, user_data);
		}

		public virtual RailResult AsyncAcquireGameServerSessionTicket(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_AsyncAcquireGameServerSessionTicket(swigCPtr_, user_data);
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
				return (RailResult)RAIL_API_PINVOKE.IRailGameServer_AsyncStartSessionWithPlayer(swigCPtr_, intPtr, intPtr2, user_data);
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
				RAIL_API_PINVOKE.IRailGameServer_TerminateSessionOfPlayer(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual void AbandonGameServerSessionTicket(RailSessionTicket session_ticket)
		{
			IntPtr intPtr = ((session_ticket == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSessionTicket());
			if (session_ticket != null)
			{
				RailConverter.Csharp2Cpp(session_ticket, intPtr);
			}
			try
			{
				RAIL_API_PINVOKE.IRailGameServer_AbandonGameServerSessionTicket(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSessionTicket(intPtr);
			}
		}

		public virtual RailResult ReportPlayerJoinGameServer(List<GameServerPlayerInfo> player_infos)
		{
			IntPtr intPtr = ((player_infos == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayGameServerPlayerInfo__SWIG_0());
			if (player_infos != null)
			{
				RailConverter.Csharp2Cpp(player_infos, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServer_ReportPlayerJoinGameServer(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayGameServerPlayerInfo(intPtr);
			}
		}

		public virtual RailResult ReportPlayerQuitGameServer(List<GameServerPlayerInfo> player_infos)
		{
			IntPtr intPtr = ((player_infos == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayGameServerPlayerInfo__SWIG_0());
			if (player_infos != null)
			{
				RailConverter.Csharp2Cpp(player_infos, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServer_ReportPlayerQuitGameServer(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayGameServerPlayerInfo(intPtr);
			}
		}

		public virtual RailResult UpdateGameServerPlayerList(List<GameServerPlayerInfo> player_infos)
		{
			IntPtr intPtr = ((player_infos == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayGameServerPlayerInfo__SWIG_0());
			if (player_infos != null)
			{
				RailConverter.Csharp2Cpp(player_infos, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServer_UpdateGameServerPlayerList(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayGameServerPlayerInfo(intPtr);
			}
		}

		public virtual uint GetCurrentPlayers()
		{
			return RAIL_API_PINVOKE.IRailGameServer_GetCurrentPlayers(swigCPtr_);
		}

		public virtual void RemoveAllPlayers()
		{
			RAIL_API_PINVOKE.IRailGameServer_RemoveAllPlayers(swigCPtr_);
		}

		public virtual RailResult RegisterToGameServerList()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_RegisterToGameServerList(swigCPtr_);
		}

		public virtual RailResult UnregisterFromGameServerList()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_UnregisterFromGameServerList(swigCPtr_);
		}

		public virtual RailResult CloseGameServer()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_CloseGameServer(swigCPtr_);
		}

		public virtual RailResult GetFriendsInGameServer(List<RailID> friend_ids)
		{
			IntPtr intPtr = ((friend_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServer_GetFriendsInGameServer(swigCPtr_, intPtr);
			}
			finally
			{
				if (friend_ids != null)
				{
					RailConverter.Cpp2Csharp(intPtr, friend_ids);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
		}

		public virtual bool IsUserInGameServer(RailID user_rail_id)
		{
			IntPtr intPtr = ((user_rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (user_rail_id != null)
			{
				RailConverter.Csharp2Cpp(user_rail_id, intPtr);
			}
			try
			{
				return RAIL_API_PINVOKE.IRailGameServer_IsUserInGameServer(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual bool SetServerInfo(string server_info)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetServerInfo(swigCPtr_, server_info);
		}

		public virtual bool GetServerInfo(out string server_info)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return RAIL_API_PINVOKE.IRailGameServer_GetServerInfo(swigCPtr_, intPtr);
			}
			finally
			{
				server_info = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult EnableTeamVoice(bool enable)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_EnableTeamVoice(swigCPtr_, enable);
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
