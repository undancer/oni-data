using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbySearchRemoveParameterOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Key;

		private ComparisonOp m_ComparisonOp;

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

		public ComparisonOp ComparisonOp
		{
			get
			{
				ComparisonOp target = Helper.GetDefault<ComparisonOp>();
				Helper.TryMarshalGet(m_ComparisonOp, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ComparisonOp, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
