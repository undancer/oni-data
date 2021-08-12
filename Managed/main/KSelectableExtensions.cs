using UnityEngine;

public static class KSelectableExtensions
{
	public static string GetProperName(this Component cmp)
	{
		if (cmp != null && cmp.gameObject != null)
		{
			return cmp.gameObject.GetProperName();
		}
		return "";
	}

	public static string GetProperName(this GameObject go)
	{
		if (go != null)
		{
			KSelectable component = go.GetComponent<KSelectable>();
			if (component != null)
			{
				return component.GetName();
			}
		}
		return "";
	}

	public static string GetProperName(this KSelectable cmp)
	{
		if (cmp != null)
		{
			return cmp.GetName();
		}
		return "";
	}
}
