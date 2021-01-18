using System.Collections.Generic;
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
		saveButton.onClick += OnPressSave;
		discardButton.onClick += OnPressDiscard;
	}

	private void ClearMutations()
	{
		for (int num = mutationLineItems.Count - 1; num >= 0; num--)
		{
			Util.KDestroyGameObject(mutationLineItems[num]);
		}
		mutationLineItems.Clear();
	}

	public void DisplayDiscovery(string speciesID, int subSpeciesID, GeneticAnalysisStation station)
	{
		SetSubspecies(speciesID, subSpeciesID);
		targetStation = station;
	}

	private void SetSubspecies(string speciesID, int subSpeciesID)
	{
		ClearMutations();
		PlantSubSpeciesCatalog.PlantSubSpecies subSpecies = PlantSubSpeciesCatalog.instance.GetSubSpecies(speciesID, subSpeciesID);
		plantIcon.sprite = Def.GetUISprite(Assets.GetPrefab(subSpecies.rootSpeciesID)).first;
		for (int i = 0; i < subSpecies.mutations.Count; i++)
		{
			GameObject gameObject = Util.KInstantiateUI(mutationsItemPrefab, mutationsList.gameObject, force_active: true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("nameLabel").text = subSpecies.mutations[i].Name;
			component.GetReference<LocText>("descriptionLabel").text = subSpecies.mutations[i].description;
			mutationLineItems.Add(gameObject);
		}
	}

	private void OnPressSave()
	{
		targetStation.EjectSeed();
		Deactivate();
	}

	private void OnPressDiscard()
	{
		targetStation.DiscardSeed();
		Deactivate();
	}
}
