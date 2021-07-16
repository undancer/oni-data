using System.IO;

namespace KSerialization
{
	public class Serializer
	{
		private BinaryWriter writer;

		public Serializer(BinaryWriter writer)
		{
			this.writer = writer;
		}

		public void Serialize(object obj)
		{
			Serialize(obj, writer);
		}

		public static void Serialize(object obj, BinaryWriter writer)
		{
			SerializationTemplate serializationTemplate = Manager.GetSerializationTemplate(obj.GetType());
			string kTypeString = obj.GetType().GetKTypeString();
			writer.WriteKleiString(kTypeString);
			serializationTemplate.SerializeData(obj, writer);
		}

		public static void SerializeTypeless(object obj, BinaryWriter writer)
		{
			Manager.GetSerializationTemplate(obj.GetType()).SerializeData(obj, writer);
		}
	}
}
