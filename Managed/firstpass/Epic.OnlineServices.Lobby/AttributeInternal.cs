using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct AttributeInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_Data;

		private LobbyAttributeVisibility m_Visibility;

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

		public AttributeDataInternal? Data
		{
			get
			{
				AttributeDataInternal? target = Helper.GetDefault<AttributeDataInternal?>();
				Helper.TryMarshalGet(m_Data, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Data, value);
			}
		}

		public LobbyAttributeVisibility Visibility
		{
			get
			{
				LobbyAttributeVisibility target = Helper.GetDefault<LobbyAttributeVisibility>();
				Helper.TryMarshalGet(m_Visibility, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Visibility, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_Data);
		}
	}
}
