using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CatalogReleaseInternal : IDisposable
	{
		private int m_ApiVersion;

		private uint m_CompatibleAppIdCount;

		private IntPtr m_CompatibleAppIds;

		private uint m_CompatiblePlatformCount;

		private IntPtr m_CompatiblePlatforms;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ReleaseNote;

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

		public string[] CompatibleAppIds
		{
			get
			{
				string[] target = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet(m_CompatibleAppIds, out target, m_CompatibleAppIdCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_CompatibleAppIds, value, out m_CompatibleAppIdCount);
			}
		}

		public string[] CompatiblePlatforms
		{
			get
			{
				string[] target = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet(m_CompatiblePlatforms, out target, m_CompatiblePlatformCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_CompatiblePlatforms, value, out m_CompatiblePlatformCount);
			}
		}

		public string ReleaseNote
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_ReleaseNote, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ReleaseNote, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_CompatibleAppIds);
			Helper.TryMarshalDispose(ref m_CompatiblePlatforms);
		}
	}
}
