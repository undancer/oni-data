using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.EventEmitters;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.ObjectGraphTraversalStrategies;
using YamlDotNet.Serialization.ObjectGraphVisitors;
using YamlDotNet.Serialization.TypeInspectors;
using YamlDotNet.Serialization.TypeResolvers;

namespace YamlDotNet.Serialization
{
	public sealed class Serializer
	{
		private class BackwardsCompatibleConfiguration : IValueSerializer
		{
			private readonly SerializationOptions options;

			private readonly INamingConvention namingConvention;

			private readonly ITypeResolver typeResolver;

			private readonly YamlAttributeOverrides overrides;

			public IList<IYamlTypeConverter> Converters
			{
				get;
				private set;
			}

			public BackwardsCompatibleConfiguration(SerializationOptions options, INamingConvention namingConvention, YamlAttributeOverrides overrides)
			{
				this.options = options;
				this.namingConvention = namingConvention ?? new NullNamingConvention();
				this.overrides = overrides;
				Converters = new List<IYamlTypeConverter>();
				Converters.Add(new GuidConverter(IsOptionSet(SerializationOptions.JsonCompatible)));
				ITypeResolver obj;
				if (!IsOptionSet(SerializationOptions.DefaultToStaticType))
				{
					ITypeResolver typeResolver = new DynamicTypeResolver();
					obj = typeResolver;
				}
				else
				{
					ITypeResolver typeResolver = new StaticTypeResolver();
					obj = typeResolver;
				}
				this.typeResolver = obj;
			}

			public bool IsOptionSet(SerializationOptions option)
			{
				return (options & option) != 0;
			}

			private IObjectGraphVisitor<IEmitter> CreateEmittingVisitor(IEmitter emitter, IObjectGraphTraversalStrategy traversalStrategy, IEventEmitter eventEmitter, IObjectDescriptor graph)
			{
				IObjectGraphVisitor<IEmitter> nextVisitor = new EmittingObjectGraphVisitor(eventEmitter);
				ObjectSerializer nestedObjectSerializer = delegate(object v, Type t)
				{
					SerializeValue(emitter, v, t);
				};
				nextVisitor = new CustomSerializationObjectGraphVisitor(nextVisitor, Converters, nestedObjectSerializer);
				if (!IsOptionSet(SerializationOptions.DisableAliases))
				{
					AnchorAssigner anchorAssigner = new AnchorAssigner(Converters);
					traversalStrategy.Traverse(graph, anchorAssigner, null);
					nextVisitor = new AnchorAssigningObjectGraphVisitor(nextVisitor, eventEmitter, anchorAssigner);
				}
				if (!IsOptionSet(SerializationOptions.EmitDefaults))
				{
					nextVisitor = new DefaultExclusiveObjectGraphVisitor(nextVisitor);
				}
				return nextVisitor;
			}

			private IEventEmitter CreateEventEmitter()
			{
				WriterEventEmitter nextEmitter = new WriterEventEmitter();
				if (IsOptionSet(SerializationOptions.JsonCompatible))
				{
					return new JsonEventEmitter(nextEmitter);
				}
				return new TypeAssigningEventEmitter(nextEmitter, IsOptionSet(SerializationOptions.Roundtrip));
			}

			private IObjectGraphTraversalStrategy CreateTraversalStrategy()
			{
				ITypeInspector innerTypeDescriptor = new ReadablePropertiesTypeInspector(typeResolver);
				if (IsOptionSet(SerializationOptions.Roundtrip))
				{
					innerTypeDescriptor = new ReadableAndWritablePropertiesTypeInspector(innerTypeDescriptor);
				}
				innerTypeDescriptor = new YamlAttributeOverridesInspector(innerTypeDescriptor, overrides);
				innerTypeDescriptor = new YamlAttributesTypeInspector(innerTypeDescriptor);
				innerTypeDescriptor = new NamingConventionTypeInspector(innerTypeDescriptor, namingConvention);
				if (IsOptionSet(SerializationOptions.DefaultToStaticType))
				{
					innerTypeDescriptor = new CachedTypeInspector(innerTypeDescriptor);
				}
				if (IsOptionSet(SerializationOptions.Roundtrip))
				{
					return new RoundtripObjectGraphTraversalStrategy(Converters, innerTypeDescriptor, typeResolver, 50);
				}
				return new FullObjectGraphTraversalStrategy(innerTypeDescriptor, typeResolver, 50, namingConvention);
			}

