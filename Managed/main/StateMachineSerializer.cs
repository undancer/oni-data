using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;

public class StateMachineSerializer
{
	private class Entry
	{
		public int version;

		public Type type;

		public string typeSuffix;

		public string currentState;

		public FastReader entryData;

		public static bool TrySerialize(StateMachine.Instance smi, BinaryWriter writer)
		{
			if (!smi.IsRunning())
			{
				return false;
			}
			int num = (int)writer.BaseStream.Position;
			writer.Write(0);
			writer.WriteKleiString(smi.GetType().FullName);
			writer.WriteKleiString(smi.serializationSuffix);
			writer.WriteKleiString(smi.GetCurrentState().name);
			int num2 = (int)writer.BaseStream.Position;
			writer.Write(0);
			int num3 = (int)writer.BaseStream.Position;
			Serializer.SerializeTypeless(smi, writer);
			if (smi.GetStateMachine().serializable == StateMachine.SerializeType.ParamsOnly || smi.GetStateMachine().serializable == StateMachine.SerializeType.Both_DEPRECATED)
			{
				StateMachine.Parameter.Context[] parameterContexts = smi.GetParameterContexts();
				writer.Write(parameterContexts.Length);
				StateMachine.Parameter.Context[] array = parameterContexts;
				foreach (StateMachine.Parameter.Context context in array)
				{
					long position = (int)writer.BaseStream.Position;
					writer.Write(0);
					long num4 = (int)writer.BaseStream.Position;
					writer.WriteKleiString(context.GetType().FullName);
					writer.WriteKleiString(context.parameter.name);
					context.Serialize(writer);
					long num5 = (int)writer.BaseStream.Position;
					writer.BaseStream.Position = position;
					long num6 = num5 - num4;
					writer.Write((int)num6);
					writer.BaseStream.Position = num5;
				}
			}
			int num7 = (int)writer.BaseStream.Position - num3;
			if (num7 > 0)
			{
				int num8 = (int)writer.BaseStream.Position;
				writer.BaseStream.Position = num2;
				writer.Write(num7);
				writer.BaseStream.Position = num8;
				return true;
			}
			writer.BaseStream.Position = num;
			writer.BaseStream.SetLength(num);
			return false;
		}

		public static Entry Deserialize(IReader reader, int serializerVersion)
		{
			Entry entry = new Entry();
			reader.ReadInt32();
			entry.version = serializerVersion;
			string typeName = reader.ReadKleiString();
			entry.type = Type.GetType(typeName);
			entry.typeSuffix = (DoesVersionHaveTypeSuffix(serializerVersion) ? reader.ReadKleiString() : null);
			entry.currentState = reader.ReadKleiString();
			int length = reader.ReadInt32();
			entry.entryData = new FastReader(reader.ReadBytes(length));
			if (entry.type == null)
			{
				return null;
			}
			return entry;
		}

