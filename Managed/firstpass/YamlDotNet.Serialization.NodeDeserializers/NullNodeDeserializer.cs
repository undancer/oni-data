using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class NullNodeDeserializer : INodeDeserializer
	{
		bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			value = null;
			NodeEvent nodeEvent = parser.Peek<NodeEvent>();
			int num;
			if (nodeEvent != null)
			{
				num = (NodeIsNull(nodeEvent) ? 1 : 0);
				if (num != 0)
				{
					parser.SkipThisAndNestedEvents();
				}
			}
			else
			{
				num = 0;
			}
			return (byte)num != 0;
		}

		private bool NodeIsNull(NodeEvent nodeEvent)
		{
			if (nodeEvent.Tag == "tag:yaml.org,2002:null")
			{
				return true;
			}
			if (!(nodeEvent is Scalar scalar) || scalar.Style != ScalarStyle.Plain)
			{
				return false;
			}
			string value = scalar.Value;
			switch (value)
			{
			default:
				return value == "NULL";
			case "":
			case "~":
			case "null":
			case "Null":
				return true;
			}
		}
	}
}
