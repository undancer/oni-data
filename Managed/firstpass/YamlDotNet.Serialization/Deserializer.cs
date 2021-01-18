using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;
using YamlDotNet.Serialization.NodeTypeResolvers;
using YamlDotNet.Serialization.ObjectFactories;
using YamlDotNet.Serialization.TypeInspectors;
using YamlDotNet.Serialization.TypeResolvers;
using YamlDotNet.Serialization.Utilities;
using YamlDotNet.Serialization.ValueDeserializers;

namespace YamlDotNet.Serialization
{
	public sealed class Deserializer
	{
		private class BackwardsCompatibleConfiguration
		{
			private class TypeDescriptorProxy : ITypeInspector
			{
				public ITypeInspector TypeDescriptor;

				public IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
				{
					return TypeDescriptor.GetProperties(type, container);
				}

				public IPropertyDescriptor GetProperty(Type type, object container, string name, bool ignoreUnmatched)
				{
					return TypeDescriptor.GetProperty(type, container, name, ignoreUnmatched);
				}
			}

			private static readonly Dictionary<string, Type> predefinedTagMappings = new Dictionary<string, Type>
			{
				{
					"tag:yaml.org,2002:map",
					typeof(Dictionary<object, object>)
				},
				{
					"tag:yaml.org,2002:bool",
					typeof(bool)
				},
				{
					"tag:yaml.org,2002:float",
					typeof(double)
				},
				{
					"tag:yaml.org,2002:int",
					typeof(int)
				},
				{
					"tag:yaml.org,2002:str",
					typeof(string)
				},
				{
					"tag:yaml.org,2002:timestamp",
					typeof(DateTime)
				}
			};

			private readonly Dictionary<string, Type> tagMappings;

			private readonly List<IYamlTypeConverter> converters;

			private TypeDescriptorProxy typeDescriptor = new TypeDescriptorProxy();

			public IValueDeserializer valueDeserializer;

			public IList<INodeDeserializer> NodeDeserializers
			{
				get;
				private set;
			}

			public IList<INodeTypeResolver> TypeResolvers
			{
				get;
				private set;
			}

			public BackwardsCompatibleConfiguration(IObjectFactory objectFactory, INamingConvention namingConvention, bool ignoreUnmatched, YamlAttributeOverrides overrides)
			{
				objectFactory = objectFactory ?? new DefaultObjectFactory();
				namingConvention = namingConvention ?? new NullNamingConvention();
				typeDescriptor.TypeDescriptor = new CachedTypeInspector(new NamingConventionTypeInspector(new YamlAttributesTypeInspector(new YamlAttributeOverridesInspector(new ReadableAndWritablePropertiesTypeInspector(new ReadablePropertiesTypeInspector(new StaticTypeResolver())), overrides)), namingConvention));
				converters = new List<IYamlTypeConverter>();
				converters.Add(new GuidConverter(jsonCompatible: false));
				NodeDeserializers = new List<INodeDeserializer>();
				NodeDeserializers.Add(new YamlConvertibleNodeDeserializer(objectFactory));
				NodeDeserializers.Add(new YamlSerializableNodeDeserializer(objectFactory));
				NodeDeserializers.Add(new TypeConverterNodeDeserializer(converters));
				NodeDeserializers.Add(new NullNodeDeserializer());
				NodeDeserializers.Add(new ScalarNodeDeserializer());
				NodeDeserializers.Add(new ArrayNodeDeserializer());
				NodeDeserializers.Add(new DictionaryNodeDeserializer(objectFactory));
				NodeDeserializers.Add(new CollectionNodeDeserializer(objectFactory));
				NodeDeserializers.Add(new EnumerableNodeDeserializer());
				NodeDeserializers.Add(new ObjectNodeDeserializer(objectFactory, typeDescriptor, ignoreUnmatched));
				tagMappings = new Dictionary<string, Type>(predefinedTagMappings);
				TypeResolvers = new List<INodeTypeResolver>();
				TypeResolvers.Add(new YamlConvertibleTypeResolver());
				TypeResolvers.Add(new YamlSerializableTypeResolver());
				TypeResolvers.Add(new TagNodeTypeResolver(tagMappings));
				TypeResolvers.Add(new TypeNameInTagNodeTypeResolver());
				TypeResolvers.Add(new DefaultContainersNodeTypeResolver());
				valueDeserializer = new AliasValueDeserializer(new NodeValueDeserializer(NodeDeserializers, TypeResolvers));
			}

			public void RegisterTagMapping(string tag, Type type)
			{
				tagMappings.Add(tag, type);
			}

