using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Logging
{
	public static class LoggingInterface
	{
		private static LogMessageFuncInternal s_LogMessageFuncInternal;

		private static LogMessageFunc s_LogMessageFunc;

		public static Result SetCallback(LogMessageFunc callback)
		{
			LogMessageFuncInternal callback2 = LogMessageFunc;
			s_LogMessageFunc = callback;
			s_LogMessageFuncInternal = callback2;
			Result source = EOS_Logging_SetCallback(callback2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public static Result SetLogLevel(LogCategory logCategory, LogLevel logLevel)
		{
			Result source = EOS_Logging_SetLogLevel(logCategory, logLevel);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[MonoPInvokeCallback]
		internal static void LogMessageFunc(IntPtr address)
		{
			LogMessage target = null;
			if (Helper.TryMarshalGet<LogMessageInternal, LogMessage>(address, out target))
			{
				s_LogMessageFunc(target);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Logging_SetLogLevel(LogCategory logCategory, LogLevel logLevel);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Logging_SetCallback(LogMessageFuncInternal callback);
	}
}
