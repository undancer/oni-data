using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyDetailsCopyMemberAttributeByKeyOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_TargetUserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_AttrKey;

		public int ApiVersion
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_ApiVersion, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ApiVersion, value);
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
			set
			{
				Helper.TryMarshalSet(ref m_TargetUserId, value);
			}
		}

		public string AttrKey
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_AttrKey, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AttrKey, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
