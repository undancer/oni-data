using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Stats
{
	public sealed class StatsInterface : Handle
	{
		public const int CopystatbynameApiLatest = 1;

		public const int CopystatbyindexApiLatest = 1;

		public const int GetstatcountApiLatest = 1;

		public const int GetstatscountApiLatest = 1;

		public const int StatApiLatest = 1;

		public const int TimeUndefined = -1;

		public const int QuerystatsApiLatest = 2;

		public const int MaxQueryStats = 1000;

		public const int IngeststatApiLatest = 2;

		public const int MaxIngestStats = 3000;

		public const int IngestdataApiLatest = 1;

		public StatsInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void IngestStat(IngestStatOptions options, object clientData, OnIngestStatCompleteCallback completionDelegate)
		{
			IngestStatOptionsInternal options2 = Helper.CopyProperties<IngestStatOptionsInternal>(options);
			OnIngestStatCompleteCallbackInternal onIngestStatCompleteCallbackInternal = OnIngestStatComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onIngestStatCompleteCallbackInternal);
			EOS_Stats_IngestStat(base.InnerHandle, ref options2, clientDataAddress, onIngestStatCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void QueryStats(QueryStatsOptions options, object clientData, OnQueryStatsCompleteCallback completionDelegate)
		{
			QueryStatsOptionsInternal options2 = Helper.CopyProperties<QueryStatsOptionsInternal>(options);
			OnQueryStatsCompleteCallbackInternal onQueryStatsCompleteCallbackInternal = OnQueryStatsComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryStatsCompleteCallbackInternal);
			EOS_Stats_QueryStats(base.InnerHandle, ref options2, clientDataAddress, onQueryStatsCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public uint GetStatsCount(GetStatCountOptions options)
		{
			GetStatCountOptionsInternal options2 = Helper.CopyProperties<GetStatCountOptionsInternal>(options);
			uint source = EOS_Stats_GetStatsCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyStatByIndex(CopyStatByIndexOptions options, out Stat outStat)
		{
			CopyStatByIndexOptionsInternal options2 = Helper.CopyProperties<CopyStatByIndexOptionsInternal>(options);
			outStat = Helper.GetDefault<Stat>();
			IntPtr outStat2 = IntPtr.Zero;
			Result source = EOS_Stats_CopyStatByIndex(base.InnerHandle, ref options2, ref outStat2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<StatInternal, Stat>(outStat2, out outStat))
			{
				EOS_Stats_Stat_Release(outStat2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyStatByName(CopyStatByNameOptions options, out Stat outStat)
		{
			CopyStatByNameOptionsInternal options2 = Helper.CopyProperties<CopyStatByNameOptionsInternal>(options);
			outStat = Helper.GetDefault<Stat>();
			IntPtr outStat2 = IntPtr.Zero;
			Result source = EOS_Stats_CopyStatByName(base.InnerHandle, ref options2, ref outStat2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<StatInternal, Stat>(outStat2, out outStat))
			{
				EOS_Stats_Stat_Release(outStat2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[MonoPInvokeCallback]
		internal static void OnQueryStatsComplete(IntPtr address)
		{
			OnQueryStatsCompleteCallback callback = null;
			OnQueryStatsCompleteCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryStatsCompleteCallback, OnQueryStatsCompleteCallbackInfoInternal, OnQueryStatsCompleteCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnIngestStatComplete(IntPtr address)
		{
			OnIngestStatCompleteCallback callback = null;
			IngestStatCompleteCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnIngestStatCompleteCallback, IngestStatCompleteCallbackInfoInternal, IngestStatCompleteCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Stats_Stat_Release(IntPtr stat);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Stats_CopyStatByName(IntPtr handle, ref CopyStatByNameOptionsInternal options, ref IntPtr outStat);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Stats_CopyStatByIndex(IntPtr handle, ref CopyStatByIndexOptionsInternal options, ref IntPtr outStat);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Stats_GetStatsCount(IntPtr handle, ref GetStatCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Stats_QueryStats(IntPtr handle, ref QueryStatsOptionsInternal options, IntPtr clientData, OnQueryStatsCompleteCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Stats_IngestStat(IntPtr handle, ref IngestStatOptionsInternal options, IntPtr clientData, OnIngestStatCompleteCallbackInternal completionDelegate);
	}
}
