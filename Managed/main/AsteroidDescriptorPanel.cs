using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/AsteroidDescriptorPanel")]
public class AsteroidDescriptorPanel : KMonoBehaviour
{
	[Header("Destination Details")]
	[SerializeField]
	private GameObject customLabelPrefab;

	[SerializeField]
	private GameObject prefabTraitWidget;

	[SerializeField]
	private GameObject prefabTraitCategoryWidget;

	[SerializeField]
	private GameObject prefabParameterWidget;

	[SerializeField]
	private GameObject startingAsteroidRowContainer;

	[SerializeField]
	private GameObject nearbyAsteroidRowContainer;

	[SerializeField]
	private GameObject distantAsteroidRowContainer;

	[SerializeField]
	private LocText clusterNameLabel;

	[SerializeField]
	private LocText clusterDifficultyLabel;

	[SerializeField]
	public LocText headerLabel;

	[SerializeField]
	public MultiToggle clusterDetailsButton;

	private List<GameObject> labels = new List<GameObject>();

	[Header("Selected Asteroid Details")]
	[SerializeField]
	private GameObject SpacedOutContentContainer;

	public Image selectedAsteroidIcon;

	public LocText selectedAsteroidLabel;

	public LocText selectedAsteroidDescription;

	[SerializeField]
	private GameObject prefabAsteroidLine;

	private Dictionary<ProcGen.World, GameObject> asteroidLines = new Dictionary<ProcGen.World, GameObject>();

	private List<GameObject> traitWidgets = new List<GameObject>();

	private List<GameObject> traitCategoryWidgets = new List<GameObject>();

	private List<GameObject> parameterWidgets = new List<GameObject>();

	public bool HasDescriptors()
	{
		return labels.Count > 0;
	}

