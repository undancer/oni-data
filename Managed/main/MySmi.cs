using System;
using System.Collections.Generic;
using System.Reflection;

public class MySmi : MyAttributeManager<StateMachine.Instance>
{
	public static void Init()
	{
		MyAttributes.Register(new MySmi(new Dictionary<Type, MethodInfo>
		{
			{
				typeof(MySmiGet),
				typeof(MySmi).GetMethod("FindSmi")
			},
			{
				typeof(MySmiReq),
				typeof(MySmi).GetMethod("RequireSmi")
			}
		}));
	}

	public MySmi(Dictionary<Type, MethodInfo> attributeMap)
		: base(attributeMap, (Action<StateMachine.Instance>)null)
	{
	}

	public static StateMachine.Instance FindSmi<T>(KMonoBehaviour c, bool isStart) where T : StateMachine.Instance
	{
		StateMachineController component = c.GetComponent<StateMachineController>();
		if (component != null)
		{
			return component.GetSMI<T>();
		}
		return null;
	}

	public static StateMachine.Instance RequireSmi<T>(KMonoBehaviour c, bool isStart) where T : StateMachine.Instance
	{
		if (isStart)
		{
			StateMachine.Instance instance = FindSmi<T>(c, isStart);
			Debug.Assert(instance != null, $"{c.GetType().ToString()} '{c.name}' requires a StateMachineInstance of type {typeof(T)}!");
			return instance;
		}
		return FindSmi<T>(c, isStart);
	}
}
