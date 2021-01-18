using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LinkAccountOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private LinkAccountFlags m_LinkAccountFlags;

		private IntPtr m_ContinuanceToken;

		private IntPtr m_LocalUserId;

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

		public LinkAccountFlags LinkAccountFlags
		{
			get
			{
				LinkAccountFlags target = Helper.GetDefault<LinkAccountFlags>();
				Helper.TryMarshalGet(m_LinkAccountFlags, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LinkAccountFlags, value);
			}
		}

		public ContinuanceToken ContinuanceToken
		{
			get
			{
				ContinuanceToken target = Helper.GetDefault<ContinuanceToken>();
				Helper.TryMarshalGet(m_ContinuanceToken, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ContinuanceToken, value);
			}
		}

		public EpicAccountId LocalUserId
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