	public void EnableClusterDetails(bool setActive)
	{
		clusterNameLabel.gameObject.SetActive(setActive);
		clusterDifficultyLabel.gameObject.SetActive(setActive);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public void SetClusterDetailLabels(ColonyDestinationAsteroidBeltData cluster)
	{
		Strings.TryGet(cluster.properName, out var result);
		clusterNameLabel.SetText((result == null) ? "" : string.Format(WORLDS.SURVIVAL_CHANCE.CLUSTERNAME, result.String));
		int index = Mathf.Clamp(cluster.difficulty, 0, ColonyDestinationAsteroidBeltData.survivalOptions.Count - 1);
		Tuple<string, string, string> tuple = ColonyDestinationAsteroidBeltData.survivalOptions[index];
		string text = string.Format(WORLDS.SURVIVAL_CHANCE.TITLE, tuple.first, tuple.third);
		text = text.Trim('\n');
		clusterDifficultyLabel.SetText(text);
	}

	public void SetParameterDescriptors(IList<AsteroidDescriptor> descriptors)
	{
		for (int i = 0; i < parameterWidgets.Count; i++)
		{
			UnityEngine.Object.Destroy(parameterWidgets[i]);
		}
		parameterWidgets.Clear();
		for (int j = 0; j < descriptors.Count; j++)
		{
			GameObject gameObject = Util.KInstantiateUI(prefabParameterWidget, base.gameObject, force_active: true);
			gameObject.GetComponent<LocText>().SetText(descriptors[j].text);
			ToolTip component = gameObject.GetComponent<ToolTip>();
			if (!string.IsNullOrEmpty(descriptors[j].tooltip))
			{
				component.SetSimpleTooltip(descriptors[j].tooltip);
			}
			parameterWidgets.Add(gameObject);
		}
	}

	private void ClearTraitDescriptors()
	{
		for (int i = 0; i < traitWidgets.Count; i++)
		{
			UnityEngine.Object.Destroy(traitWidgets[i]);
		}
		traitWidgets.Clear();
		for (int j = 0; j < traitCategoryWidgets.Count; j++)
		{
			UnityEngine.Object.Destroy(traitCategoryWidgets[j]);
		}
		traitCategoryWidgets.Clear();
	}

	public void SetTraitDescriptors(IList<AsteroidDescriptor> descriptors, bool includeDescriptions = true)
	{
		SetTraitDescriptors(new List<IList<AsteroidDescriptor>> { descriptors }, includeDescriptions);
	}

	public void SetTraitDescriptors(List<IList<AsteroidDescriptor>> descriptorSets, bool includeDescriptions = true, List<Tuple<string, Sprite>> headerData = null)
	{
		ClearTraitDescriptors();
		for (int i = 0; i < descriptorSets.Count; i++)
		{
			IList<AsteroidDescriptor> list = descriptorSets[i];
			GameObject parent = base.gameObject;
			if (descriptorSets.Count > 1)
			{
				Debug.Assert(headerData != null, "Asteroid Header data is null - traits wont have their world as contex in the selection UI");
				GameObject gameObject = Util.KInstantiate(prefabTraitCategoryWidget, base.gameObject);
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				gameObject.transform.localScale = Vector3.one;
				Strings.TryGet(headerData[i].first, out var result);
				component.GetReference<LocText>("NameLabel").SetText(result.String);
				component.GetReference<Image>("Icon").sprite = headerData[i].second;
				gameObject.SetActive(value: true);
				parent = component.GetReference<RectTransform>("Contents").gameObject;
				traitCategoryWidgets.Add(gameObject);
			}
			for (int j = 0; j < list.Count; j++)
			{
				GameObject gameObject2 = Util.KInstantiate(prefabTraitWidget, parent);
				HierarchyReferences component2 = gameObject2.GetComponent<HierarchyReferences>();
				gameObject2.SetActive(value: true);
				component2.GetReference<LocText>("NameLabel").SetText("<b>" + list[j].text + "</b>");
				Image reference = component2.GetReference<Image>("Icon");
				reference.color = list[j].associatedColor;
				if (list[j].associatedIcon != null)
				{
					Sprite sprite = Assets.GetSprite(list[j].associatedIcon);
					if (sprite != null)
					{
						reference.sprite = sprite;
					}
				}
				if (gameObject2.GetComponent<ToolTip>() != null)
				{
					gameObject2.GetComponent<ToolTip>().SetSimpleTooltip(list[j].tooltip);
				}
				LocText reference2 = component2.GetReference<LocText>("DescLabel");
				if (includeDescriptions && !string.IsNullOrEmpty(list[j].tooltip))
				{
					reference2.SetText(list[j].tooltip);
				}
				else
				{
					reference2.gameObject.SetActive(value: false);
				}
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject2.SetActive(value: true);
				traitWidgets.Add(gameObject2);
			}
		}
	}

	public void EnableClusterLocationLabels(bool enable)
	{
		startingAsteroidRowContainer.transform.parent.gameObject.SetActive(enable);
		nearbyAsteroidRowContainer.transform.parent.gameObject.SetActive(enable);
		distantAsteroidRowContainer.transform.parent.gameObject.SetActive(enable);
	}

	public void RefreshAsteroidLines(ColonyDestinationAsteroidBeltData cluster, AsteroidDescriptorPanel selectedAsteroidDetailsPanel)
	{
		foreach (KeyValuePair<ProcGen.World, GameObject> asteroidLine in asteroidLines)
		{
			if (!asteroidLine.Value.IsNullOrDestroyed())
			{
				UnityEngine.Object.Destroy(asteroidLine.Value);
			}
		}
		asteroidLines.Clear();
		SpawnAsteroidLine(cluster.GetStartWorld, startingAsteroidRowContainer, cluster);
		for (int i = 0; i < cluster.worlds.Count; i++)
		{
			ProcGen.World world = cluster.worlds[i];
			WorldPlacement worldPlacement = null;
			for (int j = 0; j < cluster.Layout.worldPlacements.Count; j++)
			{
				if (cluster.Layout.worldPlacements[j].world == world.filePath)
				{
					worldPlacement = cluster.Layout.worldPlacements[j];
					break;
				}
			}
			SpawnAsteroidLine(world, (worldPlacement.locationType == WorldPlacement.LocationType.InnerCluster) ? nearbyAsteroidRowContainer : distantAsteroidRowContainer, cluster);
		}
		foreach (KeyValuePair<ProcGen.World, GameObject> line in asteroidLines)
		{
			MultiToggle component = line.Value.GetComponent<MultiToggle>();
			component.onClick = (System.Action)Delegate.Combine(component.onClick, (System.Action)delegate
			{
				SelectAsteroidInCluster(line.Key, cluster, selectedAsteroidDetailsPanel);
			});
		}
		SelectWholeClusterDetails(cluster, selectedAsteroidDetailsPanel);
	}

	private void SelectAsteroidInCluster(ProcGen.World asteroid, ColonyDestinationAsteroidBeltData cluster, AsteroidDescriptorPanel selectedAsteroidDetailsPanel)
	{
		selectedAsteroidDetailsPanel.SpacedOutContentContainer.SetActive(value: true);
		clusterDetailsButton.GetComponent<MultiToggle>().ChangeState(0);
		foreach (KeyValuePair<ProcGen.World, GameObject> asteroidLine in asteroidLines)
		{
			asteroidLine.Value.GetComponent<MultiToggle>().ChangeState((asteroidLine.Key == asteroid) ? 1 : 0);
			if (asteroidLine.Key == asteroid)
			{
				SetSelectedAsteroid(asteroidLine.Key, selectedAsteroidDetailsPanel, cluster.GenerateTraitDescriptors(asteroidLine.Key));
			}
		}
	}

	public void SelectWholeClusterDetails(ColonyDestinationAsteroidBeltData cluster, AsteroidDescriptorPanel selectedAsteroidDetailsPanel)
	{
		selectedAsteroidDetailsPanel.SpacedOutContentContainer.SetActive(value: false);
		foreach (KeyValuePair<ProcGen.World, GameObject> asteroidLine in asteroidLines)
		{
			asteroidLine.Value.GetComponent<MultiToggle>().ChangeState(0);
		}
		SetSelectedCluster(cluster, selectedAsteroidDetailsPanel);
		clusterDetailsButton.GetComponent<MultiToggle>().ChangeState(1);
	}

	private void SpawnAsteroidLine(ProcGen.World asteroid, GameObject parentContainer, ColonyDestinationAsteroidBeltData cluster)
	{
		if (asteroidLines.ContainsKey(asteroid))
		{
			return;
		}
		GameObject gameObject = Util.KInstantiateUI(prefabAsteroidLine, parentContainer.gameObject, force_active: true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		Image reference = component.GetReference<Image>("Icon");
		LocText reference2 = component.GetReference<LocText>("Label");
		RectTransform reference3 = component.GetReference<RectTransform>("TraitsRow");
		LocText reference4 = component.GetReference<LocText>("TraitLabel");
		ToolTip component2 = gameObject.GetComponent<ToolTip>();
		Sprite sprite = (reference.sprite = ColonyDestinationAsteroidBeltData.GetUISprite(asteroid.asteroidIcon));
		Strings.TryGet(asteroid.name, out var result);
		reference2.SetText(result.String);
		List<WorldTrait> worldTraits = cluster.GetWorldTraits(asteroid);
		reference4.gameObject.SetActive(worldTraits.Count == 0);
		reference4.SetText(UI.FRONTEND.COLONYDESTINATIONSCREEN.NO_TRAITS);
		RectTransform reference5 = component.GetReference<RectTransform>("TraitIconPrefab");
		foreach (WorldTrait item in worldTraits)
		{
			Image component3 = Util.KInstantiateUI(reference5.gameObject, reference3.gameObject, force_active: true).GetComponent<Image>();
			Sprite sprite2 = Assets.GetSprite(item.filePath.Substring(item.filePath.LastIndexOf("/") + 1));
			if (sprite2 != null)
			{
				component3.sprite = sprite2;
			}
			component3.color = Util.ColorFromHex(item.colorHex);
		}
		string text = "";
		if (worldTraits.Count > 0)
		{
			for (int i = 0; i < worldTraits.Count; i++)
			{
				Strings.TryGet(worldTraits[i].name, out var result2);
				Strings.TryGet(worldTraits[i].description, out var result3);
				text = text + "<color=#" + worldTraits[i].colorHex + ">" + result2.String + "</color>\n" + result3.String;
				if (i != worldTraits.Count - 1)
				{
					text += "\n\n";
				}
			}
		}
		else
		{
			text = UI.FRONTEND.COLONYDESTINATIONSCREEN.NO_TRAITS;
		}
		component2.SetSimpleTooltip(text);
		asteroidLines.Add(asteroid, gameObject);
	}

	private void SetSelectedAsteroid(ProcGen.World asteroid, AsteroidDescriptorPanel detailPanel, List<AsteroidDescriptor> traitDescriptors)
	{
		detailPanel.SetTraitDescriptors(traitDescriptors);
		detailPanel.selectedAsteroidIcon.sprite = ColonyDestinationAsteroidBeltData.GetUISprite(asteroid.asteroidIcon);
		detailPanel.selectedAsteroidIcon.gameObject.SetActive(value: true);
		Strings.TryGet(asteroid.name, out var result);
		detailPanel.selectedAsteroidLabel.SetText(result.String);
		Strings.TryGet(asteroid.description, out var result2);
		detailPanel.selectedAsteroidDescription.SetText(result2.String);
	}

	private void SetSelectedCluster(ColonyDestinationAsteroidBeltData cluster, AsteroidDescriptorPanel detailPanel)
	{
		List<IList<AsteroidDescriptor>> list = new List<IList<AsteroidDescriptor>>();
		List<Tuple<string, Sprite>> list2 = new List<Tuple<string, Sprite>>();
		List<AsteroidDescriptor> list3 = cluster.GenerateTraitDescriptors(cluster.GetStartWorld, includeDefaultTrait: false);
		if (list3.Count != 0)
		{
			list2.Add(new Tuple<string, Sprite>(cluster.GetStartWorld.name, ColonyDestinationAsteroidBeltData.GetUISprite(cluster.GetStartWorld.asteroidIcon)));
			list.Add(list3);
		}
		foreach (ProcGen.World world in cluster.worlds)
		{
			List<AsteroidDescriptor> list4 = cluster.GenerateTraitDescriptors(world, includeDefaultTrait: false);
			if (list4.Count != 0)
			{
				list2.Add(new Tuple<string, Sprite>(world.name, ColonyDestinationAsteroidBeltData.GetUISprite(world.asteroidIcon)));
				list.Add(list4);
			}
		}
		detailPanel.SetTraitDescriptors(list, includeDescriptions: false, list2);
		detailPanel.selectedAsteroidIcon.gameObject.SetActive(value: false);
		Strings.TryGet(cluster.properName, out var result);
		detailPanel.selectedAsteroidLabel.SetText(result.String);
		detailPanel.selectedAsteroidDescription.SetText("");
	}
}
