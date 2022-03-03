using System;
using System.IO;
using System.Runtime.InteropServices;

namespace rail
{
	internal class RAIL_API_PINVOKE
	{
		protected class SWIGExceptionHelper
		{
			public delegate void ExceptionDelegate(string message);

			public delegate void ExceptionArgumentDelegate(string message, string paramName);

			private static ExceptionDelegate applicationDelegate;

			private static ExceptionDelegate arithmeticDelegate;

			private static ExceptionDelegate divideByZeroDelegate;

			private static ExceptionDelegate indexOutOfRangeDelegate;

			private static ExceptionDelegate invalidCastDelegate;

			private static ExceptionDelegate invalidOperationDelegate;

			private static ExceptionDelegate ioDelegate;

			private static ExceptionDelegate nullReferenceDelegate;

			private static ExceptionDelegate outOfMemoryDelegate;

			private static ExceptionDelegate overflowDelegate;

			private static ExceptionDelegate systemDelegate;

			private static ExceptionArgumentDelegate argumentDelegate;

			private static ExceptionArgumentDelegate argumentNullDelegate;

			private static ExceptionArgumentDelegate argumentOutOfRangeDelegate;

			[DllImport("rail_api64")]
			public static extern void SWIGRegisterExceptionCallbacks_rail_api(ExceptionDelegate applicationDelegate, ExceptionDelegate arithmeticDelegate, ExceptionDelegate divideByZeroDelegate, ExceptionDelegate indexOutOfRangeDelegate, ExceptionDelegate invalidCastDelegate, ExceptionDelegate invalidOperationDelegate, ExceptionDelegate ioDelegate, ExceptionDelegate nullReferenceDelegate, ExceptionDelegate outOfMemoryDelegate, ExceptionDelegate overflowDelegate, ExceptionDelegate systemExceptionDelegate);

			[DllImport("rail_api64", EntryPoint = "SWIGRegisterExceptionArgumentCallbacks_rail_api")]
			public static extern void SWIGRegisterExceptionCallbacksArgument_rail_api(ExceptionArgumentDelegate argumentDelegate, ExceptionArgumentDelegate argumentNullDelegate, ExceptionArgumentDelegate argumentOutOfRangeDelegate);

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingApplicationException(string message)
			{
				SWIGPendingException.Set(new ApplicationException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingArithmeticException(string message)
			{
				SWIGPendingException.Set(new ArithmeticException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingDivideByZeroException(string message)
			{
				SWIGPendingException.Set(new DivideByZeroException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingIndexOutOfRangeException(string message)
			{
				SWIGPendingException.Set(new IndexOutOfRangeException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingInvalidCastException(string message)
			{
				SWIGPendingException.Set(new InvalidCastException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingInvalidOperationException(string message)
			{
				SWIGPendingException.Set(new InvalidOperationException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingIOException(string message)
			{
				SWIGPendingException.Set(new IOException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingNullReferenceException(string message)
			{
				SWIGPendingException.Set(new NullReferenceException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingOutOfMemoryException(string message)
			{
				SWIGPendingException.Set(new OutOfMemoryException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingOverflowException(string message)
			{
				SWIGPendingException.Set(new OverflowException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingSystemException(string message)
			{
				SWIGPendingException.Set(new SystemException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingArgumentException(string message, string paramName)
			{
				SWIGPendingException.Set(new ArgumentException(message, paramName, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingArgumentNullException(string message, string paramName)
			{
				Exception ex = SWIGPendingException.Retrieve();
				if (ex != null)
				{
					message = message + " Inner Exception: " + ex.Message;
				}
				SWIGPendingException.Set(new ArgumentNullException(paramName, message));
			}

			[MonoPInvokeCallback(typeof(ExceptionDelegate))]
			private static void SetPendingArgumentOutOfRangeException(string message, string paramName)
			{
				Exception ex = SWIGPendingException.Retrieve();
				if (ex != null)
				{
					message = message + " Inner Exception: " + ex.Message;
				}
				SWIGPendingException.Set(new ArgumentOutOfRangeException(paramName, message));
			}

			static SWIGExceptionHelper()
			{
				applicationDelegate = SetPendingApplicationException;
				arithmeticDelegate = SetPendingArithmeticException;
				divideByZeroDelegate = SetPendingDivideByZeroException;
				indexOutOfRangeDelegate = SetPendingIndexOutOfRangeException;
				invalidCastDelegate = SetPendingInvalidCastException;
				invalidOperationDelegate = SetPendingInvalidOperationException;
				ioDelegate = SetPendingIOException;
				nullReferenceDelegate = SetPendingNullReferenceException;
				outOfMemoryDelegate = SetPendingOutOfMemoryException;
				overflowDelegate = SetPendingOverflowException;
				systemDelegate = SetPendingSystemException;
				argumentDelegate = SetPendingArgumentException;
				argumentNullDelegate = SetPendingArgumentNullException;
				argumentOutOfRangeDelegate = SetPendingArgumentOutOfRangeException;
				try
				{
					SWIGRegisterExceptionCallbacks_rail_api(applicationDelegate, arithmeticDelegate, divideByZeroDelegate, indexOutOfRangeDelegate, invalidCastDelegate, invalidOperationDelegate, ioDelegate, nullReferenceDelegate, outOfMemoryDelegate, overflowDelegate, systemDelegate);
					SWIGRegisterExceptionCallbacksArgument_rail_api(argumentDelegate, argumentNullDelegate, argumentOutOfRangeDelegate);
				}
				catch (DllNotFoundException)
				{
					throw new ApplicationException("rail_api.dll or rail_api64.dll is missing! Please make sure it`s placed in the search directory. (You could place it beside the executable or Plugins folder for unity games.)");
				}
			}
		}

		public class SWIGPendingException
		{
			[ThreadStatic]
			private static Exception pendingException;

			private static int numExceptionsPending;

			public static bool Pending
			{
				get
				{
					bool result = false;
					if (numExceptionsPending > 0 && pendingException != null)
					{
						result = true;
					}
					return result;
				}
			}

			public static void Set(Exception e)
			{
				if (pendingException != null)
				{
					throw new ApplicationException("FATAL: An earlier pending exception from unmanaged code was missed and thus not thrown (" + pendingException.ToString() + ")", e);
				}
				pendingException = e;
				lock (typeof(RAIL_API_PINVOKE))
				{
					numExceptionsPending++;
				}
			}

			public static Exception Retrieve()
			{
				Exception result = null;
				if (numExceptionsPending > 0 && pendingException != null)
				{
					result = pendingException;
					pendingException = null;
					lock (typeof(RAIL_API_PINVOKE))
					{
						numExceptionsPending--;
						return result;
					}
				}
				return result;
			}
		}

		protected class SWIGStringHelper
		{
			public delegate string SWIGStringDelegate(string message);

			private static SWIGStringDelegate stringDelegate;

			[DllImport("rail_api64")]
			public static extern void SWIGRegisterStringCallback_rail_api(SWIGStringDelegate stringDelegate);

			[MonoPInvokeCallback(typeof(SWIGStringDelegate))]
			private static string CreateString(string cString)
			{
				return cString;
			}

			static SWIGStringHelper()
			{
				stringDelegate = CreateString;
				SWIGRegisterStringCallback_rail_api(stringDelegate);
			}
		}

		public const string dll_path = "rail_api64";

		protected static SWIGExceptionHelper swigExceptionHelper;

		protected static SWIGStringHelper swigStringHelper;

		static RAIL_API_PINVOKE()
		{
			swigExceptionHelper = new SWIGExceptionHelper();
			swigStringHelper = new SWIGStringHelper();
		}

		[DllImport("rail_api64", EntryPoint = "CSharp_USE_MANUAL_ALLOC_get")]
		public static extern int USE_MANUAL_ALLOC_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RAIL_SDK_PACKING_get")]
		public static extern int RAIL_SDK_PACKING_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailString__SWIG_0")]
		public static extern IntPtr new_RailString__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailString__SWIG_1")]
		public static extern IntPtr new_RailString__SWIG_1(string jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailString_SetValue")]
		public static extern IntPtr RailString_SetValue(IntPtr jarg1, string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailString_assign")]
		public static extern void RailString_assign(IntPtr jarg1, string jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailString_c_str")]
		public static extern IntPtr RailString_c_str(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailString_data")]
		public static extern IntPtr RailString_data(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailString_clear")]
		public static extern void RailString_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailString_size")]
		public static extern uint RailString_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailString")]
		public static extern void delete_RailString(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailID__SWIG_0")]
		public static extern IntPtr new_RailID__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailID__SWIG_1")]
		public static extern IntPtr new_RailID__SWIG_1(ulong jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailID_set_id")]
		public static extern void RailID_set_id(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailID_get_id")]
		public static extern ulong RailID_get_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailID_IsValid")]
		public static extern bool RailID_IsValid(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailID_GetDomain")]
		public static extern int RailID_GetDomain(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailID")]
		public static extern void delete_RailID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGameID__SWIG_0")]
		public static extern IntPtr new_RailGameID__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGameID__SWIG_1")]
		public static extern IntPtr new_RailGameID__SWIG_1(ulong jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameID_set_id")]
		public static extern void RailGameID_set_id(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameID_get_id")]
		public static extern ulong RailGameID_get_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameID_IsValid")]
		public static extern bool RailGameID_IsValid(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailGameID")]
		public static extern void delete_RailGameID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailDlcID__SWIG_0")]
		public static extern IntPtr new_RailDlcID__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailDlcID__SWIG_1")]
		public static extern IntPtr new_RailDlcID__SWIG_1(ulong jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcID_set_id")]
		public static extern void RailDlcID_set_id(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcID_get_id")]
		public static extern ulong RailDlcID_get_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcID_IsValid")]
		public static extern bool RailDlcID_IsValid(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailDlcID")]
		public static extern void delete_RailDlcID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailVoiceChannelID__SWIG_0")]
		public static extern IntPtr new_RailVoiceChannelID__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailVoiceChannelID__SWIG_1")]
		public static extern IntPtr new_RailVoiceChannelID__SWIG_1(ulong jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceChannelID_set_id")]
		public static extern void RailVoiceChannelID_set_id(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceChannelID_get_id")]
		public static extern ulong RailVoiceChannelID_get_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceChannelID_IsValid")]
		public static extern bool RailVoiceChannelID_IsValid(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailVoiceChannelID")]
		public static extern void delete_RailVoiceChannelID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailZoneID__SWIG_0")]
		public static extern IntPtr new_RailZoneID__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailZoneID__SWIG_1")]
		public static extern IntPtr new_RailZoneID__SWIG_1(ulong jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailZoneID_set_id")]
		public static extern void RailZoneID_set_id(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailZoneID_get_id")]
		public static extern ulong RailZoneID_get_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailZoneID_IsValid")]
		public static extern bool RailZoneID_IsValid(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailZoneID")]
		public static extern void delete_RailZoneID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailKeyValue_key_set")]
		public static extern void RailKeyValue_key_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailKeyValue_key_get")]
		public static extern IntPtr RailKeyValue_key_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailKeyValue_value_set")]
		public static extern void RailKeyValue_value_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailKeyValue_value_get")]
		public static extern IntPtr RailKeyValue_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailKeyValue")]
		public static extern IntPtr new_RailKeyValue();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailKeyValue")]
		public static extern void delete_RailKeyValue(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSessionTicket")]
		public static extern IntPtr new_RailSessionTicket();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSessionTicket_ticket_set")]
		public static extern void RailSessionTicket_ticket_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSessionTicket_ticket_get")]
		public static extern IntPtr RailSessionTicket_ticket_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSessionTicket")]
		public static extern void delete_RailSessionTicket(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailComponent_GetComponentVersion")]
		public static extern ulong IRailComponent_GetComponentVersion(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailComponent_Release")]
		public static extern void IRailComponent_Release(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_EventBase")]
		public static extern void delete_EventBase(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_EventBase_get_event_id")]
		public static extern int EventBase_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_EventBase_rail_id_set")]
		public static extern void EventBase_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_EventBase_rail_id_get")]
		public static extern IntPtr EventBase_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_EventBase_game_id_set")]
		public static extern void EventBase_game_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_EventBase_game_id_get")]
		public static extern IntPtr EventBase_game_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_EventBase_user_data_set")]
		public static extern void EventBase_user_data_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_EventBase_user_data_get")]
		public static extern IntPtr EventBase_user_data_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_EventBase_result_set")]
		public static extern void EventBase_result_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_EventBase_result_get")]
		public static extern int EventBase_result_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailEvent_OnRailEvent")]
		public static extern void IRailEvent_OnRailEvent(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailEvent")]
		public static extern void delete_IRailEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailProductItem__SWIG_0")]
		public static extern IntPtr new_RailArrayRailProductItem__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailProductItem__SWIG_1")]
		public static extern IntPtr new_RailArrayRailProductItem__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailProductItem__SWIG_2")]
		public static extern IntPtr new_RailArrayRailProductItem__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailProductItem__SWIG_3")]
		public static extern IntPtr new_RailArrayRailProductItem__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailProductItem_SetValue")]
		public static extern IntPtr RailArrayRailProductItem_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailProductItem")]
		public static extern void delete_RailArrayRailProductItem(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailProductItem_assign")]
		public static extern void RailArrayRailProductItem_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailProductItem_buf")]
		public static extern IntPtr RailArrayRailProductItem_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailProductItem_size")]
		public static extern uint RailArrayRailProductItem_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailProductItem_Item")]
		public static extern IntPtr RailArrayRailProductItem_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailProductItem_resize")]
		public static extern void RailArrayRailProductItem_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailProductItem_push_back")]
		public static extern void RailArrayRailProductItem_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailProductItem_clear")]
		public static extern void RailArrayRailProductItem_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailProductItem_erase")]
		public static extern void RailArrayRailProductItem_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayPlayerPersonalInfo__SWIG_0")]
		public static extern IntPtr new_RailArrayPlayerPersonalInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayPlayerPersonalInfo__SWIG_1")]
		public static extern IntPtr new_RailArrayPlayerPersonalInfo__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayPlayerPersonalInfo__SWIG_2")]
		public static extern IntPtr new_RailArrayPlayerPersonalInfo__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayPlayerPersonalInfo__SWIG_3")]
		public static extern IntPtr new_RailArrayPlayerPersonalInfo__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayPlayerPersonalInfo_SetValue")]
		public static extern IntPtr RailArrayPlayerPersonalInfo_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayPlayerPersonalInfo")]
		public static extern void delete_RailArrayPlayerPersonalInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayPlayerPersonalInfo_assign")]
		public static extern void RailArrayPlayerPersonalInfo_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayPlayerPersonalInfo_buf")]
		public static extern IntPtr RailArrayPlayerPersonalInfo_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayPlayerPersonalInfo_size")]
		public static extern uint RailArrayPlayerPersonalInfo_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayPlayerPersonalInfo_Item")]
		public static extern IntPtr RailArrayPlayerPersonalInfo_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayPlayerPersonalInfo_resize")]
		public static extern void RailArrayPlayerPersonalInfo_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayPlayerPersonalInfo_push_back")]
		public static extern void RailArrayPlayerPersonalInfo_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayPlayerPersonalInfo_clear")]
		public static extern void RailArrayPlayerPersonalInfo_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayPlayerPersonalInfo_erase")]
		public static extern void RailArrayPlayerPersonalInfo_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayuint64_t__SWIG_0")]
		public static extern IntPtr new_RailArrayuint64_t__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayuint64_t__SWIG_1")]
		public static extern IntPtr new_RailArrayuint64_t__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayuint64_t__SWIG_2")]
		public static extern IntPtr new_RailArrayuint64_t__SWIG_2(out ulong jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayuint64_t__SWIG_3")]
		public static extern IntPtr new_RailArrayuint64_t__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint64_t_SetValue")]
		public static extern IntPtr RailArrayuint64_t_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayuint64_t")]
		public static extern void delete_RailArrayuint64_t(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint64_t_assign")]
		public static extern void RailArrayuint64_t_assign(IntPtr jarg1, out ulong jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint64_t_buf")]
		public static extern IntPtr RailArrayuint64_t_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint64_t_size")]
		public static extern uint RailArrayuint64_t_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint64_t_Item")]
		public static extern IntPtr RailArrayuint64_t_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint64_t_resize")]
		public static extern void RailArrayuint64_t_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint64_t_push_back")]
		public static extern void RailArrayuint64_t_push_back(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint64_t_clear")]
		public static extern void RailArrayuint64_t_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint64_t_erase")]
		public static extern void RailArrayuint64_t_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayuint32_t__SWIG_0")]
		public static extern IntPtr new_RailArrayuint32_t__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayuint32_t__SWIG_1")]
		public static extern IntPtr new_RailArrayuint32_t__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayuint32_t__SWIG_2")]
		public static extern IntPtr new_RailArrayuint32_t__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayuint32_t__SWIG_3")]
		public static extern IntPtr new_RailArrayuint32_t__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint32_t_SetValue")]
		public static extern IntPtr RailArrayuint32_t_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayuint32_t")]
		public static extern void delete_RailArrayuint32_t(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint32_t_assign")]
		public static extern void RailArrayuint32_t_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint32_t_buf")]
		public static extern IntPtr RailArrayuint32_t_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint32_t_size")]
		public static extern uint RailArrayuint32_t_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint32_t_Item")]
		public static extern IntPtr RailArrayuint32_t_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint32_t_resize")]
		public static extern void RailArrayuint32_t_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint32_t_push_back")]
		public static extern void RailArrayuint32_t_push_back(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint32_t_clear")]
		public static extern void RailArrayuint32_t_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint32_t_erase")]
		public static extern void RailArrayuint32_t_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailAssetInfo__SWIG_0")]
		public static extern IntPtr new_RailArrayRailAssetInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailAssetInfo__SWIG_1")]
		public static extern IntPtr new_RailArrayRailAssetInfo__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailAssetInfo__SWIG_2")]
		public static extern IntPtr new_RailArrayRailAssetInfo__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailAssetInfo__SWIG_3")]
		public static extern IntPtr new_RailArrayRailAssetInfo__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetInfo_SetValue")]
		public static extern IntPtr RailArrayRailAssetInfo_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailAssetInfo")]
		public static extern void delete_RailArrayRailAssetInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetInfo_assign")]
		public static extern void RailArrayRailAssetInfo_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetInfo_buf")]
		public static extern IntPtr RailArrayRailAssetInfo_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetInfo_size")]
		public static extern uint RailArrayRailAssetInfo_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetInfo_Item")]
		public static extern IntPtr RailArrayRailAssetInfo_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetInfo_resize")]
		public static extern void RailArrayRailAssetInfo_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetInfo_push_back")]
		public static extern void RailArrayRailAssetInfo_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetInfo_clear")]
		public static extern void RailArrayRailAssetInfo_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetInfo_erase")]
		public static extern void RailArrayRailAssetInfo_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailAssetItem__SWIG_0")]
		public static extern IntPtr new_RailArrayRailAssetItem__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailAssetItem__SWIG_1")]
		public static extern IntPtr new_RailArrayRailAssetItem__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailAssetItem__SWIG_2")]
		public static extern IntPtr new_RailArrayRailAssetItem__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailAssetItem__SWIG_3")]
		public static extern IntPtr new_RailArrayRailAssetItem__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetItem_SetValue")]
		public static extern IntPtr RailArrayRailAssetItem_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailAssetItem")]
		public static extern void delete_RailArrayRailAssetItem(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetItem_assign")]
		public static extern void RailArrayRailAssetItem_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetItem_buf")]
		public static extern IntPtr RailArrayRailAssetItem_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetItem_size")]
		public static extern uint RailArrayRailAssetItem_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetItem_Item")]
		public static extern IntPtr RailArrayRailAssetItem_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetItem_resize")]
		public static extern void RailArrayRailAssetItem_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetItem_push_back")]
		public static extern void RailArrayRailAssetItem_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetItem_clear")]
		public static extern void RailArrayRailAssetItem_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetItem_erase")]
		public static extern void RailArrayRailAssetItem_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomMemberInfo__SWIG_0")]
		public static extern IntPtr new_RailArrayRoomMemberInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomMemberInfo__SWIG_1")]
		public static extern IntPtr new_RailArrayRoomMemberInfo__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomMemberInfo__SWIG_2")]
		public static extern IntPtr new_RailArrayRoomMemberInfo__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomMemberInfo__SWIG_3")]
		public static extern IntPtr new_RailArrayRoomMemberInfo__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomMemberInfo_SetValue")]
		public static extern IntPtr RailArrayRoomMemberInfo_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRoomMemberInfo")]
		public static extern void delete_RailArrayRoomMemberInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomMemberInfo_assign")]
		public static extern void RailArrayRoomMemberInfo_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomMemberInfo_buf")]
		public static extern IntPtr RailArrayRoomMemberInfo_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomMemberInfo_size")]
		public static extern uint RailArrayRoomMemberInfo_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomMemberInfo_Item")]
		public static extern IntPtr RailArrayRoomMemberInfo_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomMemberInfo_resize")]
		public static extern void RailArrayRoomMemberInfo_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomMemberInfo_push_back")]
		public static extern void RailArrayRoomMemberInfo_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomMemberInfo_clear")]
		public static extern void RailArrayRoomMemberInfo_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomMemberInfo_erase")]
		public static extern void RailArrayRoomMemberInfo_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailCoinInfo__SWIG_0")]
		public static extern IntPtr new_RailArrayRailCoinInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailCoinInfo__SWIG_1")]
		public static extern IntPtr new_RailArrayRailCoinInfo__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailCoinInfo__SWIG_2")]
		public static extern IntPtr new_RailArrayRailCoinInfo__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailCoinInfo__SWIG_3")]
		public static extern IntPtr new_RailArrayRailCoinInfo__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailCoinInfo_SetValue")]
		public static extern IntPtr RailArrayRailCoinInfo_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailCoinInfo")]
		public static extern void delete_RailArrayRailCoinInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailCoinInfo_assign")]
		public static extern void RailArrayRailCoinInfo_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailCoinInfo_buf")]
		public static extern IntPtr RailArrayRailCoinInfo_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailCoinInfo_size")]
		public static extern uint RailArrayRailCoinInfo_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailCoinInfo_Item")]
		public static extern IntPtr RailArrayRailCoinInfo_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailCoinInfo_resize")]
		public static extern void RailArrayRailCoinInfo_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailCoinInfo_push_back")]
		public static extern void RailArrayRailCoinInfo_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailCoinInfo_clear")]
		public static extern void RailArrayRailCoinInfo_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailCoinInfo_erase")]
		public static extern void RailArrayRailCoinInfo_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailAssetProperty__SWIG_0")]
		public static extern IntPtr new_RailArrayRailAssetProperty__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailAssetProperty__SWIG_1")]
		public static extern IntPtr new_RailArrayRailAssetProperty__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailAssetProperty__SWIG_2")]
		public static extern IntPtr new_RailArrayRailAssetProperty__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailAssetProperty__SWIG_3")]
		public static extern IntPtr new_RailArrayRailAssetProperty__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetProperty_SetValue")]
		public static extern IntPtr RailArrayRailAssetProperty_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailAssetProperty")]
		public static extern void delete_RailArrayRailAssetProperty(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetProperty_assign")]
		public static extern void RailArrayRailAssetProperty_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetProperty_buf")]
		public static extern IntPtr RailArrayRailAssetProperty_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetProperty_size")]
		public static extern uint RailArrayRailAssetProperty_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetProperty_Item")]
		public static extern IntPtr RailArrayRailAssetProperty_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetProperty_resize")]
		public static extern void RailArrayRailAssetProperty_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetProperty_push_back")]
		public static extern void RailArrayRailAssetProperty_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetProperty_clear")]
		public static extern void RailArrayRailAssetProperty_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailAssetProperty_erase")]
		public static extern void RailArrayRailAssetProperty_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerListFilterKey__SWIG_0")]
		public static extern IntPtr new_RailArrayGameServerListFilterKey__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerListFilterKey__SWIG_1")]
		public static extern IntPtr new_RailArrayGameServerListFilterKey__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerListFilterKey__SWIG_2")]
		public static extern IntPtr new_RailArrayGameServerListFilterKey__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerListFilterKey__SWIG_3")]
		public static extern IntPtr new_RailArrayGameServerListFilterKey__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilterKey_SetValue")]
		public static extern IntPtr RailArrayGameServerListFilterKey_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayGameServerListFilterKey")]
		public static extern void delete_RailArrayGameServerListFilterKey(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilterKey_assign")]
		public static extern void RailArrayGameServerListFilterKey_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilterKey_buf")]
		public static extern IntPtr RailArrayGameServerListFilterKey_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilterKey_size")]
		public static extern uint RailArrayGameServerListFilterKey_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilterKey_Item")]
		public static extern IntPtr RailArrayGameServerListFilterKey_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilterKey_resize")]
		public static extern void RailArrayGameServerListFilterKey_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilterKey_push_back")]
		public static extern void RailArrayGameServerListFilterKey_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilterKey_clear")]
		public static extern void RailArrayGameServerListFilterKey_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilterKey_erase")]
		public static extern void RailArrayGameServerListFilterKey_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailStreamFileInfo__SWIG_0")]
		public static extern IntPtr new_RailArrayRailStreamFileInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailStreamFileInfo__SWIG_1")]
		public static extern IntPtr new_RailArrayRailStreamFileInfo__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailStreamFileInfo__SWIG_2")]
		public static extern IntPtr new_RailArrayRailStreamFileInfo__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailStreamFileInfo__SWIG_3")]
		public static extern IntPtr new_RailArrayRailStreamFileInfo__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailStreamFileInfo_SetValue")]
		public static extern IntPtr RailArrayRailStreamFileInfo_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailStreamFileInfo")]
		public static extern void delete_RailArrayRailStreamFileInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailStreamFileInfo_assign")]
		public static extern void RailArrayRailStreamFileInfo_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailStreamFileInfo_buf")]
		public static extern IntPtr RailArrayRailStreamFileInfo_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailStreamFileInfo_size")]
		public static extern uint RailArrayRailStreamFileInfo_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailStreamFileInfo_Item")]
		public static extern IntPtr RailArrayRailStreamFileInfo_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailStreamFileInfo_resize")]
		public static extern void RailArrayRailStreamFileInfo_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailStreamFileInfo_push_back")]
		public static extern void RailArrayRailStreamFileInfo_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailStreamFileInfo_clear")]
		public static extern void RailArrayRailStreamFileInfo_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailStreamFileInfo_erase")]
		public static extern void RailArrayRailStreamFileInfo_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfoListFilter__SWIG_0")]
		public static extern IntPtr new_RailArrayRoomInfoListFilter__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfoListFilter__SWIG_1")]
		public static extern IntPtr new_RailArrayRoomInfoListFilter__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfoListFilter__SWIG_2")]
		public static extern IntPtr new_RailArrayRoomInfoListFilter__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfoListFilter__SWIG_3")]
		public static extern IntPtr new_RailArrayRoomInfoListFilter__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilter_SetValue")]
		public static extern IntPtr RailArrayRoomInfoListFilter_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRoomInfoListFilter")]
		public static extern void delete_RailArrayRoomInfoListFilter(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilter_assign")]
		public static extern void RailArrayRoomInfoListFilter_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilter_buf")]
		public static extern IntPtr RailArrayRoomInfoListFilter_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilter_size")]
		public static extern uint RailArrayRoomInfoListFilter_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilter_Item")]
		public static extern IntPtr RailArrayRoomInfoListFilter_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilter_resize")]
		public static extern void RailArrayRoomInfoListFilter_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilter_push_back")]
		public static extern void RailArrayRoomInfoListFilter_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilter_clear")]
		public static extern void RailArrayRoomInfoListFilter_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilter_erase")]
		public static extern void RailArrayRoomInfoListFilter_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerListFilter__SWIG_0")]
		public static extern IntPtr new_RailArrayGameServerListFilter__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerListFilter__SWIG_1")]
		public static extern IntPtr new_RailArrayGameServerListFilter__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerListFilter__SWIG_2")]
		public static extern IntPtr new_RailArrayGameServerListFilter__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerListFilter__SWIG_3")]
		public static extern IntPtr new_RailArrayGameServerListFilter__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilter_SetValue")]
		public static extern IntPtr RailArrayGameServerListFilter_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayGameServerListFilter")]
		public static extern void delete_RailArrayGameServerListFilter(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilter_assign")]
		public static extern void RailArrayGameServerListFilter_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilter_buf")]
		public static extern IntPtr RailArrayGameServerListFilter_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilter_size")]
		public static extern uint RailArrayGameServerListFilter_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilter_Item")]
		public static extern IntPtr RailArrayGameServerListFilter_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilter_resize")]
		public static extern void RailArrayGameServerListFilter_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilter_push_back")]
		public static extern void RailArrayGameServerListFilter_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilter_clear")]
		public static extern void RailArrayGameServerListFilter_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListFilter_erase")]
		public static extern void RailArrayGameServerListFilter_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailSpaceWorkType__SWIG_0")]
		public static extern IntPtr new_RailArrayEnumRailSpaceWorkType__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailSpaceWorkType__SWIG_1")]
		public static extern IntPtr new_RailArrayEnumRailSpaceWorkType__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailSpaceWorkType__SWIG_2")]
		public static extern IntPtr new_RailArrayEnumRailSpaceWorkType__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailSpaceWorkType__SWIG_3")]
		public static extern IntPtr new_RailArrayEnumRailSpaceWorkType__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailSpaceWorkType_SetValue")]
		public static extern IntPtr RailArrayEnumRailSpaceWorkType_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayEnumRailSpaceWorkType")]
		public static extern void delete_RailArrayEnumRailSpaceWorkType(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailSpaceWorkType_assign")]
		public static extern void RailArrayEnumRailSpaceWorkType_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailSpaceWorkType_buf")]
		public static extern IntPtr RailArrayEnumRailSpaceWorkType_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailSpaceWorkType_size")]
		public static extern uint RailArrayEnumRailSpaceWorkType_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailSpaceWorkType_Item")]
		public static extern IntPtr RailArrayEnumRailSpaceWorkType_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailSpaceWorkType_resize")]
		public static extern void RailArrayEnumRailSpaceWorkType_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailSpaceWorkType_push_back")]
		public static extern void RailArrayEnumRailSpaceWorkType_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailSpaceWorkType_clear")]
		public static extern void RailArrayEnumRailSpaceWorkType_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailSpaceWorkType_erase")]
		public static extern void RailArrayEnumRailSpaceWorkType_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailSpaceWorkType__SWIG_4")]
		public static extern IntPtr new_RailArrayEnumRailSpaceWorkType__SWIG_4(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGameActivityInfo__SWIG_0")]
		public static extern IntPtr new_RailArrayRailGameActivityInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGameActivityInfo__SWIG_1")]
		public static extern IntPtr new_RailArrayRailGameActivityInfo__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGameActivityInfo__SWIG_2")]
		public static extern IntPtr new_RailArrayRailGameActivityInfo__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGameActivityInfo__SWIG_3")]
		public static extern IntPtr new_RailArrayRailGameActivityInfo__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameActivityInfo_SetValue")]
		public static extern IntPtr RailArrayRailGameActivityInfo_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailGameActivityInfo")]
		public static extern void delete_RailArrayRailGameActivityInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameActivityInfo_assign")]
		public static extern void RailArrayRailGameActivityInfo_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameActivityInfo_buf")]
		public static extern IntPtr RailArrayRailGameActivityInfo_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameActivityInfo_size")]
		public static extern uint RailArrayRailGameActivityInfo_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameActivityInfo_Item")]
		public static extern IntPtr RailArrayRailGameActivityInfo_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameActivityInfo_resize")]
		public static extern void RailArrayRailGameActivityInfo_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameActivityInfo_push_back")]
		public static extern void RailArrayRailGameActivityInfo_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameActivityInfo_clear")]
		public static extern void RailArrayRailGameActivityInfo_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameActivityInfo_erase")]
		public static extern void RailArrayRailGameActivityInfo_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailUsersLimits__SWIG_0")]
		public static extern IntPtr new_RailArrayEnumRailUsersLimits__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailUsersLimits__SWIG_1")]
		public static extern IntPtr new_RailArrayEnumRailUsersLimits__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailUsersLimits__SWIG_2")]
		public static extern IntPtr new_RailArrayEnumRailUsersLimits__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailUsersLimits__SWIG_3")]
		public static extern IntPtr new_RailArrayEnumRailUsersLimits__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailUsersLimits_SetValue")]
		public static extern IntPtr RailArrayEnumRailUsersLimits_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayEnumRailUsersLimits")]
		public static extern void delete_RailArrayEnumRailUsersLimits(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailUsersLimits_assign")]
		public static extern void RailArrayEnumRailUsersLimits_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailUsersLimits_buf")]
		public static extern IntPtr RailArrayEnumRailUsersLimits_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailUsersLimits_size")]
		public static extern uint RailArrayEnumRailUsersLimits_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailUsersLimits_Item")]
		public static extern IntPtr RailArrayEnumRailUsersLimits_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailUsersLimits_resize")]
		public static extern void RailArrayEnumRailUsersLimits_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailUsersLimits_push_back")]
		public static extern void RailArrayEnumRailUsersLimits_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailUsersLimits_clear")]
		public static extern void RailArrayEnumRailUsersLimits_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailUsersLimits_erase")]
		public static extern void RailArrayEnumRailUsersLimits_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailUsersLimits__SWIG_4")]
		public static extern IntPtr new_RailArrayEnumRailUsersLimits__SWIG_4(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailUserSpaceDownloadProgress__SWIG_0")]
		public static extern IntPtr new_RailArrayRailUserSpaceDownloadProgress__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailUserSpaceDownloadProgress__SWIG_1")]
		public static extern IntPtr new_RailArrayRailUserSpaceDownloadProgress__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailUserSpaceDownloadProgress__SWIG_2")]
		public static extern IntPtr new_RailArrayRailUserSpaceDownloadProgress__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailUserSpaceDownloadProgress__SWIG_3")]
		public static extern IntPtr new_RailArrayRailUserSpaceDownloadProgress__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadProgress_SetValue")]
		public static extern IntPtr RailArrayRailUserSpaceDownloadProgress_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailUserSpaceDownloadProgress")]
		public static extern void delete_RailArrayRailUserSpaceDownloadProgress(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadProgress_assign")]
		public static extern void RailArrayRailUserSpaceDownloadProgress_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadProgress_buf")]
		public static extern IntPtr RailArrayRailUserSpaceDownloadProgress_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadProgress_size")]
		public static extern uint RailArrayRailUserSpaceDownloadProgress_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadProgress_Item")]
		public static extern IntPtr RailArrayRailUserSpaceDownloadProgress_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadProgress_resize")]
		public static extern void RailArrayRailUserSpaceDownloadProgress_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadProgress_push_back")]
		public static extern void RailArrayRailUserSpaceDownloadProgress_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadProgress_clear")]
		public static extern void RailArrayRailUserSpaceDownloadProgress_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadProgress_erase")]
		public static extern void RailArrayRailUserSpaceDownloadProgress_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailPlayedWithFriendsGameItem__SWIG_0")]
		public static extern IntPtr new_RailArrayRailPlayedWithFriendsGameItem__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailPlayedWithFriendsGameItem__SWIG_1")]
		public static extern IntPtr new_RailArrayRailPlayedWithFriendsGameItem__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailPlayedWithFriendsGameItem__SWIG_2")]
		public static extern IntPtr new_RailArrayRailPlayedWithFriendsGameItem__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailPlayedWithFriendsGameItem__SWIG_3")]
		public static extern IntPtr new_RailArrayRailPlayedWithFriendsGameItem__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsGameItem_SetValue")]
		public static extern IntPtr RailArrayRailPlayedWithFriendsGameItem_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailPlayedWithFriendsGameItem")]
		public static extern void delete_RailArrayRailPlayedWithFriendsGameItem(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsGameItem_assign")]
		public static extern void RailArrayRailPlayedWithFriendsGameItem_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsGameItem_buf")]
		public static extern IntPtr RailArrayRailPlayedWithFriendsGameItem_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsGameItem_size")]
		public static extern uint RailArrayRailPlayedWithFriendsGameItem_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsGameItem_Item")]
		public static extern IntPtr RailArrayRailPlayedWithFriendsGameItem_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsGameItem_resize")]
		public static extern void RailArrayRailPlayedWithFriendsGameItem_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsGameItem_push_back")]
		public static extern void RailArrayRailPlayedWithFriendsGameItem_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsGameItem_clear")]
		public static extern void RailArrayRailPlayedWithFriendsGameItem_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsGameItem_erase")]
		public static extern void RailArrayRailPlayedWithFriendsGameItem_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArraySpaceWorkID__SWIG_0")]
		public static extern IntPtr new_RailArraySpaceWorkID__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArraySpaceWorkID__SWIG_1")]
		public static extern IntPtr new_RailArraySpaceWorkID__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArraySpaceWorkID__SWIG_2")]
		public static extern IntPtr new_RailArraySpaceWorkID__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArraySpaceWorkID__SWIG_3")]
		public static extern IntPtr new_RailArraySpaceWorkID__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArraySpaceWorkID_SetValue")]
		public static extern IntPtr RailArraySpaceWorkID_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArraySpaceWorkID")]
		public static extern void delete_RailArraySpaceWorkID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArraySpaceWorkID_assign")]
		public static extern void RailArraySpaceWorkID_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArraySpaceWorkID_buf")]
		public static extern IntPtr RailArraySpaceWorkID_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArraySpaceWorkID_size")]
		public static extern uint RailArraySpaceWorkID_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArraySpaceWorkID_Item")]
		public static extern IntPtr RailArraySpaceWorkID_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArraySpaceWorkID_resize")]
		public static extern void RailArraySpaceWorkID_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArraySpaceWorkID_push_back")]
		public static extern void RailArraySpaceWorkID_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArraySpaceWorkID_clear")]
		public static extern void RailArraySpaceWorkID_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArraySpaceWorkID_erase")]
		public static extern void RailArraySpaceWorkID_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfoListSorter__SWIG_0")]
		public static extern IntPtr new_RailArrayRoomInfoListSorter__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfoListSorter__SWIG_1")]
		public static extern IntPtr new_RailArrayRoomInfoListSorter__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfoListSorter__SWIG_2")]
		public static extern IntPtr new_RailArrayRoomInfoListSorter__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfoListSorter__SWIG_3")]
		public static extern IntPtr new_RailArrayRoomInfoListSorter__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListSorter_SetValue")]
		public static extern IntPtr RailArrayRoomInfoListSorter_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRoomInfoListSorter")]
		public static extern void delete_RailArrayRoomInfoListSorter(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListSorter_assign")]
		public static extern void RailArrayRoomInfoListSorter_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListSorter_buf")]
		public static extern IntPtr RailArrayRoomInfoListSorter_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListSorter_size")]
		public static extern uint RailArrayRoomInfoListSorter_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListSorter_Item")]
		public static extern IntPtr RailArrayRoomInfoListSorter_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListSorter_resize")]
		public static extern void RailArrayRoomInfoListSorter_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListSorter_push_back")]
		public static extern void RailArrayRoomInfoListSorter_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListSorter_clear")]
		public static extern void RailArrayRoomInfoListSorter_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListSorter_erase")]
		public static extern void RailArrayRoomInfoListSorter_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailKeyValueResult__SWIG_0")]
		public static extern IntPtr new_RailArrayRailKeyValueResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailKeyValueResult__SWIG_1")]
		public static extern IntPtr new_RailArrayRailKeyValueResult__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailKeyValueResult__SWIG_2")]
		public static extern IntPtr new_RailArrayRailKeyValueResult__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailKeyValueResult__SWIG_3")]
		public static extern IntPtr new_RailArrayRailKeyValueResult__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValueResult_SetValue")]
		public static extern IntPtr RailArrayRailKeyValueResult_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailKeyValueResult")]
		public static extern void delete_RailArrayRailKeyValueResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValueResult_assign")]
		public static extern void RailArrayRailKeyValueResult_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValueResult_buf")]
		public static extern IntPtr RailArrayRailKeyValueResult_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValueResult_size")]
		public static extern uint RailArrayRailKeyValueResult_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValueResult_Item")]
		public static extern IntPtr RailArrayRailKeyValueResult_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValueResult_resize")]
		public static extern void RailArrayRailKeyValueResult_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValueResult_push_back")]
		public static extern void RailArrayRailKeyValueResult_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValueResult_clear")]
		public static extern void RailArrayRailKeyValueResult_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValueResult_erase")]
		public static extern void RailArrayRailKeyValueResult_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailString__SWIG_0")]
		public static extern IntPtr new_RailArrayRailString__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailString__SWIG_1")]
		public static extern IntPtr new_RailArrayRailString__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailString__SWIG_2")]
		public static extern IntPtr new_RailArrayRailString__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailString__SWIG_3")]
		public static extern IntPtr new_RailArrayRailString__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailString_SetValue")]
		public static extern IntPtr RailArrayRailString_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailString")]
		public static extern void delete_RailArrayRailString(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailString_assign")]
		public static extern void RailArrayRailString_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailString_buf")]
		public static extern IntPtr RailArrayRailString_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailString_size")]
		public static extern uint RailArrayRailString_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailString_Item")]
		public static extern IntPtr RailArrayRailString_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailString_resize")]
		public static extern void RailArrayRailString_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailString_push_back")]
		public static extern void RailArrayRailString_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailString_clear")]
		public static extern void RailArrayRailString_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailString_erase")]
		public static extern void RailArrayRailString_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailString__SWIG_4")]
		public static extern IntPtr new_RailArrayRailString__SWIG_4(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailUserSpaceDownloadResult__SWIG_0")]
		public static extern IntPtr new_RailArrayRailUserSpaceDownloadResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailUserSpaceDownloadResult__SWIG_1")]
		public static extern IntPtr new_RailArrayRailUserSpaceDownloadResult__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailUserSpaceDownloadResult__SWIG_2")]
		public static extern IntPtr new_RailArrayRailUserSpaceDownloadResult__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailUserSpaceDownloadResult__SWIG_3")]
		public static extern IntPtr new_RailArrayRailUserSpaceDownloadResult__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadResult_SetValue")]
		public static extern IntPtr RailArrayRailUserSpaceDownloadResult_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailUserSpaceDownloadResult")]
		public static extern void delete_RailArrayRailUserSpaceDownloadResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadResult_assign")]
		public static extern void RailArrayRailUserSpaceDownloadResult_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadResult_buf")]
		public static extern IntPtr RailArrayRailUserSpaceDownloadResult_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadResult_size")]
		public static extern uint RailArrayRailUserSpaceDownloadResult_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadResult_Item")]
		public static extern IntPtr RailArrayRailUserSpaceDownloadResult_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadResult_resize")]
		public static extern void RailArrayRailUserSpaceDownloadResult_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadResult_push_back")]
		public static extern void RailArrayRailUserSpaceDownloadResult_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadResult_clear")]
		public static extern void RailArrayRailUserSpaceDownloadResult_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserSpaceDownloadResult_erase")]
		public static extern void RailArrayRailUserSpaceDownloadResult_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailFriendInfo__SWIG_0")]
		public static extern IntPtr new_RailArrayRailFriendInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailFriendInfo__SWIG_1")]
		public static extern IntPtr new_RailArrayRailFriendInfo__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailFriendInfo__SWIG_2")]
		public static extern IntPtr new_RailArrayRailFriendInfo__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailFriendInfo__SWIG_3")]
		public static extern IntPtr new_RailArrayRailFriendInfo__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendInfo_SetValue")]
		public static extern IntPtr RailArrayRailFriendInfo_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailFriendInfo")]
		public static extern void delete_RailArrayRailFriendInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendInfo_assign")]
		public static extern void RailArrayRailFriendInfo_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendInfo_buf")]
		public static extern IntPtr RailArrayRailFriendInfo_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendInfo_size")]
		public static extern uint RailArrayRailFriendInfo_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendInfo_Item")]
		public static extern IntPtr RailArrayRailFriendInfo_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendInfo_resize")]
		public static extern void RailArrayRailFriendInfo_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendInfo_push_back")]
		public static extern void RailArrayRailFriendInfo_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendInfo_clear")]
		public static extern void RailArrayRailFriendInfo_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendInfo_erase")]
		public static extern void RailArrayRailFriendInfo_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailZoneID__SWIG_0")]
		public static extern IntPtr new_RailArrayRailZoneID__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailZoneID__SWIG_1")]
		public static extern IntPtr new_RailArrayRailZoneID__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailZoneID__SWIG_2")]
		public static extern IntPtr new_RailArrayRailZoneID__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailZoneID__SWIG_3")]
		public static extern IntPtr new_RailArrayRailZoneID__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailZoneID_SetValue")]
		public static extern IntPtr RailArrayRailZoneID_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailZoneID")]
		public static extern void delete_RailArrayRailZoneID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailZoneID_assign")]
		public static extern void RailArrayRailZoneID_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailZoneID_buf")]
		public static extern IntPtr RailArrayRailZoneID_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailZoneID_size")]
		public static extern uint RailArrayRailZoneID_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailZoneID_Item")]
		public static extern IntPtr RailArrayRailZoneID_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailZoneID_resize")]
		public static extern void RailArrayRailZoneID_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailZoneID_push_back")]
		public static extern void RailArrayRailZoneID_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailZoneID_clear")]
		public static extern void RailArrayRailZoneID_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailZoneID_erase")]
		public static extern void RailArrayRailZoneID_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailKeyValue__SWIG_0")]
		public static extern IntPtr new_RailArrayRailKeyValue__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailKeyValue__SWIG_1")]
		public static extern IntPtr new_RailArrayRailKeyValue__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailKeyValue__SWIG_2")]
		public static extern IntPtr new_RailArrayRailKeyValue__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailKeyValue__SWIG_3")]
		public static extern IntPtr new_RailArrayRailKeyValue__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValue_SetValue")]
		public static extern IntPtr RailArrayRailKeyValue_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailKeyValue")]
		public static extern void delete_RailArrayRailKeyValue(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValue_assign")]
		public static extern void RailArrayRailKeyValue_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValue_buf")]
		public static extern IntPtr RailArrayRailKeyValue_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValue_size")]
		public static extern uint RailArrayRailKeyValue_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValue_Item")]
		public static extern IntPtr RailArrayRailKeyValue_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValue_resize")]
		public static extern void RailArrayRailKeyValue_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValue_push_back")]
		public static extern void RailArrayRailKeyValue_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValue_clear")]
		public static extern void RailArrayRailKeyValue_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailKeyValue_erase")]
		public static extern void RailArrayRailKeyValue_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSpaceWorkVoteDetail__SWIG_0")]
		public static extern IntPtr new_RailArrayRailSpaceWorkVoteDetail__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSpaceWorkVoteDetail__SWIG_1")]
		public static extern IntPtr new_RailArrayRailSpaceWorkVoteDetail__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSpaceWorkVoteDetail__SWIG_2")]
		public static extern IntPtr new_RailArrayRailSpaceWorkVoteDetail__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSpaceWorkVoteDetail__SWIG_3")]
		public static extern IntPtr new_RailArrayRailSpaceWorkVoteDetail__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkVoteDetail_SetValue")]
		public static extern IntPtr RailArrayRailSpaceWorkVoteDetail_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailSpaceWorkVoteDetail")]
		public static extern void delete_RailArrayRailSpaceWorkVoteDetail(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkVoteDetail_assign")]
		public static extern void RailArrayRailSpaceWorkVoteDetail_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkVoteDetail_buf")]
		public static extern IntPtr RailArrayRailSpaceWorkVoteDetail_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkVoteDetail_size")]
		public static extern uint RailArrayRailSpaceWorkVoteDetail_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkVoteDetail_Item")]
		public static extern IntPtr RailArrayRailSpaceWorkVoteDetail_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkVoteDetail_resize")]
		public static extern void RailArrayRailSpaceWorkVoteDetail_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkVoteDetail_push_back")]
		public static extern void RailArrayRailSpaceWorkVoteDetail_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkVoteDetail_clear")]
		public static extern void RailArrayRailSpaceWorkVoteDetail_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkVoteDetail_erase")]
		public static extern void RailArrayRailSpaceWorkVoteDetail_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSmallObjectState__SWIG_0")]
		public static extern IntPtr new_RailArrayRailSmallObjectState__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSmallObjectState__SWIG_1")]
		public static extern IntPtr new_RailArrayRailSmallObjectState__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSmallObjectState__SWIG_2")]
		public static extern IntPtr new_RailArrayRailSmallObjectState__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSmallObjectState__SWIG_3")]
		public static extern IntPtr new_RailArrayRailSmallObjectState__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectState_SetValue")]
		public static extern IntPtr RailArrayRailSmallObjectState_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailSmallObjectState")]
		public static extern void delete_RailArrayRailSmallObjectState(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectState_assign")]
		public static extern void RailArrayRailSmallObjectState_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectState_buf")]
		public static extern IntPtr RailArrayRailSmallObjectState_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectState_size")]
		public static extern uint RailArrayRailSmallObjectState_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectState_Item")]
		public static extern IntPtr RailArrayRailSmallObjectState_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectState_resize")]
		public static extern void RailArrayRailSmallObjectState_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectState_push_back")]
		public static extern void RailArrayRailSmallObjectState_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectState_clear")]
		public static extern void RailArrayRailSmallObjectState_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectState_erase")]
		public static extern void RailArrayRailSmallObjectState_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailFriendPlayedGameInfo__SWIG_0")]
		public static extern IntPtr new_RailArrayRailFriendPlayedGameInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailFriendPlayedGameInfo__SWIG_1")]
		public static extern IntPtr new_RailArrayRailFriendPlayedGameInfo__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailFriendPlayedGameInfo__SWIG_2")]
		public static extern IntPtr new_RailArrayRailFriendPlayedGameInfo__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailFriendPlayedGameInfo__SWIG_3")]
		public static extern IntPtr new_RailArrayRailFriendPlayedGameInfo__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendPlayedGameInfo_SetValue")]
		public static extern IntPtr RailArrayRailFriendPlayedGameInfo_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailFriendPlayedGameInfo")]
		public static extern void delete_RailArrayRailFriendPlayedGameInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendPlayedGameInfo_assign")]
		public static extern void RailArrayRailFriendPlayedGameInfo_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendPlayedGameInfo_buf")]
		public static extern IntPtr RailArrayRailFriendPlayedGameInfo_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendPlayedGameInfo_size")]
		public static extern uint RailArrayRailFriendPlayedGameInfo_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendPlayedGameInfo_Item")]
		public static extern IntPtr RailArrayRailFriendPlayedGameInfo_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendPlayedGameInfo_resize")]
		public static extern void RailArrayRailFriendPlayedGameInfo_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendPlayedGameInfo_push_back")]
		public static extern void RailArrayRailFriendPlayedGameInfo_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendPlayedGameInfo_clear")]
		public static extern void RailArrayRailFriendPlayedGameInfo_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendPlayedGameInfo_erase")]
		public static extern void RailArrayRailFriendPlayedGameInfo_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailVoiceChannelUserSpeakingState__SWIG_0")]
		public static extern IntPtr new_RailArrayRailVoiceChannelUserSpeakingState__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailVoiceChannelUserSpeakingState__SWIG_1")]
		public static extern IntPtr new_RailArrayRailVoiceChannelUserSpeakingState__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailVoiceChannelUserSpeakingState__SWIG_2")]
		public static extern IntPtr new_RailArrayRailVoiceChannelUserSpeakingState__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailVoiceChannelUserSpeakingState__SWIG_3")]
		public static extern IntPtr new_RailArrayRailVoiceChannelUserSpeakingState__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailVoiceChannelUserSpeakingState_SetValue")]
		public static extern IntPtr RailArrayRailVoiceChannelUserSpeakingState_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailVoiceChannelUserSpeakingState")]
		public static extern void delete_RailArrayRailVoiceChannelUserSpeakingState(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailVoiceChannelUserSpeakingState_assign")]
		public static extern void RailArrayRailVoiceChannelUserSpeakingState_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailVoiceChannelUserSpeakingState_buf")]
		public static extern IntPtr RailArrayRailVoiceChannelUserSpeakingState_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailVoiceChannelUserSpeakingState_size")]
		public static extern uint RailArrayRailVoiceChannelUserSpeakingState_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailVoiceChannelUserSpeakingState_Item")]
		public static extern IntPtr RailArrayRailVoiceChannelUserSpeakingState_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailVoiceChannelUserSpeakingState_resize")]
		public static extern void RailArrayRailVoiceChannelUserSpeakingState_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailVoiceChannelUserSpeakingState_push_back")]
		public static extern void RailArrayRailVoiceChannelUserSpeakingState_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailVoiceChannelUserSpeakingState_clear")]
		public static extern void RailArrayRailVoiceChannelUserSpeakingState_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailVoiceChannelUserSpeakingState_erase")]
		public static extern void RailArrayRailVoiceChannelUserSpeakingState_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGameID__SWIG_0")]
		public static extern IntPtr new_RailArrayRailGameID__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGameID__SWIG_1")]
		public static extern IntPtr new_RailArrayRailGameID__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGameID__SWIG_2")]
		public static extern IntPtr new_RailArrayRailGameID__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGameID__SWIG_3")]
		public static extern IntPtr new_RailArrayRailGameID__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameID_SetValue")]
		public static extern IntPtr RailArrayRailGameID_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailGameID")]
		public static extern void delete_RailArrayRailGameID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameID_assign")]
		public static extern void RailArrayRailGameID_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameID_buf")]
		public static extern IntPtr RailArrayRailGameID_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameID_size")]
		public static extern uint RailArrayRailGameID_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameID_Item")]
		public static extern IntPtr RailArrayRailGameID_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameID_resize")]
		public static extern void RailArrayRailGameID_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameID_push_back")]
		public static extern void RailArrayRailGameID_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameID_clear")]
		public static extern void RailArrayRailGameID_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameID_erase")]
		public static extern void RailArrayRailGameID_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayuint8_t__SWIG_0")]
		public static extern IntPtr new_RailArrayuint8_t__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayuint8_t__SWIG_1")]
		public static extern IntPtr new_RailArrayuint8_t__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayuint8_t__SWIG_2")]
		public static extern IntPtr new_RailArrayuint8_t__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayuint8_t__SWIG_3")]
		public static extern IntPtr new_RailArrayuint8_t__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint8_t_SetValue")]
		public static extern IntPtr RailArrayuint8_t_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayuint8_t")]
		public static extern void delete_RailArrayuint8_t(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint8_t_assign")]
		public static extern void RailArrayuint8_t_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint8_t_buf")]
		public static extern IntPtr RailArrayuint8_t_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint8_t_size")]
		public static extern uint RailArrayuint8_t_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint8_t_Item")]
		public static extern IntPtr RailArrayuint8_t_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint8_t_resize")]
		public static extern void RailArrayuint8_t_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint8_t_push_back")]
		public static extern void RailArrayuint8_t_push_back(IntPtr jarg1, byte jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint8_t_clear")]
		public static extern void RailArrayuint8_t_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayuint8_t_erase")]
		public static extern void RailArrayuint8_t_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailWorkFileClass__SWIG_0")]
		public static extern IntPtr new_RailArrayEnumRailWorkFileClass__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailWorkFileClass__SWIG_1")]
		public static extern IntPtr new_RailArrayEnumRailWorkFileClass__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailWorkFileClass__SWIG_2")]
		public static extern IntPtr new_RailArrayEnumRailWorkFileClass__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailWorkFileClass__SWIG_3")]
		public static extern IntPtr new_RailArrayEnumRailWorkFileClass__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailWorkFileClass_SetValue")]
		public static extern IntPtr RailArrayEnumRailWorkFileClass_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayEnumRailWorkFileClass")]
		public static extern void delete_RailArrayEnumRailWorkFileClass(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailWorkFileClass_assign")]
		public static extern void RailArrayEnumRailWorkFileClass_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailWorkFileClass_buf")]
		public static extern IntPtr RailArrayEnumRailWorkFileClass_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailWorkFileClass_size")]
		public static extern uint RailArrayEnumRailWorkFileClass_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailWorkFileClass_Item")]
		public static extern IntPtr RailArrayEnumRailWorkFileClass_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailWorkFileClass_resize")]
		public static extern void RailArrayEnumRailWorkFileClass_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailWorkFileClass_push_back")]
		public static extern void RailArrayEnumRailWorkFileClass_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailWorkFileClass_clear")]
		public static extern void RailArrayEnumRailWorkFileClass_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayEnumRailWorkFileClass_erase")]
		public static extern void RailArrayEnumRailWorkFileClass_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayEnumRailWorkFileClass__SWIG_4")]
		public static extern IntPtr new_RailArrayEnumRailWorkFileClass__SWIG_4(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailPurchaseProductInfo__SWIG_0")]
		public static extern IntPtr new_RailArrayRailPurchaseProductInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailPurchaseProductInfo__SWIG_1")]
		public static extern IntPtr new_RailArrayRailPurchaseProductInfo__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailPurchaseProductInfo__SWIG_2")]
		public static extern IntPtr new_RailArrayRailPurchaseProductInfo__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailPurchaseProductInfo__SWIG_3")]
		public static extern IntPtr new_RailArrayRailPurchaseProductInfo__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPurchaseProductInfo_SetValue")]
		public static extern IntPtr RailArrayRailPurchaseProductInfo_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailPurchaseProductInfo")]
		public static extern void delete_RailArrayRailPurchaseProductInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPurchaseProductInfo_assign")]
		public static extern void RailArrayRailPurchaseProductInfo_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPurchaseProductInfo_buf")]
		public static extern IntPtr RailArrayRailPurchaseProductInfo_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPurchaseProductInfo_size")]
		public static extern uint RailArrayRailPurchaseProductInfo_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPurchaseProductInfo_Item")]
		public static extern IntPtr RailArrayRailPurchaseProductInfo_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPurchaseProductInfo_resize")]
		public static extern void RailArrayRailPurchaseProductInfo_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPurchaseProductInfo_push_back")]
		public static extern void RailArrayRailPurchaseProductInfo_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPurchaseProductInfo_clear")]
		public static extern void RailArrayRailPurchaseProductInfo_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPurchaseProductInfo_erase")]
		public static extern void RailArrayRailPurchaseProductInfo_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfoListFilterKey__SWIG_0")]
		public static extern IntPtr new_RailArrayRoomInfoListFilterKey__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfoListFilterKey__SWIG_1")]
		public static extern IntPtr new_RailArrayRoomInfoListFilterKey__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfoListFilterKey__SWIG_2")]
		public static extern IntPtr new_RailArrayRoomInfoListFilterKey__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfoListFilterKey__SWIG_3")]
		public static extern IntPtr new_RailArrayRoomInfoListFilterKey__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilterKey_SetValue")]
		public static extern IntPtr RailArrayRoomInfoListFilterKey_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRoomInfoListFilterKey")]
		public static extern void delete_RailArrayRoomInfoListFilterKey(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilterKey_assign")]
		public static extern void RailArrayRoomInfoListFilterKey_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilterKey_buf")]
		public static extern IntPtr RailArrayRoomInfoListFilterKey_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilterKey_size")]
		public static extern uint RailArrayRoomInfoListFilterKey_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilterKey_Item")]
		public static extern IntPtr RailArrayRoomInfoListFilterKey_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilterKey_resize")]
		public static extern void RailArrayRoomInfoListFilterKey_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilterKey_push_back")]
		public static extern void RailArrayRoomInfoListFilterKey_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilterKey_clear")]
		public static extern void RailArrayRoomInfoListFilterKey_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfoListFilterKey_erase")]
		public static extern void RailArrayRoomInfoListFilterKey_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerListSorter__SWIG_0")]
		public static extern IntPtr new_RailArrayGameServerListSorter__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerListSorter__SWIG_1")]
		public static extern IntPtr new_RailArrayGameServerListSorter__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerListSorter__SWIG_2")]
		public static extern IntPtr new_RailArrayGameServerListSorter__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerListSorter__SWIG_3")]
		public static extern IntPtr new_RailArrayGameServerListSorter__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListSorter_SetValue")]
		public static extern IntPtr RailArrayGameServerListSorter_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayGameServerListSorter")]
		public static extern void delete_RailArrayGameServerListSorter(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListSorter_assign")]
		public static extern void RailArrayGameServerListSorter_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListSorter_buf")]
		public static extern IntPtr RailArrayGameServerListSorter_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListSorter_size")]
		public static extern uint RailArrayGameServerListSorter_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListSorter_Item")]
		public static extern IntPtr RailArrayGameServerListSorter_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListSorter_resize")]
		public static extern void RailArrayGameServerListSorter_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListSorter_push_back")]
		public static extern void RailArrayGameServerListSorter_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListSorter_clear")]
		public static extern void RailArrayGameServerListSorter_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerListSorter_erase")]
		public static extern void RailArrayGameServerListSorter_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailDlcID__SWIG_0")]
		public static extern IntPtr new_RailArrayRailDlcID__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailDlcID__SWIG_1")]
		public static extern IntPtr new_RailArrayRailDlcID__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailDlcID__SWIG_2")]
		public static extern IntPtr new_RailArrayRailDlcID__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailDlcID__SWIG_3")]
		public static extern IntPtr new_RailArrayRailDlcID__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcID_SetValue")]
		public static extern IntPtr RailArrayRailDlcID_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailDlcID")]
		public static extern void delete_RailArrayRailDlcID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcID_assign")]
		public static extern void RailArrayRailDlcID_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcID_buf")]
		public static extern IntPtr RailArrayRailDlcID_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcID_size")]
		public static extern uint RailArrayRailDlcID_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcID_Item")]
		public static extern IntPtr RailArrayRailDlcID_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcID_resize")]
		public static extern void RailArrayRailDlcID_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcID_push_back")]
		public static extern void RailArrayRailDlcID_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcID_clear")]
		public static extern void RailArrayRailDlcID_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcID_erase")]
		public static extern void RailArrayRailDlcID_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailID__SWIG_0")]
		public static extern IntPtr new_RailArrayRailID__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailID__SWIG_1")]
		public static extern IntPtr new_RailArrayRailID__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailID__SWIG_2")]
		public static extern IntPtr new_RailArrayRailID__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailID__SWIG_3")]
		public static extern IntPtr new_RailArrayRailID__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailID_SetValue")]
		public static extern IntPtr RailArrayRailID_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailID")]
		public static extern void delete_RailArrayRailID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailID_assign")]
		public static extern void RailArrayRailID_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailID_buf")]
		public static extern IntPtr RailArrayRailID_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailID_size")]
		public static extern uint RailArrayRailID_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailID_Item")]
		public static extern IntPtr RailArrayRailID_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailID_resize")]
		public static extern void RailArrayRailID_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailID_push_back")]
		public static extern void RailArrayRailID_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailID_clear")]
		public static extern void RailArrayRailID_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailID_erase")]
		public static extern void RailArrayRailID_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailQuerySpaceWorkInfoResult__SWIG_0")]
		public static extern IntPtr new_RailArrayRailQuerySpaceWorkInfoResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailQuerySpaceWorkInfoResult__SWIG_1")]
		public static extern IntPtr new_RailArrayRailQuerySpaceWorkInfoResult__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailQuerySpaceWorkInfoResult__SWIG_2")]
		public static extern IntPtr new_RailArrayRailQuerySpaceWorkInfoResult__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailQuerySpaceWorkInfoResult__SWIG_3")]
		public static extern IntPtr new_RailArrayRailQuerySpaceWorkInfoResult__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailQuerySpaceWorkInfoResult_SetValue")]
		public static extern IntPtr RailArrayRailQuerySpaceWorkInfoResult_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailQuerySpaceWorkInfoResult")]
		public static extern void delete_RailArrayRailQuerySpaceWorkInfoResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailQuerySpaceWorkInfoResult_assign")]
		public static extern void RailArrayRailQuerySpaceWorkInfoResult_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailQuerySpaceWorkInfoResult_buf")]
		public static extern IntPtr RailArrayRailQuerySpaceWorkInfoResult_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailQuerySpaceWorkInfoResult_size")]
		public static extern uint RailArrayRailQuerySpaceWorkInfoResult_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailQuerySpaceWorkInfoResult_Item")]
		public static extern IntPtr RailArrayRailQuerySpaceWorkInfoResult_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailQuerySpaceWorkInfoResult_resize")]
		public static extern void RailArrayRailQuerySpaceWorkInfoResult_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailQuerySpaceWorkInfoResult_push_back")]
		public static extern void RailArrayRailQuerySpaceWorkInfoResult_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailQuerySpaceWorkInfoResult_clear")]
		public static extern void RailArrayRailQuerySpaceWorkInfoResult_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailQuerySpaceWorkInfoResult_erase")]
		public static extern void RailArrayRailQuerySpaceWorkInfoResult_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailUserPlayedWith__SWIG_0")]
		public static extern IntPtr new_RailArrayRailUserPlayedWith__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailUserPlayedWith__SWIG_1")]
		public static extern IntPtr new_RailArrayRailUserPlayedWith__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailUserPlayedWith__SWIG_2")]
		public static extern IntPtr new_RailArrayRailUserPlayedWith__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailUserPlayedWith__SWIG_3")]
		public static extern IntPtr new_RailArrayRailUserPlayedWith__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserPlayedWith_SetValue")]
		public static extern IntPtr RailArrayRailUserPlayedWith_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailUserPlayedWith")]
		public static extern void delete_RailArrayRailUserPlayedWith(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserPlayedWith_assign")]
		public static extern void RailArrayRailUserPlayedWith_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserPlayedWith_buf")]
		public static extern IntPtr RailArrayRailUserPlayedWith_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserPlayedWith_size")]
		public static extern uint RailArrayRailUserPlayedWith_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserPlayedWith_Item")]
		public static extern IntPtr RailArrayRailUserPlayedWith_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserPlayedWith_resize")]
		public static extern void RailArrayRailUserPlayedWith_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserPlayedWith_push_back")]
		public static extern void RailArrayRailUserPlayedWith_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserPlayedWith_clear")]
		public static extern void RailArrayRailUserPlayedWith_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailUserPlayedWith_erase")]
		public static extern void RailArrayRailUserPlayedWith_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGameDefineGamePlayingState__SWIG_0")]
		public static extern IntPtr new_RailArrayRailGameDefineGamePlayingState__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGameDefineGamePlayingState__SWIG_1")]
		public static extern IntPtr new_RailArrayRailGameDefineGamePlayingState__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGameDefineGamePlayingState__SWIG_2")]
		public static extern IntPtr new_RailArrayRailGameDefineGamePlayingState__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGameDefineGamePlayingState__SWIG_3")]
		public static extern IntPtr new_RailArrayRailGameDefineGamePlayingState__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameDefineGamePlayingState_SetValue")]
		public static extern IntPtr RailArrayRailGameDefineGamePlayingState_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailGameDefineGamePlayingState")]
		public static extern void delete_RailArrayRailGameDefineGamePlayingState(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameDefineGamePlayingState_assign")]
		public static extern void RailArrayRailGameDefineGamePlayingState_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameDefineGamePlayingState_buf")]
		public static extern IntPtr RailArrayRailGameDefineGamePlayingState_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameDefineGamePlayingState_size")]
		public static extern uint RailArrayRailGameDefineGamePlayingState_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameDefineGamePlayingState_Item")]
		public static extern IntPtr RailArrayRailGameDefineGamePlayingState_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameDefineGamePlayingState_resize")]
		public static extern void RailArrayRailGameDefineGamePlayingState_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameDefineGamePlayingState_push_back")]
		public static extern void RailArrayRailGameDefineGamePlayingState_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameDefineGamePlayingState_clear")]
		public static extern void RailArrayRailGameDefineGamePlayingState_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGameDefineGamePlayingState_erase")]
		public static extern void RailArrayRailGameDefineGamePlayingState_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerInfo__SWIG_0")]
		public static extern IntPtr new_RailArrayGameServerInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerInfo__SWIG_1")]
		public static extern IntPtr new_RailArrayGameServerInfo__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerInfo__SWIG_2")]
		public static extern IntPtr new_RailArrayGameServerInfo__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerInfo__SWIG_3")]
		public static extern IntPtr new_RailArrayGameServerInfo__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerInfo_SetValue")]
		public static extern IntPtr RailArrayGameServerInfo_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayGameServerInfo")]
		public static extern void delete_RailArrayGameServerInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerInfo_assign")]
		public static extern void RailArrayGameServerInfo_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerInfo_buf")]
		public static extern IntPtr RailArrayGameServerInfo_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerInfo_size")]
		public static extern uint RailArrayGameServerInfo_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerInfo_Item")]
		public static extern IntPtr RailArrayGameServerInfo_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerInfo_resize")]
		public static extern void RailArrayGameServerInfo_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerInfo_push_back")]
		public static extern void RailArrayGameServerInfo_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerInfo_clear")]
		public static extern void RailArrayGameServerInfo_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerInfo_erase")]
		public static extern void RailArrayGameServerInfo_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfo__SWIG_0")]
		public static extern IntPtr new_RailArrayRoomInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfo__SWIG_1")]
		public static extern IntPtr new_RailArrayRoomInfo__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfo__SWIG_2")]
		public static extern IntPtr new_RailArrayRoomInfo__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRoomInfo__SWIG_3")]
		public static extern IntPtr new_RailArrayRoomInfo__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfo_SetValue")]
		public static extern IntPtr RailArrayRoomInfo_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRoomInfo")]
		public static extern void delete_RailArrayRoomInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfo_assign")]
		public static extern void RailArrayRoomInfo_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfo_buf")]
		public static extern IntPtr RailArrayRoomInfo_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfo_size")]
		public static extern uint RailArrayRoomInfo_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfo_Item")]
		public static extern IntPtr RailArrayRoomInfo_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfo_resize")]
		public static extern void RailArrayRoomInfo_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfo_push_back")]
		public static extern void RailArrayRoomInfo_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfo_clear")]
		public static extern void RailArrayRoomInfo_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRoomInfo_erase")]
		public static extern void RailArrayRoomInfo_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailPlayedWithFriendsTimeItem__SWIG_0")]
		public static extern IntPtr new_RailArrayRailPlayedWithFriendsTimeItem__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailPlayedWithFriendsTimeItem__SWIG_1")]
		public static extern IntPtr new_RailArrayRailPlayedWithFriendsTimeItem__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailPlayedWithFriendsTimeItem__SWIG_2")]
		public static extern IntPtr new_RailArrayRailPlayedWithFriendsTimeItem__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailPlayedWithFriendsTimeItem__SWIG_3")]
		public static extern IntPtr new_RailArrayRailPlayedWithFriendsTimeItem__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsTimeItem_SetValue")]
		public static extern IntPtr RailArrayRailPlayedWithFriendsTimeItem_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailPlayedWithFriendsTimeItem")]
		public static extern void delete_RailArrayRailPlayedWithFriendsTimeItem(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsTimeItem_assign")]
		public static extern void RailArrayRailPlayedWithFriendsTimeItem_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsTimeItem_buf")]
		public static extern IntPtr RailArrayRailPlayedWithFriendsTimeItem_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsTimeItem_size")]
		public static extern uint RailArrayRailPlayedWithFriendsTimeItem_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsTimeItem_Item")]
		public static extern IntPtr RailArrayRailPlayedWithFriendsTimeItem_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsTimeItem_resize")]
		public static extern void RailArrayRailPlayedWithFriendsTimeItem_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsTimeItem_push_back")]
		public static extern void RailArrayRailPlayedWithFriendsTimeItem_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsTimeItem_clear")]
		public static extern void RailArrayRailPlayedWithFriendsTimeItem_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailPlayedWithFriendsTimeItem_erase")]
		public static extern void RailArrayRailPlayedWithFriendsTimeItem_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGeneratedAssetItem__SWIG_0")]
		public static extern IntPtr new_RailArrayRailGeneratedAssetItem__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGeneratedAssetItem__SWIG_1")]
		public static extern IntPtr new_RailArrayRailGeneratedAssetItem__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGeneratedAssetItem__SWIG_2")]
		public static extern IntPtr new_RailArrayRailGeneratedAssetItem__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailGeneratedAssetItem__SWIG_3")]
		public static extern IntPtr new_RailArrayRailGeneratedAssetItem__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGeneratedAssetItem_SetValue")]
		public static extern IntPtr RailArrayRailGeneratedAssetItem_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailGeneratedAssetItem")]
		public static extern void delete_RailArrayRailGeneratedAssetItem(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGeneratedAssetItem_assign")]
		public static extern void RailArrayRailGeneratedAssetItem_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGeneratedAssetItem_buf")]
		public static extern IntPtr RailArrayRailGeneratedAssetItem_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGeneratedAssetItem_size")]
		public static extern uint RailArrayRailGeneratedAssetItem_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGeneratedAssetItem_Item")]
		public static extern IntPtr RailArrayRailGeneratedAssetItem_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGeneratedAssetItem_resize")]
		public static extern void RailArrayRailGeneratedAssetItem_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGeneratedAssetItem_push_back")]
		public static extern void RailArrayRailGeneratedAssetItem_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGeneratedAssetItem_clear")]
		public static extern void RailArrayRailGeneratedAssetItem_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailGeneratedAssetItem_erase")]
		public static extern void RailArrayRailGeneratedAssetItem_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailFriendMetadata__SWIG_0")]
		public static extern IntPtr new_RailArrayRailFriendMetadata__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailFriendMetadata__SWIG_1")]
		public static extern IntPtr new_RailArrayRailFriendMetadata__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailFriendMetadata__SWIG_2")]
		public static extern IntPtr new_RailArrayRailFriendMetadata__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailFriendMetadata__SWIG_3")]
		public static extern IntPtr new_RailArrayRailFriendMetadata__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendMetadata_SetValue")]
		public static extern IntPtr RailArrayRailFriendMetadata_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailFriendMetadata")]
		public static extern void delete_RailArrayRailFriendMetadata(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendMetadata_assign")]
		public static extern void RailArrayRailFriendMetadata_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendMetadata_buf")]
		public static extern IntPtr RailArrayRailFriendMetadata_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendMetadata_size")]
		public static extern uint RailArrayRailFriendMetadata_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendMetadata_Item")]
		public static extern IntPtr RailArrayRailFriendMetadata_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendMetadata_resize")]
		public static extern void RailArrayRailFriendMetadata_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendMetadata_push_back")]
		public static extern void RailArrayRailFriendMetadata_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendMetadata_clear")]
		public static extern void RailArrayRailFriendMetadata_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailFriendMetadata_erase")]
		public static extern void RailArrayRailFriendMetadata_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSmallObjectDownloadInfo__SWIG_0")]
		public static extern IntPtr new_RailArrayRailSmallObjectDownloadInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSmallObjectDownloadInfo__SWIG_1")]
		public static extern IntPtr new_RailArrayRailSmallObjectDownloadInfo__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSmallObjectDownloadInfo__SWIG_2")]
		public static extern IntPtr new_RailArrayRailSmallObjectDownloadInfo__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSmallObjectDownloadInfo__SWIG_3")]
		public static extern IntPtr new_RailArrayRailSmallObjectDownloadInfo__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectDownloadInfo_SetValue")]
		public static extern IntPtr RailArrayRailSmallObjectDownloadInfo_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailSmallObjectDownloadInfo")]
		public static extern void delete_RailArrayRailSmallObjectDownloadInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectDownloadInfo_assign")]
		public static extern void RailArrayRailSmallObjectDownloadInfo_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectDownloadInfo_buf")]
		public static extern IntPtr RailArrayRailSmallObjectDownloadInfo_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectDownloadInfo_size")]
		public static extern uint RailArrayRailSmallObjectDownloadInfo_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectDownloadInfo_Item")]
		public static extern IntPtr RailArrayRailSmallObjectDownloadInfo_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectDownloadInfo_resize")]
		public static extern void RailArrayRailSmallObjectDownloadInfo_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectDownloadInfo_push_back")]
		public static extern void RailArrayRailSmallObjectDownloadInfo_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectDownloadInfo_clear")]
		public static extern void RailArrayRailSmallObjectDownloadInfo_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSmallObjectDownloadInfo_erase")]
		public static extern void RailArrayRailSmallObjectDownloadInfo_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailDlcOwned__SWIG_0")]
		public static extern IntPtr new_RailArrayRailDlcOwned__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailDlcOwned__SWIG_1")]
		public static extern IntPtr new_RailArrayRailDlcOwned__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailDlcOwned__SWIG_2")]
		public static extern IntPtr new_RailArrayRailDlcOwned__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailDlcOwned__SWIG_3")]
		public static extern IntPtr new_RailArrayRailDlcOwned__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcOwned_SetValue")]
		public static extern IntPtr RailArrayRailDlcOwned_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailDlcOwned")]
		public static extern void delete_RailArrayRailDlcOwned(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcOwned_assign")]
		public static extern void RailArrayRailDlcOwned_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcOwned_buf")]
		public static extern IntPtr RailArrayRailDlcOwned_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcOwned_size")]
		public static extern uint RailArrayRailDlcOwned_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcOwned_Item")]
		public static extern IntPtr RailArrayRailDlcOwned_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcOwned_resize")]
		public static extern void RailArrayRailDlcOwned_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcOwned_push_back")]
		public static extern void RailArrayRailDlcOwned_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcOwned_clear")]
		public static extern void RailArrayRailDlcOwned_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailDlcOwned_erase")]
		public static extern void RailArrayRailDlcOwned_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSpaceWorkDescriptor__SWIG_0")]
		public static extern IntPtr new_RailArrayRailSpaceWorkDescriptor__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSpaceWorkDescriptor__SWIG_1")]
		public static extern IntPtr new_RailArrayRailSpaceWorkDescriptor__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSpaceWorkDescriptor__SWIG_2")]
		public static extern IntPtr new_RailArrayRailSpaceWorkDescriptor__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayRailSpaceWorkDescriptor__SWIG_3")]
		public static extern IntPtr new_RailArrayRailSpaceWorkDescriptor__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkDescriptor_SetValue")]
		public static extern IntPtr RailArrayRailSpaceWorkDescriptor_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayRailSpaceWorkDescriptor")]
		public static extern void delete_RailArrayRailSpaceWorkDescriptor(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkDescriptor_assign")]
		public static extern void RailArrayRailSpaceWorkDescriptor_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkDescriptor_buf")]
		public static extern IntPtr RailArrayRailSpaceWorkDescriptor_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkDescriptor_size")]
		public static extern uint RailArrayRailSpaceWorkDescriptor_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkDescriptor_Item")]
		public static extern IntPtr RailArrayRailSpaceWorkDescriptor_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkDescriptor_resize")]
		public static extern void RailArrayRailSpaceWorkDescriptor_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkDescriptor_push_back")]
		public static extern void RailArrayRailSpaceWorkDescriptor_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkDescriptor_clear")]
		public static extern void RailArrayRailSpaceWorkDescriptor_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayRailSpaceWorkDescriptor_erase")]
		public static extern void RailArrayRailSpaceWorkDescriptor_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerPlayerInfo__SWIG_0")]
		public static extern IntPtr new_RailArrayGameServerPlayerInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerPlayerInfo__SWIG_1")]
		public static extern IntPtr new_RailArrayGameServerPlayerInfo__SWIG_1(uint jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerPlayerInfo__SWIG_2")]
		public static extern IntPtr new_RailArrayGameServerPlayerInfo__SWIG_2(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailArrayGameServerPlayerInfo__SWIG_3")]
		public static extern IntPtr new_RailArrayGameServerPlayerInfo__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerPlayerInfo_SetValue")]
		public static extern IntPtr RailArrayGameServerPlayerInfo_SetValue(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailArrayGameServerPlayerInfo")]
		public static extern void delete_RailArrayGameServerPlayerInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerPlayerInfo_assign")]
		public static extern void RailArrayGameServerPlayerInfo_assign(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerPlayerInfo_buf")]
		public static extern IntPtr RailArrayGameServerPlayerInfo_buf(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerPlayerInfo_size")]
		public static extern uint RailArrayGameServerPlayerInfo_size(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerPlayerInfo_Item")]
		public static extern IntPtr RailArrayGameServerPlayerInfo_Item(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerPlayerInfo_resize")]
		public static extern void RailArrayGameServerPlayerInfo_resize(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerPlayerInfo_push_back")]
		public static extern void RailArrayGameServerPlayerInfo_push_back(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerPlayerInfo_clear")]
		public static extern void RailArrayGameServerPlayerInfo_clear(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailArrayGameServerPlayerInfo_erase")]
		public static extern void RailArrayGameServerPlayerInfo_erase(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsRequestAllAssetsFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsRequestAllAssetsFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsRequestAllAssetsFinished")]
		public static extern void delete_RailEventkRailEventAssetsRequestAllAssetsFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsRequestAllAssetsFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsRequestAllAssetsFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsRequestAllAssetsFinished_get_event_id")]
		public static extern int RailEventkRailEventAssetsRequestAllAssetsFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsRequestAllAssetsFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsRequestAllAssetsFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomSetRoomTypeResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomSetRoomTypeResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomSetRoomTypeResult")]
		public static extern void delete_RailEventkRailEventRoomSetRoomTypeResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetRoomTypeResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomSetRoomTypeResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetRoomTypeResult_get_event_id")]
		public static extern int RailEventkRailEventRoomSetRoomTypeResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomSetRoomTypeResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomSetRoomTypeResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsMergeToFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsMergeToFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsMergeToFinished")]
		public static extern void delete_RailEventkRailEventAssetsMergeToFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsMergeToFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsMergeToFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsMergeToFinished_get_event_id")]
		public static extern int RailEventkRailEventAssetsMergeToFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsMergeToFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsMergeToFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceSubscribeResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceSubscribeResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceSubscribeResult")]
		public static extern void delete_RailEventkRailEventUserSpaceSubscribeResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceSubscribeResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceSubscribeResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceSubscribeResult_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceSubscribeResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceSubscribeResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceSubscribeResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyMemberChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyMemberChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomNotifyMemberChanged")]
		public static extern void delete_RailEventkRailEventRoomNotifyMemberChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyMemberChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomNotifyMemberChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyMemberChanged_get_event_id")]
		public static extern int RailEventkRailEventRoomNotifyMemberChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyMemberChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyMemberChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGamePurchasePurchaseProductsResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGamePurchasePurchaseProductsResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGamePurchasePurchaseProductsResult")]
		public static extern void delete_RailEventkRailEventInGamePurchasePurchaseProductsResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchasePurchaseProductsResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGamePurchasePurchaseProductsResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchasePurchaseProductsResult_get_event_id")]
		public static extern int RailEventkRailEventInGamePurchasePurchaseProductsResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGamePurchasePurchaseProductsResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGamePurchasePurchaseProductsResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventPlayerGetEncryptedGameTicketResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventPlayerGetEncryptedGameTicketResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventPlayerGetEncryptedGameTicketResult")]
		public static extern void delete_RailEventkRailEventPlayerGetEncryptedGameTicketResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerGetEncryptedGameTicketResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventPlayerGetEncryptedGameTicketResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerGetEncryptedGameTicketResult_get_event_id")]
		public static extern int RailEventkRailEventPlayerGetEncryptedGameTicketResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventPlayerGetEncryptedGameTicketResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventPlayerGetEncryptedGameTicketResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerListResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventGameServerListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventGameServerListResult")]
		public static extern void delete_RailEventkRailEventGameServerListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerListResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventGameServerListResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerListResult_get_event_id")]
		public static extern int RailEventkRailEventGameServerListResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerListResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventGameServerListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomSetRoomMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomSetRoomMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomSetRoomMetadataResult")]
		public static extern void delete_RailEventkRailEventRoomSetRoomMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetRoomMetadataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomSetRoomMetadataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetRoomMetadataResult_get_event_id")]
		public static extern int RailEventkRailEventRoomSetRoomMetadataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomSetRoomMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomSetRoomMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserCloseResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventBrowserCloseResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventBrowserCloseResult")]
		public static extern void delete_RailEventkRailEventBrowserCloseResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserCloseResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventBrowserCloseResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserCloseResult_get_event_id")]
		public static extern int RailEventkRailEventBrowserCloseResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserCloseResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventBrowserCloseResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventNetworkCreateSessionRequest__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventNetworkCreateSessionRequest__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventNetworkCreateSessionRequest")]
		public static extern void delete_RailEventkRailEventNetworkCreateSessionRequest(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventNetworkCreateSessionRequest_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventNetworkCreateSessionRequest_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventNetworkCreateSessionRequest_get_event_id")]
		public static extern int RailEventkRailEventNetworkCreateSessionRequest_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventNetworkCreateSessionRequest__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventNetworkCreateSessionRequest__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsExchangeAssetsToFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsExchangeAssetsToFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsExchangeAssetsToFinished")]
		public static extern void delete_RailEventkRailEventAssetsExchangeAssetsToFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsExchangeAssetsToFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsExchangeAssetsToFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsExchangeAssetsToFinished_get_event_id")]
		public static extern int RailEventkRailEventAssetsExchangeAssetsToFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsExchangeAssetsToFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsExchangeAssetsToFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailPlatformNotifyEventJoinGameByUser__SWIG_0")]
		public static extern IntPtr new_RailEventkRailPlatformNotifyEventJoinGameByUser__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailPlatformNotifyEventJoinGameByUser")]
		public static extern void delete_RailEventkRailPlatformNotifyEventJoinGameByUser(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailPlatformNotifyEventJoinGameByUser_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailPlatformNotifyEventJoinGameByUser_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailPlatformNotifyEventJoinGameByUser_get_event_id")]
		public static extern int RailEventkRailPlatformNotifyEventJoinGameByUser_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailPlatformNotifyEventJoinGameByUser__SWIG_1")]
		public static extern IntPtr new_RailEventkRailPlatformNotifyEventJoinGameByUser__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcInstallProgress__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventDlcInstallProgress__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventDlcInstallProgress")]
		public static extern void delete_RailEventkRailEventDlcInstallProgress(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcInstallProgress_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventDlcInstallProgress_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcInstallProgress_get_event_id")]
		public static extern int RailEventkRailEventDlcInstallProgress_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcInstallProgress__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventDlcInstallProgress__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersNotifyInviter__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUsersNotifyInviter__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUsersNotifyInviter")]
		public static extern void delete_RailEventkRailEventUsersNotifyInviter(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersNotifyInviter_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUsersNotifyInviter_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersNotifyInviter_get_event_id")]
		public static extern int RailEventkRailEventUsersNotifyInviter_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersNotifyInviter__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUsersNotifyInviter__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsQueryPlayedWithFriendsListResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFriendsQueryPlayedWithFriendsListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFriendsQueryPlayedWithFriendsListResult")]
		public static extern void delete_RailEventkRailEventFriendsQueryPlayedWithFriendsListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsQueryPlayedWithFriendsListResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFriendsQueryPlayedWithFriendsListResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsQueryPlayedWithFriendsListResult_get_event_id")]
		public static extern int RailEventkRailEventFriendsQueryPlayedWithFriendsListResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsQueryPlayedWithFriendsListResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFriendsQueryPlayedWithFriendsListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventLeaderboardAsyncCreated__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventLeaderboardAsyncCreated__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventLeaderboardAsyncCreated")]
		public static extern void delete_RailEventkRailEventLeaderboardAsyncCreated(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardAsyncCreated_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventLeaderboardAsyncCreated_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardAsyncCreated_get_event_id")]
		public static extern int RailEventkRailEventLeaderboardAsyncCreated_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventLeaderboardAsyncCreated__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventLeaderboardAsyncCreated__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventLeaderboardAttachSpaceWork__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventLeaderboardAttachSpaceWork__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventLeaderboardAttachSpaceWork")]
		public static extern void delete_RailEventkRailEventLeaderboardAttachSpaceWork(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardAttachSpaceWork_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventLeaderboardAttachSpaceWork_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardAttachSpaceWork_get_event_id")]
		public static extern int RailEventkRailEventLeaderboardAttachSpaceWork_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventLeaderboardAttachSpaceWork__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventLeaderboardAttachSpaceWork__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsMetadataChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFriendsMetadataChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFriendsMetadataChanged")]
		public static extern void delete_RailEventkRailEventFriendsMetadataChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsMetadataChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFriendsMetadataChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsMetadataChanged_get_event_id")]
		public static extern int RailEventkRailEventFriendsMetadataChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsMetadataChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFriendsMetadataChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetUserRoomListResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomGetUserRoomListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomGetUserRoomListResult")]
		public static extern void delete_RailEventkRailEventRoomGetUserRoomListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetUserRoomListResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomGetUserRoomListResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetUserRoomListResult_get_event_id")]
		public static extern int RailEventkRailEventRoomGetUserRoomListResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetUserRoomListResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomGetUserRoomListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserTryNavigateNewPageRequest__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventBrowserTryNavigateNewPageRequest__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventBrowserTryNavigateNewPageRequest")]
		public static extern void delete_RailEventkRailEventBrowserTryNavigateNewPageRequest(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserTryNavigateNewPageRequest_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventBrowserTryNavigateNewPageRequest_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserTryNavigateNewPageRequest_get_event_id")]
		public static extern int RailEventkRailEventBrowserTryNavigateNewPageRequest_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserTryNavigateNewPageRequest__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventBrowserTryNavigateNewPageRequest__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcUninstallFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventDlcUninstallFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventDlcUninstallFinished")]
		public static extern void delete_RailEventkRailEventDlcUninstallFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcUninstallFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventDlcUninstallFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcUninstallFinished_get_event_id")]
		public static extern int RailEventkRailEventDlcUninstallFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcUninstallFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventDlcUninstallFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsSplitFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsSplitFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsSplitFinished")]
		public static extern void delete_RailEventkRailEventAssetsSplitFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsSplitFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsSplitFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsSplitFinished_get_event_id")]
		public static extern int RailEventkRailEventAssetsSplitFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsSplitFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsSplitFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsQueryPlayedWithFriendsGamesResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFriendsQueryPlayedWithFriendsGamesResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFriendsQueryPlayedWithFriendsGamesResult")]
		public static extern void delete_RailEventkRailEventFriendsQueryPlayedWithFriendsGamesResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsQueryPlayedWithFriendsGamesResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFriendsQueryPlayedWithFriendsGamesResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsQueryPlayedWithFriendsGamesResult_get_event_id")]
		public static extern int RailEventkRailEventFriendsQueryPlayedWithFriendsGamesResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsQueryPlayedWithFriendsGamesResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFriendsQueryPlayedWithFriendsGamesResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventLeaderboardUploaded__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventLeaderboardUploaded__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventLeaderboardUploaded")]
		public static extern void delete_RailEventkRailEventLeaderboardUploaded(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardUploaded_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventLeaderboardUploaded_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardUploaded_get_event_id")]
		public static extern int RailEventkRailEventLeaderboardUploaded_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventLeaderboardUploaded__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventLeaderboardUploaded__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelRemoveUsersResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelRemoveUsersResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventVoiceChannelRemoveUsersResult")]
		public static extern void delete_RailEventkRailEventVoiceChannelRemoveUsersResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelRemoveUsersResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventVoiceChannelRemoveUsersResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelRemoveUsersResult_get_event_id")]
		public static extern int RailEventkRailEventVoiceChannelRemoveUsersResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelRemoveUsersResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelRemoveUsersResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventSmallObjectServiceQueryObjectStateResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventSmallObjectServiceQueryObjectStateResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventSmallObjectServiceQueryObjectStateResult")]
		public static extern void delete_RailEventkRailEventSmallObjectServiceQueryObjectStateResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSmallObjectServiceQueryObjectStateResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventSmallObjectServiceQueryObjectStateResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSmallObjectServiceQueryObjectStateResult_get_event_id")]
		public static extern int RailEventkRailEventSmallObjectServiceQueryObjectStateResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventSmallObjectServiceQueryObjectStateResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventSmallObjectServiceQueryObjectStateResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUtilsGameSettingMetadataChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUtilsGameSettingMetadataChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUtilsGameSettingMetadataChanged")]
		public static extern void delete_RailEventkRailEventUtilsGameSettingMetadataChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUtilsGameSettingMetadataChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUtilsGameSettingMetadataChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUtilsGameSettingMetadataChanged_get_event_id")]
		public static extern int RailEventkRailEventUtilsGameSettingMetadataChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUtilsGameSettingMetadataChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUtilsGameSettingMetadataChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerCreated__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventGameServerCreated__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventGameServerCreated")]
		public static extern void delete_RailEventkRailEventGameServerCreated(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerCreated_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventGameServerCreated_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerCreated_get_event_id")]
		public static extern int RailEventkRailEventGameServerCreated_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerCreated__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventGameServerCreated__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcInstallStart__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventDlcInstallStart__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventDlcInstallStart")]
		public static extern void delete_RailEventkRailEventDlcInstallStart(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcInstallStart_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventDlcInstallStart_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcInstallStart_get_event_id")]
		public static extern int RailEventkRailEventDlcInstallStart_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcInstallStart__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventDlcInstallStart__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersGetInviteDetailResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUsersGetInviteDetailResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUsersGetInviteDetailResult")]
		public static extern void delete_RailEventkRailEventUsersGetInviteDetailResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersGetInviteDetailResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUsersGetInviteDetailResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersGetInviteDetailResult_get_event_id")]
		public static extern int RailEventkRailEventUsersGetInviteDetailResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersGetInviteDetailResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUsersGetInviteDetailResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceQuerySpaceWorksResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceQuerySpaceWorksResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceQuerySpaceWorksResult")]
		public static extern void delete_RailEventkRailEventUserSpaceQuerySpaceWorksResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceQuerySpaceWorksResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceQuerySpaceWorksResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceQuerySpaceWorksResult_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceQuerySpaceWorksResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceQuerySpaceWorksResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceQuerySpaceWorksResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAchievementGlobalAchievementReceived__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAchievementGlobalAchievementReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAchievementGlobalAchievementReceived")]
		public static extern void delete_RailEventkRailEventAchievementGlobalAchievementReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAchievementGlobalAchievementReceived_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAchievementGlobalAchievementReceived_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAchievementGlobalAchievementReceived_get_event_id")]
		public static extern int RailEventkRailEventAchievementGlobalAchievementReceived_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAchievementGlobalAchievementReceived__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAchievementGlobalAchievementReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncRenameStreamFileResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncRenameStreamFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventStorageAsyncRenameStreamFileResult")]
		public static extern void delete_RailEventkRailEventStorageAsyncRenameStreamFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncRenameStreamFileResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventStorageAsyncRenameStreamFileResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncRenameStreamFileResult_get_event_id")]
		public static extern int RailEventkRailEventStorageAsyncRenameStreamFileResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncRenameStreamFileResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncRenameStreamFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsReportPlayedWithUserListResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFriendsReportPlayedWithUserListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFriendsReportPlayedWithUserListResult")]
		public static extern void delete_RailEventkRailEventFriendsReportPlayedWithUserListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsReportPlayedWithUserListResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFriendsReportPlayedWithUserListResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsReportPlayedWithUserListResult_get_event_id")]
		public static extern int RailEventkRailEventFriendsReportPlayedWithUserListResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsReportPlayedWithUserListResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFriendsReportPlayedWithUserListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelInviteEvent__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelInviteEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventVoiceChannelInviteEvent")]
		public static extern void delete_RailEventkRailEventVoiceChannelInviteEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelInviteEvent_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventVoiceChannelInviteEvent_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelInviteEvent_get_event_id")]
		public static extern int RailEventkRailEventVoiceChannelInviteEvent_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelInviteEvent__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelInviteEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncWriteStreamFileResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncWriteStreamFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventStorageAsyncWriteStreamFileResult")]
		public static extern void delete_RailEventkRailEventStorageAsyncWriteStreamFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncWriteStreamFileResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventStorageAsyncWriteStreamFileResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncWriteStreamFileResult_get_event_id")]
		public static extern int RailEventkRailEventStorageAsyncWriteStreamFileResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncWriteStreamFileResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncWriteStreamFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomKickOffMemberResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomKickOffMemberResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomKickOffMemberResult")]
		public static extern void delete_RailEventkRailEventRoomKickOffMemberResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomKickOffMemberResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomKickOffMemberResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomKickOffMemberResult_get_event_id")]
		public static extern int RailEventkRailEventRoomKickOffMemberResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomKickOffMemberResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomKickOffMemberResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventShowFloatingWindow__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventShowFloatingWindow__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventShowFloatingWindow")]
		public static extern void delete_RailEventkRailEventShowFloatingWindow(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventShowFloatingWindow_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventShowFloatingWindow_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventShowFloatingWindow_get_event_id")]
		public static extern int RailEventkRailEventShowFloatingWindow_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventShowFloatingWindow__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventShowFloatingWindow__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyRoomOwnerChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyRoomOwnerChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomNotifyRoomOwnerChanged")]
		public static extern void delete_RailEventkRailEventRoomNotifyRoomOwnerChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyRoomOwnerChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomNotifyRoomOwnerChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyRoomOwnerChanged_get_event_id")]
		public static extern int RailEventkRailEventRoomNotifyRoomOwnerChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyRoomOwnerChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyRoomOwnerChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFinalize__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFinalize__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFinalize")]
		public static extern void delete_RailEventkRailEventFinalize(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFinalize_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFinalize_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFinalize_get_event_id")]
		public static extern int RailEventkRailEventFinalize_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFinalize__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFinalize__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersCancelInviteResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUsersCancelInviteResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUsersCancelInviteResult")]
		public static extern void delete_RailEventkRailEventUsersCancelInviteResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersCancelInviteResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUsersCancelInviteResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersCancelInviteResult_get_event_id")]
		public static extern int RailEventkRailEventUsersCancelInviteResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersCancelInviteResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUsersCancelInviteResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcInstallStartResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventDlcInstallStartResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventDlcInstallStartResult")]
		public static extern void delete_RailEventkRailEventDlcInstallStartResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcInstallStartResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventDlcInstallStartResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcInstallStartResult_get_event_id")]
		public static extern int RailEventkRailEventDlcInstallStartResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcInstallStartResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventDlcInstallStartResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailThirdPartyAccountLoginResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailThirdPartyAccountLoginResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailThirdPartyAccountLoginResult")]
		public static extern void delete_RailEventkRailThirdPartyAccountLoginResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailThirdPartyAccountLoginResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailThirdPartyAccountLoginResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailThirdPartyAccountLoginResult_get_event_id")]
		public static extern int RailEventkRailThirdPartyAccountLoginResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailThirdPartyAccountLoginResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailThirdPartyAccountLoginResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserDamageRectPaint__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventBrowserDamageRectPaint__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventBrowserDamageRectPaint")]
		public static extern void delete_RailEventkRailEventBrowserDamageRectPaint(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserDamageRectPaint_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventBrowserDamageRectPaint_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserDamageRectPaint_get_event_id")]
		public static extern int RailEventkRailEventBrowserDamageRectPaint_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserDamageRectPaint__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventBrowserDamageRectPaint__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsUpdateAssetPropertyFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsUpdateAssetPropertyFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsUpdateAssetPropertyFinished")]
		public static extern void delete_RailEventkRailEventAssetsUpdateAssetPropertyFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsUpdateAssetPropertyFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsUpdateAssetPropertyFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsUpdateAssetPropertyFinished_get_event_id")]
		public static extern int RailEventkRailEventAssetsUpdateAssetPropertyFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsUpdateAssetPropertyFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsUpdateAssetPropertyFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetRoomMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomGetRoomMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomGetRoomMetadataResult")]
		public static extern void delete_RailEventkRailEventRoomGetRoomMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetRoomMetadataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomGetRoomMetadataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetRoomMetadataResult_get_event_id")]
		public static extern int RailEventkRailEventRoomGetRoomMetadataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetRoomMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomGetRoomMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventZoneServerSwitchPlayerSelectedZoneResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventZoneServerSwitchPlayerSelectedZoneResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventZoneServerSwitchPlayerSelectedZoneResult")]
		public static extern void delete_RailEventkRailEventZoneServerSwitchPlayerSelectedZoneResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventZoneServerSwitchPlayerSelectedZoneResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventZoneServerSwitchPlayerSelectedZoneResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventZoneServerSwitchPlayerSelectedZoneResult_get_event_id")]
		public static extern int RailEventkRailEventZoneServerSwitchPlayerSelectedZoneResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventZoneServerSwitchPlayerSelectedZoneResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventZoneServerSwitchPlayerSelectedZoneResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventPlayerGetGamePurchaseKey__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventPlayerGetGamePurchaseKey__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventPlayerGetGamePurchaseKey")]
		public static extern void delete_RailEventkRailEventPlayerGetGamePurchaseKey(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerGetGamePurchaseKey_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventPlayerGetGamePurchaseKey_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerGetGamePurchaseKey_get_event_id")]
		public static extern int RailEventkRailEventPlayerGetGamePurchaseKey_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventPlayerGetGamePurchaseKey__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventPlayerGetGamePurchaseKey__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailPlatformNotifyEventJoinGameByRoom__SWIG_0")]
		public static extern IntPtr new_RailEventkRailPlatformNotifyEventJoinGameByRoom__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailPlatformNotifyEventJoinGameByRoom")]
		public static extern void delete_RailEventkRailPlatformNotifyEventJoinGameByRoom(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailPlatformNotifyEventJoinGameByRoom_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailPlatformNotifyEventJoinGameByRoom_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailPlatformNotifyEventJoinGameByRoom_get_event_id")]
		public static extern int RailEventkRailPlatformNotifyEventJoinGameByRoom_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailPlatformNotifyEventJoinGameByRoom__SWIG_1")]
		public static extern IntPtr new_RailEventkRailPlatformNotifyEventJoinGameByRoom__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomSetMemberMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomSetMemberMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomSetMemberMetadataResult")]
		public static extern void delete_RailEventkRailEventRoomSetMemberMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetMemberMetadataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomSetMemberMetadataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetMemberMetadataResult_get_event_id")]
		public static extern int RailEventkRailEventRoomSetMemberMetadataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomSetMemberMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomSetMemberMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceModifyFavoritesWorksResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceModifyFavoritesWorksResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceModifyFavoritesWorksResult")]
		public static extern void delete_RailEventkRailEventUserSpaceModifyFavoritesWorksResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceModifyFavoritesWorksResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceModifyFavoritesWorksResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceModifyFavoritesWorksResult_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceModifyFavoritesWorksResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceModifyFavoritesWorksResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceModifyFavoritesWorksResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelSpeakingUsersChangedEvent__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelSpeakingUsersChangedEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventVoiceChannelSpeakingUsersChangedEvent")]
		public static extern void delete_RailEventkRailEventVoiceChannelSpeakingUsersChangedEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelSpeakingUsersChangedEvent_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventVoiceChannelSpeakingUsersChangedEvent_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelSpeakingUsersChangedEvent_get_event_id")]
		public static extern int RailEventkRailEventVoiceChannelSpeakingUsersChangedEvent_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelSpeakingUsersChangedEvent__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelSpeakingUsersChangedEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersRespondInvitation__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUsersRespondInvitation__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUsersRespondInvitation")]
		public static extern void delete_RailEventkRailEventUsersRespondInvitation(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersRespondInvitation_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUsersRespondInvitation_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersRespondInvitation_get_event_id")]
		public static extern int RailEventkRailEventUsersRespondInvitation_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersRespondInvitation__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUsersRespondInvitation__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsDirectConsumeFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsDirectConsumeFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsDirectConsumeFinished")]
		public static extern void delete_RailEventkRailEventAssetsDirectConsumeFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsDirectConsumeFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsDirectConsumeFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsDirectConsumeFinished_get_event_id")]
		public static extern int RailEventkRailEventAssetsDirectConsumeFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsDirectConsumeFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsDirectConsumeFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncWriteFileResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncWriteFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventStorageAsyncWriteFileResult")]
		public static extern void delete_RailEventkRailEventStorageAsyncWriteFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncWriteFileResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventStorageAsyncWriteFileResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncWriteFileResult_get_event_id")]
		public static extern int RailEventkRailEventStorageAsyncWriteFileResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncWriteFileResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncWriteFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceGetMyFavoritesWorksResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceGetMyFavoritesWorksResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceGetMyFavoritesWorksResult")]
		public static extern void delete_RailEventkRailEventUserSpaceGetMyFavoritesWorksResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceGetMyFavoritesWorksResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceGetMyFavoritesWorksResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceGetMyFavoritesWorksResult_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceGetMyFavoritesWorksResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceGetMyFavoritesWorksResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceGetMyFavoritesWorksResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyRoomDestroyed__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyRoomDestroyed__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomNotifyRoomDestroyed")]
		public static extern void delete_RailEventkRailEventRoomNotifyRoomDestroyed(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyRoomDestroyed_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomNotifyRoomDestroyed_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyRoomDestroyed_get_event_id")]
		public static extern int RailEventkRailEventRoomNotifyRoomDestroyed_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyRoomDestroyed__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyRoomDestroyed__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserPaint__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventBrowserPaint__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventBrowserPaint")]
		public static extern void delete_RailEventkRailEventBrowserPaint(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserPaint_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventBrowserPaint_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserPaint_get_event_id")]
		public static extern int RailEventkRailEventBrowserPaint_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserPaint__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventBrowserPaint__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsExchangeAssetsFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsExchangeAssetsFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsExchangeAssetsFinished")]
		public static extern void delete_RailEventkRailEventAssetsExchangeAssetsFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsExchangeAssetsFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsExchangeAssetsFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsExchangeAssetsFinished_get_event_id")]
		public static extern int RailEventkRailEventAssetsExchangeAssetsFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsExchangeAssetsFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsExchangeAssetsFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGamePurchaseAllPurchasableProductsInfoReceived__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGamePurchaseAllPurchasableProductsInfoReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGamePurchaseAllPurchasableProductsInfoReceived")]
		public static extern void delete_RailEventkRailEventInGamePurchaseAllPurchasableProductsInfoReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchaseAllPurchasableProductsInfoReceived_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGamePurchaseAllPurchasableProductsInfoReceived_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchaseAllPurchasableProductsInfoReceived_get_event_id")]
		public static extern int RailEventkRailEventInGamePurchaseAllPurchasableProductsInfoReceived_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGamePurchaseAllPurchasableProductsInfoReceived__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGamePurchaseAllPurchasableProductsInfoReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsGetFriendPlayedGamesResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFriendsGetFriendPlayedGamesResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFriendsGetFriendPlayedGamesResult")]
		public static extern void delete_RailEventkRailEventFriendsGetFriendPlayedGamesResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsGetFriendPlayedGamesResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFriendsGetFriendPlayedGamesResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsGetFriendPlayedGamesResult_get_event_id")]
		public static extern int RailEventkRailEventFriendsGetFriendPlayedGamesResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsGetFriendPlayedGamesResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFriendsGetFriendPlayedGamesResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsClearMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFriendsClearMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFriendsClearMetadataResult")]
		public static extern void delete_RailEventkRailEventFriendsClearMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsClearMetadataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFriendsClearMetadataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsClearMetadataResult_get_event_id")]
		public static extern int RailEventkRailEventFriendsClearMetadataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsClearMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFriendsClearMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelMemberChangedEvent__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelMemberChangedEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventVoiceChannelMemberChangedEvent")]
		public static extern void delete_RailEventkRailEventVoiceChannelMemberChangedEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelMemberChangedEvent_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventVoiceChannelMemberChangedEvent_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelMemberChangedEvent_get_event_id")]
		public static extern int RailEventkRailEventVoiceChannelMemberChangedEvent_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelMemberChangedEvent__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelMemberChangedEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsAddFriendResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFriendsAddFriendResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFriendsAddFriendResult")]
		public static extern void delete_RailEventkRailEventFriendsAddFriendResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsAddFriendResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFriendsAddFriendResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsAddFriendResult_get_event_id")]
		public static extern int RailEventkRailEventFriendsAddFriendResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsAddFriendResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFriendsAddFriendResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGamePurchaseAllProductsInfoReceived__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGamePurchaseAllProductsInfoReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGamePurchaseAllProductsInfoReceived")]
		public static extern void delete_RailEventkRailEventInGamePurchaseAllProductsInfoReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchaseAllProductsInfoReceived_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGamePurchaseAllProductsInfoReceived_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchaseAllProductsInfoReceived_get_event_id")]
		public static extern int RailEventkRailEventInGamePurchaseAllProductsInfoReceived_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGamePurchaseAllProductsInfoReceived__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGamePurchaseAllProductsInfoReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsMergeFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsMergeFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsMergeFinished")]
		public static extern void delete_RailEventkRailEventAssetsMergeFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsMergeFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsMergeFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsMergeFinished_get_event_id")]
		public static extern int RailEventkRailEventAssetsMergeFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsMergeFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsMergeFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyMemberkicked__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyMemberkicked__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomNotifyMemberkicked")]
		public static extern void delete_RailEventkRailEventRoomNotifyMemberkicked(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyMemberkicked_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomNotifyMemberkicked_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyMemberkicked_get_event_id")]
		public static extern int RailEventkRailEventRoomNotifyMemberkicked_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyMemberkicked__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyMemberkicked__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomJoinRoomResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomJoinRoomResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomJoinRoomResult")]
		public static extern void delete_RailEventkRailEventRoomJoinRoomResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomJoinRoomResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomJoinRoomResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomJoinRoomResult_get_event_id")]
		public static extern int RailEventkRailEventRoomJoinRoomResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomJoinRoomResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomJoinRoomResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerAuthSessionTicket__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventGameServerAuthSessionTicket__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventGameServerAuthSessionTicket")]
		public static extern void delete_RailEventkRailEventGameServerAuthSessionTicket(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerAuthSessionTicket_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventGameServerAuthSessionTicket_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerAuthSessionTicket_get_event_id")]
		public static extern int RailEventkRailEventGameServerAuthSessionTicket_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerAuthSessionTicket__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventGameServerAuthSessionTicket__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsQueryPlayedWithFriendsTimeResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFriendsQueryPlayedWithFriendsTimeResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFriendsQueryPlayedWithFriendsTimeResult")]
		public static extern void delete_RailEventkRailEventFriendsQueryPlayedWithFriendsTimeResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsQueryPlayedWithFriendsTimeResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFriendsQueryPlayedWithFriendsTimeResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsQueryPlayedWithFriendsTimeResult_get_event_id")]
		public static extern int RailEventkRailEventFriendsQueryPlayedWithFriendsTimeResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsQueryPlayedWithFriendsTimeResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFriendsQueryPlayedWithFriendsTimeResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerRegisterToServerListResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventGameServerRegisterToServerListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventGameServerRegisterToServerListResult")]
		public static extern void delete_RailEventkRailEventGameServerRegisterToServerListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerRegisterToServerListResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventGameServerRegisterToServerListResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerRegisterToServerListResult_get_event_id")]
		public static extern int RailEventkRailEventGameServerRegisterToServerListResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerRegisterToServerListResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventGameServerRegisterToServerListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameStorePurchasePaymentResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGameStorePurchasePaymentResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGameStorePurchasePaymentResult")]
		public static extern void delete_RailEventkRailEventInGameStorePurchasePaymentResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameStorePurchasePaymentResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGameStorePurchasePaymentResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameStorePurchasePaymentResult_get_event_id")]
		public static extern int RailEventkRailEventInGameStorePurchasePaymentResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameStorePurchasePaymentResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGameStorePurchasePaymentResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcCheckAllDlcsStateReadyResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventDlcCheckAllDlcsStateReadyResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventDlcCheckAllDlcsStateReadyResult")]
		public static extern void delete_RailEventkRailEventDlcCheckAllDlcsStateReadyResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcCheckAllDlcsStateReadyResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventDlcCheckAllDlcsStateReadyResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcCheckAllDlcsStateReadyResult_get_event_id")]
		public static extern int RailEventkRailEventDlcCheckAllDlcsStateReadyResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcCheckAllDlcsStateReadyResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventDlcCheckAllDlcsStateReadyResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventScreenshotTakeScreenshotFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventScreenshotTakeScreenshotFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventScreenshotTakeScreenshotFinished")]
		public static extern void delete_RailEventkRailEventScreenshotTakeScreenshotFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventScreenshotTakeScreenshotFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventScreenshotTakeScreenshotFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventScreenshotTakeScreenshotFinished_get_event_id")]
		public static extern int RailEventkRailEventScreenshotTakeScreenshotFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventScreenshotTakeScreenshotFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventScreenshotTakeScreenshotFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameActivityNotifyNewGameActivities__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGameActivityNotifyNewGameActivities__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGameActivityNotifyNewGameActivities")]
		public static extern void delete_RailEventkRailEventInGameActivityNotifyNewGameActivities(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameActivityNotifyNewGameActivities_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGameActivityNotifyNewGameActivities_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameActivityNotifyNewGameActivities_get_event_id")]
		public static extern int RailEventkRailEventInGameActivityNotifyNewGameActivities_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameActivityNotifyNewGameActivities__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGameActivityNotifyNewGameActivities__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventShowFloatingNotifyWindow__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventShowFloatingNotifyWindow__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventShowFloatingNotifyWindow")]
		public static extern void delete_RailEventkRailEventShowFloatingNotifyWindow(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventShowFloatingNotifyWindow_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventShowFloatingNotifyWindow_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventShowFloatingNotifyWindow_get_event_id")]
		public static extern int RailEventkRailEventShowFloatingNotifyWindow_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventShowFloatingNotifyWindow__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventShowFloatingNotifyWindow__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGamePurchasePurchaseProductsToAssetsResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGamePurchasePurchaseProductsToAssetsResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGamePurchasePurchaseProductsToAssetsResult")]
		public static extern void delete_RailEventkRailEventInGamePurchasePurchaseProductsToAssetsResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchasePurchaseProductsToAssetsResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGamePurchasePurchaseProductsToAssetsResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchasePurchaseProductsToAssetsResult_get_event_id")]
		public static extern int RailEventkRailEventInGamePurchasePurchaseProductsToAssetsResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGamePurchasePurchaseProductsToAssetsResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGamePurchasePurchaseProductsToAssetsResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserReloadResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventBrowserReloadResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventBrowserReloadResult")]
		public static extern void delete_RailEventkRailEventBrowserReloadResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserReloadResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventBrowserReloadResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserReloadResult_get_event_id")]
		public static extern int RailEventkRailEventBrowserReloadResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserReloadResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventBrowserReloadResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomSetRoomMaxMemberResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomSetRoomMaxMemberResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomSetRoomMaxMemberResult")]
		public static extern void delete_RailEventkRailEventRoomSetRoomMaxMemberResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetRoomMaxMemberResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomSetRoomMaxMemberResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetRoomMaxMemberResult_get_event_id")]
		public static extern int RailEventkRailEventRoomSetRoomMaxMemberResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomSetRoomMaxMemberResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomSetRoomMaxMemberResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersShowUserHomepageWindowResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUsersShowUserHomepageWindowResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUsersShowUserHomepageWindowResult")]
		public static extern void delete_RailEventkRailEventUsersShowUserHomepageWindowResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersShowUserHomepageWindowResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUsersShowUserHomepageWindowResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersShowUserHomepageWindowResult_get_event_id")]
		public static extern int RailEventkRailEventUsersShowUserHomepageWindowResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersShowUserHomepageWindowResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUsersShowUserHomepageWindowResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameStorePurchasePayWindowDisplayed__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGameStorePurchasePayWindowDisplayed__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGameStorePurchasePayWindowDisplayed")]
		public static extern void delete_RailEventkRailEventInGameStorePurchasePayWindowDisplayed(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameStorePurchasePayWindowDisplayed_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGameStorePurchasePayWindowDisplayed_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameStorePurchasePayWindowDisplayed_get_event_id")]
		public static extern int RailEventkRailEventInGameStorePurchasePayWindowDisplayed_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameStorePurchasePayWindowDisplayed__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGameStorePurchasePayWindowDisplayed__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventNetworkCreateRawSessionRequest__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventNetworkCreateRawSessionRequest__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventNetworkCreateRawSessionRequest")]
		public static extern void delete_RailEventkRailEventNetworkCreateRawSessionRequest(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventNetworkCreateRawSessionRequest_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventNetworkCreateRawSessionRequest_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventNetworkCreateRawSessionRequest_get_event_id")]
		public static extern int RailEventkRailEventNetworkCreateRawSessionRequest_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventNetworkCreateRawSessionRequest__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventNetworkCreateRawSessionRequest__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventIMEHelperTextInputSelectedResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventIMEHelperTextInputSelectedResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventIMEHelperTextInputSelectedResult")]
		public static extern void delete_RailEventkRailEventIMEHelperTextInputSelectedResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventIMEHelperTextInputSelectedResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventIMEHelperTextInputSelectedResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventIMEHelperTextInputSelectedResult_get_event_id")]
		public static extern int RailEventkRailEventIMEHelperTextInputSelectedResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventIMEHelperTextInputSelectedResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventIMEHelperTextInputSelectedResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGroupChatQueryGroupsInfoResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventGroupChatQueryGroupsInfoResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventGroupChatQueryGroupsInfoResult")]
		public static extern void delete_RailEventkRailEventGroupChatQueryGroupsInfoResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGroupChatQueryGroupsInfoResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventGroupChatQueryGroupsInfoResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGroupChatQueryGroupsInfoResult_get_event_id")]
		public static extern int RailEventkRailEventGroupChatQueryGroupsInfoResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGroupChatQueryGroupsInfoResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventGroupChatQueryGroupsInfoResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventTextInputShowTextInputWindowResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventTextInputShowTextInputWindowResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventTextInputShowTextInputWindowResult")]
		public static extern void delete_RailEventkRailEventTextInputShowTextInputWindowResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventTextInputShowTextInputWindowResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventTextInputShowTextInputWindowResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventTextInputShowTextInputWindowResult_get_event_id")]
		public static extern int RailEventkRailEventTextInputShowTextInputWindowResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventTextInputShowTextInputWindowResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventTextInputShowTextInputWindowResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerSetMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventGameServerSetMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventGameServerSetMetadataResult")]
		public static extern void delete_RailEventkRailEventGameServerSetMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerSetMetadataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventGameServerSetMetadataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerSetMetadataResult_get_event_id")]
		public static extern int RailEventkRailEventGameServerSetMetadataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerSetMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventGameServerSetMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceDownloadProgress__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceDownloadProgress__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceDownloadProgress")]
		public static extern void delete_RailEventkRailEventUserSpaceDownloadProgress(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceDownloadProgress_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceDownloadProgress_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceDownloadProgress_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceDownloadProgress_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceDownloadProgress__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceDownloadProgress__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAntiAddictionQueryGameOnlineTimeResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAntiAddictionQueryGameOnlineTimeResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAntiAddictionQueryGameOnlineTimeResult")]
		public static extern void delete_RailEventkRailEventAntiAddictionQueryGameOnlineTimeResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAntiAddictionQueryGameOnlineTimeResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAntiAddictionQueryGameOnlineTimeResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAntiAddictionQueryGameOnlineTimeResult_get_event_id")]
		public static extern int RailEventkRailEventAntiAddictionQueryGameOnlineTimeResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAntiAddictionQueryGameOnlineTimeResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAntiAddictionQueryGameOnlineTimeResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceQuerySpaceWorksInfoResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceQuerySpaceWorksInfoResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceQuerySpaceWorksInfoResult")]
		public static extern void delete_RailEventkRailEventUserSpaceQuerySpaceWorksInfoResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceQuerySpaceWorksInfoResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceQuerySpaceWorksInfoResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceQuerySpaceWorksInfoResult_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceQuerySpaceWorksInfoResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceQuerySpaceWorksInfoResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceQuerySpaceWorksInfoResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameCoinRequestCoinInfoResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGameCoinRequestCoinInfoResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGameCoinRequestCoinInfoResult")]
		public static extern void delete_RailEventkRailEventInGameCoinRequestCoinInfoResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameCoinRequestCoinInfoResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGameCoinRequestCoinInfoResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameCoinRequestCoinInfoResult_get_event_id")]
		public static extern int RailEventkRailEventInGameCoinRequestCoinInfoResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameCoinRequestCoinInfoResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGameCoinRequestCoinInfoResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsOnlineStateChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFriendsOnlineStateChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFriendsOnlineStateChanged")]
		public static extern void delete_RailEventkRailEventFriendsOnlineStateChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsOnlineStateChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFriendsOnlineStateChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsOnlineStateChanged_get_event_id")]
		public static extern int RailEventkRailEventFriendsOnlineStateChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsOnlineStateChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFriendsOnlineStateChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceUpdateMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceUpdateMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceUpdateMetadataResult")]
		public static extern void delete_RailEventkRailEventUserSpaceUpdateMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceUpdateMetadataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceUpdateMetadataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceUpdateMetadataResult_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceUpdateMetadataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceUpdateMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceUpdateMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailThirdPartyAccountLoginNotifyQrCodeInfo__SWIG_0")]
		public static extern IntPtr new_RailEventkRailThirdPartyAccountLoginNotifyQrCodeInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailThirdPartyAccountLoginNotifyQrCodeInfo")]
		public static extern void delete_RailEventkRailThirdPartyAccountLoginNotifyQrCodeInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailThirdPartyAccountLoginNotifyQrCodeInfo_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailThirdPartyAccountLoginNotifyQrCodeInfo_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailThirdPartyAccountLoginNotifyQrCodeInfo_get_event_id")]
		public static extern int RailEventkRailThirdPartyAccountLoginNotifyQrCodeInfo_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailThirdPartyAccountLoginNotifyQrCodeInfo__SWIG_1")]
		public static extern IntPtr new_RailEventkRailThirdPartyAccountLoginNotifyQrCodeInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsUpdateConsumeFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsUpdateConsumeFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsUpdateConsumeFinished")]
		public static extern void delete_RailEventkRailEventAssetsUpdateConsumeFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsUpdateConsumeFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsUpdateConsumeFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsUpdateConsumeFinished_get_event_id")]
		public static extern int RailEventkRailEventAssetsUpdateConsumeFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsUpdateConsumeFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsUpdateConsumeFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetRoomMembersResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomGetRoomMembersResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomGetRoomMembersResult")]
		public static extern void delete_RailEventkRailEventRoomGetRoomMembersResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetRoomMembersResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomGetRoomMembersResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetRoomMembersResult_get_event_id")]
		public static extern int RailEventkRailEventRoomGetRoomMembersResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetRoomMembersResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomGetRoomMembersResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventPlayerGetPlayerMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventPlayerGetPlayerMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventPlayerGetPlayerMetadataResult")]
		public static extern void delete_RailEventkRailEventPlayerGetPlayerMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerGetPlayerMetadataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventPlayerGetPlayerMetadataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerGetPlayerMetadataResult_get_event_id")]
		public static extern int RailEventkRailEventPlayerGetPlayerMetadataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventPlayerGetPlayerMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventPlayerGetPlayerMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelJoinedResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelJoinedResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventVoiceChannelJoinedResult")]
		public static extern void delete_RailEventkRailEventVoiceChannelJoinedResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelJoinedResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventVoiceChannelJoinedResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelJoinedResult_get_event_id")]
		public static extern int RailEventkRailEventVoiceChannelJoinedResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelJoinedResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelJoinedResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGamePurchaseFinishOrderResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGamePurchaseFinishOrderResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGamePurchaseFinishOrderResult")]
		public static extern void delete_RailEventkRailEventInGamePurchaseFinishOrderResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchaseFinishOrderResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGamePurchaseFinishOrderResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchaseFinishOrderResult_get_event_id")]
		public static extern int RailEventkRailEventInGamePurchaseFinishOrderResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGamePurchaseFinishOrderResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGamePurchaseFinishOrderResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsStartConsumeFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsStartConsumeFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsStartConsumeFinished")]
		public static extern void delete_RailEventkRailEventAssetsStartConsumeFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsStartConsumeFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsStartConsumeFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsStartConsumeFinished_get_event_id")]
		public static extern int RailEventkRailEventAssetsStartConsumeFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsStartConsumeFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsStartConsumeFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerFavoriteGameServers__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventGameServerFavoriteGameServers__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventGameServerFavoriteGameServers")]
		public static extern void delete_RailEventkRailEventGameServerFavoriteGameServers(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerFavoriteGameServers_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventGameServerFavoriteGameServers_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerFavoriteGameServers_get_event_id")]
		public static extern int RailEventkRailEventGameServerFavoriteGameServers_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerFavoriteGameServers__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventGameServerFavoriteGameServers__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserJavascriptEvent__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventBrowserJavascriptEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventBrowserJavascriptEvent")]
		public static extern void delete_RailEventkRailEventBrowserJavascriptEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserJavascriptEvent_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventBrowserJavascriptEvent_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserJavascriptEvent_get_event_id")]
		public static extern int RailEventkRailEventBrowserJavascriptEvent_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserJavascriptEvent__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventBrowserJavascriptEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAchievementPlayerAchievementReceived__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAchievementPlayerAchievementReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAchievementPlayerAchievementReceived")]
		public static extern void delete_RailEventkRailEventAchievementPlayerAchievementReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAchievementPlayerAchievementReceived_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAchievementPlayerAchievementReceived_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAchievementPlayerAchievementReceived_get_event_id")]
		public static extern int RailEventkRailEventAchievementPlayerAchievementReceived_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAchievementPlayerAchievementReceived__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAchievementPlayerAchievementReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomLeaveRoomResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomLeaveRoomResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomLeaveRoomResult")]
		public static extern void delete_RailEventkRailEventRoomLeaveRoomResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomLeaveRoomResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomLeaveRoomResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomLeaveRoomResult_get_event_id")]
		public static extern int RailEventkRailEventRoomLeaveRoomResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomLeaveRoomResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomLeaveRoomResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomCreated__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomCreated__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomCreated")]
		public static extern void delete_RailEventkRailEventRoomCreated(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomCreated_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomCreated_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomCreated_get_event_id")]
		public static extern int RailEventkRailEventRoomCreated_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomCreated__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomCreated__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageQueryQuotaResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventStorageQueryQuotaResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventStorageQueryQuotaResult")]
		public static extern void delete_RailEventkRailEventStorageQueryQuotaResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageQueryQuotaResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventStorageQueryQuotaResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageQueryQuotaResult_get_event_id")]
		public static extern int RailEventkRailEventStorageQueryQuotaResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageQueryQuotaResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventStorageQueryQuotaResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcOwnershipChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventDlcOwnershipChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventDlcOwnershipChanged")]
		public static extern void delete_RailEventkRailEventDlcOwnershipChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcOwnershipChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventDlcOwnershipChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcOwnershipChanged_get_event_id")]
		public static extern int RailEventkRailEventDlcOwnershipChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcOwnershipChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventDlcOwnershipChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserNavigeteResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventBrowserNavigeteResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventBrowserNavigeteResult")]
		public static extern void delete_RailEventkRailEventBrowserNavigeteResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserNavigeteResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventBrowserNavigeteResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserNavigeteResult_get_event_id")]
		public static extern int RailEventkRailEventBrowserNavigeteResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserNavigeteResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventBrowserNavigeteResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsGetInviteCommandLine__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFriendsGetInviteCommandLine__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFriendsGetInviteCommandLine")]
		public static extern void delete_RailEventkRailEventFriendsGetInviteCommandLine(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsGetInviteCommandLine_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFriendsGetInviteCommandLine_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsGetInviteCommandLine_get_event_id")]
		public static extern int RailEventkRailEventFriendsGetInviteCommandLine_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsGetInviteCommandLine__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFriendsGetInviteCommandLine__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelUsersSpeakingStateChangedEvent__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelUsersSpeakingStateChangedEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventVoiceChannelUsersSpeakingStateChangedEvent")]
		public static extern void delete_RailEventkRailEventVoiceChannelUsersSpeakingStateChangedEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelUsersSpeakingStateChangedEvent_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventVoiceChannelUsersSpeakingStateChangedEvent_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelUsersSpeakingStateChangedEvent_get_event_id")]
		public static extern int RailEventkRailEventVoiceChannelUsersSpeakingStateChangedEvent_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelUsersSpeakingStateChangedEvent__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelUsersSpeakingStateChangedEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStatsNumOfPlayerReceived__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventStatsNumOfPlayerReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventStatsNumOfPlayerReceived")]
		public static extern void delete_RailEventkRailEventStatsNumOfPlayerReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStatsNumOfPlayerReceived_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventStatsNumOfPlayerReceived_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStatsNumOfPlayerReceived_get_event_id")]
		public static extern int RailEventkRailEventStatsNumOfPlayerReceived_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStatsNumOfPlayerReceived__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventStatsNumOfPlayerReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceVoteSpaceWorkResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceVoteSpaceWorkResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceVoteSpaceWorkResult")]
		public static extern void delete_RailEventkRailEventUserSpaceVoteSpaceWorkResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceVoteSpaceWorkResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceVoteSpaceWorkResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceVoteSpaceWorkResult_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceVoteSpaceWorkResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceVoteSpaceWorkResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceVoteSpaceWorkResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameActivityQueryGameActivityResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGameActivityQueryGameActivityResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGameActivityQueryGameActivityResult")]
		public static extern void delete_RailEventkRailEventInGameActivityQueryGameActivityResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameActivityQueryGameActivityResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGameActivityQueryGameActivityResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameActivityQueryGameActivityResult_get_event_id")]
		public static extern int RailEventkRailEventInGameActivityQueryGameActivityResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameActivityQueryGameActivityResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGameActivityQueryGameActivityResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcInstallFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventDlcInstallFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventDlcInstallFinished")]
		public static extern void delete_RailEventkRailEventDlcInstallFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcInstallFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventDlcInstallFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcInstallFinished_get_event_id")]
		public static extern int RailEventkRailEventDlcInstallFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcInstallFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventDlcInstallFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStatsGlobalStatsReceived__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventStatsGlobalStatsReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventStatsGlobalStatsReceived")]
		public static extern void delete_RailEventkRailEventStatsGlobalStatsReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStatsGlobalStatsReceived_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventStatsGlobalStatsReceived_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStatsGlobalStatsReceived_get_event_id")]
		public static extern int RailEventkRailEventStatsGlobalStatsReceived_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStatsGlobalStatsReceived__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventStatsGlobalStatsReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceSearchSpaceWorkResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceSearchSpaceWorkResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceSearchSpaceWorkResult")]
		public static extern void delete_RailEventkRailEventUserSpaceSearchSpaceWorkResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceSearchSpaceWorkResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceSearchSpaceWorkResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceSearchSpaceWorkResult_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceSearchSpaceWorkResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceSearchSpaceWorkResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceSearchSpaceWorkResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelCreateResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelCreateResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventVoiceChannelCreateResult")]
		public static extern void delete_RailEventkRailEventVoiceChannelCreateResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelCreateResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventVoiceChannelCreateResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelCreateResult_get_event_id")]
		public static extern int RailEventkRailEventVoiceChannelCreateResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelCreateResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelCreateResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsCompleteConsumeByExchangeAssetsToFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsCompleteConsumeByExchangeAssetsToFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsCompleteConsumeByExchangeAssetsToFinished")]
		public static extern void delete_RailEventkRailEventAssetsCompleteConsumeByExchangeAssetsToFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsCompleteConsumeByExchangeAssetsToFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsCompleteConsumeByExchangeAssetsToFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsCompleteConsumeByExchangeAssetsToFinished_get_event_id")]
		public static extern int RailEventkRailEventAssetsCompleteConsumeByExchangeAssetsToFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsCompleteConsumeByExchangeAssetsToFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsCompleteConsumeByExchangeAssetsToFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceRateSpaceWorkResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceRateSpaceWorkResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceRateSpaceWorkResult")]
		public static extern void delete_RailEventkRailEventUserSpaceRateSpaceWorkResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceRateSpaceWorkResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceRateSpaceWorkResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceRateSpaceWorkResult_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceRateSpaceWorkResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceRateSpaceWorkResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceRateSpaceWorkResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetMemberMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomGetMemberMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomGetMemberMetadataResult")]
		public static extern void delete_RailEventkRailEventRoomGetMemberMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetMemberMetadataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomGetMemberMetadataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetMemberMetadataResult_get_event_id")]
		public static extern int RailEventkRailEventRoomGetMemberMetadataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetMemberMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomGetMemberMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsGetMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFriendsGetMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFriendsGetMetadataResult")]
		public static extern void delete_RailEventkRailEventFriendsGetMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsGetMetadataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFriendsGetMetadataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsGetMetadataResult_get_event_id")]
		public static extern int RailEventkRailEventFriendsGetMetadataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsGetMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFriendsGetMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelPushToTalkKeyChangedEvent__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelPushToTalkKeyChangedEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventVoiceChannelPushToTalkKeyChangedEvent")]
		public static extern void delete_RailEventkRailEventVoiceChannelPushToTalkKeyChangedEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelPushToTalkKeyChangedEvent_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventVoiceChannelPushToTalkKeyChangedEvent_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelPushToTalkKeyChangedEvent_get_event_id")]
		public static extern int RailEventkRailEventVoiceChannelPushToTalkKeyChangedEvent_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelPushToTalkKeyChangedEvent__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelPushToTalkKeyChangedEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStatsPlayerStatsReceived__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventStatsPlayerStatsReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventStatsPlayerStatsReceived")]
		public static extern void delete_RailEventkRailEventStatsPlayerStatsReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStatsPlayerStatsReceived_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventStatsPlayerStatsReceived_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStatsPlayerStatsReceived_get_event_id")]
		public static extern int RailEventkRailEventStatsPlayerStatsReceived_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStatsPlayerStatsReceived__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventStatsPlayerStatsReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsCompleteConsumeFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsCompleteConsumeFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsCompleteConsumeFinished")]
		public static extern void delete_RailEventkRailEventAssetsCompleteConsumeFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsCompleteConsumeFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsCompleteConsumeFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsCompleteConsumeFinished_get_event_id")]
		public static extern int RailEventkRailEventAssetsCompleteConsumeFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsCompleteConsumeFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsCompleteConsumeFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameActivityOpenGameActivityWindowResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGameActivityOpenGameActivityWindowResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGameActivityOpenGameActivityWindowResult")]
		public static extern void delete_RailEventkRailEventInGameActivityOpenGameActivityWindowResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameActivityOpenGameActivityWindowResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGameActivityOpenGameActivityWindowResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameActivityOpenGameActivityWindowResult_get_event_id")]
		public static extern int RailEventkRailEventInGameActivityOpenGameActivityWindowResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameActivityOpenGameActivityWindowResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGameActivityOpenGameActivityWindowResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerGetMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventGameServerGetMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventGameServerGetMetadataResult")]
		public static extern void delete_RailEventkRailEventGameServerGetMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerGetMetadataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventGameServerGetMetadataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerGetMetadataResult_get_event_id")]
		public static extern int RailEventkRailEventGameServerGetMetadataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerGetMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventGameServerGetMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventPlayerGetAuthenticateURL__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventPlayerGetAuthenticateURL__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventPlayerGetAuthenticateURL")]
		public static extern void delete_RailEventkRailEventPlayerGetAuthenticateURL(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerGetAuthenticateURL_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventPlayerGetAuthenticateURL_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerGetAuthenticateURL_get_event_id")]
		public static extern int RailEventkRailEventPlayerGetAuthenticateURL_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventPlayerGetAuthenticateURL__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventPlayerGetAuthenticateURL__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelLeaveResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelLeaveResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventVoiceChannelLeaveResult")]
		public static extern void delete_RailEventkRailEventVoiceChannelLeaveResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelLeaveResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventVoiceChannelLeaveResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelLeaveResult_get_event_id")]
		public static extern int RailEventkRailEventVoiceChannelLeaveResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelLeaveResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelLeaveResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyMetadataChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyMetadataChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomNotifyMetadataChanged")]
		public static extern void delete_RailEventkRailEventRoomNotifyMetadataChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyMetadataChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomNotifyMetadataChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyMetadataChanged_get_event_id")]
		public static extern int RailEventkRailEventRoomNotifyMetadataChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyMetadataChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyMetadataChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAchievementPlayerAchievementStored__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAchievementPlayerAchievementStored__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAchievementPlayerAchievementStored")]
		public static extern void delete_RailEventkRailEventAchievementPlayerAchievementStored(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAchievementPlayerAchievementStored_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAchievementPlayerAchievementStored_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAchievementPlayerAchievementStored_get_event_id")]
		public static extern int RailEventkRailEventAchievementPlayerAchievementStored_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAchievementPlayerAchievementStored__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAchievementPlayerAchievementStored__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameStorePurchasePayWindowClosed__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGameStorePurchasePayWindowClosed__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGameStorePurchasePayWindowClosed")]
		public static extern void delete_RailEventkRailEventInGameStorePurchasePayWindowClosed(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameStorePurchasePayWindowClosed_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGameStorePurchasePayWindowClosed_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameStorePurchasePayWindowClosed_get_event_id")]
		public static extern int RailEventkRailEventInGameStorePurchasePayWindowClosed_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameStorePurchasePayWindowClosed__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGameStorePurchasePayWindowClosed__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomOpenRoomResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomOpenRoomResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomOpenRoomResult")]
		public static extern void delete_RailEventkRailEventRoomOpenRoomResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomOpenRoomResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomOpenRoomResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomOpenRoomResult_get_event_id")]
		public static extern int RailEventkRailEventRoomOpenRoomResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomOpenRoomResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomOpenRoomResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelDataCaptured__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelDataCaptured__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventVoiceChannelDataCaptured")]
		public static extern void delete_RailEventkRailEventVoiceChannelDataCaptured(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelDataCaptured_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventVoiceChannelDataCaptured_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelDataCaptured_get_event_id")]
		public static extern int RailEventkRailEventVoiceChannelDataCaptured_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelDataCaptured__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelDataCaptured__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventPlayerAntiAddictionGameOnlineTimeChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventPlayerAntiAddictionGameOnlineTimeChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventPlayerAntiAddictionGameOnlineTimeChanged")]
		public static extern void delete_RailEventkRailEventPlayerAntiAddictionGameOnlineTimeChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerAntiAddictionGameOnlineTimeChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventPlayerAntiAddictionGameOnlineTimeChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerAntiAddictionGameOnlineTimeChanged_get_event_id")]
		public static extern int RailEventkRailEventPlayerAntiAddictionGameOnlineTimeChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventPlayerAntiAddictionGameOnlineTimeChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventPlayerAntiAddictionGameOnlineTimeChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomSetRoomTagResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomSetRoomTagResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomSetRoomTagResult")]
		public static extern void delete_RailEventkRailEventRoomSetRoomTagResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetRoomTagResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomSetRoomTagResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetRoomTagResult_get_event_id")]
		public static extern int RailEventkRailEventRoomSetRoomTagResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomSetRoomTagResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomSetRoomTagResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetRoomListResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomGetRoomListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomGetRoomListResult")]
		public static extern void delete_RailEventkRailEventRoomGetRoomListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetRoomListResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomGetRoomListResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetRoomListResult_get_event_id")]
		public static extern int RailEventkRailEventRoomGetRoomListResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetRoomListResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomGetRoomListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomSetNewRoomOwnerResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomSetNewRoomOwnerResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomSetNewRoomOwnerResult")]
		public static extern void delete_RailEventkRailEventRoomSetNewRoomOwnerResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetNewRoomOwnerResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomSetNewRoomOwnerResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetNewRoomOwnerResult_get_event_id")]
		public static extern int RailEventkRailEventRoomSetNewRoomOwnerResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomSetNewRoomOwnerResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomSetNewRoomOwnerResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncReadStreamFileResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncReadStreamFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventStorageAsyncReadStreamFileResult")]
		public static extern void delete_RailEventkRailEventStorageAsyncReadStreamFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncReadStreamFileResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventStorageAsyncReadStreamFileResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncReadStreamFileResult_get_event_id")]
		public static extern int RailEventkRailEventStorageAsyncReadStreamFileResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncReadStreamFileResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncReadStreamFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserTitleChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventBrowserTitleChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventBrowserTitleChanged")]
		public static extern void delete_RailEventkRailEventBrowserTitleChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserTitleChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventBrowserTitleChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserTitleChanged_get_event_id")]
		public static extern int RailEventkRailEventBrowserTitleChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserTitleChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventBrowserTitleChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersShowChatWindowWithFriendResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUsersShowChatWindowWithFriendResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUsersShowChatWindowWithFriendResult")]
		public static extern void delete_RailEventkRailEventUsersShowChatWindowWithFriendResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersShowChatWindowWithFriendResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUsersShowChatWindowWithFriendResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersShowChatWindowWithFriendResult_get_event_id")]
		public static extern int RailEventkRailEventUsersShowChatWindowWithFriendResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersShowChatWindowWithFriendResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUsersShowChatWindowWithFriendResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceGetMySubscribedWorksResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceGetMySubscribedWorksResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceGetMySubscribedWorksResult")]
		public static extern void delete_RailEventkRailEventUserSpaceGetMySubscribedWorksResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceGetMySubscribedWorksResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceGetMySubscribedWorksResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceGetMySubscribedWorksResult_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceGetMySubscribedWorksResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceGetMySubscribedWorksResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceGetMySubscribedWorksResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageShareToSpaceWorkResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventStorageShareToSpaceWorkResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventStorageShareToSpaceWorkResult")]
		public static extern void delete_RailEventkRailEventStorageShareToSpaceWorkResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageShareToSpaceWorkResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventStorageShareToSpaceWorkResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageShareToSpaceWorkResult_get_event_id")]
		public static extern int RailEventkRailEventStorageShareToSpaceWorkResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageShareToSpaceWorkResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventStorageShareToSpaceWorkResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsFriendsListChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFriendsFriendsListChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFriendsFriendsListChanged")]
		public static extern void delete_RailEventkRailEventFriendsFriendsListChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsFriendsListChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFriendsFriendsListChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsFriendsListChanged_get_event_id")]
		public static extern int RailEventkRailEventFriendsFriendsListChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsFriendsListChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFriendsFriendsListChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUtilsGetImageDataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUtilsGetImageDataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUtilsGetImageDataResult")]
		public static extern void delete_RailEventkRailEventUtilsGetImageDataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUtilsGetImageDataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUtilsGetImageDataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUtilsGetImageDataResult_get_event_id")]
		public static extern int RailEventkRailEventUtilsGetImageDataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUtilsGetImageDataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUtilsGetImageDataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersGetUserLimitsResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUsersGetUserLimitsResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUsersGetUserLimitsResult")]
		public static extern void delete_RailEventkRailEventUsersGetUserLimitsResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersGetUserLimitsResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUsersGetUserLimitsResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersGetUserLimitsResult_get_event_id")]
		public static extern int RailEventkRailEventUsersGetUserLimitsResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersGetUserLimitsResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUsersGetUserLimitsResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventSystemStateChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventSystemStateChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventSystemStateChanged")]
		public static extern void delete_RailEventkRailEventSystemStateChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSystemStateChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventSystemStateChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSystemStateChanged_get_event_id")]
		public static extern int RailEventkRailEventSystemStateChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventSystemStateChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventSystemStateChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventLeaderboardReceived__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventLeaderboardReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventLeaderboardReceived")]
		public static extern void delete_RailEventkRailEventLeaderboardReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardReceived_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventLeaderboardReceived_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardReceived_get_event_id")]
		public static extern int RailEventkRailEventLeaderboardReceived_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventLeaderboardReceived__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventLeaderboardReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersInviteUsersResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUsersInviteUsersResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUsersInviteUsersResult")]
		public static extern void delete_RailEventkRailEventUsersInviteUsersResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersInviteUsersResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUsersInviteUsersResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersInviteUsersResult_get_event_id")]
		public static extern int RailEventkRailEventUsersInviteUsersResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersInviteUsersResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUsersInviteUsersResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventHttpSessionResponseResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventHttpSessionResponseResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventHttpSessionResponseResult")]
		public static extern void delete_RailEventkRailEventHttpSessionResponseResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventHttpSessionResponseResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventHttpSessionResponseResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventHttpSessionResponseResult_get_event_id")]
		public static extern int RailEventkRailEventHttpSessionResponseResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventHttpSessionResponseResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventHttpSessionResponseResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceDownloadResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceDownloadResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceDownloadResult")]
		public static extern void delete_RailEventkRailEventUserSpaceDownloadResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceDownloadResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceDownloadResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceDownloadResult_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceDownloadResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceDownloadResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceDownloadResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomClearRoomMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomClearRoomMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomClearRoomMetadataResult")]
		public static extern void delete_RailEventkRailEventRoomClearRoomMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomClearRoomMetadataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomClearRoomMetadataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomClearRoomMetadataResult_get_event_id")]
		public static extern int RailEventkRailEventRoomClearRoomMetadataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomClearRoomMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomClearRoomMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventSmallObjectServiceDownloadResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventSmallObjectServiceDownloadResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventSmallObjectServiceDownloadResult")]
		public static extern void delete_RailEventkRailEventSmallObjectServiceDownloadResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSmallObjectServiceDownloadResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventSmallObjectServiceDownloadResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSmallObjectServiceDownloadResult_get_event_id")]
		public static extern int RailEventkRailEventSmallObjectServiceDownloadResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventSmallObjectServiceDownloadResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventSmallObjectServiceDownloadResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetAllDataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomGetAllDataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomGetAllDataResult")]
		public static extern void delete_RailEventkRailEventRoomGetAllDataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetAllDataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomGetAllDataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetAllDataResult_get_event_id")]
		public static extern int RailEventkRailEventRoomGetAllDataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetAllDataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomGetAllDataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncListStreamFileResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncListStreamFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventStorageAsyncListStreamFileResult")]
		public static extern void delete_RailEventkRailEventStorageAsyncListStreamFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncListStreamFileResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventStorageAsyncListStreamFileResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncListStreamFileResult_get_event_id")]
		public static extern int RailEventkRailEventStorageAsyncListStreamFileResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncListStreamFileResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncListStreamFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGroupChatOpenGroupChatResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventGroupChatOpenGroupChatResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventGroupChatOpenGroupChatResult")]
		public static extern void delete_RailEventkRailEventGroupChatOpenGroupChatResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGroupChatOpenGroupChatResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventGroupChatOpenGroupChatResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGroupChatOpenGroupChatResult_get_event_id")]
		public static extern int RailEventkRailEventGroupChatOpenGroupChatResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGroupChatOpenGroupChatResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventGroupChatOpenGroupChatResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventLeaderboardEntryReceived__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventLeaderboardEntryReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventLeaderboardEntryReceived")]
		public static extern void delete_RailEventkRailEventLeaderboardEntryReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardEntryReceived_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventLeaderboardEntryReceived_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardEntryReceived_get_event_id")]
		public static extern int RailEventkRailEventLeaderboardEntryReceived_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventLeaderboardEntryReceived__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventLeaderboardEntryReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyRoomGameServerChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyRoomGameServerChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomNotifyRoomGameServerChanged")]
		public static extern void delete_RailEventkRailEventRoomNotifyRoomGameServerChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyRoomGameServerChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomNotifyRoomGameServerChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyRoomGameServerChanged_get_event_id")]
		public static extern int RailEventkRailEventRoomNotifyRoomGameServerChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyRoomGameServerChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyRoomGameServerChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyRoomDataReceived__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyRoomDataReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomNotifyRoomDataReceived")]
		public static extern void delete_RailEventkRailEventRoomNotifyRoomDataReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyRoomDataReceived_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomNotifyRoomDataReceived_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyRoomDataReceived_get_event_id")]
		public static extern int RailEventkRailEventRoomNotifyRoomDataReceived_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomNotifyRoomDataReceived__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomNotifyRoomDataReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcQueryIsOwnedDlcsResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventDlcQueryIsOwnedDlcsResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventDlcQueryIsOwnedDlcsResult")]
		public static extern void delete_RailEventkRailEventDlcQueryIsOwnedDlcsResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcQueryIsOwnedDlcsResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventDlcQueryIsOwnedDlcsResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcQueryIsOwnedDlcsResult_get_event_id")]
		public static extern int RailEventkRailEventDlcQueryIsOwnedDlcsResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcQueryIsOwnedDlcsResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventDlcQueryIsOwnedDlcsResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceSyncResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceSyncResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceSyncResult")]
		public static extern void delete_RailEventkRailEventUserSpaceSyncResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceSyncResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceSyncResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceSyncResult_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceSyncResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceSyncResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceSyncResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsAssetsChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsAssetsChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsAssetsChanged")]
		public static extern void delete_RailEventkRailEventAssetsAssetsChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsAssetsChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsAssetsChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsAssetsChanged_get_event_id")]
		public static extern int RailEventkRailEventAssetsAssetsChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsAssetsChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsAssetsChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventIMEHelperTextInputCompositionStateChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventIMEHelperTextInputCompositionStateChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventIMEHelperTextInputCompositionStateChanged")]
		public static extern void delete_RailEventkRailEventIMEHelperTextInputCompositionStateChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventIMEHelperTextInputCompositionStateChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventIMEHelperTextInputCompositionStateChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventIMEHelperTextInputCompositionStateChanged_get_event_id")]
		public static extern int RailEventkRailEventIMEHelperTextInputCompositionStateChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventIMEHelperTextInputCompositionStateChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventIMEHelperTextInputCompositionStateChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAntiAddictionCustomizeAntiAddictionActions__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAntiAddictionCustomizeAntiAddictionActions__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAntiAddictionCustomizeAntiAddictionActions")]
		public static extern void delete_RailEventkRailEventAntiAddictionCustomizeAntiAddictionActions(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAntiAddictionCustomizeAntiAddictionActions_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAntiAddictionCustomizeAntiAddictionActions_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAntiAddictionCustomizeAntiAddictionActions_get_event_id")]
		public static extern int RailEventkRailEventAntiAddictionCustomizeAntiAddictionActions_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAntiAddictionCustomizeAntiAddictionActions__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAntiAddictionCustomizeAntiAddictionActions__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcRefundChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventDlcRefundChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventDlcRefundChanged")]
		public static extern void delete_RailEventkRailEventDlcRefundChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcRefundChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventDlcRefundChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcRefundChanged_get_event_id")]
		public static extern int RailEventkRailEventDlcRefundChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventDlcRefundChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventDlcRefundChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerPlayerListResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventGameServerPlayerListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventGameServerPlayerListResult")]
		public static extern void delete_RailEventkRailEventGameServerPlayerListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerPlayerListResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventGameServerPlayerListResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerPlayerListResult_get_event_id")]
		public static extern int RailEventkRailEventGameServerPlayerListResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerPlayerListResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventGameServerPlayerListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersGetUsersInfo__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUsersGetUsersInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUsersGetUsersInfo")]
		public static extern void delete_RailEventkRailEventUsersGetUsersInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersGetUsersInfo_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUsersGetUsersInfo_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersGetUsersInfo_get_event_id")]
		public static extern int RailEventkRailEventUsersGetUsersInfo_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersGetUsersInfo__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUsersGetUsersInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncDeleteStreamFileResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncDeleteStreamFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventStorageAsyncDeleteStreamFileResult")]
		public static extern void delete_RailEventkRailEventStorageAsyncDeleteStreamFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncDeleteStreamFileResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventStorageAsyncDeleteStreamFileResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncDeleteStreamFileResult_get_event_id")]
		public static extern int RailEventkRailEventStorageAsyncDeleteStreamFileResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncDeleteStreamFileResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncDeleteStreamFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsSetMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventFriendsSetMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventFriendsSetMetadataResult")]
		public static extern void delete_RailEventkRailEventFriendsSetMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsSetMetadataResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventFriendsSetMetadataResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsSetMetadataResult_get_event_id")]
		public static extern int RailEventkRailEventFriendsSetMetadataResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventFriendsSetMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventFriendsSetMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserCreateResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventBrowserCreateResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventBrowserCreateResult")]
		public static extern void delete_RailEventkRailEventBrowserCreateResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserCreateResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventBrowserCreateResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserCreateResult_get_event_id")]
		public static extern int RailEventkRailEventBrowserCreateResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserCreateResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventBrowserCreateResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsSplitToFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAssetsSplitToFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAssetsSplitToFinished")]
		public static extern void delete_RailEventkRailEventAssetsSplitToFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsSplitToFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAssetsSplitToFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsSplitToFinished_get_event_id")]
		public static extern int RailEventkRailEventAssetsSplitToFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAssetsSplitToFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAssetsSplitToFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventScreenshotPublishScreenshotFinished__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventScreenshotPublishScreenshotFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventScreenshotPublishScreenshotFinished")]
		public static extern void delete_RailEventkRailEventScreenshotPublishScreenshotFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventScreenshotPublishScreenshotFinished_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventScreenshotPublishScreenshotFinished_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventScreenshotPublishScreenshotFinished_get_event_id")]
		public static extern int RailEventkRailEventScreenshotPublishScreenshotFinished_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventScreenshotPublishScreenshotFinished__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventScreenshotPublishScreenshotFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelAddUsersResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelAddUsersResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventVoiceChannelAddUsersResult")]
		public static extern void delete_RailEventkRailEventVoiceChannelAddUsersResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelAddUsersResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventVoiceChannelAddUsersResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelAddUsersResult_get_event_id")]
		public static extern int RailEventkRailEventVoiceChannelAddUsersResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventVoiceChannelAddUsersResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventVoiceChannelAddUsersResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerAddFavoriteGameServer__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventGameServerAddFavoriteGameServer__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventGameServerAddFavoriteGameServer")]
		public static extern void delete_RailEventkRailEventGameServerAddFavoriteGameServer(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerAddFavoriteGameServer_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventGameServerAddFavoriteGameServer_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerAddFavoriteGameServer_get_event_id")]
		public static extern int RailEventkRailEventGameServerAddFavoriteGameServer_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerAddFavoriteGameServer__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventGameServerAddFavoriteGameServer__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventSessionTicketGetSessionTicket__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventSessionTicketGetSessionTicket__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventSessionTicketGetSessionTicket")]
		public static extern void delete_RailEventkRailEventSessionTicketGetSessionTicket(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSessionTicketGetSessionTicket_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventSessionTicketGetSessionTicket_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSessionTicketGetSessionTicket_get_event_id")]
		public static extern int RailEventkRailEventSessionTicketGetSessionTicket_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventSessionTicketGetSessionTicket__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventSessionTicketGetSessionTicket__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerRemoveFavoriteGameServer__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventGameServerRemoveFavoriteGameServer__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventGameServerRemoveFavoriteGameServer")]
		public static extern void delete_RailEventkRailEventGameServerRemoveFavoriteGameServer(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerRemoveFavoriteGameServer_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventGameServerRemoveFavoriteGameServer_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerRemoveFavoriteGameServer_get_event_id")]
		public static extern int RailEventkRailEventGameServerRemoveFavoriteGameServer_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerRemoveFavoriteGameServer__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventGameServerRemoveFavoriteGameServer__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetRoomTagResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventRoomGetRoomTagResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventRoomGetRoomTagResult")]
		public static extern void delete_RailEventkRailEventRoomGetRoomTagResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetRoomTagResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventRoomGetRoomTagResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetRoomTagResult_get_event_id")]
		public static extern int RailEventkRailEventRoomGetRoomTagResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventRoomGetRoomTagResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventRoomGetRoomTagResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStatsPlayerStatsStored__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventStatsPlayerStatsStored__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventStatsPlayerStatsStored")]
		public static extern void delete_RailEventkRailEventStatsPlayerStatsStored(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStatsPlayerStatsStored_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventStatsPlayerStatsStored_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStatsPlayerStatsStored_get_event_id")]
		public static extern int RailEventkRailEventStatsPlayerStatsStored_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStatsPlayerStatsStored__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventStatsPlayerStatsStored__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncReadFileResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncReadFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventStorageAsyncReadFileResult")]
		public static extern void delete_RailEventkRailEventStorageAsyncReadFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncReadFileResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventStorageAsyncReadFileResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncReadFileResult_get_event_id")]
		public static extern int RailEventkRailEventStorageAsyncReadFileResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventStorageAsyncReadFileResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventStorageAsyncReadFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventSessionTicketAuthSessionTicket__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventSessionTicketAuthSessionTicket__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventSessionTicketAuthSessionTicket")]
		public static extern void delete_RailEventkRailEventSessionTicketAuthSessionTicket(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSessionTicketAuthSessionTicket_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventSessionTicketAuthSessionTicket_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSessionTicketAuthSessionTicket_get_event_id")]
		public static extern int RailEventkRailEventSessionTicketAuthSessionTicket_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventSessionTicketAuthSessionTicket__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventSessionTicketAuthSessionTicket__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventScreenshotTakeScreenshotRequest__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventScreenshotTakeScreenshotRequest__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventScreenshotTakeScreenshotRequest")]
		public static extern void delete_RailEventkRailEventScreenshotTakeScreenshotRequest(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventScreenshotTakeScreenshotRequest_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventScreenshotTakeScreenshotRequest_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventScreenshotTakeScreenshotRequest_get_event_id")]
		public static extern int RailEventkRailEventScreenshotTakeScreenshotRequest_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventScreenshotTakeScreenshotRequest__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventScreenshotTakeScreenshotRequest__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceRemoveSpaceWorkResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceRemoveSpaceWorkResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUserSpaceRemoveSpaceWorkResult")]
		public static extern void delete_RailEventkRailEventUserSpaceRemoveSpaceWorkResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceRemoveSpaceWorkResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUserSpaceRemoveSpaceWorkResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceRemoveSpaceWorkResult_get_event_id")]
		public static extern int RailEventkRailEventUserSpaceRemoveSpaceWorkResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUserSpaceRemoveSpaceWorkResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUserSpaceRemoveSpaceWorkResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserStateChanged__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventBrowserStateChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventBrowserStateChanged")]
		public static extern void delete_RailEventkRailEventBrowserStateChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserStateChanged_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventBrowserStateChanged_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserStateChanged_get_event_id")]
		public static extern int RailEventkRailEventBrowserStateChanged_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventBrowserStateChanged__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventBrowserStateChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerGetSessionTicket__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventGameServerGetSessionTicket__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventGameServerGetSessionTicket")]
		public static extern void delete_RailEventkRailEventGameServerGetSessionTicket(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerGetSessionTicket_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventGameServerGetSessionTicket_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerGetSessionTicket_get_event_id")]
		public static extern int RailEventkRailEventGameServerGetSessionTicket_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventGameServerGetSessionTicket__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventGameServerGetSessionTicket__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersInviteJoinGameResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventUsersInviteJoinGameResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventUsersInviteJoinGameResult")]
		public static extern void delete_RailEventkRailEventUsersInviteJoinGameResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersInviteJoinGameResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventUsersInviteJoinGameResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersInviteJoinGameResult_get_event_id")]
		public static extern int RailEventkRailEventUsersInviteJoinGameResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventUsersInviteJoinGameResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventUsersInviteJoinGameResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventNetworkCreateRawSessionFailed__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventNetworkCreateRawSessionFailed__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventNetworkCreateRawSessionFailed")]
		public static extern void delete_RailEventkRailEventNetworkCreateRawSessionFailed(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventNetworkCreateRawSessionFailed_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventNetworkCreateRawSessionFailed_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventNetworkCreateRawSessionFailed_get_event_id")]
		public static extern int RailEventkRailEventNetworkCreateRawSessionFailed_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventNetworkCreateRawSessionFailed__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventNetworkCreateRawSessionFailed__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameCoinPurchaseCoinsResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGameCoinPurchaseCoinsResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGameCoinPurchaseCoinsResult")]
		public static extern void delete_RailEventkRailEventInGameCoinPurchaseCoinsResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameCoinPurchaseCoinsResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGameCoinPurchaseCoinsResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameCoinPurchaseCoinsResult_get_event_id")]
		public static extern int RailEventkRailEventInGameCoinPurchaseCoinsResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameCoinPurchaseCoinsResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGameCoinPurchaseCoinsResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAppQuerySubscribeWishPlayStateResult__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventAppQuerySubscribeWishPlayStateResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventAppQuerySubscribeWishPlayStateResult")]
		public static extern void delete_RailEventkRailEventAppQuerySubscribeWishPlayStateResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAppQuerySubscribeWishPlayStateResult_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventAppQuerySubscribeWishPlayStateResult_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAppQuerySubscribeWishPlayStateResult_get_event_id")]
		public static extern int RailEventkRailEventAppQuerySubscribeWishPlayStateResult_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventAppQuerySubscribeWishPlayStateResult__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventAppQuerySubscribeWishPlayStateResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventNetworkCreateSessionFailed__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventNetworkCreateSessionFailed__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventNetworkCreateSessionFailed")]
		public static extern void delete_RailEventkRailEventNetworkCreateSessionFailed(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventNetworkCreateSessionFailed_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventNetworkCreateSessionFailed_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventNetworkCreateSessionFailed_get_event_id")]
		public static extern int RailEventkRailEventNetworkCreateSessionFailed_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventNetworkCreateSessionFailed__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventNetworkCreateSessionFailed__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventQueryPlayerBannedStatus__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventQueryPlayerBannedStatus__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventQueryPlayerBannedStatus")]
		public static extern void delete_RailEventkRailEventQueryPlayerBannedStatus(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventQueryPlayerBannedStatus_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventQueryPlayerBannedStatus_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventQueryPlayerBannedStatus_get_event_id")]
		public static extern int RailEventkRailEventQueryPlayerBannedStatus_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventQueryPlayerBannedStatus__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventQueryPlayerBannedStatus__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameActivityGameActivityPlayerEvent__SWIG_0")]
		public static extern IntPtr new_RailEventkRailEventInGameActivityGameActivityPlayerEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailEventInGameActivityGameActivityPlayerEvent")]
		public static extern void delete_RailEventkRailEventInGameActivityGameActivityPlayerEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameActivityGameActivityPlayerEvent_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailEventInGameActivityGameActivityPlayerEvent_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameActivityGameActivityPlayerEvent_get_event_id")]
		public static extern int RailEventkRailEventInGameActivityGameActivityPlayerEvent_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailEventInGameActivityGameActivityPlayerEvent__SWIG_1")]
		public static extern IntPtr new_RailEventkRailEventInGameActivityGameActivityPlayerEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailPlatformNotifyEventJoinGameByGameServer__SWIG_0")]
		public static extern IntPtr new_RailEventkRailPlatformNotifyEventJoinGameByGameServer__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailEventkRailPlatformNotifyEventJoinGameByGameServer")]
		public static extern void delete_RailEventkRailPlatformNotifyEventJoinGameByGameServer(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailPlatformNotifyEventJoinGameByGameServer_kInternalRailEventEventId_get")]
		public static extern int RailEventkRailPlatformNotifyEventJoinGameByGameServer_kInternalRailEventEventId_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailPlatformNotifyEventJoinGameByGameServer_get_event_id")]
		public static extern int RailEventkRailPlatformNotifyEventJoinGameByGameServer_get_event_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailEventkRailPlatformNotifyEventJoinGameByGameServer__SWIG_1")]
		public static extern IntPtr new_RailEventkRailPlatformNotifyEventJoinGameByGameServer__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_kRailMaxQuerySpaceWorksLimit_get")]
		public static extern uint kRailMaxQuerySpaceWorksLimit_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SpaceWorkID__SWIG_0")]
		public static extern IntPtr new_SpaceWorkID__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SpaceWorkID__SWIG_1")]
		public static extern IntPtr new_SpaceWorkID__SWIG_1(ulong jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SpaceWorkID_set_id")]
		public static extern void SpaceWorkID_set_id(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_SpaceWorkID_get_id")]
		public static extern ulong SpaceWorkID_get_id(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SpaceWorkID_IsValid")]
		public static extern bool SpaceWorkID_IsValid(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SpaceWorkID__SWIG_2")]
		public static extern IntPtr new_SpaceWorkID__SWIG_2(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_SpaceWorkID")]
		public static extern void delete_SpaceWorkID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkFilter_subscriber_list_set")]
		public static extern void RailSpaceWorkFilter_subscriber_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkFilter_subscriber_list_get")]
		public static extern IntPtr RailSpaceWorkFilter_subscriber_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkFilter_creator_list_set")]
		public static extern void RailSpaceWorkFilter_creator_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkFilter_creator_list_get")]
		public static extern IntPtr RailSpaceWorkFilter_creator_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkFilter_type_set")]
		public static extern void RailSpaceWorkFilter_type_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkFilter_type_get")]
		public static extern IntPtr RailSpaceWorkFilter_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkFilter_collector_list_set")]
		public static extern void RailSpaceWorkFilter_collector_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkFilter_collector_list_get")]
		public static extern IntPtr RailSpaceWorkFilter_collector_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkFilter_classes_set")]
		public static extern void RailSpaceWorkFilter_classes_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkFilter_classes_get")]
		public static extern IntPtr RailSpaceWorkFilter_classes_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSpaceWorkFilter__SWIG_0")]
		public static extern IntPtr new_RailSpaceWorkFilter__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSpaceWorkFilter__SWIG_1")]
		public static extern IntPtr new_RailSpaceWorkFilter__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSpaceWorkFilter")]
		public static extern void delete_RailSpaceWorkFilter(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_with_url_set")]
		public static extern void RailQueryWorkFileOptions_with_url_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_with_url_get")]
		public static extern bool RailQueryWorkFileOptions_with_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_with_description_set")]
		public static extern void RailQueryWorkFileOptions_with_description_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_with_description_get")]
		public static extern bool RailQueryWorkFileOptions_with_description_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_query_total_only_set")]
		public static extern void RailQueryWorkFileOptions_query_total_only_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_query_total_only_get")]
		public static extern bool RailQueryWorkFileOptions_query_total_only_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_with_uploader_ids_set")]
		public static extern void RailQueryWorkFileOptions_with_uploader_ids_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_with_uploader_ids_get")]
		public static extern bool RailQueryWorkFileOptions_with_uploader_ids_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_with_preview_url_set")]
		public static extern void RailQueryWorkFileOptions_with_preview_url_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_with_preview_url_get")]
		public static extern bool RailQueryWorkFileOptions_with_preview_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_with_vote_detail_set")]
		public static extern void RailQueryWorkFileOptions_with_vote_detail_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_with_vote_detail_get")]
		public static extern bool RailQueryWorkFileOptions_with_vote_detail_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_preview_scaling_rate_set")]
		public static extern void RailQueryWorkFileOptions_preview_scaling_rate_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryWorkFileOptions_preview_scaling_rate_get")]
		public static extern uint RailQueryWorkFileOptions_preview_scaling_rate_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailQueryWorkFileOptions__SWIG_0")]
		public static extern IntPtr new_RailQueryWorkFileOptions__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailQueryWorkFileOptions__SWIG_1")]
		public static extern IntPtr new_RailQueryWorkFileOptions__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailQueryWorkFileOptions")]
		public static extern void delete_RailQueryWorkFileOptions(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSyncProgress_finished_bytes_set")]
		public static extern void RailSpaceWorkSyncProgress_finished_bytes_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSyncProgress_finished_bytes_get")]
		public static extern ulong RailSpaceWorkSyncProgress_finished_bytes_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSyncProgress_total_bytes_set")]
		public static extern void RailSpaceWorkSyncProgress_total_bytes_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSyncProgress_total_bytes_get")]
		public static extern ulong RailSpaceWorkSyncProgress_total_bytes_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSyncProgress_progress_set")]
		public static extern void RailSpaceWorkSyncProgress_progress_set(IntPtr jarg1, float jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSyncProgress_progress_get")]
		public static extern float RailSpaceWorkSyncProgress_progress_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSyncProgress_current_state_set")]
		public static extern void RailSpaceWorkSyncProgress_current_state_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSyncProgress_current_state_get")]
		public static extern int RailSpaceWorkSyncProgress_current_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSpaceWorkSyncProgress__SWIG_0")]
		public static extern IntPtr new_RailSpaceWorkSyncProgress__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSpaceWorkSyncProgress__SWIG_1")]
		public static extern IntPtr new_RailSpaceWorkSyncProgress__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSpaceWorkSyncProgress")]
		public static extern void delete_RailSpaceWorkSyncProgress(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkVoteDetail_vote_value_set")]
		public static extern void RailSpaceWorkVoteDetail_vote_value_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkVoteDetail_vote_value_get")]
		public static extern int RailSpaceWorkVoteDetail_vote_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkVoteDetail_voted_players_set")]
		public static extern void RailSpaceWorkVoteDetail_voted_players_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkVoteDetail_voted_players_get")]
		public static extern uint RailSpaceWorkVoteDetail_voted_players_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSpaceWorkVoteDetail__SWIG_0")]
		public static extern IntPtr new_RailSpaceWorkVoteDetail__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSpaceWorkVoteDetail__SWIG_1")]
		public static extern IntPtr new_RailSpaceWorkVoteDetail__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSpaceWorkVoteDetail")]
		public static extern void delete_RailSpaceWorkVoteDetail(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_id_set")]
		public static extern void RailSpaceWorkDescriptor_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_id_get")]
		public static extern IntPtr RailSpaceWorkDescriptor_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_name_set")]
		public static extern void RailSpaceWorkDescriptor_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_name_get")]
		public static extern IntPtr RailSpaceWorkDescriptor_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_description_set")]
		public static extern void RailSpaceWorkDescriptor_description_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_description_get")]
		public static extern IntPtr RailSpaceWorkDescriptor_description_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_detail_url_set")]
		public static extern void RailSpaceWorkDescriptor_detail_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_detail_url_get")]
		public static extern IntPtr RailSpaceWorkDescriptor_detail_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_create_time_set")]
		public static extern void RailSpaceWorkDescriptor_create_time_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_create_time_get")]
		public static extern uint RailSpaceWorkDescriptor_create_time_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_uploader_ids_set")]
		public static extern void RailSpaceWorkDescriptor_uploader_ids_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_uploader_ids_get")]
		public static extern IntPtr RailSpaceWorkDescriptor_uploader_ids_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_preview_url_set")]
		public static extern void RailSpaceWorkDescriptor_preview_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_preview_url_get")]
		public static extern IntPtr RailSpaceWorkDescriptor_preview_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_preview_scaling_url_set")]
		public static extern void RailSpaceWorkDescriptor_preview_scaling_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_preview_scaling_url_get")]
		public static extern IntPtr RailSpaceWorkDescriptor_preview_scaling_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_vote_details_set")]
		public static extern void RailSpaceWorkDescriptor_vote_details_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_vote_details_get")]
		public static extern IntPtr RailSpaceWorkDescriptor_vote_details_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_recommendation_rate_set")]
		public static extern void RailSpaceWorkDescriptor_recommendation_rate_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkDescriptor_recommendation_rate_get")]
		public static extern IntPtr RailSpaceWorkDescriptor_recommendation_rate_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSpaceWorkDescriptor__SWIG_0")]
		public static extern IntPtr new_RailSpaceWorkDescriptor__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSpaceWorkDescriptor__SWIG_1")]
		public static extern IntPtr new_RailSpaceWorkDescriptor__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSpaceWorkDescriptor")]
		public static extern void delete_RailSpaceWorkDescriptor(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadProgress_id_set")]
		public static extern void RailUserSpaceDownloadProgress_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadProgress_id_get")]
		public static extern IntPtr RailUserSpaceDownloadProgress_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadProgress_progress_set")]
		public static extern void RailUserSpaceDownloadProgress_progress_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadProgress_progress_get")]
		public static extern uint RailUserSpaceDownloadProgress_progress_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadProgress_total_set")]
		public static extern void RailUserSpaceDownloadProgress_total_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadProgress_total_get")]
		public static extern ulong RailUserSpaceDownloadProgress_total_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadProgress_finidshed_set")]
		public static extern void RailUserSpaceDownloadProgress_finidshed_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadProgress_finidshed_get")]
		public static extern ulong RailUserSpaceDownloadProgress_finidshed_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadProgress_speed_set")]
		public static extern void RailUserSpaceDownloadProgress_speed_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadProgress_speed_get")]
		public static extern uint RailUserSpaceDownloadProgress_speed_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUserSpaceDownloadProgress__SWIG_0")]
		public static extern IntPtr new_RailUserSpaceDownloadProgress__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUserSpaceDownloadProgress__SWIG_1")]
		public static extern IntPtr new_RailUserSpaceDownloadProgress__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailUserSpaceDownloadProgress")]
		public static extern void delete_RailUserSpaceDownloadProgress(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_id_set")]
		public static extern void RailUserSpaceDownloadResult_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_id_get")]
		public static extern IntPtr RailUserSpaceDownloadResult_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_err_code_set")]
		public static extern void RailUserSpaceDownloadResult_err_code_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_err_code_get")]
		public static extern uint RailUserSpaceDownloadResult_err_code_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_total_bytes_set")]
		public static extern void RailUserSpaceDownloadResult_total_bytes_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_total_bytes_get")]
		public static extern ulong RailUserSpaceDownloadResult_total_bytes_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_finished_bytes_set")]
		public static extern void RailUserSpaceDownloadResult_finished_bytes_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_finished_bytes_get")]
		public static extern ulong RailUserSpaceDownloadResult_finished_bytes_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_total_files_set")]
		public static extern void RailUserSpaceDownloadResult_total_files_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_total_files_get")]
		public static extern uint RailUserSpaceDownloadResult_total_files_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_finished_files_set")]
		public static extern void RailUserSpaceDownloadResult_finished_files_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_finished_files_get")]
		public static extern uint RailUserSpaceDownloadResult_finished_files_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_err_msg_set")]
		public static extern void RailUserSpaceDownloadResult_err_msg_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserSpaceDownloadResult_err_msg_get")]
		public static extern IntPtr RailUserSpaceDownloadResult_err_msg_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUserSpaceDownloadResult__SWIG_0")]
		public static extern IntPtr new_RailUserSpaceDownloadResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUserSpaceDownloadResult__SWIG_1")]
		public static extern IntPtr new_RailUserSpaceDownloadResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailUserSpaceDownloadResult")]
		public static extern void delete_RailUserSpaceDownloadResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQuerySpaceWorkInfoResult_id_set")]
		public static extern void RailQuerySpaceWorkInfoResult_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQuerySpaceWorkInfoResult_id_get")]
		public static extern IntPtr RailQuerySpaceWorkInfoResult_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQuerySpaceWorkInfoResult_error_code_set")]
		public static extern void RailQuerySpaceWorkInfoResult_error_code_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQuerySpaceWorkInfoResult_error_code_get")]
		public static extern int RailQuerySpaceWorkInfoResult_error_code_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQuerySpaceWorkInfoResult_spacework_descriptor_set")]
		public static extern void RailQuerySpaceWorkInfoResult_spacework_descriptor_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQuerySpaceWorkInfoResult_spacework_descriptor_get")]
		public static extern IntPtr RailQuerySpaceWorkInfoResult_spacework_descriptor_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailQuerySpaceWorkInfoResult__SWIG_0")]
		public static extern IntPtr new_RailQuerySpaceWorkInfoResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailQuerySpaceWorkInfoResult__SWIG_1")]
		public static extern IntPtr new_RailQuerySpaceWorkInfoResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailQuerySpaceWorkInfoResult")]
		public static extern void delete_RailQuerySpaceWorkInfoResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncGetMySubscribedWorksResult_spacework_descriptors_set")]
		public static extern void AsyncGetMySubscribedWorksResult_spacework_descriptors_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncGetMySubscribedWorksResult_spacework_descriptors_get")]
		public static extern IntPtr AsyncGetMySubscribedWorksResult_spacework_descriptors_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncGetMySubscribedWorksResult_total_available_works_set")]
		public static extern void AsyncGetMySubscribedWorksResult_total_available_works_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncGetMySubscribedWorksResult_total_available_works_get")]
		public static extern uint AsyncGetMySubscribedWorksResult_total_available_works_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncGetMySubscribedWorksResult__SWIG_0")]
		public static extern IntPtr new_AsyncGetMySubscribedWorksResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncGetMySubscribedWorksResult__SWIG_1")]
		public static extern IntPtr new_AsyncGetMySubscribedWorksResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncGetMySubscribedWorksResult")]
		public static extern void delete_AsyncGetMySubscribedWorksResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncGetMyFavoritesWorksResult_spacework_descriptors_set")]
		public static extern void AsyncGetMyFavoritesWorksResult_spacework_descriptors_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncGetMyFavoritesWorksResult_spacework_descriptors_get")]
		public static extern IntPtr AsyncGetMyFavoritesWorksResult_spacework_descriptors_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncGetMyFavoritesWorksResult_total_available_works_set")]
		public static extern void AsyncGetMyFavoritesWorksResult_total_available_works_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncGetMyFavoritesWorksResult_total_available_works_get")]
		public static extern uint AsyncGetMyFavoritesWorksResult_total_available_works_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncGetMyFavoritesWorksResult__SWIG_0")]
		public static extern IntPtr new_AsyncGetMyFavoritesWorksResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncGetMyFavoritesWorksResult__SWIG_1")]
		public static extern IntPtr new_AsyncGetMyFavoritesWorksResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncGetMyFavoritesWorksResult")]
		public static extern void delete_AsyncGetMyFavoritesWorksResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncQuerySpaceWorksResult_spacework_descriptors_set")]
		public static extern void AsyncQuerySpaceWorksResult_spacework_descriptors_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncQuerySpaceWorksResult_spacework_descriptors_get")]
		public static extern IntPtr AsyncQuerySpaceWorksResult_spacework_descriptors_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncQuerySpaceWorksResult_total_available_works_set")]
		public static extern void AsyncQuerySpaceWorksResult_total_available_works_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncQuerySpaceWorksResult_total_available_works_get")]
		public static extern uint AsyncQuerySpaceWorksResult_total_available_works_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncQuerySpaceWorksResult__SWIG_0")]
		public static extern IntPtr new_AsyncQuerySpaceWorksResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncQuerySpaceWorksResult__SWIG_1")]
		public static extern IntPtr new_AsyncQuerySpaceWorksResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncQuerySpaceWorksResult")]
		public static extern void delete_AsyncQuerySpaceWorksResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncUpdateMetadataResult_id_set")]
		public static extern void AsyncUpdateMetadataResult_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncUpdateMetadataResult_id_get")]
		public static extern IntPtr AsyncUpdateMetadataResult_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncUpdateMetadataResult_type_set")]
		public static extern void AsyncUpdateMetadataResult_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncUpdateMetadataResult_type_get")]
		public static extern int AsyncUpdateMetadataResult_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncUpdateMetadataResult__SWIG_0")]
		public static extern IntPtr new_AsyncUpdateMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncUpdateMetadataResult__SWIG_1")]
		public static extern IntPtr new_AsyncUpdateMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncUpdateMetadataResult")]
		public static extern void delete_AsyncUpdateMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SyncSpaceWorkResult_id_set")]
		public static extern void SyncSpaceWorkResult_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_SyncSpaceWorkResult_id_get")]
		public static extern IntPtr SyncSpaceWorkResult_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SyncSpaceWorkResult__SWIG_0")]
		public static extern IntPtr new_SyncSpaceWorkResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SyncSpaceWorkResult__SWIG_1")]
		public static extern IntPtr new_SyncSpaceWorkResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_SyncSpaceWorkResult")]
		public static extern void delete_SyncSpaceWorkResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncSubscribeSpaceWorksResult_success_ids_set")]
		public static extern void AsyncSubscribeSpaceWorksResult_success_ids_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncSubscribeSpaceWorksResult_success_ids_get")]
		public static extern IntPtr AsyncSubscribeSpaceWorksResult_success_ids_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncSubscribeSpaceWorksResult_failure_ids_set")]
		public static extern void AsyncSubscribeSpaceWorksResult_failure_ids_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncSubscribeSpaceWorksResult_failure_ids_get")]
		public static extern IntPtr AsyncSubscribeSpaceWorksResult_failure_ids_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncSubscribeSpaceWorksResult_subscribe_set")]
		public static extern void AsyncSubscribeSpaceWorksResult_subscribe_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncSubscribeSpaceWorksResult_subscribe_get")]
		public static extern bool AsyncSubscribeSpaceWorksResult_subscribe_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncSubscribeSpaceWorksResult__SWIG_0")]
		public static extern IntPtr new_AsyncSubscribeSpaceWorksResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncSubscribeSpaceWorksResult__SWIG_1")]
		public static extern IntPtr new_AsyncSubscribeSpaceWorksResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncSubscribeSpaceWorksResult")]
		public static extern void delete_AsyncSubscribeSpaceWorksResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncModifyFavoritesWorksResult_success_ids_set")]
		public static extern void AsyncModifyFavoritesWorksResult_success_ids_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncModifyFavoritesWorksResult_success_ids_get")]
		public static extern IntPtr AsyncModifyFavoritesWorksResult_success_ids_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncModifyFavoritesWorksResult_failure_ids_set")]
		public static extern void AsyncModifyFavoritesWorksResult_failure_ids_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncModifyFavoritesWorksResult_failure_ids_get")]
		public static extern IntPtr AsyncModifyFavoritesWorksResult_failure_ids_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncModifyFavoritesWorksResult_modify_flag_set")]
		public static extern void AsyncModifyFavoritesWorksResult_modify_flag_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncModifyFavoritesWorksResult_modify_flag_get")]
		public static extern int AsyncModifyFavoritesWorksResult_modify_flag_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncModifyFavoritesWorksResult__SWIG_0")]
		public static extern IntPtr new_AsyncModifyFavoritesWorksResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncModifyFavoritesWorksResult__SWIG_1")]
		public static extern IntPtr new_AsyncModifyFavoritesWorksResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncModifyFavoritesWorksResult")]
		public static extern void delete_AsyncModifyFavoritesWorksResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRemoveSpaceWorkResult_id_set")]
		public static extern void AsyncRemoveSpaceWorkResult_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRemoveSpaceWorkResult_id_get")]
		public static extern IntPtr AsyncRemoveSpaceWorkResult_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncRemoveSpaceWorkResult__SWIG_0")]
		public static extern IntPtr new_AsyncRemoveSpaceWorkResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncRemoveSpaceWorkResult__SWIG_1")]
		public static extern IntPtr new_AsyncRemoveSpaceWorkResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncRemoveSpaceWorkResult")]
		public static extern void delete_AsyncRemoveSpaceWorkResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncVoteSpaceWorkResult_id_set")]
		public static extern void AsyncVoteSpaceWorkResult_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncVoteSpaceWorkResult_id_get")]
		public static extern IntPtr AsyncVoteSpaceWorkResult_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncVoteSpaceWorkResult__SWIG_0")]
		public static extern IntPtr new_AsyncVoteSpaceWorkResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncVoteSpaceWorkResult__SWIG_1")]
		public static extern IntPtr new_AsyncVoteSpaceWorkResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncVoteSpaceWorkResult")]
		public static extern void delete_AsyncVoteSpaceWorkResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRateSpaceWorkResult_id_set")]
		public static extern void AsyncRateSpaceWorkResult_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRateSpaceWorkResult_id_get")]
		public static extern IntPtr AsyncRateSpaceWorkResult_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncRateSpaceWorkResult__SWIG_0")]
		public static extern IntPtr new_AsyncRateSpaceWorkResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncRateSpaceWorkResult__SWIG_1")]
		public static extern IntPtr new_AsyncRateSpaceWorkResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncRateSpaceWorkResult")]
		public static extern void delete_AsyncRateSpaceWorkResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncSearchSpaceWorksResult_spacework_descriptors_set")]
		public static extern void AsyncSearchSpaceWorksResult_spacework_descriptors_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncSearchSpaceWorksResult_spacework_descriptors_get")]
		public static extern IntPtr AsyncSearchSpaceWorksResult_spacework_descriptors_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncSearchSpaceWorksResult_total_available_works_set")]
		public static extern void AsyncSearchSpaceWorksResult_total_available_works_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncSearchSpaceWorksResult_total_available_works_get")]
		public static extern uint AsyncSearchSpaceWorksResult_total_available_works_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncSearchSpaceWorksResult__SWIG_0")]
		public static extern IntPtr new_AsyncSearchSpaceWorksResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncSearchSpaceWorksResult__SWIG_1")]
		public static extern IntPtr new_AsyncSearchSpaceWorksResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncSearchSpaceWorksResult")]
		public static extern void delete_AsyncSearchSpaceWorksResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_UserSpaceDownloadProgress_progress_set")]
		public static extern void UserSpaceDownloadProgress_progress_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_UserSpaceDownloadProgress_progress_get")]
		public static extern IntPtr UserSpaceDownloadProgress_progress_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_UserSpaceDownloadProgress_total_progress_set")]
		public static extern void UserSpaceDownloadProgress_total_progress_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_UserSpaceDownloadProgress_total_progress_get")]
		public static extern uint UserSpaceDownloadProgress_total_progress_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_UserSpaceDownloadProgress__SWIG_0")]
		public static extern IntPtr new_UserSpaceDownloadProgress__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_UserSpaceDownloadProgress__SWIG_1")]
		public static extern IntPtr new_UserSpaceDownloadProgress__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_UserSpaceDownloadProgress")]
		public static extern void delete_UserSpaceDownloadProgress(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_UserSpaceDownloadResult_results_set")]
		public static extern void UserSpaceDownloadResult_results_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_UserSpaceDownloadResult_results_get")]
		public static extern IntPtr UserSpaceDownloadResult_results_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_UserSpaceDownloadResult_total_results_set")]
		public static extern void UserSpaceDownloadResult_total_results_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_UserSpaceDownloadResult_total_results_get")]
		public static extern uint UserSpaceDownloadResult_total_results_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_UserSpaceDownloadResult__SWIG_0")]
		public static extern IntPtr new_UserSpaceDownloadResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_UserSpaceDownloadResult__SWIG_1")]
		public static extern IntPtr new_UserSpaceDownloadResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_UserSpaceDownloadResult")]
		public static extern void delete_UserSpaceDownloadResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncQuerySpaceWorksInfoResult_query_spaceworks_info_result_set")]
		public static extern void AsyncQuerySpaceWorksInfoResult_query_spaceworks_info_result_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncQuerySpaceWorksInfoResult_query_spaceworks_info_result_get")]
		public static extern IntPtr AsyncQuerySpaceWorksInfoResult_query_spaceworks_info_result_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncQuerySpaceWorksInfoResult__SWIG_0")]
		public static extern IntPtr new_AsyncQuerySpaceWorksInfoResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncQuerySpaceWorksInfoResult__SWIG_1")]
		public static extern IntPtr new_AsyncQuerySpaceWorksInfoResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncQuerySpaceWorksInfoResult")]
		public static extern void delete_AsyncQuerySpaceWorksInfoResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_QueryMySubscribedSpaceWorksResult_spacework_descriptors_set")]
		public static extern void QueryMySubscribedSpaceWorksResult_spacework_descriptors_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_QueryMySubscribedSpaceWorksResult_spacework_descriptors_get")]
		public static extern IntPtr QueryMySubscribedSpaceWorksResult_spacework_descriptors_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_QueryMySubscribedSpaceWorksResult_spacework_type_set")]
		public static extern void QueryMySubscribedSpaceWorksResult_spacework_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_QueryMySubscribedSpaceWorksResult_spacework_type_get")]
		public static extern int QueryMySubscribedSpaceWorksResult_spacework_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_QueryMySubscribedSpaceWorksResult_total_available_works_set")]
		public static extern void QueryMySubscribedSpaceWorksResult_total_available_works_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_QueryMySubscribedSpaceWorksResult_total_available_works_get")]
		public static extern uint QueryMySubscribedSpaceWorksResult_total_available_works_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_QueryMySubscribedSpaceWorksResult__SWIG_0")]
		public static extern IntPtr new_QueryMySubscribedSpaceWorksResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_QueryMySubscribedSpaceWorksResult__SWIG_1")]
		public static extern IntPtr new_QueryMySubscribedSpaceWorksResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_QueryMySubscribedSpaceWorksResult")]
		public static extern void delete_QueryMySubscribedSpaceWorksResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkUpdateOptions_with_detail_set")]
		public static extern void RailSpaceWorkUpdateOptions_with_detail_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkUpdateOptions_with_detail_get")]
		public static extern bool RailSpaceWorkUpdateOptions_with_detail_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkUpdateOptions_with_metadata_set")]
		public static extern void RailSpaceWorkUpdateOptions_with_metadata_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkUpdateOptions_with_metadata_get")]
		public static extern bool RailSpaceWorkUpdateOptions_with_metadata_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkUpdateOptions_check_has_subscribed_set")]
		public static extern void RailSpaceWorkUpdateOptions_check_has_subscribed_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkUpdateOptions_check_has_subscribed_get")]
		public static extern bool RailSpaceWorkUpdateOptions_check_has_subscribed_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkUpdateOptions_check_has_favorited_set")]
		public static extern void RailSpaceWorkUpdateOptions_check_has_favorited_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkUpdateOptions_check_has_favorited_get")]
		public static extern bool RailSpaceWorkUpdateOptions_check_has_favorited_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkUpdateOptions_with_my_vote_set")]
		public static extern void RailSpaceWorkUpdateOptions_with_my_vote_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkUpdateOptions_with_my_vote_get")]
		public static extern bool RailSpaceWorkUpdateOptions_with_my_vote_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkUpdateOptions_with_vote_detail_set")]
		public static extern void RailSpaceWorkUpdateOptions_with_vote_detail_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkUpdateOptions_with_vote_detail_get")]
		public static extern bool RailSpaceWorkUpdateOptions_with_vote_detail_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSpaceWorkUpdateOptions__SWIG_0")]
		public static extern IntPtr new_RailSpaceWorkUpdateOptions__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSpaceWorkUpdateOptions__SWIG_1")]
		public static extern IntPtr new_RailSpaceWorkUpdateOptions__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSpaceWorkUpdateOptions")]
		public static extern void delete_RailSpaceWorkUpdateOptions(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSpaceWorkSearchFilter__SWIG_0")]
		public static extern IntPtr new_RailSpaceWorkSearchFilter__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_search_text_set")]
		public static extern void RailSpaceWorkSearchFilter_search_text_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_search_text_get")]
		public static extern IntPtr RailSpaceWorkSearchFilter_search_text_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_required_tags_set")]
		public static extern void RailSpaceWorkSearchFilter_required_tags_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_required_tags_get")]
		public static extern IntPtr RailSpaceWorkSearchFilter_required_tags_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_excluded_tags_set")]
		public static extern void RailSpaceWorkSearchFilter_excluded_tags_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_excluded_tags_get")]
		public static extern IntPtr RailSpaceWorkSearchFilter_excluded_tags_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_required_metadata_set")]
		public static extern void RailSpaceWorkSearchFilter_required_metadata_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_required_metadata_get")]
		public static extern IntPtr RailSpaceWorkSearchFilter_required_metadata_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_excluded_metadata_set")]
		public static extern void RailSpaceWorkSearchFilter_excluded_metadata_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_excluded_metadata_get")]
		public static extern IntPtr RailSpaceWorkSearchFilter_excluded_metadata_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_match_all_required_metadata_set")]
		public static extern void RailSpaceWorkSearchFilter_match_all_required_metadata_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_match_all_required_metadata_get")]
		public static extern bool RailSpaceWorkSearchFilter_match_all_required_metadata_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_match_all_required_tags_set")]
		public static extern void RailSpaceWorkSearchFilter_match_all_required_tags_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSpaceWorkSearchFilter_match_all_required_tags_get")]
		public static extern bool RailSpaceWorkSearchFilter_match_all_required_tags_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSpaceWorkSearchFilter__SWIG_1")]
		public static extern IntPtr new_RailSpaceWorkSearchFilter__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSpaceWorkSearchFilter")]
		public static extern void delete_RailSpaceWorkSearchFilter(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInviteOptions_invite_type_set")]
		public static extern void RailInviteOptions_invite_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInviteOptions_invite_type_get")]
		public static extern int RailInviteOptions_invite_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInviteOptions_need_respond_in_game_set")]
		public static extern void RailInviteOptions_need_respond_in_game_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInviteOptions_need_respond_in_game_get")]
		public static extern bool RailInviteOptions_need_respond_in_game_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInviteOptions_additional_message_set")]
		public static extern void RailInviteOptions_additional_message_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInviteOptions_additional_message_get")]
		public static extern IntPtr RailInviteOptions_additional_message_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInviteOptions_expire_time_set")]
		public static extern void RailInviteOptions_expire_time_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInviteOptions_expire_time_get")]
		public static extern uint RailInviteOptions_expire_time_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInviteOptions__SWIG_0")]
		public static extern IntPtr new_RailInviteOptions__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInviteOptions__SWIG_1")]
		public static extern IntPtr new_RailInviteOptions__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailInviteOptions")]
		public static extern void delete_RailInviteOptions(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersInfoData__SWIG_0")]
		public static extern IntPtr new_RailUsersInfoData__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersInfoData_user_info_list_set")]
		public static extern void RailUsersInfoData_user_info_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersInfoData_user_info_list_get")]
		public static extern IntPtr RailUsersInfoData_user_info_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersInfoData__SWIG_1")]
		public static extern IntPtr new_RailUsersInfoData__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailUsersInfoData")]
		public static extern void delete_RailUsersInfoData(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersNotifyInviter__SWIG_0")]
		public static extern IntPtr new_RailUsersNotifyInviter__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersNotifyInviter_invitee_id_set")]
		public static extern void RailUsersNotifyInviter_invitee_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersNotifyInviter_invitee_id_get")]
		public static extern IntPtr RailUsersNotifyInviter_invitee_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersNotifyInviter__SWIG_1")]
		public static extern IntPtr new_RailUsersNotifyInviter__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailUsersNotifyInviter")]
		public static extern void delete_RailUsersNotifyInviter(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersRespondInvitation__SWIG_0")]
		public static extern IntPtr new_RailUsersRespondInvitation__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersRespondInvitation_inviter_id_set")]
		public static extern void RailUsersRespondInvitation_inviter_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersRespondInvitation_inviter_id_get")]
		public static extern IntPtr RailUsersRespondInvitation_inviter_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersRespondInvitation_response_set")]
		public static extern void RailUsersRespondInvitation_response_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersRespondInvitation_response_get")]
		public static extern int RailUsersRespondInvitation_response_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersRespondInvitation_original_invite_option_set")]
		public static extern void RailUsersRespondInvitation_original_invite_option_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersRespondInvitation_original_invite_option_get")]
		public static extern IntPtr RailUsersRespondInvitation_original_invite_option_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersRespondInvitation__SWIG_1")]
		public static extern IntPtr new_RailUsersRespondInvitation__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailUsersRespondInvitation")]
		public static extern void delete_RailUsersRespondInvitation(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersInviteJoinGameResult__SWIG_0")]
		public static extern IntPtr new_RailUsersInviteJoinGameResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersInviteJoinGameResult_invitee_id_set")]
		public static extern void RailUsersInviteJoinGameResult_invitee_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersInviteJoinGameResult_invitee_id_get")]
		public static extern IntPtr RailUsersInviteJoinGameResult_invitee_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersInviteJoinGameResult_response_value_set")]
		public static extern void RailUsersInviteJoinGameResult_response_value_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersInviteJoinGameResult_response_value_get")]
		public static extern int RailUsersInviteJoinGameResult_response_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersInviteJoinGameResult_invite_type_set")]
		public static extern void RailUsersInviteJoinGameResult_invite_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersInviteJoinGameResult_invite_type_get")]
		public static extern int RailUsersInviteJoinGameResult_invite_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersInviteJoinGameResult__SWIG_1")]
		public static extern IntPtr new_RailUsersInviteJoinGameResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailUsersInviteJoinGameResult")]
		public static extern void delete_RailUsersInviteJoinGameResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersGetInviteDetailResult__SWIG_0")]
		public static extern IntPtr new_RailUsersGetInviteDetailResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersGetInviteDetailResult_inviter_id_set")]
		public static extern void RailUsersGetInviteDetailResult_inviter_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersGetInviteDetailResult_inviter_id_get")]
		public static extern IntPtr RailUsersGetInviteDetailResult_inviter_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersGetInviteDetailResult_command_line_set")]
		public static extern void RailUsersGetInviteDetailResult_command_line_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersGetInviteDetailResult_command_line_get")]
		public static extern IntPtr RailUsersGetInviteDetailResult_command_line_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersGetInviteDetailResult_invite_type_set")]
		public static extern void RailUsersGetInviteDetailResult_invite_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersGetInviteDetailResult_invite_type_get")]
		public static extern int RailUsersGetInviteDetailResult_invite_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersGetInviteDetailResult__SWIG_1")]
		public static extern IntPtr new_RailUsersGetInviteDetailResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailUsersGetInviteDetailResult")]
		public static extern void delete_RailUsersGetInviteDetailResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersCancelInviteResult__SWIG_0")]
		public static extern IntPtr new_RailUsersCancelInviteResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersCancelInviteResult_invite_type_set")]
		public static extern void RailUsersCancelInviteResult_invite_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersCancelInviteResult_invite_type_get")]
		public static extern int RailUsersCancelInviteResult_invite_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersCancelInviteResult__SWIG_1")]
		public static extern IntPtr new_RailUsersCancelInviteResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailUsersCancelInviteResult")]
		public static extern void delete_RailUsersCancelInviteResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersInviteUsersResult__SWIG_0")]
		public static extern IntPtr new_RailUsersInviteUsersResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersInviteUsersResult_invite_type_set")]
		public static extern void RailUsersInviteUsersResult_invite_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersInviteUsersResult_invite_type_get")]
		public static extern int RailUsersInviteUsersResult_invite_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersInviteUsersResult__SWIG_1")]
		public static extern IntPtr new_RailUsersInviteUsersResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailUsersInviteUsersResult")]
		public static extern void delete_RailUsersInviteUsersResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersGetUserLimitsResult__SWIG_0")]
		public static extern IntPtr new_RailUsersGetUserLimitsResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersGetUserLimitsResult_user_id_set")]
		public static extern void RailUsersGetUserLimitsResult_user_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersGetUserLimitsResult_user_id_get")]
		public static extern IntPtr RailUsersGetUserLimitsResult_user_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersGetUserLimitsResult_user_limits_set")]
		public static extern void RailUsersGetUserLimitsResult_user_limits_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersGetUserLimitsResult_user_limits_get")]
		public static extern IntPtr RailUsersGetUserLimitsResult_user_limits_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUsersGetUserLimitsResult__SWIG_1")]
		public static extern IntPtr new_RailUsersGetUserLimitsResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailUsersGetUserLimitsResult")]
		public static extern void delete_RailUsersGetUserLimitsResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailShowChatWindowWithFriendResult__SWIG_0")]
		public static extern IntPtr new_RailShowChatWindowWithFriendResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailShowChatWindowWithFriendResult_is_show_set")]
		public static extern void RailShowChatWindowWithFriendResult_is_show_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailShowChatWindowWithFriendResult_is_show_get")]
		public static extern bool RailShowChatWindowWithFriendResult_is_show_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailShowChatWindowWithFriendResult__SWIG_1")]
		public static extern IntPtr new_RailShowChatWindowWithFriendResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailShowChatWindowWithFriendResult")]
		public static extern void delete_RailShowChatWindowWithFriendResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailShowUserHomepageWindowResult__SWIG_0")]
		public static extern IntPtr new_RailShowUserHomepageWindowResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailShowUserHomepageWindowResult_is_show_set")]
		public static extern void RailShowUserHomepageWindowResult_is_show_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailShowUserHomepageWindowResult_is_show_get")]
		public static extern bool RailShowUserHomepageWindowResult_is_show_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailShowUserHomepageWindowResult__SWIG_1")]
		public static extern IntPtr new_RailShowUserHomepageWindowResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailShowUserHomepageWindowResult")]
		public static extern void delete_RailShowUserHomepageWindowResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailImageDataDescriptor__SWIG_0")]
		public static extern IntPtr new_RailImageDataDescriptor__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailImageDataDescriptor_image_width_set")]
		public static extern void RailImageDataDescriptor_image_width_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailImageDataDescriptor_image_width_get")]
		public static extern uint RailImageDataDescriptor_image_width_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailImageDataDescriptor_image_height_set")]
		public static extern void RailImageDataDescriptor_image_height_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailImageDataDescriptor_image_height_get")]
		public static extern uint RailImageDataDescriptor_image_height_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailImageDataDescriptor_stride_in_bytes_set")]
		public static extern void RailImageDataDescriptor_stride_in_bytes_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailImageDataDescriptor_stride_in_bytes_get")]
		public static extern uint RailImageDataDescriptor_stride_in_bytes_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailImageDataDescriptor_bits_per_pixel_set")]
		public static extern void RailImageDataDescriptor_bits_per_pixel_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailImageDataDescriptor_bits_per_pixel_get")]
		public static extern uint RailImageDataDescriptor_bits_per_pixel_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailImageDataDescriptor_pixel_format_set")]
		public static extern void RailImageDataDescriptor_pixel_format_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailImageDataDescriptor_pixel_format_get")]
		public static extern int RailImageDataDescriptor_pixel_format_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailImageDataDescriptor__SWIG_1")]
		public static extern IntPtr new_RailImageDataDescriptor__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailImageDataDescriptor")]
		public static extern void delete_RailImageDataDescriptor(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailDirtyWordsCheckResult__SWIG_0")]
		public static extern IntPtr new_RailDirtyWordsCheckResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDirtyWordsCheckResult_replace_string_set")]
		public static extern void RailDirtyWordsCheckResult_replace_string_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDirtyWordsCheckResult_replace_string_get")]
		public static extern IntPtr RailDirtyWordsCheckResult_replace_string_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDirtyWordsCheckResult_dirty_type_set")]
		public static extern void RailDirtyWordsCheckResult_dirty_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDirtyWordsCheckResult_dirty_type_get")]
		public static extern int RailDirtyWordsCheckResult_dirty_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailDirtyWordsCheckResult__SWIG_1")]
		public static extern IntPtr new_RailDirtyWordsCheckResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailDirtyWordsCheckResult")]
		public static extern void delete_RailDirtyWordsCheckResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCrashInfo_exception_type_set")]
		public static extern void RailCrashInfo_exception_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCrashInfo_exception_type_get")]
		public static extern int RailCrashInfo_exception_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailCrashInfo__SWIG_0")]
		public static extern IntPtr new_RailCrashInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailCrashInfo__SWIG_1")]
		public static extern IntPtr new_RailCrashInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailCrashInfo")]
		public static extern void delete_RailCrashInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCrashBuffer_GetData")]
		public static extern IntPtr RailCrashBuffer_GetData(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCrashBuffer_GetBufferLength")]
		public static extern uint RailCrashBuffer_GetBufferLength(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCrashBuffer_GetValidLength")]
		public static extern uint RailCrashBuffer_GetValidLength(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCrashBuffer_SetData__SWIG_0")]
		public static extern uint RailCrashBuffer_SetData__SWIG_0(IntPtr jarg1, string jarg2, uint jarg3, uint jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCrashBuffer_SetData__SWIG_1")]
		public static extern uint RailCrashBuffer_SetData__SWIG_1(IntPtr jarg1, string jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCrashBuffer_AppendData")]
		public static extern uint RailCrashBuffer_AppendData(IntPtr jarg1, string jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailCrashBuffer")]
		public static extern void delete_RailCrashBuffer(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGetImageDataResult__SWIG_0")]
		public static extern IntPtr new_RailGetImageDataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetImageDataResult_image_data_set")]
		public static extern void RailGetImageDataResult_image_data_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetImageDataResult_image_data_get")]
		public static extern IntPtr RailGetImageDataResult_image_data_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetImageDataResult_image_data_descriptor_set")]
		public static extern void RailGetImageDataResult_image_data_descriptor_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetImageDataResult_image_data_descriptor_get")]
		public static extern IntPtr RailGetImageDataResult_image_data_descriptor_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGetImageDataResult__SWIG_1")]
		public static extern IntPtr new_RailGetImageDataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailGetImageDataResult")]
		public static extern void delete_RailGetImageDataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGameSettingMetadataChanged__SWIG_0")]
		public static extern IntPtr new_RailGameSettingMetadataChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameSettingMetadataChanged_source_set")]
		public static extern void RailGameSettingMetadataChanged_source_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameSettingMetadataChanged_source_get")]
		public static extern int RailGameSettingMetadataChanged_source_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameSettingMetadataChanged_key_values_set")]
		public static extern void RailGameSettingMetadataChanged_key_values_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameSettingMetadataChanged_key_values_get")]
		public static extern IntPtr RailGameSettingMetadataChanged_key_values_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGameSettingMetadataChanged__SWIG_1")]
		public static extern IntPtr new_RailGameSettingMetadataChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailGameSettingMetadataChanged")]
		public static extern void delete_RailGameSettingMetadataChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_TakeScreenshotResult__SWIG_0")]
		public static extern IntPtr new_TakeScreenshotResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_TakeScreenshotResult_image_file_path_set")]
		public static extern void TakeScreenshotResult_image_file_path_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_TakeScreenshotResult_image_file_path_get")]
		public static extern IntPtr TakeScreenshotResult_image_file_path_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_TakeScreenshotResult_image_file_size_set")]
		public static extern void TakeScreenshotResult_image_file_size_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_TakeScreenshotResult_image_file_size_get")]
		public static extern uint TakeScreenshotResult_image_file_size_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_TakeScreenshotResult_thumbnail_filepath_set")]
		public static extern void TakeScreenshotResult_thumbnail_filepath_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_TakeScreenshotResult_thumbnail_filepath_get")]
		public static extern IntPtr TakeScreenshotResult_thumbnail_filepath_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_TakeScreenshotResult_thumbnail_file_size_set")]
		public static extern void TakeScreenshotResult_thumbnail_file_size_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_TakeScreenshotResult_thumbnail_file_size_get")]
		public static extern uint TakeScreenshotResult_thumbnail_file_size_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_TakeScreenshotResult__SWIG_1")]
		public static extern IntPtr new_TakeScreenshotResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_TakeScreenshotResult")]
		public static extern void delete_TakeScreenshotResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ScreenshotRequestInfo__SWIG_0")]
		public static extern IntPtr new_ScreenshotRequestInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ScreenshotRequestInfo__SWIG_1")]
		public static extern IntPtr new_ScreenshotRequestInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_ScreenshotRequestInfo")]
		public static extern void delete_ScreenshotRequestInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PublishScreenshotResult__SWIG_0")]
		public static extern IntPtr new_PublishScreenshotResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_PublishScreenshotResult_work_id_set")]
		public static extern void PublishScreenshotResult_work_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_PublishScreenshotResult_work_id_get")]
		public static extern IntPtr PublishScreenshotResult_work_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PublishScreenshotResult__SWIG_1")]
		public static extern IntPtr new_PublishScreenshotResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_PublishScreenshotResult")]
		public static extern void delete_PublishScreenshotResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSystemStateChanged__SWIG_0")]
		public static extern IntPtr new_RailSystemStateChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSystemStateChanged_state_set")]
		public static extern void RailSystemStateChanged_state_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSystemStateChanged_state_get")]
		public static extern int RailSystemStateChanged_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSystemStateChanged__SWIG_1")]
		public static extern IntPtr new_RailSystemStateChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSystemStateChanged")]
		public static extern void delete_RailSystemStateChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPlatformNotifyEventJoinGameByGameServer__SWIG_0")]
		public static extern IntPtr new_RailPlatformNotifyEventJoinGameByGameServer__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByGameServer_gameserver_railid_set")]
		public static extern void RailPlatformNotifyEventJoinGameByGameServer_gameserver_railid_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByGameServer_gameserver_railid_get")]
		public static extern IntPtr RailPlatformNotifyEventJoinGameByGameServer_gameserver_railid_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByGameServer_commandline_info_set")]
		public static extern void RailPlatformNotifyEventJoinGameByGameServer_commandline_info_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByGameServer_commandline_info_get")]
		public static extern IntPtr RailPlatformNotifyEventJoinGameByGameServer_commandline_info_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPlatformNotifyEventJoinGameByGameServer__SWIG_1")]
		public static extern IntPtr new_RailPlatformNotifyEventJoinGameByGameServer__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailPlatformNotifyEventJoinGameByGameServer")]
		public static extern void delete_RailPlatformNotifyEventJoinGameByGameServer(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPlatformNotifyEventJoinGameByRoom__SWIG_0")]
		public static extern IntPtr new_RailPlatformNotifyEventJoinGameByRoom__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByRoom_room_id_set")]
		public static extern void RailPlatformNotifyEventJoinGameByRoom_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByRoom_room_id_get")]
		public static extern ulong RailPlatformNotifyEventJoinGameByRoom_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByRoom_commandline_info_set")]
		public static extern void RailPlatformNotifyEventJoinGameByRoom_commandline_info_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByRoom_commandline_info_get")]
		public static extern IntPtr RailPlatformNotifyEventJoinGameByRoom_commandline_info_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPlatformNotifyEventJoinGameByRoom__SWIG_1")]
		public static extern IntPtr new_RailPlatformNotifyEventJoinGameByRoom__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailPlatformNotifyEventJoinGameByRoom")]
		public static extern void delete_RailPlatformNotifyEventJoinGameByRoom(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPlatformNotifyEventJoinGameByUser__SWIG_0")]
		public static extern IntPtr new_RailPlatformNotifyEventJoinGameByUser__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByUser_rail_id_to_join_set")]
		public static extern void RailPlatformNotifyEventJoinGameByUser_rail_id_to_join_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByUser_rail_id_to_join_get")]
		public static extern IntPtr RailPlatformNotifyEventJoinGameByUser_rail_id_to_join_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByUser_commandline_info_set")]
		public static extern void RailPlatformNotifyEventJoinGameByUser_commandline_info_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByUser_commandline_info_get")]
		public static extern IntPtr RailPlatformNotifyEventJoinGameByUser_commandline_info_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPlatformNotifyEventJoinGameByUser__SWIG_1")]
		public static extern IntPtr new_RailPlatformNotifyEventJoinGameByUser__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailPlatformNotifyEventJoinGameByUser")]
		public static extern void delete_RailPlatformNotifyEventJoinGameByUser(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFinalize__SWIG_0")]
		public static extern IntPtr new_RailFinalize__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFinalize__SWIG_1")]
		public static extern IntPtr new_RailFinalize__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFinalize")]
		public static extern void delete_RailFinalize(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailAssetItem__SWIG_0")]
		public static extern IntPtr new_RailAssetItem__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetItem_asset_id_set")]
		public static extern void RailAssetItem_asset_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetItem_asset_id_get")]
		public static extern ulong RailAssetItem_asset_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetItem_quantity_set")]
		public static extern void RailAssetItem_quantity_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetItem_quantity_get")]
		public static extern uint RailAssetItem_quantity_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailAssetItem__SWIG_1")]
		public static extern IntPtr new_RailAssetItem__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailAssetItem")]
		public static extern void delete_RailAssetItem(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGeneratedAssetItem__SWIG_0")]
		public static extern IntPtr new_RailGeneratedAssetItem__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGeneratedAssetItem_product_id_set")]
		public static extern void RailGeneratedAssetItem_product_id_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGeneratedAssetItem_product_id_get")]
		public static extern uint RailGeneratedAssetItem_product_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGeneratedAssetItem_asset_set")]
		public static extern void RailGeneratedAssetItem_asset_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGeneratedAssetItem_asset_get")]
		public static extern IntPtr RailGeneratedAssetItem_asset_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGeneratedAssetItem_container_id_set")]
		public static extern void RailGeneratedAssetItem_container_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGeneratedAssetItem_container_id_get")]
		public static extern ulong RailGeneratedAssetItem_container_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGeneratedAssetItem__SWIG_1")]
		public static extern IntPtr new_RailGeneratedAssetItem__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailGeneratedAssetItem")]
		public static extern void delete_RailGeneratedAssetItem(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailAssetInfo__SWIG_0")]
		public static extern IntPtr new_RailAssetInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_asset_id_set")]
		public static extern void RailAssetInfo_asset_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_asset_id_get")]
		public static extern ulong RailAssetInfo_asset_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_product_id_set")]
		public static extern void RailAssetInfo_product_id_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_product_id_get")]
		public static extern uint RailAssetInfo_product_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_product_name_set")]
		public static extern void RailAssetInfo_product_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_product_name_get")]
		public static extern IntPtr RailAssetInfo_product_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_position_set")]
		public static extern void RailAssetInfo_position_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_position_get")]
		public static extern int RailAssetInfo_position_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_progress_set")]
		public static extern void RailAssetInfo_progress_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_progress_get")]
		public static extern IntPtr RailAssetInfo_progress_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_quantity_set")]
		public static extern void RailAssetInfo_quantity_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_quantity_get")]
		public static extern uint RailAssetInfo_quantity_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_state_set")]
		public static extern void RailAssetInfo_state_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_state_get")]
		public static extern uint RailAssetInfo_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_flag_set")]
		public static extern void RailAssetInfo_flag_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_flag_get")]
		public static extern uint RailAssetInfo_flag_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_origin_set")]
		public static extern void RailAssetInfo_origin_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_origin_get")]
		public static extern uint RailAssetInfo_origin_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_expire_time_set")]
		public static extern void RailAssetInfo_expire_time_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_expire_time_get")]
		public static extern uint RailAssetInfo_expire_time_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_container_id_set")]
		public static extern void RailAssetInfo_container_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetInfo_container_id_get")]
		public static extern ulong RailAssetInfo_container_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailAssetInfo__SWIG_1")]
		public static extern IntPtr new_RailAssetInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailAssetInfo")]
		public static extern void delete_RailAssetInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailProductItem__SWIG_0")]
		public static extern IntPtr new_RailProductItem__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailProductItem_product_id_set")]
		public static extern void RailProductItem_product_id_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailProductItem_product_id_get")]
		public static extern uint RailProductItem_product_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailProductItem_quantity_set")]
		public static extern void RailProductItem_quantity_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailProductItem_quantity_get")]
		public static extern uint RailProductItem_quantity_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailProductItem__SWIG_1")]
		public static extern IntPtr new_RailProductItem__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailProductItem")]
		public static extern void delete_RailProductItem(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailAssetProperty__SWIG_0")]
		public static extern IntPtr new_RailAssetProperty__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetProperty_asset_id_set")]
		public static extern void RailAssetProperty_asset_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetProperty_asset_id_get")]
		public static extern ulong RailAssetProperty_asset_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetProperty_position_set")]
		public static extern void RailAssetProperty_position_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetProperty_position_get")]
		public static extern uint RailAssetProperty_position_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailAssetProperty__SWIG_1")]
		public static extern IntPtr new_RailAssetProperty__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailAssetProperty")]
		public static extern void delete_RailAssetProperty(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RequestAllAssetsFinished__SWIG_0")]
		public static extern IntPtr new_RequestAllAssetsFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RequestAllAssetsFinished_assetinfo_list_set")]
		public static extern void RequestAllAssetsFinished_assetinfo_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RequestAllAssetsFinished_assetinfo_list_get")]
		public static extern IntPtr RequestAllAssetsFinished_assetinfo_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RequestAllAssetsFinished__SWIG_1")]
		public static extern IntPtr new_RequestAllAssetsFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RequestAllAssetsFinished")]
		public static extern void delete_RequestAllAssetsFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_UpdateAssetsPropertyFinished__SWIG_0")]
		public static extern IntPtr new_UpdateAssetsPropertyFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_UpdateAssetsPropertyFinished_asset_property_list_set")]
		public static extern void UpdateAssetsPropertyFinished_asset_property_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_UpdateAssetsPropertyFinished_asset_property_list_get")]
		public static extern IntPtr UpdateAssetsPropertyFinished_asset_property_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_UpdateAssetsPropertyFinished__SWIG_1")]
		public static extern IntPtr new_UpdateAssetsPropertyFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_UpdateAssetsPropertyFinished")]
		public static extern void delete_UpdateAssetsPropertyFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DirectConsumeAssetsFinished__SWIG_0")]
		public static extern IntPtr new_DirectConsumeAssetsFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_DirectConsumeAssetsFinished_assets_set")]
		public static extern void DirectConsumeAssetsFinished_assets_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DirectConsumeAssetsFinished_assets_get")]
		public static extern IntPtr DirectConsumeAssetsFinished_assets_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DirectConsumeAssetsFinished__SWIG_1")]
		public static extern IntPtr new_DirectConsumeAssetsFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_DirectConsumeAssetsFinished")]
		public static extern void delete_DirectConsumeAssetsFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_StartConsumeAssetsFinished__SWIG_0")]
		public static extern IntPtr new_StartConsumeAssetsFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_StartConsumeAssetsFinished_asset_id_set")]
		public static extern void StartConsumeAssetsFinished_asset_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_StartConsumeAssetsFinished_asset_id_get")]
		public static extern ulong StartConsumeAssetsFinished_asset_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_StartConsumeAssetsFinished__SWIG_1")]
		public static extern IntPtr new_StartConsumeAssetsFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_StartConsumeAssetsFinished")]
		public static extern void delete_StartConsumeAssetsFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_UpdateConsumeAssetsFinished__SWIG_0")]
		public static extern IntPtr new_UpdateConsumeAssetsFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_UpdateConsumeAssetsFinished_asset_id_set")]
		public static extern void UpdateConsumeAssetsFinished_asset_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_UpdateConsumeAssetsFinished_asset_id_get")]
		public static extern ulong UpdateConsumeAssetsFinished_asset_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_UpdateConsumeAssetsFinished__SWIG_1")]
		public static extern IntPtr new_UpdateConsumeAssetsFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_UpdateConsumeAssetsFinished")]
		public static extern void delete_UpdateConsumeAssetsFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CompleteConsumeAssetsFinished__SWIG_0")]
		public static extern IntPtr new_CompleteConsumeAssetsFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_CompleteConsumeAssetsFinished_asset_item_set")]
		public static extern void CompleteConsumeAssetsFinished_asset_item_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CompleteConsumeAssetsFinished_asset_item_get")]
		public static extern IntPtr CompleteConsumeAssetsFinished_asset_item_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CompleteConsumeAssetsFinished__SWIG_1")]
		public static extern IntPtr new_CompleteConsumeAssetsFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CompleteConsumeAssetsFinished")]
		public static extern void delete_CompleteConsumeAssetsFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SplitAssetsFinished__SWIG_0")]
		public static extern IntPtr new_SplitAssetsFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsFinished_source_asset_set")]
		public static extern void SplitAssetsFinished_source_asset_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsFinished_source_asset_get")]
		public static extern ulong SplitAssetsFinished_source_asset_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsFinished_to_quantity_set")]
		public static extern void SplitAssetsFinished_to_quantity_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsFinished_to_quantity_get")]
		public static extern uint SplitAssetsFinished_to_quantity_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsFinished_new_asset_id_set")]
		public static extern void SplitAssetsFinished_new_asset_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsFinished_new_asset_id_get")]
		public static extern ulong SplitAssetsFinished_new_asset_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SplitAssetsFinished__SWIG_1")]
		public static extern IntPtr new_SplitAssetsFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_SplitAssetsFinished")]
		public static extern void delete_SplitAssetsFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SplitAssetsToFinished__SWIG_0")]
		public static extern IntPtr new_SplitAssetsToFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsToFinished_source_asset_set")]
		public static extern void SplitAssetsToFinished_source_asset_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsToFinished_source_asset_get")]
		public static extern ulong SplitAssetsToFinished_source_asset_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsToFinished_to_quantity_set")]
		public static extern void SplitAssetsToFinished_to_quantity_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsToFinished_to_quantity_get")]
		public static extern uint SplitAssetsToFinished_to_quantity_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsToFinished_split_to_asset_id_set")]
		public static extern void SplitAssetsToFinished_split_to_asset_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsToFinished_split_to_asset_id_get")]
		public static extern ulong SplitAssetsToFinished_split_to_asset_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SplitAssetsToFinished__SWIG_1")]
		public static extern IntPtr new_SplitAssetsToFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_SplitAssetsToFinished")]
		public static extern void delete_SplitAssetsToFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_MergeAssetsFinished__SWIG_0")]
		public static extern IntPtr new_MergeAssetsFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_MergeAssetsFinished_source_assets_set")]
		public static extern void MergeAssetsFinished_source_assets_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_MergeAssetsFinished_source_assets_get")]
		public static extern IntPtr MergeAssetsFinished_source_assets_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_MergeAssetsFinished_new_asset_id_set")]
		public static extern void MergeAssetsFinished_new_asset_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_MergeAssetsFinished_new_asset_id_get")]
		public static extern ulong MergeAssetsFinished_new_asset_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_MergeAssetsFinished__SWIG_1")]
		public static extern IntPtr new_MergeAssetsFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_MergeAssetsFinished")]
		public static extern void delete_MergeAssetsFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_MergeAssetsToFinished__SWIG_0")]
		public static extern IntPtr new_MergeAssetsToFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_MergeAssetsToFinished_source_assets_set")]
		public static extern void MergeAssetsToFinished_source_assets_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_MergeAssetsToFinished_source_assets_get")]
		public static extern IntPtr MergeAssetsToFinished_source_assets_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_MergeAssetsToFinished_merge_to_asset_id_set")]
		public static extern void MergeAssetsToFinished_merge_to_asset_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_MergeAssetsToFinished_merge_to_asset_id_get")]
		public static extern ulong MergeAssetsToFinished_merge_to_asset_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_MergeAssetsToFinished__SWIG_1")]
		public static extern IntPtr new_MergeAssetsToFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_MergeAssetsToFinished")]
		public static extern void delete_MergeAssetsToFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CompleteConsumeByExchangeAssetsToFinished__SWIG_0")]
		public static extern IntPtr new_CompleteConsumeByExchangeAssetsToFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CompleteConsumeByExchangeAssetsToFinished__SWIG_1")]
		public static extern IntPtr new_CompleteConsumeByExchangeAssetsToFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CompleteConsumeByExchangeAssetsToFinished")]
		public static extern void delete_CompleteConsumeByExchangeAssetsToFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ExchangeAssetsFinished__SWIG_0")]
		public static extern IntPtr new_ExchangeAssetsFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_ExchangeAssetsFinished_old_assets_set")]
		public static extern void ExchangeAssetsFinished_old_assets_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_ExchangeAssetsFinished_old_assets_get")]
		public static extern IntPtr ExchangeAssetsFinished_old_assets_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_ExchangeAssetsFinished_new_asset_item_list_set")]
		public static extern void ExchangeAssetsFinished_new_asset_item_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_ExchangeAssetsFinished_new_asset_item_list_get")]
		public static extern IntPtr ExchangeAssetsFinished_new_asset_item_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ExchangeAssetsFinished__SWIG_1")]
		public static extern IntPtr new_ExchangeAssetsFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_ExchangeAssetsFinished")]
		public static extern void delete_ExchangeAssetsFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ExchangeAssetsToFinished__SWIG_0")]
		public static extern IntPtr new_ExchangeAssetsToFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_ExchangeAssetsToFinished_old_assets_set")]
		public static extern void ExchangeAssetsToFinished_old_assets_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_ExchangeAssetsToFinished_old_assets_get")]
		public static extern IntPtr ExchangeAssetsToFinished_old_assets_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_ExchangeAssetsToFinished_to_product_info_set")]
		public static extern void ExchangeAssetsToFinished_to_product_info_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_ExchangeAssetsToFinished_to_product_info_get")]
		public static extern IntPtr ExchangeAssetsToFinished_to_product_info_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_ExchangeAssetsToFinished_exchange_to_asset_id_set")]
		public static extern void ExchangeAssetsToFinished_exchange_to_asset_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_ExchangeAssetsToFinished_exchange_to_asset_id_get")]
		public static extern ulong ExchangeAssetsToFinished_exchange_to_asset_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ExchangeAssetsToFinished__SWIG_1")]
		public static extern IntPtr new_ExchangeAssetsToFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_ExchangeAssetsToFinished")]
		public static extern void delete_ExchangeAssetsToFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailAssetsChanged__SWIG_0")]
		public static extern IntPtr new_RailAssetsChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailAssetsChanged__SWIG_1")]
		public static extern IntPtr new_RailAssetsChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailAssetsChanged")]
		public static extern void delete_RailAssetsChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateBrowserOptions__SWIG_0")]
		public static extern IntPtr new_CreateBrowserOptions__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_has_maximum_button_set")]
		public static extern void CreateBrowserOptions_has_maximum_button_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_has_maximum_button_get")]
		public static extern bool CreateBrowserOptions_has_maximum_button_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_has_minimum_button_set")]
		public static extern void CreateBrowserOptions_has_minimum_button_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_has_minimum_button_get")]
		public static extern bool CreateBrowserOptions_has_minimum_button_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_has_border_set")]
		public static extern void CreateBrowserOptions_has_border_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_has_border_get")]
		public static extern bool CreateBrowserOptions_has_border_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_is_movable_set")]
		public static extern void CreateBrowserOptions_is_movable_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_is_movable_get")]
		public static extern bool CreateBrowserOptions_is_movable_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_margin_top_set")]
		public static extern void CreateBrowserOptions_margin_top_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_margin_top_get")]
		public static extern int CreateBrowserOptions_margin_top_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_margin_left_set")]
		public static extern void CreateBrowserOptions_margin_left_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_margin_left_get")]
		public static extern int CreateBrowserOptions_margin_left_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_allow_alternate_external_browser_set")]
		public static extern void CreateBrowserOptions_allow_alternate_external_browser_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserOptions_allow_alternate_external_browser_get")]
		public static extern bool CreateBrowserOptions_allow_alternate_external_browser_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateBrowserOptions__SWIG_1")]
		public static extern IntPtr new_CreateBrowserOptions__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CreateBrowserOptions")]
		public static extern void delete_CreateBrowserOptions(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateCustomerDrawBrowserOptions__SWIG_0")]
		public static extern IntPtr new_CreateCustomerDrawBrowserOptions__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateCustomerDrawBrowserOptions_content_offset_x_set")]
		public static extern void CreateCustomerDrawBrowserOptions_content_offset_x_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateCustomerDrawBrowserOptions_content_offset_x_get")]
		public static extern int CreateCustomerDrawBrowserOptions_content_offset_x_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateCustomerDrawBrowserOptions_content_offset_y_set")]
		public static extern void CreateCustomerDrawBrowserOptions_content_offset_y_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateCustomerDrawBrowserOptions_content_offset_y_get")]
		public static extern int CreateCustomerDrawBrowserOptions_content_offset_y_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateCustomerDrawBrowserOptions_content_window_width_set")]
		public static extern void CreateCustomerDrawBrowserOptions_content_window_width_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateCustomerDrawBrowserOptions_content_window_width_get")]
		public static extern uint CreateCustomerDrawBrowserOptions_content_window_width_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateCustomerDrawBrowserOptions_content_window_height_set")]
		public static extern void CreateCustomerDrawBrowserOptions_content_window_height_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateCustomerDrawBrowserOptions_content_window_height_get")]
		public static extern uint CreateCustomerDrawBrowserOptions_content_window_height_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateCustomerDrawBrowserOptions_has_scroll_set")]
		public static extern void CreateCustomerDrawBrowserOptions_has_scroll_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateCustomerDrawBrowserOptions_has_scroll_get")]
		public static extern bool CreateCustomerDrawBrowserOptions_has_scroll_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateCustomerDrawBrowserOptions__SWIG_1")]
		public static extern IntPtr new_CreateCustomerDrawBrowserOptions__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CreateCustomerDrawBrowserOptions")]
		public static extern void delete_CreateCustomerDrawBrowserOptions(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateBrowserResult__SWIG_0")]
		public static extern IntPtr new_CreateBrowserResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateBrowserResult__SWIG_1")]
		public static extern IntPtr new_CreateBrowserResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CreateBrowserResult")]
		public static extern void delete_CreateBrowserResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ReloadBrowserResult__SWIG_0")]
		public static extern IntPtr new_ReloadBrowserResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ReloadBrowserResult__SWIG_1")]
		public static extern IntPtr new_ReloadBrowserResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_ReloadBrowserResult")]
		public static extern void delete_ReloadBrowserResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CloseBrowserResult__SWIG_0")]
		public static extern IntPtr new_CloseBrowserResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CloseBrowserResult__SWIG_1")]
		public static extern IntPtr new_CloseBrowserResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CloseBrowserResult")]
		public static extern void delete_CloseBrowserResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_JavascriptEventResult__SWIG_0")]
		public static extern IntPtr new_JavascriptEventResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_JavascriptEventResult_event_name_set")]
		public static extern void JavascriptEventResult_event_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_JavascriptEventResult_event_name_get")]
		public static extern IntPtr JavascriptEventResult_event_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_JavascriptEventResult_event_value_set")]
		public static extern void JavascriptEventResult_event_value_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_JavascriptEventResult_event_value_get")]
		public static extern IntPtr JavascriptEventResult_event_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_JavascriptEventResult__SWIG_1")]
		public static extern IntPtr new_JavascriptEventResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_JavascriptEventResult")]
		public static extern void delete_JavascriptEventResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_BrowserNeedsPaintRequest__SWIG_0")]
		public static extern IntPtr new_BrowserNeedsPaintRequest__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_bgra_data_set")]
		public static extern void BrowserNeedsPaintRequest_bgra_data_set(IntPtr jarg1, string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_bgra_data_get")]
		public static extern IntPtr BrowserNeedsPaintRequest_bgra_data_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_offset_x_set")]
		public static extern void BrowserNeedsPaintRequest_offset_x_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_offset_x_get")]
		public static extern int BrowserNeedsPaintRequest_offset_x_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_offset_y_set")]
		public static extern void BrowserNeedsPaintRequest_offset_y_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_offset_y_get")]
		public static extern int BrowserNeedsPaintRequest_offset_y_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_bgra_width_set")]
		public static extern void BrowserNeedsPaintRequest_bgra_width_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_bgra_width_get")]
		public static extern uint BrowserNeedsPaintRequest_bgra_width_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_bgra_height_set")]
		public static extern void BrowserNeedsPaintRequest_bgra_height_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_bgra_height_get")]
		public static extern uint BrowserNeedsPaintRequest_bgra_height_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_scroll_x_pos_set")]
		public static extern void BrowserNeedsPaintRequest_scroll_x_pos_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_scroll_x_pos_get")]
		public static extern uint BrowserNeedsPaintRequest_scroll_x_pos_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_scroll_y_pos_set")]
		public static extern void BrowserNeedsPaintRequest_scroll_y_pos_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_scroll_y_pos_get")]
		public static extern uint BrowserNeedsPaintRequest_scroll_y_pos_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_page_scale_factor_set")]
		public static extern void BrowserNeedsPaintRequest_page_scale_factor_set(IntPtr jarg1, float jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_page_scale_factor_get")]
		public static extern float BrowserNeedsPaintRequest_page_scale_factor_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_BrowserNeedsPaintRequest__SWIG_1")]
		public static extern IntPtr new_BrowserNeedsPaintRequest__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_BrowserNeedsPaintRequest")]
		public static extern void delete_BrowserNeedsPaintRequest(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_BrowserDamageRectNeedsPaintRequest__SWIG_0")]
		public static extern IntPtr new_BrowserDamageRectNeedsPaintRequest__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_bgra_data_set")]
		public static extern void BrowserDamageRectNeedsPaintRequest_bgra_data_set(IntPtr jarg1, string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_bgra_data_get")]
		public static extern IntPtr BrowserDamageRectNeedsPaintRequest_bgra_data_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_offset_x_set")]
		public static extern void BrowserDamageRectNeedsPaintRequest_offset_x_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_offset_x_get")]
		public static extern int BrowserDamageRectNeedsPaintRequest_offset_x_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_offset_y_set")]
		public static extern void BrowserDamageRectNeedsPaintRequest_offset_y_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_offset_y_get")]
		public static extern int BrowserDamageRectNeedsPaintRequest_offset_y_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_bgra_width_set")]
		public static extern void BrowserDamageRectNeedsPaintRequest_bgra_width_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_bgra_width_get")]
		public static extern uint BrowserDamageRectNeedsPaintRequest_bgra_width_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_bgra_height_set")]
		public static extern void BrowserDamageRectNeedsPaintRequest_bgra_height_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_bgra_height_get")]
		public static extern uint BrowserDamageRectNeedsPaintRequest_bgra_height_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_update_offset_x_set")]
		public static extern void BrowserDamageRectNeedsPaintRequest_update_offset_x_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_update_offset_x_get")]
		public static extern int BrowserDamageRectNeedsPaintRequest_update_offset_x_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_update_offset_y_set")]
		public static extern void BrowserDamageRectNeedsPaintRequest_update_offset_y_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_update_offset_y_get")]
		public static extern int BrowserDamageRectNeedsPaintRequest_update_offset_y_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_update_bgra_width_set")]
		public static extern void BrowserDamageRectNeedsPaintRequest_update_bgra_width_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_update_bgra_width_get")]
		public static extern uint BrowserDamageRectNeedsPaintRequest_update_bgra_width_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_update_bgra_height_set")]
		public static extern void BrowserDamageRectNeedsPaintRequest_update_bgra_height_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_update_bgra_height_get")]
		public static extern uint BrowserDamageRectNeedsPaintRequest_update_bgra_height_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_scroll_x_pos_set")]
		public static extern void BrowserDamageRectNeedsPaintRequest_scroll_x_pos_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_scroll_x_pos_get")]
		public static extern uint BrowserDamageRectNeedsPaintRequest_scroll_x_pos_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_scroll_y_pos_set")]
		public static extern void BrowserDamageRectNeedsPaintRequest_scroll_y_pos_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_scroll_y_pos_get")]
		public static extern uint BrowserDamageRectNeedsPaintRequest_scroll_y_pos_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_page_scale_factor_set")]
		public static extern void BrowserDamageRectNeedsPaintRequest_page_scale_factor_set(IntPtr jarg1, float jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_page_scale_factor_get")]
		public static extern float BrowserDamageRectNeedsPaintRequest_page_scale_factor_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_BrowserDamageRectNeedsPaintRequest__SWIG_1")]
		public static extern IntPtr new_BrowserDamageRectNeedsPaintRequest__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_BrowserDamageRectNeedsPaintRequest")]
		public static extern void delete_BrowserDamageRectNeedsPaintRequest(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_BrowserRenderNavigateResult__SWIG_0")]
		public static extern IntPtr new_BrowserRenderNavigateResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserRenderNavigateResult_url_set")]
		public static extern void BrowserRenderNavigateResult_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserRenderNavigateResult_url_get")]
		public static extern IntPtr BrowserRenderNavigateResult_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_BrowserRenderNavigateResult__SWIG_1")]
		public static extern IntPtr new_BrowserRenderNavigateResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_BrowserRenderNavigateResult")]
		public static extern void delete_BrowserRenderNavigateResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_BrowserRenderStateChanged__SWIG_0")]
		public static extern IntPtr new_BrowserRenderStateChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserRenderStateChanged_can_go_back_set")]
		public static extern void BrowserRenderStateChanged_can_go_back_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserRenderStateChanged_can_go_back_get")]
		public static extern bool BrowserRenderStateChanged_can_go_back_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserRenderStateChanged_can_go_forward_set")]
		public static extern void BrowserRenderStateChanged_can_go_forward_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserRenderStateChanged_can_go_forward_get")]
		public static extern bool BrowserRenderStateChanged_can_go_forward_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_BrowserRenderStateChanged__SWIG_1")]
		public static extern IntPtr new_BrowserRenderStateChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_BrowserRenderStateChanged")]
		public static extern void delete_BrowserRenderStateChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_BrowserRenderTitleChanged__SWIG_0")]
		public static extern IntPtr new_BrowserRenderTitleChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserRenderTitleChanged_new_title_set")]
		public static extern void BrowserRenderTitleChanged_new_title_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserRenderTitleChanged_new_title_get")]
		public static extern IntPtr BrowserRenderTitleChanged_new_title_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_BrowserRenderTitleChanged__SWIG_1")]
		public static extern IntPtr new_BrowserRenderTitleChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_BrowserRenderTitleChanged")]
		public static extern void delete_BrowserRenderTitleChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_BrowserTryNavigateNewPageRequest__SWIG_0")]
		public static extern IntPtr new_BrowserTryNavigateNewPageRequest__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserTryNavigateNewPageRequest_url_set")]
		public static extern void BrowserTryNavigateNewPageRequest_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserTryNavigateNewPageRequest_url_get")]
		public static extern IntPtr BrowserTryNavigateNewPageRequest_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserTryNavigateNewPageRequest_target_type_set")]
		public static extern void BrowserTryNavigateNewPageRequest_target_type_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserTryNavigateNewPageRequest_target_type_get")]
		public static extern IntPtr BrowserTryNavigateNewPageRequest_target_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserTryNavigateNewPageRequest_is_redirect_request_set")]
		public static extern void BrowserTryNavigateNewPageRequest_is_redirect_request_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserTryNavigateNewPageRequest_is_redirect_request_get")]
		public static extern bool BrowserTryNavigateNewPageRequest_is_redirect_request_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_BrowserTryNavigateNewPageRequest__SWIG_1")]
		public static extern IntPtr new_BrowserTryNavigateNewPageRequest__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_BrowserTryNavigateNewPageRequest")]
		public static extern void delete_BrowserTryNavigateNewPageRequest(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_dlc_id_set")]
		public static extern void RailDlcInfo_dlc_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_dlc_id_get")]
		public static extern IntPtr RailDlcInfo_dlc_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_game_id_set")]
		public static extern void RailDlcInfo_game_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_game_id_get")]
		public static extern IntPtr RailDlcInfo_game_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_version_set")]
		public static extern void RailDlcInfo_version_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_version_get")]
		public static extern IntPtr RailDlcInfo_version_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_name_set")]
		public static extern void RailDlcInfo_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_name_get")]
		public static extern IntPtr RailDlcInfo_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_description_set")]
		public static extern void RailDlcInfo_description_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_description_get")]
		public static extern IntPtr RailDlcInfo_description_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_original_price_set")]
		public static extern void RailDlcInfo_original_price_set(IntPtr jarg1, double jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_original_price_get")]
		public static extern double RailDlcInfo_original_price_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_discount_price_set")]
		public static extern void RailDlcInfo_discount_price_set(IntPtr jarg1, double jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInfo_discount_price_get")]
		public static extern double RailDlcInfo_discount_price_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailDlcInfo__SWIG_0")]
		public static extern IntPtr new_RailDlcInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailDlcInfo__SWIG_1")]
		public static extern IntPtr new_RailDlcInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailDlcInfo")]
		public static extern void delete_RailDlcInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInstallProgress_progress_set")]
		public static extern void RailDlcInstallProgress_progress_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInstallProgress_progress_get")]
		public static extern uint RailDlcInstallProgress_progress_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInstallProgress_finished_bytes_set")]
		public static extern void RailDlcInstallProgress_finished_bytes_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInstallProgress_finished_bytes_get")]
		public static extern ulong RailDlcInstallProgress_finished_bytes_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInstallProgress_total_bytes_set")]
		public static extern void RailDlcInstallProgress_total_bytes_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInstallProgress_total_bytes_get")]
		public static extern ulong RailDlcInstallProgress_total_bytes_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInstallProgress_speed_set")]
		public static extern void RailDlcInstallProgress_speed_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcInstallProgress_speed_get")]
		public static extern uint RailDlcInstallProgress_speed_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailDlcInstallProgress__SWIG_0")]
		public static extern IntPtr new_RailDlcInstallProgress__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailDlcInstallProgress__SWIG_1")]
		public static extern IntPtr new_RailDlcInstallProgress__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailDlcInstallProgress")]
		public static extern void delete_RailDlcInstallProgress(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcOwned_is_owned_set")]
		public static extern void RailDlcOwned_is_owned_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcOwned_is_owned_get")]
		public static extern bool RailDlcOwned_is_owned_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcOwned_dlc_id_set")]
		public static extern void RailDlcOwned_dlc_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDlcOwned_dlc_id_get")]
		public static extern IntPtr RailDlcOwned_dlc_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailDlcOwned__SWIG_0")]
		public static extern IntPtr new_RailDlcOwned__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailDlcOwned__SWIG_1")]
		public static extern IntPtr new_RailDlcOwned__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailDlcOwned")]
		public static extern void delete_RailDlcOwned(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallStart_dlc_id_set")]
		public static extern void DlcInstallStart_dlc_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallStart_dlc_id_get")]
		public static extern IntPtr DlcInstallStart_dlc_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcInstallStart__SWIG_0")]
		public static extern IntPtr new_DlcInstallStart__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcInstallStart__SWIG_1")]
		public static extern IntPtr new_DlcInstallStart__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_DlcInstallStart")]
		public static extern void delete_DlcInstallStart(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallStartResult_dlc_id_set")]
		public static extern void DlcInstallStartResult_dlc_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallStartResult_dlc_id_get")]
		public static extern IntPtr DlcInstallStartResult_dlc_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallStartResult_result_set")]
		public static extern void DlcInstallStartResult_result_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallStartResult_result_get")]
		public static extern int DlcInstallStartResult_result_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcInstallStartResult__SWIG_0")]
		public static extern IntPtr new_DlcInstallStartResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcInstallStartResult__SWIG_1")]
		public static extern IntPtr new_DlcInstallStartResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_DlcInstallStartResult")]
		public static extern void delete_DlcInstallStartResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallProgress_dlc_id_set")]
		public static extern void DlcInstallProgress_dlc_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallProgress_dlc_id_get")]
		public static extern IntPtr DlcInstallProgress_dlc_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallProgress_progress_set")]
		public static extern void DlcInstallProgress_progress_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallProgress_progress_get")]
		public static extern IntPtr DlcInstallProgress_progress_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcInstallProgress__SWIG_0")]
		public static extern IntPtr new_DlcInstallProgress__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcInstallProgress__SWIG_1")]
		public static extern IntPtr new_DlcInstallProgress__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_DlcInstallProgress")]
		public static extern void delete_DlcInstallProgress(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallFinished_dlc_id_set")]
		public static extern void DlcInstallFinished_dlc_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallFinished_dlc_id_get")]
		public static extern IntPtr DlcInstallFinished_dlc_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallFinished_result_set")]
		public static extern void DlcInstallFinished_result_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallFinished_result_get")]
		public static extern int DlcInstallFinished_result_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcInstallFinished__SWIG_0")]
		public static extern IntPtr new_DlcInstallFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcInstallFinished__SWIG_1")]
		public static extern IntPtr new_DlcInstallFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_DlcInstallFinished")]
		public static extern void delete_DlcInstallFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcUninstallFinished_dlc_id_set")]
		public static extern void DlcUninstallFinished_dlc_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcUninstallFinished_dlc_id_get")]
		public static extern IntPtr DlcUninstallFinished_dlc_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcUninstallFinished_result_set")]
		public static extern void DlcUninstallFinished_result_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcUninstallFinished_result_get")]
		public static extern int DlcUninstallFinished_result_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcUninstallFinished__SWIG_0")]
		public static extern IntPtr new_DlcUninstallFinished__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcUninstallFinished__SWIG_1")]
		public static extern IntPtr new_DlcUninstallFinished__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_DlcUninstallFinished")]
		public static extern void delete_DlcUninstallFinished(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CheckAllDlcsStateReadyResult__SWIG_0")]
		public static extern IntPtr new_CheckAllDlcsStateReadyResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CheckAllDlcsStateReadyResult__SWIG_1")]
		public static extern IntPtr new_CheckAllDlcsStateReadyResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CheckAllDlcsStateReadyResult")]
		public static extern void delete_CheckAllDlcsStateReadyResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_QueryIsOwnedDlcsResult_dlc_owned_list_set")]
		public static extern void QueryIsOwnedDlcsResult_dlc_owned_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_QueryIsOwnedDlcsResult_dlc_owned_list_get")]
		public static extern IntPtr QueryIsOwnedDlcsResult_dlc_owned_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_QueryIsOwnedDlcsResult__SWIG_0")]
		public static extern IntPtr new_QueryIsOwnedDlcsResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_QueryIsOwnedDlcsResult__SWIG_1")]
		public static extern IntPtr new_QueryIsOwnedDlcsResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_QueryIsOwnedDlcsResult")]
		public static extern void delete_QueryIsOwnedDlcsResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcOwnershipChanged__SWIG_0")]
		public static extern IntPtr new_DlcOwnershipChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcOwnershipChanged_dlc_id_set")]
		public static extern void DlcOwnershipChanged_dlc_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcOwnershipChanged_dlc_id_get")]
		public static extern IntPtr DlcOwnershipChanged_dlc_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcOwnershipChanged_is_active_set")]
		public static extern void DlcOwnershipChanged_is_active_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcOwnershipChanged_is_active_get")]
		public static extern bool DlcOwnershipChanged_is_active_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcOwnershipChanged__SWIG_1")]
		public static extern IntPtr new_DlcOwnershipChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_DlcOwnershipChanged")]
		public static extern void delete_DlcOwnershipChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcRefundChanged__SWIG_0")]
		public static extern IntPtr new_DlcRefundChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcRefundChanged_dlc_id_set")]
		public static extern void DlcRefundChanged_dlc_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcRefundChanged_dlc_id_get")]
		public static extern IntPtr DlcRefundChanged_dlc_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcRefundChanged_refund_state_set")]
		public static extern void DlcRefundChanged_refund_state_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcRefundChanged_refund_state_get")]
		public static extern int DlcRefundChanged_refund_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_DlcRefundChanged__SWIG_1")]
		public static extern IntPtr new_DlcRefundChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_DlcRefundChanged")]
		public static extern void delete_DlcRefundChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailWindowLayout__SWIG_0")]
		public static extern IntPtr new_RailWindowLayout__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailWindowLayout_x_margin_set")]
		public static extern void RailWindowLayout_x_margin_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailWindowLayout_x_margin_get")]
		public static extern uint RailWindowLayout_x_margin_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailWindowLayout_y_margin_set")]
		public static extern void RailWindowLayout_y_margin_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailWindowLayout_y_margin_get")]
		public static extern uint RailWindowLayout_y_margin_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailWindowLayout_position_type_set")]
		public static extern void RailWindowLayout_position_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailWindowLayout_position_type_get")]
		public static extern int RailWindowLayout_position_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailWindowLayout__SWIG_1")]
		public static extern IntPtr new_RailWindowLayout__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailWindowLayout")]
		public static extern void delete_RailWindowLayout(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailStoreOptions__SWIG_0")]
		public static extern IntPtr new_RailStoreOptions__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStoreOptions_store_type_set")]
		public static extern void RailStoreOptions_store_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStoreOptions_store_type_get")]
		public static extern int RailStoreOptions_store_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStoreOptions_window_margin_top_set")]
		public static extern void RailStoreOptions_window_margin_top_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStoreOptions_window_margin_top_get")]
		public static extern int RailStoreOptions_window_margin_top_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStoreOptions_window_margin_left_set")]
		public static extern void RailStoreOptions_window_margin_left_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStoreOptions_window_margin_left_get")]
		public static extern int RailStoreOptions_window_margin_left_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailStoreOptions__SWIG_1")]
		public static extern IntPtr new_RailStoreOptions__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailStoreOptions")]
		public static extern void delete_RailStoreOptions(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ShowFloatingWindowResult__SWIG_0")]
		public static extern IntPtr new_ShowFloatingWindowResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_ShowFloatingWindowResult_is_show_set")]
		public static extern void ShowFloatingWindowResult_is_show_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_ShowFloatingWindowResult_is_show_get")]
		public static extern bool ShowFloatingWindowResult_is_show_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_ShowFloatingWindowResult_window_type_set")]
		public static extern void ShowFloatingWindowResult_window_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_ShowFloatingWindowResult_window_type_get")]
		public static extern int ShowFloatingWindowResult_window_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ShowFloatingWindowResult__SWIG_1")]
		public static extern IntPtr new_ShowFloatingWindowResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_ShowFloatingWindowResult")]
		public static extern void delete_ShowFloatingWindowResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ShowNotifyWindow__SWIG_0")]
		public static extern IntPtr new_ShowNotifyWindow__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_ShowNotifyWindow_window_type_set")]
		public static extern void ShowNotifyWindow_window_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_ShowNotifyWindow_window_type_get")]
		public static extern int ShowNotifyWindow_window_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_ShowNotifyWindow_json_content_set")]
		public static extern void ShowNotifyWindow_json_content_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_ShowNotifyWindow_json_content_get")]
		public static extern IntPtr ShowNotifyWindow_json_content_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ShowNotifyWindow__SWIG_1")]
		public static extern IntPtr new_ShowNotifyWindow__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_ShowNotifyWindow")]
		public static extern void delete_ShowNotifyWindow(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerInfo__SWIG_0")]
		public static extern IntPtr new_GameServerInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_Reset")]
		public static extern void GameServerInfo_Reset(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_game_server_rail_id_set")]
		public static extern void GameServerInfo_game_server_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_game_server_rail_id_get")]
		public static extern IntPtr GameServerInfo_game_server_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_owner_rail_id_set")]
		public static extern void GameServerInfo_owner_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_owner_rail_id_get")]
		public static extern IntPtr GameServerInfo_owner_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_is_dedicated_set")]
		public static extern void GameServerInfo_is_dedicated_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_is_dedicated_get")]
		public static extern bool GameServerInfo_is_dedicated_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_game_server_name_set")]
		public static extern void GameServerInfo_game_server_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_game_server_name_get")]
		public static extern IntPtr GameServerInfo_game_server_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_game_server_map_set")]
		public static extern void GameServerInfo_game_server_map_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_game_server_map_get")]
		public static extern IntPtr GameServerInfo_game_server_map_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_has_password_set")]
		public static extern void GameServerInfo_has_password_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_has_password_get")]
		public static extern bool GameServerInfo_has_password_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_is_friend_only_set")]
		public static extern void GameServerInfo_is_friend_only_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_is_friend_only_get")]
		public static extern bool GameServerInfo_is_friend_only_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_max_players_set")]
		public static extern void GameServerInfo_max_players_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_max_players_get")]
		public static extern uint GameServerInfo_max_players_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_current_players_set")]
		public static extern void GameServerInfo_current_players_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_current_players_get")]
		public static extern uint GameServerInfo_current_players_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_bot_players_set")]
		public static extern void GameServerInfo_bot_players_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_bot_players_get")]
		public static extern uint GameServerInfo_bot_players_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_host_set")]
		public static extern void GameServerInfo_server_host_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_host_get")]
		public static extern IntPtr GameServerInfo_server_host_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_fullname_set")]
		public static extern void GameServerInfo_server_fullname_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_fullname_get")]
		public static extern IntPtr GameServerInfo_server_fullname_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_description_set")]
		public static extern void GameServerInfo_server_description_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_description_get")]
		public static extern IntPtr GameServerInfo_server_description_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_tags_set")]
		public static extern void GameServerInfo_server_tags_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_tags_get")]
		public static extern IntPtr GameServerInfo_server_tags_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_version_set")]
		public static extern void GameServerInfo_server_version_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_version_get")]
		public static extern IntPtr GameServerInfo_server_version_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_spectator_host_set")]
		public static extern void GameServerInfo_spectator_host_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_spectator_host_get")]
		public static extern IntPtr GameServerInfo_spectator_host_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_info_set")]
		public static extern void GameServerInfo_server_info_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_info_get")]
		public static extern IntPtr GameServerInfo_server_info_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_mods_set")]
		public static extern void GameServerInfo_server_mods_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_mods_get")]
		public static extern IntPtr GameServerInfo_server_mods_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_kvs_set")]
		public static extern void GameServerInfo_server_kvs_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerInfo_server_kvs_get")]
		public static extern IntPtr GameServerInfo_server_kvs_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerInfo__SWIG_1")]
		public static extern IntPtr new_GameServerInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GameServerInfo")]
		public static extern void delete_GameServerInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateGameServerOptions__SWIG_0")]
		public static extern IntPtr new_CreateGameServerOptions__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateGameServerOptions_enable_team_voice_set")]
		public static extern void CreateGameServerOptions_enable_team_voice_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateGameServerOptions_enable_team_voice_get")]
		public static extern bool CreateGameServerOptions_enable_team_voice_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateGameServerOptions_has_password_set")]
		public static extern void CreateGameServerOptions_has_password_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateGameServerOptions_has_password_get")]
		public static extern bool CreateGameServerOptions_has_password_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateGameServerOptions__SWIG_1")]
		public static extern IntPtr new_CreateGameServerOptions__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CreateGameServerOptions")]
		public static extern void delete_CreateGameServerOptions(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerListSorter__SWIG_0")]
		public static extern IntPtr new_GameServerListSorter__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListSorter_sorter_key_type_set")]
		public static extern void GameServerListSorter_sorter_key_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListSorter_sorter_key_type_get")]
		public static extern int GameServerListSorter_sorter_key_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListSorter_sort_key_set")]
		public static extern void GameServerListSorter_sort_key_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListSorter_sort_key_get")]
		public static extern IntPtr GameServerListSorter_sort_key_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListSorter_sort_value_type_set")]
		public static extern void GameServerListSorter_sort_value_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListSorter_sort_value_type_get")]
		public static extern int GameServerListSorter_sort_value_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListSorter_sort_type_set")]
		public static extern void GameServerListSorter_sort_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListSorter_sort_type_get")]
		public static extern int GameServerListSorter_sort_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerListSorter__SWIG_1")]
		public static extern IntPtr new_GameServerListSorter__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GameServerListSorter")]
		public static extern void delete_GameServerListSorter(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerListFilterKey__SWIG_0")]
		public static extern IntPtr new_GameServerListFilterKey__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilterKey_key_name_set")]
		public static extern void GameServerListFilterKey_key_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilterKey_key_name_get")]
		public static extern IntPtr GameServerListFilterKey_key_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilterKey_value_type_set")]
		public static extern void GameServerListFilterKey_value_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilterKey_value_type_get")]
		public static extern int GameServerListFilterKey_value_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilterKey_comparison_type_set")]
		public static extern void GameServerListFilterKey_comparison_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilterKey_comparison_type_get")]
		public static extern int GameServerListFilterKey_comparison_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilterKey_filter_value_set")]
		public static extern void GameServerListFilterKey_filter_value_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilterKey_filter_value_get")]
		public static extern IntPtr GameServerListFilterKey_filter_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerListFilterKey__SWIG_1")]
		public static extern IntPtr new_GameServerListFilterKey__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GameServerListFilterKey")]
		public static extern void delete_GameServerListFilterKey(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerListFilter__SWIG_0")]
		public static extern IntPtr new_GameServerListFilter__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filters_set")]
		public static extern void GameServerListFilter_filters_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filters_get")]
		public static extern IntPtr GameServerListFilter_filters_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_owner_id_set")]
		public static extern void GameServerListFilter_owner_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_owner_id_get")]
		public static extern IntPtr GameServerListFilter_owner_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filter_dedicated_server_set")]
		public static extern void GameServerListFilter_filter_dedicated_server_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filter_dedicated_server_get")]
		public static extern int GameServerListFilter_filter_dedicated_server_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filter_game_server_name_set")]
		public static extern void GameServerListFilter_filter_game_server_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filter_game_server_name_get")]
		public static extern IntPtr GameServerListFilter_filter_game_server_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filter_game_server_map_set")]
		public static extern void GameServerListFilter_filter_game_server_map_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filter_game_server_map_get")]
		public static extern IntPtr GameServerListFilter_filter_game_server_map_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filter_game_server_host_set")]
		public static extern void GameServerListFilter_filter_game_server_host_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filter_game_server_host_get")]
		public static extern IntPtr GameServerListFilter_filter_game_server_host_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filter_password_set")]
		public static extern void GameServerListFilter_filter_password_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filter_password_get")]
		public static extern int GameServerListFilter_filter_password_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filter_friends_created_set")]
		public static extern void GameServerListFilter_filter_friends_created_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_filter_friends_created_get")]
		public static extern int GameServerListFilter_filter_friends_created_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_tags_contained_set")]
		public static extern void GameServerListFilter_tags_contained_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_tags_contained_get")]
		public static extern IntPtr GameServerListFilter_tags_contained_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_tags_not_contained_set")]
		public static extern void GameServerListFilter_tags_not_contained_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerListFilter_tags_not_contained_get")]
		public static extern IntPtr GameServerListFilter_tags_not_contained_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerListFilter__SWIG_1")]
		public static extern IntPtr new_GameServerListFilter__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GameServerListFilter")]
		public static extern void delete_GameServerListFilter(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerPlayerInfo__SWIG_0")]
		public static extern IntPtr new_GameServerPlayerInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerPlayerInfo_member_id_set")]
		public static extern void GameServerPlayerInfo_member_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerPlayerInfo_member_id_get")]
		public static extern IntPtr GameServerPlayerInfo_member_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerPlayerInfo_member_nickname_set")]
		public static extern void GameServerPlayerInfo_member_nickname_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerPlayerInfo_member_nickname_get")]
		public static extern IntPtr GameServerPlayerInfo_member_nickname_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerPlayerInfo_member_score_set")]
		public static extern void GameServerPlayerInfo_member_score_set(IntPtr jarg1, long jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerPlayerInfo_member_score_get")]
		public static extern long GameServerPlayerInfo_member_score_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerPlayerInfo__SWIG_1")]
		public static extern IntPtr new_GameServerPlayerInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GameServerPlayerInfo")]
		public static extern void delete_GameServerPlayerInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncAcquireGameServerSessionTicketResponse__SWIG_0")]
		public static extern IntPtr new_AsyncAcquireGameServerSessionTicketResponse__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncAcquireGameServerSessionTicketResponse_session_ticket_set")]
		public static extern void AsyncAcquireGameServerSessionTicketResponse_session_ticket_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncAcquireGameServerSessionTicketResponse_session_ticket_get")]
		public static extern IntPtr AsyncAcquireGameServerSessionTicketResponse_session_ticket_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncAcquireGameServerSessionTicketResponse__SWIG_1")]
		public static extern IntPtr new_AsyncAcquireGameServerSessionTicketResponse__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncAcquireGameServerSessionTicketResponse")]
		public static extern void delete_AsyncAcquireGameServerSessionTicketResponse(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerStartSessionWithPlayerResponse__SWIG_0")]
		public static extern IntPtr new_GameServerStartSessionWithPlayerResponse__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerStartSessionWithPlayerResponse_remote_rail_id_set")]
		public static extern void GameServerStartSessionWithPlayerResponse_remote_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerStartSessionWithPlayerResponse_remote_rail_id_get")]
		public static extern IntPtr GameServerStartSessionWithPlayerResponse_remote_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerStartSessionWithPlayerResponse__SWIG_1")]
		public static extern IntPtr new_GameServerStartSessionWithPlayerResponse__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GameServerStartSessionWithPlayerResponse")]
		public static extern void delete_GameServerStartSessionWithPlayerResponse(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateGameServerResult__SWIG_0")]
		public static extern IntPtr new_CreateGameServerResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateGameServerResult_game_server_id_set")]
		public static extern void CreateGameServerResult_game_server_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateGameServerResult_game_server_id_get")]
		public static extern IntPtr CreateGameServerResult_game_server_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateGameServerResult__SWIG_1")]
		public static extern IntPtr new_CreateGameServerResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CreateGameServerResult")]
		public static extern void delete_CreateGameServerResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetGameServerMetadataResult__SWIG_0")]
		public static extern IntPtr new_SetGameServerMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_SetGameServerMetadataResult_game_server_id_set")]
		public static extern void SetGameServerMetadataResult_game_server_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetGameServerMetadataResult_game_server_id_get")]
		public static extern IntPtr SetGameServerMetadataResult_game_server_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetGameServerMetadataResult__SWIG_1")]
		public static extern IntPtr new_SetGameServerMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_SetGameServerMetadataResult")]
		public static extern void delete_SetGameServerMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetGameServerMetadataResult__SWIG_0")]
		public static extern IntPtr new_GetGameServerMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerMetadataResult_game_server_id_set")]
		public static extern void GetGameServerMetadataResult_game_server_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerMetadataResult_game_server_id_get")]
		public static extern IntPtr GetGameServerMetadataResult_game_server_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerMetadataResult_key_value_set")]
		public static extern void GetGameServerMetadataResult_key_value_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerMetadataResult_key_value_get")]
		public static extern IntPtr GetGameServerMetadataResult_key_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetGameServerMetadataResult__SWIG_1")]
		public static extern IntPtr new_GetGameServerMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GetGameServerMetadataResult")]
		public static extern void delete_GetGameServerMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerRegisterToServerListResult__SWIG_0")]
		public static extern IntPtr new_GameServerRegisterToServerListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GameServerRegisterToServerListResult__SWIG_1")]
		public static extern IntPtr new_GameServerRegisterToServerListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GameServerRegisterToServerListResult")]
		public static extern void delete_GameServerRegisterToServerListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetGameServerPlayerListResult__SWIG_0")]
		public static extern IntPtr new_GetGameServerPlayerListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerPlayerListResult_game_server_id_set")]
		public static extern void GetGameServerPlayerListResult_game_server_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerPlayerListResult_game_server_id_get")]
		public static extern IntPtr GetGameServerPlayerListResult_game_server_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerPlayerListResult_server_player_info_set")]
		public static extern void GetGameServerPlayerListResult_server_player_info_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerPlayerListResult_server_player_info_get")]
		public static extern IntPtr GetGameServerPlayerListResult_server_player_info_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetGameServerPlayerListResult__SWIG_1")]
		public static extern IntPtr new_GetGameServerPlayerListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GetGameServerPlayerListResult")]
		public static extern void delete_GetGameServerPlayerListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetGameServerListResult__SWIG_0")]
		public static extern IntPtr new_GetGameServerListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerListResult_start_index_set")]
		public static extern void GetGameServerListResult_start_index_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerListResult_start_index_get")]
		public static extern uint GetGameServerListResult_start_index_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerListResult_end_index_set")]
		public static extern void GetGameServerListResult_end_index_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerListResult_end_index_get")]
		public static extern uint GetGameServerListResult_end_index_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerListResult_total_num_set")]
		public static extern void GetGameServerListResult_total_num_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerListResult_total_num_get")]
		public static extern uint GetGameServerListResult_total_num_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerListResult_server_info_set")]
		public static extern void GetGameServerListResult_server_info_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerListResult_server_info_get")]
		public static extern IntPtr GetGameServerListResult_server_info_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetGameServerListResult__SWIG_1")]
		public static extern IntPtr new_GetGameServerListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GetGameServerListResult")]
		public static extern void delete_GetGameServerListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncGetFavoriteGameServersResult__SWIG_0")]
		public static extern IntPtr new_AsyncGetFavoriteGameServersResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncGetFavoriteGameServersResult_server_id_array_set")]
		public static extern void AsyncGetFavoriteGameServersResult_server_id_array_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncGetFavoriteGameServersResult_server_id_array_get")]
		public static extern IntPtr AsyncGetFavoriteGameServersResult_server_id_array_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncGetFavoriteGameServersResult__SWIG_1")]
		public static extern IntPtr new_AsyncGetFavoriteGameServersResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncGetFavoriteGameServersResult")]
		public static extern void delete_AsyncGetFavoriteGameServersResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncAddFavoriteGameServerResult__SWIG_0")]
		public static extern IntPtr new_AsyncAddFavoriteGameServerResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncAddFavoriteGameServerResult_server_id_set")]
		public static extern void AsyncAddFavoriteGameServerResult_server_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncAddFavoriteGameServerResult_server_id_get")]
		public static extern IntPtr AsyncAddFavoriteGameServerResult_server_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncAddFavoriteGameServerResult__SWIG_1")]
		public static extern IntPtr new_AsyncAddFavoriteGameServerResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncAddFavoriteGameServerResult")]
		public static extern void delete_AsyncAddFavoriteGameServerResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncRemoveFavoriteGameServerResult__SWIG_0")]
		public static extern IntPtr new_AsyncRemoveFavoriteGameServerResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRemoveFavoriteGameServerResult_server_id_set")]
		public static extern void AsyncRemoveFavoriteGameServerResult_server_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRemoveFavoriteGameServerResult_server_id_get")]
		public static extern IntPtr AsyncRemoveFavoriteGameServerResult_server_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncRemoveFavoriteGameServerResult__SWIG_1")]
		public static extern IntPtr new_AsyncRemoveFavoriteGameServerResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncRemoveFavoriteGameServerResult")]
		public static extern void delete_AsyncRemoveFavoriteGameServerResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PlayerPersonalInfo__SWIG_0")]
		public static extern IntPtr new_PlayerPersonalInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerPersonalInfo_rail_id_set")]
		public static extern void PlayerPersonalInfo_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerPersonalInfo_rail_id_get")]
		public static extern IntPtr PlayerPersonalInfo_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerPersonalInfo_error_code_set")]
		public static extern void PlayerPersonalInfo_error_code_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerPersonalInfo_error_code_get")]
		public static extern int PlayerPersonalInfo_error_code_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerPersonalInfo_rail_level_set")]
		public static extern void PlayerPersonalInfo_rail_level_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerPersonalInfo_rail_level_get")]
		public static extern uint PlayerPersonalInfo_rail_level_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerPersonalInfo_rail_name_set")]
		public static extern void PlayerPersonalInfo_rail_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerPersonalInfo_rail_name_get")]
		public static extern IntPtr PlayerPersonalInfo_rail_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerPersonalInfo_avatar_url_set")]
		public static extern void PlayerPersonalInfo_avatar_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerPersonalInfo_avatar_url_get")]
		public static extern IntPtr PlayerPersonalInfo_avatar_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerPersonalInfo_email_address_set")]
		public static extern void PlayerPersonalInfo_email_address_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerPersonalInfo_email_address_get")]
		public static extern IntPtr PlayerPersonalInfo_email_address_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PlayerPersonalInfo__SWIG_1")]
		public static extern IntPtr new_PlayerPersonalInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_PlayerPersonalInfo")]
		public static extern void delete_PlayerPersonalInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGetAuthenticateURLOptions__SWIG_0")]
		public static extern IntPtr new_RailGetAuthenticateURLOptions__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetAuthenticateURLOptions_url_set")]
		public static extern void RailGetAuthenticateURLOptions_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetAuthenticateURLOptions_url_get")]
		public static extern IntPtr RailGetAuthenticateURLOptions_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetAuthenticateURLOptions_oauth2_state_set")]
		public static extern void RailGetAuthenticateURLOptions_oauth2_state_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetAuthenticateURLOptions_oauth2_state_get")]
		public static extern IntPtr RailGetAuthenticateURLOptions_oauth2_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetAuthenticateURLOptions_client_id_set")]
		public static extern void RailGetAuthenticateURLOptions_client_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetAuthenticateURLOptions_client_id_get")]
		public static extern ulong RailGetAuthenticateURLOptions_client_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGetAuthenticateURLOptions__SWIG_1")]
		public static extern IntPtr new_RailGetAuthenticateURLOptions__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailGetAuthenticateURLOptions")]
		public static extern void delete_RailGetAuthenticateURLOptions(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AcquireSessionTicketResponse__SWIG_0")]
		public static extern IntPtr new_AcquireSessionTicketResponse__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_AcquireSessionTicketResponse_session_ticket_set")]
		public static extern void AcquireSessionTicketResponse_session_ticket_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AcquireSessionTicketResponse_session_ticket_get")]
		public static extern IntPtr AcquireSessionTicketResponse_session_ticket_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AcquireSessionTicketResponse__SWIG_1")]
		public static extern IntPtr new_AcquireSessionTicketResponse__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AcquireSessionTicketResponse")]
		public static extern void delete_AcquireSessionTicketResponse(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_StartSessionWithPlayerResponse__SWIG_0")]
		public static extern IntPtr new_StartSessionWithPlayerResponse__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_StartSessionWithPlayerResponse_remote_rail_id_set")]
		public static extern void StartSessionWithPlayerResponse_remote_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_StartSessionWithPlayerResponse_remote_rail_id_get")]
		public static extern IntPtr StartSessionWithPlayerResponse_remote_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_StartSessionWithPlayerResponse__SWIG_1")]
		public static extern IntPtr new_StartSessionWithPlayerResponse__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_StartSessionWithPlayerResponse")]
		public static extern void delete_StartSessionWithPlayerResponse(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PlayerGetGamePurchaseKeyResult__SWIG_0")]
		public static extern IntPtr new_PlayerGetGamePurchaseKeyResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerGetGamePurchaseKeyResult_purchase_key_set")]
		public static extern void PlayerGetGamePurchaseKeyResult_purchase_key_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerGetGamePurchaseKeyResult_purchase_key_get")]
		public static extern IntPtr PlayerGetGamePurchaseKeyResult_purchase_key_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PlayerGetGamePurchaseKeyResult__SWIG_1")]
		public static extern IntPtr new_PlayerGetGamePurchaseKeyResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_PlayerGetGamePurchaseKeyResult")]
		public static extern void delete_PlayerGetGamePurchaseKeyResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_QueryPlayerBannedStatus__SWIG_0")]
		public static extern IntPtr new_QueryPlayerBannedStatus__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_QueryPlayerBannedStatus_status_set")]
		public static extern void QueryPlayerBannedStatus_status_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_QueryPlayerBannedStatus_status_get")]
		public static extern int QueryPlayerBannedStatus_status_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_QueryPlayerBannedStatus__SWIG_1")]
		public static extern IntPtr new_QueryPlayerBannedStatus__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_QueryPlayerBannedStatus")]
		public static extern void delete_QueryPlayerBannedStatus(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetAuthenticateURLResult__SWIG_0")]
		public static extern IntPtr new_GetAuthenticateURLResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GetAuthenticateURLResult_source_url_set")]
		public static extern void GetAuthenticateURLResult_source_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetAuthenticateURLResult_source_url_get")]
		public static extern IntPtr GetAuthenticateURLResult_source_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetAuthenticateURLResult_authenticate_url_set")]
		public static extern void GetAuthenticateURLResult_authenticate_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetAuthenticateURLResult_authenticate_url_get")]
		public static extern IntPtr GetAuthenticateURLResult_authenticate_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetAuthenticateURLResult_ticket_expire_time_set")]
		public static extern void GetAuthenticateURLResult_ticket_expire_time_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetAuthenticateURLResult_ticket_expire_time_get")]
		public static extern uint GetAuthenticateURLResult_ticket_expire_time_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetAuthenticateURLResult__SWIG_1")]
		public static extern IntPtr new_GetAuthenticateURLResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GetAuthenticateURLResult")]
		public static extern void delete_GetAuthenticateURLResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailAntiAddictionGameOnlineTimeChanged__SWIG_0")]
		public static extern IntPtr new_RailAntiAddictionGameOnlineTimeChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAntiAddictionGameOnlineTimeChanged_game_online_time_count_minutes_set")]
		public static extern void RailAntiAddictionGameOnlineTimeChanged_game_online_time_count_minutes_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAntiAddictionGameOnlineTimeChanged_game_online_time_count_minutes_get")]
		public static extern uint RailAntiAddictionGameOnlineTimeChanged_game_online_time_count_minutes_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailAntiAddictionGameOnlineTimeChanged__SWIG_1")]
		public static extern IntPtr new_RailAntiAddictionGameOnlineTimeChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailAntiAddictionGameOnlineTimeChanged")]
		public static extern void delete_RailAntiAddictionGameOnlineTimeChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGetEncryptedGameTicketResult__SWIG_0")]
		public static extern IntPtr new_RailGetEncryptedGameTicketResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetEncryptedGameTicketResult_encrypted_game_ticket_set")]
		public static extern void RailGetEncryptedGameTicketResult_encrypted_game_ticket_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetEncryptedGameTicketResult_encrypted_game_ticket_get")]
		public static extern IntPtr RailGetEncryptedGameTicketResult_encrypted_game_ticket_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGetEncryptedGameTicketResult__SWIG_1")]
		public static extern IntPtr new_RailGetEncryptedGameTicketResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailGetEncryptedGameTicketResult")]
		public static extern void delete_RailGetEncryptedGameTicketResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGetPlayerMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailGetPlayerMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetPlayerMetadataResult_key_values_set")]
		public static extern void RailGetPlayerMetadataResult_key_values_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetPlayerMetadataResult_key_values_get")]
		public static extern IntPtr RailGetPlayerMetadataResult_key_values_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGetPlayerMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailGetPlayerMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailGetPlayerMetadataResult")]
		public static extern void delete_RailGetPlayerMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_kRailMaxQueryPlayedWithFriendsTimeLimit_get")]
		public static extern uint kRailMaxQueryPlayedWithFriendsTimeLimit_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailKeyValueResult__SWIG_0")]
		public static extern IntPtr new_RailKeyValueResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailKeyValueResult_error_code_set")]
		public static extern void RailKeyValueResult_error_code_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailKeyValueResult_error_code_get")]
		public static extern int RailKeyValueResult_error_code_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailKeyValueResult_key_set")]
		public static extern void RailKeyValueResult_key_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailKeyValueResult_key_get")]
		public static extern IntPtr RailKeyValueResult_key_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailKeyValueResult_value_set")]
		public static extern void RailKeyValueResult_value_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailKeyValueResult_value_get")]
		public static extern IntPtr RailKeyValueResult_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailKeyValueResult__SWIG_1")]
		public static extern IntPtr new_RailKeyValueResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailKeyValueResult")]
		public static extern void delete_RailKeyValueResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUserPlayedWith__SWIG_0")]
		public static extern IntPtr new_RailUserPlayedWith__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserPlayedWith_rail_id_set")]
		public static extern void RailUserPlayedWith_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserPlayedWith_rail_id_get")]
		public static extern IntPtr RailUserPlayedWith_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserPlayedWith_user_rich_content_set")]
		public static extern void RailUserPlayedWith_user_rich_content_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUserPlayedWith_user_rich_content_get")]
		public static extern IntPtr RailUserPlayedWith_user_rich_content_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailUserPlayedWith__SWIG_1")]
		public static extern IntPtr new_RailUserPlayedWith__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailUserPlayedWith")]
		public static extern void delete_RailUserPlayedWith(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendPlayedGameInfo__SWIG_0")]
		public static extern IntPtr new_RailFriendPlayedGameInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_in_game_server_set")]
		public static extern void RailFriendPlayedGameInfo_in_game_server_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_in_game_server_get")]
		public static extern bool RailFriendPlayedGameInfo_in_game_server_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_in_room_set")]
		public static extern void RailFriendPlayedGameInfo_in_room_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_in_room_get")]
		public static extern bool RailFriendPlayedGameInfo_in_room_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_game_server_id_list_set")]
		public static extern void RailFriendPlayedGameInfo_game_server_id_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_game_server_id_list_get")]
		public static extern IntPtr RailFriendPlayedGameInfo_game_server_id_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_room_id_list_set")]
		public static extern void RailFriendPlayedGameInfo_room_id_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_room_id_list_get")]
		public static extern IntPtr RailFriendPlayedGameInfo_room_id_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_friend_id_set")]
		public static extern void RailFriendPlayedGameInfo_friend_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_friend_id_get")]
		public static extern IntPtr RailFriendPlayedGameInfo_friend_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_game_id_set")]
		public static extern void RailFriendPlayedGameInfo_game_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_game_id_get")]
		public static extern IntPtr RailFriendPlayedGameInfo_game_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_friend_played_game_play_state_set")]
		public static extern void RailFriendPlayedGameInfo_friend_played_game_play_state_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendPlayedGameInfo_friend_played_game_play_state_get")]
		public static extern int RailFriendPlayedGameInfo_friend_played_game_play_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendPlayedGameInfo__SWIG_1")]
		public static extern IntPtr new_RailFriendPlayedGameInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendPlayedGameInfo")]
		public static extern void delete_RailFriendPlayedGameInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPlayedWithFriendsTimeItem__SWIG_0")]
		public static extern IntPtr new_RailPlayedWithFriendsTimeItem__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayedWithFriendsTimeItem_play_time_set")]
		public static extern void RailPlayedWithFriendsTimeItem_play_time_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayedWithFriendsTimeItem_play_time_get")]
		public static extern uint RailPlayedWithFriendsTimeItem_play_time_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayedWithFriendsTimeItem_rail_id_set")]
		public static extern void RailPlayedWithFriendsTimeItem_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayedWithFriendsTimeItem_rail_id_get")]
		public static extern IntPtr RailPlayedWithFriendsTimeItem_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPlayedWithFriendsTimeItem__SWIG_1")]
		public static extern IntPtr new_RailPlayedWithFriendsTimeItem__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailPlayedWithFriendsTimeItem")]
		public static extern void delete_RailPlayedWithFriendsTimeItem(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPlayedWithFriendsGameItem__SWIG_0")]
		public static extern IntPtr new_RailPlayedWithFriendsGameItem__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayedWithFriendsGameItem_game_ids_set")]
		public static extern void RailPlayedWithFriendsGameItem_game_ids_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayedWithFriendsGameItem_game_ids_get")]
		public static extern IntPtr RailPlayedWithFriendsGameItem_game_ids_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayedWithFriendsGameItem_rail_id_set")]
		public static extern void RailPlayedWithFriendsGameItem_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayedWithFriendsGameItem_rail_id_get")]
		public static extern IntPtr RailPlayedWithFriendsGameItem_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPlayedWithFriendsGameItem__SWIG_1")]
		public static extern IntPtr new_RailPlayedWithFriendsGameItem__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailPlayedWithFriendsGameItem")]
		public static extern void delete_RailPlayedWithFriendsGameItem(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsAddFriendRequest__SWIG_0")]
		public static extern IntPtr new_RailFriendsAddFriendRequest__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsAddFriendRequest_target_rail_id_set")]
		public static extern void RailFriendsAddFriendRequest_target_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsAddFriendRequest_target_rail_id_get")]
		public static extern IntPtr RailFriendsAddFriendRequest_target_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsAddFriendRequest__SWIG_1")]
		public static extern IntPtr new_RailFriendsAddFriendRequest__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsAddFriendRequest")]
		public static extern void delete_RailFriendsAddFriendRequest(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendOnLineState__SWIG_0")]
		public static extern IntPtr new_RailFriendOnLineState__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendOnLineState_friend_rail_id_set")]
		public static extern void RailFriendOnLineState_friend_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendOnLineState_friend_rail_id_get")]
		public static extern IntPtr RailFriendOnLineState_friend_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendOnLineState_friend_online_state_set")]
		public static extern void RailFriendOnLineState_friend_online_state_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendOnLineState_friend_online_state_get")]
		public static extern int RailFriendOnLineState_friend_online_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendOnLineState_game_define_game_playing_state_set")]
		public static extern void RailFriendOnLineState_game_define_game_playing_state_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendOnLineState_game_define_game_playing_state_get")]
		public static extern uint RailFriendOnLineState_game_define_game_playing_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendOnLineState__SWIG_1")]
		public static extern IntPtr new_RailFriendOnLineState__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendOnLineState")]
		public static extern void delete_RailFriendOnLineState(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendInfo__SWIG_0")]
		public static extern IntPtr new_RailFriendInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendInfo_friend_rail_id_set")]
		public static extern void RailFriendInfo_friend_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendInfo_friend_rail_id_get")]
		public static extern IntPtr RailFriendInfo_friend_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendInfo_friend_type_set")]
		public static extern void RailFriendInfo_friend_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendInfo_friend_type_get")]
		public static extern int RailFriendInfo_friend_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendInfo_online_state_set")]
		public static extern void RailFriendInfo_online_state_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendInfo_online_state_get")]
		public static extern IntPtr RailFriendInfo_online_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendInfo__SWIG_1")]
		public static extern IntPtr new_RailFriendInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendInfo")]
		public static extern void delete_RailFriendInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendMetadata__SWIG_0")]
		public static extern IntPtr new_RailFriendMetadata__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendMetadata_friend_rail_id_set")]
		public static extern void RailFriendMetadata_friend_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendMetadata_friend_rail_id_get")]
		public static extern IntPtr RailFriendMetadata_friend_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendMetadata_metadatas_set")]
		public static extern void RailFriendMetadata_metadatas_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendMetadata_metadatas_get")]
		public static extern IntPtr RailFriendMetadata_metadatas_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendMetadata__SWIG_1")]
		public static extern IntPtr new_RailFriendMetadata__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendMetadata")]
		public static extern void delete_RailFriendMetadata(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsSetMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailFriendsSetMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsSetMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailFriendsSetMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsSetMetadataResult")]
		public static extern void delete_RailFriendsSetMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsGetMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailFriendsGetMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsGetMetadataResult_friend_id_set")]
		public static extern void RailFriendsGetMetadataResult_friend_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsGetMetadataResult_friend_id_get")]
		public static extern IntPtr RailFriendsGetMetadataResult_friend_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsGetMetadataResult_friend_kvs_set")]
		public static extern void RailFriendsGetMetadataResult_friend_kvs_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsGetMetadataResult_friend_kvs_get")]
		public static extern IntPtr RailFriendsGetMetadataResult_friend_kvs_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsGetMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailFriendsGetMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsGetMetadataResult")]
		public static extern void delete_RailFriendsGetMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsClearMetadataResult__SWIG_0")]
		public static extern IntPtr new_RailFriendsClearMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsClearMetadataResult__SWIG_1")]
		public static extern IntPtr new_RailFriendsClearMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsClearMetadataResult")]
		public static extern void delete_RailFriendsClearMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsGetInviteCommandLine__SWIG_0")]
		public static extern IntPtr new_RailFriendsGetInviteCommandLine__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsGetInviteCommandLine_friend_id_set")]
		public static extern void RailFriendsGetInviteCommandLine_friend_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsGetInviteCommandLine_friend_id_get")]
		public static extern IntPtr RailFriendsGetInviteCommandLine_friend_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsGetInviteCommandLine_invite_command_line_set")]
		public static extern void RailFriendsGetInviteCommandLine_invite_command_line_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsGetInviteCommandLine_invite_command_line_get")]
		public static extern IntPtr RailFriendsGetInviteCommandLine_invite_command_line_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsGetInviteCommandLine__SWIG_1")]
		public static extern IntPtr new_RailFriendsGetInviteCommandLine__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsGetInviteCommandLine")]
		public static extern void delete_RailFriendsGetInviteCommandLine(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsReportPlayedWithUserListResult__SWIG_0")]
		public static extern IntPtr new_RailFriendsReportPlayedWithUserListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsReportPlayedWithUserListResult__SWIG_1")]
		public static extern IntPtr new_RailFriendsReportPlayedWithUserListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsReportPlayedWithUserListResult")]
		public static extern void delete_RailFriendsReportPlayedWithUserListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsListChanged__SWIG_0")]
		public static extern IntPtr new_RailFriendsListChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsListChanged__SWIG_1")]
		public static extern IntPtr new_RailFriendsListChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsListChanged")]
		public static extern void delete_RailFriendsListChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsQueryFriendPlayedGamesResult__SWIG_0")]
		public static extern IntPtr new_RailFriendsQueryFriendPlayedGamesResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsQueryFriendPlayedGamesResult_friend_played_games_info_list_set")]
		public static extern void RailFriendsQueryFriendPlayedGamesResult_friend_played_games_info_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsQueryFriendPlayedGamesResult_friend_played_games_info_list_get")]
		public static extern IntPtr RailFriendsQueryFriendPlayedGamesResult_friend_played_games_info_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsQueryFriendPlayedGamesResult__SWIG_1")]
		public static extern IntPtr new_RailFriendsQueryFriendPlayedGamesResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsQueryFriendPlayedGamesResult")]
		public static extern void delete_RailFriendsQueryFriendPlayedGamesResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsQueryPlayedWithFriendsListResult__SWIG_0")]
		public static extern IntPtr new_RailFriendsQueryPlayedWithFriendsListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsQueryPlayedWithFriendsListResult_played_with_friends_list_set")]
		public static extern void RailFriendsQueryPlayedWithFriendsListResult_played_with_friends_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsQueryPlayedWithFriendsListResult_played_with_friends_list_get")]
		public static extern IntPtr RailFriendsQueryPlayedWithFriendsListResult_played_with_friends_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsQueryPlayedWithFriendsListResult__SWIG_1")]
		public static extern IntPtr new_RailFriendsQueryPlayedWithFriendsListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsQueryPlayedWithFriendsListResult")]
		public static extern void delete_RailFriendsQueryPlayedWithFriendsListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsQueryPlayedWithFriendsTimeResult__SWIG_0")]
		public static extern IntPtr new_RailFriendsQueryPlayedWithFriendsTimeResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsQueryPlayedWithFriendsTimeResult_played_with_friends_time_list_set")]
		public static extern void RailFriendsQueryPlayedWithFriendsTimeResult_played_with_friends_time_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsQueryPlayedWithFriendsTimeResult_played_with_friends_time_list_get")]
		public static extern IntPtr RailFriendsQueryPlayedWithFriendsTimeResult_played_with_friends_time_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsQueryPlayedWithFriendsTimeResult__SWIG_1")]
		public static extern IntPtr new_RailFriendsQueryPlayedWithFriendsTimeResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsQueryPlayedWithFriendsTimeResult")]
		public static extern void delete_RailFriendsQueryPlayedWithFriendsTimeResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsQueryPlayedWithFriendsGamesResult__SWIG_0")]
		public static extern IntPtr new_RailFriendsQueryPlayedWithFriendsGamesResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsQueryPlayedWithFriendsGamesResult_played_with_friends_game_list_set")]
		public static extern void RailFriendsQueryPlayedWithFriendsGamesResult_played_with_friends_game_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsQueryPlayedWithFriendsGamesResult_played_with_friends_game_list_get")]
		public static extern IntPtr RailFriendsQueryPlayedWithFriendsGamesResult_played_with_friends_game_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsQueryPlayedWithFriendsGamesResult__SWIG_1")]
		public static extern IntPtr new_RailFriendsQueryPlayedWithFriendsGamesResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsQueryPlayedWithFriendsGamesResult")]
		public static extern void delete_RailFriendsQueryPlayedWithFriendsGamesResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsAddFriendResult__SWIG_0")]
		public static extern IntPtr new_RailFriendsAddFriendResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsAddFriendResult_target_rail_id_set")]
		public static extern void RailFriendsAddFriendResult_target_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsAddFriendResult_target_rail_id_get")]
		public static extern IntPtr RailFriendsAddFriendResult_target_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsAddFriendResult__SWIG_1")]
		public static extern IntPtr new_RailFriendsAddFriendResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsAddFriendResult")]
		public static extern void delete_RailFriendsAddFriendResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsOnlineStateChanged__SWIG_0")]
		public static extern IntPtr new_RailFriendsOnlineStateChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsOnlineStateChanged_friend_online_state_set")]
		public static extern void RailFriendsOnlineStateChanged_friend_online_state_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsOnlineStateChanged_friend_online_state_get")]
		public static extern IntPtr RailFriendsOnlineStateChanged_friend_online_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsOnlineStateChanged__SWIG_1")]
		public static extern IntPtr new_RailFriendsOnlineStateChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsOnlineStateChanged")]
		public static extern void delete_RailFriendsOnlineStateChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsMetadataChanged__SWIG_0")]
		public static extern IntPtr new_RailFriendsMetadataChanged__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsMetadataChanged_friends_changed_metadata_set")]
		public static extern void RailFriendsMetadataChanged_friends_changed_metadata_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsMetadataChanged_friends_changed_metadata_get")]
		public static extern IntPtr RailFriendsMetadataChanged_friends_changed_metadata_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailFriendsMetadataChanged__SWIG_1")]
		public static extern IntPtr new_RailFriendsMetadataChanged__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailFriendsMetadataChanged")]
		public static extern void delete_RailFriendsMetadataChanged(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailCurrencyExchangeCoinRate__SWIG_0")]
		public static extern IntPtr new_RailCurrencyExchangeCoinRate__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCurrencyExchangeCoinRate_currency_set")]
		public static extern void RailCurrencyExchangeCoinRate_currency_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCurrencyExchangeCoinRate_currency_get")]
		public static extern IntPtr RailCurrencyExchangeCoinRate_currency_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCurrencyExchangeCoinRate_to_exchange_coins_set")]
		public static extern void RailCurrencyExchangeCoinRate_to_exchange_coins_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCurrencyExchangeCoinRate_to_exchange_coins_get")]
		public static extern uint RailCurrencyExchangeCoinRate_to_exchange_coins_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCurrencyExchangeCoinRate_pay_price_set")]
		public static extern void RailCurrencyExchangeCoinRate_pay_price_set(IntPtr jarg1, float jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCurrencyExchangeCoinRate_pay_price_get")]
		public static extern float RailCurrencyExchangeCoinRate_pay_price_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailCurrencyExchangeCoinRate__SWIG_1")]
		public static extern IntPtr new_RailCurrencyExchangeCoinRate__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailCurrencyExchangeCoinRate")]
		public static extern void delete_RailCurrencyExchangeCoinRate(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailCoinInfo__SWIG_0")]
		public static extern IntPtr new_RailCoinInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoinInfo_name_set")]
		public static extern void RailCoinInfo_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoinInfo_name_get")]
		public static extern IntPtr RailCoinInfo_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoinInfo_description_set")]
		public static extern void RailCoinInfo_description_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoinInfo_description_get")]
		public static extern IntPtr RailCoinInfo_description_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoinInfo_icon_url_set")]
		public static extern void RailCoinInfo_icon_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoinInfo_icon_url_get")]
		public static extern IntPtr RailCoinInfo_icon_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoinInfo_exchange_rate_set")]
		public static extern void RailCoinInfo_exchange_rate_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoinInfo_exchange_rate_get")]
		public static extern IntPtr RailCoinInfo_exchange_rate_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoinInfo_coin_class_id_set")]
		public static extern void RailCoinInfo_coin_class_id_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoinInfo_coin_class_id_get")]
		public static extern uint RailCoinInfo_coin_class_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoinInfo_metadata_set")]
		public static extern void RailCoinInfo_metadata_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoinInfo_metadata_get")]
		public static extern IntPtr RailCoinInfo_metadata_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailCoinInfo__SWIG_1")]
		public static extern IntPtr new_RailCoinInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailCoinInfo")]
		public static extern void delete_RailCoinInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailCoins__SWIG_0")]
		public static extern IntPtr new_RailCoins__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoins_total_price_set")]
		public static extern void RailCoins_total_price_set(IntPtr jarg1, float jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoins_total_price_get")]
		public static extern float RailCoins_total_price_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoins_quantity_set")]
		public static extern void RailCoins_quantity_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoins_quantity_get")]
		public static extern uint RailCoins_quantity_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoins_coin_class_id_set")]
		public static extern void RailCoins_coin_class_id_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoins_coin_class_id_get")]
		public static extern uint RailCoins_coin_class_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoins_zone_id_set")]
		public static extern void RailCoins_zone_id_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCoins_zone_id_get")]
		public static extern IntPtr RailCoins_zone_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailCoins__SWIG_1")]
		public static extern IntPtr new_RailCoins__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailCoins")]
		public static extern void delete_RailCoins(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGameCoinRequestCoinInfoResponse__SWIG_0")]
		public static extern IntPtr new_RailInGameCoinRequestCoinInfoResponse__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGameCoinRequestCoinInfoResponse_coin_infos_set")]
		public static extern void RailInGameCoinRequestCoinInfoResponse_coin_infos_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGameCoinRequestCoinInfoResponse_coin_infos_get")]
		public static extern IntPtr RailInGameCoinRequestCoinInfoResponse_coin_infos_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGameCoinRequestCoinInfoResponse__SWIG_1")]
		public static extern IntPtr new_RailInGameCoinRequestCoinInfoResponse__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailInGameCoinRequestCoinInfoResponse")]
		public static extern void delete_RailInGameCoinRequestCoinInfoResponse(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGameCoinPurchaseCoinsResponse__SWIG_0")]
		public static extern IntPtr new_RailInGameCoinPurchaseCoinsResponse__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGameCoinPurchaseCoinsResponse__SWIG_1")]
		public static extern IntPtr new_RailInGameCoinPurchaseCoinsResponse__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailInGameCoinPurchaseCoinsResponse")]
		public static extern void delete_RailInGameCoinPurchaseCoinsResponse(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailDiscountInfo__SWIG_0")]
		public static extern IntPtr new_RailDiscountInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDiscountInfo_off_set")]
		public static extern void RailDiscountInfo_off_set(IntPtr jarg1, float jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDiscountInfo_off_get")]
		public static extern float RailDiscountInfo_off_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDiscountInfo_discount_price_set")]
		public static extern void RailDiscountInfo_discount_price_set(IntPtr jarg1, float jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDiscountInfo_discount_price_get")]
		public static extern float RailDiscountInfo_discount_price_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDiscountInfo_type_set")]
		public static extern void RailDiscountInfo_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDiscountInfo_type_get")]
		public static extern int RailDiscountInfo_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDiscountInfo_start_time_set")]
		public static extern void RailDiscountInfo_start_time_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDiscountInfo_start_time_get")]
		public static extern uint RailDiscountInfo_start_time_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDiscountInfo_end_time_set")]
		public static extern void RailDiscountInfo_end_time_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailDiscountInfo_end_time_get")]
		public static extern uint RailDiscountInfo_end_time_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailDiscountInfo__SWIG_1")]
		public static extern IntPtr new_RailDiscountInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailDiscountInfo")]
		public static extern void delete_RailDiscountInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPurchaseProductExtraInfo__SWIG_0")]
		public static extern IntPtr new_RailPurchaseProductExtraInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductExtraInfo_exchange_rule_set")]
		public static extern void RailPurchaseProductExtraInfo_exchange_rule_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductExtraInfo_exchange_rule_get")]
		public static extern IntPtr RailPurchaseProductExtraInfo_exchange_rule_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductExtraInfo_bundle_rule_set")]
		public static extern void RailPurchaseProductExtraInfo_bundle_rule_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductExtraInfo_bundle_rule_get")]
		public static extern IntPtr RailPurchaseProductExtraInfo_bundle_rule_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPurchaseProductExtraInfo__SWIG_1")]
		public static extern IntPtr new_RailPurchaseProductExtraInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailPurchaseProductExtraInfo")]
		public static extern void delete_RailPurchaseProductExtraInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPurchaseProductInfo__SWIG_0")]
		public static extern IntPtr new_RailPurchaseProductInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_product_id_set")]
		public static extern void RailPurchaseProductInfo_product_id_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_product_id_get")]
		public static extern uint RailPurchaseProductInfo_product_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_is_purchasable_set")]
		public static extern void RailPurchaseProductInfo_is_purchasable_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_is_purchasable_get")]
		public static extern bool RailPurchaseProductInfo_is_purchasable_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_name_set")]
		public static extern void RailPurchaseProductInfo_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_name_get")]
		public static extern IntPtr RailPurchaseProductInfo_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_description_set")]
		public static extern void RailPurchaseProductInfo_description_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_description_get")]
		public static extern IntPtr RailPurchaseProductInfo_description_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_category_set")]
		public static extern void RailPurchaseProductInfo_category_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_category_get")]
		public static extern IntPtr RailPurchaseProductInfo_category_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_product_thumbnail_set")]
		public static extern void RailPurchaseProductInfo_product_thumbnail_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_product_thumbnail_get")]
		public static extern IntPtr RailPurchaseProductInfo_product_thumbnail_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_extra_info_set")]
		public static extern void RailPurchaseProductInfo_extra_info_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_extra_info_get")]
		public static extern IntPtr RailPurchaseProductInfo_extra_info_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_original_price_set")]
		public static extern void RailPurchaseProductInfo_original_price_set(IntPtr jarg1, float jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_original_price_get")]
		public static extern float RailPurchaseProductInfo_original_price_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_currency_type_set")]
		public static extern void RailPurchaseProductInfo_currency_type_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_currency_type_get")]
		public static extern IntPtr RailPurchaseProductInfo_currency_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_discount_set")]
		public static extern void RailPurchaseProductInfo_discount_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPurchaseProductInfo_discount_get")]
		public static extern IntPtr RailPurchaseProductInfo_discount_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPurchaseProductInfo__SWIG_1")]
		public static extern IntPtr new_RailPurchaseProductInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailPurchaseProductInfo")]
		public static extern void delete_RailPurchaseProductInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGamePurchaseRequestAllPurchasableProductsResponse__SWIG_0")]
		public static extern IntPtr new_RailInGamePurchaseRequestAllPurchasableProductsResponse__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchaseRequestAllPurchasableProductsResponse_purchasable_products_set")]
		public static extern void RailInGamePurchaseRequestAllPurchasableProductsResponse_purchasable_products_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchaseRequestAllPurchasableProductsResponse_purchasable_products_get")]
		public static extern IntPtr RailInGamePurchaseRequestAllPurchasableProductsResponse_purchasable_products_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGamePurchaseRequestAllPurchasableProductsResponse__SWIG_1")]
		public static extern IntPtr new_RailInGamePurchaseRequestAllPurchasableProductsResponse__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailInGamePurchaseRequestAllPurchasableProductsResponse")]
		public static extern void delete_RailInGamePurchaseRequestAllPurchasableProductsResponse(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGamePurchaseRequestAllProductsResponse__SWIG_0")]
		public static extern IntPtr new_RailInGamePurchaseRequestAllProductsResponse__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchaseRequestAllProductsResponse_all_products_set")]
		public static extern void RailInGamePurchaseRequestAllProductsResponse_all_products_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchaseRequestAllProductsResponse_all_products_get")]
		public static extern IntPtr RailInGamePurchaseRequestAllProductsResponse_all_products_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGamePurchaseRequestAllProductsResponse__SWIG_1")]
		public static extern IntPtr new_RailInGamePurchaseRequestAllProductsResponse__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailInGamePurchaseRequestAllProductsResponse")]
		public static extern void delete_RailInGamePurchaseRequestAllProductsResponse(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGamePurchasePurchaseProductsResponse__SWIG_0")]
		public static extern IntPtr new_RailInGamePurchasePurchaseProductsResponse__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchasePurchaseProductsResponse_order_id_set")]
		public static extern void RailInGamePurchasePurchaseProductsResponse_order_id_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchasePurchaseProductsResponse_order_id_get")]
		public static extern IntPtr RailInGamePurchasePurchaseProductsResponse_order_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchasePurchaseProductsResponse_delivered_products_set")]
		public static extern void RailInGamePurchasePurchaseProductsResponse_delivered_products_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchasePurchaseProductsResponse_delivered_products_get")]
		public static extern IntPtr RailInGamePurchasePurchaseProductsResponse_delivered_products_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGamePurchasePurchaseProductsResponse__SWIG_1")]
		public static extern IntPtr new_RailInGamePurchasePurchaseProductsResponse__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailInGamePurchasePurchaseProductsResponse")]
		public static extern void delete_RailInGamePurchasePurchaseProductsResponse(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGamePurchasePurchaseProductsToAssetsResponse__SWIG_0")]
		public static extern IntPtr new_RailInGamePurchasePurchaseProductsToAssetsResponse__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchasePurchaseProductsToAssetsResponse_order_id_set")]
		public static extern void RailInGamePurchasePurchaseProductsToAssetsResponse_order_id_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchasePurchaseProductsToAssetsResponse_order_id_get")]
		public static extern IntPtr RailInGamePurchasePurchaseProductsToAssetsResponse_order_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchasePurchaseProductsToAssetsResponse_delivered_assets_set")]
		public static extern void RailInGamePurchasePurchaseProductsToAssetsResponse_delivered_assets_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchasePurchaseProductsToAssetsResponse_delivered_assets_get")]
		public static extern IntPtr RailInGamePurchasePurchaseProductsToAssetsResponse_delivered_assets_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGamePurchasePurchaseProductsToAssetsResponse__SWIG_1")]
		public static extern IntPtr new_RailInGamePurchasePurchaseProductsToAssetsResponse__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailInGamePurchasePurchaseProductsToAssetsResponse")]
		public static extern void delete_RailInGamePurchasePurchaseProductsToAssetsResponse(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGamePurchaseFinishOrderResponse__SWIG_0")]
		public static extern IntPtr new_RailInGamePurchaseFinishOrderResponse__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchaseFinishOrderResponse_order_id_set")]
		public static extern void RailInGamePurchaseFinishOrderResponse_order_id_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchaseFinishOrderResponse_order_id_get")]
		public static extern IntPtr RailInGamePurchaseFinishOrderResponse_order_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGamePurchaseFinishOrderResponse__SWIG_1")]
		public static extern IntPtr new_RailInGamePurchaseFinishOrderResponse__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailInGamePurchaseFinishOrderResponse")]
		public static extern void delete_RailInGamePurchaseFinishOrderResponse(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGameStorePurchasePayWindowDisplayed__SWIG_0")]
		public static extern IntPtr new_RailInGameStorePurchasePayWindowDisplayed__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGameStorePurchasePayWindowDisplayed_order_id_set")]
		public static extern void RailInGameStorePurchasePayWindowDisplayed_order_id_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGameStorePurchasePayWindowDisplayed_order_id_get")]
		public static extern IntPtr RailInGameStorePurchasePayWindowDisplayed_order_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGameStorePurchasePayWindowDisplayed__SWIG_1")]
		public static extern IntPtr new_RailInGameStorePurchasePayWindowDisplayed__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailInGameStorePurchasePayWindowDisplayed")]
		public static extern void delete_RailInGameStorePurchasePayWindowDisplayed(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGameStorePurchasePayWindowClosed__SWIG_0")]
		public static extern IntPtr new_RailInGameStorePurchasePayWindowClosed__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGameStorePurchasePayWindowClosed_order_id_set")]
		public static extern void RailInGameStorePurchasePayWindowClosed_order_id_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGameStorePurchasePayWindowClosed_order_id_get")]
		public static extern IntPtr RailInGameStorePurchasePayWindowClosed_order_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGameStorePurchasePayWindowClosed__SWIG_1")]
		public static extern IntPtr new_RailInGameStorePurchasePayWindowClosed__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailInGameStorePurchasePayWindowClosed")]
		public static extern void delete_RailInGameStorePurchasePayWindowClosed(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGameStorePurchaseResult__SWIG_0")]
		public static extern IntPtr new_RailInGameStorePurchaseResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGameStorePurchaseResult_order_id_set")]
		public static extern void RailInGameStorePurchaseResult_order_id_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGameStorePurchaseResult_order_id_get")]
		public static extern IntPtr RailInGameStorePurchaseResult_order_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailInGameStorePurchaseResult__SWIG_1")]
		public static extern IntPtr new_RailInGameStorePurchaseResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailInGameStorePurchaseResult")]
		public static extern void delete_RailInGameStorePurchaseResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGameActivityInfo__SWIG_0")]
		public static extern IntPtr new_RailGameActivityInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityInfo_activity_id_set")]
		public static extern void RailGameActivityInfo_activity_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityInfo_activity_id_get")]
		public static extern ulong RailGameActivityInfo_activity_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityInfo_begin_time_set")]
		public static extern void RailGameActivityInfo_begin_time_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityInfo_begin_time_get")]
		public static extern uint RailGameActivityInfo_begin_time_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityInfo_end_time_set")]
		public static extern void RailGameActivityInfo_end_time_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityInfo_end_time_get")]
		public static extern uint RailGameActivityInfo_end_time_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityInfo_activity_name_set")]
		public static extern void RailGameActivityInfo_activity_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityInfo_activity_name_get")]
		public static extern IntPtr RailGameActivityInfo_activity_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityInfo_activity_description_set")]
		public static extern void RailGameActivityInfo_activity_description_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityInfo_activity_description_get")]
		public static extern IntPtr RailGameActivityInfo_activity_description_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityInfo_metadata_key_values_set")]
		public static extern void RailGameActivityInfo_metadata_key_values_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityInfo_metadata_key_values_get")]
		public static extern IntPtr RailGameActivityInfo_metadata_key_values_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGameActivityInfo__SWIG_1")]
		public static extern IntPtr new_RailGameActivityInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailGameActivityInfo")]
		public static extern void delete_RailGameActivityInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailQueryGameActivityResult__SWIG_0")]
		public static extern IntPtr new_RailQueryGameActivityResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryGameActivityResult_game_activities_set")]
		public static extern void RailQueryGameActivityResult_game_activities_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryGameActivityResult_game_activities_get")]
		public static extern IntPtr RailQueryGameActivityResult_game_activities_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailQueryGameActivityResult__SWIG_1")]
		public static extern IntPtr new_RailQueryGameActivityResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailQueryGameActivityResult")]
		public static extern void delete_RailQueryGameActivityResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailOpenGameActivityWindowResult__SWIG_0")]
		public static extern IntPtr new_RailOpenGameActivityWindowResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailOpenGameActivityWindowResult_activity_id_set")]
		public static extern void RailOpenGameActivityWindowResult_activity_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailOpenGameActivityWindowResult_activity_id_get")]
		public static extern ulong RailOpenGameActivityWindowResult_activity_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailOpenGameActivityWindowResult_is_show_set")]
		public static extern void RailOpenGameActivityWindowResult_is_show_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailOpenGameActivityWindowResult_is_show_get")]
		public static extern bool RailOpenGameActivityWindowResult_is_show_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailOpenGameActivityWindowResult__SWIG_1")]
		public static extern IntPtr new_RailOpenGameActivityWindowResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailOpenGameActivityWindowResult")]
		public static extern void delete_RailOpenGameActivityWindowResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailNotifyNewGameActivities__SWIG_0")]
		public static extern IntPtr new_RailNotifyNewGameActivities__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNotifyNewGameActivities_game_activities_set")]
		public static extern void RailNotifyNewGameActivities_game_activities_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNotifyNewGameActivities_game_activities_get")]
		public static extern IntPtr RailNotifyNewGameActivities_game_activities_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailNotifyNewGameActivities__SWIG_1")]
		public static extern IntPtr new_RailNotifyNewGameActivities__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailNotifyNewGameActivities")]
		public static extern void delete_RailNotifyNewGameActivities(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGameActivityPlayerEvent__SWIG_0")]
		public static extern IntPtr new_RailGameActivityPlayerEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityPlayerEvent_from_activity_id_set")]
		public static extern void RailGameActivityPlayerEvent_from_activity_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityPlayerEvent_from_activity_id_get")]
		public static extern ulong RailGameActivityPlayerEvent_from_activity_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityPlayerEvent_event_name_set")]
		public static extern void RailGameActivityPlayerEvent_event_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityPlayerEvent_event_name_get")]
		public static extern IntPtr RailGameActivityPlayerEvent_event_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityPlayerEvent_event_value_set")]
		public static extern void RailGameActivityPlayerEvent_event_value_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityPlayerEvent_event_value_get")]
		public static extern IntPtr RailGameActivityPlayerEvent_event_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGameActivityPlayerEvent__SWIG_1")]
		public static extern IntPtr new_RailGameActivityPlayerEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailGameActivityPlayerEvent")]
		public static extern void delete_RailGameActivityPlayerEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailNetworkSessionState__SWIG_0")]
		public static extern IntPtr new_RailNetworkSessionState__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_is_connection_active_set")]
		public static extern void RailNetworkSessionState_is_connection_active_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_is_connection_active_get")]
		public static extern bool RailNetworkSessionState_is_connection_active_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_is_connecting_set")]
		public static extern void RailNetworkSessionState_is_connecting_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_is_connecting_get")]
		public static extern bool RailNetworkSessionState_is_connecting_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_is_using_relay_set")]
		public static extern void RailNetworkSessionState_is_using_relay_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_is_using_relay_get")]
		public static extern bool RailNetworkSessionState_is_using_relay_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_session_error_set")]
		public static extern void RailNetworkSessionState_session_error_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_session_error_get")]
		public static extern int RailNetworkSessionState_session_error_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_bytes_in_send_buffer_set")]
		public static extern void RailNetworkSessionState_bytes_in_send_buffer_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_bytes_in_send_buffer_get")]
		public static extern uint RailNetworkSessionState_bytes_in_send_buffer_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_packets_in_send_buffer_set")]
		public static extern void RailNetworkSessionState_packets_in_send_buffer_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_packets_in_send_buffer_get")]
		public static extern uint RailNetworkSessionState_packets_in_send_buffer_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_remote_ip_set")]
		public static extern void RailNetworkSessionState_remote_ip_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_remote_ip_get")]
		public static extern uint RailNetworkSessionState_remote_ip_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_remote_port_set")]
		public static extern void RailNetworkSessionState_remote_port_set(IntPtr jarg1, ushort jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNetworkSessionState_remote_port_get")]
		public static extern ushort RailNetworkSessionState_remote_port_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailNetworkSessionState__SWIG_1")]
		public static extern IntPtr new_RailNetworkSessionState__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailNetworkSessionState")]
		public static extern void delete_RailNetworkSessionState(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGamePeer__SWIG_0")]
		public static extern IntPtr new_RailGamePeer__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGamePeer_peer_set")]
		public static extern void RailGamePeer_peer_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGamePeer_peer_get")]
		public static extern IntPtr RailGamePeer_peer_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGamePeer_game_id_set")]
		public static extern void RailGamePeer_game_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGamePeer_game_id_get")]
		public static extern IntPtr RailGamePeer_game_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGamePeer__SWIG_1")]
		public static extern IntPtr new_RailGamePeer__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailGamePeer")]
		public static extern void delete_RailGamePeer(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateSessionRequest__SWIG_0")]
		public static extern IntPtr new_CreateSessionRequest__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateSessionRequest_local_peer_set")]
		public static extern void CreateSessionRequest_local_peer_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateSessionRequest_local_peer_get")]
		public static extern IntPtr CreateSessionRequest_local_peer_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateSessionRequest_remote_peer_set")]
		public static extern void CreateSessionRequest_remote_peer_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateSessionRequest_remote_peer_get")]
		public static extern IntPtr CreateSessionRequest_remote_peer_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateSessionRequest__SWIG_1")]
		public static extern IntPtr new_CreateSessionRequest__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CreateSessionRequest")]
		public static extern void delete_CreateSessionRequest(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateSessionFailed__SWIG_0")]
		public static extern IntPtr new_CreateSessionFailed__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateSessionFailed_local_peer_set")]
		public static extern void CreateSessionFailed_local_peer_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateSessionFailed_local_peer_get")]
		public static extern IntPtr CreateSessionFailed_local_peer_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateSessionFailed_remote_peer_set")]
		public static extern void CreateSessionFailed_remote_peer_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateSessionFailed_remote_peer_get")]
		public static extern IntPtr CreateSessionFailed_remote_peer_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateSessionFailed__SWIG_1")]
		public static extern IntPtr new_CreateSessionFailed__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CreateSessionFailed")]
		public static extern void delete_CreateSessionFailed(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NetworkCreateRawSessionRequest__SWIG_0")]
		public static extern IntPtr new_NetworkCreateRawSessionRequest__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_NetworkCreateRawSessionRequest_local_peer_set")]
		public static extern void NetworkCreateRawSessionRequest_local_peer_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NetworkCreateRawSessionRequest_local_peer_get")]
		public static extern IntPtr NetworkCreateRawSessionRequest_local_peer_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NetworkCreateRawSessionRequest_remote_game_peer_set")]
		public static extern void NetworkCreateRawSessionRequest_remote_game_peer_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NetworkCreateRawSessionRequest_remote_game_peer_get")]
		public static extern IntPtr NetworkCreateRawSessionRequest_remote_game_peer_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NetworkCreateRawSessionRequest__SWIG_1")]
		public static extern IntPtr new_NetworkCreateRawSessionRequest__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_NetworkCreateRawSessionRequest")]
		public static extern void delete_NetworkCreateRawSessionRequest(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NetworkCreateRawSessionFailed__SWIG_0")]
		public static extern IntPtr new_NetworkCreateRawSessionFailed__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_NetworkCreateRawSessionFailed_local_peer_set")]
		public static extern void NetworkCreateRawSessionFailed_local_peer_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NetworkCreateRawSessionFailed_local_peer_get")]
		public static extern IntPtr NetworkCreateRawSessionFailed_local_peer_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NetworkCreateRawSessionFailed_remote_game_peer_set")]
		public static extern void NetworkCreateRawSessionFailed_remote_game_peer_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NetworkCreateRawSessionFailed_remote_game_peer_get")]
		public static extern IntPtr NetworkCreateRawSessionFailed_remote_game_peer_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NetworkCreateRawSessionFailed__SWIG_1")]
		public static extern IntPtr new_NetworkCreateRawSessionFailed__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_NetworkCreateRawSessionFailed")]
		public static extern void delete_NetworkCreateRawSessionFailed(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_kRailRoomDefaultMaxMemberNumber_get")]
		public static extern uint kRailRoomDefaultMaxMemberNumber_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_kRailRoomDataKeyValuePairsLimit_get")]
		public static extern uint kRailRoomDataKeyValuePairsLimit_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomOptions__SWIG_0")]
		public static extern IntPtr new_RoomOptions__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomOptions_type_set")]
		public static extern void RoomOptions_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomOptions_type_get")]
		public static extern int RoomOptions_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomOptions_max_members_set")]
		public static extern void RoomOptions_max_members_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomOptions_max_members_get")]
		public static extern uint RoomOptions_max_members_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomOptions_password_set")]
		public static extern void RoomOptions_password_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomOptions_password_get")]
		public static extern IntPtr RoomOptions_password_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomOptions_room_tag_set")]
		public static extern void RoomOptions_room_tag_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomOptions_room_tag_get")]
		public static extern IntPtr RoomOptions_room_tag_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomOptions_enable_team_voice_set")]
		public static extern void RoomOptions_enable_team_voice_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomOptions_enable_team_voice_get")]
		public static extern bool RoomOptions_enable_team_voice_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomOptions__SWIG_1")]
		public static extern IntPtr new_RoomOptions__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RoomOptions")]
		public static extern void delete_RoomOptions(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomInfoListSorter__SWIG_0")]
		public static extern IntPtr new_RoomInfoListSorter__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListSorter_property_value_type_set")]
		public static extern void RoomInfoListSorter_property_value_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListSorter_property_value_type_get")]
		public static extern int RoomInfoListSorter_property_value_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListSorter_property_sort_type_set")]
		public static extern void RoomInfoListSorter_property_sort_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListSorter_property_sort_type_get")]
		public static extern int RoomInfoListSorter_property_sort_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListSorter_property_key_set")]
		public static extern void RoomInfoListSorter_property_key_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListSorter_property_key_get")]
		public static extern IntPtr RoomInfoListSorter_property_key_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListSorter_close_to_value_set")]
		public static extern void RoomInfoListSorter_close_to_value_set(IntPtr jarg1, double jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListSorter_close_to_value_get")]
		public static extern double RoomInfoListSorter_close_to_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomInfoListSorter__SWIG_1")]
		public static extern IntPtr new_RoomInfoListSorter__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RoomInfoListSorter")]
		public static extern void delete_RoomInfoListSorter(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomInfoListFilterKey__SWIG_0")]
		public static extern IntPtr new_RoomInfoListFilterKey__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilterKey_key_name_set")]
		public static extern void RoomInfoListFilterKey_key_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilterKey_key_name_get")]
		public static extern IntPtr RoomInfoListFilterKey_key_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilterKey_value_type_set")]
		public static extern void RoomInfoListFilterKey_value_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilterKey_value_type_get")]
		public static extern int RoomInfoListFilterKey_value_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilterKey_comparison_type_set")]
		public static extern void RoomInfoListFilterKey_comparison_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilterKey_comparison_type_get")]
		public static extern int RoomInfoListFilterKey_comparison_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilterKey_filter_value_set")]
		public static extern void RoomInfoListFilterKey_filter_value_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilterKey_filter_value_get")]
		public static extern IntPtr RoomInfoListFilterKey_filter_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomInfoListFilterKey__SWIG_1")]
		public static extern IntPtr new_RoomInfoListFilterKey__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RoomInfoListFilterKey")]
		public static extern void delete_RoomInfoListFilterKey(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomInfoListFilter__SWIG_0")]
		public static extern IntPtr new_RoomInfoListFilter__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_key_filters_set")]
		public static extern void RoomInfoListFilter_key_filters_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_key_filters_get")]
		public static extern IntPtr RoomInfoListFilter_key_filters_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_room_name_contained_set")]
		public static extern void RoomInfoListFilter_room_name_contained_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_room_name_contained_get")]
		public static extern IntPtr RoomInfoListFilter_room_name_contained_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_room_tag_set")]
		public static extern void RoomInfoListFilter_room_tag_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_room_tag_get")]
		public static extern IntPtr RoomInfoListFilter_room_tag_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_filter_password_set")]
		public static extern void RoomInfoListFilter_filter_password_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_filter_password_get")]
		public static extern int RoomInfoListFilter_filter_password_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_filter_friends_owned_set")]
		public static extern void RoomInfoListFilter_filter_friends_owned_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_filter_friends_owned_get")]
		public static extern int RoomInfoListFilter_filter_friends_owned_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_filter_friends_in_room_set")]
		public static extern void RoomInfoListFilter_filter_friends_in_room_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_filter_friends_in_room_get")]
		public static extern int RoomInfoListFilter_filter_friends_in_room_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_available_slot_at_least_set")]
		public static extern void RoomInfoListFilter_available_slot_at_least_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfoListFilter_available_slot_at_least_get")]
		public static extern uint RoomInfoListFilter_available_slot_at_least_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomInfoListFilter__SWIG_1")]
		public static extern IntPtr new_RoomInfoListFilter__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RoomInfoListFilter")]
		public static extern void delete_RoomInfoListFilter(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomInfo__SWIG_0")]
		public static extern IntPtr new_RoomInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_room_id_set")]
		public static extern void RoomInfo_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_room_id_get")]
		public static extern ulong RoomInfo_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_owner_id_set")]
		public static extern void RoomInfo_owner_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_owner_id_get")]
		public static extern IntPtr RoomInfo_owner_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_max_members_set")]
		public static extern void RoomInfo_max_members_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_max_members_get")]
		public static extern uint RoomInfo_max_members_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_current_members_set")]
		public static extern void RoomInfo_current_members_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_current_members_get")]
		public static extern uint RoomInfo_current_members_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_create_time_set")]
		public static extern void RoomInfo_create_time_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_create_time_get")]
		public static extern uint RoomInfo_create_time_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_room_name_set")]
		public static extern void RoomInfo_room_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_room_name_get")]
		public static extern IntPtr RoomInfo_room_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_room_tag_set")]
		public static extern void RoomInfo_room_tag_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_room_tag_get")]
		public static extern IntPtr RoomInfo_room_tag_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_has_password_set")]
		public static extern void RoomInfo_has_password_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_has_password_get")]
		public static extern bool RoomInfo_has_password_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_is_joinable_set")]
		public static extern void RoomInfo_is_joinable_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_is_joinable_get")]
		public static extern bool RoomInfo_is_joinable_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_type_set")]
		public static extern void RoomInfo_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_type_get")]
		public static extern int RoomInfo_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_game_server_rail_id_set")]
		public static extern void RoomInfo_game_server_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_game_server_rail_id_get")]
		public static extern IntPtr RoomInfo_game_server_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_room_kvs_set")]
		public static extern void RoomInfo_room_kvs_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomInfo_room_kvs_get")]
		public static extern IntPtr RoomInfo_room_kvs_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomInfo__SWIG_1")]
		public static extern IntPtr new_RoomInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RoomInfo")]
		public static extern void delete_RoomInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomMemberInfo__SWIG_0")]
		public static extern IntPtr new_RoomMemberInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomMemberInfo_room_id_set")]
		public static extern void RoomMemberInfo_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomMemberInfo_room_id_get")]
		public static extern ulong RoomMemberInfo_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomMemberInfo_member_id_set")]
		public static extern void RoomMemberInfo_member_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomMemberInfo_member_id_get")]
		public static extern IntPtr RoomMemberInfo_member_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomMemberInfo_member_index_set")]
		public static extern void RoomMemberInfo_member_index_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomMemberInfo_member_index_get")]
		public static extern uint RoomMemberInfo_member_index_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomMemberInfo_member_name_set")]
		public static extern void RoomMemberInfo_member_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomMemberInfo_member_name_get")]
		public static extern IntPtr RoomMemberInfo_member_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomMemberInfo_member_kvs_set")]
		public static extern void RoomMemberInfo_member_kvs_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomMemberInfo_member_kvs_get")]
		public static extern IntPtr RoomMemberInfo_member_kvs_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomMemberInfo__SWIG_1")]
		public static extern IntPtr new_RoomMemberInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RoomMemberInfo")]
		public static extern void delete_RoomMemberInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateRoomResult__SWIG_0")]
		public static extern IntPtr new_CreateRoomResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateRoomResult_room_id_set")]
		public static extern void CreateRoomResult_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateRoomResult_room_id_get")]
		public static extern ulong CreateRoomResult_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateRoomResult__SWIG_1")]
		public static extern IntPtr new_CreateRoomResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CreateRoomResult")]
		public static extern void delete_CreateRoomResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_OpenRoomResult__SWIG_0")]
		public static extern IntPtr new_OpenRoomResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_OpenRoomResult_room_id_set")]
		public static extern void OpenRoomResult_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_OpenRoomResult_room_id_get")]
		public static extern ulong OpenRoomResult_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_OpenRoomResult__SWIG_1")]
		public static extern IntPtr new_OpenRoomResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_OpenRoomResult")]
		public static extern void delete_OpenRoomResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetRoomListResult__SWIG_0")]
		public static extern IntPtr new_GetRoomListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomListResult_begin_index_set")]
		public static extern void GetRoomListResult_begin_index_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomListResult_begin_index_get")]
		public static extern uint GetRoomListResult_begin_index_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomListResult_end_index_set")]
		public static extern void GetRoomListResult_end_index_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomListResult_end_index_get")]
		public static extern uint GetRoomListResult_end_index_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomListResult_total_room_num_set")]
		public static extern void GetRoomListResult_total_room_num_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomListResult_total_room_num_get")]
		public static extern uint GetRoomListResult_total_room_num_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomListResult_room_infos_set")]
		public static extern void GetRoomListResult_room_infos_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomListResult_room_infos_get")]
		public static extern IntPtr GetRoomListResult_room_infos_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetRoomListResult__SWIG_1")]
		public static extern IntPtr new_GetRoomListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GetRoomListResult")]
		public static extern void delete_GetRoomListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetUserRoomListResult__SWIG_0")]
		public static extern IntPtr new_GetUserRoomListResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GetUserRoomListResult_room_info_set")]
		public static extern void GetUserRoomListResult_room_info_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetUserRoomListResult_room_info_get")]
		public static extern IntPtr GetUserRoomListResult_room_info_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetUserRoomListResult__SWIG_1")]
		public static extern IntPtr new_GetUserRoomListResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GetUserRoomListResult")]
		public static extern void delete_GetUserRoomListResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetNewRoomOwnerResult__SWIG_0")]
		public static extern IntPtr new_SetNewRoomOwnerResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetNewRoomOwnerResult__SWIG_1")]
		public static extern IntPtr new_SetNewRoomOwnerResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_SetNewRoomOwnerResult")]
		public static extern void delete_SetNewRoomOwnerResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetRoomMembersResult__SWIG_0")]
		public static extern IntPtr new_GetRoomMembersResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomMembersResult_room_id_set")]
		public static extern void GetRoomMembersResult_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomMembersResult_room_id_get")]
		public static extern ulong GetRoomMembersResult_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomMembersResult_member_num_set")]
		public static extern void GetRoomMembersResult_member_num_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomMembersResult_member_num_get")]
		public static extern uint GetRoomMembersResult_member_num_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomMembersResult_member_infos_set")]
		public static extern void GetRoomMembersResult_member_infos_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomMembersResult_member_infos_get")]
		public static extern IntPtr GetRoomMembersResult_member_infos_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetRoomMembersResult__SWIG_1")]
		public static extern IntPtr new_GetRoomMembersResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GetRoomMembersResult")]
		public static extern void delete_GetRoomMembersResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaveRoomResult__SWIG_0")]
		public static extern IntPtr new_LeaveRoomResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaveRoomResult_room_id_set")]
		public static extern void LeaveRoomResult_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaveRoomResult_room_id_get")]
		public static extern ulong LeaveRoomResult_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaveRoomResult_reason_set")]
		public static extern void LeaveRoomResult_reason_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaveRoomResult_reason_get")]
		public static extern int LeaveRoomResult_reason_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaveRoomResult__SWIG_1")]
		public static extern IntPtr new_LeaveRoomResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_LeaveRoomResult")]
		public static extern void delete_LeaveRoomResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_JoinRoomResult__SWIG_0")]
		public static extern IntPtr new_JoinRoomResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_JoinRoomResult_room_id_set")]
		public static extern void JoinRoomResult_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_JoinRoomResult_room_id_get")]
		public static extern ulong JoinRoomResult_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_JoinRoomResult__SWIG_1")]
		public static extern IntPtr new_JoinRoomResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_JoinRoomResult")]
		public static extern void delete_JoinRoomResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetAllRoomDataResult__SWIG_0")]
		public static extern IntPtr new_GetAllRoomDataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GetAllRoomDataResult_room_info_set")]
		public static extern void GetAllRoomDataResult_room_info_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetAllRoomDataResult_room_info_get")]
		public static extern IntPtr GetAllRoomDataResult_room_info_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetAllRoomDataResult__SWIG_1")]
		public static extern IntPtr new_GetAllRoomDataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GetAllRoomDataResult")]
		public static extern void delete_GetAllRoomDataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_KickOffMemberResult__SWIG_0")]
		public static extern IntPtr new_KickOffMemberResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_KickOffMemberResult_room_id_set")]
		public static extern void KickOffMemberResult_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_KickOffMemberResult_room_id_get")]
		public static extern ulong KickOffMemberResult_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_KickOffMemberResult_kicked_id_set")]
		public static extern void KickOffMemberResult_kicked_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_KickOffMemberResult_kicked_id_get")]
		public static extern IntPtr KickOffMemberResult_kicked_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_KickOffMemberResult__SWIG_1")]
		public static extern IntPtr new_KickOffMemberResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_KickOffMemberResult")]
		public static extern void delete_KickOffMemberResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetRoomTagResult__SWIG_0")]
		public static extern IntPtr new_SetRoomTagResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetRoomTagResult__SWIG_1")]
		public static extern IntPtr new_SetRoomTagResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_SetRoomTagResult")]
		public static extern void delete_SetRoomTagResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetRoomTagResult__SWIG_0")]
		public static extern IntPtr new_GetRoomTagResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomTagResult_room_tag_set")]
		public static extern void GetRoomTagResult_room_tag_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomTagResult_room_tag_get")]
		public static extern IntPtr GetRoomTagResult_room_tag_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetRoomTagResult__SWIG_1")]
		public static extern IntPtr new_GetRoomTagResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GetRoomTagResult")]
		public static extern void delete_GetRoomTagResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetRoomMetadataResult__SWIG_0")]
		public static extern IntPtr new_SetRoomMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_SetRoomMetadataResult_room_id_set")]
		public static extern void SetRoomMetadataResult_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetRoomMetadataResult_room_id_get")]
		public static extern ulong SetRoomMetadataResult_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetRoomMetadataResult__SWIG_1")]
		public static extern IntPtr new_SetRoomMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_SetRoomMetadataResult")]
		public static extern void delete_SetRoomMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetRoomMetadataResult__SWIG_0")]
		public static extern IntPtr new_GetRoomMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomMetadataResult_room_id_set")]
		public static extern void GetRoomMetadataResult_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomMetadataResult_room_id_get")]
		public static extern ulong GetRoomMetadataResult_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomMetadataResult_key_value_set")]
		public static extern void GetRoomMetadataResult_key_value_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomMetadataResult_key_value_get")]
		public static extern IntPtr GetRoomMetadataResult_key_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetRoomMetadataResult__SWIG_1")]
		public static extern IntPtr new_GetRoomMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GetRoomMetadataResult")]
		public static extern void delete_GetRoomMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ClearRoomMetadataResult__SWIG_0")]
		public static extern IntPtr new_ClearRoomMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_ClearRoomMetadataResult_room_id_set")]
		public static extern void ClearRoomMetadataResult_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_ClearRoomMetadataResult_room_id_get")]
		public static extern ulong ClearRoomMetadataResult_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ClearRoomMetadataResult__SWIG_1")]
		public static extern IntPtr new_ClearRoomMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_ClearRoomMetadataResult")]
		public static extern void delete_ClearRoomMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetMemberMetadataResult__SWIG_0")]
		public static extern IntPtr new_SetMemberMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_SetMemberMetadataResult_room_id_set")]
		public static extern void SetMemberMetadataResult_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetMemberMetadataResult_room_id_get")]
		public static extern ulong SetMemberMetadataResult_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetMemberMetadataResult_member_id_set")]
		public static extern void SetMemberMetadataResult_member_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetMemberMetadataResult_member_id_get")]
		public static extern IntPtr SetMemberMetadataResult_member_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetMemberMetadataResult__SWIG_1")]
		public static extern IntPtr new_SetMemberMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_SetMemberMetadataResult")]
		public static extern void delete_SetMemberMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetMemberMetadataResult__SWIG_0")]
		public static extern IntPtr new_GetMemberMetadataResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GetMemberMetadataResult_room_id_set")]
		public static extern void GetMemberMetadataResult_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetMemberMetadataResult_room_id_get")]
		public static extern ulong GetMemberMetadataResult_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetMemberMetadataResult_member_id_set")]
		public static extern void GetMemberMetadataResult_member_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetMemberMetadataResult_member_id_get")]
		public static extern IntPtr GetMemberMetadataResult_member_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetMemberMetadataResult_key_value_set")]
		public static extern void GetMemberMetadataResult_key_value_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetMemberMetadataResult_key_value_get")]
		public static extern IntPtr GetMemberMetadataResult_key_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GetMemberMetadataResult__SWIG_1")]
		public static extern IntPtr new_GetMemberMetadataResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GetMemberMetadataResult")]
		public static extern void delete_GetMemberMetadataResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomDataReceived__SWIG_0")]
		public static extern IntPtr new_RoomDataReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomDataReceived_remote_peer_set")]
		public static extern void RoomDataReceived_remote_peer_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomDataReceived_remote_peer_get")]
		public static extern IntPtr RoomDataReceived_remote_peer_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomDataReceived_message_type_set")]
		public static extern void RoomDataReceived_message_type_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomDataReceived_message_type_get")]
		public static extern uint RoomDataReceived_message_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomDataReceived_data_len_set")]
		public static extern void RoomDataReceived_data_len_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomDataReceived_data_len_get")]
		public static extern uint RoomDataReceived_data_len_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomDataReceived_data_buf_set")]
		public static extern void RoomDataReceived_data_buf_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomDataReceived_data_buf_get")]
		public static extern IntPtr RoomDataReceived_data_buf_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RoomDataReceived__SWIG_1")]
		public static extern IntPtr new_RoomDataReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RoomDataReceived")]
		public static extern void delete_RoomDataReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetRoomTypeResult__SWIG_0")]
		public static extern IntPtr new_SetRoomTypeResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_SetRoomTypeResult_room_type_set")]
		public static extern void SetRoomTypeResult_room_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetRoomTypeResult_room_type_get")]
		public static extern int SetRoomTypeResult_room_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetRoomTypeResult__SWIG_1")]
		public static extern IntPtr new_SetRoomTypeResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_SetRoomTypeResult")]
		public static extern void delete_SetRoomTypeResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetRoomMaxMemberResult__SWIG_0")]
		public static extern IntPtr new_SetRoomMaxMemberResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_SetRoomMaxMemberResult__SWIG_1")]
		public static extern IntPtr new_SetRoomMaxMemberResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_SetRoomMaxMemberResult")]
		public static extern void delete_SetRoomMaxMemberResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NotifyMetadataChange__SWIG_0")]
		public static extern IntPtr new_NotifyMetadataChange__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyMetadataChange_room_id_set")]
		public static extern void NotifyMetadataChange_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyMetadataChange_room_id_get")]
		public static extern ulong NotifyMetadataChange_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyMetadataChange_changer_id_set")]
		public static extern void NotifyMetadataChange_changer_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyMetadataChange_changer_id_get")]
		public static extern IntPtr NotifyMetadataChange_changer_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NotifyMetadataChange__SWIG_1")]
		public static extern IntPtr new_NotifyMetadataChange__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_NotifyMetadataChange")]
		public static extern void delete_NotifyMetadataChange(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NotifyRoomMemberChange__SWIG_0")]
		public static extern IntPtr new_NotifyRoomMemberChange__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberChange_room_id_set")]
		public static extern void NotifyRoomMemberChange_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberChange_room_id_get")]
		public static extern ulong NotifyRoomMemberChange_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberChange_changer_id_set")]
		public static extern void NotifyRoomMemberChange_changer_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberChange_changer_id_get")]
		public static extern IntPtr NotifyRoomMemberChange_changer_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberChange_id_for_making_change_set")]
		public static extern void NotifyRoomMemberChange_id_for_making_change_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberChange_id_for_making_change_get")]
		public static extern IntPtr NotifyRoomMemberChange_id_for_making_change_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberChange_state_change_set")]
		public static extern void NotifyRoomMemberChange_state_change_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberChange_state_change_get")]
		public static extern int NotifyRoomMemberChange_state_change_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NotifyRoomMemberChange__SWIG_1")]
		public static extern IntPtr new_NotifyRoomMemberChange__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_NotifyRoomMemberChange")]
		public static extern void delete_NotifyRoomMemberChange(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NotifyRoomMemberKicked__SWIG_0")]
		public static extern IntPtr new_NotifyRoomMemberKicked__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberKicked_room_id_set")]
		public static extern void NotifyRoomMemberKicked_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberKicked_room_id_get")]
		public static extern ulong NotifyRoomMemberKicked_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberKicked_id_for_making_kick_set")]
		public static extern void NotifyRoomMemberKicked_id_for_making_kick_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberKicked_id_for_making_kick_get")]
		public static extern IntPtr NotifyRoomMemberKicked_id_for_making_kick_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberKicked_kicked_id_set")]
		public static extern void NotifyRoomMemberKicked_kicked_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberKicked_kicked_id_get")]
		public static extern IntPtr NotifyRoomMemberKicked_kicked_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberKicked_due_to_kicker_lost_connect_set")]
		public static extern void NotifyRoomMemberKicked_due_to_kicker_lost_connect_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberKicked_due_to_kicker_lost_connect_get")]
		public static extern uint NotifyRoomMemberKicked_due_to_kicker_lost_connect_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NotifyRoomMemberKicked__SWIG_1")]
		public static extern IntPtr new_NotifyRoomMemberKicked__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_NotifyRoomMemberKicked")]
		public static extern void delete_NotifyRoomMemberKicked(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NotifyRoomDestroy__SWIG_0")]
		public static extern IntPtr new_NotifyRoomDestroy__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomDestroy_room_id_set")]
		public static extern void NotifyRoomDestroy_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomDestroy_room_id_get")]
		public static extern ulong NotifyRoomDestroy_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NotifyRoomDestroy__SWIG_1")]
		public static extern IntPtr new_NotifyRoomDestroy__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_NotifyRoomDestroy")]
		public static extern void delete_NotifyRoomDestroy(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NotifyRoomOwnerChange__SWIG_0")]
		public static extern IntPtr new_NotifyRoomOwnerChange__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomOwnerChange_room_id_set")]
		public static extern void NotifyRoomOwnerChange_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomOwnerChange_room_id_get")]
		public static extern ulong NotifyRoomOwnerChange_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomOwnerChange_old_owner_id_set")]
		public static extern void NotifyRoomOwnerChange_old_owner_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomOwnerChange_old_owner_id_get")]
		public static extern IntPtr NotifyRoomOwnerChange_old_owner_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomOwnerChange_new_owner_id_set")]
		public static extern void NotifyRoomOwnerChange_new_owner_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomOwnerChange_new_owner_id_get")]
		public static extern IntPtr NotifyRoomOwnerChange_new_owner_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomOwnerChange_reason_set")]
		public static extern void NotifyRoomOwnerChange_reason_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomOwnerChange_reason_get")]
		public static extern int NotifyRoomOwnerChange_reason_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NotifyRoomOwnerChange__SWIG_1")]
		public static extern IntPtr new_NotifyRoomOwnerChange__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_NotifyRoomOwnerChange")]
		public static extern void delete_NotifyRoomOwnerChange(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NotifyRoomGameServerChange__SWIG_0")]
		public static extern IntPtr new_NotifyRoomGameServerChange__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomGameServerChange_room_id_set")]
		public static extern void NotifyRoomGameServerChange_room_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomGameServerChange_room_id_get")]
		public static extern ulong NotifyRoomGameServerChange_room_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomGameServerChange_game_server_rail_id_set")]
		public static extern void NotifyRoomGameServerChange_game_server_rail_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomGameServerChange_game_server_rail_id_get")]
		public static extern IntPtr NotifyRoomGameServerChange_game_server_rail_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomGameServerChange_game_server_channel_id_set")]
		public static extern void NotifyRoomGameServerChange_game_server_channel_id_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomGameServerChange_game_server_channel_id_get")]
		public static extern ulong NotifyRoomGameServerChange_game_server_channel_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NotifyRoomGameServerChange__SWIG_1")]
		public static extern IntPtr new_NotifyRoomGameServerChange__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_NotifyRoomGameServerChange")]
		public static extern void delete_NotifyRoomGameServerChange(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSyncFileOption__SWIG_0")]
		public static extern IntPtr new_RailSyncFileOption__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSyncFileOption_sync_file_not_to_remote_set")]
		public static extern void RailSyncFileOption_sync_file_not_to_remote_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSyncFileOption_sync_file_not_to_remote_get")]
		public static extern bool RailSyncFileOption_sync_file_not_to_remote_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSyncFileOption__SWIG_1")]
		public static extern IntPtr new_RailSyncFileOption__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSyncFileOption")]
		public static extern void delete_RailSyncFileOption(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailStreamFileOption__SWIG_0")]
		public static extern IntPtr new_RailStreamFileOption__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStreamFileOption_unavaliabe_when_new_file_writing_set")]
		public static extern void RailStreamFileOption_unavaliabe_when_new_file_writing_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStreamFileOption_unavaliabe_when_new_file_writing_get")]
		public static extern bool RailStreamFileOption_unavaliabe_when_new_file_writing_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStreamFileOption_open_type_set")]
		public static extern void RailStreamFileOption_open_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStreamFileOption_open_type_get")]
		public static extern int RailStreamFileOption_open_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailStreamFileOption__SWIG_1")]
		public static extern IntPtr new_RailStreamFileOption__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailStreamFileOption")]
		public static extern void delete_RailStreamFileOption(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailListStreamFileOption__SWIG_0")]
		public static extern IntPtr new_RailListStreamFileOption__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailListStreamFileOption_start_index_set")]
		public static extern void RailListStreamFileOption_start_index_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailListStreamFileOption_start_index_get")]
		public static extern uint RailListStreamFileOption_start_index_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailListStreamFileOption_num_files_set")]
		public static extern void RailListStreamFileOption_num_files_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailListStreamFileOption_num_files_get")]
		public static extern uint RailListStreamFileOption_num_files_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailListStreamFileOption__SWIG_1")]
		public static extern IntPtr new_RailListStreamFileOption__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailListStreamFileOption")]
		public static extern void delete_RailListStreamFileOption(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPublishFileToUserSpaceOption__SWIG_0")]
		public static extern IntPtr new_RailPublishFileToUserSpaceOption__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_type_set")]
		public static extern void RailPublishFileToUserSpaceOption_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_type_get")]
		public static extern int RailPublishFileToUserSpaceOption_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_space_work_name_set")]
		public static extern void RailPublishFileToUserSpaceOption_space_work_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_space_work_name_get")]
		public static extern IntPtr RailPublishFileToUserSpaceOption_space_work_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_description_set")]
		public static extern void RailPublishFileToUserSpaceOption_description_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_description_get")]
		public static extern IntPtr RailPublishFileToUserSpaceOption_description_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_preview_path_filename_set")]
		public static extern void RailPublishFileToUserSpaceOption_preview_path_filename_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_preview_path_filename_get")]
		public static extern IntPtr RailPublishFileToUserSpaceOption_preview_path_filename_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_version_set")]
		public static extern void RailPublishFileToUserSpaceOption_version_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_version_get")]
		public static extern IntPtr RailPublishFileToUserSpaceOption_version_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_tags_set")]
		public static extern void RailPublishFileToUserSpaceOption_tags_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_tags_get")]
		public static extern IntPtr RailPublishFileToUserSpaceOption_tags_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_level_set")]
		public static extern void RailPublishFileToUserSpaceOption_level_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_level_get")]
		public static extern int RailPublishFileToUserSpaceOption_level_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_key_value_set")]
		public static extern void RailPublishFileToUserSpaceOption_key_value_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPublishFileToUserSpaceOption_key_value_get")]
		public static extern IntPtr RailPublishFileToUserSpaceOption_key_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPublishFileToUserSpaceOption__SWIG_1")]
		public static extern IntPtr new_RailPublishFileToUserSpaceOption__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailPublishFileToUserSpaceOption")]
		public static extern void delete_RailPublishFileToUserSpaceOption(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStreamFileInfo_filename_set")]
		public static extern void RailStreamFileInfo_filename_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStreamFileInfo_filename_get")]
		public static extern IntPtr RailStreamFileInfo_filename_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStreamFileInfo_file_size_set")]
		public static extern void RailStreamFileInfo_file_size_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailStreamFileInfo_file_size_get")]
		public static extern ulong RailStreamFileInfo_file_size_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailStreamFileInfo__SWIG_0")]
		public static extern IntPtr new_RailStreamFileInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailStreamFileInfo__SWIG_1")]
		public static extern IntPtr new_RailStreamFileInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailStreamFileInfo")]
		public static extern void delete_RailStreamFileInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncQueryQuotaResult__SWIG_0")]
		public static extern IntPtr new_AsyncQueryQuotaResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncQueryQuotaResult_total_quota_set")]
		public static extern void AsyncQueryQuotaResult_total_quota_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncQueryQuotaResult_total_quota_get")]
		public static extern ulong AsyncQueryQuotaResult_total_quota_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncQueryQuotaResult_available_quota_set")]
		public static extern void AsyncQueryQuotaResult_available_quota_set(IntPtr jarg1, ulong jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncQueryQuotaResult_available_quota_get")]
		public static extern ulong AsyncQueryQuotaResult_available_quota_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncQueryQuotaResult__SWIG_1")]
		public static extern IntPtr new_AsyncQueryQuotaResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncQueryQuotaResult")]
		public static extern void delete_AsyncQueryQuotaResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ShareStorageToSpaceWorkResult__SWIG_0")]
		public static extern IntPtr new_ShareStorageToSpaceWorkResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_ShareStorageToSpaceWorkResult_space_work_id_set")]
		public static extern void ShareStorageToSpaceWorkResult_space_work_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_ShareStorageToSpaceWorkResult_space_work_id_get")]
		public static extern IntPtr ShareStorageToSpaceWorkResult_space_work_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_ShareStorageToSpaceWorkResult__SWIG_1")]
		public static extern IntPtr new_ShareStorageToSpaceWorkResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_ShareStorageToSpaceWorkResult")]
		public static extern void delete_ShareStorageToSpaceWorkResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncReadFileResult__SWIG_0")]
		public static extern IntPtr new_AsyncReadFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadFileResult_filename_set")]
		public static extern void AsyncReadFileResult_filename_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadFileResult_filename_get")]
		public static extern IntPtr AsyncReadFileResult_filename_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadFileResult_data_set")]
		public static extern void AsyncReadFileResult_data_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadFileResult_data_get")]
		public static extern IntPtr AsyncReadFileResult_data_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadFileResult_offset_set")]
		public static extern void AsyncReadFileResult_offset_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadFileResult_offset_get")]
		public static extern int AsyncReadFileResult_offset_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadFileResult_try_read_length_set")]
		public static extern void AsyncReadFileResult_try_read_length_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadFileResult_try_read_length_get")]
		public static extern uint AsyncReadFileResult_try_read_length_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncReadFileResult__SWIG_1")]
		public static extern IntPtr new_AsyncReadFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncReadFileResult")]
		public static extern void delete_AsyncReadFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncWriteFileResult__SWIG_0")]
		public static extern IntPtr new_AsyncWriteFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteFileResult_filename_set")]
		public static extern void AsyncWriteFileResult_filename_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteFileResult_filename_get")]
		public static extern IntPtr AsyncWriteFileResult_filename_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteFileResult_offset_set")]
		public static extern void AsyncWriteFileResult_offset_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteFileResult_offset_get")]
		public static extern int AsyncWriteFileResult_offset_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteFileResult_try_write_length_set")]
		public static extern void AsyncWriteFileResult_try_write_length_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteFileResult_try_write_length_get")]
		public static extern uint AsyncWriteFileResult_try_write_length_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteFileResult_written_length_set")]
		public static extern void AsyncWriteFileResult_written_length_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteFileResult_written_length_get")]
		public static extern uint AsyncWriteFileResult_written_length_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncWriteFileResult__SWIG_1")]
		public static extern IntPtr new_AsyncWriteFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncWriteFileResult")]
		public static extern void delete_AsyncWriteFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncReadStreamFileResult__SWIG_0")]
		public static extern IntPtr new_AsyncReadStreamFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadStreamFileResult_filename_set")]
		public static extern void AsyncReadStreamFileResult_filename_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadStreamFileResult_filename_get")]
		public static extern IntPtr AsyncReadStreamFileResult_filename_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadStreamFileResult_data_set")]
		public static extern void AsyncReadStreamFileResult_data_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadStreamFileResult_data_get")]
		public static extern IntPtr AsyncReadStreamFileResult_data_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadStreamFileResult_offset_set")]
		public static extern void AsyncReadStreamFileResult_offset_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadStreamFileResult_offset_get")]
		public static extern int AsyncReadStreamFileResult_offset_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadStreamFileResult_try_read_length_set")]
		public static extern void AsyncReadStreamFileResult_try_read_length_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadStreamFileResult_try_read_length_get")]
		public static extern uint AsyncReadStreamFileResult_try_read_length_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncReadStreamFileResult__SWIG_1")]
		public static extern IntPtr new_AsyncReadStreamFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncReadStreamFileResult")]
		public static extern void delete_AsyncReadStreamFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncWriteStreamFileResult__SWIG_0")]
		public static extern IntPtr new_AsyncWriteStreamFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteStreamFileResult_filename_set")]
		public static extern void AsyncWriteStreamFileResult_filename_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteStreamFileResult_filename_get")]
		public static extern IntPtr AsyncWriteStreamFileResult_filename_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteStreamFileResult_offset_set")]
		public static extern void AsyncWriteStreamFileResult_offset_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteStreamFileResult_offset_get")]
		public static extern int AsyncWriteStreamFileResult_offset_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteStreamFileResult_try_write_length_set")]
		public static extern void AsyncWriteStreamFileResult_try_write_length_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteStreamFileResult_try_write_length_get")]
		public static extern uint AsyncWriteStreamFileResult_try_write_length_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteStreamFileResult_written_length_set")]
		public static extern void AsyncWriteStreamFileResult_written_length_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteStreamFileResult_written_length_get")]
		public static extern uint AsyncWriteStreamFileResult_written_length_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncWriteStreamFileResult__SWIG_1")]
		public static extern IntPtr new_AsyncWriteStreamFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncWriteStreamFileResult")]
		public static extern void delete_AsyncWriteStreamFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncListFileResult__SWIG_0")]
		public static extern IntPtr new_AsyncListFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncListFileResult_file_list_set")]
		public static extern void AsyncListFileResult_file_list_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncListFileResult_file_list_get")]
		public static extern IntPtr AsyncListFileResult_file_list_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncListFileResult_start_index_set")]
		public static extern void AsyncListFileResult_start_index_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncListFileResult_start_index_get")]
		public static extern uint AsyncListFileResult_start_index_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncListFileResult_try_list_file_num_set")]
		public static extern void AsyncListFileResult_try_list_file_num_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncListFileResult_try_list_file_num_get")]
		public static extern uint AsyncListFileResult_try_list_file_num_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncListFileResult_all_file_num_set")]
		public static extern void AsyncListFileResult_all_file_num_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncListFileResult_all_file_num_get")]
		public static extern uint AsyncListFileResult_all_file_num_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncListFileResult__SWIG_1")]
		public static extern IntPtr new_AsyncListFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncListFileResult")]
		public static extern void delete_AsyncListFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncRenameStreamFileResult__SWIG_0")]
		public static extern IntPtr new_AsyncRenameStreamFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRenameStreamFileResult_old_filename_set")]
		public static extern void AsyncRenameStreamFileResult_old_filename_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRenameStreamFileResult_old_filename_get")]
		public static extern IntPtr AsyncRenameStreamFileResult_old_filename_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRenameStreamFileResult_new_filename_set")]
		public static extern void AsyncRenameStreamFileResult_new_filename_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRenameStreamFileResult_new_filename_get")]
		public static extern IntPtr AsyncRenameStreamFileResult_new_filename_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncRenameStreamFileResult__SWIG_1")]
		public static extern IntPtr new_AsyncRenameStreamFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncRenameStreamFileResult")]
		public static extern void delete_AsyncRenameStreamFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncDeleteStreamFileResult__SWIG_0")]
		public static extern IntPtr new_AsyncDeleteStreamFileResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncDeleteStreamFileResult_filename_set")]
		public static extern void AsyncDeleteStreamFileResult_filename_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncDeleteStreamFileResult_filename_get")]
		public static extern IntPtr AsyncDeleteStreamFileResult_filename_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_AsyncDeleteStreamFileResult__SWIG_1")]
		public static extern IntPtr new_AsyncDeleteStreamFileResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_AsyncDeleteStreamFileResult")]
		public static extern void delete_AsyncDeleteStreamFileResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailVoiceCaptureOption__SWIG_0")]
		public static extern IntPtr new_RailVoiceCaptureOption__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceCaptureOption_voice_data_format_set")]
		public static extern void RailVoiceCaptureOption_voice_data_format_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceCaptureOption_voice_data_format_get")]
		public static extern int RailVoiceCaptureOption_voice_data_format_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailVoiceCaptureOption__SWIG_1")]
		public static extern IntPtr new_RailVoiceCaptureOption__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailVoiceCaptureOption")]
		public static extern void delete_RailVoiceCaptureOption(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailVoiceCaptureSpecification__SWIG_0")]
		public static extern IntPtr new_RailVoiceCaptureSpecification__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceCaptureSpecification_capture_format_set")]
		public static extern void RailVoiceCaptureSpecification_capture_format_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceCaptureSpecification_capture_format_get")]
		public static extern int RailVoiceCaptureSpecification_capture_format_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceCaptureSpecification_bits_per_sample_set")]
		public static extern void RailVoiceCaptureSpecification_bits_per_sample_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceCaptureSpecification_bits_per_sample_get")]
		public static extern uint RailVoiceCaptureSpecification_bits_per_sample_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceCaptureSpecification_samples_per_second_set")]
		public static extern void RailVoiceCaptureSpecification_samples_per_second_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceCaptureSpecification_samples_per_second_get")]
		public static extern uint RailVoiceCaptureSpecification_samples_per_second_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceCaptureSpecification_channels_set")]
		public static extern void RailVoiceCaptureSpecification_channels_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceCaptureSpecification_channels_get")]
		public static extern int RailVoiceCaptureSpecification_channels_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailVoiceCaptureSpecification__SWIG_1")]
		public static extern IntPtr new_RailVoiceCaptureSpecification__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailVoiceCaptureSpecification")]
		public static extern void delete_RailVoiceCaptureSpecification(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateVoiceChannelOption__SWIG_0")]
		public static extern IntPtr new_CreateVoiceChannelOption__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateVoiceChannelOption_join_channel_after_created_set")]
		public static extern void CreateVoiceChannelOption_join_channel_after_created_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateVoiceChannelOption_join_channel_after_created_get")]
		public static extern bool CreateVoiceChannelOption_join_channel_after_created_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateVoiceChannelOption__SWIG_1")]
		public static extern IntPtr new_CreateVoiceChannelOption__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CreateVoiceChannelOption")]
		public static extern void delete_CreateVoiceChannelOption(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailVoiceChannelUserSpeakingState__SWIG_0")]
		public static extern IntPtr new_RailVoiceChannelUserSpeakingState__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceChannelUserSpeakingState_user_id_set")]
		public static extern void RailVoiceChannelUserSpeakingState_user_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceChannelUserSpeakingState_user_id_get")]
		public static extern IntPtr RailVoiceChannelUserSpeakingState_user_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceChannelUserSpeakingState_speaking_limit_set")]
		public static extern void RailVoiceChannelUserSpeakingState_speaking_limit_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailVoiceChannelUserSpeakingState_speaking_limit_get")]
		public static extern int RailVoiceChannelUserSpeakingState_speaking_limit_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailVoiceChannelUserSpeakingState__SWIG_1")]
		public static extern IntPtr new_RailVoiceChannelUserSpeakingState__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailVoiceChannelUserSpeakingState")]
		public static extern void delete_RailVoiceChannelUserSpeakingState(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateVoiceChannelResult_voice_channel_id_set")]
		public static extern void CreateVoiceChannelResult_voice_channel_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateVoiceChannelResult_voice_channel_id_get")]
		public static extern IntPtr CreateVoiceChannelResult_voice_channel_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateVoiceChannelResult__SWIG_0")]
		public static extern IntPtr new_CreateVoiceChannelResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_CreateVoiceChannelResult__SWIG_1")]
		public static extern IntPtr new_CreateVoiceChannelResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_CreateVoiceChannelResult")]
		public static extern void delete_CreateVoiceChannelResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_JoinVoiceChannelResult_voice_channel_id_set")]
		public static extern void JoinVoiceChannelResult_voice_channel_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_JoinVoiceChannelResult_voice_channel_id_get")]
		public static extern IntPtr JoinVoiceChannelResult_voice_channel_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_JoinVoiceChannelResult_already_joined_channel_id_set")]
		public static extern void JoinVoiceChannelResult_already_joined_channel_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_JoinVoiceChannelResult_already_joined_channel_id_get")]
		public static extern IntPtr JoinVoiceChannelResult_already_joined_channel_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_JoinVoiceChannelResult__SWIG_0")]
		public static extern IntPtr new_JoinVoiceChannelResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_JoinVoiceChannelResult__SWIG_1")]
		public static extern IntPtr new_JoinVoiceChannelResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_JoinVoiceChannelResult")]
		public static extern void delete_JoinVoiceChannelResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaveVoiceChannelResult__SWIG_0")]
		public static extern IntPtr new_LeaveVoiceChannelResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaveVoiceChannelResult_voice_channel_id_set")]
		public static extern void LeaveVoiceChannelResult_voice_channel_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaveVoiceChannelResult_voice_channel_id_get")]
		public static extern IntPtr LeaveVoiceChannelResult_voice_channel_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaveVoiceChannelResult_reason_set")]
		public static extern void LeaveVoiceChannelResult_reason_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaveVoiceChannelResult_reason_get")]
		public static extern int LeaveVoiceChannelResult_reason_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaveVoiceChannelResult__SWIG_1")]
		public static extern IntPtr new_LeaveVoiceChannelResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_LeaveVoiceChannelResult")]
		public static extern void delete_LeaveVoiceChannelResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelAddUsersResult_voice_channel_id_set")]
		public static extern void VoiceChannelAddUsersResult_voice_channel_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelAddUsersResult_voice_channel_id_get")]
		public static extern IntPtr VoiceChannelAddUsersResult_voice_channel_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelAddUsersResult_success_ids_set")]
		public static extern void VoiceChannelAddUsersResult_success_ids_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelAddUsersResult_success_ids_get")]
		public static extern IntPtr VoiceChannelAddUsersResult_success_ids_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelAddUsersResult_failed_ids_set")]
		public static extern void VoiceChannelAddUsersResult_failed_ids_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelAddUsersResult_failed_ids_get")]
		public static extern IntPtr VoiceChannelAddUsersResult_failed_ids_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelAddUsersResult__SWIG_0")]
		public static extern IntPtr new_VoiceChannelAddUsersResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelAddUsersResult__SWIG_1")]
		public static extern IntPtr new_VoiceChannelAddUsersResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_VoiceChannelAddUsersResult")]
		public static extern void delete_VoiceChannelAddUsersResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelRemoveUsersResult_voice_channel_id_set")]
		public static extern void VoiceChannelRemoveUsersResult_voice_channel_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelRemoveUsersResult_voice_channel_id_get")]
		public static extern IntPtr VoiceChannelRemoveUsersResult_voice_channel_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelRemoveUsersResult_success_ids_set")]
		public static extern void VoiceChannelRemoveUsersResult_success_ids_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelRemoveUsersResult_success_ids_get")]
		public static extern IntPtr VoiceChannelRemoveUsersResult_success_ids_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelRemoveUsersResult_failed_ids_set")]
		public static extern void VoiceChannelRemoveUsersResult_failed_ids_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelRemoveUsersResult_failed_ids_get")]
		public static extern IntPtr VoiceChannelRemoveUsersResult_failed_ids_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelRemoveUsersResult__SWIG_0")]
		public static extern IntPtr new_VoiceChannelRemoveUsersResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelRemoveUsersResult__SWIG_1")]
		public static extern IntPtr new_VoiceChannelRemoveUsersResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_VoiceChannelRemoveUsersResult")]
		public static extern void delete_VoiceChannelRemoveUsersResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelInviteEvent_inviter_name_set")]
		public static extern void VoiceChannelInviteEvent_inviter_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelInviteEvent_inviter_name_get")]
		public static extern IntPtr VoiceChannelInviteEvent_inviter_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelInviteEvent_channel_name_set")]
		public static extern void VoiceChannelInviteEvent_channel_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelInviteEvent_channel_name_get")]
		public static extern IntPtr VoiceChannelInviteEvent_channel_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelInviteEvent_voice_channel_id_set")]
		public static extern void VoiceChannelInviteEvent_voice_channel_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelInviteEvent_voice_channel_id_get")]
		public static extern IntPtr VoiceChannelInviteEvent_voice_channel_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelInviteEvent__SWIG_0")]
		public static extern IntPtr new_VoiceChannelInviteEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelInviteEvent__SWIG_1")]
		public static extern IntPtr new_VoiceChannelInviteEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_VoiceChannelInviteEvent")]
		public static extern void delete_VoiceChannelInviteEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelMemeberChangedEvent_voice_channel_id_set")]
		public static extern void VoiceChannelMemeberChangedEvent_voice_channel_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelMemeberChangedEvent_voice_channel_id_get")]
		public static extern IntPtr VoiceChannelMemeberChangedEvent_voice_channel_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelMemeberChangedEvent_member_ids_set")]
		public static extern void VoiceChannelMemeberChangedEvent_member_ids_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelMemeberChangedEvent_member_ids_get")]
		public static extern IntPtr VoiceChannelMemeberChangedEvent_member_ids_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelMemeberChangedEvent__SWIG_0")]
		public static extern IntPtr new_VoiceChannelMemeberChangedEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelMemeberChangedEvent__SWIG_1")]
		public static extern IntPtr new_VoiceChannelMemeberChangedEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_VoiceChannelMemeberChangedEvent")]
		public static extern void delete_VoiceChannelMemeberChangedEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelPushToTalkKeyChangedEvent__SWIG_0")]
		public static extern IntPtr new_VoiceChannelPushToTalkKeyChangedEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelPushToTalkKeyChangedEvent_push_to_talk_hot_key_set")]
		public static extern void VoiceChannelPushToTalkKeyChangedEvent_push_to_talk_hot_key_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelPushToTalkKeyChangedEvent_push_to_talk_hot_key_get")]
		public static extern uint VoiceChannelPushToTalkKeyChangedEvent_push_to_talk_hot_key_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelPushToTalkKeyChangedEvent__SWIG_1")]
		public static extern IntPtr new_VoiceChannelPushToTalkKeyChangedEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_VoiceChannelPushToTalkKeyChangedEvent")]
		public static extern void delete_VoiceChannelPushToTalkKeyChangedEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelUsersSpeakingStateChangedEvent_voice_channel_id_set")]
		public static extern void VoiceChannelUsersSpeakingStateChangedEvent_voice_channel_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelUsersSpeakingStateChangedEvent_voice_channel_id_get")]
		public static extern IntPtr VoiceChannelUsersSpeakingStateChangedEvent_voice_channel_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelUsersSpeakingStateChangedEvent_users_speaking_state_set")]
		public static extern void VoiceChannelUsersSpeakingStateChangedEvent_users_speaking_state_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelUsersSpeakingStateChangedEvent_users_speaking_state_get")]
		public static extern IntPtr VoiceChannelUsersSpeakingStateChangedEvent_users_speaking_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelUsersSpeakingStateChangedEvent__SWIG_0")]
		public static extern IntPtr new_VoiceChannelUsersSpeakingStateChangedEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelUsersSpeakingStateChangedEvent__SWIG_1")]
		public static extern IntPtr new_VoiceChannelUsersSpeakingStateChangedEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_VoiceChannelUsersSpeakingStateChangedEvent")]
		public static extern void delete_VoiceChannelUsersSpeakingStateChangedEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelSpeakingUsersChangedEvent_voice_channel_id_set")]
		public static extern void VoiceChannelSpeakingUsersChangedEvent_voice_channel_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelSpeakingUsersChangedEvent_voice_channel_id_get")]
		public static extern IntPtr VoiceChannelSpeakingUsersChangedEvent_voice_channel_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelSpeakingUsersChangedEvent_speaking_users_set")]
		public static extern void VoiceChannelSpeakingUsersChangedEvent_speaking_users_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelSpeakingUsersChangedEvent_speaking_users_get")]
		public static extern IntPtr VoiceChannelSpeakingUsersChangedEvent_speaking_users_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelSpeakingUsersChangedEvent_not_speaking_users_set")]
		public static extern void VoiceChannelSpeakingUsersChangedEvent_not_speaking_users_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelSpeakingUsersChangedEvent_not_speaking_users_get")]
		public static extern IntPtr VoiceChannelSpeakingUsersChangedEvent_not_speaking_users_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelSpeakingUsersChangedEvent__SWIG_0")]
		public static extern IntPtr new_VoiceChannelSpeakingUsersChangedEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceChannelSpeakingUsersChangedEvent__SWIG_1")]
		public static extern IntPtr new_VoiceChannelSpeakingUsersChangedEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_VoiceChannelSpeakingUsersChangedEvent")]
		public static extern void delete_VoiceChannelSpeakingUsersChangedEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceDataCapturedEvent__SWIG_0")]
		public static extern IntPtr new_VoiceDataCapturedEvent__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceDataCapturedEvent_is_last_package_set")]
		public static extern void VoiceDataCapturedEvent_is_last_package_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceDataCapturedEvent_is_last_package_get")]
		public static extern bool VoiceDataCapturedEvent_is_last_package_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_VoiceDataCapturedEvent__SWIG_1")]
		public static extern IntPtr new_VoiceDataCapturedEvent__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_VoiceDataCapturedEvent")]
		public static extern void delete_VoiceDataCapturedEvent(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailTextInputWindowOption__SWIG_0")]
		public static extern IntPtr new_RailTextInputWindowOption__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_show_password_input_set")]
		public static extern void RailTextInputWindowOption_show_password_input_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_show_password_input_get")]
		public static extern bool RailTextInputWindowOption_show_password_input_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_enable_multi_line_edit_set")]
		public static extern void RailTextInputWindowOption_enable_multi_line_edit_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_enable_multi_line_edit_get")]
		public static extern bool RailTextInputWindowOption_enable_multi_line_edit_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_auto_cancel_set")]
		public static extern void RailTextInputWindowOption_auto_cancel_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_auto_cancel_get")]
		public static extern bool RailTextInputWindowOption_auto_cancel_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_caption_text_set")]
		public static extern void RailTextInputWindowOption_caption_text_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_caption_text_get")]
		public static extern IntPtr RailTextInputWindowOption_caption_text_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_description_set")]
		public static extern void RailTextInputWindowOption_description_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_description_get")]
		public static extern IntPtr RailTextInputWindowOption_description_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_content_placeholder_set")]
		public static extern void RailTextInputWindowOption_content_placeholder_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_content_placeholder_get")]
		public static extern IntPtr RailTextInputWindowOption_content_placeholder_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_is_min_window_set")]
		public static extern void RailTextInputWindowOption_is_min_window_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_is_min_window_get")]
		public static extern bool RailTextInputWindowOption_is_min_window_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_position_left_set")]
		public static extern void RailTextInputWindowOption_position_left_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_position_left_get")]
		public static extern uint RailTextInputWindowOption_position_left_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_position_top_set")]
		public static extern void RailTextInputWindowOption_position_top_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputWindowOption_position_top_get")]
		public static extern uint RailTextInputWindowOption_position_top_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailTextInputWindowOption__SWIG_1")]
		public static extern IntPtr new_RailTextInputWindowOption__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailTextInputWindowOption")]
		public static extern void delete_RailTextInputWindowOption(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailTextInputResult__SWIG_0")]
		public static extern IntPtr new_RailTextInputResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputResult_content_set")]
		public static extern void RailTextInputResult_content_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputResult_content_get")]
		public static extern IntPtr RailTextInputResult_content_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailTextInputResult__SWIG_1")]
		public static extern IntPtr new_RailTextInputResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailTextInputResult")]
		public static extern void delete_RailTextInputResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_kRailMaxGameDefinePlayingStateValue_get")]
		public static extern uint kRailMaxGameDefinePlayingStateValue_get();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailBranchInfo__SWIG_0")]
		public static extern IntPtr new_RailBranchInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailBranchInfo_branch_name_set")]
		public static extern void RailBranchInfo_branch_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailBranchInfo_branch_name_get")]
		public static extern IntPtr RailBranchInfo_branch_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailBranchInfo_branch_type_set")]
		public static extern void RailBranchInfo_branch_type_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailBranchInfo_branch_type_get")]
		public static extern IntPtr RailBranchInfo_branch_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailBranchInfo_branch_id_set")]
		public static extern void RailBranchInfo_branch_id_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailBranchInfo_branch_id_get")]
		public static extern IntPtr RailBranchInfo_branch_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailBranchInfo_build_number_set")]
		public static extern void RailBranchInfo_build_number_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailBranchInfo_build_number_get")]
		public static extern IntPtr RailBranchInfo_build_number_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailBranchInfo__SWIG_1")]
		public static extern IntPtr new_RailBranchInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailBranchInfo")]
		public static extern void delete_RailBranchInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGameDefineGamePlayingState__SWIG_0")]
		public static extern IntPtr new_RailGameDefineGamePlayingState__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameDefineGamePlayingState_game_define_game_playing_state_set")]
		public static extern void RailGameDefineGamePlayingState_game_define_game_playing_state_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameDefineGamePlayingState_game_define_game_playing_state_get")]
		public static extern uint RailGameDefineGamePlayingState_game_define_game_playing_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameDefineGamePlayingState_state_name_zh_cn_set")]
		public static extern void RailGameDefineGamePlayingState_state_name_zh_cn_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameDefineGamePlayingState_state_name_zh_cn_get")]
		public static extern IntPtr RailGameDefineGamePlayingState_state_name_zh_cn_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameDefineGamePlayingState_state_name_en_us_set")]
		public static extern void RailGameDefineGamePlayingState_state_name_en_us_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameDefineGamePlayingState_state_name_en_us_get")]
		public static extern IntPtr RailGameDefineGamePlayingState_state_name_en_us_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGameDefineGamePlayingState__SWIG_1")]
		public static extern IntPtr new_RailGameDefineGamePlayingState__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailGameDefineGamePlayingState")]
		public static extern void delete_RailGameDefineGamePlayingState(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_QuerySubscribeWishPlayStateResult__SWIG_0")]
		public static extern IntPtr new_QuerySubscribeWishPlayStateResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_QuerySubscribeWishPlayStateResult_is_subscribed_set")]
		public static extern void QuerySubscribeWishPlayStateResult_is_subscribed_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_QuerySubscribeWishPlayStateResult_is_subscribed_get")]
		public static extern bool QuerySubscribeWishPlayStateResult_is_subscribed_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_QuerySubscribeWishPlayStateResult__SWIG_1")]
		public static extern IntPtr new_QuerySubscribeWishPlayStateResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_QuerySubscribeWishPlayStateResult")]
		public static extern void delete_QuerySubscribeWishPlayStateResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailWindowPosition__SWIG_0")]
		public static extern IntPtr new_RailWindowPosition__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailWindowPosition_position_left_set")]
		public static extern void RailWindowPosition_position_left_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailWindowPosition_position_left_get")]
		public static extern uint RailWindowPosition_position_left_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailWindowPosition_position_top_set")]
		public static extern void RailWindowPosition_position_top_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailWindowPosition_position_top_get")]
		public static extern uint RailWindowPosition_position_top_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailWindowPosition__SWIG_1")]
		public static extern IntPtr new_RailWindowPosition__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailWindowPosition")]
		public static extern void delete_RailWindowPosition(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailTextInputImeWindowOption__SWIG_0")]
		public static extern IntPtr new_RailTextInputImeWindowOption__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputImeWindowOption_position_set")]
		public static extern void RailTextInputImeWindowOption_position_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputImeWindowOption_position_get")]
		public static extern IntPtr RailTextInputImeWindowOption_position_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputImeWindowOption_show_rail_ime_window_set")]
		public static extern void RailTextInputImeWindowOption_show_rail_ime_window_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputImeWindowOption_show_rail_ime_window_get")]
		public static extern bool RailTextInputImeWindowOption_show_rail_ime_window_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailTextInputImeWindowOption__SWIG_1")]
		public static extern IntPtr new_RailTextInputImeWindowOption__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailTextInputImeWindowOption")]
		public static extern void delete_RailTextInputImeWindowOption(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailIMEHelperTextInputCompositionState__SWIG_0")]
		public static extern IntPtr new_RailIMEHelperTextInputCompositionState__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailIMEHelperTextInputCompositionState_composition_text_set")]
		public static extern void RailIMEHelperTextInputCompositionState_composition_text_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailIMEHelperTextInputCompositionState_composition_text_get")]
		public static extern IntPtr RailIMEHelperTextInputCompositionState_composition_text_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailIMEHelperTextInputCompositionState_composition_state_set")]
		public static extern void RailIMEHelperTextInputCompositionState_composition_state_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailIMEHelperTextInputCompositionState_composition_state_get")]
		public static extern int RailIMEHelperTextInputCompositionState_composition_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailIMEHelperTextInputCompositionState__SWIG_1")]
		public static extern IntPtr new_RailIMEHelperTextInputCompositionState__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailIMEHelperTextInputCompositionState")]
		public static extern void delete_RailIMEHelperTextInputCompositionState(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailIMEHelperTextInputSelectedResult__SWIG_0")]
		public static extern IntPtr new_RailIMEHelperTextInputSelectedResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailIMEHelperTextInputSelectedResult_content_set")]
		public static extern void RailIMEHelperTextInputSelectedResult_content_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailIMEHelperTextInputSelectedResult_content_get")]
		public static extern IntPtr RailIMEHelperTextInputSelectedResult_content_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailIMEHelperTextInputSelectedResult__SWIG_1")]
		public static extern IntPtr new_RailIMEHelperTextInputSelectedResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailIMEHelperTextInputSelectedResult")]
		public static extern void delete_RailIMEHelperTextInputSelectedResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailHttpSessionResponse__SWIG_0")]
		public static extern IntPtr new_RailHttpSessionResponse__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailHttpSessionResponse_http_response_data_set")]
		public static extern void RailHttpSessionResponse_http_response_data_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailHttpSessionResponse_http_response_data_get")]
		public static extern IntPtr RailHttpSessionResponse_http_response_data_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailHttpSessionResponse__SWIG_1")]
		public static extern IntPtr new_RailHttpSessionResponse__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailHttpSessionResponse")]
		public static extern void delete_RailHttpSessionResponse(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSmallObjectState__SWIG_0")]
		public static extern IntPtr new_RailSmallObjectState__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectState_update_state_set")]
		public static extern void RailSmallObjectState_update_state_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectState_update_state_get")]
		public static extern int RailSmallObjectState_update_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectState_index_set")]
		public static extern void RailSmallObjectState_index_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectState_index_get")]
		public static extern uint RailSmallObjectState_index_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSmallObjectState__SWIG_1")]
		public static extern IntPtr new_RailSmallObjectState__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSmallObjectState")]
		public static extern void delete_RailSmallObjectState(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSmallObjectDownloadInfo__SWIG_0")]
		public static extern IntPtr new_RailSmallObjectDownloadInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectDownloadInfo_index_set")]
		public static extern void RailSmallObjectDownloadInfo_index_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectDownloadInfo_index_get")]
		public static extern uint RailSmallObjectDownloadInfo_index_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectDownloadInfo_result_set")]
		public static extern void RailSmallObjectDownloadInfo_result_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectDownloadInfo_result_get")]
		public static extern int RailSmallObjectDownloadInfo_result_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSmallObjectDownloadInfo__SWIG_1")]
		public static extern IntPtr new_RailSmallObjectDownloadInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSmallObjectDownloadInfo")]
		public static extern void delete_RailSmallObjectDownloadInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectStateQueryResult_objects_state_set")]
		public static extern void RailSmallObjectStateQueryResult_objects_state_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectStateQueryResult_objects_state_get")]
		public static extern IntPtr RailSmallObjectStateQueryResult_objects_state_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSmallObjectStateQueryResult__SWIG_0")]
		public static extern IntPtr new_RailSmallObjectStateQueryResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSmallObjectStateQueryResult__SWIG_1")]
		public static extern IntPtr new_RailSmallObjectStateQueryResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSmallObjectStateQueryResult")]
		public static extern void delete_RailSmallObjectStateQueryResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectDownloadResult_download_infos_set")]
		public static extern void RailSmallObjectDownloadResult_download_infos_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectDownloadResult_download_infos_get")]
		public static extern IntPtr RailSmallObjectDownloadResult_download_infos_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSmallObjectDownloadResult__SWIG_0")]
		public static extern IntPtr new_RailSmallObjectDownloadResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSmallObjectDownloadResult__SWIG_1")]
		public static extern IntPtr new_RailSmallObjectDownloadResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSmallObjectDownloadResult")]
		public static extern void delete_RailSmallObjectDownloadResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSwitchPlayerSelectedZoneResult__SWIG_0")]
		public static extern IntPtr new_RailSwitchPlayerSelectedZoneResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailSwitchPlayerSelectedZoneResult__SWIG_1")]
		public static extern IntPtr new_RailSwitchPlayerSelectedZoneResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailSwitchPlayerSelectedZoneResult")]
		public static extern void delete_RailSwitchPlayerSelectedZoneResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGroupInfo__SWIG_0")]
		public static extern IntPtr new_RailGroupInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGroupInfo_group_id_set")]
		public static extern void RailGroupInfo_group_id_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGroupInfo_group_id_get")]
		public static extern IntPtr RailGroupInfo_group_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGroupInfo_group_name_set")]
		public static extern void RailGroupInfo_group_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGroupInfo_group_name_get")]
		public static extern IntPtr RailGroupInfo_group_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGroupInfo_group_icon_url_set")]
		public static extern void RailGroupInfo_group_icon_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGroupInfo_group_icon_url_get")]
		public static extern IntPtr RailGroupInfo_group_icon_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailGroupInfo__SWIG_1")]
		public static extern IntPtr new_RailGroupInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailGroupInfo")]
		public static extern void delete_RailGroupInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailQueryGroupsInfoResult__SWIG_0")]
		public static extern IntPtr new_RailQueryGroupsInfoResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryGroupsInfoResult_group_ids_set")]
		public static extern void RailQueryGroupsInfoResult_group_ids_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryGroupsInfoResult_group_ids_get")]
		public static extern IntPtr RailQueryGroupsInfoResult_group_ids_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailQueryGroupsInfoResult__SWIG_1")]
		public static extern IntPtr new_RailQueryGroupsInfoResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailQueryGroupsInfoResult")]
		public static extern void delete_RailQueryGroupsInfoResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailOpenGroupChatResult__SWIG_0")]
		public static extern IntPtr new_RailOpenGroupChatResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailOpenGroupChatResult__SWIG_1")]
		public static extern IntPtr new_RailOpenGroupChatResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailOpenGroupChatResult")]
		public static extern void delete_RailOpenGroupChatResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailQueryGameOnlineTimeResult__SWIG_0")]
		public static extern IntPtr new_RailQueryGameOnlineTimeResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryGameOnlineTimeResult_game_online_time_seconds_set")]
		public static extern void RailQueryGameOnlineTimeResult_game_online_time_seconds_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryGameOnlineTimeResult_game_online_time_seconds_get")]
		public static extern uint RailQueryGameOnlineTimeResult_game_online_time_seconds_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailQueryGameOnlineTimeResult__SWIG_1")]
		public static extern IntPtr new_RailQueryGameOnlineTimeResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailQueryGameOnlineTimeResult")]
		public static extern void delete_RailQueryGameOnlineTimeResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailCustomizeAntiAddictionActions__SWIG_0")]
		public static extern IntPtr new_RailCustomizeAntiAddictionActions__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCustomizeAntiAddictionActions_anti_addiction_actions_set")]
		public static extern void RailCustomizeAntiAddictionActions_anti_addiction_actions_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCustomizeAntiAddictionActions_anti_addiction_actions_get")]
		public static extern IntPtr RailCustomizeAntiAddictionActions_anti_addiction_actions_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailCustomizeAntiAddictionActions__SWIG_1")]
		public static extern IntPtr new_RailCustomizeAntiAddictionActions__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailCustomizeAntiAddictionActions")]
		public static extern void delete_RailCustomizeAntiAddictionActions(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailThirdPartyAccountInfo__SWIG_0")]
		public static extern IntPtr new_RailThirdPartyAccountInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_error_code_set")]
		public static extern void RailThirdPartyAccountInfo_error_code_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_error_code_get")]
		public static extern uint RailThirdPartyAccountInfo_error_code_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_error_msg_set")]
		public static extern void RailThirdPartyAccountInfo_error_msg_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_error_msg_get")]
		public static extern IntPtr RailThirdPartyAccountInfo_error_msg_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_open_id_set")]
		public static extern void RailThirdPartyAccountInfo_open_id_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_open_id_get")]
		public static extern IntPtr RailThirdPartyAccountInfo_open_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_token_set")]
		public static extern void RailThirdPartyAccountInfo_token_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_token_get")]
		public static extern IntPtr RailThirdPartyAccountInfo_token_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_token_expire_time_set")]
		public static extern void RailThirdPartyAccountInfo_token_expire_time_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_token_expire_time_get")]
		public static extern uint RailThirdPartyAccountInfo_token_expire_time_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_channel_set")]
		public static extern void RailThirdPartyAccountInfo_channel_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_channel_get")]
		public static extern IntPtr RailThirdPartyAccountInfo_channel_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_pf_set")]
		public static extern void RailThirdPartyAccountInfo_pf_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_pf_get")]
		public static extern IntPtr RailThirdPartyAccountInfo_pf_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_user_name_set")]
		public static extern void RailThirdPartyAccountInfo_user_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_user_name_get")]
		public static extern IntPtr RailThirdPartyAccountInfo_user_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_real_name_auth_set")]
		public static extern void RailThirdPartyAccountInfo_real_name_auth_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountInfo_real_name_auth_get")]
		public static extern bool RailThirdPartyAccountInfo_real_name_auth_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailThirdPartyAccountInfo__SWIG_1")]
		public static extern IntPtr new_RailThirdPartyAccountInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailThirdPartyAccountInfo")]
		public static extern void delete_RailThirdPartyAccountInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailThirdPartyAccountLoginOptions__SWIG_0")]
		public static extern IntPtr new_RailThirdPartyAccountLoginOptions__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountLoginOptions_account_type_set")]
		public static extern void RailThirdPartyAccountLoginOptions_account_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountLoginOptions_account_type_get")]
		public static extern int RailThirdPartyAccountLoginOptions_account_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountLoginOptions_code_set")]
		public static extern void RailThirdPartyAccountLoginOptions_code_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountLoginOptions_code_get")]
		public static extern IntPtr RailThirdPartyAccountLoginOptions_code_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailThirdPartyAccountLoginOptions__SWIG_1")]
		public static extern IntPtr new_RailThirdPartyAccountLoginOptions__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailThirdPartyAccountLoginOptions")]
		public static extern void delete_RailThirdPartyAccountLoginOptions(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailThirdPartyAccountLoginResult__SWIG_0")]
		public static extern IntPtr new_RailThirdPartyAccountLoginResult__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountLoginResult_account_info_set")]
		public static extern void RailThirdPartyAccountLoginResult_account_info_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountLoginResult_account_info_get")]
		public static extern IntPtr RailThirdPartyAccountLoginResult_account_info_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailThirdPartyAccountLoginResult__SWIG_1")]
		public static extern IntPtr new_RailThirdPartyAccountLoginResult__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailThirdPartyAccountLoginResult")]
		public static extern void delete_RailThirdPartyAccountLoginResult(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailNotifyThirdPartyAccountQrCodeInfo__SWIG_0")]
		public static extern IntPtr new_RailNotifyThirdPartyAccountQrCodeInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNotifyThirdPartyAccountQrCodeInfo_qr_code_url_set")]
		public static extern void RailNotifyThirdPartyAccountQrCodeInfo_qr_code_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNotifyThirdPartyAccountQrCodeInfo_qr_code_url_get")]
		public static extern IntPtr RailNotifyThirdPartyAccountQrCodeInfo_qr_code_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailNotifyThirdPartyAccountQrCodeInfo__SWIG_1")]
		public static extern IntPtr new_RailNotifyThirdPartyAccountQrCodeInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailNotifyThirdPartyAccountQrCodeInfo")]
		public static extern void delete_RailNotifyThirdPartyAccountQrCodeInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_api_name_set")]
		public static extern void RailPlayerAchievementInfo_api_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_api_name_get")]
		public static extern IntPtr RailPlayerAchievementInfo_api_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_display_name_set")]
		public static extern void RailPlayerAchievementInfo_display_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_display_name_get")]
		public static extern IntPtr RailPlayerAchievementInfo_display_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_description_set")]
		public static extern void RailPlayerAchievementInfo_description_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_description_get")]
		public static extern IntPtr RailPlayerAchievementInfo_description_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_achieved_icon_url_set")]
		public static extern void RailPlayerAchievementInfo_achieved_icon_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_achieved_icon_url_get")]
		public static extern IntPtr RailPlayerAchievementInfo_achieved_icon_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_unachieved_icon_url_set")]
		public static extern void RailPlayerAchievementInfo_unachieved_icon_url_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_unachieved_icon_url_get")]
		public static extern IntPtr RailPlayerAchievementInfo_unachieved_icon_url_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_is_process_achievement_set")]
		public static extern void RailPlayerAchievementInfo_is_process_achievement_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_is_process_achievement_get")]
		public static extern bool RailPlayerAchievementInfo_is_process_achievement_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_is_achieved_set")]
		public static extern void RailPlayerAchievementInfo_is_achieved_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_is_achieved_get")]
		public static extern bool RailPlayerAchievementInfo_is_achieved_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_current_process_value_set")]
		public static extern void RailPlayerAchievementInfo_current_process_value_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_current_process_value_get")]
		public static extern uint RailPlayerAchievementInfo_current_process_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_unlock_process_value_set")]
		public static extern void RailPlayerAchievementInfo_unlock_process_value_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_unlock_process_value_get")]
		public static extern uint RailPlayerAchievementInfo_unlock_process_value_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_unlock_time_in_seconds_set")]
		public static extern void RailPlayerAchievementInfo_unlock_time_in_seconds_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlayerAchievementInfo_unlock_time_in_seconds_get")]
		public static extern uint RailPlayerAchievementInfo_unlock_time_in_seconds_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPlayerAchievementInfo__SWIG_0")]
		public static extern IntPtr new_RailPlayerAchievementInfo__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RailPlayerAchievementInfo__SWIG_1")]
		public static extern IntPtr new_RailPlayerAchievementInfo__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RailPlayerAchievementInfo")]
		public static extern void delete_RailPlayerAchievementInfo(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAchievementHelper_CreatePlayerAchievement")]
		public static extern IntPtr IRailAchievementHelper_CreatePlayerAchievement(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAchievementHelper_GetGlobalAchievement")]
		public static extern IntPtr IRailAchievementHelper_GetGlobalAchievement(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailAchievementHelper")]
		public static extern void delete_IRailAchievementHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_GetRailID")]
		public static extern IntPtr IRailPlayerAchievement_GetRailID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_AsyncRequestAchievement")]
		public static extern int IRailPlayerAchievement_AsyncRequestAchievement(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_HasAchieved")]
		public static extern int IRailPlayerAchievement_HasAchieved(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, out bool jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_GetAchievementInfo__SWIG_0")]
		public static extern int IRailPlayerAchievement_GetAchievementInfo__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_AsyncTriggerAchievementProgress__SWIG_0")]
		public static extern int IRailPlayerAchievement_AsyncTriggerAchievementProgress__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, uint jarg3, uint jarg4, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_AsyncTriggerAchievementProgress__SWIG_1")]
		public static extern int IRailPlayerAchievement_AsyncTriggerAchievementProgress__SWIG_1(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, uint jarg3, uint jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_AsyncTriggerAchievementProgress__SWIG_2")]
		public static extern int IRailPlayerAchievement_AsyncTriggerAchievementProgress__SWIG_2(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_MakeAchievement")]
		public static extern int IRailPlayerAchievement_MakeAchievement(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_CancelAchievement")]
		public static extern int IRailPlayerAchievement_CancelAchievement(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_AsyncStoreAchievement")]
		public static extern int IRailPlayerAchievement_AsyncStoreAchievement(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_ResetAllAchievements")]
		public static extern int IRailPlayerAchievement_ResetAllAchievements(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_GetAllAchievementsName")]
		public static extern int IRailPlayerAchievement_GetAllAchievementsName(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_GetAchievementInfo__SWIG_1")]
		public static extern int IRailPlayerAchievement_GetAchievementInfo__SWIG_1(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailPlayerAchievement")]
		public static extern void delete_IRailPlayerAchievement(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGlobalAchievement_AsyncRequestAchievement")]
		public static extern int IRailGlobalAchievement_AsyncRequestAchievement(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGlobalAchievement_GetGlobalAchievedPercent")]
		public static extern int IRailGlobalAchievement_GetGlobalAchievedPercent(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, out double jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGlobalAchievement_GetGlobalAchievedPercentDescending")]
		public static extern int IRailGlobalAchievement_GetGlobalAchievedPercentDescending(IntPtr jarg1, int jarg2, IntPtr jarg3, out double jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailGlobalAchievement")]
		public static extern void delete_IRailGlobalAchievement(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PlayerAchievementReceived__SWIG_0")]
		public static extern IntPtr new_PlayerAchievementReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PlayerAchievementReceived__SWIG_1")]
		public static extern IntPtr new_PlayerAchievementReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_PlayerAchievementReceived")]
		public static extern void delete_PlayerAchievementReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PlayerAchievementStored__SWIG_0")]
		public static extern IntPtr new_PlayerAchievementStored__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerAchievementStored_group_achievement_set")]
		public static extern void PlayerAchievementStored_group_achievement_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerAchievementStored_group_achievement_get")]
		public static extern bool PlayerAchievementStored_group_achievement_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerAchievementStored_achievement_name_set")]
		public static extern void PlayerAchievementStored_achievement_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerAchievementStored_achievement_name_get")]
		public static extern IntPtr PlayerAchievementStored_achievement_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerAchievementStored_current_progress_set")]
		public static extern void PlayerAchievementStored_current_progress_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerAchievementStored_current_progress_get")]
		public static extern uint PlayerAchievementStored_current_progress_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerAchievementStored_max_progress_set")]
		public static extern void PlayerAchievementStored_max_progress_set(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerAchievementStored_max_progress_get")]
		public static extern uint PlayerAchievementStored_max_progress_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PlayerAchievementStored__SWIG_1")]
		public static extern IntPtr new_PlayerAchievementStored__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_PlayerAchievementStored")]
		public static extern void delete_PlayerAchievementStored(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GlobalAchievementReceived__SWIG_0")]
		public static extern IntPtr new_GlobalAchievementReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_GlobalAchievementReceived_count_set")]
		public static extern void GlobalAchievementReceived_count_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_GlobalAchievementReceived_count_get")]
		public static extern int GlobalAchievementReceived_count_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GlobalAchievementReceived__SWIG_1")]
		public static extern IntPtr new_GlobalAchievementReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GlobalAchievementReceived")]
		public static extern void delete_GlobalAchievementReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssetsHelper_OpenAssets")]
		public static extern IntPtr IRailAssetsHelper_OpenAssets(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssetsHelper_OpenGameServerAssets")]
		public static extern IntPtr IRailAssetsHelper_OpenGameServerAssets(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailAssetsHelper")]
		public static extern void delete_IRailAssetsHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_AsyncRequestAllAssets")]
		public static extern int IRailAssets_AsyncRequestAllAssets(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_QueryAssetInfo")]
		public static extern int IRailAssets_QueryAssetInfo(IntPtr jarg1, ulong jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_AsyncUpdateAssetsProperty")]
		public static extern int IRailAssets_AsyncUpdateAssetsProperty(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_AsyncDirectConsumeAssets")]
		public static extern int IRailAssets_AsyncDirectConsumeAssets(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_AsyncStartConsumeAsset")]
		public static extern int IRailAssets_AsyncStartConsumeAsset(IntPtr jarg1, ulong jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_AsyncUpdateConsumeProgress")]
		public static extern int IRailAssets_AsyncUpdateConsumeProgress(IntPtr jarg1, ulong jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_AsyncCompleteConsumeAsset")]
		public static extern int IRailAssets_AsyncCompleteConsumeAsset(IntPtr jarg1, ulong jarg2, uint jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_AsyncExchangeAssets")]
		public static extern int IRailAssets_AsyncExchangeAssets(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_AsyncExchangeAssetsTo")]
		public static extern int IRailAssets_AsyncExchangeAssetsTo(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, ulong jarg4, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_AsyncSplitAsset")]
		public static extern int IRailAssets_AsyncSplitAsset(IntPtr jarg1, ulong jarg2, uint jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_AsyncSplitAssetTo")]
		public static extern int IRailAssets_AsyncSplitAssetTo(IntPtr jarg1, ulong jarg2, uint jarg3, ulong jarg4, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_AsyncMergeAsset")]
		public static extern int IRailAssets_AsyncMergeAsset(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_AsyncMergeAssetTo")]
		public static extern int IRailAssets_AsyncMergeAssetTo(IntPtr jarg1, IntPtr jarg2, ulong jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_SerializeAssetsToBuffer__SWIG_0")]
		public static extern int IRailAssets_SerializeAssetsToBuffer__SWIG_0(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_SerializeAssetsToBuffer__SWIG_1")]
		public static extern int IRailAssets_SerializeAssetsToBuffer__SWIG_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_DeserializeAssetsFromBuffer")]
		public static extern int IRailAssets_DeserializeAssetsFromBuffer(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3, IntPtr jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailAssets")]
		public static extern void delete_IRailAssets(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserHelper_AsyncCreateBrowser__SWIG_0")]
		public static extern IntPtr IRailBrowserHelper_AsyncCreateBrowser__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, uint jarg3, uint jarg4, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg5, IntPtr jarg6, out RailResult jarg7);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserHelper_AsyncCreateBrowser__SWIG_1")]
		public static extern IntPtr IRailBrowserHelper_AsyncCreateBrowser__SWIG_1(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, uint jarg3, uint jarg4, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg5, IntPtr jarg6);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserHelper_AsyncCreateBrowser__SWIG_2")]
		public static extern IntPtr IRailBrowserHelper_AsyncCreateBrowser__SWIG_2(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, uint jarg3, uint jarg4, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserHelper_CreateCustomerDrawBrowser__SWIG_0")]
		public static extern IntPtr IRailBrowserHelper_CreateCustomerDrawBrowser__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3, IntPtr jarg4, out RailResult jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserHelper_CreateCustomerDrawBrowser__SWIG_1")]
		public static extern IntPtr IRailBrowserHelper_CreateCustomerDrawBrowser__SWIG_1(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3, IntPtr jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserHelper_CreateCustomerDrawBrowser__SWIG_2")]
		public static extern IntPtr IRailBrowserHelper_CreateCustomerDrawBrowser__SWIG_2(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserHelper_NavigateWebPage")]
		public static extern int IRailBrowserHelper_NavigateWebPage(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, bool jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailBrowserHelper")]
		public static extern void delete_IRailBrowserHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowser_GetCurrentUrl")]
		public static extern bool IRailBrowser_GetCurrentUrl(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowser_ReloadWithUrl__SWIG_0")]
		public static extern bool IRailBrowser_ReloadWithUrl__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowser_ReloadWithUrl__SWIG_1")]
		public static extern bool IRailBrowser_ReloadWithUrl__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowser_StopLoad")]
		public static extern void IRailBrowser_StopLoad(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowser_AddJavascriptEventListener")]
		public static extern bool IRailBrowser_AddJavascriptEventListener(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowser_RemoveAllJavascriptEventListener")]
		public static extern bool IRailBrowser_RemoveAllJavascriptEventListener(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowser_AllowNavigateNewPage")]
		public static extern void IRailBrowser_AllowNavigateNewPage(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowser_Close")]
		public static extern void IRailBrowser_Close(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailBrowser")]
		public static extern void delete_IRailBrowser(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_GetCurrentUrl")]
		public static extern bool IRailBrowserRender_GetCurrentUrl(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_ReloadWithUrl__SWIG_0")]
		public static extern bool IRailBrowserRender_ReloadWithUrl__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_ReloadWithUrl__SWIG_1")]
		public static extern bool IRailBrowserRender_ReloadWithUrl__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_StopLoad")]
		public static extern void IRailBrowserRender_StopLoad(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_AddJavascriptEventListener")]
		public static extern bool IRailBrowserRender_AddJavascriptEventListener(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_RemoveAllJavascriptEventListener")]
		public static extern bool IRailBrowserRender_RemoveAllJavascriptEventListener(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_AllowNavigateNewPage")]
		public static extern void IRailBrowserRender_AllowNavigateNewPage(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_Close")]
		public static extern void IRailBrowserRender_Close(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_UpdateCustomDrawWindowPos")]
		public static extern void IRailBrowserRender_UpdateCustomDrawWindowPos(IntPtr jarg1, int jarg2, int jarg3, uint jarg4, uint jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_SetBrowserActive")]
		public static extern void IRailBrowserRender_SetBrowserActive(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_GoBack")]
		public static extern void IRailBrowserRender_GoBack(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_GoForward")]
		public static extern void IRailBrowserRender_GoForward(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_ExecuteJavascript")]
		public static extern bool IRailBrowserRender_ExecuteJavascript(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_DispatchWindowsMessage")]
		public static extern void IRailBrowserRender_DispatchWindowsMessage(IntPtr jarg1, uint jarg2, uint jarg3, uint jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_DispatchMouseMessage")]
		public static extern void IRailBrowserRender_DispatchMouseMessage(IntPtr jarg1, int jarg2, uint jarg3, uint jarg4, uint jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_MouseWheel")]
		public static extern void IRailBrowserRender_MouseWheel(IntPtr jarg1, int jarg2, uint jarg3, uint jarg4, uint jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_SetFocus")]
		public static extern void IRailBrowserRender_SetFocus(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_KeyDown")]
		public static extern void IRailBrowserRender_KeyDown(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_KeyUp")]
		public static extern void IRailBrowserRender_KeyUp(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_KeyChar")]
		public static extern void IRailBrowserRender_KeyChar(IntPtr jarg1, uint jarg2, bool jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailBrowserRender")]
		public static extern void delete_IRailBrowserRender(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailDlcHelper_AsyncQueryIsOwnedDlcsOnServer")]
		public static extern int IRailDlcHelper_AsyncQueryIsOwnedDlcsOnServer(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailDlcHelper_AsyncCheckAllDlcsStateReady")]
		public static extern int IRailDlcHelper_AsyncCheckAllDlcsStateReady(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailDlcHelper_IsDlcInstalled__SWIG_0")]
		public static extern bool IRailDlcHelper_IsDlcInstalled__SWIG_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailDlcHelper_IsDlcInstalled__SWIG_1")]
		public static extern bool IRailDlcHelper_IsDlcInstalled__SWIG_1(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailDlcHelper_IsOwnedDlc")]
		public static extern bool IRailDlcHelper_IsOwnedDlc(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailDlcHelper_GetDlcCount")]
		public static extern uint IRailDlcHelper_GetDlcCount(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailDlcHelper_GetDlcInfo")]
		public static extern bool IRailDlcHelper_GetDlcInfo(IntPtr jarg1, uint jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailDlcHelper_AsyncInstallDlc")]
		public static extern bool IRailDlcHelper_AsyncInstallDlc(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailDlcHelper_AsyncRemoveDlc")]
		public static extern bool IRailDlcHelper_AsyncRemoveDlc(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailDlcHelper")]
		public static extern void delete_IRailDlcHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFloatingWindow_AsyncShowRailFloatingWindow")]
		public static extern int IRailFloatingWindow_AsyncShowRailFloatingWindow(IntPtr jarg1, int jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFloatingWindow_AsyncCloseRailFloatingWindow")]
		public static extern int IRailFloatingWindow_AsyncCloseRailFloatingWindow(IntPtr jarg1, int jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFloatingWindow_SetNotifyWindowPosition")]
		public static extern int IRailFloatingWindow_SetNotifyWindowPosition(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFloatingWindow_AsyncShowStoreWindow")]
		public static extern int IRailFloatingWindow_AsyncShowStoreWindow(IntPtr jarg1, ulong jarg2, IntPtr jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFloatingWindow_IsFloatingWindowAvailable")]
		public static extern bool IRailFloatingWindow_IsFloatingWindowAvailable(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFloatingWindow_AsyncShowDefaultGameStoreWindow")]
		public static extern int IRailFloatingWindow_AsyncShowDefaultGameStoreWindow(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFloatingWindow_SetNotifyWindowEnable")]
		public static extern int IRailFloatingWindow_SetNotifyWindowEnable(IntPtr jarg1, int jarg2, bool jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailFloatingWindow")]
		public static extern void delete_IRailFloatingWindow(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_AsyncGetPersonalInfo")]
		public static extern int IRailFriends_AsyncGetPersonalInfo(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_AsyncGetFriendMetadata")]
		public static extern int IRailFriends_AsyncGetFriendMetadata(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_AsyncSetMyMetadata")]
		public static extern int IRailFriends_AsyncSetMyMetadata(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_AsyncClearAllMyMetadata")]
		public static extern int IRailFriends_AsyncClearAllMyMetadata(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_AsyncSetInviteCommandLine")]
		public static extern int IRailFriends_AsyncSetInviteCommandLine(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_AsyncGetInviteCommandLine")]
		public static extern int IRailFriends_AsyncGetInviteCommandLine(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_AsyncReportPlayedWithUserList")]
		public static extern int IRailFriends_AsyncReportPlayedWithUserList(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_GetFriendsList")]
		public static extern int IRailFriends_GetFriendsList(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_AsyncQueryFriendPlayedGamesInfo")]
		public static extern int IRailFriends_AsyncQueryFriendPlayedGamesInfo(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_AsyncQueryPlayedWithFriendsList")]
		public static extern int IRailFriends_AsyncQueryPlayedWithFriendsList(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_AsyncQueryPlayedWithFriendsTime")]
		public static extern int IRailFriends_AsyncQueryPlayedWithFriendsTime(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_AsyncQueryPlayedWithFriendsGames")]
		public static extern int IRailFriends_AsyncQueryPlayedWithFriendsGames(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_AsyncAddFriend")]
		public static extern int IRailFriends_AsyncAddFriend(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFriends_AsyncUpdateFriendsData")]
		public static extern int IRailFriends_AsyncUpdateFriendsData(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailFriends")]
		public static extern void delete_IRailFriends(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServerHelper_AsyncGetGameServerPlayerList")]
		public static extern int IRailGameServerHelper_AsyncGetGameServerPlayerList(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServerHelper_AsyncGetGameServerList")]
		public static extern int IRailGameServerHelper_AsyncGetGameServerList(IntPtr jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg6);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServerHelper_AsyncCreateGameServer__SWIG_0")]
		public static extern IntPtr IRailGameServerHelper_AsyncCreateGameServer__SWIG_0(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServerHelper_AsyncCreateGameServer__SWIG_1")]
		public static extern IntPtr IRailGameServerHelper_AsyncCreateGameServer__SWIG_1(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServerHelper_AsyncCreateGameServer__SWIG_2")]
		public static extern IntPtr IRailGameServerHelper_AsyncCreateGameServer__SWIG_2(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServerHelper_AsyncCreateGameServer__SWIG_3")]
		public static extern IntPtr IRailGameServerHelper_AsyncCreateGameServer__SWIG_3(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServerHelper_AsyncGetFavoriteGameServers__SWIG_0")]
		public static extern int IRailGameServerHelper_AsyncGetFavoriteGameServers__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServerHelper_AsyncGetFavoriteGameServers__SWIG_1")]
		public static extern int IRailGameServerHelper_AsyncGetFavoriteGameServers__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServerHelper_AsyncAddFavoriteGameServer__SWIG_0")]
		public static extern int IRailGameServerHelper_AsyncAddFavoriteGameServer__SWIG_0(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServerHelper_AsyncAddFavoriteGameServer__SWIG_1")]
		public static extern int IRailGameServerHelper_AsyncAddFavoriteGameServer__SWIG_1(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServerHelper_AsyncRemoveFavoriteGameServer__SWIG_0")]
		public static extern int IRailGameServerHelper_AsyncRemoveFavoriteGameServer__SWIG_0(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServerHelper_AsyncRemoveFavoriteGameServer__SWIG_1")]
		public static extern int IRailGameServerHelper_AsyncRemoveFavoriteGameServer__SWIG_1(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailGameServerHelper")]
		public static extern void delete_IRailGameServerHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetGameServerRailID")]
		public static extern IntPtr IRailGameServer_GetGameServerRailID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetGameServerName")]
		public static extern int IRailGameServer_GetGameServerName(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetGameServerFullName")]
		public static extern int IRailGameServer_GetGameServerFullName(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetOwnerRailID")]
		public static extern IntPtr IRailGameServer_GetOwnerRailID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SetHost")]
		public static extern bool IRailGameServer_SetHost(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetHost")]
		public static extern bool IRailGameServer_GetHost(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SetMapName")]
		public static extern bool IRailGameServer_SetMapName(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetMapName")]
		public static extern bool IRailGameServer_GetMapName(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SetPasswordProtect")]
		public static extern bool IRailGameServer_SetPasswordProtect(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetPasswordProtect")]
		public static extern bool IRailGameServer_GetPasswordProtect(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SetMaxPlayers")]
		public static extern bool IRailGameServer_SetMaxPlayers(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetMaxPlayers")]
		public static extern uint IRailGameServer_GetMaxPlayers(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SetBotPlayers")]
		public static extern bool IRailGameServer_SetBotPlayers(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetBotPlayers")]
		public static extern uint IRailGameServer_GetBotPlayers(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SetGameServerDescription")]
		public static extern bool IRailGameServer_SetGameServerDescription(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetGameServerDescription")]
		public static extern bool IRailGameServer_GetGameServerDescription(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SetGameServerTags")]
		public static extern bool IRailGameServer_SetGameServerTags(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetGameServerTags")]
		public static extern bool IRailGameServer_GetGameServerTags(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SetMods")]
		public static extern bool IRailGameServer_SetMods(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetMods")]
		public static extern bool IRailGameServer_GetMods(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SetSpectatorHost")]
		public static extern bool IRailGameServer_SetSpectatorHost(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetSpectatorHost")]
		public static extern bool IRailGameServer_GetSpectatorHost(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SetGameServerVersion")]
		public static extern bool IRailGameServer_SetGameServerVersion(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetGameServerVersion")]
		public static extern bool IRailGameServer_GetGameServerVersion(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SetIsFriendOnly")]
		public static extern bool IRailGameServer_SetIsFriendOnly(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetIsFriendOnly")]
		public static extern bool IRailGameServer_GetIsFriendOnly(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_ClearAllMetadata")]
		public static extern bool IRailGameServer_ClearAllMetadata(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetMetadata")]
		public static extern int IRailGameServer_GetMetadata(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SetMetadata")]
		public static extern int IRailGameServer_SetMetadata(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_AsyncSetMetadata")]
		public static extern int IRailGameServer_AsyncSetMetadata(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_AsyncGetMetadata")]
		public static extern int IRailGameServer_AsyncGetMetadata(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_AsyncGetAllMetadata")]
		public static extern int IRailGameServer_AsyncGetAllMetadata(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_AsyncAcquireGameServerSessionTicket")]
		public static extern int IRailGameServer_AsyncAcquireGameServerSessionTicket(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_AsyncStartSessionWithPlayer")]
		public static extern int IRailGameServer_AsyncStartSessionWithPlayer(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_TerminateSessionOfPlayer")]
		public static extern void IRailGameServer_TerminateSessionOfPlayer(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_AbandonGameServerSessionTicket")]
		public static extern void IRailGameServer_AbandonGameServerSessionTicket(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_ReportPlayerJoinGameServer")]
		public static extern int IRailGameServer_ReportPlayerJoinGameServer(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_ReportPlayerQuitGameServer")]
		public static extern int IRailGameServer_ReportPlayerQuitGameServer(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_UpdateGameServerPlayerList")]
		public static extern int IRailGameServer_UpdateGameServerPlayerList(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetCurrentPlayers")]
		public static extern uint IRailGameServer_GetCurrentPlayers(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_RemoveAllPlayers")]
		public static extern void IRailGameServer_RemoveAllPlayers(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_RegisterToGameServerList")]
		public static extern int IRailGameServer_RegisterToGameServerList(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_UnregisterFromGameServerList")]
		public static extern int IRailGameServer_UnregisterFromGameServerList(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_CloseGameServer")]
		public static extern int IRailGameServer_CloseGameServer(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetFriendsInGameServer")]
		public static extern int IRailGameServer_GetFriendsInGameServer(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_IsUserInGameServer")]
		public static extern bool IRailGameServer_IsUserInGameServer(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SetServerInfo")]
		public static extern bool IRailGameServer_SetServerInfo(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_GetServerInfo")]
		public static extern bool IRailGameServer_GetServerInfo(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_EnableTeamVoice")]
		public static extern int IRailGameServer_EnableTeamVoice(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailGameServer")]
		public static extern void delete_IRailGameServer(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailInGameCoin_AsyncRequestCoinInfo")]
		public static extern int IRailInGameCoin_AsyncRequestCoinInfo(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailInGameCoin_AsyncPurchaseCoins")]
		public static extern int IRailInGameCoin_AsyncPurchaseCoins(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailInGameCoin")]
		public static extern void delete_IRailInGameCoin(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailInGamePurchase_AsyncRequestAllPurchasableProducts")]
		public static extern int IRailInGamePurchase_AsyncRequestAllPurchasableProducts(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailInGamePurchase_AsyncRequestAllProducts")]
		public static extern int IRailInGamePurchase_AsyncRequestAllProducts(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailInGamePurchase_GetProductInfo")]
		public static extern int IRailInGamePurchase_GetProductInfo(IntPtr jarg1, uint jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailInGamePurchase_AsyncPurchaseProducts")]
		public static extern int IRailInGamePurchase_AsyncPurchaseProducts(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailInGamePurchase_AsyncFinishOrder")]
		public static extern int IRailInGamePurchase_AsyncFinishOrder(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailInGamePurchase_AsyncPurchaseProductsToAssets")]
		public static extern int IRailInGamePurchase_AsyncPurchaseProductsToAssets(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailInGamePurchase")]
		public static extern void delete_IRailInGamePurchase(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailInGameActivityHelper_AsyncQueryGameActivity")]
		public static extern int IRailInGameActivityHelper_AsyncQueryGameActivity(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailInGameActivityHelper_AsyncOpenDefaultGameActivityWindow")]
		public static extern int IRailInGameActivityHelper_AsyncOpenDefaultGameActivityWindow(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailInGameActivityHelper_AsyncOpenGameActivityWindow")]
		public static extern int IRailInGameActivityHelper_AsyncOpenGameActivityWindow(IntPtr jarg1, ulong jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailInGameActivityHelper")]
		public static extern void delete_IRailInGameActivityHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardParameters__SWIG_0")]
		public static extern IntPtr new_LeaderboardParameters__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardParameters_param_set")]
		public static extern void LeaderboardParameters_param_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardParameters_param_get")]
		public static extern IntPtr LeaderboardParameters_param_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardParameters__SWIG_1")]
		public static extern IntPtr new_LeaderboardParameters__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_LeaderboardParameters")]
		public static extern void delete_LeaderboardParameters(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RequestLeaderboardEntryParam__SWIG_0")]
		public static extern IntPtr new_RequestLeaderboardEntryParam__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_RequestLeaderboardEntryParam_type_set")]
		public static extern void RequestLeaderboardEntryParam_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RequestLeaderboardEntryParam_type_get")]
		public static extern int RequestLeaderboardEntryParam_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RequestLeaderboardEntryParam_range_start_set")]
		public static extern void RequestLeaderboardEntryParam_range_start_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RequestLeaderboardEntryParam_range_start_get")]
		public static extern int RequestLeaderboardEntryParam_range_start_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RequestLeaderboardEntryParam_range_end_set")]
		public static extern void RequestLeaderboardEntryParam_range_end_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RequestLeaderboardEntryParam_range_end_get")]
		public static extern int RequestLeaderboardEntryParam_range_end_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RequestLeaderboardEntryParam_user_coordinate_set")]
		public static extern void RequestLeaderboardEntryParam_user_coordinate_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_RequestLeaderboardEntryParam_user_coordinate_get")]
		public static extern bool RequestLeaderboardEntryParam_user_coordinate_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_RequestLeaderboardEntryParam__SWIG_1")]
		public static extern IntPtr new_RequestLeaderboardEntryParam__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_RequestLeaderboardEntryParam")]
		public static extern void delete_RequestLeaderboardEntryParam(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardData__SWIG_0")]
		public static extern IntPtr new_LeaderboardData__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardData_score_set")]
		public static extern void LeaderboardData_score_set(IntPtr jarg1, double jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardData_score_get")]
		public static extern double LeaderboardData_score_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardData_rank_set")]
		public static extern void LeaderboardData_rank_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardData_rank_get")]
		public static extern int LeaderboardData_rank_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardData_spacework_id_set")]
		public static extern void LeaderboardData_spacework_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardData_spacework_id_get")]
		public static extern IntPtr LeaderboardData_spacework_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardData_additional_infomation_set")]
		public static extern void LeaderboardData_additional_infomation_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardData_additional_infomation_get")]
		public static extern IntPtr LeaderboardData_additional_infomation_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardData__SWIG_1")]
		public static extern IntPtr new_LeaderboardData__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_LeaderboardData")]
		public static extern void delete_LeaderboardData(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardEntry__SWIG_0")]
		public static extern IntPtr new_LeaderboardEntry__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardEntry_player_id_set")]
		public static extern void LeaderboardEntry_player_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardEntry_player_id_get")]
		public static extern IntPtr LeaderboardEntry_player_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardEntry_data_set")]
		public static extern void LeaderboardEntry_data_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardEntry_data_get")]
		public static extern IntPtr LeaderboardEntry_data_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardEntry__SWIG_1")]
		public static extern IntPtr new_LeaderboardEntry__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_LeaderboardEntry")]
		public static extern void delete_LeaderboardEntry(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_UploadLeaderboardParam__SWIG_0")]
		public static extern IntPtr new_UploadLeaderboardParam__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_UploadLeaderboardParam_type_set")]
		public static extern void UploadLeaderboardParam_type_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_UploadLeaderboardParam_type_get")]
		public static extern int UploadLeaderboardParam_type_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_UploadLeaderboardParam_data_set")]
		public static extern void UploadLeaderboardParam_data_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_UploadLeaderboardParam_data_get")]
		public static extern IntPtr UploadLeaderboardParam_data_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_UploadLeaderboardParam__SWIG_1")]
		public static extern IntPtr new_UploadLeaderboardParam__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_UploadLeaderboardParam")]
		public static extern void delete_UploadLeaderboardParam(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailLeaderboardHelper")]
		public static extern void delete_IRailLeaderboardHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboardHelper_OpenLeaderboard")]
		public static extern IntPtr IRailLeaderboardHelper_OpenLeaderboard(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboardHelper_AsyncCreateLeaderboard")]
		public static extern IntPtr IRailLeaderboardHelper_AsyncCreateLeaderboard(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, int jarg3, int jarg4, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg5, out RailResult jarg6);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboard_GetLeaderboardName")]
		public static extern IntPtr IRailLeaderboard_GetLeaderboardName(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboard_GetLeaderboardDisplayName")]
		public static extern IntPtr IRailLeaderboard_GetLeaderboardDisplayName(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboard_GetTotalEntriesCount")]
		public static extern int IRailLeaderboard_GetTotalEntriesCount(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboard_AsyncGetLeaderboard")]
		public static extern int IRailLeaderboard_AsyncGetLeaderboard(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboard_GetLeaderboardParameters")]
		public static extern int IRailLeaderboard_GetLeaderboardParameters(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboard_CreateLeaderboardEntries")]
		public static extern IntPtr IRailLeaderboard_CreateLeaderboardEntries(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboard_AsyncUploadLeaderboard")]
		public static extern int IRailLeaderboard_AsyncUploadLeaderboard(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboard_GetLeaderboardSortType")]
		public static extern int IRailLeaderboard_GetLeaderboardSortType(IntPtr jarg1, out int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboard_GetLeaderboardDisplayType")]
		public static extern int IRailLeaderboard_GetLeaderboardDisplayType(IntPtr jarg1, out int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboard_AsyncAttachSpaceWork")]
		public static extern int IRailLeaderboard_AsyncAttachSpaceWork(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailLeaderboard")]
		public static extern void delete_IRailLeaderboard(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboardEntries_GetRailID")]
		public static extern IntPtr IRailLeaderboardEntries_GetRailID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboardEntries_GetLeaderboardName")]
		public static extern IntPtr IRailLeaderboardEntries_GetLeaderboardName(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboardEntries_AsyncRequestLeaderboardEntries")]
		public static extern int IRailLeaderboardEntries_AsyncRequestLeaderboardEntries(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboardEntries_GetEntriesParam")]
		public static extern IntPtr IRailLeaderboardEntries_GetEntriesParam(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboardEntries_GetEntriesCount")]
		public static extern int IRailLeaderboardEntries_GetEntriesCount(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboardEntries_GetLeaderboardEntry")]
		public static extern int IRailLeaderboardEntries_GetLeaderboardEntry(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailLeaderboardEntries")]
		public static extern void delete_IRailLeaderboardEntries(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardReceived__SWIG_0")]
		public static extern IntPtr new_LeaderboardReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardReceived_leaderboard_name_set")]
		public static extern void LeaderboardReceived_leaderboard_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardReceived_leaderboard_name_get")]
		public static extern IntPtr LeaderboardReceived_leaderboard_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardReceived_does_exist_set")]
		public static extern void LeaderboardReceived_does_exist_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardReceived_does_exist_get")]
		public static extern bool LeaderboardReceived_does_exist_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardReceived__SWIG_1")]
		public static extern IntPtr new_LeaderboardReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_LeaderboardReceived")]
		public static extern void delete_LeaderboardReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardCreated__SWIG_0")]
		public static extern IntPtr new_LeaderboardCreated__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardCreated_leaderboard_name_set")]
		public static extern void LeaderboardCreated_leaderboard_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardCreated_leaderboard_name_get")]
		public static extern IntPtr LeaderboardCreated_leaderboard_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardCreated__SWIG_1")]
		public static extern IntPtr new_LeaderboardCreated__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_LeaderboardCreated")]
		public static extern void delete_LeaderboardCreated(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardEntryReceived__SWIG_0")]
		public static extern IntPtr new_LeaderboardEntryReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardEntryReceived_leaderboard_name_set")]
		public static extern void LeaderboardEntryReceived_leaderboard_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardEntryReceived_leaderboard_name_get")]
		public static extern IntPtr LeaderboardEntryReceived_leaderboard_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardEntryReceived__SWIG_1")]
		public static extern IntPtr new_LeaderboardEntryReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_LeaderboardEntryReceived")]
		public static extern void delete_LeaderboardEntryReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardUploaded__SWIG_0")]
		public static extern IntPtr new_LeaderboardUploaded__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardUploaded_leaderboard_name_set")]
		public static extern void LeaderboardUploaded_leaderboard_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardUploaded_leaderboard_name_get")]
		public static extern IntPtr LeaderboardUploaded_leaderboard_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardUploaded_score_set")]
		public static extern void LeaderboardUploaded_score_set(IntPtr jarg1, double jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardUploaded_score_get")]
		public static extern double LeaderboardUploaded_score_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardUploaded_better_score_set")]
		public static extern void LeaderboardUploaded_better_score_set(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardUploaded_better_score_get")]
		public static extern bool LeaderboardUploaded_better_score_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardUploaded_new_rank_set")]
		public static extern void LeaderboardUploaded_new_rank_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardUploaded_new_rank_get")]
		public static extern int LeaderboardUploaded_new_rank_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardUploaded_old_rank_set")]
		public static extern void LeaderboardUploaded_old_rank_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardUploaded_old_rank_get")]
		public static extern int LeaderboardUploaded_old_rank_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardUploaded__SWIG_1")]
		public static extern IntPtr new_LeaderboardUploaded__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_LeaderboardUploaded")]
		public static extern void delete_LeaderboardUploaded(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardAttachSpaceWork__SWIG_0")]
		public static extern IntPtr new_LeaderboardAttachSpaceWork__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardAttachSpaceWork_leaderboard_name_set")]
		public static extern void LeaderboardAttachSpaceWork_leaderboard_name_set(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardAttachSpaceWork_leaderboard_name_get")]
		public static extern IntPtr LeaderboardAttachSpaceWork_leaderboard_name_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardAttachSpaceWork_spacework_id_set")]
		public static extern void LeaderboardAttachSpaceWork_spacework_id_set(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardAttachSpaceWork_spacework_id_get")]
		public static extern IntPtr LeaderboardAttachSpaceWork_spacework_id_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_LeaderboardAttachSpaceWork__SWIG_1")]
		public static extern IntPtr new_LeaderboardAttachSpaceWork__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_LeaderboardAttachSpaceWork")]
		public static extern void delete_LeaderboardAttachSpaceWork(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_AcceptSessionRequest")]
		public static extern int IRailNetwork_AcceptSessionRequest(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_SendData__SWIG_0")]
		public static extern int IRailNetwork_SendData__SWIG_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg4, uint jarg5, uint jarg6);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_SendData__SWIG_1")]
		public static extern int IRailNetwork_SendData__SWIG_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg4, uint jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_SendReliableData__SWIG_0")]
		public static extern int IRailNetwork_SendReliableData__SWIG_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg4, uint jarg5, uint jarg6);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_SendReliableData__SWIG_1")]
		public static extern int IRailNetwork_SendReliableData__SWIG_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg4, uint jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_IsDataReady__SWIG_0")]
		public static extern bool IRailNetwork_IsDataReady__SWIG_0(IntPtr jarg1, IntPtr jarg2, out uint jarg3, out uint jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_IsDataReady__SWIG_1")]
		public static extern bool IRailNetwork_IsDataReady__SWIG_1(IntPtr jarg1, IntPtr jarg2, out uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_ReadData__SWIG_0")]
		public static extern int IRailNetwork_ReadData__SWIG_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg4, uint jarg5, uint jarg6);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_ReadData__SWIG_1")]
		public static extern int IRailNetwork_ReadData__SWIG_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg4, uint jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_BlockMessageType")]
		public static extern int IRailNetwork_BlockMessageType(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_UnblockMessageType")]
		public static extern int IRailNetwork_UnblockMessageType(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_CloseSession")]
		public static extern int IRailNetwork_CloseSession(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_ResolveHostname")]
		public static extern int IRailNetwork_ResolveHostname(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_GetSessionState")]
		public static extern int IRailNetwork_GetSessionState(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_ForbidSessionRelay")]
		public static extern int IRailNetwork_ForbidSessionRelay(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_SendRawData")]
		public static extern int IRailNetwork_SendRawData(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg4, uint jarg5, bool jarg6, uint jarg7);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_AcceptRawSessionRequest")]
		public static extern int IRailNetwork_AcceptRawSessionRequest(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_ReadRawData__SWIG_0")]
		public static extern int IRailNetwork_ReadRawData__SWIG_0(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg4, uint jarg5, uint jarg6);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailNetwork_ReadRawData__SWIG_1")]
		public static extern int IRailNetwork_ReadRawData__SWIG_1(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg4, uint jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailNetwork")]
		public static extern void delete_IRailNetwork(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_AlreadyLoggedIn")]
		public static extern bool IRailPlayer_AlreadyLoggedIn(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_GetRailID")]
		public static extern IntPtr IRailPlayer_GetRailID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_GetPlayerDataPath")]
		public static extern int IRailPlayer_GetPlayerDataPath(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_AsyncAcquireSessionTicket")]
		public static extern int IRailPlayer_AsyncAcquireSessionTicket(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_AsyncStartSessionWithPlayer")]
		public static extern int IRailPlayer_AsyncStartSessionWithPlayer(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_TerminateSessionOfPlayer")]
		public static extern void IRailPlayer_TerminateSessionOfPlayer(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_AbandonSessionTicket")]
		public static extern void IRailPlayer_AbandonSessionTicket(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_GetPlayerName")]
		public static extern int IRailPlayer_GetPlayerName(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_GetPlayerOwnershipType")]
		public static extern int IRailPlayer_GetPlayerOwnershipType(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_AsyncGetGamePurchaseKey")]
		public static extern int IRailPlayer_AsyncGetGamePurchaseKey(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_IsGameRevenueLimited")]
		public static extern bool IRailPlayer_IsGameRevenueLimited(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_GetRateOfGameRevenue")]
		public static extern float IRailPlayer_GetRateOfGameRevenue(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_AsyncQueryPlayerBannedStatus")]
		public static extern int IRailPlayer_AsyncQueryPlayerBannedStatus(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_AsyncGetAuthenticateURL")]
		public static extern int IRailPlayer_AsyncGetAuthenticateURL(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_AsyncGetPlayerMetadata")]
		public static extern int IRailPlayer_AsyncGetPlayerMetadata(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_AsyncGetEncryptedGameTicket")]
		public static extern int IRailPlayer_AsyncGetEncryptedGameTicket(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayer_GetPlayerAccountType")]
		public static extern int IRailPlayer_GetPlayerAccountType(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailPlayer")]
		public static extern void delete_IRailPlayer(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoomHelper_CreateRoom")]
		public static extern IntPtr IRailRoomHelper_CreateRoom(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3, out RailResult jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoomHelper_AsyncCreateRoom")]
		public static extern IntPtr IRailRoomHelper_AsyncCreateRoom(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoomHelper_OpenRoom")]
		public static extern IntPtr IRailRoomHelper_OpenRoom(IntPtr jarg1, ulong jarg2, out RailResult jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoomHelper_AsyncOpenRoom")]
		public static extern IntPtr IRailRoomHelper_AsyncOpenRoom(IntPtr jarg1, ulong jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoomHelper_AsyncGetRoomList")]
		public static extern int IRailRoomHelper_AsyncGetRoomList(IntPtr jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg6);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoomHelper_AsyncGetRoomListByTags")]
		public static extern int IRailRoomHelper_AsyncGetRoomListByTags(IntPtr jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg6);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoomHelper_AsyncGetUserRoomList")]
		public static extern int IRailRoomHelper_AsyncGetUserRoomList(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailRoomHelper")]
		public static extern void delete_IRailRoomHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncJoinRoom")]
		public static extern int IRailRoom_AsyncJoinRoom(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_GetRoomID")]
		public static extern ulong IRailRoom_GetRoomID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_GetRoomName")]
		public static extern int IRailRoom_GetRoomName(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_GetOwnerID")]
		public static extern IntPtr IRailRoom_GetOwnerID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_HasPassword")]
		public static extern bool IRailRoom_HasPassword(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_GetRoomType")]
		public static extern int IRailRoom_GetRoomType(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_GetMembers")]
		public static extern uint IRailRoom_GetMembers(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_GetMemberByIndex")]
		public static extern IntPtr IRailRoom_GetMemberByIndex(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_GetMemberNameByIndex")]
		public static extern int IRailRoom_GetMemberNameByIndex(IntPtr jarg1, uint jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_GetMaxMembers")]
		public static extern uint IRailRoom_GetMaxMembers(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_Leave")]
		public static extern void IRailRoom_Leave(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncSetNewRoomOwner")]
		public static extern int IRailRoom_AsyncSetNewRoomOwner(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncGetRoomMembers")]
		public static extern int IRailRoom_AsyncGetRoomMembers(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncGetAllRoomData")]
		public static extern int IRailRoom_AsyncGetAllRoomData(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncKickOffMember")]
		public static extern int IRailRoom_AsyncKickOffMember(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncSetRoomTag")]
		public static extern int IRailRoom_AsyncSetRoomTag(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncGetRoomTag")]
		public static extern int IRailRoom_AsyncGetRoomTag(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncSetRoomMetadata")]
		public static extern int IRailRoom_AsyncSetRoomMetadata(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncGetRoomMetadata")]
		public static extern int IRailRoom_AsyncGetRoomMetadata(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncClearRoomMetadata")]
		public static extern int IRailRoom_AsyncClearRoomMetadata(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncSetMemberMetadata")]
		public static extern int IRailRoom_AsyncSetMemberMetadata(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncGetMemberMetadata")]
		public static extern int IRailRoom_AsyncGetMemberMetadata(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_SendDataToMember__SWIG_0")]
		public static extern int IRailRoom_SendDataToMember__SWIG_0(IntPtr jarg1, IntPtr jarg2, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg3, uint jarg4, uint jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_SendDataToMember__SWIG_1")]
		public static extern int IRailRoom_SendDataToMember__SWIG_1(IntPtr jarg1, IntPtr jarg2, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg3, uint jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_SetGameServerID")]
		public static extern int IRailRoom_SetGameServerID(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_GetGameServerID")]
		public static extern IntPtr IRailRoom_GetGameServerID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_SetRoomJoinable")]
		public static extern int IRailRoom_SetRoomJoinable(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_IsRoomJoinable")]
		public static extern bool IRailRoom_IsRoomJoinable(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_GetFriendsInRoom")]
		public static extern int IRailRoom_GetFriendsInRoom(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_IsUserInRoom")]
		public static extern bool IRailRoom_IsUserInRoom(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_EnableTeamVoice")]
		public static extern int IRailRoom_EnableTeamVoice(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncSetRoomType")]
		public static extern int IRailRoom_AsyncSetRoomType(IntPtr jarg1, int jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_AsyncSetRoomMaxMember")]
		public static extern int IRailRoom_AsyncSetRoomMaxMember(IntPtr jarg1, uint jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailRoom")]
		public static extern void delete_IRailRoom(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailScreenshotHelper_CreateScreenshotWithRawData")]
		public static extern IntPtr IRailScreenshotHelper_CreateScreenshotWithRawData(IntPtr jarg1, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg2, uint jarg3, uint jarg4, uint jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailScreenshotHelper_CreateScreenshotWithLocalImage")]
		public static extern IntPtr IRailScreenshotHelper_CreateScreenshotWithLocalImage(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailScreenshotHelper_AsyncTakeScreenshot")]
		public static extern void IRailScreenshotHelper_AsyncTakeScreenshot(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailScreenshotHelper_HookScreenshotHotKey")]
		public static extern void IRailScreenshotHelper_HookScreenshotHotKey(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailScreenshotHelper_IsScreenshotHotKeyHooked")]
		public static extern bool IRailScreenshotHelper_IsScreenshotHotKeyHooked(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailScreenshotHelper")]
		public static extern void delete_IRailScreenshotHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailScreenshot_SetLocation")]
		public static extern bool IRailScreenshot_SetLocation(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailScreenshot_SetUsers")]
		public static extern bool IRailScreenshot_SetUsers(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailScreenshot_AssociatePublishedFiles")]
		public static extern bool IRailScreenshot_AssociatePublishedFiles(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailScreenshot_AsyncPublishScreenshot")]
		public static extern int IRailScreenshot_AsyncPublishScreenshot(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailScreenshot")]
		public static extern void delete_IRailScreenshot(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStatisticHelper_CreatePlayerStats")]
		public static extern IntPtr IRailStatisticHelper_CreatePlayerStats(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStatisticHelper_GetGlobalStats")]
		public static extern IntPtr IRailStatisticHelper_GetGlobalStats(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStatisticHelper_AsyncGetNumberOfPlayer")]
		public static extern int IRailStatisticHelper_AsyncGetNumberOfPlayer(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailStatisticHelper")]
		public static extern void delete_IRailStatisticHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerStats_GetRailID")]
		public static extern IntPtr IRailPlayerStats_GetRailID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerStats_AsyncRequestStats")]
		public static extern int IRailPlayerStats_AsyncRequestStats(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerStats_GetStatValue__SWIG_0")]
		public static extern int IRailPlayerStats_GetStatValue__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, out int jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerStats_GetStatValue__SWIG_1")]
		public static extern int IRailPlayerStats_GetStatValue__SWIG_1(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, out double jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerStats_SetStatValue__SWIG_0")]
		public static extern int IRailPlayerStats_SetStatValue__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, int jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerStats_SetStatValue__SWIG_1")]
		public static extern int IRailPlayerStats_SetStatValue__SWIG_1(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, double jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerStats_UpdateAverageStatValue")]
		public static extern int IRailPlayerStats_UpdateAverageStatValue(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, double jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerStats_AsyncStoreStats")]
		public static extern int IRailPlayerStats_AsyncStoreStats(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerStats_ResetAllStats")]
		public static extern int IRailPlayerStats_ResetAllStats(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailPlayerStats")]
		public static extern void delete_IRailPlayerStats(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGlobalStats_AsyncRequestGlobalStats")]
		public static extern int IRailGlobalStats_AsyncRequestGlobalStats(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGlobalStats_GetGlobalStatValue__SWIG_0")]
		public static extern int IRailGlobalStats_GetGlobalStatValue__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, out long jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGlobalStats_GetGlobalStatValue__SWIG_1")]
		public static extern int IRailGlobalStats_GetGlobalStatValue__SWIG_1(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, out double jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGlobalStats_GetGlobalStatValueHistory__SWIG_0")]
		public static extern int IRailGlobalStats_GetGlobalStatValueHistory__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [Out][MarshalAs(UnmanagedType.LPArray)] long[] jarg3, uint jarg4, out int jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGlobalStats_GetGlobalStatValueHistory__SWIG_1")]
		public static extern int IRailGlobalStats_GetGlobalStatValueHistory__SWIG_1(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [Out][MarshalAs(UnmanagedType.LPArray)] double[] jarg3, uint jarg4, out int jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailGlobalStats")]
		public static extern void delete_IRailGlobalStats(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PlayerStatsReceived__SWIG_0")]
		public static extern IntPtr new_PlayerStatsReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PlayerStatsReceived__SWIG_1")]
		public static extern IntPtr new_PlayerStatsReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_PlayerStatsReceived")]
		public static extern void delete_PlayerStatsReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PlayerStatsStored__SWIG_0")]
		public static extern IntPtr new_PlayerStatsStored__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_PlayerStatsStored__SWIG_1")]
		public static extern IntPtr new_PlayerStatsStored__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_PlayerStatsStored")]
		public static extern void delete_PlayerStatsStored(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NumberOfPlayerReceived__SWIG_0")]
		public static extern IntPtr new_NumberOfPlayerReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_NumberOfPlayerReceived_online_number_set")]
		public static extern void NumberOfPlayerReceived_online_number_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NumberOfPlayerReceived_online_number_get")]
		public static extern int NumberOfPlayerReceived_online_number_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NumberOfPlayerReceived_offline_number_set")]
		public static extern void NumberOfPlayerReceived_offline_number_set(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NumberOfPlayerReceived_offline_number_get")]
		public static extern int NumberOfPlayerReceived_offline_number_get(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_NumberOfPlayerReceived__SWIG_1")]
		public static extern IntPtr new_NumberOfPlayerReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_NumberOfPlayerReceived")]
		public static extern void delete_NumberOfPlayerReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GlobalStatsRequestReceived__SWIG_0")]
		public static extern IntPtr new_GlobalStatsRequestReceived__SWIG_0();

		[DllImport("rail_api64", EntryPoint = "CSharp_new_GlobalStatsRequestReceived__SWIG_1")]
		public static extern IntPtr new_GlobalStatsRequestReceived__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_GlobalStatsRequestReceived")]
		public static extern void delete_GlobalStatsRequestReceived(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailStorageHelper")]
		public static extern void delete_IRailStorageHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_OpenFile__SWIG_0")]
		public static extern IntPtr IRailStorageHelper_OpenFile__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, out RailResult jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_OpenFile__SWIG_1")]
		public static extern IntPtr IRailStorageHelper_OpenFile__SWIG_1(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_CreateFile__SWIG_0")]
		public static extern IntPtr IRailStorageHelper_CreateFile__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, out RailResult jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_CreateFile__SWIG_1")]
		public static extern IntPtr IRailStorageHelper_CreateFile__SWIG_1(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_IsFileExist")]
		public static extern bool IRailStorageHelper_IsFileExist(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_ListFiles")]
		public static extern bool IRailStorageHelper_ListFiles(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_RemoveFile")]
		public static extern int IRailStorageHelper_RemoveFile(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_IsFileSyncedToCloud")]
		public static extern bool IRailStorageHelper_IsFileSyncedToCloud(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_GetFileTimestamp")]
		public static extern int IRailStorageHelper_GetFileTimestamp(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, out ulong jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_GetFileCount")]
		public static extern uint IRailStorageHelper_GetFileCount(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_GetFileNameAndSize")]
		public static extern int IRailStorageHelper_GetFileNameAndSize(IntPtr jarg1, uint jarg2, IntPtr jarg3, out ulong jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_AsyncQueryQuota")]
		public static extern int IRailStorageHelper_AsyncQueryQuota(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_SetSyncFileOption")]
		public static extern int IRailStorageHelper_SetSyncFileOption(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_IsCloudStorageEnabledForApp")]
		public static extern bool IRailStorageHelper_IsCloudStorageEnabledForApp(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_IsCloudStorageEnabledForPlayer")]
		public static extern bool IRailStorageHelper_IsCloudStorageEnabledForPlayer(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_AsyncPublishFileToUserSpace")]
		public static extern int IRailStorageHelper_AsyncPublishFileToUserSpace(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_OpenStreamFile__SWIG_0")]
		public static extern IntPtr IRailStorageHelper_OpenStreamFile__SWIG_0(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, IntPtr jarg3, out RailResult jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_OpenStreamFile__SWIG_1")]
		public static extern IntPtr IRailStorageHelper_OpenStreamFile__SWIG_1(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_AsyncListStreamFiles")]
		public static extern int IRailStorageHelper_AsyncListStreamFiles(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, IntPtr jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_AsyncRenameStreamFile")]
		public static extern int IRailStorageHelper_AsyncRenameStreamFile(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_AsyncDeleteStreamFile")]
		public static extern int IRailStorageHelper_AsyncDeleteStreamFile(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_GetRailFileEnabledOS")]
		public static extern uint IRailStorageHelper_GetRailFileEnabledOS(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStorageHelper_SetRailFileEnabledOS")]
		public static extern int IRailStorageHelper_SetRailFileEnabledOS(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, int jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailFile")]
		public static extern void delete_IRailFile(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFile_GetFilename")]
		public static extern IntPtr IRailFile_GetFilename(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFile_Read__SWIG_0")]
		public static extern uint IRailFile_Read__SWIG_0(IntPtr jarg1, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg2, uint jarg3, out RailResult jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFile_Read__SWIG_1")]
		public static extern uint IRailFile_Read__SWIG_1(IntPtr jarg1, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFile_Write__SWIG_0")]
		public static extern uint IRailFile_Write__SWIG_0(IntPtr jarg1, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg2, uint jarg3, out RailResult jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFile_Write__SWIG_1")]
		public static extern uint IRailFile_Write__SWIG_1(IntPtr jarg1, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFile_AsyncRead")]
		public static extern int IRailFile_AsyncRead(IntPtr jarg1, uint jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFile_AsyncWrite")]
		public static extern int IRailFile_AsyncWrite(IntPtr jarg1, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg2, uint jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFile_GetSize")]
		public static extern uint IRailFile_GetSize(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFile_Close")]
		public static extern void IRailFile_Close(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailStreamFile")]
		public static extern void delete_IRailStreamFile(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStreamFile_GetFilename")]
		public static extern IntPtr IRailStreamFile_GetFilename(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStreamFile_AsyncRead")]
		public static extern int IRailStreamFile_AsyncRead(IntPtr jarg1, int jarg2, uint jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStreamFile_AsyncWrite")]
		public static extern int IRailStreamFile_AsyncWrite(IntPtr jarg1, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg2, uint jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStreamFile_GetSize")]
		public static extern ulong IRailStreamFile_GetSize(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStreamFile_Close")]
		public static extern int IRailStreamFile_Close(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStreamFile_Cancel")]
		public static extern void IRailStreamFile_Cancel(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUsersHelper_AsyncGetUsersInfo")]
		public static extern int IRailUsersHelper_AsyncGetUsersInfo(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUsersHelper_AsyncInviteUsers")]
		public static extern int IRailUsersHelper_AsyncInviteUsers(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, IntPtr jarg3, IntPtr jarg4, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUsersHelper_AsyncGetInviteDetail")]
		public static extern int IRailUsersHelper_AsyncGetInviteDetail(IntPtr jarg1, IntPtr jarg2, int jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUsersHelper_AsyncCancelInvite")]
		public static extern int IRailUsersHelper_AsyncCancelInvite(IntPtr jarg1, int jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUsersHelper_AsyncCancelAllInvites")]
		public static extern int IRailUsersHelper_AsyncCancelAllInvites(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUsersHelper_AsyncGetUserLimits")]
		public static extern int IRailUsersHelper_AsyncGetUserLimits(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUsersHelper_AsyncShowChatWindowWithFriend")]
		public static extern int IRailUsersHelper_AsyncShowChatWindowWithFriend(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUsersHelper_AsyncShowUserHomepageWindow")]
		public static extern int IRailUsersHelper_AsyncShowUserHomepageWindow(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailUsersHelper")]
		public static extern void delete_IRailUsersHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncGetMySubscribedWorks__SWIG_0")]
		public static extern int IRailUserSpaceHelper_AsyncGetMySubscribedWorks__SWIG_0(IntPtr jarg1, uint jarg2, uint jarg3, int jarg4, IntPtr jarg5, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg6);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncGetMySubscribedWorks__SWIG_1")]
		public static extern int IRailUserSpaceHelper_AsyncGetMySubscribedWorks__SWIG_1(IntPtr jarg1, uint jarg2, uint jarg3, int jarg4, IntPtr jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncGetMySubscribedWorks__SWIG_2")]
		public static extern int IRailUserSpaceHelper_AsyncGetMySubscribedWorks__SWIG_2(IntPtr jarg1, uint jarg2, uint jarg3, int jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncGetMyFavoritesWorks__SWIG_0")]
		public static extern int IRailUserSpaceHelper_AsyncGetMyFavoritesWorks__SWIG_0(IntPtr jarg1, uint jarg2, uint jarg3, int jarg4, IntPtr jarg5, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg6);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncGetMyFavoritesWorks__SWIG_1")]
		public static extern int IRailUserSpaceHelper_AsyncGetMyFavoritesWorks__SWIG_1(IntPtr jarg1, uint jarg2, uint jarg3, int jarg4, IntPtr jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncGetMyFavoritesWorks__SWIG_2")]
		public static extern int IRailUserSpaceHelper_AsyncGetMyFavoritesWorks__SWIG_2(IntPtr jarg1, uint jarg2, uint jarg3, int jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_0")]
		public static extern int IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_0(IntPtr jarg1, IntPtr jarg2, uint jarg3, uint jarg4, int jarg5, IntPtr jarg6, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg7);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_1")]
		public static extern int IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_1(IntPtr jarg1, IntPtr jarg2, uint jarg3, uint jarg4, int jarg5, IntPtr jarg6);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_2")]
		public static extern int IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_2(IntPtr jarg1, IntPtr jarg2, uint jarg3, uint jarg4, int jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_3")]
		public static extern int IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_3(IntPtr jarg1, IntPtr jarg2, uint jarg3, uint jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncSubscribeSpaceWorks")]
		public static extern int IRailUserSpaceHelper_AsyncSubscribeSpaceWorks(IntPtr jarg1, IntPtr jarg2, bool jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_OpenSpaceWork")]
		public static extern IntPtr IRailUserSpaceHelper_OpenSpaceWork(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_CreateSpaceWork")]
		public static extern IntPtr IRailUserSpaceHelper_CreateSpaceWork(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_GetMySubscribedWorks")]
		public static extern int IRailUserSpaceHelper_GetMySubscribedWorks(IntPtr jarg1, uint jarg2, uint jarg3, int jarg4, IntPtr jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_GetMySubscribedWorksCount")]
		public static extern uint IRailUserSpaceHelper_GetMySubscribedWorksCount(IntPtr jarg1, int jarg2, out RailResult jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncRemoveSpaceWork")]
		public static extern int IRailUserSpaceHelper_AsyncRemoveSpaceWork(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncModifyFavoritesWorks")]
		public static extern int IRailUserSpaceHelper_AsyncModifyFavoritesWorks(IntPtr jarg1, IntPtr jarg2, int jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncVoteSpaceWork")]
		public static extern int IRailUserSpaceHelper_AsyncVoteSpaceWork(IntPtr jarg1, IntPtr jarg2, int jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncSearchSpaceWork")]
		public static extern int IRailUserSpaceHelper_AsyncSearchSpaceWork(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, uint jarg5, uint jarg6, int jarg7, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg8);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncRateSpaceWork")]
		public static extern int IRailUserSpaceHelper_AsyncRateSpaceWork(IntPtr jarg1, IntPtr jarg2, int jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUserSpaceHelper_AsyncQuerySpaceWorksInfo")]
		public static extern int IRailUserSpaceHelper_AsyncQuerySpaceWorksInfo(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailUserSpaceHelper")]
		public static extern void delete_IRailUserSpaceHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_Close")]
		public static extern void IRailSpaceWork_Close(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetSpaceWorkID")]
		public static extern IntPtr IRailSpaceWork_GetSpaceWorkID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_Editable")]
		public static extern bool IRailSpaceWork_Editable(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_StartSync")]
		public static extern int IRailSpaceWork_StartSync(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetSyncProgress")]
		public static extern int IRailSpaceWork_GetSyncProgress(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_CancelSync")]
		public static extern int IRailSpaceWork_CancelSync(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetWorkLocalFolder")]
		public static extern int IRailSpaceWork_GetWorkLocalFolder(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_AsyncUpdateMetadata")]
		public static extern int IRailSpaceWork_AsyncUpdateMetadata(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetName")]
		public static extern int IRailSpaceWork_GetName(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetDescription")]
		public static extern int IRailSpaceWork_GetDescription(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetUrl")]
		public static extern int IRailSpaceWork_GetUrl(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetCreateTime")]
		public static extern uint IRailSpaceWork_GetCreateTime(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetLastUpdateTime")]
		public static extern uint IRailSpaceWork_GetLastUpdateTime(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetWorkFileSize")]
		public static extern ulong IRailSpaceWork_GetWorkFileSize(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetTags")]
		public static extern int IRailSpaceWork_GetTags(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetPreviewImage")]
		public static extern int IRailSpaceWork_GetPreviewImage(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetVersion")]
		public static extern int IRailSpaceWork_GetVersion(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetDownloadCount")]
		public static extern ulong IRailSpaceWork_GetDownloadCount(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetSubscribedCount")]
		public static extern ulong IRailSpaceWork_GetSubscribedCount(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetShareLevel")]
		public static extern int IRailSpaceWork_GetShareLevel(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetScore")]
		public static extern ulong IRailSpaceWork_GetScore(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetMetadata")]
		public static extern int IRailSpaceWork_GetMetadata(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetMyVote")]
		public static extern int IRailSpaceWork_GetMyVote(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_IsFavorite")]
		public static extern bool IRailSpaceWork_IsFavorite(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_IsSubscribed")]
		public static extern bool IRailSpaceWork_IsSubscribed(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SetName")]
		public static extern int IRailSpaceWork_SetName(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SetDescription")]
		public static extern int IRailSpaceWork_SetDescription(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SetTags")]
		public static extern int IRailSpaceWork_SetTags(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SetPreviewImage")]
		public static extern int IRailSpaceWork_SetPreviewImage(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SetVersion")]
		public static extern int IRailSpaceWork_SetVersion(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SetShareLevel__SWIG_0")]
		public static extern int IRailSpaceWork_SetShareLevel__SWIG_0(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SetShareLevel__SWIG_1")]
		public static extern int IRailSpaceWork_SetShareLevel__SWIG_1(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SetMetadata")]
		public static extern int IRailSpaceWork_SetMetadata(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SetContentFromFolder")]
		public static extern int IRailSpaceWork_SetContentFromFolder(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetAllMetadata")]
		public static extern int IRailSpaceWork_GetAllMetadata(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetAdditionalPreviewUrls")]
		public static extern int IRailSpaceWork_GetAdditionalPreviewUrls(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetAssociatedSpaceWorks")]
		public static extern int IRailSpaceWork_GetAssociatedSpaceWorks(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetLanguages")]
		public static extern int IRailSpaceWork_GetLanguages(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_RemoveMetadata")]
		public static extern int IRailSpaceWork_RemoveMetadata(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SetAdditionalPreviews")]
		public static extern int IRailSpaceWork_SetAdditionalPreviews(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SetAssociatedSpaceWorks")]
		public static extern int IRailSpaceWork_SetAssociatedSpaceWorks(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SetLanguages")]
		public static extern int IRailSpaceWork_SetLanguages(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetPreviewUrl__SWIG_0")]
		public static extern int IRailSpaceWork_GetPreviewUrl__SWIG_0(IntPtr jarg1, IntPtr jarg2, uint jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetPreviewUrl__SWIG_1")]
		public static extern int IRailSpaceWork_GetPreviewUrl__SWIG_1(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetVoteDetail")]
		public static extern int IRailSpaceWork_GetVoteDetail(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetUploaderIDs")]
		public static extern int IRailSpaceWork_GetUploaderIDs(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SetUpdateOptions")]
		public static extern int IRailSpaceWork_SetUpdateOptions(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetStatistic")]
		public static extern int IRailSpaceWork_GetStatistic(IntPtr jarg1, int jarg2, out ulong jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_RemovePreviewImage")]
		public static extern int IRailSpaceWork_RemovePreviewImage(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetState")]
		public static extern uint IRailSpaceWork_GetState(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_AddAssociatedGameIDs")]
		public static extern int IRailSpaceWork_AddAssociatedGameIDs(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_RemoveAssociatedGameIDs")]
		public static extern int IRailSpaceWork_RemoveAssociatedGameIDs(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetAssociatedGameIDs")]
		public static extern int IRailSpaceWork_GetAssociatedGameIDs(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_GetLocalVersion")]
		public static extern int IRailSpaceWork_GetLocalVersion(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailSpaceWork")]
		public static extern void delete_IRailSpaceWork(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUtils_GetTimeCountSinceGameLaunch")]
		public static extern uint IRailUtils_GetTimeCountSinceGameLaunch(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUtils_GetTimeCountSinceComputerLaunch")]
		public static extern uint IRailUtils_GetTimeCountSinceComputerLaunch(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUtils_GetTimeFromServer")]
		public static extern uint IRailUtils_GetTimeFromServer(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUtils_AsyncGetImageData")]
		public static extern int IRailUtils_AsyncGetImageData(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, uint jarg3, uint jarg4, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUtils_GetErrorString")]
		public static extern void IRailUtils_GetErrorString(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUtils_DirtyWordsFilter")]
		public static extern int IRailUtils_DirtyWordsFilter(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, bool jarg3, IntPtr jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUtils_GetRailPlatformType")]
		public static extern int IRailUtils_GetRailPlatformType(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUtils_GetLaunchAppParameters")]
		public static extern int IRailUtils_GetLaunchAppParameters(IntPtr jarg1, int jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUtils_GetPlatformLanguageCode")]
		public static extern int IRailUtils_GetPlatformLanguageCode(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUtils_SetWarningMessageCallback")]
		public static extern int IRailUtils_SetWarningMessageCallback(IntPtr jarg1, RailWarningMessageCallbackFunction jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailUtils_GetCountryCodeOfCurrentLoggedInIP")]
		public static extern int IRailUtils_GetCountryCodeOfCurrentLoggedInIP(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailUtils")]
		public static extern void delete_IRailUtils(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailApps_IsGameInstalled")]
		public static extern bool IRailApps_IsGameInstalled(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailApps_AsyncQuerySubscribeWishPlayState")]
		public static extern int IRailApps_AsyncQuerySubscribeWishPlayState(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailApps")]
		public static extern void delete_IRailApps(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailVoiceHelper")]
		public static extern void delete_IRailVoiceHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_AsyncCreateVoiceChannel")]
		public static extern IntPtr IRailVoiceHelper_AsyncCreateVoiceChannel(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg4, out RailResult jarg5);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_OpenVoiceChannel")]
		public static extern IntPtr IRailVoiceHelper_OpenVoiceChannel(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3, out RailResult jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_GetSpeakerState")]
		public static extern int IRailVoiceHelper_GetSpeakerState(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_MuteSpeaker")]
		public static extern int IRailVoiceHelper_MuteSpeaker(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_ResumeSpeaker")]
		public static extern int IRailVoiceHelper_ResumeSpeaker(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_SetupVoiceCapture__SWIG_0")]
		public static extern int IRailVoiceHelper_SetupVoiceCapture__SWIG_0(IntPtr jarg1, IntPtr jarg2, RailCaptureVoiceCallback jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_SetupVoiceCapture__SWIG_1")]
		public static extern int IRailVoiceHelper_SetupVoiceCapture__SWIG_1(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_StartVoiceCapturing__SWIG_0")]
		public static extern int IRailVoiceHelper_StartVoiceCapturing__SWIG_0(IntPtr jarg1, uint jarg2, bool jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_StartVoiceCapturing__SWIG_1")]
		public static extern int IRailVoiceHelper_StartVoiceCapturing__SWIG_1(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_StartVoiceCapturing__SWIG_2")]
		public static extern int IRailVoiceHelper_StartVoiceCapturing__SWIG_2(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_StopVoiceCapturing")]
		public static extern int IRailVoiceHelper_StopVoiceCapturing(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_GetCapturedVoiceData")]
		public static extern int IRailVoiceHelper_GetCapturedVoiceData(IntPtr jarg1, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg2, uint jarg3, out uint jarg4);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_DecodeVoice")]
		public static extern int IRailVoiceHelper_DecodeVoice(IntPtr jarg1, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg2, uint jarg3, [In][Out][MarshalAs(UnmanagedType.LPArray)] byte[] jarg4, uint jarg5, out uint jarg6);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_GetVoiceCaptureSpecification")]
		public static extern int IRailVoiceHelper_GetVoiceCaptureSpecification(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_EnableInGameVoiceSpeaking")]
		public static extern int IRailVoiceHelper_EnableInGameVoiceSpeaking(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_SetPlayerNicknameInVoiceChannel")]
		public static extern int IRailVoiceHelper_SetPlayerNicknameInVoiceChannel(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_SetPushToTalkKeyInVoiceChannel")]
		public static extern int IRailVoiceHelper_SetPushToTalkKeyInVoiceChannel(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_GetPushToTalkKeyInVoiceChannel")]
		public static extern uint IRailVoiceHelper_GetPushToTalkKeyInVoiceChannel(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_ShowOverlayUI")]
		public static extern int IRailVoiceHelper_ShowOverlayUI(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_SetMicroVolume")]
		public static extern int IRailVoiceHelper_SetMicroVolume(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceHelper_SetSpeakerVolume")]
		public static extern int IRailVoiceHelper_SetSpeakerVolume(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailVoiceChannel")]
		public static extern void delete_IRailVoiceChannel(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_GetVoiceChannelID")]
		public static extern IntPtr IRailVoiceChannel_GetVoiceChannelID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_GetVoiceChannelName")]
		public static extern IntPtr IRailVoiceChannel_GetVoiceChannelName(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_GetJoinState")]
		public static extern int IRailVoiceChannel_GetJoinState(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_AsyncJoinVoiceChannel")]
		public static extern int IRailVoiceChannel_AsyncJoinVoiceChannel(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_AsyncLeaveVoiceChannel")]
		public static extern int IRailVoiceChannel_AsyncLeaveVoiceChannel(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_GetUsers")]
		public static extern int IRailVoiceChannel_GetUsers(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_AsyncAddUsers")]
		public static extern int IRailVoiceChannel_AsyncAddUsers(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_AsyncRemoveUsers")]
		public static extern int IRailVoiceChannel_AsyncRemoveUsers(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_CloseChannel")]
		public static extern int IRailVoiceChannel_CloseChannel(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_SetSelfSpeaking")]
		public static extern int IRailVoiceChannel_SetSelfSpeaking(IntPtr jarg1, bool jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_IsSelfSpeaking")]
		public static extern bool IRailVoiceChannel_IsSelfSpeaking(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_AsyncSetUsersSpeakingState")]
		public static extern int IRailVoiceChannel_AsyncSetUsersSpeakingState(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_GetUsersSpeakingState")]
		public static extern int IRailVoiceChannel_GetUsersSpeakingState(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_GetSpeakingUsers")]
		public static extern int IRailVoiceChannel_GetSpeakingUsers(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_IsOwner")]
		public static extern bool IRailVoiceChannel_IsOwner(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailTextInputHelper_ShowTextInputWindow")]
		public static extern int IRailTextInputHelper_ShowTextInputWindow(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailTextInputHelper_GetTextInputContent")]
		public static extern void IRailTextInputHelper_GetTextInputContent(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailTextInputHelper_HideTextInputWindow")]
		public static extern int IRailTextInputHelper_HideTextInputWindow(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailTextInputHelper")]
		public static extern void delete_IRailTextInputHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_GetGameID")]
		public static extern IntPtr IRailGame_GetGameID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_ReportGameContentDamaged")]
		public static extern int IRailGame_ReportGameContentDamaged(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_GetGameInstallPath")]
		public static extern int IRailGame_GetGameInstallPath(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_AsyncQuerySubscribeWishPlayState")]
		public static extern int IRailGame_AsyncQuerySubscribeWishPlayState(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_GetPlayerSelectedLanguageCode")]
		public static extern int IRailGame_GetPlayerSelectedLanguageCode(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_GetGameSupportedLanguageCodes")]
		public static extern int IRailGame_GetGameSupportedLanguageCodes(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_SetGameState")]
		public static extern int IRailGame_SetGameState(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_GetGameState")]
		public static extern int IRailGame_GetGameState(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_RegisterGameDefineGamePlayingState")]
		public static extern int IRailGame_RegisterGameDefineGamePlayingState(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_SetGameDefineGamePlayingState")]
		public static extern int IRailGame_SetGameDefineGamePlayingState(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_GetGameDefineGamePlayingState")]
		public static extern int IRailGame_GetGameDefineGamePlayingState(IntPtr jarg1, out uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_GetBranchBuildNumber")]
		public static extern int IRailGame_GetBranchBuildNumber(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_GetCurrentBranchInfo")]
		public static extern int IRailGame_GetCurrentBranchInfo(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_StartGameTimeCounting")]
		public static extern int IRailGame_StartGameTimeCounting(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_EndGameTimeCounting")]
		public static extern int IRailGame_EndGameTimeCounting(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_GetGamePurchasePlayerRailID")]
		public static extern IntPtr IRailGame_GetGamePurchasePlayerRailID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_GetGameEarliestPurchaseTime")]
		public static extern uint IRailGame_GetGameEarliestPurchaseTime(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_GetTimeCountSinceGameActivated")]
		public static extern uint IRailGame_GetTimeCountSinceGameActivated(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGame_GetTimeCountSinceLastMouseMoved")]
		public static extern uint IRailGame_GetTimeCountSinceLastMouseMoved(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailGame")]
		public static extern void delete_IRailGame(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailIMEHelper_EnableIMEHelperTextInputWindow")]
		public static extern int IRailIMEHelper_EnableIMEHelperTextInputWindow(IntPtr jarg1, bool jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailIMEHelper_UpdateIMEHelperTextInputWindowPosition")]
		public static extern int IRailIMEHelper_UpdateIMEHelperTextInputWindowPosition(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailIMEHelper")]
		public static extern void delete_IRailIMEHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailSmallObjectServiceHelper")]
		public static extern void delete_IRailSmallObjectServiceHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSmallObjectServiceHelper_AsyncDownloadObjects")]
		public static extern int IRailSmallObjectServiceHelper_AsyncDownloadObjects(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSmallObjectServiceHelper_GetObjectContent")]
		public static extern int IRailSmallObjectServiceHelper_GetObjectContent(IntPtr jarg1, uint jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSmallObjectServiceHelper_AsyncQueryObjectState")]
		public static extern int IRailSmallObjectServiceHelper_AsyncQueryObjectState(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSystemHelper_SetTerminationTimeoutOwnershipExpired")]
		public static extern int IRailSystemHelper_SetTerminationTimeoutOwnershipExpired(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSystemHelper_GetPlatformSystemState")]
		public static extern int IRailSystemHelper_GetPlatformSystemState(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailSystemHelper")]
		public static extern void delete_IRailSystemHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpSessionHelper_CreateHttpSession")]
		public static extern IntPtr IRailHttpSessionHelper_CreateHttpSession(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpSessionHelper_CreateHttpResponse")]
		public static extern IntPtr IRailHttpSessionHelper_CreateHttpResponse(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailHttpSessionHelper")]
		public static extern void delete_IRailHttpSessionHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpSession_SetRequestMethod")]
		public static extern int IRailHttpSession_SetRequestMethod(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpSession_SetParameters")]
		public static extern int IRailHttpSession_SetParameters(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpSession_SetPostBodyContent")]
		public static extern int IRailHttpSession_SetPostBodyContent(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpSession_SetRequestTimeOut")]
		public static extern int IRailHttpSession_SetRequestTimeOut(IntPtr jarg1, uint jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpSession_SetRequestHeaders")]
		public static extern int IRailHttpSession_SetRequestHeaders(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpSession_AsyncSendRequest")]
		public static extern int IRailHttpSession_AsyncSendRequest(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailHttpSession")]
		public static extern void delete_IRailHttpSession(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpResponse_GetHttpResponseCode")]
		public static extern int IRailHttpResponse_GetHttpResponseCode(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpResponse_GetResponseHeaderKeys")]
		public static extern int IRailHttpResponse_GetResponseHeaderKeys(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpResponse_GetResponseHeaderValue")]
		public static extern IntPtr IRailHttpResponse_GetResponseHeaderValue(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpResponse_GetResponseBodyData")]
		public static extern IntPtr IRailHttpResponse_GetResponseBodyData(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpResponse_GetContentLength")]
		public static extern uint IRailHttpResponse_GetContentLength(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpResponse_GetContentType")]
		public static extern IntPtr IRailHttpResponse_GetContentType(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpResponse_GetContentRange")]
		public static extern IntPtr IRailHttpResponse_GetContentRange(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpResponse_GetContentLanguage")]
		public static extern IntPtr IRailHttpResponse_GetContentLanguage(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpResponse_GetContentEncoding")]
		public static extern IntPtr IRailHttpResponse_GetContentEncoding(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpResponse_GetLastModified")]
		public static extern IntPtr IRailHttpResponse_GetLastModified(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailHttpResponse")]
		public static extern void delete_IRailHttpResponse(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServerHelper_GetPlayerSelectedZoneID")]
		public static extern IntPtr IRailZoneServerHelper_GetPlayerSelectedZoneID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServerHelper_GetRootZoneID")]
		public static extern IntPtr IRailZoneServerHelper_GetRootZoneID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServerHelper_OpenZoneServer")]
		public static extern IntPtr IRailZoneServerHelper_OpenZoneServer(IntPtr jarg1, IntPtr jarg2, out RailResult jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServerHelper_AsyncSwitchPlayerSelectedZone")]
		public static extern int IRailZoneServerHelper_AsyncSwitchPlayerSelectedZone(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailZoneServerHelper")]
		public static extern void delete_IRailZoneServerHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServer_GetZoneID")]
		public static extern IntPtr IRailZoneServer_GetZoneID(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServer_GetZoneNameLanguages")]
		public static extern int IRailZoneServer_GetZoneNameLanguages(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServer_GetZoneName")]
		public static extern int IRailZoneServer_GetZoneName(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServer_GetZoneDescriptionLanguages")]
		public static extern int IRailZoneServer_GetZoneDescriptionLanguages(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServer_GetZoneDescription")]
		public static extern int IRailZoneServer_GetZoneDescription(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, IntPtr jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServer_GetGameServerAddresses")]
		public static extern int IRailZoneServer_GetGameServerAddresses(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServer_GetZoneMetadatas")]
		public static extern int IRailZoneServer_GetZoneMetadatas(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServer_GetChildrenZoneIDs")]
		public static extern int IRailZoneServer_GetChildrenZoneIDs(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServer_IsZoneVisiable")]
		public static extern bool IRailZoneServer_IsZoneVisiable(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServer_IsZoneJoinable")]
		public static extern bool IRailZoneServer_IsZoneJoinable(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServer_GetZoneEnableStartTime")]
		public static extern uint IRailZoneServer_GetZoneEnableStartTime(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServer_GetZoneEnableEndTime")]
		public static extern uint IRailZoneServer_GetZoneEnableEndTime(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailZoneServer")]
		public static extern void delete_IRailZoneServer(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGroupChatHelper_AsyncQueryGroupsInfo")]
		public static extern int IRailGroupChatHelper_AsyncQueryGroupsInfo(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGroupChatHelper_AsyncOpenGroupChat")]
		public static extern IntPtr IRailGroupChatHelper_AsyncOpenGroupChat(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailGroupChatHelper")]
		public static extern void delete_IRailGroupChatHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGroupChat_GetGroupInfo")]
		public static extern int IRailGroupChat_GetGroupInfo(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGroupChat_OpenGroupWindow")]
		public static extern int IRailGroupChat_OpenGroupWindow(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailGroupChat")]
		public static extern void delete_IRailGroupChat(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailInGameStorePurchaseHelper_AsyncShowPaymentWindow")]
		public static extern int IRailInGameStorePurchaseHelper_AsyncShowPaymentWindow(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailInGameStorePurchaseHelper")]
		public static extern void delete_IRailInGameStorePurchaseHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAntiAddictionHelper_AsyncQueryGameOnlineTime")]
		public static extern int IRailAntiAddictionHelper_AsyncQueryGameOnlineTime(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailAntiAddictionHelper")]
		public static extern void delete_IRailAntiAddictionHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailThirdPartyAccountLoginHelper_AsyncAutoLogin")]
		public static extern int IRailThirdPartyAccountLoginHelper_AsyncAutoLogin(IntPtr jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailThirdPartyAccountLoginHelper_AsyncLogin")]
		public static extern int IRailThirdPartyAccountLoginHelper_AsyncLogin(IntPtr jarg1, IntPtr jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "UTF8Marshaler")] string jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailThirdPartyAccountLoginHelper_GetAccountInfo")]
		public static extern int IRailThirdPartyAccountLoginHelper_GetAccountInfo(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailThirdPartyAccountLoginHelper")]
		public static extern void delete_IRailThirdPartyAccountLoginHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailPlayer")]
		public static extern IntPtr IRailFactory_RailPlayer(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailUsersHelper")]
		public static extern IntPtr IRailFactory_RailUsersHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailFriends")]
		public static extern IntPtr IRailFactory_RailFriends(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailFloatingWindow")]
		public static extern IntPtr IRailFactory_RailFloatingWindow(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailBrowserHelper")]
		public static extern IntPtr IRailFactory_RailBrowserHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailInGamePurchase")]
		public static extern IntPtr IRailFactory_RailInGamePurchase(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailInGameCoin")]
		public static extern IntPtr IRailFactory_RailInGameCoin(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailRoomHelper")]
		public static extern IntPtr IRailFactory_RailRoomHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailGameServerHelper")]
		public static extern IntPtr IRailFactory_RailGameServerHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailStorageHelper")]
		public static extern IntPtr IRailFactory_RailStorageHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailUserSpaceHelper")]
		public static extern IntPtr IRailFactory_RailUserSpaceHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailStatisticHelper")]
		public static extern IntPtr IRailFactory_RailStatisticHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailLeaderboardHelper")]
		public static extern IntPtr IRailFactory_RailLeaderboardHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailAchievementHelper")]
		public static extern IntPtr IRailFactory_RailAchievementHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailNetworkHelper")]
		public static extern IntPtr IRailFactory_RailNetworkHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailApps")]
		public static extern IntPtr IRailFactory_RailApps(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailGame")]
		public static extern IntPtr IRailFactory_RailGame(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailUtils")]
		public static extern IntPtr IRailFactory_RailUtils(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailAssetsHelper")]
		public static extern IntPtr IRailFactory_RailAssetsHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailDlcHelper")]
		public static extern IntPtr IRailFactory_RailDlcHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailScreenshotHelper")]
		public static extern IntPtr IRailFactory_RailScreenshotHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailVoiceHelper")]
		public static extern IntPtr IRailFactory_RailVoiceHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailSystemHelper")]
		public static extern IntPtr IRailFactory_RailSystemHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailTextInputHelper")]
		public static extern IntPtr IRailFactory_RailTextInputHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailIMETextInputHelper")]
		public static extern IntPtr IRailFactory_RailIMETextInputHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailHttpSessionHelper")]
		public static extern IntPtr IRailFactory_RailHttpSessionHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailSmallObjectServiceHelper")]
		public static extern IntPtr IRailFactory_RailSmallObjectServiceHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailZoneServerHelper")]
		public static extern IntPtr IRailFactory_RailZoneServerHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailGroupChatHelper")]
		public static extern IntPtr IRailFactory_RailGroupChatHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailInGameStorePurchaseHelper")]
		public static extern IntPtr IRailFactory_RailInGameStorePurchaseHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailInGameActivityHelper")]
		public static extern IntPtr IRailFactory_RailInGameActivityHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailAntiAddictionHelper")]
		public static extern IntPtr IRailFactory_RailAntiAddictionHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFactory_RailThirdPartyAccountLoginHelper")]
		public static extern IntPtr IRailFactory_RailThirdPartyAccountLoginHelper(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_delete_IRailFactory")]
		public static extern void delete_IRailFactory(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNeedRestartAppForCheckingEnvironment")]
		public static extern bool RailNeedRestartAppForCheckingEnvironment(IntPtr jarg1, int jarg2, [In][MarshalAs(UnmanagedType.LPArray)] string[] jarg3);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInitialize")]
		public static extern bool RailInitialize();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFinalize")]
		public static extern void RailFinalize();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFireEvents")]
		public static extern void RailFireEvents();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFactory")]
		public static extern IntPtr RailFactory();

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetSdkVersion")]
		public static extern void RailGetSdkVersion(IntPtr jarg1, IntPtr jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CSharpRailRegisterEvent")]
		public static extern void CSharpRailRegisterEvent(int jarg1, RailEventCallBackFunction jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CSharpRailUnRegisterEvent")]
		public static extern void CSharpRailUnRegisterEvent(int jarg1, RailEventCallBackFunction jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_CSharpRailUnRegisterAllEvent")]
		public static extern void CSharpRailUnRegisterAllEvent();

		[DllImport("rail_api64", EntryPoint = "CSharp_NewInt")]
		public static extern IntPtr NewInt();

		[DllImport("rail_api64", EntryPoint = "CSharp_DeleteInt")]
		public static extern void DeleteInt(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetInt")]
		public static extern int GetInt(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetInt")]
		public static extern void SetInt(IntPtr jarg1, int jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_NewInt64")]
		public static extern IntPtr NewInt64();

		[DllImport("rail_api64", EntryPoint = "CSharp_GetInt64")]
		public static extern long GetInt64(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetInt64")]
		public static extern void SetInt64(IntPtr jarg1, long jarg2);

		[DllImport("rail_api64", EntryPoint = "CSharp_DeleteInt64")]
		public static extern void DeleteInt64(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetInt8")]
		public static extern sbyte GetInt8(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsRequestAllAssetsFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsRequestAllAssetsFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetRoomTypeResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomSetRoomTypeResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsMergeToFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsMergeToFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceSubscribeResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceSubscribeResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyMemberChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomNotifyMemberChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchasePurchaseProductsResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGamePurchasePurchaseProductsResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerGetEncryptedGameTicketResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventPlayerGetEncryptedGameTicketResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerListResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventGameServerListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetRoomMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomSetRoomMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserCloseResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventBrowserCloseResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventNetworkCreateSessionRequest_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventNetworkCreateSessionRequest_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsExchangeAssetsToFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsExchangeAssetsToFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailPlatformNotifyEventJoinGameByUser_SWIGUpcast")]
		public static extern IntPtr RailEventkRailPlatformNotifyEventJoinGameByUser_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcInstallProgress_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventDlcInstallProgress_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersNotifyInviter_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUsersNotifyInviter_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsQueryPlayedWithFriendsListResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFriendsQueryPlayedWithFriendsListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardAsyncCreated_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventLeaderboardAsyncCreated_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardAttachSpaceWork_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventLeaderboardAttachSpaceWork_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsMetadataChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFriendsMetadataChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetUserRoomListResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomGetUserRoomListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserTryNavigateNewPageRequest_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventBrowserTryNavigateNewPageRequest_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcUninstallFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventDlcUninstallFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsSplitFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsSplitFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsQueryPlayedWithFriendsGamesResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFriendsQueryPlayedWithFriendsGamesResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardUploaded_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventLeaderboardUploaded_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelRemoveUsersResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventVoiceChannelRemoveUsersResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSmallObjectServiceQueryObjectStateResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventSmallObjectServiceQueryObjectStateResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUtilsGameSettingMetadataChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUtilsGameSettingMetadataChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerCreated_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventGameServerCreated_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcInstallStart_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventDlcInstallStart_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersGetInviteDetailResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUsersGetInviteDetailResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceQuerySpaceWorksResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceQuerySpaceWorksResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAchievementGlobalAchievementReceived_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAchievementGlobalAchievementReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncRenameStreamFileResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventStorageAsyncRenameStreamFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsReportPlayedWithUserListResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFriendsReportPlayedWithUserListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelInviteEvent_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventVoiceChannelInviteEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncWriteStreamFileResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventStorageAsyncWriteStreamFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomKickOffMemberResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomKickOffMemberResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventShowFloatingWindow_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventShowFloatingWindow_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyRoomOwnerChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomNotifyRoomOwnerChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFinalize_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFinalize_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersCancelInviteResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUsersCancelInviteResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcInstallStartResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventDlcInstallStartResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailThirdPartyAccountLoginResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailThirdPartyAccountLoginResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserDamageRectPaint_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventBrowserDamageRectPaint_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsUpdateAssetPropertyFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsUpdateAssetPropertyFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetRoomMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomGetRoomMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventZoneServerSwitchPlayerSelectedZoneResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventZoneServerSwitchPlayerSelectedZoneResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerGetGamePurchaseKey_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventPlayerGetGamePurchaseKey_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailPlatformNotifyEventJoinGameByRoom_SWIGUpcast")]
		public static extern IntPtr RailEventkRailPlatformNotifyEventJoinGameByRoom_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetMemberMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomSetMemberMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceModifyFavoritesWorksResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceModifyFavoritesWorksResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelSpeakingUsersChangedEvent_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventVoiceChannelSpeakingUsersChangedEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersRespondInvitation_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUsersRespondInvitation_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsDirectConsumeFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsDirectConsumeFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncWriteFileResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventStorageAsyncWriteFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceGetMyFavoritesWorksResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceGetMyFavoritesWorksResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyRoomDestroyed_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomNotifyRoomDestroyed_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserPaint_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventBrowserPaint_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsExchangeAssetsFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsExchangeAssetsFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchaseAllPurchasableProductsInfoReceived_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGamePurchaseAllPurchasableProductsInfoReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsGetFriendPlayedGamesResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFriendsGetFriendPlayedGamesResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsClearMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFriendsClearMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelMemberChangedEvent_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventVoiceChannelMemberChangedEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsAddFriendResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFriendsAddFriendResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchaseAllProductsInfoReceived_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGamePurchaseAllProductsInfoReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsMergeFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsMergeFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyMemberkicked_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomNotifyMemberkicked_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomJoinRoomResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomJoinRoomResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerAuthSessionTicket_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventGameServerAuthSessionTicket_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsQueryPlayedWithFriendsTimeResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFriendsQueryPlayedWithFriendsTimeResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerRegisterToServerListResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventGameServerRegisterToServerListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameStorePurchasePaymentResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGameStorePurchasePaymentResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcCheckAllDlcsStateReadyResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventDlcCheckAllDlcsStateReadyResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventScreenshotTakeScreenshotFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventScreenshotTakeScreenshotFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameActivityNotifyNewGameActivities_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGameActivityNotifyNewGameActivities_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventShowFloatingNotifyWindow_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventShowFloatingNotifyWindow_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchasePurchaseProductsToAssetsResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGamePurchasePurchaseProductsToAssetsResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserReloadResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventBrowserReloadResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetRoomMaxMemberResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomSetRoomMaxMemberResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersShowUserHomepageWindowResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUsersShowUserHomepageWindowResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameStorePurchasePayWindowDisplayed_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGameStorePurchasePayWindowDisplayed_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventNetworkCreateRawSessionRequest_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventNetworkCreateRawSessionRequest_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventIMEHelperTextInputSelectedResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventIMEHelperTextInputSelectedResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGroupChatQueryGroupsInfoResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventGroupChatQueryGroupsInfoResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventTextInputShowTextInputWindowResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventTextInputShowTextInputWindowResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerSetMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventGameServerSetMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceDownloadProgress_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceDownloadProgress_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAntiAddictionQueryGameOnlineTimeResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAntiAddictionQueryGameOnlineTimeResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceQuerySpaceWorksInfoResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceQuerySpaceWorksInfoResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameCoinRequestCoinInfoResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGameCoinRequestCoinInfoResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsOnlineStateChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFriendsOnlineStateChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceUpdateMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceUpdateMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailThirdPartyAccountLoginNotifyQrCodeInfo_SWIGUpcast")]
		public static extern IntPtr RailEventkRailThirdPartyAccountLoginNotifyQrCodeInfo_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsUpdateConsumeFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsUpdateConsumeFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetRoomMembersResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomGetRoomMembersResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerGetPlayerMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventPlayerGetPlayerMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelJoinedResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventVoiceChannelJoinedResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGamePurchaseFinishOrderResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGamePurchaseFinishOrderResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsStartConsumeFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsStartConsumeFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerFavoriteGameServers_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventGameServerFavoriteGameServers_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserJavascriptEvent_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventBrowserJavascriptEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAchievementPlayerAchievementReceived_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAchievementPlayerAchievementReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomLeaveRoomResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomLeaveRoomResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomCreated_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomCreated_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageQueryQuotaResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventStorageQueryQuotaResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcOwnershipChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventDlcOwnershipChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserNavigeteResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventBrowserNavigeteResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsGetInviteCommandLine_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFriendsGetInviteCommandLine_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelUsersSpeakingStateChangedEvent_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventVoiceChannelUsersSpeakingStateChangedEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStatsNumOfPlayerReceived_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventStatsNumOfPlayerReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceVoteSpaceWorkResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceVoteSpaceWorkResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameActivityQueryGameActivityResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGameActivityQueryGameActivityResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcInstallFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventDlcInstallFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStatsGlobalStatsReceived_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventStatsGlobalStatsReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceSearchSpaceWorkResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceSearchSpaceWorkResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelCreateResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventVoiceChannelCreateResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsCompleteConsumeByExchangeAssetsToFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsCompleteConsumeByExchangeAssetsToFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceRateSpaceWorkResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceRateSpaceWorkResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetMemberMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomGetMemberMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsGetMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFriendsGetMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelPushToTalkKeyChangedEvent_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventVoiceChannelPushToTalkKeyChangedEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStatsPlayerStatsReceived_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventStatsPlayerStatsReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsCompleteConsumeFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsCompleteConsumeFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameActivityOpenGameActivityWindowResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGameActivityOpenGameActivityWindowResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerGetMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventGameServerGetMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerGetAuthenticateURL_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventPlayerGetAuthenticateURL_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelLeaveResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventVoiceChannelLeaveResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyMetadataChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomNotifyMetadataChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAchievementPlayerAchievementStored_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAchievementPlayerAchievementStored_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameStorePurchasePayWindowClosed_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGameStorePurchasePayWindowClosed_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomOpenRoomResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomOpenRoomResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelDataCaptured_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventVoiceChannelDataCaptured_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventPlayerAntiAddictionGameOnlineTimeChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventPlayerAntiAddictionGameOnlineTimeChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetRoomTagResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomSetRoomTagResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetRoomListResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomGetRoomListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomSetNewRoomOwnerResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomSetNewRoomOwnerResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncReadStreamFileResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventStorageAsyncReadStreamFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserTitleChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventBrowserTitleChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersShowChatWindowWithFriendResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUsersShowChatWindowWithFriendResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceGetMySubscribedWorksResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceGetMySubscribedWorksResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageShareToSpaceWorkResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventStorageShareToSpaceWorkResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsFriendsListChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFriendsFriendsListChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUtilsGetImageDataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUtilsGetImageDataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersGetUserLimitsResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUsersGetUserLimitsResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSystemStateChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventSystemStateChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardReceived_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventLeaderboardReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersInviteUsersResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUsersInviteUsersResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventHttpSessionResponseResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventHttpSessionResponseResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceDownloadResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceDownloadResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomClearRoomMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomClearRoomMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSmallObjectServiceDownloadResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventSmallObjectServiceDownloadResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetAllDataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomGetAllDataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncListStreamFileResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventStorageAsyncListStreamFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGroupChatOpenGroupChatResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventGroupChatOpenGroupChatResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventLeaderboardEntryReceived_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventLeaderboardEntryReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyRoomGameServerChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomNotifyRoomGameServerChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomNotifyRoomDataReceived_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomNotifyRoomDataReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcQueryIsOwnedDlcsResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventDlcQueryIsOwnedDlcsResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceSyncResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceSyncResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsAssetsChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsAssetsChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventIMEHelperTextInputCompositionStateChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventIMEHelperTextInputCompositionStateChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAntiAddictionCustomizeAntiAddictionActions_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAntiAddictionCustomizeAntiAddictionActions_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventDlcRefundChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventDlcRefundChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerPlayerListResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventGameServerPlayerListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersGetUsersInfo_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUsersGetUsersInfo_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncDeleteStreamFileResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventStorageAsyncDeleteStreamFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventFriendsSetMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventFriendsSetMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserCreateResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventBrowserCreateResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAssetsSplitToFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAssetsSplitToFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventScreenshotPublishScreenshotFinished_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventScreenshotPublishScreenshotFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventVoiceChannelAddUsersResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventVoiceChannelAddUsersResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerAddFavoriteGameServer_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventGameServerAddFavoriteGameServer_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSessionTicketGetSessionTicket_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventSessionTicketGetSessionTicket_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerRemoveFavoriteGameServer_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventGameServerRemoveFavoriteGameServer_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventRoomGetRoomTagResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventRoomGetRoomTagResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStatsPlayerStatsStored_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventStatsPlayerStatsStored_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventStorageAsyncReadFileResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventStorageAsyncReadFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventSessionTicketAuthSessionTicket_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventSessionTicketAuthSessionTicket_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventScreenshotTakeScreenshotRequest_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventScreenshotTakeScreenshotRequest_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUserSpaceRemoveSpaceWorkResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUserSpaceRemoveSpaceWorkResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventBrowserStateChanged_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventBrowserStateChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventGameServerGetSessionTicket_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventGameServerGetSessionTicket_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventUsersInviteJoinGameResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventUsersInviteJoinGameResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventNetworkCreateRawSessionFailed_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventNetworkCreateRawSessionFailed_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameCoinPurchaseCoinsResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGameCoinPurchaseCoinsResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventAppQuerySubscribeWishPlayStateResult_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventAppQuerySubscribeWishPlayStateResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventNetworkCreateSessionFailed_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventNetworkCreateSessionFailed_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventQueryPlayerBannedStatus_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventQueryPlayerBannedStatus_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailEventInGameActivityGameActivityPlayerEvent_SWIGUpcast")]
		public static extern IntPtr RailEventkRailEventInGameActivityGameActivityPlayerEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailEventkRailPlatformNotifyEventJoinGameByGameServer_SWIGUpcast")]
		public static extern IntPtr RailEventkRailPlatformNotifyEventJoinGameByGameServer_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncGetMySubscribedWorksResult_SWIGUpcast")]
		public static extern IntPtr AsyncGetMySubscribedWorksResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncGetMyFavoritesWorksResult_SWIGUpcast")]
		public static extern IntPtr AsyncGetMyFavoritesWorksResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncQuerySpaceWorksResult_SWIGUpcast")]
		public static extern IntPtr AsyncQuerySpaceWorksResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncUpdateMetadataResult_SWIGUpcast")]
		public static extern IntPtr AsyncUpdateMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SyncSpaceWorkResult_SWIGUpcast")]
		public static extern IntPtr SyncSpaceWorkResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncSubscribeSpaceWorksResult_SWIGUpcast")]
		public static extern IntPtr AsyncSubscribeSpaceWorksResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncModifyFavoritesWorksResult_SWIGUpcast")]
		public static extern IntPtr AsyncModifyFavoritesWorksResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRemoveSpaceWorkResult_SWIGUpcast")]
		public static extern IntPtr AsyncRemoveSpaceWorkResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncVoteSpaceWorkResult_SWIGUpcast")]
		public static extern IntPtr AsyncVoteSpaceWorkResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRateSpaceWorkResult_SWIGUpcast")]
		public static extern IntPtr AsyncRateSpaceWorkResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncSearchSpaceWorksResult_SWIGUpcast")]
		public static extern IntPtr AsyncSearchSpaceWorksResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_UserSpaceDownloadProgress_SWIGUpcast")]
		public static extern IntPtr UserSpaceDownloadProgress_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_UserSpaceDownloadResult_SWIGUpcast")]
		public static extern IntPtr UserSpaceDownloadResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncQuerySpaceWorksInfoResult_SWIGUpcast")]
		public static extern IntPtr AsyncQuerySpaceWorksInfoResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersInfoData_SWIGUpcast")]
		public static extern IntPtr RailUsersInfoData_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersNotifyInviter_SWIGUpcast")]
		public static extern IntPtr RailUsersNotifyInviter_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersRespondInvitation_SWIGUpcast")]
		public static extern IntPtr RailUsersRespondInvitation_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersInviteJoinGameResult_SWIGUpcast")]
		public static extern IntPtr RailUsersInviteJoinGameResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersGetInviteDetailResult_SWIGUpcast")]
		public static extern IntPtr RailUsersGetInviteDetailResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersCancelInviteResult_SWIGUpcast")]
		public static extern IntPtr RailUsersCancelInviteResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersInviteUsersResult_SWIGUpcast")]
		public static extern IntPtr RailUsersInviteUsersResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailUsersGetUserLimitsResult_SWIGUpcast")]
		public static extern IntPtr RailUsersGetUserLimitsResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailShowChatWindowWithFriendResult_SWIGUpcast")]
		public static extern IntPtr RailShowChatWindowWithFriendResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailShowUserHomepageWindowResult_SWIGUpcast")]
		public static extern IntPtr RailShowUserHomepageWindowResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetImageDataResult_SWIGUpcast")]
		public static extern IntPtr RailGetImageDataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameSettingMetadataChanged_SWIGUpcast")]
		public static extern IntPtr RailGameSettingMetadataChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_TakeScreenshotResult_SWIGUpcast")]
		public static extern IntPtr TakeScreenshotResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_ScreenshotRequestInfo_SWIGUpcast")]
		public static extern IntPtr ScreenshotRequestInfo_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PublishScreenshotResult_SWIGUpcast")]
		public static extern IntPtr PublishScreenshotResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSystemStateChanged_SWIGUpcast")]
		public static extern IntPtr RailSystemStateChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByGameServer_SWIGUpcast")]
		public static extern IntPtr RailPlatformNotifyEventJoinGameByGameServer_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByRoom_SWIGUpcast")]
		public static extern IntPtr RailPlatformNotifyEventJoinGameByRoom_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailPlatformNotifyEventJoinGameByUser_SWIGUpcast")]
		public static extern IntPtr RailPlatformNotifyEventJoinGameByUser_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFinalize_SWIGUpcast")]
		public static extern IntPtr RailFinalize_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RequestAllAssetsFinished_SWIGUpcast")]
		public static extern IntPtr RequestAllAssetsFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_UpdateAssetsPropertyFinished_SWIGUpcast")]
		public static extern IntPtr UpdateAssetsPropertyFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DirectConsumeAssetsFinished_SWIGUpcast")]
		public static extern IntPtr DirectConsumeAssetsFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_StartConsumeAssetsFinished_SWIGUpcast")]
		public static extern IntPtr StartConsumeAssetsFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_UpdateConsumeAssetsFinished_SWIGUpcast")]
		public static extern IntPtr UpdateConsumeAssetsFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CompleteConsumeAssetsFinished_SWIGUpcast")]
		public static extern IntPtr CompleteConsumeAssetsFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsFinished_SWIGUpcast")]
		public static extern IntPtr SplitAssetsFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SplitAssetsToFinished_SWIGUpcast")]
		public static extern IntPtr SplitAssetsToFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_MergeAssetsFinished_SWIGUpcast")]
		public static extern IntPtr MergeAssetsFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_MergeAssetsToFinished_SWIGUpcast")]
		public static extern IntPtr MergeAssetsToFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CompleteConsumeByExchangeAssetsToFinished_SWIGUpcast")]
		public static extern IntPtr CompleteConsumeByExchangeAssetsToFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_ExchangeAssetsFinished_SWIGUpcast")]
		public static extern IntPtr ExchangeAssetsFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_ExchangeAssetsToFinished_SWIGUpcast")]
		public static extern IntPtr ExchangeAssetsToFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAssetsChanged_SWIGUpcast")]
		public static extern IntPtr RailAssetsChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateBrowserResult_SWIGUpcast")]
		public static extern IntPtr CreateBrowserResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_ReloadBrowserResult_SWIGUpcast")]
		public static extern IntPtr ReloadBrowserResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CloseBrowserResult_SWIGUpcast")]
		public static extern IntPtr CloseBrowserResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_JavascriptEventResult_SWIGUpcast")]
		public static extern IntPtr JavascriptEventResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserNeedsPaintRequest_SWIGUpcast")]
		public static extern IntPtr BrowserNeedsPaintRequest_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserDamageRectNeedsPaintRequest_SWIGUpcast")]
		public static extern IntPtr BrowserDamageRectNeedsPaintRequest_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserRenderNavigateResult_SWIGUpcast")]
		public static extern IntPtr BrowserRenderNavigateResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserRenderStateChanged_SWIGUpcast")]
		public static extern IntPtr BrowserRenderStateChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserRenderTitleChanged_SWIGUpcast")]
		public static extern IntPtr BrowserRenderTitleChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_BrowserTryNavigateNewPageRequest_SWIGUpcast")]
		public static extern IntPtr BrowserTryNavigateNewPageRequest_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallStart_SWIGUpcast")]
		public static extern IntPtr DlcInstallStart_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallStartResult_SWIGUpcast")]
		public static extern IntPtr DlcInstallStartResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallProgress_SWIGUpcast")]
		public static extern IntPtr DlcInstallProgress_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcInstallFinished_SWIGUpcast")]
		public static extern IntPtr DlcInstallFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcUninstallFinished_SWIGUpcast")]
		public static extern IntPtr DlcUninstallFinished_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CheckAllDlcsStateReadyResult_SWIGUpcast")]
		public static extern IntPtr CheckAllDlcsStateReadyResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_QueryIsOwnedDlcsResult_SWIGUpcast")]
		public static extern IntPtr QueryIsOwnedDlcsResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcOwnershipChanged_SWIGUpcast")]
		public static extern IntPtr DlcOwnershipChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_DlcRefundChanged_SWIGUpcast")]
		public static extern IntPtr DlcRefundChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_ShowFloatingWindowResult_SWIGUpcast")]
		public static extern IntPtr ShowFloatingWindowResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_ShowNotifyWindow_SWIGUpcast")]
		public static extern IntPtr ShowNotifyWindow_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncAcquireGameServerSessionTicketResponse_SWIGUpcast")]
		public static extern IntPtr AsyncAcquireGameServerSessionTicketResponse_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerStartSessionWithPlayerResponse_SWIGUpcast")]
		public static extern IntPtr GameServerStartSessionWithPlayerResponse_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateGameServerResult_SWIGUpcast")]
		public static extern IntPtr CreateGameServerResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetGameServerMetadataResult_SWIGUpcast")]
		public static extern IntPtr SetGameServerMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerMetadataResult_SWIGUpcast")]
		public static extern IntPtr GetGameServerMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GameServerRegisterToServerListResult_SWIGUpcast")]
		public static extern IntPtr GameServerRegisterToServerListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerPlayerListResult_SWIGUpcast")]
		public static extern IntPtr GetGameServerPlayerListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetGameServerListResult_SWIGUpcast")]
		public static extern IntPtr GetGameServerListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncGetFavoriteGameServersResult_SWIGUpcast")]
		public static extern IntPtr AsyncGetFavoriteGameServersResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncAddFavoriteGameServerResult_SWIGUpcast")]
		public static extern IntPtr AsyncAddFavoriteGameServerResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRemoveFavoriteGameServerResult_SWIGUpcast")]
		public static extern IntPtr AsyncRemoveFavoriteGameServerResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AcquireSessionTicketResponse_SWIGUpcast")]
		public static extern IntPtr AcquireSessionTicketResponse_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_StartSessionWithPlayerResponse_SWIGUpcast")]
		public static extern IntPtr StartSessionWithPlayerResponse_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerGetGamePurchaseKeyResult_SWIGUpcast")]
		public static extern IntPtr PlayerGetGamePurchaseKeyResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_QueryPlayerBannedStatus_SWIGUpcast")]
		public static extern IntPtr QueryPlayerBannedStatus_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetAuthenticateURLResult_SWIGUpcast")]
		public static extern IntPtr GetAuthenticateURLResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailAntiAddictionGameOnlineTimeChanged_SWIGUpcast")]
		public static extern IntPtr RailAntiAddictionGameOnlineTimeChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetEncryptedGameTicketResult_SWIGUpcast")]
		public static extern IntPtr RailGetEncryptedGameTicketResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGetPlayerMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailGetPlayerMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsSetMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailFriendsSetMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsGetMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailFriendsGetMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsClearMetadataResult_SWIGUpcast")]
		public static extern IntPtr RailFriendsClearMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsGetInviteCommandLine_SWIGUpcast")]
		public static extern IntPtr RailFriendsGetInviteCommandLine_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsReportPlayedWithUserListResult_SWIGUpcast")]
		public static extern IntPtr RailFriendsReportPlayedWithUserListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsListChanged_SWIGUpcast")]
		public static extern IntPtr RailFriendsListChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsQueryFriendPlayedGamesResult_SWIGUpcast")]
		public static extern IntPtr RailFriendsQueryFriendPlayedGamesResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsQueryPlayedWithFriendsListResult_SWIGUpcast")]
		public static extern IntPtr RailFriendsQueryPlayedWithFriendsListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsQueryPlayedWithFriendsTimeResult_SWIGUpcast")]
		public static extern IntPtr RailFriendsQueryPlayedWithFriendsTimeResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsQueryPlayedWithFriendsGamesResult_SWIGUpcast")]
		public static extern IntPtr RailFriendsQueryPlayedWithFriendsGamesResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsAddFriendResult_SWIGUpcast")]
		public static extern IntPtr RailFriendsAddFriendResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsOnlineStateChanged_SWIGUpcast")]
		public static extern IntPtr RailFriendsOnlineStateChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailFriendsMetadataChanged_SWIGUpcast")]
		public static extern IntPtr RailFriendsMetadataChanged_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGameCoinRequestCoinInfoResponse_SWIGUpcast")]
		public static extern IntPtr RailInGameCoinRequestCoinInfoResponse_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGameCoinPurchaseCoinsResponse_SWIGUpcast")]
		public static extern IntPtr RailInGameCoinPurchaseCoinsResponse_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchaseRequestAllPurchasableProductsResponse_SWIGUpcast")]
		public static extern IntPtr RailInGamePurchaseRequestAllPurchasableProductsResponse_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchaseRequestAllProductsResponse_SWIGUpcast")]
		public static extern IntPtr RailInGamePurchaseRequestAllProductsResponse_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchasePurchaseProductsResponse_SWIGUpcast")]
		public static extern IntPtr RailInGamePurchasePurchaseProductsResponse_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchasePurchaseProductsToAssetsResponse_SWIGUpcast")]
		public static extern IntPtr RailInGamePurchasePurchaseProductsToAssetsResponse_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGamePurchaseFinishOrderResponse_SWIGUpcast")]
		public static extern IntPtr RailInGamePurchaseFinishOrderResponse_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGameStorePurchasePayWindowDisplayed_SWIGUpcast")]
		public static extern IntPtr RailInGameStorePurchasePayWindowDisplayed_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGameStorePurchasePayWindowClosed_SWIGUpcast")]
		public static extern IntPtr RailInGameStorePurchasePayWindowClosed_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailInGameStorePurchaseResult_SWIGUpcast")]
		public static extern IntPtr RailInGameStorePurchaseResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryGameActivityResult_SWIGUpcast")]
		public static extern IntPtr RailQueryGameActivityResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailOpenGameActivityWindowResult_SWIGUpcast")]
		public static extern IntPtr RailOpenGameActivityWindowResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNotifyNewGameActivities_SWIGUpcast")]
		public static extern IntPtr RailNotifyNewGameActivities_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailGameActivityPlayerEvent_SWIGUpcast")]
		public static extern IntPtr RailGameActivityPlayerEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateSessionRequest_SWIGUpcast")]
		public static extern IntPtr CreateSessionRequest_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateSessionFailed_SWIGUpcast")]
		public static extern IntPtr CreateSessionFailed_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NetworkCreateRawSessionRequest_SWIGUpcast")]
		public static extern IntPtr NetworkCreateRawSessionRequest_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NetworkCreateRawSessionFailed_SWIGUpcast")]
		public static extern IntPtr NetworkCreateRawSessionFailed_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateRoomResult_SWIGUpcast")]
		public static extern IntPtr CreateRoomResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_OpenRoomResult_SWIGUpcast")]
		public static extern IntPtr OpenRoomResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomListResult_SWIGUpcast")]
		public static extern IntPtr GetRoomListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetUserRoomListResult_SWIGUpcast")]
		public static extern IntPtr GetUserRoomListResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetNewRoomOwnerResult_SWIGUpcast")]
		public static extern IntPtr SetNewRoomOwnerResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomMembersResult_SWIGUpcast")]
		public static extern IntPtr GetRoomMembersResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaveRoomResult_SWIGUpcast")]
		public static extern IntPtr LeaveRoomResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_JoinRoomResult_SWIGUpcast")]
		public static extern IntPtr JoinRoomResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetAllRoomDataResult_SWIGUpcast")]
		public static extern IntPtr GetAllRoomDataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_KickOffMemberResult_SWIGUpcast")]
		public static extern IntPtr KickOffMemberResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetRoomTagResult_SWIGUpcast")]
		public static extern IntPtr SetRoomTagResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomTagResult_SWIGUpcast")]
		public static extern IntPtr GetRoomTagResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetRoomMetadataResult_SWIGUpcast")]
		public static extern IntPtr SetRoomMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetRoomMetadataResult_SWIGUpcast")]
		public static extern IntPtr GetRoomMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_ClearRoomMetadataResult_SWIGUpcast")]
		public static extern IntPtr ClearRoomMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetMemberMetadataResult_SWIGUpcast")]
		public static extern IntPtr SetMemberMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GetMemberMetadataResult_SWIGUpcast")]
		public static extern IntPtr GetMemberMetadataResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RoomDataReceived_SWIGUpcast")]
		public static extern IntPtr RoomDataReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetRoomTypeResult_SWIGUpcast")]
		public static extern IntPtr SetRoomTypeResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_SetRoomMaxMemberResult_SWIGUpcast")]
		public static extern IntPtr SetRoomMaxMemberResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyMetadataChange_SWIGUpcast")]
		public static extern IntPtr NotifyMetadataChange_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberChange_SWIGUpcast")]
		public static extern IntPtr NotifyRoomMemberChange_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomMemberKicked_SWIGUpcast")]
		public static extern IntPtr NotifyRoomMemberKicked_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomDestroy_SWIGUpcast")]
		public static extern IntPtr NotifyRoomDestroy_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomOwnerChange_SWIGUpcast")]
		public static extern IntPtr NotifyRoomOwnerChange_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NotifyRoomGameServerChange_SWIGUpcast")]
		public static extern IntPtr NotifyRoomGameServerChange_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncQueryQuotaResult_SWIGUpcast")]
		public static extern IntPtr AsyncQueryQuotaResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_ShareStorageToSpaceWorkResult_SWIGUpcast")]
		public static extern IntPtr ShareStorageToSpaceWorkResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadFileResult_SWIGUpcast")]
		public static extern IntPtr AsyncReadFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteFileResult_SWIGUpcast")]
		public static extern IntPtr AsyncWriteFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncReadStreamFileResult_SWIGUpcast")]
		public static extern IntPtr AsyncReadStreamFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncWriteStreamFileResult_SWIGUpcast")]
		public static extern IntPtr AsyncWriteStreamFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncListFileResult_SWIGUpcast")]
		public static extern IntPtr AsyncListFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncRenameStreamFileResult_SWIGUpcast")]
		public static extern IntPtr AsyncRenameStreamFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_AsyncDeleteStreamFileResult_SWIGUpcast")]
		public static extern IntPtr AsyncDeleteStreamFileResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_CreateVoiceChannelResult_SWIGUpcast")]
		public static extern IntPtr CreateVoiceChannelResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_JoinVoiceChannelResult_SWIGUpcast")]
		public static extern IntPtr JoinVoiceChannelResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaveVoiceChannelResult_SWIGUpcast")]
		public static extern IntPtr LeaveVoiceChannelResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelAddUsersResult_SWIGUpcast")]
		public static extern IntPtr VoiceChannelAddUsersResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelRemoveUsersResult_SWIGUpcast")]
		public static extern IntPtr VoiceChannelRemoveUsersResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelInviteEvent_SWIGUpcast")]
		public static extern IntPtr VoiceChannelInviteEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelMemeberChangedEvent_SWIGUpcast")]
		public static extern IntPtr VoiceChannelMemeberChangedEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelPushToTalkKeyChangedEvent_SWIGUpcast")]
		public static extern IntPtr VoiceChannelPushToTalkKeyChangedEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelUsersSpeakingStateChangedEvent_SWIGUpcast")]
		public static extern IntPtr VoiceChannelUsersSpeakingStateChangedEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceChannelSpeakingUsersChangedEvent_SWIGUpcast")]
		public static extern IntPtr VoiceChannelSpeakingUsersChangedEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_VoiceDataCapturedEvent_SWIGUpcast")]
		public static extern IntPtr VoiceDataCapturedEvent_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailTextInputResult_SWIGUpcast")]
		public static extern IntPtr RailTextInputResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_QuerySubscribeWishPlayStateResult_SWIGUpcast")]
		public static extern IntPtr QuerySubscribeWishPlayStateResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailIMEHelperTextInputCompositionState_SWIGUpcast")]
		public static extern IntPtr RailIMEHelperTextInputCompositionState_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailIMEHelperTextInputSelectedResult_SWIGUpcast")]
		public static extern IntPtr RailIMEHelperTextInputSelectedResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailHttpSessionResponse_SWIGUpcast")]
		public static extern IntPtr RailHttpSessionResponse_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectStateQueryResult_SWIGUpcast")]
		public static extern IntPtr RailSmallObjectStateQueryResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSmallObjectDownloadResult_SWIGUpcast")]
		public static extern IntPtr RailSmallObjectDownloadResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailSwitchPlayerSelectedZoneResult_SWIGUpcast")]
		public static extern IntPtr RailSwitchPlayerSelectedZoneResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryGroupsInfoResult_SWIGUpcast")]
		public static extern IntPtr RailQueryGroupsInfoResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailOpenGroupChatResult_SWIGUpcast")]
		public static extern IntPtr RailOpenGroupChatResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailQueryGameOnlineTimeResult_SWIGUpcast")]
		public static extern IntPtr RailQueryGameOnlineTimeResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailCustomizeAntiAddictionActions_SWIGUpcast")]
		public static extern IntPtr RailCustomizeAntiAddictionActions_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailThirdPartyAccountLoginResult_SWIGUpcast")]
		public static extern IntPtr RailThirdPartyAccountLoginResult_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_RailNotifyThirdPartyAccountQrCodeInfo_SWIGUpcast")]
		public static extern IntPtr RailNotifyThirdPartyAccountQrCodeInfo_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerAchievement_SWIGUpcast")]
		public static extern IntPtr IRailPlayerAchievement_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGlobalAchievement_SWIGUpcast")]
		public static extern IntPtr IRailGlobalAchievement_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerAchievementReceived_SWIGUpcast")]
		public static extern IntPtr PlayerAchievementReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerAchievementStored_SWIGUpcast")]
		public static extern IntPtr PlayerAchievementStored_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GlobalAchievementReceived_SWIGUpcast")]
		public static extern IntPtr GlobalAchievementReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailAssets_SWIGUpcast")]
		public static extern IntPtr IRailAssets_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowser_SWIGUpcast")]
		public static extern IntPtr IRailBrowser_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailBrowserRender_SWIGUpcast")]
		public static extern IntPtr IRailBrowserRender_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGameServer_SWIGUpcast")]
		public static extern IntPtr IRailGameServer_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboard_SWIGUpcast")]
		public static extern IntPtr IRailLeaderboard_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailLeaderboardEntries_SWIGUpcast")]
		public static extern IntPtr IRailLeaderboardEntries_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardReceived_SWIGUpcast")]
		public static extern IntPtr LeaderboardReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardCreated_SWIGUpcast")]
		public static extern IntPtr LeaderboardCreated_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardEntryReceived_SWIGUpcast")]
		public static extern IntPtr LeaderboardEntryReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardUploaded_SWIGUpcast")]
		public static extern IntPtr LeaderboardUploaded_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_LeaderboardAttachSpaceWork_SWIGUpcast")]
		public static extern IntPtr LeaderboardAttachSpaceWork_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailRoom_SWIGUpcast")]
		public static extern IntPtr IRailRoom_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailScreenshot_SWIGUpcast")]
		public static extern IntPtr IRailScreenshot_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailPlayerStats_SWIGUpcast")]
		public static extern IntPtr IRailPlayerStats_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGlobalStats_SWIGUpcast")]
		public static extern IntPtr IRailGlobalStats_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerStatsReceived_SWIGUpcast")]
		public static extern IntPtr PlayerStatsReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_PlayerStatsStored_SWIGUpcast")]
		public static extern IntPtr PlayerStatsStored_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_NumberOfPlayerReceived_SWIGUpcast")]
		public static extern IntPtr NumberOfPlayerReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_GlobalStatsRequestReceived_SWIGUpcast")]
		public static extern IntPtr GlobalStatsRequestReceived_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailFile_SWIGUpcast")]
		public static extern IntPtr IRailFile_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailStreamFile_SWIGUpcast")]
		public static extern IntPtr IRailStreamFile_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailSpaceWork_SWIGUpcast")]
		public static extern IntPtr IRailSpaceWork_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailVoiceChannel_SWIGUpcast")]
		public static extern IntPtr IRailVoiceChannel_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpSession_SWIGUpcast")]
		public static extern IntPtr IRailHttpSession_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailHttpResponse_SWIGUpcast")]
		public static extern IntPtr IRailHttpResponse_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailZoneServer_SWIGUpcast")]
		public static extern IntPtr IRailZoneServer_SWIGUpcast(IntPtr jarg1);

		[DllImport("rail_api64", EntryPoint = "CSharp_IRailGroupChat_SWIGUpcast")]
		public static extern IntPtr IRailGroupChat_SWIGUpcast(IntPtr jarg1);
	}
}
