using System.Collections.Generic;
using Klei.AI;
using UnityEngine;
using UnityEngine.UI;

public class SubSpeciesInfoScreen : KModalScreen
{
	[SerializeField]
	private KButton renameButton;

	[SerializeField]
	private KButton saveButton;

	[SerializeField]
	private KButton discardButton;

	[SerializeField]
	private RectTransform mutationsList;

	[SerializeField]
	private Image plantIcon;

	[SerializeField]
	private GameObject mutationsItemPrefab;

	private List<GameObject> mutationLineItems = new List<GameObject>();

	private GeneticAnalysisStation targetStation;

	public override bool IsModal()
	{
		return true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	private void ClearMutations()
	{
		for (int num = mutationLineItems.Count - 1; num >= 0; num--)
		{
			Util.KDestroyGameObject(mutationLineItems[num]);
		}
		mutationLineItems.Clear();
	}

	public void DisplayDiscovery(Tag speciesID, Tag subSpeciesID, GeneticAnalysisStation station)
	{
		SetSubspecies(speciesID, subSpeciesID);
		targetStation = station;
	}

	private void SetSubspecies(Tag speciesID, Tag subSpeciesID)
	{
		ClearMutations();
		PlantSubSpeciesCatalog.SubSpeciesInfo subSpecies = PlantSubSpeciesCatalog.Instance.GetSubSpecies(speciesID, subSpeciesID);
		plantIcon.sprite = Def.GetUISprite(Assets.GetPrefab(speciesID)).first;
		foreach (string mutationID in subSpecies.mutationIDs)
		{
			PlantMutation plantMutation = Db.Get().PlantMutations.Get(mutationID);
			GameObject gameObject = Util.KInstantiateUI(mutationsItemPrefab, mutationsList.gameObject, force_active: true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("nameLabel").text = plantMutation.Name;
			component.GetReference<LocText>("descriptionLabel").text = plantMutation.description;
			mutationLineItems.Add(gameObject);
		}
	}
}