			public void SerializeValue(IEmitter emitter, object value, Type type)
			{
				ObjectDescriptor graph = ((type != null) ? new ObjectDescriptor(value, type, type) : new ObjectDescriptor(value, (value != null) ? value.GetType() : typeof(object), typeof(object)));
				IObjectGraphTraversalStrategy objectGraphTraversalStrategy = CreateTraversalStrategy();
				IObjectGraphVisitor<IEmitter> visitor = CreateEmittingVisitor(emitter, objectGraphTraversalStrategy, CreateEventEmitter(), graph);
				objectGraphTraversalStrategy.Traverse(graph, visitor, emitter);
			}
		}

		private readonly IValueSerializer valueSerializer;

		private readonly BackwardsCompatibleConfiguration backwardsCompatibleConfiguration;

		private void ThrowUnlessInBackwardsCompatibleMode()
		{
			if (backwardsCompatibleConfiguration == null)
			{
				throw new InvalidOperationException("This method / property exists for backwards compatibility reasons, but the Serializer was created using the new configuration mechanism. To configure the Serializer, use the SerializerBuilder.");
			}
		}

		[Obsolete("Please use SerializerBuilder to customize the Serializer. This constructor will be removed in future releases.")]
		public Serializer(SerializationOptions options = SerializationOptions.None, INamingConvention namingConvention = null, YamlAttributeOverrides overrides = null)
		{
			backwardsCompatibleConfiguration = new BackwardsCompatibleConfiguration(options, namingConvention, overrides);
		}

		[Obsolete("Please use SerializerBuilder to customize the Serializer. This method will be removed in future releases.")]
		public void RegisterTypeConverter(IYamlTypeConverter converter)
		{
			ThrowUnlessInBackwardsCompatibleMode();
			backwardsCompatibleConfiguration.Converters.Insert(0, converter);
		}

		public Serializer()
		{
			backwardsCompatibleConfiguration = new BackwardsCompatibleConfiguration(SerializationOptions.None, null, null);
		}

		private Serializer(IValueSerializer valueSerializer)
		{
			if (valueSerializer == null)
			{
				throw new ArgumentNullException("valueSerializer");
			}
			this.valueSerializer = valueSerializer;
		}

		public static Serializer FromValueSerializer(IValueSerializer valueSerializer)
		{
			return new Serializer(valueSerializer);
		}

		public void Serialize(TextWriter writer, object graph)
		{
			Serialize(new Emitter(writer), graph);
		}

		public string Serialize(object graph)
		{
			using StringWriter stringWriter = new StringWriter();
			Serialize(stringWriter, graph);
			return stringWriter.ToString();
		}

		public void Serialize(TextWriter writer, object graph, Type type)
		{
			Serialize(new Emitter(writer), graph, type);
		}

		public void Serialize(IEmitter emitter, object graph)
		{
			if (emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}
			EmitDocument(emitter, graph, null);
		}

		public void Serialize(IEmitter emitter, object graph, Type type)
		{
			if (emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			EmitDocument(emitter, graph, type);
		}

		private void EmitDocument(IEmitter emitter, object graph, Type type)
		{
			emitter.Emit(new StreamStart());
			emitter.Emit(new DocumentStart());
			IValueSerializer valueSerializer = backwardsCompatibleConfiguration;
			(valueSerializer ?? this.valueSerializer).SerializeValue(emitter, graph, type);
			emitter.Emit(new DocumentEnd(isImplicit: true));
			emitter.Emit(new StreamEnd());
		}
	}
}
