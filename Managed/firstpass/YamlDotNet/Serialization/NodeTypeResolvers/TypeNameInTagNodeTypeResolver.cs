using System;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.NodeTypeResolvers
{
	public sealed class TypeNameInTagNodeTypeResolver : INodeTypeResolver
	{
		bool INodeTypeResolver.Resolve(NodeEvent nodeEvent, ref Type currentType)
		{
			if (!string.IsNullOrEmpty(nodeEvent.Tag))
			{
				currentType = Type.GetType(nodeEvent.Tag.Substring(1), throwOnError: false);
				return currentType != null;
			}
			return false;
		}
	}
}
