using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public static class Debug
{
	private static bool s_loggingDisabled;

	public static bool isDebugBuild => UnityEngine.Debug.isDebugBuild;

	public static bool developerConsoleVisible
	{
		get
		{
			return UnityEngine.Debug.developerConsoleVisible;
		}
		set
		{
			UnityEngine.Debug.developerConsoleVisible = value;
		}
	}

	private static string TimeStamp()
	{
		return DateTime.UtcNow.ToString("[HH:mm:ss.fff] [") + Thread.CurrentThread.ManagedThreadId + "] ";
	}

	private static void WriteTimeStamped(params object[] objs)
	{
		Console.WriteLine(TimeStamp() + DebugUtil.BuildString(objs));
	}

	public static void Break()
	{
	}

	public static void LogException(Exception exception)
	{
		if (!s_loggingDisabled)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}

	public static void Log(object obj)
	{
		if (!s_loggingDisabled)
		{
			WriteTimeStamped("[INFO]", obj);
		}
	}

	public static void Log(object obj, UnityEngine.Object context)
	{
		if (!s_loggingDisabled)
		{
			WriteTimeStamped("[INFO]", (context != null) ? context.name : "null", obj);
		}
	}

	public static void LogFormat(string format, params object[] args)
	{
		if (!s_loggingDisabled)
		{
			WriteTimeStamped("[INFO]", string.Format(format, args));
		}
	}

	public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
	{
		if (!s_loggingDisabled)
		{
			WriteTimeStamped("[INFO]", (context != null) ? context.name : "null", string.Format(format, args));
		}
	}

	public static void LogWarning(object obj)
	{
		if (!s_loggingDisabled)
		{
			WriteTimeStamped("[WARNING]", obj);
		}
	}

	public static void LogWarning(object obj, UnityEngine.Object context)
	{
		if (!s_loggingDisabled)
		{
			WriteTimeStamped("[WARNING]", (context != null) ? context.name : "null", obj);
		}
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		if (!s_loggingDisabled)
		{
			WriteTimeStamped("[WARNING]", string.Format(format, args));
		}
	}

	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
	{
		if (!s_loggingDisabled)
		{
			WriteTimeStamped("[WARNING]", (context != null) ? context.name : "null", string.Format(format, args));
		}
	}

	public static void LogError(object obj)
	{
		if (!s_loggingDisabled)
		{
			WriteTimeStamped("[ERROR]", obj);
			UnityEngine.Debug.LogError(obj);
		}
	}

	public static void LogError(object obj, UnityEngine.Object context)
	{
		if (!s_loggingDisabled)
		{
			WriteTimeStamped("[ERROR]", (context != null) ? context.name : "null", obj);
			UnityEngine.Debug.LogError(obj, context);
		}
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		if (!s_loggingDisabled)
		{
			WriteTimeStamped("[ERROR]", string.Format(format, args));
			UnityEngine.Debug.LogErrorFormat(format, args);
		}
	}

	public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
	{
		if (!s_loggingDisabled)
		{
			WriteTimeStamped("[ERROR]", (context != null) ? context.name : "null", string.Format(format, args));
			UnityEngine.Debug.LogErrorFormat(context, format, args);
		}
	}

	public static void Assert(bool condition)
	{
		if (!condition)
		{
			LogError("Assert failed");
			Break();
		}
	}

	public static void Assert(bool condition, object message)
	{
		if (!condition)
		{
			LogError("Assert failed: " + message);
			Break();
		}
	}

	public static void Assert(bool condition, object message, UnityEngine.Object context)
	{
		if (!condition)
		{
			LogError("Assert failed: " + message, context);
			Break();
		}
	}

	public static void DisableLogging()
	{
		s_loggingDisabled = true;
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
	}
}
