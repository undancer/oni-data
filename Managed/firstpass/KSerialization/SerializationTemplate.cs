using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace KSerialization
{
	public class SerializationTemplate
	{
		public struct SerializationField
		{
			public FieldInfo field;

			public TypeInfo typeInfo;
		}

		public struct SerializationProperty
		{
			public PropertyInfo property;

			public TypeInfo typeInfo;
		}

		public Type serializableType;

		public TypeInfo typeInfo;

		public List<SerializationField> serializableFields = new List<SerializationField>();

		public List<SerializationProperty> serializableProperties = new List<SerializationProperty>();

		public MethodInfo onSerializing;

		public MethodInfo onSerialized;

		public MethodInfo customSerialize;

		private MemberSerialization GetSerializationConfig(Type type)
		{
			MemberSerialization memberSerialization = MemberSerialization.Invalid;
			Type type2 = null;
			while (type != typeof(object))
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(SerializationConfig), inherit: false);
				for (int i = 0; i < customAttributes.Length; i++)
				{
					Attribute attribute = (Attribute)customAttributes[i];
					if (attribute is SerializationConfig)
					{
						SerializationConfig serializationConfig = attribute as SerializationConfig;
						if (serializationConfig.MemberSerialization != memberSerialization && memberSerialization != MemberSerialization.Invalid)
						{
							string text = "Found conflicting serialization configurations on type " + type2.ToString() + " and " + type.ToString();
							Debug.LogError(text);
							throw new ArgumentException(text);
						}
						memberSerialization = serializationConfig.MemberSerialization;
						type2 = type.BaseType;
						break;
					}
				}
				type = type.BaseType;
			}
			if (memberSerialization == MemberSerialization.Invalid)
			{
				memberSerialization = MemberSerialization.OptOut;
			}
			return memberSerialization;
		}

		public SerializationTemplate(Type type)
		{
			serializableType = type;
			typeInfo = Manager.GetTypeInfo(type);
			type.GetSerializationMethods(typeof(OnSerializingAttribute), typeof(OnSerializedAttribute), typeof(CustomSerialize), out onSerializing, out onSerialized, out customSerialize);
			MemberSerialization serializationConfig = GetSerializationConfig(type);
			if (serializationConfig != 0)
			{
				if (serializationConfig == MemberSerialization.OptIn)
				{
					while (type != typeof(object))
					{
						AddOptInFields(type);
						AddOptInProperties(type);
						type = type.BaseType;
					}
				}
			}
			else
			{
				while (type != typeof(object))
				{
					AddPublicFields(type);
					AddPublicProperties(type);
					type = type.BaseType;
				}
			}
		}

		public override string ToString()
		{
			string text = "Template: " + serializableType.ToString() + "\n";
			foreach (SerializationField serializableField in serializableFields)
			{
				text = text + "\t" + serializableField.ToString() + "\n";
			}
			return text;
		}

		private void AddPublicFields(Type type)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo field in fields)
			{
				AddValidField(field);
			}
		}

		private void AddOptInFields(Type type)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(inherit: false);
				foreach (object obj in customAttributes)
				{
					if (obj != null && obj is Serialize)
					{
						AddValidField(fieldInfo);
					}
				}
			}
		}

		private void AddValidField(FieldInfo field)
		{
			object[] customAttributes = field.GetCustomAttributes(typeof(NonSerializedAttribute), inherit: false);
			if (customAttributes == null || customAttributes.Length == 0)
			{
				serializableFields.Add(new SerializationField
				{
					field = field,
					typeInfo = Manager.GetTypeInfo(field.FieldType)
				});
			}
		}

		private void AddPublicProperties(Type type)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			foreach (PropertyInfo property in properties)
			{
				AddValidProperty(property);
			}
		}

		private void AddOptInProperties(Type type)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				object[] customAttributes = propertyInfo.GetCustomAttributes(inherit: false);
				foreach (object obj in customAttributes)
				{
					if (obj != null && obj is Serialize)
					{
						AddValidProperty(propertyInfo);
					}
				}
			}
		}

		private void AddValidProperty(PropertyInfo property)
		{
			if (property.GetIndexParameters().Length == 0)
			{
				object[] customAttributes = property.GetCustomAttributes(typeof(NonSerializedAttribute), inherit: false);
				if ((customAttributes == null || customAttributes.Length == 0) && property.GetSetMethod() != null)
				{
					serializableProperties.Add(new SerializationProperty
					{
						property = property,
						typeInfo = Manager.GetTypeInfo(property.PropertyType)
					});
				}
			}
		}

		public void SerializeTemplate(BinaryWriter writer)
		{
			writer.Write(serializableFields.Count);
			writer.Write(serializableProperties.Count);
			foreach (SerializationField serializableField in serializableFields)
			{
				writer.WriteKleiString(serializableField.field.Name);
				Type fieldType = serializableField.field.FieldType;
				WriteType(writer, fieldType);
			}
			foreach (SerializationProperty serializableProperty in serializableProperties)
			{
				writer.WriteKleiString(serializableProperty.property.Name);
				Type propertyType = serializableProperty.property.PropertyType;
				WriteType(writer, propertyType);
			}
		}

		private void WriteType(BinaryWriter writer, Type type)
		{
			SerializationTypeInfo serializationTypeInfo = Helper.EncodeSerializationType(type);
			writer.Write((byte)serializationTypeInfo);
			if (type.IsGenericType)
			{
				if (Helper.IsUserDefinedType(serializationTypeInfo))
				{
					writer.WriteKleiString(type.GetKTypeString());
				}
				Type[] genericArguments = type.GetGenericArguments();
				writer.Write((byte)genericArguments.Length);
				for (int i = 0; i < genericArguments.Length; i++)
				{
					WriteType(writer, genericArguments[i]);
				}
			}
			else if (Helper.IsArray(serializationTypeInfo))
			{
				Type elementType = type.GetElementType();
				WriteType(writer, elementType);
			}
			else if (type.IsEnum || Helper.IsUserDefinedType(serializationTypeInfo))
			{
				writer.WriteKleiString(type.GetKTypeString());
			}
		}

		public void SerializeData(object obj, BinaryWriter writer)
		{
			if (onSerializing != null)
			{
				onSerializing.Invoke(obj, null);
			}
			foreach (SerializationField serializableField in serializableFields)
			{
				try
				{
					object value = serializableField.field.GetValue(obj);
					writer.WriteValue(serializableField.typeInfo, value);
				}
				catch (Exception innerException)
				{
					string text = $"Error occurred while serializing field {serializableField.field.Name} on template {serializableType.Name}";
					Debug.LogError(text);
					throw new ArgumentException(text, innerException);
				}
			}
			foreach (SerializationProperty serializableProperty in serializableProperties)
			{
				try
				{
					object value2 = serializableProperty.property.GetValue(obj, null);
					writer.WriteValue(serializableProperty.typeInfo, value2);
				}
				catch (Exception innerException2)
				{
					string text2 = $"Error occurred while serializing property {serializableProperty.property.Name} on template {serializableType.Name}";
					Debug.LogError(text2);
					throw new ArgumentException(text2, innerException2);
				}
			}
			if (customSerialize != null)
			{
				customSerialize.Invoke(obj, new object[1] { writer });
			}
			if (onSerialized != null)
			{
				onSerialized.Invoke(obj, null);
			}
		}
	}
}
