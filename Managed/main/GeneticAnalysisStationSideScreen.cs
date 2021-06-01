using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class GeneticAnalysisStationSideScreen : SideScreenContent
{
	[SerializeField]
	private LocText message;

	[SerializeField]
	private GameObject contents;

	[SerializeField]
	private GameObject rowContainer;

	[SerializeField]
	private HierarchyReferences rowPrefab;

	private List<HierarchyReferences> rows = new List<HierarchyReferences>();

	private GeneticAnalysisStation.StatesInstance target;

	private Dictionary<Tag, bool> expandedSeeds = new Dictionary<Tag, bool>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Refresh();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetSMI<GeneticAnalysisStation.StatesInstance>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		this.target = target.GetSMI<GeneticAnalysisStation.StatesInstance>();
		GeneticAnalysisStationWorkable component = target.GetComponent<GeneticAnalysisStationWorkable>();
		Refresh();
	}

	private void Refresh()
	{
		if (target != null)
		{
			DrawPickerMenu();
		}
	}

	private void DrawPickerMenu()
	{
		Dictionary<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>> dictionary = new Dictionary<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>>();
		foreach (Tag allDiscoveredSpecy in PlantSubSpeciesCatalog.Instance.GetAllDiscoveredSpecies())
		{
			dictionary[allDiscoveredSpecy] = new List<PlantSubSpeciesCatalog.SubSpeciesInfo>(PlantSubSpeciesCatalog.Instance.GetAllSubSpeciesForSpecies(allDiscoveredSpecy));
		}
		int num = 0;
		foreach (KeyValuePair<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>> item in dictionary)
		{
			List<PlantSubSpeciesCatalog.SubSpeciesInfo> allSubSpeciesForSpecies = PlantSubSpeciesCatalog.Instance.GetAllSubSpeciesForSpecies(item.Key);
			if (allSubSpeciesForSpecies.Count <= 1)
			{
				continue;
			}
			GameObject prefab = Assets.GetPrefab(item.Key);
			if (prefab == null)
			{
				continue;
			}
			SeedProducer component = prefab.GetComponent<SeedProducer>();
			if (component == null)
			{
				continue;
			}
			SeedProducer.SeedInfo seedInfo = component.seedInfo;
			Tag tag = seedInfo.seedId.ToTag();
			if (DiscoveredResources.Instance.IsDiscovered(tag))
			{
				HierarchyReferences hierarchyReferences;
				if (num < rows.Count)
				{
					hierarchyReferences = rows[num];
				}
				else
				{
					hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(rowPrefab.gameObject, rowContainer);
					rows.Add(hierarchyReferences);
				}
				ConfigureButton(hierarchyReferences, item.Key);
				rows[num].gameObject.SetActive(value: true);
				num++;
			}
		}
		for (int i = num; i < rows.Count; i++)
		{
			rows[i].gameObject.SetActive(value: false);
		}
		if (num == 0)
		{
			message.text = UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.NONE_DISCOVERED;
			contents.gameObject.SetActive(value: false);
		}
		else
		{
			message.text = UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.SELECT_SEEDS;
			contents.gameObject.SetActive(value: true);
		}
	}

	private void ConfigureButton(HierarchyReferences button, Tag speciesID)
	{
		LocText reference = button.GetReference<LocText>("Label");
		Image reference2 = button.GetReference<Image>("Icon");
		LocText reference3 = button.GetReference<LocText>("ProgressLabel");
		ToolTip reference4 = button.GetReference<ToolTip>("ToolTip");
		Tag seedID = GetSeedIDFromPlantID(speciesID);
		bool isForbidden = target.GetSeedForbidden(seedID);
		reference.text = seedID.ProperName();
		reference2.sprite = Def.GetUISprite(seedID).first;
		List<PlantSubSpeciesCatalog.SubSpeciesInfo> allSubSpeciesForSpecies = PlantSubSpeciesCatalog.Instance.GetAllSubSpeciesForSpecies(speciesID);
		if (allSubSpeciesForSpecies.Count > 0)
		{
			reference3.text = (isForbidden ? UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.SEED_FORBIDDEN : UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.SEED_ALLOWED);
		}
		else
		{
			reference3.text = UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.SEED_NO_MUTANTS;
		}
		KToggle component = button.GetComponent<KToggle>();
		component.isOn = !isForbidden;
		component.ClearOnClick();
		component.onClick += delegate
		{
			target.SetSeedForbidden(seedID, !isForbidden);
			Refresh();
		};
	}

	private Tag GetSeedIDFromPlantID(Tag speciesID)
	{
		GameObject prefab = Assets.GetPrefab(speciesID);
		SeedProducer component = prefab.GetComponent<SeedProducer>();
		string seedId = component.seedInfo.seedId;
		return seedId;
	}
}
