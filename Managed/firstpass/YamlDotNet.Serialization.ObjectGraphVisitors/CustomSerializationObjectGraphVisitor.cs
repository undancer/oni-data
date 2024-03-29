using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class CustomSerializationObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		private readonly IEnumerable<IYamlTypeConverter> typeConverters;

		private readonly ObjectSerializer nestedObjectSerializer;

		public CustomSerializationObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor, IEnumerable<IYamlTypeConverter> typeConverters, ObjectSerializer nestedObjectSerializer)
			: base(nextVisitor)
		{
			IEnumerable<IYamlTypeConverter> enumerable;
			if (typeConverters == null)
			{
				enumerable = Enumerable.Empty<IYamlTypeConverter>();
			}
			else
			{
				IEnumerable<IYamlTypeConverter> enumerable2 = typeConverters.ToList();
				enumerable = enumerable2;
			}
			this.typeConverters = enumerable;
			this.nestedObjectSerializer = nestedObjectSerializer;
		}

		public override bool Enter(IObjectDescriptor value, IEmitter context)
		{
			IYamlTypeConverter yamlTypeConverter = typeConverters.FirstOrDefault((IYamlTypeConverter t) => t.Accepts(value.Type));
			if (yamlTypeConverter != null)
			{
				yamlTypeConverter.WriteYaml(context, value.Value, value.Type);
				return false;
			}
			if (value.Value is IYamlConvertible yamlConvertible)
			{
				yamlConvertible.Write(context, nestedObjectSerializer);
				return false;
			}
			if (value.Value is IYamlSerializable yamlSerializable)
			{
				yamlSerializable.WriteYaml(context);
				return false;
			}
			return base.Enter(value, context);
		}
	}
}
