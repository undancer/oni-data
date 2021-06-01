using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MutantPlant : KMonoBehaviour, IGameObjectEffectDescriptor
{
	[Serialize]
	private bool analyzed = false;

	[Serialize]
	private List<string> mutationIDs;

	private List<Guid> statusItemHandles = new List<Guid>();

	private const int MAX_MUTATIONS = 1;

	private Tag cachedSpeciesID;

	private Tag cachedSubspeciesID;

	public List<string> MutationIDs => mutationIDs;

	public bool IsOriginal => mutationIDs == null || mutationIDs.Count == 0;

	public bool IsIdentified => analyzed && PlantSubSpeciesCatalog.Instance.IsSubSpeciesIdentified(SubSpeciesID);

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
		if (IsOriginal || this.HasTag(GameTags.Plant))
		{
			analyzed = true;
		}
		this.AddTag(SubSpeciesID);
		Components.MutantPlants.Add(this);
		base.OnSpawn();
		ApplyMutations();
		UpdateNameAndTags();
		PlantSubSpeciesCatalog.Instance.DiscoverSubSpecies(GetSubSpeciesInfo(), this);
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

	public void Analyze()
	{
		analyzed = true;
		UpdateNameAndTags();
	}

	public void ApplyMutations()
	{
		if (IsOriginal)
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
		target.analyzed = analyzed;
	}

	public void UpdateNameAndTags()
	{
		bool flag = !IsInitialized() || IsIdentified;
		bool flag2 = PlantSubSpeciesCatalog.Instance == null || PlantSubSpeciesCatalog.Instance.GetAllSubSpeciesForSpecies(SpeciesID).Count == 1;
		KPrefabID component = GetComponent<KPrefabID>();
		component.AddTag(SubSpeciesID);
		component.SetTag(GameTags.UnidentifiedSeed, !flag);
		base.gameObject.name = component.PrefabTag.ToString() + " (" + SubSpeciesID.ToString() + ")";
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
		if (IsOriginal)
		{
			statusItemHandles.Add(component2.AddStatusItem(Db.Get().CreatureStatusItems.OriginalPlantMutation));
			return;
		}
		if (!flag)
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
		if (IsOriginal)
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
