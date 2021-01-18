using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MyCmp
{
	public enum MyCmpType
	{
		Req,
		Add,
		Get
	}

	public class FieldData
	{
		public MyCmpType myCmpType;

		public CmpFns cmpFns;

		public FieldInfo fieldInfo;
	}

	private static Dictionary<Type, FieldData[]> typeFieldInfos;

	private static void GetFieldDatas(List<FieldData> field_data_list, Type type)
	{
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			object[] customAttributes = fieldInfo.GetCustomAttributes(inherit: false);
			foreach (object obj in customAttributes)
			{
				bool flag = obj.GetType() == typeof(MyCmpAdd);
				bool flag2 = obj.GetType() == typeof(MyCmpReq);
				bool flag3 = obj.GetType() == typeof(MyCmpGet);
				if (!(flag || flag2 || flag3))
				{
					continue;
				}
				bool flag4 = true;
				foreach (FieldData item in field_data_list)
				{
					if (item.fieldInfo.Name == fieldInfo.Name)
					{
						flag4 = false;
						break;
					}
				}
				if (flag4)
				{
					FieldData fieldData = new FieldData();
					if (flag)
					{
						fieldData.myCmpType = MyCmpType.Add;
					}
					else if (flag2)
					{
						fieldData.myCmpType = MyCmpType.Req;
					}
					else if (flag3)
					{
						fieldData.myCmpType = MyCmpType.Get;
					}
					fieldData.cmpFns = CmpUtil.GetCmpFns(fieldInfo.FieldType);
					fieldData.fieldInfo = fieldInfo;
					field_data_list.Add(fieldData);
				}
			}
		}
		Type baseType = type.BaseType;
		if (baseType != typeof(KMonoBehaviour))
		{
			GetFieldDatas(field_data_list, baseType);
		}
	}

	public static FieldData[] GetFields(Type type)
	{
		if (typeFieldInfos == null)
		{
			typeFieldInfos = new Dictionary<Type, FieldData[]>();
		}
		FieldData[] value = null;
		if (!typeFieldInfos.TryGetValue(type, out value))
		{
			List<FieldData> list = new List<FieldData>();
			GetFieldDatas(list, type);
			value = list.ToArray();
			typeFieldInfos[type] = value;
		}
		return value;
	}

	public static void OnAwake(KMonoBehaviour c)
	{
		FieldData[] fields = GetFields(c.GetType());
		foreach (FieldData fieldData in fields)
		{
			CmpFns cmpFns = fieldData.cmpFns;
			FieldInfo fieldInfo = fieldData.fieldInfo;
			if (!((Component)fieldInfo.GetValue(c) != null))
			{
				if (fieldData.myCmpType == MyCmpType.Add)
				{
					Component value = cmpFns.mFindOrAddFn(c);
					fieldInfo.SetValue(c, value);
				}
				else if (fieldData.myCmpType == MyCmpType.Req)
				{
					Component value2 = cmpFns.mFindFn(c);
					fieldInfo.SetValue(c, value2);
				}
				else if (fieldData.myCmpType == MyCmpType.Get)
				{
					Component value3 = cmpFns.mFindFn(c);
					fieldInfo.SetValue(c, value3);
				}
			}
		}
	}

	public static void OnStart(KMonoBehaviour c)
	{
		Type type = c.GetType();
		FieldData[] fields = GetFields(type);
		foreach (FieldData fieldData in fields)
		{
			CmpFns cmpFns = fieldData.cmpFns;
			FieldInfo fieldInfo = fieldData.fieldInfo;
			if ((Component)fieldInfo.GetValue(c) != null)
			{
				Util.SpawnComponent(fieldInfo.GetValue(c) as Component);
			}
			else if (fieldData.myCmpType == MyCmpType.Add)
			{
				Util.SpawnComponent(cmpFns.mFindOrAddFn(c));
			}
			else if (fieldData.myCmpType == MyCmpType.Req)
			{
				Component component = cmpFns.mRequireFn(c);
				if (component == null)
				{
					Debug.LogError("The behaviour " + type.ToString() + " required but couldn't find a " + fieldInfo.FieldType.Name);
				}
				Util.SpawnComponent(component);
				fieldInfo.SetValue(c, component);
			}
			else if (fieldData.myCmpType == MyCmpType.Get)
			{
				Component component2 = cmpFns.mFindFn(c);
				Util.SpawnComponent(component2);
				fieldInfo.SetValue(c, component2);
			}
		}
	}
}
