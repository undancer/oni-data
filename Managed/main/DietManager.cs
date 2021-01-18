using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DietManager")]
public class DietManager : KMonoBehaviour
{
	private Dictionary<Tag, Diet> diets;

	public static DietManager Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		diets = CollectDiets(null);
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (Tag item in WorldInventory.Instance.GetDiscovered())
		{
			Discover(item);
		}
		foreach (KeyValuePair<Tag, Diet> diet in diets)
		{
			Diet.Info[] infos = diet.Value.infos;
			for (int i = 0; i < infos.Length; i++)
			{
				foreach (Tag consumedTag in infos[i].consumedTags)
				{
					if (Assets.GetPrefab(consumedTag) == null)
					{
						Debug.LogError("Could not find prefab: " + consumedTag);
					}
				}
			}
		}
		WorldInventory.Instance.OnDiscover += OnWorldInventoryDiscover;
	}

	private void Discover(Tag tag)
	{
		foreach (KeyValuePair<Tag, Diet> diet in diets)
		{
			if (diet.Value.GetDietInfo(tag) != null)
			{
				WorldInventory.Instance.Discover(tag, diet.Key);
			}
		}
	}

	private void OnWorldInventoryDiscover(Tag category_tag, Tag tag)
	{
		Discover(tag);
	}

	public static Dictionary<Tag, Diet> CollectDiets(Tag[] target_species)
	{
		Dictionary<Tag, Diet> dictionary = new Dictionary<Tag, Diet>();
		foreach (KPrefabID prefab in Assets.Prefabs)
		{
			CreatureCalorieMonitor.Def def = prefab.GetDef<CreatureCalorieMonitor.Def>();
			if (def != null && (target_species == null || Array.IndexOf(target_species, prefab.GetComponent<CreatureBrain>().species) >= 0))
			{
				dictionary[prefab.PrefabTag] = def.diet;
			}
		}
		return dictionary;
	}
}
