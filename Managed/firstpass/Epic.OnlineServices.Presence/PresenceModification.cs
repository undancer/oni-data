using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	public sealed class PresenceModification : Handle
	{
		public PresenceModification(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result SetStatus(PresenceModificationSetStatusOptions options)
		{
			PresenceModificationSetStatusOptionsInternal options2 = Helper.CopyProperties<PresenceModificationSetStatusOptionsInternal>(options);
			Result source = EOS_PresenceModification_SetStatus(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetRawRichText(PresenceModificationSetRawRichTextOptions options)
		{
			PresenceModificationSetRawRichTextOptionsInternal options2 = Helper.CopyProperties<PresenceModificationSetRawRichTextOptionsInternal>(options);
			Result source = EOS_PresenceModification_SetRawRichText(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetData(PresenceModificationSetDataOptions options)
		{
			PresenceModificationSetDataOptionsInternal options2 = Helper.CopyProperties<PresenceModificationSetDataOptionsInternal>(options);
			Result source = EOS_PresenceModification_SetData(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result DeleteData(PresenceModificationDeleteDataOptions options)
		{
			PresenceModificationDeleteDataOptionsInternal options2 = Helper.CopyProperties<PresenceModificationDeleteDataOptionsInternal>(options);
			Result source = EOS_PresenceModification_DeleteData(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetJoinInfo(PresenceModificationSetJoinInfoOptions options)
		{
			PresenceModificationSetJoinInfoOptionsInternal options2 = Helper.CopyProperties<PresenceModificationSetJoinInfoOptionsInternal>(options);
			Result source = EOS_PresenceModification_SetJoinInfo(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void Release()
		{
			EOS_PresenceModification_Release(base.InnerHandle);
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_PresenceModification_Release(IntPtr presenceModificationHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_PresenceModification_SetJoinInfo(IntPtr handle, ref PresenceModificationSetJoinInfoOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_PresenceModification_DeleteData(IntPtr handle, ref PresenceModificationDeleteDataOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_PresenceModification_SetData(IntPtr handle, ref PresenceModificationSetDataOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_PresenceModification_SetRawRichText(IntPtr handle, ref PresenceModificationSetRawRichTextOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_PresenceModification_SetStatus(IntPtr handle, ref PresenceModificationSetStatusOptionsInternal options);
	}
}
