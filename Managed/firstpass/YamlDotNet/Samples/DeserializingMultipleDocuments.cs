using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Samples.Helpers;
using YamlDotNet.Serialization;

namespace YamlDotNet.Samples
{
	public class DeserializingMultipleDocuments
	{
		private readonly ITestOutputHelper output;

		private const string Document = "---\n- Prisoner\n- Goblet\n- Phoenix\n---\n- Memoirs\n- Snow \n- Ghost\t\t\n...";

		public DeserializingMultipleDocuments(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Sample(Title = "Deserializing multiple documents", Description = "Explains how to load multiple YAML documents from a stream.")]
		public void Main()
		{
			StringReader input = new StringReader("---\n- Prisoner\n- Goblet\n- Phoenix\n---\n- Memoirs\n- Snow \n- Ghost\t\t\n...");
			Deserializer deserializer = new DeserializerBuilder().Build();
			Parser parser = new Parser(input);
			parser.Expect<StreamStart>();
			while (parser.Accept<DocumentStart>())
			{
				List<string> list = deserializer.Deserialize<List<string>>(parser);
				output.WriteLine("## Document");
				foreach (string item in list)
				{
					output.WriteLine(item);
				}
			}
		}
	}
}
