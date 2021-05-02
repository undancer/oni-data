using UnityEngine;

public static class KPrefabIDExtensions
{
	public static Tag PrefabID(this Component cmp)
	{
		return cmp.gameObject.PrefabID();
	}

	public static Tag PrefabID(this GameObject go)
	{
		return go.GetComponent<KPrefabID>().PrefabTag;
	}

	public static Tag PrefabID(this StateMachine.Instance smi)
	{
		return smi.GetComponent<KPrefabID>().PrefabTag;
	}

	public static bool HasTag(this Component cmp, Tag tag)
	{
		return cmp.gameObject.HasTag(tag);
	}

	public static bool HasTag(this GameObject go, Tag tag)
	{
		return go.GetComponent<KPrefabID>().HasTag(tag);
	}

	public static bool HasAnyTags(this Component cmp, Tag[] tags)
	{
		return cmp.gameObject.HasAnyTags(tags);
	}

	public static bool HasAnyTags(this GameObject go, Tag[] tags)
	{
		return go.GetComponent<KPrefabID>().HasAnyTags(tags);
	}

	public static bool HasAllTags(this Component cmp, Tag[] tags)
	{
		return cmp.gameObject.HasAllTags(tags);
	}

	public static bool HasAllTags(this GameObject go, Tag[] tags)
	{
		return go.GetComponent<KPrefabID>().HasAllTags(tags);
	}

	public static void AddTag(this GameObject go, Tag tag)
	{
		go.GetComponent<KPrefabID>().AddTag(tag);
	}

	public static void AddTag(this Component cmp, Tag tag)
	{
		cmp.gameObject.AddTag(tag);
	}

	public static void RemoveTag(this GameObject go, Tag tag)
	{
		go.GetComponent<KPrefabID>().RemoveTag(tag);
	}

	public static void RemoveTag(this Component cmp, Tag tag)
	{
		cmp.gameObject.RemoveTag(tag);
	}
}
