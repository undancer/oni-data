using System;

namespace rail
{
	public class rail_api
	{
		public static readonly int USE_MANUAL_ALLOC = RAIL_API_PINVOKE.USE_MANUAL_ALLOC_get();

		public static readonly int RAIL_SDK_PACKING = RAIL_API_PINVOKE.RAIL_SDK_PACKING_get();

		public static uint kRailMaxQuerySpaceWorksLimit => RAIL_API_PINVOKE.kRailMaxQuerySpaceWorksLimit_get();

		public static uint kRailMaxQueryPlayedWithFriendsTimeLimit => RAIL_API_PINVOKE.kRailMaxQueryPlayedWithFriendsTimeLimit_get();

		public static uint kRailRoomDefaultMaxMemberNumber => RAIL_API_PINVOKE.kRailRoomDefaultMaxMemberNumber_get();

		public static uint kRailRoomDataKeyValuePairsLimit => RAIL_API_PINVOKE.kRailRoomDataKeyValuePairsLimit_get();

		public static uint kRailMaxGameDefinePlayingStateValue => RAIL_API_PINVOKE.kRailMaxGameDefinePlayingStateValue_get();

		public static bool RailNeedRestartAppForCheckingEnvironment(RailGameID game_id, int argc, string[] argv)
		{
			IntPtr intPtr = ((game_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailGameID__SWIG_0());
			if (game_id != null)
			{
				RailConverter.Csharp2Cpp(game_id, intPtr);
			}
			try
			{
				return RAIL_API_PINVOKE.RailNeedRestartAppForCheckingEnvironment(intPtr, argc, argv);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailGameID(intPtr);
			}
		}

		public static bool RailInitialize()
		{
			return RAIL_API_PINVOKE.RailInitialize();
		}

		public static void RailFinalize()
		{
			RAIL_API_PINVOKE.RailFinalize();
		}

		public static void RailFireEvents()
		{
			RAIL_API_PINVOKE.RailFireEvents();
		}

		public static IRailFactory RailFactory()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.RailFactory();
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailFactoryImpl(intPtr);
			}
			return null;
		}

		public static void RailGetSdkVersion(out string version, out string description)
		{
			IntPtr jarg = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				RAIL_API_PINVOKE.RailGetSdkVersion(jarg, intPtr);
			}
			finally
			{
				version = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(jarg));
				RAIL_API_PINVOKE.delete_RailString(jarg);
				description = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}
	}
}
