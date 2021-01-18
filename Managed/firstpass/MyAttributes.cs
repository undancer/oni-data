using System.Collections.Generic;

public static class MyAttributes
{
	private static List<IAttributeManager> s_attributeMgrs = new List<IAttributeManager>();

	public static void Register(IAttributeManager mgr)
	{
		s_attributeMgrs.Add(mgr);
	}

	public static void OnAwake(object obj, KMonoBehaviour cmp)
	{
		foreach (IAttributeManager s_attributeMgr in s_attributeMgrs)
		{
			s_attributeMgr.OnAwake(obj, cmp);
		}
	}

	public static void OnAwake(KMonoBehaviour c)
	{
		OnAwake(c, c);
	}

	public static void OnStart(object obj, KMonoBehaviour cmp)
	{
		foreach (IAttributeManager s_attributeMgr in s_attributeMgrs)
		{
			s_attributeMgr.OnStart(obj, cmp);
		}
	}

	public static void OnStart(KMonoBehaviour c)
	{
		OnStart(c, c);
	}
}
