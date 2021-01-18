using System.IO;
using YamlDotNet.Samples.Helpers;
using YamlDotNet.Serialization;

namespace YamlDotNet.Samples
{
	public class ConvertYamlToJson
	{
		private readonly ITestOutputHelper output;

		public ConvertYamlToJson(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Sample(Title = "Convert YAML to JSON", Description = "Shows how to convert a YAML document to JSON.")]
		public void Main()
		{
			StringReader input = new StringReader("\nscalar: a scalar\nsequence:\n  - one\n  - two\n");
			Deserializer deserializer = new DeserializerBuilder().Build();
			object graph = deserializer.Deserialize(input);
			Serializer serializer = new SerializerBuilder().JsonCompatible().Build();
			string value = serializer.Serialize(graph);
			output.WriteLine(value);
		}
	}
}
