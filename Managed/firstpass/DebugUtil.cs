using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public static class DebugUtil
{
	private static StringBuilder s_errorMessageBuilder = new StringBuilder();

	private static Exception s_lastExceptionLogged;

	private static StringBuilder fullNameBuilder = new StringBuilder();

	public static void Assert(bool test)
	{
		Debug.Assert(test);
	}

	public static void Assert(bool test, string message)
	{
		Debug.Assert(test, message);
	}

	public static void Assert(bool test, string message0, string message1)
	{
		if (!test)
		{
			s_errorMessageBuilder.Length = 0;
			Debug.Assert(test, s_errorMessageBuilder.Append(message0).Append(" ").Append(message1)
				.ToString());
		}
	}

	public static void Assert(bool test, string message0, string message1, string message2)
	{
		if (!test)
		{
			s_errorMessageBuilder.Length = 0;
			Debug.Assert(test, s_errorMessageBuilder.Append(message0).Append(" ").Append(message1)
				.Append(" ")
				.Append(message2)
				.ToString());
		}
	}

	public static string BuildString(object[] objs)
	{
		string text = "";
		if (objs.Length != 0)
		{
			text = ((objs[0] != null) ? objs[0].ToString() : "null");
			for (int i = 1; i < objs.Length; i++)
			{
				object obj = objs[i];
				text = text + " " + ((obj != null) ? obj.ToString() : "null");
			}
		}
		return text;
	}

	public static void DevAssert(bool test, string msg, UnityEngine.Object context = null)
	{
		if (!test)
		{
			Debug.LogWarning(msg, context);
		}
	}

	public static void DevAssertArgs(bool test, params object[] objs)
	{
		if (!test)
		{
			Debug.LogWarning(BuildString(objs));
		}
	}

	public static void DevAssertArgsWithStack(bool test, params object[] objs)
	{
		if (!test)
		{
			StackTrace arg = new StackTrace(1, fNeedFileInfo: true);
			string obj = $"{BuildString(objs)}\n{arg}";
			Debug.LogWarning(obj);
		}
	}

	public static void DevLogError(UnityEngine.Object context, string msg)
	{
		Debug.LogWarningFormat(context, msg);
	}

	public static void DevLogError(string msg)
	{
		Debug.LogWarningFormat(msg);
	}

	public static void DevLogErrorFormat(UnityEngine.Object context, string format, params object[] args)
	{
		Debug.LogWarningFormat(context, format, args);
	}

	public static void DevLogErrorFormat(string format, params object[] args)
	{
		Debug.LogWarningFormat(format, args);
	}

	public static void LogArgs(params object[] objs)
	{
		string obj = BuildString(objs);
		Debug.Log(obj);
	}

	public static void LogArgs(UnityEngine.Object context, params object[] objs)
	{
		string obj = BuildString(objs);
		Debug.Log(obj, context);
	}

	public static void LogWarningArgs(params object[] objs)
	{
		string obj = BuildString(objs);
		Debug.LogWarning(obj);
	}

	public static void LogWarningArgs(UnityEngine.Object context, params object[] objs)
	{
		string obj = BuildString(objs);
		Debug.LogWarning(obj, context);
	}

	public static void LogErrorArgs(params object[] objs)
	{
		string obj = BuildString(objs);
		Debug.LogError(obj);
	}

	public static void LogErrorArgs(UnityEngine.Object context, params object[] objs)
	{
		string obj = BuildString(objs);
		Debug.LogError(obj, context);
	}

	public static void LogException(UnityEngine.Object context, string errorMessage, Exception e)
	{
		s_lastExceptionLogged = e;
		LogErrorArgs(context, errorMessage, "\n" + e.ToString());
	}

	public static Exception RetrieveLastExceptionLogged()
	{
		Exception result = s_lastExceptionLogged;
		s_lastExceptionLogged = null;
		return result;
	}

	private static void RecursiveBuildFullName(GameObject obj)
	{
		if (!(obj == null))
		{
			RecursiveBuildFullName(obj.transform.parent.gameObject);
			fullNameBuilder.Append("/").Append(obj.name);
		}
	}

	private static StringBuilder BuildFullName(GameObject obj)
	{
		fullNameBuilder.Length = 0;
		RecursiveBuildFullName(obj);
		return fullNameBuilder.Append(" (").Append(obj.GetInstanceID()).Append(")");
	}

	public static string FullName(GameObject obj)
	{
		return BuildFullName(obj).ToString();
	}

	public static string FullName(Component cmp)
	{
		return BuildFullName(cmp.gameObject).Append(" (").Append(cmp.GetType()).Append(" ")
			.Append(cmp.GetInstanceID().ToString())
			.Append(")")
			.ToString();
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogIfSelected(GameObject obj, params object[] objs)
	{
	}

	[Conditional("ENABLE_DETAILED_PROFILING")]
	public static void ProfileBegin(string str)
	{
	}

	[Conditional("ENABLE_DETAILED_PROFILING")]
	public static void ProfileBegin(string str, UnityEngine.Object target)
	{
	}

	[Conditional("ENABLE_DETAILED_PROFILING")]
	public static void ProfileEnd()
	{
	}
}
