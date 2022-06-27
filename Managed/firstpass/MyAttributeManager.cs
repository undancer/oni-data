using System;
using System.Collections.Generic;
using System.Reflection;

public abstract class MyAttributeManager<T> : IAttributeManager where T : class
{
	private class FieldData
	{
		public Type myAttributeType;

		public AttrFns attrFns;

		public FieldInfo fieldInfo;
	}

	private class AttrFns
	{
		private Dictionary<Type, Func<KMonoBehaviour, bool, T>> m_fnsByAttribute = new Dictionary<Type, Func<KMonoBehaviour, bool, T>>();

		public AttrFns(Type type, Dictionary<Type, MethodInfo> methodInfosByAttribute)
		{
			foreach (KeyValuePair<Type, MethodInfo> item in methodInfosByAttribute)
			{
				MethodInfo method = null;
				try
				{
					method = item.Value.MakeGenericMethod(type);
				}
				catch (Exception arg)
				{
					Debug.LogError($"Exception for type {type}: {arg}");
				}
				Func<KMonoBehaviour, bool, T> value = (Func<KMonoBehaviour, bool, T>)Delegate.CreateDelegate(typeof(Func<KMonoBehaviour, bool, T>), method);
				m_fnsByAttribute[item.Key] = value;
			}
		}

		public Func<KMonoBehaviour, bool, T> GetFunction(Type attribute)
		{
			return m_fnsByAttribute[attribute];
		}
	}

	private Dictionary<Type, FieldData[]> m_typeFieldInfos;

	private Action<T> m_spawnFunc;

	private Dictionary<Type, MethodInfo> m_methodInfosByAttribute = new Dictionary<Type, MethodInfo>();

	private Dictionary<Type, AttrFns> m_attrFns = new Dictionary<Type, AttrFns>();

	public MyAttributeManager(Dictionary<Type, MethodInfo> attributeMap, Action<T> spawnFunc = null)
	{
		m_methodInfosByAttribute = attributeMap;
		m_spawnFunc = spawnFunc;
	}

	private void GetFieldDatas(List<FieldData> field_data_list, Type type)
	{
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			object[] customAttributes = fieldInfo.GetCustomAttributes(inherit: false);
			for (int j = 0; j < customAttributes.Length; j++)
			{
				Type type2 = customAttributes[j].GetType();
				if (!IsFunctionAttribute(type2))
				{
					continue;
				}
				bool flag = true;
				foreach (FieldData item in field_data_list)
				{
					if (item.fieldInfo.Name == fieldInfo.Name)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					FieldData fieldData = new FieldData();
					fieldData.myAttributeType = type2;
					fieldData.attrFns = GetAttrFns(fieldInfo.FieldType);
					fieldData.fieldInfo = fieldInfo;
					field_data_list.Add(fieldData);
				}
			}
		}
		Type baseType = type.BaseType;
		if (baseType != typeof(KMonoBehaviour) && baseType != typeof(object) && baseType != null)
		{
			GetFieldDatas(field_data_list, baseType);
		}
	}

	private FieldData[] GetFields(Type type)
	{
		if (m_typeFieldInfos == null)
		{
			m_typeFieldInfos = new Dictionary<Type, FieldData[]>();
		}
		FieldData[] value = null;
		if (!m_typeFieldInfos.TryGetValue(type, out value))
		{
			List<FieldData> list = new List<FieldData>();
			GetFieldDatas(list, type);
			value = list.ToArray();
			m_typeFieldInfos[type] = value;
		}
		return value;
	}

	public void OnAwake(object obj, KMonoBehaviour cmp)
	{
		Type type = obj.GetType();
		FieldData[] fields = GetFields(type);
		foreach (FieldData fieldData in fields)
		{
			AttrFns attrFns = fieldData.attrFns;
			FieldInfo fieldInfo = fieldData.fieldInfo;
			if ((T)fieldInfo.GetValue(obj) == null)
			{
				T value = attrFns.GetFunction(fieldData.myAttributeType)(cmp, arg2: false);
				fieldInfo.SetValue(obj, value);
			}
		}
	}

	public void OnStart(object obj, KMonoBehaviour cmp)
	{
		Type type = obj.GetType();
		FieldData[] fields = GetFields(type);
		foreach (FieldData fieldData in fields)
		{
			AttrFns attrFns = fieldData.attrFns;
			FieldInfo fieldInfo = fieldData.fieldInfo;
			if (fieldInfo.GetValue(obj) is T obj2)
			{
				if (m_spawnFunc != null)
				{
					m_spawnFunc(obj2);
				}
				continue;
			}
			T val = attrFns.GetFunction(fieldData.myAttributeType)(cmp, arg2: true);
			if (val != null && m_spawnFunc != null)
			{
				m_spawnFunc(val);
			}
			fieldInfo.SetValue(obj, val);
		}
	}

	private bool IsFunctionAttribute(Type attribute)
	{
		foreach (KeyValuePair<Type, MethodInfo> item in m_methodInfosByAttribute)
		{
			if (attribute == item.Key)
			{
				return true;
			}
		}
		return false;
	}

	private AttrFns GetAttrFns(Type type)
	{
		AttrFns value = null;
		if (!m_attrFns.TryGetValue(type, out value))
		{
			value = new AttrFns(type, m_methodInfosByAttribute);
			m_attrFns[type] = value;
		}
		return value;
	}
}
