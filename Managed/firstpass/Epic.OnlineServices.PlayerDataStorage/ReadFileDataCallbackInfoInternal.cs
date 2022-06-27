using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.PlayerDataStorage
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ReadFileDataCallbackInfoInternal : ICallbackInfo
	{
		private IntPtr m_ClientData;

		private IntPtr m_LocalUserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Filename;

		private uint m_TotalFileSizeBytes;

		private int m_IsLastChunk;

		private uint m_DataChunkLengthBytes;

		private IntPtr m_DataChunk;

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

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
		}

		public string Filename
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Filename, out target);
				return target;
			}
		}

		public uint TotalFileSizeBytes
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_TotalFileSizeBytes, out target);
				return target;
			}
		}

		public bool IsLastChunk
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_IsLastChunk, out target);
				return target;
			}
		}

		public byte[] DataChunk
		{
			get
			{
				byte[] target = Helper.GetDefault<byte[]>();
				Helper.TryMarshalGet(m_DataChunk, out target, m_DataChunkLengthBytes);
				return target;
			}
		}
	}
}
