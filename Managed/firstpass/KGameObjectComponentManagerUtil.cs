using System.IO;
using KSerialization;
using UnityEngine;

public static class KGameObjectComponentManagerUtil
{
	public static void Serialize<MgrType, DataType>(MgrType mgr, GameObject go, BinaryWriter writer) where MgrType : KGameObjectComponentManager<DataType> where DataType : new()
	{
		long position = writer.BaseStream.Position;
		writer.Write(0);
		long position2 = writer.BaseStream.Position;
		HandleVector<int>.Handle handle = mgr.GetHandle(go);
		Serializer.SerializeTypeless(mgr.GetData(handle), writer);
		long position3 = writer.BaseStream.Position;
		long num = position3 - position2;
		writer.BaseStream.Position = position;
		writer.Write((int)num);
		writer.BaseStream.Position = position3;
	}

	public static void Deserialize<MgrType, DataType>(MgrType mgr, GameObject go, IReader reader) where MgrType : KGameObjectComponentManager<DataType> where DataType : new()
	{
		HandleVector<int>.Handle handle = mgr.GetHandle(go);
		Deserializer.Deserialize(typeof(DataType), reader, out var result);
		mgr.SetData(handle, (DataType)result);
	}

	public static void Serialize<MgrType, Header, Payload>(MgrType mgr, GameObject go, BinaryWriter writer) where MgrType : KGameObjectSplitComponentManager<Header, Payload> where Header : new()where Payload : new()
	{
		long position = writer.BaseStream.Position;
		writer.Write(0);
		long position2 = writer.BaseStream.Position;
		HandleVector<int>.Handle handle = mgr.GetHandle(go);
		mgr.GetData(handle, out var header, out var payload);
		Serializer.SerializeTypeless(header, writer);
		Serializer.SerializeTypeless(payload, writer);
		long position3 = writer.BaseStream.Position;
		long num = position3 - position2;
		writer.BaseStream.Position = position;
		writer.Write((int)num);
		writer.BaseStream.Position = position3;
	}

	public static void Deserialize<MgrType, Header, Payload>(MgrType mgr, GameObject go, IReader reader) where MgrType : KGameObjectSplitComponentManager<Header, Payload> where Header : new()where Payload : new()
	{
		HandleVector<int>.Handle handle = mgr.GetHandle(go);
		Deserializer.Deserialize(typeof(Header), reader, out var result);
		Deserializer.Deserialize(typeof(Payload), reader, out var result2);
		Payload new_data = (Payload)result2;
		mgr.SetData(handle, (Header)result, ref new_data);
	}
}
