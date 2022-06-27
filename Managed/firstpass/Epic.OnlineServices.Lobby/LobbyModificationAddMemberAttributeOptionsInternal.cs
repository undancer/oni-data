using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyModificationAddMemberAttributeOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_Attribute;

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

		public AttributeDataInternal? Attribute
		{
			get
			{
				AttributeDataInternal? target = Helper.GetDefault<AttributeDataInternal?>();
				Helper.TryMarshalGet(m_Attribute, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Attribute, value);
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
			Helper.TryMarshalDispose(ref m_Attribute);
		}
	}
}
