using System.Diagnostics;

namespace KSerialization
{
	internal static class DebugLog
	{
		public enum Level
		{
			Error,
			Warning,
			Info
		}

		private const Level OutputLevel = Level.Error;

		[Conditional("DEBUG_LOG")]
		public static void Output(Level msg_level, string msg)
		{
			if (msg_level <= Level.Error)
			{
				switch (msg_level)
				{
				case Level.Info:
					Debug.Log(msg);
					break;
				case Level.Warning:
					Debug.LogWarning(msg);
					break;
				case Level.Error:
					Debug.LogError(msg);
					break;
				}
			}
		}
	}
}
