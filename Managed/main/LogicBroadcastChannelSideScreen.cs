using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class LogicBroadcastChannelSideScreen : SideScreenContent
{
	private LogicBroadcastReceiver sensor;

	[SerializeField]
	private GameObject rowPrefab;

	[SerializeField]
	private GameObject listContainer;

	[SerializeField]
	private LocText headerLabel;

	private Dictionary<LogicBroadcaster, GameObject> broadcasterRows = new Dictionary<LogicBroadcaster, GameObject>();

	private GameObject emptySpaceRow;

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LogicBroadcastReceiver>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		sensor = target.GetComponent<LogicBroadcastReceiver>();
		Build();
	}

	private void ClearRows()
	{
		if (emptySpaceRow != null)
		{
			Util.KDestroyGameObject(emptySpaceRow);
		}
		foreach (KeyValuePair<LogicBroadcaster, GameObject> broadcasterRow in broadcasterRows)
		{
			Util.KDestroyGameObject(broadcasterRow.Value);
		}
		broadcasterRows.Clear();
	}

	private void Build()
	{
		headerLabel.SetText(UI.UISIDESCREENS.LOGICBROADCASTCHANNELSIDESCREEN.HEADER);
		ClearRows();
		foreach (LogicBroadcaster logicBroadcaster in Components.LogicBroadcasters)
		{
			GameObject gameObject = Util.KInstantiateUI(rowPrefab, listContainer);
			gameObject.gameObject.name = logicBroadcaster.gameObject.GetProperName();
			Debug.Assert(!broadcasterRows.ContainsKey(logicBroadcaster), "Adding two of the same broadcaster to LogicBroadcastChannelSideScreen UI: " + logicBroadcaster.gameObject.GetProperName());
			broadcasterRows.Add(logicBroadcaster, gameObject);
			gameObject.SetActive(value: true);
		}
		Refresh();
	}

	private void Refresh()
	{
		foreach (KeyValuePair<LogicBroadcaster, GameObject> kvp in broadcasterRows)
		{
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(kvp.Key.gameObject.GetProperName());
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("DistanceLabel").SetText(LogicBroadcastReceiver.CheckRange(sensor.gameObject, kvp.Key.gameObject) ? UI.UISIDESCREENS.LOGICBROADCASTCHANNELSIDESCREEN.IN_RANGE : UI.UISIDESCREENS.LOGICBROADCASTCHANNELSIDESCREEN.OUT_OF_RANGE);
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = Def.GetUISprite(kvp.Key.gameObject).first;
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").color = Def.GetUISprite(kvp.Key.gameObject).second;
			WorldContainer myWorld = kvp.Key.GetMyWorld();
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("WorldIcon").sprite = (myWorld.IsModuleInterior ? Assets.GetSprite("icon_category_rocketry") : Def.GetUISprite(myWorld.GetComponent<ClusterGridEntity>()).first);
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("WorldIcon").color = (myWorld.IsModuleInterior ? Color.white : Def.GetUISprite(myWorld.GetComponent<ClusterGridEntity>()).second);
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = delegate
			{
				sensor.SetChannel(kvp.Key);
				Refresh();
			};
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState((sensor.GetChannel() == kvp.Key) ? 1 : 0);
		}
	}
}
