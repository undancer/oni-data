using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyMemberStatusReceivedCallbackInfoInternal : ICallbackInfo
	{
		private IntPtr m_ClientData;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LobbyId;

		private IntPtr m_TargetUserId;

		private LobbyMemberStatus m_CurrentStatus;

		public object ClientData
		{
			get
			{
				object target = Helper.GetDefault<object>();
				Helper.TryMarshalGet(m_ClientData, out target);
				return target;
			}
		}

		public IntPtr ClientDataAddress => m_ClientData;

		public string LobbyId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_LobbyId, out target);
				return target;
			}
		}

		public ProductUserId TargetUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_TargetUserId, out target);
				return target;
			}
		}

		public LobbyMemberStatus CurrentStatus
		{
			get
			{
				LobbyMemberStatus target = Helper.GetDefault<LobbyMemberStatus>();
				Helper.TryMarshalGet(m_CurrentStatus, out target);
				return target;
			}
		}
	}
}
