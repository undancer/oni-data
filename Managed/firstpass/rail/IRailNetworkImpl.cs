using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailNetworkImpl : RailObject, IRailNetwork
	{
		internal IRailNetworkImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailNetworkImpl()
		{
		}

		public virtual RailResult AcceptSessionRequest(RailID local_peer, RailID remote_peer)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			IntPtr intPtr2 = ((remote_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (remote_peer != null)
			{
				RailConverter.Csharp2Cpp(remote_peer, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_AcceptSessionRequest(swigCPtr_, intPtr, intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				RAIL_API_PINVOKE.delete_RailID(intPtr2);
			}
		}

		public virtual RailResult SendData(RailID local_peer, RailID remote_peer, byte[] data_buf, uint data_len, uint message_type)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			IntPtr intPtr2 = ((remote_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (remote_peer != null)
			{
				RailConverter.Csharp2Cpp(remote_peer, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_SendData__SWIG_0(swigCPtr_, intPtr, intPtr2, data_buf, data_len, message_type);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				RAIL_API_PINVOKE.delete_RailID(intPtr2);
			}
		}

		public virtual RailResult SendData(RailID local_peer, RailID remote_peer, byte[] data_buf, uint data_len)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			IntPtr intPtr2 = ((remote_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (remote_peer != null)
			{
				RailConverter.Csharp2Cpp(remote_peer, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_SendData__SWIG_1(swigCPtr_, intPtr, intPtr2, data_buf, data_len);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				RAIL_API_PINVOKE.delete_RailID(intPtr2);
			}
		}

		public virtual RailResult SendReliableData(RailID local_peer, RailID remote_peer, byte[] data_buf, uint data_len, uint message_type)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			IntPtr intPtr2 = ((remote_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (remote_peer != null)
			{
				RailConverter.Csharp2Cpp(remote_peer, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_SendReliableData__SWIG_0(swigCPtr_, intPtr, intPtr2, data_buf, data_len, message_type);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				RAIL_API_PINVOKE.delete_RailID(intPtr2);
			}
		}

		public virtual RailResult SendReliableData(RailID local_peer, RailID remote_peer, byte[] data_buf, uint data_len)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			IntPtr intPtr2 = ((remote_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (remote_peer != null)
			{
				RailConverter.Csharp2Cpp(remote_peer, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_SendReliableData__SWIG_1(swigCPtr_, intPtr, intPtr2, data_buf, data_len);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				RAIL_API_PINVOKE.delete_RailID(intPtr2);
			}
		}

		public virtual bool IsDataReady(RailID local_peer, out uint data_len, out uint message_type)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			try
			{
				return RAIL_API_PINVOKE.IRailNetwork_IsDataReady__SWIG_0(swigCPtr_, intPtr, out data_len, out message_type);
			}
			finally
			{
				if (local_peer != null)
				{
					RailConverter.Cpp2Csharp(intPtr, local_peer);
				}
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual bool IsDataReady(RailID local_peer, out uint data_len)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			try
			{
				return RAIL_API_PINVOKE.IRailNetwork_IsDataReady__SWIG_1(swigCPtr_, intPtr, out data_len);
			}
			finally
			{
				if (local_peer != null)
				{
					RailConverter.Cpp2Csharp(intPtr, local_peer);
				}
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult ReadData(RailID local_peer, RailID remote_peer, byte[] data_buf, uint data_len, uint message_type)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			IntPtr intPtr2 = ((remote_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_ReadData__SWIG_0(swigCPtr_, intPtr, intPtr2, data_buf, data_len, message_type);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				if (remote_peer != null)
				{
					RailConverter.Cpp2Csharp(intPtr2, remote_peer);
				}
				RAIL_API_PINVOKE.delete_RailID(intPtr2);
			}
		}

		public virtual RailResult ReadData(RailID local_peer, RailID remote_peer, byte[] data_buf, uint data_len)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			IntPtr intPtr2 = ((remote_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_ReadData__SWIG_1(swigCPtr_, intPtr, intPtr2, data_buf, data_len);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				if (remote_peer != null)
				{
					RailConverter.Cpp2Csharp(intPtr2, remote_peer);
				}
				RAIL_API_PINVOKE.delete_RailID(intPtr2);
			}
		}

		public virtual RailResult BlockMessageType(RailID local_peer, uint message_type)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_BlockMessageType(swigCPtr_, intPtr, message_type);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult UnblockMessageType(RailID local_peer, uint message_type)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_UnblockMessageType(swigCPtr_, intPtr, message_type);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult CloseSession(RailID local_peer, RailID remote_peer)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			IntPtr intPtr2 = ((remote_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (remote_peer != null)
			{
				RailConverter.Csharp2Cpp(remote_peer, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_CloseSession(swigCPtr_, intPtr, intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				RAIL_API_PINVOKE.delete_RailID(intPtr2);
			}
		}

		public virtual RailResult ResolveHostname(string domain, List<string> ip_list)
		{
			IntPtr intPtr = ((ip_list == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_ResolveHostname(swigCPtr_, domain, intPtr);
			}
			finally
			{
				if (ip_list != null)
				{
					RailConverter.Cpp2Csharp(intPtr, ip_list);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult GetSessionState(RailID remote_peer, RailNetworkSessionState session_state)
		{
			IntPtr intPtr = ((remote_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (remote_peer != null)
			{
				RailConverter.Csharp2Cpp(remote_peer, intPtr);
			}
			IntPtr intPtr2 = ((session_state == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailNetworkSessionState__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_GetSessionState(swigCPtr_, intPtr, intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				if (session_state != null)
				{
					RailConverter.Cpp2Csharp(intPtr2, session_state);
				}
				RAIL_API_PINVOKE.delete_RailNetworkSessionState(intPtr2);
			}
		}

		public virtual RailResult ForbidSessionRelay(bool forbid_relay)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailNetwork_ForbidSessionRelay(swigCPtr_, forbid_relay);
		}

		public virtual RailResult SendRawData(RailID local_peer, RailGamePeer remote_game_peer, byte[] data_buf, uint data_len, bool reliable, uint message_type)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			IntPtr intPtr2 = ((remote_game_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailGamePeer__SWIG_0());
			if (remote_game_peer != null)
			{
				RailConverter.Csharp2Cpp(remote_game_peer, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_SendRawData(swigCPtr_, intPtr, intPtr2, data_buf, data_len, reliable, message_type);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				RAIL_API_PINVOKE.delete_RailGamePeer(intPtr2);
			}
		}

		public virtual RailResult AcceptRawSessionRequest(RailID local_peer, RailGamePeer remote_game_peer)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			IntPtr intPtr2 = ((remote_game_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailGamePeer__SWIG_0());
			if (remote_game_peer != null)
			{
				RailConverter.Csharp2Cpp(remote_game_peer, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_AcceptRawSessionRequest(swigCPtr_, intPtr, intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				RAIL_API_PINVOKE.delete_RailGamePeer(intPtr2);
			}
		}

		public virtual RailResult ReadRawData(RailID local_peer, RailGamePeer remote_game_peer, byte[] data_buf, uint data_len, uint message_type)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			IntPtr intPtr2 = ((remote_game_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailGamePeer__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_ReadRawData__SWIG_0(swigCPtr_, intPtr, intPtr2, data_buf, data_len, message_type);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				if (remote_game_peer != null)
				{
					RailConverter.Cpp2Csharp(intPtr2, remote_game_peer);
				}
				RAIL_API_PINVOKE.delete_RailGamePeer(intPtr2);
			}
		}

		public virtual RailResult ReadRawData(RailID local_peer, RailGamePeer remote_game_peer, byte[] data_buf, uint data_len)
		{
			IntPtr intPtr = ((local_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (local_peer != null)
			{
				RailConverter.Csharp2Cpp(local_peer, intPtr);
			}
			IntPtr intPtr2 = ((remote_game_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailGamePeer__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailNetwork_ReadRawData__SWIG_1(swigCPtr_, intPtr, intPtr2, data_buf, data_len);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				if (remote_game_peer != null)
				{
					RailConverter.Cpp2Csharp(intPtr2, remote_game_peer);
				}
				RAIL_API_PINVOKE.delete_RailGamePeer(intPtr2);
			}
		}
	}
}
