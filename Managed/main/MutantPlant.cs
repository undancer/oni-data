using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MutantPlant : KMonoBehaviour, IGameObjectEffectDescriptor
{
	[Serialize]
	private List<string> mutationIDs;

	private List<Guid> statusItemHandles = new List<Guid>();

	private const int MAX_MUTATIONS = 1;

	private Tag cachedSpeciesID;

	private Tag cachedSubspeciesID;

	public List<string> MutationIDs => mutationIDs;

	public Tag SpeciesID
	{
		get
		{
			if (cachedSpeciesID == null)
			{
				PlantableSeed component = GetComponent<PlantableSeed>();
				if (component != null)
				{
					cachedSpeciesID = component.PlantID;
				}
				else
				{
					cachedSpeciesID = this.PrefabID();
				}
			}
			return cachedSpeciesID;
		}
	}

	public Tag SubSpeciesID
	{
		get
		{
			if (cachedSubspeciesID == null)
			{
				cachedSubspeciesID = GetSubSpeciesInfo().ID;
			}
			return cachedSubspeciesID;
		}
	}

	protected override void OnSpawn()
	{
		this.AddTag(SubSpeciesID);
		Components.MutantPlants.Add(this);
		base.OnSpawn();
		ApplyMutations();
		UpdateNameAndTags();
		PlantSubSpeciesCatalog.instance.DiscoverSubSpecies(GetSubSpeciesInfo(), this);
	}

	protected override void OnCleanUp()
	{
		Components.MutantPlants.Remove(this);
		base.OnCleanUp();
	}

	public void Mutate()
	{
		List<string> list = ((mutationIDs != null) ? new List<string>(mutationIDs) : new List<string>());
		while (list.Count >= 1 && list.Count > 0)
		{
			list.RemoveAt(UnityEngine.Random.Range(0, list.Count));
		}
		list.Add(Db.Get().PlantMutations.GetRandomMutation(this.PrefabID().Name).Id);
		SetSubSpecies(list);
	}

	public void ApplyMutations()
	{
		if (mutationIDs == null || mutationIDs.Count == 0)
		{
			return;
		}
		foreach (string mutationID in mutationIDs)
		{
			PlantMutation plantMutation = Db.Get().PlantMutations.Get(mutationID);
			plantMutation.ApplyTo(this);
		}
	}

	public void DummySetSubspecies(List<string> mutations)
	{
		mutationIDs = mutations;
	}

	public void SetSubSpecies(List<string> mutations)
	{
		if (base.gameObject.HasTag(SubSpeciesID))
		{
			base.gameObject.RemoveTag(SubSpeciesID);
		}
		cachedSubspeciesID = Tag.Invalid;
		mutationIDs = mutations;
		UpdateNameAndTags();
	}

	public PlantSubSpeciesCatalog.SubSpeciesInfo GetSubSpeciesInfo()
	{
		return new PlantSubSpeciesCatalog.SubSpeciesInfo(SpeciesID, mutationIDs);
	}

	public void CopyMutationsTo(MutantPlant target)
	{
		target.SetSubSpecies(mutationIDs);
	}

	public void UpdateNameAndTags()
	{
		bool flag = !IsInitialized() || PlantSubSpeciesCatalog.instance.IsSubSpeciesIdentified(SubSpeciesID);
		bool flag2 = PlantSubSpeciesCatalog.instance == null || PlantSubSpeciesCatalog.instance.GetAllSubSpeciesForSpecies(SpeciesID).Count == 1;
		KPrefabID component = GetComponent<KPrefabID>();
		component.AddTag(SubSpeciesID);
		component.SetTag(GameTags.UnidentifiedSeed, flag);
		GetComponent<KSelectable>().SetName(GetSubSpeciesInfo().GetNameWithMutations(component.PrefabTag.ProperName(), flag, flag2));
		KSelectable component2 = GetComponent<KSelectable>();
		foreach (Guid statusItemHandle in statusItemHandles)
		{
			component2.RemoveStatusItem(statusItemHandle);
		}
		statusItemHandles.Clear();
		if (flag2)
		{
			return;
		}
		if (mutationIDs == null || mutationIDs.Count == 0)
		{
			statusItemHandles.Add(component2.AddStatusItem(Db.Get().CreatureStatusItems.OriginalPlantMutation));
			return;
		}
		if (!PlantSubSpeciesCatalog.instance.IsSubSpeciesIdentified(SubSpeciesID))
		{
			statusItemHandles.Add(component2.AddStatusItem(Db.Get().CreatureStatusItems.UnknownMutation));
			return;
		}
		foreach (string mutationID in mutationIDs)
		{
			statusItemHandles.Add(component2.AddStatusItem(Db.Get().CreatureStatusItems.SpecificPlantMutation, Db.Get().PlantMutations.Get(mutationID)));
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		if (mutationIDs == null || mutationIDs.Count == 0)
		{
			return null;
		}
		List<Descriptor> descriptors = new List<Descriptor>();
		foreach (string mutationID in mutationIDs)
		{
			PlantMutation plantMutation = Db.Get().PlantMutations.Get(mutationID);
			plantMutation.GetDescriptors(ref descriptors, go);
		}
		return descriptors;
	}
}
