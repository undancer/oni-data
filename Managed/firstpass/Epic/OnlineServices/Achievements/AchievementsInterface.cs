using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	public sealed class AchievementsInterface : Handle
	{
		public const int AddnotifyachievementsunlockedApiLatest = 1;

		public const int CopyunlockedachievementbyachievementidApiLatest = 1;

		public const int CopyunlockedachievementbyindexApiLatest = 1;

		public const int GetunlockedachievementcountApiLatest = 1;

		public const int UnlockedachievementApiLatest = 1;

		public const int CopydefinitionbyachievementidApiLatest = 1;

		public const int CopydefinitionbyindexApiLatest = 1;

		public const int DefinitionApiLatest = 1;

		public const int Addnotifyachievementsunlockedv2ApiLatest = 2;

		public const int UnlockachievementsApiLatest = 1;

		public const int CopyplayerachievementbyachievementidApiLatest = 1;

		public const int CopyplayerachievementbyindexApiLatest = 1;

		public const int GetplayerachievementcountApiLatest = 1;

		public const int PlayerachievementApiLatest = 2;

		public const int AchievementUnlocktimeUndefined = -1;

		public const int QueryplayerachievementsApiLatest = 1;

		public const int Copydefinitionv2ByachievementidApiLatest = 2;

		public const int Copyachievementdefinitionv2ByachievementidApiLatest = 2;

		public const int Copydefinitionv2ByindexApiLatest = 2;

		public const int Copyachievementdefinitionv2ByindexApiLatest = 2;

		public const int GetachievementdefinitioncountApiLatest = 1;

		public const int Definitionv2ApiLatest = 2;

		public const int PlayerstatinfoApiLatest = 1;

		public const int StatthresholdApiLatest = 1;

		public const int StatthresholdsApiLatest = 1;

		public const int QuerydefinitionsApiLatest = 2;

		public AchievementsInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryDefinitions(QueryDefinitionsOptions options, object clientData, OnQueryDefinitionsCompleteCallback completionDelegate)
		{
			QueryDefinitionsOptionsInternal options2 = Helper.CopyProperties<QueryDefinitionsOptionsInternal>(options);
			OnQueryDefinitionsCompleteCallbackInternal onQueryDefinitionsCompleteCallbackInternal = OnQueryDefinitionsComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryDefinitionsCompleteCallbackInternal);
			EOS_Achievements_QueryDefinitions(base.InnerHandle, ref options2, clientDataAddress, onQueryDefinitionsCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public uint GetAchievementDefinitionCount(GetAchievementDefinitionCountOptions options)
		{
			GetAchievementDefinitionCountOptionsInternal options2 = Helper.CopyProperties<GetAchievementDefinitionCountOptionsInternal>(options);
			uint source = EOS_Achievements_GetAchievementDefinitionCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyAchievementDefinitionV2ByIndex(CopyAchievementDefinitionV2ByIndexOptions options, out DefinitionV2 outDefinition)
		{
			CopyAchievementDefinitionV2ByIndexOptionsInternal options2 = Helper.CopyProperties<CopyAchievementDefinitionV2ByIndexOptionsInternal>(options);
			outDefinition = Helper.GetDefault<DefinitionV2>();
			IntPtr outDefinition2 = IntPtr.Zero;
			Result source = EOS_Achievements_CopyAchievementDefinitionV2ByIndex(base.InnerHandle, ref options2, ref outDefinition2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<DefinitionV2Internal, DefinitionV2>(outDefinition2, out outDefinition))
			{
				EOS_Achievements_DefinitionV2_Release(outDefinition2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyAchievementDefinitionV2ByAchievementId(CopyAchievementDefinitionV2ByAchievementIdOptions options, out DefinitionV2 outDefinition)
		{
			CopyAchievementDefinitionV2ByAchievementIdOptionsInternal options2 = Helper.CopyProperties<CopyAchievementDefinitionV2ByAchievementIdOptionsInternal>(options);
			outDefinition = Helper.GetDefault<DefinitionV2>();
			IntPtr outDefinition2 = IntPtr.Zero;
			Result source = EOS_Achievements_CopyAchievementDefinitionV2ByAchievementId(base.InnerHandle, ref options2, ref outDefinition2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<DefinitionV2Internal, DefinitionV2>(outDefinition2, out outDefinition))
			{
				EOS_Achievements_DefinitionV2_Release(outDefinition2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void QueryPlayerAchievements(QueryPlayerAchievementsOptions options, object clientData, OnQueryPlayerAchievementsCompleteCallback completionDelegate)
		{
			QueryPlayerAchievementsOptionsInternal options2 = Helper.CopyProperties<QueryPlayerAchievementsOptionsInternal>(options);
			OnQueryPlayerAchievementsCompleteCallbackInternal onQueryPlayerAchievementsCompleteCallbackInternal = OnQueryPlayerAchievementsComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryPlayerAchievementsCompleteCallbackInternal);
			EOS_Achievements_QueryPlayerAchievements(base.InnerHandle, ref options2, clientDataAddress, onQueryPlayerAchievementsCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public uint GetPlayerAchievementCount(GetPlayerAchievementCountOptions options)
		{
			GetPlayerAchievementCountOptionsInternal options2 = Helper.CopyProperties<GetPlayerAchievementCountOptionsInternal>(options);
			uint source = EOS_Achievements_GetPlayerAchievementCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyPlayerAchievementByIndex(CopyPlayerAchievementByIndexOptions options, out PlayerAchievement outAchievement)
		{
			CopyPlayerAchievementByIndexOptionsInternal options2 = Helper.CopyProperties<CopyPlayerAchievementByIndexOptionsInternal>(options);
			outAchievement = Helper.GetDefault<PlayerAchievement>();
			IntPtr outAchievement2 = IntPtr.Zero;
			Result source = EOS_Achievements_CopyPlayerAchievementByIndex(base.InnerHandle, ref options2, ref outAchievement2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<PlayerAchievementInternal, PlayerAchievement>(outAchievement2, out outAchievement))
			{
				EOS_Achievements_PlayerAchievement_Release(outAchievement2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyPlayerAchievementByAchievementId(CopyPlayerAchievementByAchievementIdOptions options, out PlayerAchievement outAchievement)
		{
			CopyPlayerAchievementByAchievementIdOptionsInternal options2 = Helper.CopyProperties<CopyPlayerAchievementByAchievementIdOptionsInternal>(options);
			outAchievement = Helper.GetDefault<PlayerAchievement>();
			IntPtr outAchievement2 = IntPtr.Zero;
			Result source = EOS_Achievements_CopyPlayerAchievementByAchievementId(base.InnerHandle, ref options2, ref outAchievement2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<PlayerAchievementInternal, PlayerAchievement>(outAchievement2, out outAchievement))
			{
				EOS_Achievements_PlayerAchievement_Release(outAchievement2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void UnlockAchievements(UnlockAchievementsOptions options, object clientData, OnUnlockAchievementsCompleteCallback completionDelegate)
		{
			UnlockAchievementsOptionsInternal options2 = Helper.CopyProperties<UnlockAchievementsOptionsInternal>(options);
			OnUnlockAchievementsCompleteCallbackInternal onUnlockAchievementsCompleteCallbackInternal = OnUnlockAchievementsComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onUnlockAchievementsCompleteCallbackInternal);
			EOS_Achievements_UnlockAchievements(base.InnerHandle, ref options2, clientDataAddress, onUnlockAchievementsCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public ulong AddNotifyAchievementsUnlockedV2(AddNotifyAchievementsUnlockedV2Options options, object clientData, OnAchievementsUnlockedCallbackV2 notificationFn)
		{
			AddNotifyAchievementsUnlockedV2OptionsInternal options2 = Helper.CopyProperties<AddNotifyAchievementsUnlockedV2OptionsInternal>(options);
			OnAchievementsUnlockedCallbackV2Internal onAchievementsUnlockedCallbackV2Internal = OnAchievementsUnlockedV2;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onAchievementsUnlockedCallbackV2Internal);
			ulong num = EOS_Achievements_AddNotifyAchievementsUnlockedV2(base.InnerHandle, ref options2, clientDataAddress, onAchievementsUnlockedCallbackV2Internal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyAchievementsUnlocked(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Achievements_RemoveNotifyAchievementsUnlocked(base.InnerHandle, inId);
		}

		public Result CopyAchievementDefinitionByIndex(CopyAchievementDefinitionByIndexOptions options, out Definition outDefinition)
		{
			CopyAchievementDefinitionByIndexOptionsInternal options2 = Helper.CopyProperties<CopyAchievementDefinitionByIndexOptionsInternal>(options);
			outDefinition = Helper.GetDefault<Definition>();
			IntPtr outDefinition2 = IntPtr.Zero;
			Result source = EOS_Achievements_CopyAchievementDefinitionByIndex(base.InnerHandle, ref options2, ref outDefinition2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<DefinitionInternal, Definition>(outDefinition2, out outDefinition))
			{
				EOS_Achievements_Definition_Release(outDefinition2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyAchievementDefinitionByAchievementId(CopyAchievementDefinitionByAchievementIdOptions options, out Definition outDefinition)
		{
			CopyAchievementDefinitionByAchievementIdOptionsInternal options2 = Helper.CopyProperties<CopyAchievementDefinitionByAchievementIdOptionsInternal>(options);
			outDefinition = Helper.GetDefault<Definition>();
			IntPtr outDefinition2 = IntPtr.Zero;
			Result source = EOS_Achievements_CopyAchievementDefinitionByAchievementId(base.InnerHandle, ref options2, ref outDefinition2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<DefinitionInternal, Definition>(outDefinition2, out outDefinition))
			{
				EOS_Achievements_Definition_Release(outDefinition2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetUnlockedAchievementCount(GetUnlockedAchievementCountOptions options)
		{
			GetUnlockedAchievementCountOptionsInternal options2 = Helper.CopyProperties<GetUnlockedAchievementCountOptionsInternal>(options);
			uint source = EOS_Achievements_GetUnlockedAchievementCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyUnlockedAchievementByIndex(CopyUnlockedAchievementByIndexOptions options, out UnlockedAchievement outAchievement)
		{
			CopyUnlockedAchievementByIndexOptionsInternal options2 = Helper.CopyProperties<CopyUnlockedAchievementByIndexOptionsInternal>(options);
			outAchievement = Helper.GetDefault<UnlockedAchievement>();
			IntPtr outAchievement2 = IntPtr.Zero;
			Result source = EOS_Achievements_CopyUnlockedAchievementByIndex(base.InnerHandle, ref options2, ref outAchievement2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<UnlockedAchievementInternal, UnlockedAchievement>(outAchievement2, out outAchievement))
			{
				EOS_Achievements_UnlockedAchievement_Release(outAchievement2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyUnlockedAchievementByAchievementId(CopyUnlockedAchievementByAchievementIdOptions options, out UnlockedAchievement outAchievement)
		{
			CopyUnlockedAchievementByAchievementIdOptionsInternal options2 = Helper.CopyProperties<CopyUnlockedAchievementByAchievementIdOptionsInternal>(options);
			outAchievement = Helper.GetDefault<UnlockedAchievement>();
			IntPtr outAchievement2 = IntPtr.Zero;
			Result source = EOS_Achievements_CopyUnlockedAchievementByAchievementId(base.InnerHandle, ref options2, ref outAchievement2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<UnlockedAchievementInternal, UnlockedAchievement>(outAchievement2, out outAchievement))
			{
				EOS_Achievements_UnlockedAchievement_Release(outAchievement2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public ulong AddNotifyAchievementsUnlocked(AddNotifyAchievementsUnlockedOptions options, object clientData, OnAchievementsUnlockedCallback notificationFn)
		{
			AddNotifyAchievementsUnlockedOptionsInternal options2 = Helper.CopyProperties<AddNotifyAchievementsUnlockedOptionsInternal>(options);
			OnAchievementsUnlockedCallbackInternal onAchievementsUnlockedCallbackInternal = OnAchievementsUnlocked;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onAchievementsUnlockedCallbackInternal);
			ulong num = EOS_Achievements_AddNotifyAchievementsUnlocked(base.InnerHandle, ref options2, clientDataAddress, onAchievementsUnlockedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		[MonoPInvokeCallback]
		internal static void OnAchievementsUnlocked(IntPtr address)
		{
			OnAchievementsUnlockedCallback callback = null;
			OnAchievementsUnlockedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnAchievementsUnlockedCallback, OnAchievementsUnlockedCallbackInfoInternal, OnAchievementsUnlockedCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnAchievementsUnlockedV2(IntPtr address)
		{
			OnAchievementsUnlockedCallbackV2 callback = null;
			OnAchievementsUnlockedCallbackV2Info callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnAchievementsUnlockedCallbackV2, OnAchievementsUnlockedCallbackV2InfoInternal, OnAchievementsUnlockedCallbackV2Info>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnUnlockAchievementsComplete(IntPtr address)
		{
			OnUnlockAchievementsCompleteCallback callback = null;
			OnUnlockAchievementsCompleteCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnUnlockAchievementsCompleteCallback, OnUnlockAchievementsCompleteCallbackInfoInternal, OnUnlockAchievementsCompleteCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryPlayerAchievementsComplete(IntPtr address)
		{
			OnQueryPlayerAchievementsCompleteCallback callback = null;
			OnQueryPlayerAchievementsCompleteCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryPlayerAchievementsCompleteCallback, OnQueryPlayerAchievementsCompleteCallbackInfoInternal, OnQueryPlayerAchievementsCompleteCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryDefinitionsComplete(IntPtr address)
		{
			OnQueryDefinitionsCompleteCallback callback = null;
			OnQueryDefinitionsCompleteCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryDefinitionsCompleteCallback, OnQueryDefinitionsCompleteCallbackInfoInternal, OnQueryDefinitionsCompleteCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Achievements_UnlockedAchievement_Release(IntPtr achievement);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Achievements_Definition_Release(IntPtr achievementDefinition);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Achievements_PlayerAchievement_Release(IntPtr achievement);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Achievements_DefinitionV2_Release(IntPtr achievementDefinition);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Achievements_AddNotifyAchievementsUnlocked(IntPtr handle, ref AddNotifyAchievementsUnlockedOptionsInternal options, IntPtr clientData, OnAchievementsUnlockedCallbackInternal notificationFn);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Achievements_CopyUnlockedAchievementByAchievementId(IntPtr handle, ref CopyUnlockedAchievementByAchievementIdOptionsInternal options, ref IntPtr outAchievement);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Achievements_CopyUnlockedAchievementByIndex(IntPtr handle, ref CopyUnlockedAchievementByIndexOptionsInternal options, ref IntPtr outAchievement);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Achievements_GetUnlockedAchievementCount(IntPtr handle, ref GetUnlockedAchievementCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Achievements_CopyAchievementDefinitionByAchievementId(IntPtr handle, ref CopyAchievementDefinitionByAchievementIdOptionsInternal options, ref IntPtr outDefinition);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Achievements_CopyAchievementDefinitionByIndex(IntPtr handle, ref CopyAchievementDefinitionByIndexOptionsInternal options, ref IntPtr outDefinition);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Achievements_RemoveNotifyAchievementsUnlocked(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Achievements_AddNotifyAchievementsUnlockedV2(IntPtr handle, ref AddNotifyAchievementsUnlockedV2OptionsInternal options, IntPtr clientData, OnAchievementsUnlockedCallbackV2Internal notificationFn);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Achievements_UnlockAchievements(IntPtr handle, ref UnlockAchievementsOptionsInternal options, IntPtr clientData, OnUnlockAchievementsCompleteCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Achievements_CopyPlayerAchievementByAchievementId(IntPtr handle, ref CopyPlayerAchievementByAchievementIdOptionsInternal options, ref IntPtr outAchievement);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Achievements_CopyPlayerAchievementByIndex(IntPtr handle, ref CopyPlayerAchievementByIndexOptionsInternal options, ref IntPtr outAchievement);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Achievements_GetPlayerAchievementCount(IntPtr handle, ref GetPlayerAchievementCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Achievements_QueryPlayerAchievements(IntPtr handle, ref QueryPlayerAchievementsOptionsInternal options, IntPtr clientData, OnQueryPlayerAchievementsCompleteCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Achievements_CopyAchievementDefinitionV2ByAchievementId(IntPtr handle, ref CopyAchievementDefinitionV2ByAchievementIdOptionsInternal options, ref IntPtr outDefinition);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Achievements_CopyAchievementDefinitionV2ByIndex(IntPtr handle, ref CopyAchievementDefinitionV2ByIndexOptionsInternal options, ref IntPtr outDefinition);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Achievements_GetAchievementDefinitionCount(IntPtr handle, ref GetAchievementDefinitionCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Achievements_QueryDefinitions(IntPtr handle, ref QueryDefinitionsOptionsInternal options, IntPtr clientData, OnQueryDefinitionsCompleteCallbackInternal completionDelegate);
	}
}
