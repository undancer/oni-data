using System;
using System.Collections.Generic;
using System.Linq;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public abstract class PreProcessingPhaseObjectGraphVisitorSkeleton : IObjectGraphVisitor<Nothing>
	{
		protected readonly IEnumerable<IYamlTypeConverter> typeConverters;

		public PreProcessingPhaseObjectGraphVisitorSkeleton(IEnumerable<IYamlTypeConverter> typeConverters)
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
		}

		bool IObjectGraphVisitor<Nothing>.Enter(IObjectDescriptor value, Nothing context)
		{
			IYamlTypeConverter yamlTypeConverter = typeConverters.FirstOrDefault((IYamlTypeConverter t) => t.Accepts(value.Type));
			if (yamlTypeConverter != null)
			{
				return false;
			}
			IYamlConvertible yamlConvertible = value.Value as IYamlConvertible;
			if (yamlConvertible != null)
			{
				return false;
			}
			IYamlSerializable yamlSerializable = value.Value as IYamlSerializable;
			if (yamlSerializable != null)
			{
				return false;
			}
			return Enter(value);
		}

		bool IObjectGraphVisitor<Nothing>.EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, Nothing context)
		{
			return EnterMapping(key, value);
		}

		bool IObjectGraphVisitor<Nothing>.EnterMapping(IObjectDescriptor key, IObjectDescriptor value, Nothing context)
		{
			return EnterMapping(key, value);
		}

		void IObjectGraphVisitor<Nothing>.VisitMappingEnd(IObjectDescriptor mapping, Nothing context)
		{
			VisitMappingEnd(mapping);
		}

		void IObjectGraphVisitor<Nothing>.VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType, Nothing context)
		{
			VisitMappingStart(mapping, keyType, valueType);
		}

		void IObjectGraphVisitor<Nothing>.VisitScalar(IObjectDescriptor scalar, Nothing context)
		{
			VisitScalar(scalar);
		}

		void IObjectGraphVisitor<Nothing>.VisitSequenceEnd(IObjectDescriptor sequence, Nothing context)
		{
			VisitSequenceEnd(sequence);
		}

		void IObjectGraphVisitor<Nothing>.VisitSequenceStart(IObjectDescriptor sequence, Type elementType, Nothing context)
		{
			VisitSequenceStart(sequence, elementType);
		}

		protected abstract bool Enter(IObjectDescriptor value);

		protected abstract bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value);

		protected abstract bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value);

		protected abstract void VisitMappingEnd(IObjectDescriptor mapping);

		protected abstract void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType);

		protected abstract void VisitScalar(IObjectDescriptor scalar);

		protected abstract void VisitSequenceEnd(IObjectDescriptor sequence);

		protected abstract void VisitSequenceStart(IObjectDescriptor sequence, Type elementType);
	}
}
