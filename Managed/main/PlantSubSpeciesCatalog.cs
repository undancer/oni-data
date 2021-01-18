using System.Collections.Generic;
using System.Runtime.Serialization;
using Database;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class PlantSubSpeciesCatalog : KMonoBehaviour
{
	public class PlantSubSpecies
	{
		[Serialize]
		public int id;

		[Serialize]
		public string rootSpeciesID;

		[Serialize]
		public List<Trait> mutations = new List<Trait>();

		public PlantSubSpecies(int id, string rootSpeciesID)
		{
			this.id = id;
			this.rootSpeciesID = rootSpeciesID;
		}

		public Tag GetFilterTag()
		{
			return new Tag(rootSpeciesID + "_" + id);
		}

		public string ProperName()
		{
			if (mutations.Count == 0)
			{
				return CREATURES.PLANT_MUTATIONS.NONE.NAME;
			}
			if (!instance.IsSubspeciesIdentified(rootSpeciesID, id))
			{
				return CREATURES.PLANT_MUTATIONS.UNIDENTIFIED;
			}
			string text = "";
			for (int i = 0; i < mutations.Count; i++)
			{
				text += mutations[i].Name;
				if (i != mutations.Count - 1)
				{
					text += ", ";
				}
			}
			return text;
		}

		[OnDeserialized]
		private void OnDeserialized()
		{
			if (mutations == null)
			{
				mutations = new List<Trait>();
			}
		}
	}

	public static PlantSubSpeciesCatalog instance;

	[Serialize]
	private Dictionary<Tag, int> nextID = new Dictionary<Tag, int>();

	public Dictionary<string, List<PlantSubSpecies>> subspeciesBySpecies = new Dictionary<string, List<PlantSubSpecies>>();

	[Serialize]
	public Dictionary<string, List<int>> identifiedSubspecies = new Dictionary<string, List<int>>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
		CollectSubSpecies();
	}

	private void CollectSubSpecies()
	{
		foreach (GameObject item in Assets.GetPrefabsWithComponent<MutantPlant>())
		{
			string key = EntityToSpeciesID(item);
			if (!subspeciesBySpecies.ContainsKey(key))
			{
				subspeciesBySpecies.Add(key, new List<PlantSubSpecies>());
			}
			if (!identifiedSubspecies.ContainsKey(key))
			{
				identifiedSubspecies.Add(key, new List<int>());
			}
		}
		foreach (KeyValuePair<string, List<PlantSubSpecies>> subspeciesBySpecy in subspeciesBySpecies)
		{
			if (subspeciesBySpecy.Value.Count == 0)
			{
				PlantSubSpecies plantSubSpecies = new PlantSubSpecies(NextSubSpeciesID(subspeciesBySpecy.Key), subspeciesBySpecy.Key);
				subspeciesBySpecies[subspeciesBySpecy.Key].Add(plantSubSpecies);
				if (!identifiedSubspecies[subspeciesBySpecy.Key].Contains(plantSubSpecies.id))
				{
					identifiedSubspecies[subspeciesBySpecy.Key].Add(plantSubSpecies.id);
				}
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public string EntityToSpeciesID(GameObject entity)
	{
		PlantableSeed component = entity.GetComponent<PlantableSeed>();
		if (component != null)
		{
			return component.PlantID.ToString();
		}
		return entity.PrefabID().ToString();
	}

	public PlantSubSpecies GetSubSpecies(string speciesID, int subSpeciesID)
	{
		foreach (PlantSubSpecies item in subspeciesBySpecies[speciesID])
		{
			if (item.id == subSpeciesID)
			{
				return item;
			}
		}
		return null;
	}

	public PlantSubSpecies GetSubSpecies(Tag tag)
	{
		foreach (KeyValuePair<string, List<PlantSubSpecies>> subspeciesBySpecy in subspeciesBySpecies)
		{
			foreach (PlantSubSpecies item in subspeciesBySpecy.Value)
			{
				if (item.GetFilterTag() == tag)
				{
					return item;
				}
			}
		}
		return null;
	}

	public void IdentifySubSpecies(string speciesID, int subSpeciesID)
	{
		if (identifiedSubspecies[speciesID].Contains(subSpeciesID))
		{
			return;
		}
		identifiedSubspecies[speciesID].Add(subSpeciesID);
		Tag filterTag = GetSubSpecies(speciesID, subSpeciesID).GetFilterTag();
		foreach (MutantPlant mutantPlant in Components.MutantPlants)
		{
			if (mutantPlant.HasTag(filterTag) && mutantPlant.gameObject.HasTag(GameTags.UnidentifiedSeed))
			{
				mutantPlant.gameObject.RemoveTag(GameTags.UnidentifiedSeed);
			}
		}
	}

	public int MutateSpecies(PlantSubSpecies fromSubSpecies, bool irrigated, bool fertilized)
	{
		Trait plantMutation = PlantMutations.GetPlantMutation(fromSubSpecies, irrigated, fertilized);
		foreach (PlantSubSpecies item in subspeciesBySpecies[fromSubSpecies.rootSpeciesID])
		{
			bool flag = true;
			foreach (Trait mutation in fromSubSpecies.mutations)
			{
				if (!item.mutations.Contains(mutation))
				{
					flag = false;
				}
			}
			if (flag && item.mutations.Contains(plantMutation))
			{
				return item.id;
			}
		}
		PlantSubSpecies plantSubSpecies = new PlantSubSpecies(NextSubSpeciesID(fromSubSpecies.rootSpeciesID), fromSubSpecies.rootSpeciesID);
		foreach (Trait mutation2 in fromSubSpecies.mutations)
		{
			plantSubSpecies.mutations.Add(mutation2);
		}
		plantSubSpecies.mutations.Add(plantMutation);
		subspeciesBySpecies[fromSubSpecies.rootSpeciesID].Add(plantSubSpecies);
		return plantSubSpecies.id;
	}

	public bool IsSubspeciesIdentified(string speciesID, int ID)
	{
		PlantSubSpecies subSpecies = GetSubSpecies(speciesID, ID);
		if (subSpecies == null)
		{
			return false;
		}
		return identifiedSubspecies[speciesID].Contains(ID);
	}

	private int NextSubSpeciesID(string speciesID)
	{
		if (!nextID.ContainsKey(speciesID))
		{
			nextID.Add(speciesID.ToTag(), 0);
		}
		return nextID[speciesID]++;
	}
}
