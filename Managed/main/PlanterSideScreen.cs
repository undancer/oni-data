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
		string text = "";
		if (plantablePlot.requestedEntityTag != Tag.Invalid && plantablePlot.requestedEntityTag != GameTags.Empty)
		{
			text = plantablePlot.requestedEntityTag.ToString();
		}
		else if (selectedEntityToggle != null)
		{
			text = depositObjectMap[selectedEntityToggle].tag.ToString();
		}
		if (!string.IsNullOrEmpty(text))
		{
			text = Assets.GetPrefab(text).GetComponent<PlantableSeed>().PlantID.ToString();
			if (plantablePlot.requestedEntityAdditionalFilterTag != Tag.Invalid && plantablePlot.requestedEntityAdditionalFilterTag != GameTags.Empty)
			{
				selectedSubspecies = plantablePlot.requestedEntityAdditionalFilterTag;
			}
			else if (selectedSubspecies == Tag.Invalid)
			{
				selectedSubspecies = PlantSubSpeciesCatalog.instance.subspeciesBySpecies[text][0].GetFilterTag();
				plantablePlot.requestedEntityAdditionalFilterTag = selectedSubspecies;
			}
			selectedSubspecies = PlantSubSpeciesCatalog.instance.subspeciesBySpecies[text][0].GetFilterTag();
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
		if (!(selectedEntityToggle != null))
		{
			return;
		}
		if (selectedDepositObjectAdditionalTag == Tag.Invalid)
		{
			requestSelectedEntityBtn.isInteractable = false;
		}
		else if (selectedDepositObjectTag != Tag.Invalid)
		{
			Tag plantID = Assets.GetPrefab(selectedDepositObjectTag).GetComponent<PlantableSeed>().PlantID;
			PlantSubSpeciesCatalog.PlantSubSpecies plantSubSpecies = PlantSubSpeciesCatalog.instance.subspeciesBySpecies[plantID.ToString()].Find((PlantSubSpeciesCatalog.PlantSubSpecies match) => match.GetFilterTag() == selectedDepositObjectAdditionalTag);
			if (plantSubSpecies == null || !PlantSubSpeciesCatalog.instance.IsSubspeciesIdentified(plantID.ToString(), plantSubSpecies.id))
			{
				requestSelectedEntityBtn.isInteractable = false;
			}
		}
	}

	private IEnumerator ExpandMutations()
	{
		float elasped = 0f;
		float duration = 0.33f;
		LayoutElement le = mutationViewport.GetComponent<LayoutElement>();
		le.minHeight = 24f;
		le.preferredHeight = 24f;
		while (elasped < duration)
		{
			elasped += Time.unscaledDeltaTime;
			le.minHeight = Mathf.Min(118f, 24f + 94f * (elasped / duration));
			le.preferredHeight = Mathf.Min(118f, 24f + 94f * (elasped / duration));
			yield return new WaitForEndOfFrame();
		}
		le.minHeight = 118f;
		le.preferredHeight = 118f;
		mutationPanelCollapsed = false;
		activeAnimationRoutine = null;
		yield return 0;
	}

	private IEnumerator CollapseMutations()
	{
		float elasped = 0f;
		float duration = 0f;
		LayoutElement le = mutationViewport.GetComponent<LayoutElement>();
		while (elasped < duration)
		{
			elasped += Time.unscaledDeltaTime;
			le.minHeight = Mathf.Max(24f, 140f - 140f * (elasped / duration));
			le.preferredHeight = Mathf.Max(24f, 140f - 140f * (elasped / duration));
			yield return new WaitForEndOfFrame();
		}
		le.minHeight = 24f;
		le.preferredHeight = 24f;
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
		foreach (GameObject blankMutationObject in blankMutationObjects)
		{
			UnityEngine.Object.Destroy(blankMutationObject);
		}
		blankMutationObjects.Clear();
		selectSpeciesPrompt.SetActive(value: false);
		if (selectedDepositObjectTag.IsValid && selectedDepositObjectTag != Tag.Invalid)
		{
			string key = Assets.GetPrefab(selectedDepositObjectTag).GetComponent<PlantableSeed>().PlantID.ToString();
			List<PlantSubSpeciesCatalog.PlantSubSpecies> list = PlantSubSpeciesCatalog.instance.subspeciesBySpecies[key];
			foreach (PlantSubSpeciesCatalog.PlantSubSpecies item in list)
			{
				GameObject option = Util.KInstantiateUI(mutationOption, mutationContainer, force_active: true);
				option.GetComponentInChildren<LocText>().text = item.ProperName();
				MultiToggle component = option.GetComponent<MultiToggle>();
				component.onClick = (System.Action)Delegate.Combine(component.onClick, (System.Action)delegate
				{
					MutationToggleClicked(option);
				});
				subspeciesToggles.Add(option, item.GetFilterTag());
			}
			for (int i = list.Count; i < 5; i++)
			{
				blankMutationObjects.Add(Util.KInstantiateUI(blankMutationOption, mutationContainer, force_active: true));
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
			bool flag3 = PlantSubSpeciesCatalog.instance.IsSubspeciesIdentified(PlantSubSpeciesCatalog.instance.GetSubSpecies(subspeciesToggle2.Value).rootSpeciesID, PlantSubSpeciesCatalog.instance.GetSubSpecies(subspeciesToggle2.Value).id);
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
		if (!mutationPanelCollapsed && !flag && !flag2 && selectedEntityToggle == null)
		{
			if (activeAnimationRoutine != null)
			{
				StopCoroutine(activeAnimationRoutine);
			}
			activeAnimationRoutine = StartCoroutine(CollapseMutations());
		}
		else if (!mutationPanelCollapsed && (flag || flag2))
		{
			if (activeAnimationRoutine != null)
			{
				StopCoroutine(activeAnimationRoutine);
			}
			activeAnimationRoutine = StartCoroutine(CollapseMutations());
		}
		else if (mutationPanelCollapsed && !flag && !flag2 && selectedEntityToggle != null)
		{
			if (activeAnimationRoutine != null)
			{
				StopCoroutine(activeAnimationRoutine);
			}
			activeAnimationRoutine = StartCoroutine(ExpandMutations());
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
		if (selectedDepositObjectAdditionalTag != Tag.Invalid)
		{
			flag = PlantSubSpeciesCatalog.instance.IsSubspeciesIdentified(PlantSubSpeciesCatalog.instance.EntityToSpeciesID(seed_or_plant), PlantSubSpeciesCatalog.instance.GetSubSpecies(selectedDepositObjectAdditionalTag).id);
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
			if (!string.IsNullOrEmpty(component.domesticatedDescription))
			{
				text += component.domesticatedDescription;
			}
		}
		else
		{
			InfoDescription component2 = gameObject.GetComponent<InfoDescription>();
			if ((bool)component2)
			{
				text += component2.description;
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
		mutationPanel.SetActive(value: false);
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
				tag2 = PlantSubSpeciesCatalog.instance.GetSubSpecies(component.GetSpeciesID(), component.subspeciesID).GetFilterTag();
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
