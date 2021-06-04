using System.Collections.Generic;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpacePOISimpleInfoPanel : SimpleInfoPanel
{
	private Dictionary<Tag, GameObject> elementRows = new Dictionary<Tag, GameObject>();

	private Dictionary<Clustercraft, GameObject> rocketRows = new Dictionary<Clustercraft, GameObject>();

	private GameObject massHeader;

	private GameObject rocketsSpacer;

	private GameObject rocketsHeader;

	private GameObject artifactsSpacer;

	private GameObject artifactRow;

	public SpacePOISimpleInfoPanel(SimpleInfoScreen simpleInfoScreen)
		: base(simpleInfoScreen)
	{
	}

	public override void Refresh(CollapsibleDetailContentPanel spacePOIPanel, GameObject selectedTarget)
	{
		spacePOIPanel.SetTitle(UI.CLUSTERMAP.POI.TITLE);
		if (selectedTarget == null)
		{
			spacePOIPanel.gameObject.SetActive(value: false);
			return;
		}
		HarvestablePOIClusterGridEntity harvestablePOIClusterGridEntity = ((selectedTarget == null) ? null : selectedTarget.GetComponent<HarvestablePOIClusterGridEntity>());
		Clustercraft clusterCraft = selectedTarget.GetComponent<Clustercraft>();
		ArtifactPOIConfigurator component = selectedTarget.GetComponent<ArtifactPOIConfigurator>();
		if (harvestablePOIClusterGridEntity == null && clusterCraft == null && component == null)
		{
			spacePOIPanel.gameObject.SetActive(value: false);
			return;
		}
		if (harvestablePOIClusterGridEntity == null && component == null && clusterCraft != null)
		{
			RocketModuleCluster rocketModuleCluster = null;
			CraftModuleInterface craftModuleInterface = null;
			RocketSimpleInfoPanel.GetRocketStuffFromTarget(selectedTarget, ref rocketModuleCluster, ref clusterCraft, ref craftModuleInterface);
			if (clusterCraft != null)
			{
				foreach (ClusterGridEntity item in ClusterGrid.Instance.GetEntitiesOnCell(clusterCraft.GetMyWorldLocation()))
				{
					HarvestablePOIClusterGridEntity harvestablePOIClusterGridEntity2 = item as HarvestablePOIClusterGridEntity;
					if (harvestablePOIClusterGridEntity2 != null)
					{
						harvestablePOIClusterGridEntity = harvestablePOIClusterGridEntity2;
						component = harvestablePOIClusterGridEntity2.GetComponent<ArtifactPOIConfigurator>();
						break;
					}
				}
			}
		}
		bool flag = harvestablePOIClusterGridEntity != null || component != null;
		spacePOIPanel.gameObject.SetActive(flag);
		if (flag)
		{
			HarvestablePOIStates.Instance harvestable = ((harvestablePOIClusterGridEntity == null) ? null : harvestablePOIClusterGridEntity.GetSMI<HarvestablePOIStates.Instance>());
			RefreshMassHeader(harvestable, selectedTarget, spacePOIPanel);
			RefreshElements(harvestable, selectedTarget, spacePOIPanel);
			RefreshArtifacts(component, selectedTarget, spacePOIPanel);
		}
	}

	private void RefreshMassHeader(HarvestablePOIStates.Instance harvestable, GameObject selectedTarget, CollapsibleDetailContentPanel spacePOIPanel)
	{
		if (massHeader == null)
		{
			massHeader = Util.KInstantiateUI(simpleInfoRoot.iconLabelRow, spacePOIPanel.Content.gameObject, force_active: true);
		}
		massHeader.SetActive(harvestable != null);
		if (harvestable != null)
		{
			HierarchyReferences component = massHeader.GetComponent<HierarchyReferences>();
			Sprite sprite = Assets.GetSprite("icon_asteroid_type");
			if (sprite != null)
			{
				component.GetReference<Image>("Icon").sprite = sprite;
			}
			component.GetReference<LocText>("NameLabel").text = UI.CLUSTERMAP.POI.MASS_REMAINING;
			component.GetReference<LocText>("ValueLabel").text = GameUtil.GetFormattedMass(harvestable.poiCapacity);
			component.GetReference<LocText>("ValueLabel").alignment = TextAlignmentOptions.MidlineRight;
		}
	}

	private void RefreshElements(HarvestablePOIStates.Instance harvestable, GameObject selectedTarget, CollapsibleDetailContentPanel spacePOIPanel)
	{
		foreach (KeyValuePair<Tag, GameObject> elementRow in elementRows)
		{
			if (elementRow.Value != null)
			{
				elementRow.Value.SetActive(value: false);
			}
		}
		if (harvestable == null)
		{
			return;
		}
		Dictionary<SimHashes, float> elementsWithWeights = harvestable.configuration.GetElementsWithWeights();
		float num = 0f;
		List<KeyValuePair<SimHashes, float>> list = new List<KeyValuePair<SimHashes, float>>();
		foreach (KeyValuePair<SimHashes, float> item in elementsWithWeights)
		{
			num += item.Value;
			list.Add(item);
		}
		list.Sort((KeyValuePair<SimHashes, float> a, KeyValuePair<SimHashes, float> b) => b.Value.CompareTo(a.Value));
		foreach (KeyValuePair<SimHashes, float> item2 in list)
		{
			SimHashes key = item2.Key;
			Tag tag = key.CreateTag();
			if (!elementRows.ContainsKey(key.CreateTag()))
			{
				elementRows.Add(tag, Util.KInstantiateUI(simpleInfoRoot.iconLabelRow, spacePOIPanel.Content.gameObject, force_active: true));
			}
			elementRows[tag].SetActive(value: true);
			HierarchyReferences component = elementRows[tag].GetComponent<HierarchyReferences>();
			Tuple<Sprite, Color> uISprite = Def.GetUISprite(tag);
			component.GetReference<Image>("Icon").sprite = uISprite.first;
			component.GetReference<Image>("Icon").color = uISprite.second;
			component.GetReference<LocText>("NameLabel").text = ElementLoader.GetElement(tag).name;
			component.GetReference<LocText>("ValueLabel").text = GameUtil.GetFormattedPercent(item2.Value / num * 100f);
			component.GetReference<LocText>("ValueLabel").alignment = TextAlignmentOptions.MidlineRight;
		}
	}

	private void RefreshRocketsAtThisLocation(HarvestablePOIStates.Instance harvestable, GameObject selectedTarget, CollapsibleDetailContentPanel spacePOIPanel)
	{
		if (rocketsHeader == null)
		{
			rocketsSpacer = Util.KInstantiateUI(simpleInfoRoot.spacerRow, spacePOIPanel.Content.gameObject, force_active: true);
			rocketsHeader = Util.KInstantiateUI(simpleInfoRoot.iconLabelRow, spacePOIPanel.Content.gameObject, force_active: true);
			HierarchyReferences component = rocketsHeader.GetComponent<HierarchyReferences>();
			Sprite sprite = Assets.GetSprite("ic_rocket");
			if (sprite != null)
			{
				component.GetReference<Image>("Icon").sprite = sprite;
				component.GetReference<Image>("Icon").color = Color.black;
			}
			component.GetReference<LocText>("NameLabel").text = UI.CLUSTERMAP.POI.ROCKETS_AT_THIS_LOCATION;
			component.GetReference<LocText>("ValueLabel").text = "";
		}
		rocketsSpacer.rectTransform().SetAsLastSibling();
		rocketsHeader.rectTransform().SetAsLastSibling();
		foreach (KeyValuePair<Clustercraft, GameObject> rocketRow in rocketRows)
		{
			rocketRow.Value.SetActive(value: false);
		}
		bool flag = true;
		for (int i = 0; i < Components.Clustercrafts.Count; i++)
		{
			Clustercraft clustercraft = Components.Clustercrafts[i];
			if (!rocketRows.ContainsKey(clustercraft))
			{
				GameObject value = Util.KInstantiateUI(simpleInfoRoot.iconLabelRow, spacePOIPanel.Content.gameObject, force_active: true);
				rocketRows.Add(clustercraft, value);
			}
			bool flag2 = clustercraft.Location == selectedTarget.GetComponent<KMonoBehaviour>().GetMyWorldLocation();
			flag = flag && !flag2;
			rocketRows[clustercraft].SetActive(flag2);
			if (flag2)
			{
				HierarchyReferences component2 = rocketRows[clustercraft].GetComponent<HierarchyReferences>();
				component2.GetReference<Image>("Icon").sprite = clustercraft.GetUISprite();
				component2.GetReference<Image>("Icon").color = Color.grey;
				component2.GetReference<LocText>("NameLabel").text = clustercraft.Name;
				component2.GetReference<LocText>("ValueLabel").text = "";
				component2.GetReference<LocText>("ValueLabel").alignment = TextAlignmentOptions.MidlineRight;
				rocketRows[clustercraft].rectTransform().SetAsLastSibling();
			}
		}
		rocketsHeader.SetActive(!flag);
		rocketsSpacer.SetActive(rocketsHeader.activeSelf);
	}

	private void RefreshArtifacts(ArtifactPOIConfigurator artifactConfigurator, GameObject selectedTarget, CollapsibleDetailContentPanel spacePOIPanel)
	{
		if (artifactsSpacer == null)
		{
			artifactsSpacer = Util.KInstantiateUI(simpleInfoRoot.spacerRow, spacePOIPanel.Content.gameObject, force_active: true);
			artifactRow = Util.KInstantiateUI(simpleInfoRoot.iconLabelRow, spacePOIPanel.Content.gameObject, force_active: true);
		}
		artifactsSpacer.rectTransform().SetAsLastSibling();
		artifactRow.rectTransform().SetAsLastSibling();
		ArtifactPOIStates.Instance sMI = artifactConfigurator.GetSMI<ArtifactPOIStates.Instance>();
		string artifactID = sMI.configuration.GetArtifactID();
		HierarchyReferences component = artifactRow.GetComponent<HierarchyReferences>();
		component.GetReference<LocText>("NameLabel").text = UI.CLUSTERMAP.POI.ARTIFACTS;
		component.GetReference<LocText>("ValueLabel").alignment = TextAlignmentOptions.MidlineRight;
		component.GetReference<Image>("Icon").sprite = Assets.GetSprite("ic_artifacts");
		component.GetReference<Image>("Icon").color = Color.black;
		if (sMI.CanHarvestArtifact())
		{
			component.GetReference<LocText>("ValueLabel").text = UI.CLUSTERMAP.POI.ARTIFACTS_AVAILABLE;
		}
		else
		{
			component.GetReference<LocText>("ValueLabel").text = UI.CLUSTERMAP.POI.ARTIFACTS_DEPLETED;
		}
	}
}
