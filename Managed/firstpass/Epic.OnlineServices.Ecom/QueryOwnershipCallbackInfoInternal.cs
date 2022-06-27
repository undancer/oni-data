using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryOwnershipCallbackInfoInternal : ICallbackInfo
	{
		private Result m_ResultCode;

		private IntPtr m_ClientData;

		private IntPtr m_LocalUserId;

		private IntPtr m_ItemOwnership;

		private uint m_ItemOwnershipCount;

		public Result ResultCode
		{
			get
			{
				Result target = Helper.GetDefault<Result>();
				Helper.TryMarshalGet(m_ResultCode, out target);
				return target;
			}
		}

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

		public EpicAccountId LocalUserId
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
		}

		public ItemOwnershipInternal[] ItemOwnership
		{
			get
			{
				ItemOwnershipInternal[] target = Helper.GetDefault<ItemOwnershipInternal[]>();
				Helper.TryMarshalGet(m_ItemOwnership, out target, m_ItemOwnershipCount);
				return target;
			}
		}
	}
}