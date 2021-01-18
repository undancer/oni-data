using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UI
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct OnDisplaySettingsUpdatedCallbackInfoInternal : ICallbackInfo
	{
		private IntPtr m_ClientData;

		private int m_IsVisible;

		private int m_IsExclusiveInput;

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

		public bool IsVisible
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_IsVisible, out target);
				return target;
			}
		}

		public bool IsExclusiveInput
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_IsExclusiveInput, out target);
				return target;
			}
		}
	}
}
