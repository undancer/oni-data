using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct AttributeDataInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Key;

		private AttributeDataValueInternal m_Value;

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

		public string Key
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Key, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Key, value);
			}
		}

		public AttributeDataValueInternal Value
		{
			get
			{
				AttributeDataValueInternal target = Helper.GetDefault<AttributeDataValueInternal>();
				Helper.TryMarshalGet(m_Value, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Value, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_Value);
		}
	}
}
