using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ClusterLocationFilterSideScreen : SideScreenContent
{
	private LogicClusterLocationSensor sensor;

	[SerializeField]
	private GameObject rowPrefab;

	[SerializeField]
	private GameObject listContainer;

	[SerializeField]
	private LocText headerLabel;

	private Dictionary<AxialI, GameObject> worldRows = new Dictionary<AxialI, GameObject>();

	private GameObject emptySpaceRow;

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LogicClusterLocationSensor>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		sensor = target.GetComponent<LogicClusterLocationSensor>();
		Build();
	}

	private void ClearRows()
	{
		if (emptySpaceRow != null)
		{
			Util.KDestroyGameObject(emptySpaceRow);
		}
		foreach (KeyValuePair<AxialI, GameObject> worldRow in worldRows)
		{
			Util.KDestroyGameObject(worldRow.Value);
		}
		worldRows.Clear();
	}

	private void Build()
	{
		headerLabel.SetText(UI.UISIDESCREENS.CLUSTERLOCATIONFILTERSIDESCREEN.HEADER);
		ClearRows();
		emptySpaceRow = Util.KInstantiateUI(rowPrefab, listContainer);
		emptySpaceRow.SetActive(value: true);
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			if (!worldContainer.IsModuleInterior)
			{
				GameObject gameObject = Util.KInstantiateUI(rowPrefab, listContainer);
				gameObject.gameObject.name = worldContainer.GetProperName();
				AxialI myWorldLocation = worldContainer.GetMyWorldLocation();
				Debug.Assert(!worldRows.ContainsKey(myWorldLocation), "Adding two worlds/POI with the same cluster location to ClusterLocationFilterSideScreen UI: " + worldContainer.GetProperName());
				worldRows.Add(myWorldLocation, gameObject);
			}
		}
		Refresh();
	}

	private void Refresh()
	{
		emptySpaceRow.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(UI.UISIDESCREENS.CLUSTERLOCATIONFILTERSIDESCREEN.EMPTY_SPACE_ROW);
		emptySpaceRow.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = Def.GetUISprite("hex_soft").first;
		emptySpaceRow.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").color = Color.black;
		emptySpaceRow.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = delegate
		{
			sensor.SetSpaceEnabled(!sensor.ActiveInSpace);
			Refresh();
		};
		emptySpaceRow.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState(sensor.ActiveInSpace ? 1 : 0);
		foreach (KeyValuePair<AxialI, GameObject> kvp in worldRows)
		{
			ClusterGridEntity clusterGridEntity = ClusterGrid.Instance.cellContents[kvp.Key][0];
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(clusterGridEntity.GetProperName());
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = Def.GetUISprite(clusterGridEntity).first;
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").color = Def.GetUISprite(clusterGridEntity).second;
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = delegate
			{
				sensor.SetLocationEnabled(kvp.Key, !sensor.CheckLocationSelected(kvp.Key));
				Refresh();
			};
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState(sensor.CheckLocationSelected(kvp.Key) ? 1 : 0);
			kvp.Value.SetActive(ClusterGrid.Instance.GetCellRevealLevel(kvp.Key) == ClusterRevealLevel.Visible);
		}
	}
}
