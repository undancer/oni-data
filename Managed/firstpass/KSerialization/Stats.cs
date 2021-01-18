using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KSerialization
{
	public static class Stats
	{
		private struct StatInfo
		{
			public int numOccurrences;

			public long numBytes;
		}

		private static Dictionary<Type, StatInfo> serializationStats = new Dictionary<Type, StatInfo>();

		private static Dictionary<Type, StatInfo> deserializationStats = new Dictionary<Type, StatInfo>();

		[Conditional("ENABLE_KSERIALIZER_STATS")]
		public static void Clear()
		{
			serializationStats.Clear();
			deserializationStats.Clear();
		}

		[Conditional("ENABLE_KSERIALIZER_STATS")]
		public static void Write(Type type, long num_bytes)
		{
			serializationStats.TryGetValue(type, out var value);
			value.numOccurrences++;
			value.numBytes += num_bytes;
			serializationStats[type] = value;
		}

		[Conditional("ENABLE_KSERIALIZER_STATS")]
		public static void Read(Type type, long num_bytes)
		{
			deserializationStats.TryGetValue(type, out var value);
			value.numOccurrences++;
			value.numBytes += num_bytes;
			deserializationStats[type] = value;
		}

		public static void Print()
		{
			if (serializationStats.Count > 0)
			{
			}
			if (deserializationStats.Count <= 0)
			{
			}
		}

		[Conditional("ENABLE_KSERIALIZER_STATS")]
		private static void Print(string header, Dictionary<Type, StatInfo> stats)
		{
			string text = header + "\n";
			foreach (KeyValuePair<Type, StatInfo> stat in stats)
			{
				text = text + stat.Key.ToString() + "," + stat.Value.numOccurrences + "," + stat.Value.numBytes + "\n";
			}
			DebugUtil.LogArgs(text);
		}
	}
}
