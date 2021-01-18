using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Platform
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct InitializeOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private AllocateMemoryFunc m_AllocateMemoryFunction;

		private ReallocateMemoryFunc m_ReallocateMemoryFunction;

		private ReleaseMemoryFunc m_ReleaseMemoryFunction;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ProductName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ProductVersion;

		private IntPtr m_Reserved;

		private IntPtr m_SystemInitializeOptions;

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

		public AllocateMemoryFunc AllocateMemoryFunction
		{
			get
			{
				AllocateMemoryFunc target = Helper.GetDefault<AllocateMemoryFunc>();
				Helper.TryMarshalGet(m_AllocateMemoryFunction, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AllocateMemoryFunction, value);
			}
		}

		public ReallocateMemoryFunc ReallocateMemoryFunction
		{
			get
			{
				ReallocateMemoryFunc target = Helper.GetDefault<ReallocateMemoryFunc>();
				Helper.TryMarshalGet(m_ReallocateMemoryFunction, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ReallocateMemoryFunction, value);
			}
		}

		public ReleaseMemoryFunc ReleaseMemoryFunction
		{
			get
			{
				ReleaseMemoryFunc target = Helper.GetDefault<ReleaseMemoryFunc>();
				Helper.TryMarshalGet(m_ReleaseMemoryFunction, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ReleaseMemoryFunction, value);
			}
		}

		public string ProductName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_ProductName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ProductName, value);
			}
		}

		public string ProductVersion
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_ProductVersion, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ProductVersion, value);
			}
		}

		public IntPtr Reserved
		{
			get
			{
				IntPtr target = Helper.GetDefault<IntPtr>();
				Helper.TryMarshalGet(m_Reserved, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Reserved, value);
			}
		}

		public IntPtr SystemInitializeOptions
		{
			get
			{
				IntPtr target = Helper.GetDefault<IntPtr>();
				Helper.TryMarshalGet(m_SystemInitializeOptions, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_SystemInitializeOptions, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