		public bool Restore(StateMachine.Instance smi)
		{
			if (Manager.HasDeserializationMapping(smi.GetType()))
			{
				Deserializer.DeserializeTypeless(smi, entryData);
			}
			StateMachine.SerializeType serializable = smi.GetStateMachine().serializable;
			int num;
			switch (serializable)
			{
			case StateMachine.SerializeType.Never:
				return false;
			case StateMachine.SerializeType.ParamsOnly:
			case StateMachine.SerializeType.Both_DEPRECATED:
				num = ((!entryData.IsFinished) ? 1 : 0);
				break;
			default:
				num = 0;
				break;
			}
			if (num != 0)
			{
				StateMachine.Parameter.Context[] parameterContexts = smi.GetParameterContexts();
				int num2 = entryData.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					int num3 = entryData.ReadInt32();
					int position = entryData.Position;
					string text = entryData.ReadKleiString();
					text = text.Replace("Version=2.0.0.0", "Version=4.0.0.0");
					string b = entryData.ReadKleiString();
					StateMachine.Parameter.Context[] array = parameterContexts;
					foreach (StateMachine.Parameter.Context context in array)
					{
						if (context.parameter.name == b && (version > 10 || !(context.parameter.GetType().Name == "TargetParameter")) && context.GetType().FullName == text)
						{
							context.Deserialize(entryData, smi);
							break;
						}
					}
					entryData.SkipBytes(num3 - (entryData.Position - position));
				}
			}
			if (serializable == StateMachine.SerializeType.Both_DEPRECATED || serializable == StateMachine.SerializeType.CurrentStateOnly_DEPRECATED)
			{
				StateMachine.BaseState state = smi.GetStateMachine().GetState(currentState);
				if (state != null)
				{
					smi.GoTo(state);
					return true;
				}
			}
			return false;
		}
	}

	private class OldEntryV11
	{
		public int version;

		public int dataPos;

		public Type type;

		public string typeSuffix;

		public string currentState;

		public OldEntryV11(int version, int dataPos, Type type, string typeSuffix, string currentState)
		{
			this.version = version;
			this.dataPos = dataPos;
			this.type = type;
			this.typeSuffix = typeSuffix;
			this.currentState = currentState;
		}

		public static List<Entry> DeserializeOldEntries(IReader reader, int serializerVersion)
		{
			Debug.Assert(serializerVersion < 12);
			List<OldEntryV11> list = ReadEntries(reader, serializerVersion);
			byte[] bytes = ReadEntryData(reader);
			List<Entry> list2 = new List<Entry>(list.Count);
			foreach (OldEntryV11 item in list)
			{
				Entry entry = new Entry();
				entry.version = serializerVersion;
				entry.type = item.type;
				entry.typeSuffix = item.typeSuffix;
				entry.currentState = item.currentState;
				entry.entryData = new FastReader(bytes);
				entry.entryData.SkipBytes(item.dataPos);
				list2.Add(entry);
			}
			return list2;
		}

		private static OldEntryV11 Deserialize(IReader reader, int serializerVersion)
		{
			int num = reader.ReadInt32();
			int num2 = reader.ReadInt32();
			string typeName = reader.ReadKleiString();
			string text = (DoesVersionHaveTypeSuffix(serializerVersion) ? reader.ReadKleiString() : null);
			string text2 = reader.ReadKleiString();
			Type left = Type.GetType(typeName);
			if (left == null)
			{
				return null;
			}
			return new OldEntryV11(num, num2, left, text, text2);
		}

		private static List<OldEntryV11> ReadEntries(IReader reader, int serializerVersion)
		{
			List<OldEntryV11> list = new List<OldEntryV11>();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				OldEntryV11 oldEntryV = Deserialize(reader, serializerVersion);
				if (oldEntryV != null)
				{
					list.Add(oldEntryV);
				}
			}
			return list;
		}

		private static byte[] ReadEntryData(IReader reader)
		{
			int length = reader.ReadInt32();
			return reader.ReadBytes(length);
		}
	}

	public const int SERIALIZER_PRE_DLC1 = 10;

	public const int SERIALIZER_TYPE_SUFFIX = 11;

	public const int SERIALIZER_OPTIMIZE_BUFFERS = 12;

	public const int SERIALIZER_EXPANSION1 = 20;

	private static int SERIALIZER_VERSION = 20;

	private const string TargetParameterName = "TargetParameter";

	private List<Entry> entries = new List<Entry>();

	public void Serialize(List<StateMachine.Instance> state_machines, BinaryWriter writer)
	{
		writer.Write(SERIALIZER_VERSION);
		long position = writer.BaseStream.Position;
		writer.Write(0);
		long position2 = writer.BaseStream.Position;
		try
		{
			int num = (int)writer.BaseStream.Position;
			int num2 = 0;
			writer.Write(num2);
			foreach (StateMachine.Instance state_machine in state_machines)
			{
				if (Entry.TrySerialize(state_machine, writer))
				{
					num2++;
				}
			}
			int num3 = (int)writer.BaseStream.Position;
			writer.BaseStream.Position = num;
			writer.Write(num2);
			writer.BaseStream.Position = num3;
		}
		catch (Exception obj)
		{
			Debug.Log("StateMachines: ");
			foreach (StateMachine.Instance state_machine2 in state_machines)
			{
				Debug.Log(state_machine2.ToString());
			}
			Debug.LogError(obj);
		}
		long position3 = writer.BaseStream.Position;
		long num4 = position3 - position2;
		writer.BaseStream.Position = position;
		writer.Write((int)num4);
		writer.BaseStream.Position = position3;
	}

	public void Deserialize(IReader reader)
	{
		int num = reader.ReadInt32();
		int length = reader.ReadInt32();
		if (num < 10)
		{
			Debug.LogWarning("State machine serializer version mismatch: " + num + "!=" + SERIALIZER_VERSION + "\nDiscarding data.");
			reader.SkipBytes(length);
			return;
		}
		if (num < 12)
		{
			entries = OldEntryV11.DeserializeOldEntries(reader, num);
			return;
		}
		int num2 = reader.ReadInt32();
		entries = new List<Entry>(num2);
		for (int i = 0; i < num2; i++)
		{
			Entry entry = Entry.Deserialize(reader, num);
			if (entry != null)
			{
				entries.Add(entry);
			}
		}
	}

	private static string TrimAssemblyInfo(string type_name)
	{
		int num = type_name.IndexOf("[[");
		if (num != -1)
		{
			return type_name.Substring(0, num);
		}
		return type_name;
	}

	public bool Restore(StateMachine.Instance instance)
	{
		Type type = instance.GetType();
		for (int i = 0; i < entries.Count; i++)
		{
			Entry entry = entries[i];
			if (entry.type == type && instance.serializationSuffix == entry.typeSuffix)
			{
				entries.RemoveAt(i);
				return entry.Restore(instance);
			}
		}
		return false;
	}

	private static bool DoesVersionHaveTypeSuffix(int version)
	{
		return version >= 20 || version == 11;
	}
}
