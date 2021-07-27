using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	public sealed class P2PInterface : Handle
	{
		public const int GetportrangeApiLatest = 1;

		public const int SetportrangeApiLatest = 1;

		public const int GetrelaycontrolApiLatest = 1;

		public const int SetrelaycontrolApiLatest = 1;

		public const int GetnattypeApiLatest = 1;

		public const int QuerynattypeApiLatest = 1;

		public const int CloseconnectionsApiLatest = 1;

		public const int CloseconnectionApiLatest = 1;

		public const int AcceptconnectionApiLatest = 1;

		public const int AddnotifypeerconnectionclosedApiLatest = 1;

		public const int AddnotifypeerconnectionrequestApiLatest = 1;

		public const int ReceivepacketApiLatest = 2;

		public const int GetnextreceivedpacketsizeApiLatest = 2;

		public const int SendpacketApiLatest = 2;

		public const int SocketidApiLatest = 1;

		public const int MaxConnections = 32;

		public const int MaxPacketSize = 1170;

		public P2PInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result SendPacket(SendPacketOptions options)
		{
			SendPacketOptionsInternal options2 = Helper.CopyProperties<SendPacketOptionsInternal>(options);
			Result source = EOS_P2P_SendPacket(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result GetNextReceivedPacketSize(GetNextReceivedPacketSizeOptions options, out uint outPacketSizeBytes)
		{
			GetNextReceivedPacketSizeOptionsInternal options2 = Helper.CopyProperties<GetNextReceivedPacketSizeOptionsInternal>(options);
			outPacketSizeBytes = Helper.GetDefault<uint>();
			Result source = EOS_P2P_GetNextReceivedPacketSize(base.InnerHandle, ref options2, ref outPacketSizeBytes);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result ReceivePacket(ReceivePacketOptions options, out ProductUserId outPeerId, out SocketId outSocketId, out byte outChannel, ref byte[] outData, out uint outBytesWritten)
		{
			ReceivePacketOptionsInternal options2 = Helper.CopyProperties<ReceivePacketOptionsInternal>(options);
			outPeerId = Helper.GetDefault<ProductUserId>();
			IntPtr outPeerId2 = IntPtr.Zero;
			outSocketId = Helper.GetDefault<SocketId>();
			SocketIdInternal outSocketId2 = default(SocketIdInternal);
			outChannel = Helper.GetDefault<byte>();
			outBytesWritten = Helper.GetDefault<uint>();
			Result source = EOS_P2P_ReceivePacket(base.InnerHandle, ref options2, ref outPeerId2, ref outSocketId2, ref outChannel, outData, ref outBytesWritten);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outPeerId2, out outPeerId);
			outSocketId = Helper.CopyProperties<SocketId>(outSocketId2);
			Helper.TryMarshalDispose(ref outSocketId2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public ulong AddNotifyPeerConnectionRequest(AddNotifyPeerConnectionRequestOptions options, object clientData, OnIncomingConnectionRequestCallback connectionRequestHandler)
		{
			AddNotifyPeerConnectionRequestOptionsInternal options2 = Helper.CopyProperties<AddNotifyPeerConnectionRequestOptionsInternal>(options);
			OnIncomingConnectionRequestCallbackInternal onIncomingConnectionRequestCallbackInternal = OnIncomingConnectionRequest;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, connectionRequestHandler, onIncomingConnectionRequestCallbackInternal);
			ulong num = EOS_P2P_AddNotifyPeerConnectionRequest(base.InnerHandle, ref options2, clientDataAddress, onIncomingConnectionRequestCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyPeerConnectionRequest(ulong notificationId)
		{
			Helper.TryRemoveCallbackByNotificationId(notificationId);
			EOS_P2P_RemoveNotifyPeerConnectionRequest(base.InnerHandle, notificationId);
		}

		public ulong AddNotifyPeerConnectionClosed(AddNotifyPeerConnectionClosedOptions options, object clientData, OnRemoteConnectionClosedCallback connectionClosedHandler)
		{
			AddNotifyPeerConnectionClosedOptionsInternal options2 = Helper.CopyProperties<AddNotifyPeerConnectionClosedOptionsInternal>(options);
			OnRemoteConnectionClosedCallbackInternal onRemoteConnectionClosedCallbackInternal = OnRemoteConnectionClosed;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, connectionClosedHandler, onRemoteConnectionClosedCallbackInternal);
			ulong num = EOS_P2P_AddNotifyPeerConnectionClosed(base.InnerHandle, ref options2, clientDataAddress, onRemoteConnectionClosedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyPeerConnectionClosed(ulong notificationId)
		{
			Helper.TryRemoveCallbackByNotificationId(notificationId);
			EOS_P2P_RemoveNotifyPeerConnectionClosed(base.InnerHandle, notificationId);
		}

		public Result AcceptConnection(AcceptConnectionOptions options)
		{
			AcceptConnectionOptionsInternal options2 = Helper.CopyProperties<AcceptConnectionOptionsInternal>(options);
			Result source = EOS_P2P_AcceptConnection(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CloseConnection(CloseConnectionOptions options)
		{
			CloseConnectionOptionsInternal options2 = Helper.CopyProperties<CloseConnectionOptionsInternal>(options);
			Result source = EOS_P2P_CloseConnection(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CloseConnections(CloseConnectionsOptions options)
		{
			CloseConnectionsOptionsInternal options2 = Helper.CopyProperties<CloseConnectionsOptionsInternal>(options);
			Result source = EOS_P2P_CloseConnections(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void QueryNATType(QueryNATTypeOptions options, object clientData, OnQueryNATTypeCompleteCallback nATTypeQueriedHandler)
		{
			QueryNATTypeOptionsInternal options2 = Helper.CopyProperties<QueryNATTypeOptionsInternal>(options);
			OnQueryNATTypeCompleteCallbackInternal onQueryNATTypeCompleteCallbackInternal = OnQueryNATTypeComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, nATTypeQueriedHandler, onQueryNATTypeCompleteCallbackInternal);
			EOS_P2P_QueryNATType(base.InnerHandle, ref options2, clientDataAddress, onQueryNATTypeCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public Result GetNATType(GetNATTypeOptions options, out NATType outNATType)
		{
			GetNATTypeOptionsInternal options2 = Helper.CopyProperties<GetNATTypeOptionsInternal>(options);
			outNATType = Helper.GetDefault<NATType>();
			Result source = EOS_P2P_GetNATType(base.InnerHandle, ref options2, ref outNATType);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetRelayControl(SetRelayControlOptions options)
		{
			SetRelayControlOptionsInternal options2 = Helper.CopyProperties<SetRelayControlOptionsInternal>(options);
			Result source = EOS_P2P_SetRelayControl(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result GetRelayControl(GetRelayControlOptions options, out RelayControl outRelayControl)
		{
			GetRelayControlOptionsInternal options2 = Helper.CopyProperties<GetRelayControlOptionsInternal>(options);
			outRelayControl = Helper.GetDefault<RelayControl>();
			Result source = EOS_P2P_GetRelayControl(base.InnerHandle, ref options2, ref outRelayControl);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetPortRange(SetPortRangeOptions options)
		{
			SetPortRangeOptionsInternal options2 = Helper.CopyProperties<SetPortRangeOptionsInternal>(options);
			Result source = EOS_P2P_SetPortRange(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result GetPortRange(GetPortRangeOptions options, out ushort outPort, out ushort outNumAdditionalPortsToTry)
		{
			GetPortRangeOptionsInternal options2 = Helper.CopyProperties<GetPortRangeOptionsInternal>(options);
			outPort = Helper.GetDefault<ushort>();
			outNumAdditionalPortsToTry = Helper.GetDefault<ushort>();
			Result source = EOS_P2P_GetPortRange(base.InnerHandle, ref options2, ref outPort, ref outNumAdditionalPortsToTry);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[MonoPInvokeCallback]
		internal static void OnQueryNATTypeComplete(IntPtr address)
		{
			OnQueryNATTypeCompleteCallback callback = null;
			OnQueryNATTypeCompleteInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryNATTypeCompleteCallback, OnQueryNATTypeCompleteInfoInternal, OnQueryNATTypeCompleteInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnRemoteConnectionClosed(IntPtr address)
		{
			OnRemoteConnectionClosedCallback callback = null;
			OnRemoteConnectionClosedInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnRemoteConnectionClosedCallback, OnRemoteConnectionClosedInfoInternal, OnRemoteConnectionClosedInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnIncomingConnectionRequest(IntPtr address)
		{
			OnIncomingConnectionRequestCallback callback = null;
			OnIncomingConnectionRequestInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnIncomingConnectionRequestCallback, OnIncomingConnectionRequestInfoInternal, OnIncomingConnectionRequestInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_P2P_GetPortRange(IntPtr handle, ref GetPortRangeOptionsInternal options, ref ushort outPort, ref ushort outNumAdditionalPortsToTry);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_P2P_SetPortRange(IntPtr handle, ref SetPortRangeOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_P2P_GetRelayControl(IntPtr handle, ref GetRelayControlOptionsInternal options, ref RelayControl outRelayControl);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_P2P_SetRelayControl(IntPtr handle, ref SetRelayControlOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_P2P_GetNATType(IntPtr handle, ref GetNATTypeOptionsInternal options, ref NATType outNATType);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_P2P_QueryNATType(IntPtr handle, ref QueryNATTypeOptionsInternal options, IntPtr clientData, OnQueryNATTypeCompleteCallbackInternal nATTypeQueriedHandler);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_P2P_CloseConnections(IntPtr handle, ref CloseConnectionsOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_P2P_CloseConnection(IntPtr handle, ref CloseConnectionOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_P2P_AcceptConnection(IntPtr handle, ref AcceptConnectionOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_P2P_RemoveNotifyPeerConnectionClosed(IntPtr handle, ulong notificationId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_P2P_AddNotifyPeerConnectionClosed(IntPtr handle, ref AddNotifyPeerConnectionClosedOptionsInternal options, IntPtr clientData, OnRemoteConnectionClosedCallbackInternal connectionClosedHandler);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_P2P_RemoveNotifyPeerConnectionRequest(IntPtr handle, ulong notificationId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_P2P_AddNotifyPeerConnectionRequest(IntPtr handle, ref AddNotifyPeerConnectionRequestOptionsInternal options, IntPtr clientData, OnIncomingConnectionRequestCallbackInternal connectionRequestHandler);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_P2P_ReceivePacket(IntPtr handle, ref ReceivePacketOptionsInternal options, ref IntPtr outPeerId, ref SocketIdInternal outSocketId, ref byte outChannel, byte[] outData, ref uint outBytesWritten);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_P2P_GetNextReceivedPacketSize(IntPtr handle, ref GetNextReceivedPacketSizeOptionsInternal options, ref uint outPacketSizeBytes);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_P2P_SendPacket(IntPtr handle, ref SendPacketOptionsInternal options);
	}
}
