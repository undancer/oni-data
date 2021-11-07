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

	private List<GameObject> labels = new List<GameObject>();

	[Header("Selected Asteroid Details")]
	private GameObject SpacedOutContentContainer;

	public Image selectedAsteroidIcon;

	public LocText selectedAsteroidLabel;

	public LocText selectedAsteroidDescription;

	[SerializeField]
	private GameObject prefabAsteroidLine;

	private Dictionary<ProcGen.World, GameObject> asteroidLines = new Dictionary<ProcGen.World, GameObject>();

	private List<GameObject> traitWidgets = new List<GameObject>();

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

	public void SetTraitDescriptors(IList<AsteroidDescriptor> descriptors)
	{
		for (int i = 0; i < traitWidgets.Count; i++)
		{
			UnityEngine.Object.Destroy(traitWidgets[i]);
		}
		traitWidgets.Clear();
		for (int j = 0; j < descriptors.Count; j++)
		{
			GameObject gameObject = Util.KInstantiate(prefabTraitWidget, base.gameObject);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("NameLabel").SetText("<b>" + descriptors[j].text + "</b>");
			component.GetReference<Image>("Icon").color = descriptors[j].associatedColor;
			LocText reference = component.GetReference<LocText>("DescLabel");
			if (!string.IsNullOrEmpty(descriptors[j].tooltip))
			{
				reference.SetText(descriptors[j].tooltip);
			}
			else
			{
				reference.gameObject.SetActive(value: false);
			}
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.SetActive(value: true);
			traitWidgets.Add(gameObject);
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
		SelectAsteroidInCluster(cluster.GetStartWorld, cluster, selectedAsteroidDetailsPanel);
	}

	private void SelectAsteroidInCluster(ProcGen.World asteroid, ColonyDestinationAsteroidBeltData cluster, AsteroidDescriptorPanel selectedAsteroidDetailsPanel)
	{
		foreach (KeyValuePair<ProcGen.World, GameObject> asteroidLine in asteroidLines)
		{
			asteroidLine.Value.GetComponent<MultiToggle>().ChangeState((asteroidLine.Key == asteroid) ? 1 : 0);
			if (asteroidLine.Key == asteroid)
			{
				SetSelectedAsteroid(asteroidLine.Key, selectedAsteroidDetailsPanel, cluster.GenerateTraitDescriptors(asteroidLine.Key));
			}
		}
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
			Util.KInstantiateUI(reference5.gameObject, reference3.gameObject, force_active: true).GetComponent<Image>().color = Util.ColorFromHex(item.colorHex);
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
}
