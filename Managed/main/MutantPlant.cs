using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MutantPlant : KMonoBehaviour
{
	[Serialize]
	private int _subspeciesID = -1;

	public int subspeciesID
	{
		get
		{
			return _subspeciesID;
		}
		private set
		{
			_subspeciesID = value;
		}
	}

	public string GetSpeciesID()
	{
		return PlantSubSpeciesCatalog.instance.EntityToSpeciesID(base.gameObject);
	}

	private void CheckTags()
	{
		if (_subspeciesID == -1)
		{
			_subspeciesID = PlantSubSpeciesCatalog.instance.subspeciesBySpecies[PlantSubSpeciesCatalog.instance.EntityToSpeciesID(base.gameObject)][0].id;
			base.gameObject.AddTag(PlantSubSpeciesCatalog.instance.GetSubSpecies(GetSpeciesID(), subspeciesID).GetFilterTag());
		}
		else if (!base.gameObject.HasTag(PlantSubSpeciesCatalog.instance.GetSubSpecies(GetSpeciesID(), subspeciesID).GetFilterTag()))
		{
			base.gameObject.AddTag(PlantSubSpeciesCatalog.instance.GetSubSpecies(GetSpeciesID(), subspeciesID).GetFilterTag());
		}
	}

	protected override void OnSpawn()
	{
		if (_subspeciesID > 0)
		{
			_subspeciesID = 0;
		}
		CheckTags();
		Components.MutantPlants.Add(this);
		base.OnSpawn();
		Traits component = GetComponent<Traits>();
		PlantSubSpeciesCatalog.PlantSubSpecies subSpecies = PlantSubSpeciesCatalog.instance.GetSubSpecies(GetSpeciesID(), subspeciesID);
		if (subSpecies != null && component != null)
		{
			foreach (Trait mutation in subSpecies.mutations)
			{
				component.Add(mutation);
			}
		}
		SetNameBasedOnSpecies();
	}

	protected override void OnCleanUp()
	{
		Components.MutantPlants.Remove(this);
		base.OnCleanUp();
	}

	public void Mutate()
	{
		PlantSubSpeciesCatalog.PlantSubSpecies subSpecies = PlantSubSpeciesCatalog.instance.GetSubSpecies(GetSpeciesID(), subspeciesID);
		GameObject prefab = Assets.GetPrefab(GetComponent<PlantableSeed>().PlantID);
		StateMachineController component = prefab.GetComponent<StateMachineController>();
		SetSubSpecies(PlantSubSpeciesCatalog.instance.MutateSpecies(subSpecies, component.GetDef<IrrigationMonitor.Def>() != null, component.GetDef<FertilizationMonitor.Def>() != null));
	}

	public void SetSubSpecies(int id)
	{
		if (subspeciesID != -1)
		{
			Tag filterTag = PlantSubSpeciesCatalog.instance.GetSubSpecies(GetSpeciesID(), subspeciesID).GetFilterTag();
			if (base.gameObject.HasTag(filterTag))
			{
				base.gameObject.RemoveTag(filterTag);
			}
		}
		subspeciesID = id;
		base.gameObject.AddTag(PlantSubSpeciesCatalog.instance.GetSubSpecies(GetSpeciesID(), subspeciesID).GetFilterTag());
		if (!PlantSubSpeciesCatalog.instance.IsSubspeciesIdentified(PlantSubSpeciesCatalog.instance.EntityToSpeciesID(base.gameObject), id))
		{
			base.gameObject.AddTag(GameTags.UnidentifiedSeed);
		}
		SetNameBasedOnSpecies();
	}

	private void SetNameBasedOnSpecies()
	{
		string properName = Assets.GetPrefab(GetComponent<KPrefabID>().PrefabID()).GetProperName();
		GetComponent<KSelectable>().SetName(properName);
	}
}
