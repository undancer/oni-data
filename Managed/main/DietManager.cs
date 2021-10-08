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
		foreach (Tag item in DiscoveredResources.Instance.GetDiscovered())
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
						Debug.LogError($"Could not find prefab {consumedTag}, required by diet for {diet.Key}");
					}
				}
			}
		}
		DiscoveredResources.Instance.OnDiscover += OnWorldInventoryDiscover;
	}

	private void Discover(Tag tag)
	{
		foreach (KeyValuePair<Tag, Diet> diet in diets)
		{
			if (diet.Value.GetDietInfo(tag) != null)
			{
				DiscoveredResources.Instance.Discover(tag, diet.Key);
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
			BeehiveCalorieMonitor.Def def2 = prefab.GetDef<BeehiveCalorieMonitor.Def>();
			Diet diet = null;
			if (def != null)
			{
				diet = def.diet;
			}
			else if (def2 != null)
			{
				diet = def2.diet;
			}
			if (diet != null && (target_species == null || Array.IndexOf(target_species, prefab.GetComponent<CreatureBrain>().species) >= 0))
			{
				dictionary[prefab.PrefabTag] = diet;
			}
		}
		return dictionary;
	}
}
