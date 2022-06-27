using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	public sealed class SessionDetails : Handle
	{
		public SessionDetails(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result CopyInfo(SessionDetailsCopyInfoOptions options, out SessionDetailsInfo outSessionInfo)
		{
			SessionDetailsCopyInfoOptionsInternal options2 = Helper.CopyProperties<SessionDetailsCopyInfoOptionsInternal>(options);
			outSessionInfo = Helper.GetDefault<SessionDetailsInfo>();
			IntPtr outSessionInfo2 = IntPtr.Zero;
			Result source = EOS_SessionDetails_CopyInfo(base.InnerHandle, ref options2, ref outSessionInfo2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<SessionDetailsInfoInternal, SessionDetailsInfo>(outSessionInfo2, out outSessionInfo))
			{
				EOS_SessionDetails_Info_Release(outSessionInfo2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetSessionAttributeCount(SessionDetailsGetSessionAttributeCountOptions options)
		{
			SessionDetailsGetSessionAttributeCountOptionsInternal options2 = Helper.CopyProperties<SessionDetailsGetSessionAttributeCountOptionsInternal>(options);
			uint source = EOS_SessionDetails_GetSessionAttributeCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopySessionAttributeByIndex(SessionDetailsCopySessionAttributeByIndexOptions options, out SessionDetailsAttribute outSessionAttribute)
		{
			SessionDetailsCopySessionAttributeByIndexOptionsInternal options2 = Helper.CopyProperties<SessionDetailsCopySessionAttributeByIndexOptionsInternal>(options);
			outSessionAttribute = Helper.GetDefault<SessionDetailsAttribute>();
			IntPtr outSessionAttribute2 = IntPtr.Zero;
			Result source = EOS_SessionDetails_CopySessionAttributeByIndex(base.InnerHandle, ref options2, ref outSessionAttribute2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<SessionDetailsAttributeInternal, SessionDetailsAttribute>(outSessionAttribute2, out outSessionAttribute))
			{
				EOS_SessionDetails_Attribute_Release(outSessionAttribute2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopySessionAttributeByKey(SessionDetailsCopySessionAttributeByKeyOptions options, out SessionDetailsAttribute outSessionAttribute)
		{
			SessionDetailsCopySessionAttributeByKeyOptionsInternal options2 = Helper.CopyProperties<SessionDetailsCopySessionAttributeByKeyOptionsInternal>(options);
			outSessionAttribute = Helper.GetDefault<SessionDetailsAttribute>();
			IntPtr outSessionAttribute2 = IntPtr.Zero;
			Result source = EOS_SessionDetails_CopySessionAttributeByKey(base.InnerHandle, ref options2, ref outSessionAttribute2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<SessionDetailsAttributeInternal, SessionDetailsAttribute>(outSessionAttribute2, out outSessionAttribute))
			{
				EOS_SessionDetails_Attribute_Release(outSessionAttribute2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void Release()
		{
			EOS_SessionDetails_Release(base.InnerHandle);
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_SessionDetails_Info_Release(IntPtr sessionInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_SessionDetails_Attribute_Release(IntPtr sessionAttribute);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_SessionDetails_Release(IntPtr sessionHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_SessionDetails_CopySessionAttributeByKey(IntPtr handle, ref SessionDetailsCopySessionAttributeByKeyOptionsInternal options, ref IntPtr outSessionAttribute);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_SessionDetails_CopySessionAttributeByIndex(IntPtr handle, ref SessionDetailsCopySessionAttributeByIndexOptionsInternal options, ref IntPtr outSessionAttribute);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_SessionDetails_GetSessionAttributeCount(IntPtr handle, ref SessionDetailsGetSessionAttributeCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_SessionDetails_CopyInfo(IntPtr handle, ref SessionDetailsCopyInfoOptionsInternal options, ref IntPtr outSessionInfo);
	}
}
