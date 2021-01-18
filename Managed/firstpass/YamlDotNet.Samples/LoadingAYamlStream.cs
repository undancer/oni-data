using System.Collections.Generic;
using System.IO;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Samples.Helpers;

namespace YamlDotNet.Samples
{
	public class LoadingAYamlStream
	{
		private readonly ITestOutputHelper output;

		private const string Document = "---\n            receipt:    Oz-Ware Purchase Invoice\n            date:        2007-08-06\n            customer:\n                given:   Dorothy\n                family:  Gale\n\n            items:\n                - part_no:   A4786\n                  descrip:   Water Bucket (Filled)\n                  price:     1.47\n                  quantity:  4\n\n                - part_no:   E1628\n                  descrip:   High Heeled \"Ruby\" Slippers\n                  price:     100.27\n                  quantity:  1\n\n            bill-to:  &id001\n                street: |\n                        123 Tornado Alley\n                        Suite 16\n                city:   East Westville\n                state:  KS\n\n            ship-to:  *id001\n\n            specialDelivery:  >\n                Follow the Yellow Brick\n                Road to the Emerald City.\n                Pay no attention to the\n                man behind the curtain.\n...";

		public LoadingAYamlStream(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Sample(Title = "Loading a YAML Stream", Description = "Explains how to load YAML using the representation model.")]
		public void Main()
		{
			StringReader input = new StringReader("---\n            receipt:    Oz-Ware Purchase Invoice\n            date:        2007-08-06\n            customer:\n                given:   Dorothy\n                family:  Gale\n\n            items:\n                - part_no:   A4786\n                  descrip:   Water Bucket (Filled)\n                  price:     1.47\n                  quantity:  4\n\n                - part_no:   E1628\n                  descrip:   High Heeled \"Ruby\" Slippers\n                  price:     100.27\n                  quantity:  1\n\n            bill-to:  &id001\n                street: |\n                        123 Tornado Alley\n                        Suite 16\n                city:   East Westville\n                state:  KS\n\n            ship-to:  *id001\n\n            specialDelivery:  >\n                Follow the Yellow Brick\n                Road to the Emerald City.\n                Pay no attention to the\n                man behind the curtain.\n...");
			YamlStream yamlStream = new YamlStream();
			yamlStream.Load(input);
			YamlMappingNode yamlMappingNode = (YamlMappingNode)yamlStream.Documents[0].RootNode;
			foreach (KeyValuePair<YamlNode, YamlNode> child in yamlMappingNode.Children)
			{
				output.WriteLine(((YamlScalarNode)child.Key).Value);
			}
			YamlSequenceNode yamlSequenceNode = (YamlSequenceNode)yamlMappingNode.Children[new YamlScalarNode("items")];
			foreach (YamlMappingNode item in yamlSequenceNode)
			{
				output.WriteLine("{0}\t{1}", item.Children[new YamlScalarNode("part_no")], item.Children[new YamlScalarNode("descrip")]);
			}
		}
	}
}