			public void RegisterTypeConverter(IYamlTypeConverter typeConverter)
			{
				converters.Insert(0, typeConverter);
			}
		}

		private readonly BackwardsCompatibleConfiguration backwardsCompatibleConfiguration;

		private readonly IValueDeserializer valueDeserializer;

		[Obsolete("Please use DeserializerBuilder to customize the Deserializer. This property will be removed in future releases.")]
		public IList<INodeDeserializer> NodeDeserializers
		{
			get
			{
				ThrowUnlessInBackwardsCompatibleMode();
				return backwardsCompatibleConfiguration.NodeDeserializers;
			}
		}

		[Obsolete("Please use DeserializerBuilder to customize the Deserializer. This property will be removed in future releases.")]
		public IList<INodeTypeResolver> TypeResolvers
		{
			get
			{
				ThrowUnlessInBackwardsCompatibleMode();
				return backwardsCompatibleConfiguration.TypeResolvers;
			}
		}

		private void ThrowUnlessInBackwardsCompatibleMode()
		{
			if (backwardsCompatibleConfiguration == null)
			{
				throw new InvalidOperationException("This method / property exists for backwards compatibility reasons, but the Deserializer was created using the new configuration mechanism. To configure the Deserializer, use the DeserializerBuilder.");
			}
		}

		[Obsolete("Please use DeserializerBuilder to customize the Deserializer. This constructor will be removed in future releases.")]
		public Deserializer(IObjectFactory objectFactory = null, INamingConvention namingConvention = null, bool ignoreUnmatched = false, YamlAttributeOverrides overrides = null)
		{
			backwardsCompatibleConfiguration = new BackwardsCompatibleConfiguration(objectFactory, namingConvention, ignoreUnmatched, overrides);
			valueDeserializer = backwardsCompatibleConfiguration.valueDeserializer;
		}

		[Obsolete("Please use DeserializerBuilder to customize the Deserializer. This method will be removed in future releases.")]
		public void RegisterTagMapping(string tag, Type type)
		{
			ThrowUnlessInBackwardsCompatibleMode();
			backwardsCompatibleConfiguration.RegisterTagMapping(tag, type);
		}

		[Obsolete("Please use DeserializerBuilder to customize the Deserializer. This method will be removed in future releases.")]
		public void RegisterTypeConverter(IYamlTypeConverter typeConverter)
		{
			ThrowUnlessInBackwardsCompatibleMode();
			backwardsCompatibleConfiguration.RegisterTypeConverter(typeConverter);
		}

		public Deserializer()
		{
			backwardsCompatibleConfiguration = new BackwardsCompatibleConfiguration(null, null, ignoreUnmatched: false, null);
			valueDeserializer = backwardsCompatibleConfiguration.valueDeserializer;
		}

		private Deserializer(IValueDeserializer valueDeserializer)
		{
			if (valueDeserializer == null)
			{
				throw new ArgumentNullException("valueDeserializer");
			}
			this.valueDeserializer = valueDeserializer;
		}

		public static Deserializer FromValueDeserializer(IValueDeserializer valueDeserializer)
		{
			return new Deserializer(valueDeserializer);
		}

		public T Deserialize<T>(string input)
		{
			using StringReader input2 = new StringReader(input);
			return (T)Deserialize(input2, typeof(T));
		}

		public T Deserialize<T>(TextReader input)
		{
			return (T)Deserialize(input, typeof(T));
		}

		public object Deserialize(TextReader input)
		{
			return Deserialize(input, typeof(object));
		}

		public object Deserialize(string input, Type type)
		{
			using StringReader input2 = new StringReader(input);
			return Deserialize(input2, type);
		}

		public object Deserialize(TextReader input, Type type)
		{
			return Deserialize(new Parser(input), type);
		}

		public T Deserialize<T>(IParser parser)
		{
			return (T)Deserialize(parser, typeof(T));
		}

		public object Deserialize(IParser parser)
		{
			return Deserialize(parser, typeof(object));
		}

		public object Deserialize(IParser parser, Type type)
		{
			if (parser == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			bool flag = parser.Allow<StreamStart>() != null;
			bool flag2 = parser.Allow<DocumentStart>() != null;
			object result = null;
			if (!parser.Accept<DocumentEnd>() && !parser.Accept<StreamEnd>())
			{
				using SerializerState serializerState = new SerializerState();
				result = valueDeserializer.DeserializeValue(parser, type, serializerState, valueDeserializer);
				serializerState.OnDeserialization();
			}
			if (flag2)
			{
				parser.Expect<DocumentEnd>();
			}
			if (flag)
			{
				parser.Expect<StreamEnd>();
			}
			return result;
		}
	}
}
