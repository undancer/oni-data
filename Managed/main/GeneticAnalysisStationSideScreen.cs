using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class GeneticAnalysisStationSideScreen : SideScreenContent
{
	[SerializeField]
	private LocText message;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private LocText buttonLabel;

	[SerializeField]
	private GameObject contents;

	[SerializeField]
	private GameObject rowContainer;

	[SerializeField]
	private HierarchyReferences rowPrefab;

	private List<HierarchyReferences> rows = new List<HierarchyReferences>();

	private GeneticAnalysisStation.StatesInstance target;

	private Tag selectedSpecies;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		button.onClick += OnButtonClick;
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
		selectedSpecies = this.target.GetTargetPlant();
		PlantSubSpeciesCatalog.instance.Subscribe(-1531232972, OnPlantSubspeciesProgress);
		PlantSubSpeciesCatalog.instance.Subscribe(-98362560, OnPlantSubspeciesProgress);
		Refresh();
	}

	public override void ClearTarget()
	{
		if (PlantSubSpeciesCatalog.instance != null)
		{
			PlantSubSpeciesCatalog.instance.Unsubscribe(-1531232972, OnPlantSubspeciesProgress);
			PlantSubSpeciesCatalog.instance.Unsubscribe(-98362560, OnPlantSubspeciesProgress);
		}
	}

	private void OnPlantSubspeciesProgress(object data)
	{
		Refresh();
	}

	private void OnButtonClick()
	{
		if (target != null)
		{
			if (DebugHandler.InstantBuildMode)
			{
				PlantSubSpeciesCatalog.instance.PartiallyIdentifySpecies(selectedSpecies, 1f);
				target.SetTargetPlant(Tag.Invalid);
			}
			else if (target.GetTargetPlant().IsValid)
			{
				target.SetTargetPlant(Tag.Invalid);
			}
			else
			{
				target.SetTargetPlant(selectedSpecies);
			}
			Refresh();
		}
	}

	private void Refresh()
	{
		if (target != null)
		{
			Tag targetPlant = target.GetTargetPlant();
			if (targetPlant.IsValid)
			{
				DrawSelectedPlant(targetPlant);
			}
			else
			{
				DrawPickerMenu();
			}
		}
	}

	private void DrawSelectedPlant(Tag targetPlant)
	{
		button.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.CANCEL_SEED;
		HierarchyReferences item;
		if (0 < rows.Count)
		{
			item = rows[0];
		}
		else
		{
			item = Util.KInstantiateUI<HierarchyReferences>(rowPrefab.gameObject, rowContainer);
			rows.Add(item);
		}
		ConfigureButton(item, targetPlant);
		rows[0].gameObject.SetActive(value: true);
		for (int i = 1; i < rows.Count; i++)
		{
			rows[i].gameObject.SetActive(value: false);
		}
		contents.gameObject.SetActive(value: true);
		message.text = UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.CURRENT_TARGET;
	}

	private void DrawPickerMenu()
	{
		Dictionary<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>> dictionary = new Dictionary<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>>();
		foreach (Tag allDiscoveredSpecy in PlantSubSpeciesCatalog.instance.GetAllDiscoveredSpecies())
		{
			dictionary[allDiscoveredSpecy] = new List<PlantSubSpeciesCatalog.SubSpeciesInfo>(PlantSubSpeciesCatalog.instance.GetAllSubSpeciesForSpecies(allDiscoveredSpecy));
		}
		button.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.SELECT_SEED;
		int num = 0;
		foreach (KeyValuePair<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>> item2 in dictionary)
		{
			List<PlantSubSpeciesCatalog.SubSpeciesInfo> allSubSpeciesForSpecies = PlantSubSpeciesCatalog.instance.GetAllSubSpeciesForSpecies(item2.Key);
			if (allSubSpeciesForSpecies.Count <= 1)
			{
				continue;
			}
			GameObject prefab = Assets.GetPrefab(item2.Key);
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
				HierarchyReferences item;
				if (num < rows.Count)
				{
					item = rows[num];
				}
				else
				{
					item = Util.KInstantiateUI<HierarchyReferences>(rowPrefab.gameObject, rowContainer);
					rows.Add(item);
				}
				ConfigureButton(item, item2.Key);
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
			button.isInteractable = false;
		}
		else
		{
			message.text = UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.PICK_TARGET;
			contents.gameObject.SetActive(value: true);
			button.isInteractable = selectedSpecies.IsValid;
		}
	}

	private void ConfigureButton(HierarchyReferences button, Tag speciesID)
	{
		LocText reference = button.GetReference<LocText>("Label");
		Image reference2 = button.GetReference<Image>("Icon");
		LocText reference3 = button.GetReference<LocText>("ProgressLabel");
		KSlider reference4 = button.GetReference<KSlider>("ProgressBar");
		ToolTip reference5 = button.GetReference<ToolTip>("ToolTip");
		reference.text = speciesID.ProperName();
		reference2.sprite = Def.GetUISprite(speciesID).first;
		if (PlantSubSpeciesCatalog.instance.GetAllUnidentifiedSubSpecies(speciesID).Count > 0)
		{
			float identificationProgress = PlantSubSpeciesCatalog.instance.GetIdentificationProgress(speciesID);
			int num = 20;
			float num2 = Mathf.Floor(identificationProgress * (float)num);
			reference4.value = identificationProgress;
			reference3.text = string.Format(UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.SEED_PROGRESS_FMT, num2, num);
			reference4.gameObject.SetActive(value: true);
		}
		else
		{
			reference3.text = UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.SEED_COMPLETE;
			reference4.gameObject.SetActive(value: false);
		}
		KToggle component = button.GetComponent<KToggle>();
		component.isOn = speciesID == selectedSpecies;
		component.ClearOnClick();
		component.onClick += delegate
		{
			selectedSpecies = speciesID;
			Refresh();
		};
	}
}
