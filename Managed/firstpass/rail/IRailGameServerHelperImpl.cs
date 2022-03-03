using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailGameServerHelperImpl : RailObject, IRailGameServerHelper
	{
		internal IRailGameServerHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailGameServerHelperImpl()
		{
		}

		public virtual RailResult AsyncGetGameServerPlayerList(RailID gameserver_rail_id, string user_data)
		{
			IntPtr intPtr = ((gameserver_rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (gameserver_rail_id != null)
			{
				RailConverter.Csharp2Cpp(gameserver_rail_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServerHelper_AsyncGetGameServerPlayerList(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult AsyncGetGameServerList(uint start_index, uint end_index, List<GameServerListFilter> alternative_filters, List<GameServerListSorter> sorter, string user_data)
		{
			IntPtr intPtr = ((alternative_filters == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayGameServerListFilter__SWIG_0());
			if (alternative_filters != null)
			{
				RailConverter.Csharp2Cpp(alternative_filters, intPtr);
			}
			IntPtr intPtr2 = ((sorter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayGameServerListSorter__SWIG_0());
			if (sorter != null)
			{
				RailConverter.Csharp2Cpp(sorter, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServerHelper_AsyncGetGameServerList(swigCPtr_, start_index, end_index, intPtr, intPtr2, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayGameServerListFilter(intPtr);
				RAIL_API_PINVOKE.delete_RailArrayGameServerListSorter(intPtr2);
			}
		}

		public virtual IRailGameServer AsyncCreateGameServer(CreateGameServerOptions options, string game_server_name, string user_data)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_CreateGameServerOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailGameServerHelper_AsyncCreateGameServer__SWIG_0(swigCPtr_, intPtr, game_server_name, user_data);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailGameServerImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_CreateGameServerOptions(intPtr);
			}
		}

		public virtual IRailGameServer AsyncCreateGameServer(CreateGameServerOptions options, string game_server_name)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_CreateGameServerOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailGameServerHelper_AsyncCreateGameServer__SWIG_1(swigCPtr_, intPtr, game_server_name);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailGameServerImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_CreateGameServerOptions(intPtr);
			}
		}

		public virtual IRailGameServer AsyncCreateGameServer(CreateGameServerOptions options)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_CreateGameServerOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailGameServerHelper_AsyncCreateGameServer__SWIG_2(swigCPtr_, intPtr);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailGameServerImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_CreateGameServerOptions(intPtr);
			}
		}

		public virtual IRailGameServer AsyncCreateGameServer()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailGameServerHelper_AsyncCreateGameServer__SWIG_3(swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailGameServerImpl(intPtr);
			}
			return null;
		}

		public virtual RailResult AsyncGetFavoriteGameServers(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServerHelper_AsyncGetFavoriteGameServers__SWIG_0(swigCPtr_, user_data);
		}

		public virtual RailResult AsyncGetFavoriteGameServers()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServerHelper_AsyncGetFavoriteGameServers__SWIG_1(swigCPtr_);
		}

		public virtual RailResult AsyncAddFavoriteGameServer(RailID game_server_id, string user_data)
		{
			IntPtr intPtr = ((game_server_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (game_server_id != null)
			{
				RailConverter.Csharp2Cpp(game_server_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServerHelper_AsyncAddFavoriteGameServer__SWIG_0(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult AsyncAddFavoriteGameServer(RailID game_server_id)
		{
			IntPtr intPtr = ((game_server_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (game_server_id != null)
			{
				RailConverter.Csharp2Cpp(game_server_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServerHelper_AsyncAddFavoriteGameServer__SWIG_1(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult AsyncRemoveFavoriteGameServer(RailID game_server_id, string user_Data)
		{
			IntPtr intPtr = ((game_server_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (game_server_id != null)
			{
				RailConverter.Csharp2Cpp(game_server_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServerHelper_AsyncRemoveFavoriteGameServer__SWIG_0(swigCPtr_, intPtr, user_Data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult AsyncRemoveFavoriteGameServer(RailID game_server_id)
		{
			IntPtr intPtr = ((game_server_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (game_server_id != null)
			{
				RailConverter.Csharp2Cpp(game_server_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailGameServerHelper_AsyncRemoveFavoriteGameServer__SWIG_1(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}
	}
}
