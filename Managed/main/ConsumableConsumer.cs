using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConsumableConsumer")]
public class ConsumableConsumer : KMonoBehaviour
{
	[Serialize]
	public Tag[] forbiddenTags;

	public System.Action consumableRulesChanged;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (ConsumerManager.instance != null)
		{
			forbiddenTags = ConsumerManager.instance.DefaultForbiddenTagsList.ToArray();
		}
		else
		{
			forbiddenTags = new Tag[0];
		}
	}

	public bool IsPermitted(string consumable_id)
	{
		Tag tag = new Tag(consumable_id);
		for (int i = 0; i < forbiddenTags.Length; i++)
		{
			if (forbiddenTags[i] == tag)
			{
				return false;
			}
		}
		return true;
	}

	public void SetPermitted(string consumable_id, bool is_allowed)
	{
		Tag item = new Tag(consumable_id);
		List<Tag> list = new List<Tag>(forbiddenTags);
		if (is_allowed)
		{
			list.Remove(item);
		}
		else if (!list.Contains(item))
		{
			list.Add(item);
		}
		forbiddenTags = list.ToArray();
		consumableRulesChanged.Signal();
	}
}
