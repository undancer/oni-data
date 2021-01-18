using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Leaderboards
{
	public sealed class LeaderboardsInterface : Handle
	{
		public const int CopyleaderboardrecordbyuseridApiLatest = 2;

		public const int CopyleaderboardrecordbyindexApiLatest = 2;

		public const int GetleaderboardrecordcountApiLatest = 1;

		public const int LeaderboardrecordApiLatest = 2;

		public const int QueryleaderboardranksApiLatest = 1;

		public const int CopyleaderboarduserscorebyuseridApiLatest = 1;

		public const int CopyleaderboarduserscorebyindexApiLatest = 1;

		public const int GetleaderboarduserscorecountApiLatest = 1;

		public const int LeaderboarduserscoreApiLatest = 1;

		public const int QueryleaderboarduserscoresApiLatest = 1;

		public const int UserscoresquerystatinfoApiLatest = 1;

		public const int CopyleaderboarddefinitionbyleaderboardidApiLatest = 1;

		public const int CopyleaderboarddefinitionbyindexApiLatest = 1;

		public const int GetleaderboarddefinitioncountApiLatest = 1;

		public const int DefinitionApiLatest = 1;

		public const int QueryleaderboarddefinitionsApiLatest = 1;

		public const int TimeUndefined = -1;

		public LeaderboardsInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryLeaderboardDefinitions(QueryLeaderboardDefinitionsOptions options, object clientData, OnQueryLeaderboardDefinitionsCompleteCallback completionDelegate)
		{
			QueryLeaderboardDefinitionsOptionsInternal options2 = Helper.CopyProperties<QueryLeaderboardDefinitionsOptionsInternal>(options);
			OnQueryLeaderboardDefinitionsCompleteCallbackInternal onQueryLeaderboardDefinitionsCompleteCallbackInternal = OnQueryLeaderboardDefinitionsComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryLeaderboardDefinitionsCompleteCallbackInternal);
			EOS_Leaderboards_QueryLeaderboardDefinitions(base.InnerHandle, ref options2, clientDataAddress, onQueryLeaderboardDefinitionsCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public uint GetLeaderboardDefinitionCount(GetLeaderboardDefinitionCountOptions options)
		{
			GetLeaderboardDefinitionCountOptionsInternal options2 = Helper.CopyProperties<GetLeaderboardDefinitionCountOptionsInternal>(options);
			uint source = EOS_Leaderboards_GetLeaderboardDefinitionCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyLeaderboardDefinitionByIndex(CopyLeaderboardDefinitionByIndexOptions options, out Definition outLeaderboardDefinition)
		{
			CopyLeaderboardDefinitionByIndexOptionsInternal options2 = Helper.CopyProperties<CopyLeaderboardDefinitionByIndexOptionsInternal>(options);
			outLeaderboardDefinition = Helper.GetDefault<Definition>();
			IntPtr outLeaderboardDefinition2 = IntPtr.Zero;
			Result source = EOS_Leaderboards_CopyLeaderboardDefinitionByIndex(base.InnerHandle, ref options2, ref outLeaderboardDefinition2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<DefinitionInternal, Definition>(outLeaderboardDefinition2, out outLeaderboardDefinition))
			{
				EOS_Leaderboards_LeaderboardDefinition_Release(outLeaderboardDefinition2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyLeaderboardDefinitionByLeaderboardId(CopyLeaderboardDefinitionByLeaderboardIdOptions options, out Definition outLeaderboardDefinition)
		{
			CopyLeaderboardDefinitionByLeaderboardIdOptionsInternal options2 = Helper.CopyProperties<CopyLeaderboardDefinitionByLeaderboardIdOptionsInternal>(options);
			outLeaderboardDefinition = Helper.GetDefault<Definition>();
			IntPtr outLeaderboardDefinition2 = IntPtr.Zero;
			Result source = EOS_Leaderboards_CopyLeaderboardDefinitionByLeaderboardId(base.InnerHandle, ref options2, ref outLeaderboardDefinition2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<DefinitionInternal, Definition>(outLeaderboardDefinition2, out outLeaderboardDefinition))
			{
				EOS_Leaderboards_LeaderboardDefinition_Release(outLeaderboardDefinition2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void QueryLeaderboardRanks(QueryLeaderboardRanksOptions options, object clientData, OnQueryLeaderboardRanksCompleteCallback completionDelegate)
		{
			QueryLeaderboardRanksOptionsInternal options2 = Helper.CopyProperties<QueryLeaderboardRanksOptionsInternal>(options);
			OnQueryLeaderboardRanksCompleteCallbackInternal onQueryLeaderboardRanksCompleteCallbackInternal = OnQueryLeaderboardRanksComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryLeaderboardRanksCompleteCallbackInternal);
			EOS_Leaderboards_QueryLeaderboardRanks(base.InnerHandle, ref options2, clientDataAddress, onQueryLeaderboardRanksCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public uint GetLeaderboardRecordCount(GetLeaderboardRecordCountOptions options)
		{
			GetLeaderboardRecordCountOptionsInternal options2 = Helper.CopyProperties<GetLeaderboardRecordCountOptionsInternal>(options);
			uint source = EOS_Leaderboards_GetLeaderboardRecordCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyLeaderboardRecordByIndex(CopyLeaderboardRecordByIndexOptions options, out LeaderboardRecord outLeaderboardRecord)
		{
			CopyLeaderboardRecordByIndexOptionsInternal options2 = Helper.CopyProperties<CopyLeaderboardRecordByIndexOptionsInternal>(options);
			outLeaderboardRecord = Helper.GetDefault<LeaderboardRecord>();
			IntPtr outLeaderboardRecord2 = IntPtr.Zero;
			Result source = EOS_Leaderboards_CopyLeaderboardRecordByIndex(base.InnerHandle, ref options2, ref outLeaderboardRecord2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<LeaderboardRecordInternal, LeaderboardRecord>(outLeaderboardRecord2, out outLeaderboardRecord))
			{
				EOS_Leaderboards_LeaderboardRecord_Release(outLeaderboardRecord2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyLeaderboardRecordByUserId(CopyLeaderboardRecordByUserIdOptions options, out LeaderboardRecord outLeaderboardRecord)
		{
			CopyLeaderboardRecordByUserIdOptionsInternal options2 = Helper.CopyProperties<CopyLeaderboardRecordByUserIdOptionsInternal>(options);
			outLeaderboardRecord = Helper.GetDefault<LeaderboardRecord>();
			IntPtr outLeaderboardRecord2 = IntPtr.Zero;
			Result source = EOS_Leaderboards_CopyLeaderboardRecordByUserId(base.InnerHandle, ref options2, ref outLeaderboardRecord2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<LeaderboardRecordInternal, LeaderboardRecord>(outLeaderboardRecord2, out outLeaderboardRecord))
			{
				EOS_Leaderboards_LeaderboardRecord_Release(outLeaderboardRecord2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void QueryLeaderboardUserScores(QueryLeaderboardUserScoresOptions options, object clientData, OnQueryLeaderboardUserScoresCompleteCallback completionDelegate)
		{
			QueryLeaderboardUserScoresOptionsInternal options2 = Helper.CopyProperties<QueryLeaderboardUserScoresOptionsInternal>(options);
			OnQueryLeaderboardUserScoresCompleteCallbackInternal onQueryLeaderboardUserScoresCompleteCallbackInternal = OnQueryLeaderboardUserScoresComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryLeaderboardUserScoresCompleteCallbackInternal);
			EOS_Leaderboards_QueryLeaderboardUserScores(base.InnerHandle, ref options2, clientDataAddress, onQueryLeaderboardUserScoresCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public uint GetLeaderboardUserScoreCount(GetLeaderboardUserScoreCountOptions options)
		{
			GetLeaderboardUserScoreCountOptionsInternal options2 = Helper.CopyProperties<GetLeaderboardUserScoreCountOptionsInternal>(options);
			uint source = EOS_Leaderboards_GetLeaderboardUserScoreCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyLeaderboardUserScoreByIndex(CopyLeaderboardUserScoreByIndexOptions options, out LeaderboardUserScore outLeaderboardUserScore)
		{
			CopyLeaderboardUserScoreByIndexOptionsInternal options2 = Helper.CopyProperties<CopyLeaderboardUserScoreByIndexOptionsInternal>(options);
			outLeaderboardUserScore = Helper.GetDefault<LeaderboardUserScore>();
			IntPtr outLeaderboardUserScore2 = IntPtr.Zero;
			Result source = EOS_Leaderboards_CopyLeaderboardUserScoreByIndex(base.InnerHandle, ref options2, ref outLeaderboardUserScore2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<LeaderboardUserScoreInternal, LeaderboardUserScore>(outLeaderboardUserScore2, out outLeaderboardUserScore))
			{
				EOS_Leaderboards_LeaderboardUserScore_Release(outLeaderboardUserScore2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyLeaderboardUserScoreByUserId(CopyLeaderboardUserScoreByUserIdOptions options, out LeaderboardUserScore outLeaderboardUserScore)
		{
			CopyLeaderboardUserScoreByUserIdOptionsInternal options2 = Helper.CopyProperties<CopyLeaderboardUserScoreByUserIdOptionsInternal>(options);
			outLeaderboardUserScore = Helper.GetDefault<LeaderboardUserScore>();
			IntPtr outLeaderboardUserScore2 = IntPtr.Zero;
			Result source = EOS_Leaderboards_CopyLeaderboardUserScoreByUserId(base.InnerHandle, ref options2, ref outLeaderboardUserScore2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<LeaderboardUserScoreInternal, LeaderboardUserScore>(outLeaderboardUserScore2, out outLeaderboardUserScore))
			{
				EOS_Leaderboards_LeaderboardUserScore_Release(outLeaderboardUserScore2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[MonoPInvokeCallback]
		internal static void OnQueryLeaderboardUserScoresComplete(IntPtr address)
		{
			OnQueryLeaderboardUserScoresCompleteCallback callback = null;
			OnQueryLeaderboardUserScoresCompleteCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryLeaderboardUserScoresCompleteCallback, OnQueryLeaderboardUserScoresCompleteCallbackInfoInternal, OnQueryLeaderboardUserScoresCompleteCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryLeaderboardRanksComplete(IntPtr address)
		{
			OnQueryLeaderboardRanksCompleteCallback callback = null;
			OnQueryLeaderboardRanksCompleteCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryLeaderboardRanksCompleteCallback, OnQueryLeaderboardRanksCompleteCallbackInfoInternal, OnQueryLeaderboardRanksCompleteCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryLeaderboardDefinitionsComplete(IntPtr address)
		{
			OnQueryLeaderboardDefinitionsCompleteCallback callback = null;
			OnQueryLeaderboardDefinitionsCompleteCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryLeaderboardDefinitionsCompleteCallback, OnQueryLeaderboardDefinitionsCompleteCallbackInfoInternal, OnQueryLeaderboardDefinitionsCompleteCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Leaderboards_LeaderboardDefinition_Release(IntPtr leaderboardDefinition);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Leaderboards_LeaderboardRecord_Release(IntPtr leaderboardRecord);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Leaderboards_LeaderboardUserScore_Release(IntPtr leaderboardUserScore);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Leaderboards_Definition_Release(IntPtr leaderboardDefinition);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Leaderboards_CopyLeaderboardUserScoreByUserId(IntPtr handle, ref CopyLeaderboardUserScoreByUserIdOptionsInternal options, ref IntPtr outLeaderboardUserScore);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Leaderboards_CopyLeaderboardUserScoreByIndex(IntPtr handle, ref CopyLeaderboardUserScoreByIndexOptionsInternal options, ref IntPtr outLeaderboardUserScore);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Leaderboards_GetLeaderboardUserScoreCount(IntPtr handle, ref GetLeaderboardUserScoreCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Leaderboards_QueryLeaderboardUserScores(IntPtr handle, ref QueryLeaderboardUserScoresOptionsInternal options, IntPtr clientData, OnQueryLeaderboardUserScoresCompleteCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Leaderboards_CopyLeaderboardRecordByUserId(IntPtr handle, ref CopyLeaderboardRecordByUserIdOptionsInternal options, ref IntPtr outLeaderboardRecord);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Leaderboards_CopyLeaderboardRecordByIndex(IntPtr handle, ref CopyLeaderboardRecordByIndexOptionsInternal options, ref IntPtr outLeaderboardRecord);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Leaderboards_GetLeaderboardRecordCount(IntPtr handle, ref GetLeaderboardRecordCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Leaderboards_QueryLeaderboardRanks(IntPtr handle, ref QueryLeaderboardRanksOptionsInternal options, IntPtr clientData, OnQueryLeaderboardRanksCompleteCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Leaderboards_CopyLeaderboardDefinitionByLeaderboardId(IntPtr handle, ref CopyLeaderboardDefinitionByLeaderboardIdOptionsInternal options, ref IntPtr outLeaderboardDefinition);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Leaderboards_CopyLeaderboardDefinitionByIndex(IntPtr handle, ref CopyLeaderboardDefinitionByIndexOptionsInternal options, ref IntPtr outLeaderboardDefinition);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Leaderboards_GetLeaderboardDefinitionCount(IntPtr handle, ref GetLeaderboardDefinitionCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Leaderboards_QueryLeaderboardDefinitions(IntPtr handle, ref QueryLeaderboardDefinitionsOptionsInternal options, IntPtr clientData, OnQueryLeaderboardDefinitionsCompleteCallbackInternal completionDelegate);
	}
}
