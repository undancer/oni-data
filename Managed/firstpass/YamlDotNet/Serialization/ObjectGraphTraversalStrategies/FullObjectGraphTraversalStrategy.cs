using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Helpers;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.ObjectGraphTraversalStrategies
{
	public class FullObjectGraphTraversalStrategy : IObjectGraphTraversalStrategy
	{
		private readonly int maxRecursion;

		private readonly ITypeInspector typeDescriptor;

		private readonly ITypeResolver typeResolver;

		private INamingConvention namingConvention;

		public FullObjectGraphTraversalStrategy(ITypeInspector typeDescriptor, ITypeResolver typeResolver, int maxRecursion, INamingConvention namingConvention)
		{
			if (maxRecursion <= 0)
			{
				throw new ArgumentOutOfRangeException("maxRecursion", maxRecursion, "maxRecursion must be greater than 1");
			}
			if (typeDescriptor == null)
			{
				throw new ArgumentNullException("typeDescriptor");
			}
			this.typeDescriptor = typeDescriptor;
			if (typeResolver == null)
			{
				throw new ArgumentNullException("typeResolver");
			}
			this.typeResolver = typeResolver;
			this.maxRecursion = maxRecursion;
			this.namingConvention = namingConvention;
		}

		void IObjectGraphTraversalStrategy.Traverse<TContext>(IObjectDescriptor graph, IObjectGraphVisitor<TContext> visitor, TContext context)
		{
			Traverse(graph, visitor, 0, context);
		}

		protected virtual void Traverse<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			if (++currentDepth > maxRecursion)
			{
				throw new InvalidOperationException("Too much recursion when traversing the object graph");
			}
			if (!visitor.Enter(value, context))
			{
				return;
			}
			TypeCode typeCode = value.Type.GetTypeCode();
			switch (typeCode)
			{
			case TypeCode.Boolean:
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
			case TypeCode.DateTime:
			case TypeCode.String:
				visitor.VisitScalar(value, context);
				return;
			case TypeCode.DBNull:
				visitor.VisitScalar(new ObjectDescriptor(null, typeof(object), typeof(object)), context);
				return;
			case TypeCode.Empty:
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "TypeCode.{0} is not supported.", typeCode));
			}
			if (value.Value == null || value.Type == typeof(TimeSpan))
			{
				visitor.VisitScalar(value, context);
				return;
			}
			Type underlyingType = Nullable.GetUnderlyingType(value.Type);
			if (underlyingType != null)
			{
				Traverse(new ObjectDescriptor(value.Value, underlyingType, value.Type, value.ScalarStyle), visitor, currentDepth, context);
			}
			else
			{
				TraverseObject(value, visitor, currentDepth, context);
			}
		}

		protected virtual void TraverseObject<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			if (typeof(IDictionary).IsAssignableFrom(value.Type))
			{
				TraverseDictionary(value, visitor, currentDepth, typeof(object), typeof(object), context);
				return;
			}
			Type implementedGenericInterface = ReflectionUtility.GetImplementedGenericInterface(value.Type, typeof(IDictionary<, >));
			if (implementedGenericInterface != null)
			{
				GenericDictionaryToNonGenericAdapter value2 = new GenericDictionaryToNonGenericAdapter(value.Value, implementedGenericInterface);
				Type[] genericArguments = implementedGenericInterface.GetGenericArguments();
				TraverseDictionary(new ObjectDescriptor(value2, value.Type, value.StaticType, value.ScalarStyle), visitor, currentDepth, genericArguments[0], genericArguments[1], context);
			}
			else if (typeof(IEnumerable).IsAssignableFrom(value.Type))
			{
				TraverseList(value, visitor, currentDepth, context);
			}
			else
			{
				TraverseProperties(value, visitor, currentDepth, context);
			}
		}

		protected virtual void TraverseDictionary<TContext>(IObjectDescriptor dictionary, IObjectGraphVisitor<TContext> visitor, int currentDepth, Type keyType, Type valueType, TContext context)
		{
			visitor.VisitMappingStart(dictionary, keyType, valueType, context);
			bool flag = dictionary.Type.FullName.Equals("System.Dynamic.ExpandoObject");
			foreach (DictionaryEntry item in (IDictionary)dictionary.Value)
			{
				string value = (flag ? namingConvention.Apply(item.Key.ToString()) : item.Key.ToString());
				IObjectDescriptor objectDescriptor = GetObjectDescriptor(value, keyType);
				IObjectDescriptor objectDescriptor2 = GetObjectDescriptor(item.Value, valueType);
				if (visitor.EnterMapping(objectDescriptor, objectDescriptor2, context))
				{
					Traverse(objectDescriptor, visitor, currentDepth, context);
					Traverse(objectDescriptor2, visitor, currentDepth, context);
				}
			}
			visitor.VisitMappingEnd(dictionary, context);
		}

		private void TraverseList<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			Type implementedGenericInterface = ReflectionUtility.GetImplementedGenericInterface(value.Type, typeof(IEnumerable<>));
			Type type = ((implementedGenericInterface != null) ? implementedGenericInterface.GetGenericArguments()[0] : typeof(object));
			visitor.VisitSequenceStart(value, type, context);
			foreach (object item in (IEnumerable)value.Value)
			{
				Traverse(GetObjectDescriptor(item, type), visitor, currentDepth, context);
			}
			visitor.VisitSequenceEnd(value, context);
		}

		protected virtual void TraverseProperties<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			visitor.VisitMappingStart(value, typeof(string), typeof(object), context);
			foreach (IPropertyDescriptor property in typeDescriptor.GetProperties(value.Type, value.Value))
			{
				IObjectDescriptor value2 = property.Read(value.Value);
				if (visitor.EnterMapping(property, value2, context))
				{
					Traverse(new ObjectDescriptor(property.Name, typeof(string), typeof(string)), visitor, currentDepth, context);
					Traverse(value2, visitor, currentDepth, context);
				}
			}
			visitor.VisitMappingEnd(value, context);
		}

		private IObjectDescriptor GetObjectDescriptor(object value, Type staticType)
		{
			return new ObjectDescriptor(value, typeResolver.Resolve(staticType, value), staticType);
		}
	}
}
