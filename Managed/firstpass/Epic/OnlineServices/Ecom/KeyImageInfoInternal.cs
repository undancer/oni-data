using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct KeyImageInfoInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Type;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Url;

		private uint m_Width;

		private uint m_Height;

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

		public string Type
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Type, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Type, value);
			}
		}

		public string Url
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Url, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Url, value);
			}
		}

		public uint Width
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_Width, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Width, value);
			}
		}

		public uint Height
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_Height, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Height, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
