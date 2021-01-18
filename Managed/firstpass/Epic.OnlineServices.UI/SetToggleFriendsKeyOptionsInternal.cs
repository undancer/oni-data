using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UI
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SetToggleFriendsKeyOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private KeyCombination m_KeyCombination;

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

		public KeyCombination KeyCombination
		{
			get
			{
				KeyCombination target = Helper.GetDefault<KeyCombination>();
				Helper.TryMarshalGet(m_KeyCombination, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_KeyCombination, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
