using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PresenceModificationSetRawRichTextOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_RichText;

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

		public string RichText
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_RichText, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_RichText, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
