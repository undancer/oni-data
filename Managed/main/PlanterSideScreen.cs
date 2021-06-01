using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class PlanterSideScreen : ReceptacleSideScreen
{
	public DescriptorPanel RequirementsDescriptorPanel;

	public DescriptorPanel HarvestDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;

	public GameObject mutationPanel;

	public GameObject mutationViewport;

	public GameObject mutationContainer;

	public GameObject mutationOption;

	public GameObject blankMutationOption;

	public GameObject selectSpeciesPrompt;

	private bool mutationPanelCollapsed = true;

	public Dictionary<GameObject, Tag> subspeciesToggles = new Dictionary<GameObject, Tag>();

	private List<GameObject> blankMutationObjects = new List<GameObject>();

	private Dictionary<PlantablePlot, Tag> entityPreviousSubSelectionMap = new Dictionary<PlantablePlot, Tag>();

	private Coroutine activeAnimationRoutine = null;

	private const float EXPAND_DURATION = 0.33f;

	private const float EXPAND_MIN = 24f;

	private const float EXPAND_MAX = 118f;

	private Tag selectedSubspecies
	{
		get
		{
			if (!entityPreviousSubSelectionMap.ContainsKey((PlantablePlot)targetReceptacle))
			{
				entityPreviousSubSelectionMap.Add((PlantablePlot)targetReceptacle, Tag.Invalid);
			}
			return entityPreviousSubSelectionMap[(PlantablePlot)targetReceptacle];
		}
		set
		{
			if (!entityPreviousSubSelectionMap.ContainsKey((PlantablePlot)targetReceptacle))
			{
				entityPreviousSubSelectionMap.Add((PlantablePlot)targetReceptacle, Tag.Invalid);
			}
			entityPreviousSubSelectionMap[(PlantablePlot)targetReceptacle] = value;
			selectedDepositObjectAdditionalTag = value;
		}
	}

	private void LoadTargetSubSpeciesRequest()
	{
		PlantablePlot plantablePlot = (PlantablePlot)targetReceptacle;
		Tag tag = Tag.Invalid;
		if (plantablePlot.requestedEntityTag != Tag.Invalid && plantablePlot.requestedEntityTag != GameTags.Empty)
		{
			tag = plantablePlot.requestedEntityTag;
		}
		else if (selectedEntityToggle != null)
		{
			tag = depositObjectMap[selectedEntityToggle].tag;
		}
		if (!DlcManager.FeaturePlantMutationsEnabled() || !tag.IsValid)
		{
			return;
		}
		tag = Assets.GetPrefab(tag).GetComponent<PlantableSeed>().PlantID;
		if (plantablePlot.requestedEntityAdditionalFilterTag != Tag.Invalid && plantablePlot.requestedEntityAdditionalFilterTag != GameTags.Empty)
		{
			selectedSubspecies = plantablePlot.requestedEntityAdditionalFilterTag;
		}
		else if (selectedSubspecies == Tag.Invalid)
		{
			if (PlantSubSpeciesCatalog.instance.GetOriginalSubSpecies(tag, out var subSpeciesInfo))
			{
				selectedSubspecies = subSpeciesInfo.ID;
			}
			plantablePlot.requestedEntityAdditionalFilterTag = selectedSubspecies;
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<PlantablePlot>() != null;
	}

	protected override void ToggleClicked(ReceptacleToggle toggle)
	{
		base.ToggleClicked(toggle);
		LoadTargetSubSpeciesRequest();
		UpdateState(null);
	}

	protected void MutationToggleClicked(GameObject toggle)
	{
		selectedSubspecies = subspeciesToggles[toggle];
		UpdateState(null);
	}

	protected override void UpdateState(object data)
	{
		base.UpdateState(data);
		requestSelectedEntityBtn.onClick += RefreshSubspeciesToggles;
		RefreshSubspeciesToggles();
		UpdatePlantButtonState();
	}

	private void UpdatePlantButtonState()
	{
		if (selectedEntityToggle != null)
		{
			if (DlcManager.FeaturePlantMutationsEnabled())
			{
				requestSelectedEntityBtn.isInteractable = selectedDepositObjectTag.IsValid && selectedDepositObjectAdditionalTag.IsValid && PlantSubSpeciesCatalog.instance.IsSubSpeciesIdentified(selectedDepositObjectAdditionalTag);
			}
			else
			{
				requestSelectedEntityBtn.isInteractable = selectedDepositObjectTag.IsValid;
			}
		}
	}

	private IEnumerator ExpandMutations()
	{
		LayoutElement le = mutationViewport.GetComponent<LayoutElement>();
		float totalTravel = 94f;
		float travelPerSecond = totalTravel / 0.33f;
		while (le.minHeight < 118f)
		{
			float pos2 = le.minHeight;
			float delta = Time.unscaledDeltaTime * travelPerSecond;
			pos2 = (le.preferredHeight = (le.minHeight = Mathf.Min(pos2 + delta, 118f)));
			yield return new WaitForEndOfFrame();
		}
		mutationPanelCollapsed = false;
		activeAnimationRoutine = null;
		yield return 0;
	}

	private IEnumerator CollapseMutations()
	{
		LayoutElement le = mutationViewport.GetComponent<LayoutElement>();
		float totalTravel = -94f;
		float travelPerSecond = totalTravel / 0.33f;
		while (le.minHeight > 24f)
		{
			float pos2 = le.minHeight;
			float delta = Time.unscaledDeltaTime * travelPerSecond;
			pos2 = (le.preferredHeight = (le.minHeight = Mathf.Max(pos2 + delta, 24f)));
			yield return new WaitForEndOfFrame();
		}
		mutationPanelCollapsed = true;
		activeAnimationRoutine = null;
		yield return 0;
	}

	private void RefreshSubspeciesToggles()
	{
		foreach (KeyValuePair<GameObject, Tag> subspeciesToggle in subspeciesToggles)
		{
			UnityEngine.Object.Destroy(subspeciesToggle.Key);
		}
		subspeciesToggles.Clear();
		if (!PlantSubSpeciesCatalog.instance.AnyNonOriginalDiscovered)
		{
			mutationPanel.SetActive(value: false);
			return;
		}
		mutationPanel.SetActive(value: true);
		foreach (GameObject blankMutationObject in blankMutationObjects)
		{
			UnityEngine.Object.Destroy(blankMutationObject);
		}
		blankMutationObjects.Clear();
		selectSpeciesPrompt.SetActive(value: false);
		if (selectedDepositObjectTag.IsValid)
		{
			Tag plantID = Assets.GetPrefab(selectedDepositObjectTag).GetComponent<PlantableSeed>().PlantID;
			List<PlantSubSpeciesCatalog.SubSpeciesInfo> allSubSpeciesForSpecies = PlantSubSpeciesCatalog.instance.GetAllSubSpeciesForSpecies(plantID);
			if (allSubSpeciesForSpecies != null)
			{
				foreach (PlantSubSpeciesCatalog.SubSpeciesInfo item in allSubSpeciesForSpecies)
				{
					GameObject option = Util.KInstantiateUI(mutationOption, mutationContainer, force_active: true);
					option.GetComponentInChildren<LocText>().text = item.GetNameWithMutations(plantID.ProperName(), PlantSubSpeciesCatalog.instance.IsSubSpeciesIdentified(item.ID), cleanOriginal: false);
					MultiToggle component = option.GetComponent<MultiToggle>();
					component.onClick = (System.Action)Delegate.Combine(component.onClick, (System.Action)delegate
					{
						MutationToggleClicked(option);
					});
					option.GetComponent<ToolTip>().SetSimpleTooltip(item.GetMutationsTooltip());
					subspeciesToggles.Add(option, item.ID);
				}
				for (int i = allSubSpeciesForSpecies.Count; i < 5; i++)
				{
					blankMutationObjects.Add(Util.KInstantiateUI(blankMutationOption, mutationContainer, force_active: true));
				}
				if (!selectedSubspecies.IsValid || !subspeciesToggles.ContainsValue(selectedSubspecies))
				{
					selectedSubspecies = allSubSpeciesForSpecies[0].ID;
				}
			}
		}
		else
		{
			selectSpeciesPrompt.SetActive(value: true);
		}
		ICollection<Pickupable> collection = new List<Pickupable>();
		bool flag = CheckReceptacleOccupied();
		bool flag2 = targetReceptacle.GetActiveRequest != null;
		WorldContainer myWorld = targetReceptacle.GetMyWorld();
		collection = myWorld.worldInventory.GetPickupables(selectedDepositObjectTag, myWorld.IsModuleInterior);
		foreach (KeyValuePair<GameObject, Tag> subspeciesToggle2 in subspeciesToggles)
		{
			float num = 0f;
			bool flag3 = PlantSubSpeciesCatalog.instance.IsSubSpeciesIdentified(subspeciesToggle2.Value);
			if (collection != null)
			{
				foreach (Pickupable item2 in collection)
				{
					if (item2.HasTag(subspeciesToggle2.Value))
					{
						num += item2.GetComponent<PrimaryElement>().Units;
					}
				}
			}
			if (num > 0f && flag3)
			{
				subspeciesToggle2.Key.GetComponent<MultiToggle>().ChangeState((subspeciesToggle2.Value == selectedSubspecies) ? 1 : 0);
			}
			else
			{
				subspeciesToggle2.Key.GetComponent<MultiToggle>().ChangeState((subspeciesToggle2.Value == selectedSubspecies) ? 3 : 2);
			}
			subspeciesToggle2.Key.GetComponentInChildren<LocText>().text += $" ({num})";
			if (flag || flag2)
			{
				if (subspeciesToggle2.Value == selectedSubspecies)
				{
					subspeciesToggle2.Key.SetActive(value: true);
					subspeciesToggle2.Key.GetComponent<MultiToggle>().ChangeState(1);
				}
				else
				{
					subspeciesToggle2.Key.SetActive(value: false);
				}
			}
			else
			{
				subspeciesToggle2.Key.SetActive(selectedEntityToggle != null);
			}
		}
		bool flag4 = !flag && !flag2 && selectedEntityToggle != null && subspeciesToggles.Count >= 1;
		if (flag4 && mutationPanelCollapsed)
		{
			if (activeAnimationRoutine != null)
			{
				StopCoroutine(activeAnimationRoutine);
			}
			activeAnimationRoutine = StartCoroutine(ExpandMutations());
		}
		else if (!flag4 && !mutationPanelCollapsed)
		{
			if (activeAnimationRoutine != null)
			{
				StopCoroutine(activeAnimationRoutine);
			}
			activeAnimationRoutine = StartCoroutine(CollapseMutations());
		}
	}

	protected override Sprite GetEntityIcon(Tag prefabTag)
	{
		GameObject prefab = Assets.GetPrefab(prefabTag);
		PlantableSeed component = prefab.GetComponent<PlantableSeed>();
		if (component != null)
		{
			return base.GetEntityIcon(new Tag(component.PlantID));
		}
		return base.GetEntityIcon(prefabTag);
	}

	protected override void SetResultDescriptions(GameObject seed_or_plant)
	{
		string text = "";
		GameObject gameObject = seed_or_plant;
		PlantableSeed component = seed_or_plant.GetComponent<PlantableSeed>();
		List<Descriptor> list = new List<Descriptor>();
		bool flag = true;
		if (seed_or_plant.GetComponent<MutantPlant>() != null && selectedDepositObjectAdditionalTag != Tag.Invalid)
		{
			flag = PlantSubSpeciesCatalog.instance.IsSubSpeciesIdentified(selectedDepositObjectAdditionalTag);
		}
		if (!flag)
		{
			text = string.Concat(CREATURES.PLANT_MUTATIONS.UNIDENTIFIED, "\n\n", CREATURES.PLANT_MUTATIONS.UNIDENTIFIED_DESC);
		}
		else if (component != null)
		{
			list = component.GetDescriptors(component.gameObject);
			if (targetReceptacle.rotatable != null && targetReceptacle.Direction != component.direction)
			{
				if (component.direction == SingleEntityReceptacle.ReceptacleDirection.Top)
				{
					text += UI.UISIDESCREENS.PLANTERSIDESCREEN.ROTATION_NEED_FLOOR;
				}
				else if (component.direction == SingleEntityReceptacle.ReceptacleDirection.Side)
				{
					text += UI.UISIDESCREENS.PLANTERSIDESCREEN.ROTATION_NEED_WALL;
				}
				else if (component.direction == SingleEntityReceptacle.ReceptacleDirection.Bottom)
				{
					text += UI.UISIDESCREENS.PLANTERSIDESCREEN.ROTATION_NEED_CEILING;
				}
				text += "\n\n";
			}
			gameObject = Assets.GetPrefab(component.PlantID);
			MutantPlant component2 = gameObject.GetComponent<MutantPlant>();
			if (component2 != null && selectedDepositObjectAdditionalTag.IsValid)
			{
				component2.DummySetSubspecies(PlantSubSpeciesCatalog.instance.GetSubSpecies(component.PlantID, selectedDepositObjectAdditionalTag).mutationIDs);
			}
			if (!string.IsNullOrEmpty(component.domesticatedDescription))
			{
				text += component.domesticatedDescription;
			}
		}
		else
		{
			InfoDescription component3 = gameObject.GetComponent<InfoDescription>();
			if ((bool)component3)
			{
				text += component3.description;
			}
		}
		descriptionLabel.SetText(text);
		List<Descriptor> plantLifeCycleDescriptors = GameUtil.GetPlantLifeCycleDescriptors(gameObject);
		if (plantLifeCycleDescriptors.Count > 0 && flag)
		{
			HarvestDescriptorPanel.SetDescriptors(plantLifeCycleDescriptors);
			HarvestDescriptorPanel.gameObject.SetActive(value: true);
		}
		else
		{
			HarvestDescriptorPanel.gameObject.SetActive(value: false);
		}
		List<Descriptor> plantRequirementDescriptors = GameUtil.GetPlantRequirementDescriptors(gameObject);
		if (list.Count > 0)
		{
			GameUtil.IndentListOfDescriptors(list);
			plantRequirementDescriptors.InsertRange(plantRequirementDescriptors.Count, list);
		}
		if (plantRequirementDescriptors.Count > 0 && flag)
		{
			RequirementsDescriptorPanel.SetDescriptors(plantRequirementDescriptors);
			RequirementsDescriptorPanel.gameObject.SetActive(value: true);
		}
		else
		{
			RequirementsDescriptorPanel.gameObject.SetActive(value: false);
		}
		List<Descriptor> plantEffectDescriptors = GameUtil.GetPlantEffectDescriptors(gameObject);
		if (plantEffectDescriptors.Count > 0 && flag)
		{
			EffectsDescriptorPanel.SetDescriptors(plantEffectDescriptors);
			EffectsDescriptorPanel.gameObject.SetActive(value: true);
		}
		else
		{
			EffectsDescriptorPanel.gameObject.SetActive(value: false);
		}
	}

	protected override bool AdditionalCanDepositTest()
	{
		PlantablePlot plantablePlot = targetReceptacle as PlantablePlot;
		WorldContainer myWorld = targetReceptacle.GetMyWorld();
		return plantablePlot.ValidPlant && selectedDepositObjectAdditionalTag.IsValid && myWorld.worldInventory.GetCountWithAdditionalTag(selectedDepositObjectTag, selectedDepositObjectAdditionalTag, myWorld.IsModuleInterior) > 0;
	}

	public override void SetTarget(GameObject target)
	{
		selectedDepositObjectTag = Tag.Invalid;
		base.SetTarget(target);
		LoadTargetSubSpeciesRequest();
		RefreshSubspeciesToggles();
		UpdatePlantButtonState();
	}

	protected override void RestoreSelectionFromOccupant()
	{
		base.RestoreSelectionFromOccupant();
		PlantablePlot plantablePlot = (PlantablePlot)targetReceptacle;
		Tag tag = Tag.Invalid;
		Tag tag2 = Tag.Invalid;
		bool flag = false;
		if (plantablePlot.Occupant != null)
		{
			tag = plantablePlot.Occupant.GetComponent<SeedProducer>().seedInfo.seedId;
			MutantPlant component = plantablePlot.Occupant.GetComponent<MutantPlant>();
			if (component != null)
			{
				tag2 = component.SubSpeciesID;
			}
		}
		else if (plantablePlot.GetActiveRequest != null)
		{
			tag = plantablePlot.requestedEntityTag;
			tag2 = plantablePlot.requestedEntityAdditionalFilterTag;
			selectedDepositObjectTag = tag;
			selectedDepositObjectAdditionalTag = tag2;
			flag = true;
		}
		if (!(tag != Tag.Invalid))
		{
			return;
		}
		if (!entityPreviousSelectionMap.ContainsKey(plantablePlot) || flag)
		{
			int value = 0;
			foreach (KeyValuePair<ReceptacleToggle, SelectableEntity> item in depositObjectMap)
			{
				if (item.Value.tag == tag)
				{
					value = entityToggles.IndexOf(item.Key);
				}
			}
			if (!entityPreviousSelectionMap.ContainsKey(plantablePlot))
			{
				entityPreviousSelectionMap.Add(plantablePlot, -1);
			}
			entityPreviousSelectionMap[plantablePlot] = value;
		}
		if (!entityPreviousSubSelectionMap.ContainsKey(plantablePlot))
		{
			entityPreviousSubSelectionMap.Add(plantablePlot, Tag.Invalid);
		}
		if (entityPreviousSubSelectionMap[plantablePlot] == Tag.Invalid || flag)
		{
			entityPreviousSubSelectionMap[plantablePlot] = tag2;
		}
	}
}