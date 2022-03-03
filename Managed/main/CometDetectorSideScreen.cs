using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CometDetectorSideScreen : SideScreenContent
{
	private CometDetector.Instance detector;

	private ClusterCometDetector.Instance clusterDetector;

	public GameObject rowPrefab;

	public RectTransform rowContainer;

	public Dictionary<object, GameObject> rows = new Dictionary<object, GameObject>();

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			RefreshOptions();
		}
	}

	private void RefreshOptions()
	{
		if (clusterDetector != null)
		{
			int num = 0;
			SetClusterRow(num++, UI.UISIDESCREENS.COMETDETECTORSIDESCREEN.COMETS, Assets.GetSprite("meteors"), ClusterCometDetector.Instance.ClusterCometDetectorState.MeteorShower);
			SetClusterRow(num++, UI.UISIDESCREENS.COMETDETECTORSIDESCREEN.DUPEMADE, Assets.GetSprite("dupe_made_ballistics"), ClusterCometDetector.Instance.ClusterCometDetectorState.BallisticObject);
			foreach (Clustercraft clustercraft in Components.Clustercrafts)
			{
				SetClusterRow(num++, clustercraft.Name, Assets.GetSprite("rocket_landing"), ClusterCometDetector.Instance.ClusterCometDetectorState.Rocket, clustercraft);
			}
			for (int i = num; i < rowContainer.childCount; i++)
			{
				rowContainer.GetChild(i).gameObject.SetActive(value: false);
			}
			return;
		}
		int num2 = 0;
		SetRow(num2++, UI.UISIDESCREENS.COMETDETECTORSIDESCREEN.COMETS, Assets.GetSprite("meteors"), null);
		foreach (Spacecraft item in SpacecraftManager.instance.GetSpacecraft())
		{
			SetRow(num2++, item.GetRocketName(), Assets.GetSprite("rocket_landing"), item.launchConditions);
		}
		for (int j = num2; j < rowContainer.childCount; j++)
		{
			rowContainer.GetChild(j).gameObject.SetActive(value: false);
		}
	}

	private void ClearRows()
	{
		for (int num = rowContainer.childCount - 1; num >= 0; num--)
		{
			Util.KDestroyGameObject(rowContainer.GetChild(num));
		}
		rows.Clear();
	}

	public override void SetTarget(GameObject target)
	{
		if (DlcManager.IsExpansion1Active())
		{
			clusterDetector = target.GetSMI<ClusterCometDetector.Instance>();
		}
		else
		{
			detector = target.GetSMI<CometDetector.Instance>();
		}
		RefreshOptions();
	}

	private void SetClusterRow(int idx, string name, Sprite icon, ClusterCometDetector.Instance.ClusterCometDetectorState state, Clustercraft rocketTarget = null)
	{
		GameObject gameObject = ((idx >= rowContainer.childCount) ? Util.KInstantiateUI(rowPrefab, rowContainer.gameObject, force_active: true) : rowContainer.GetChild(idx).gameObject);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<LocText>("label").text = name;
		component.GetReference<Image>("icon").sprite = icon;
		MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
		component2.ChangeState((clusterDetector.GetDetectorState() == state && clusterDetector.GetClustercraftTarget() == rocketTarget) ? 1 : 0);
		ClusterCometDetector.Instance.ClusterCometDetectorState _state = state;
		Clustercraft _rocketTarget = rocketTarget;
		component2.onClick = delegate
		{
			clusterDetector.SetDetectorState(_state);
			clusterDetector.SetClustercraftTarget(_rocketTarget);
			RefreshOptions();
		};
	}

	private void SetRow(int idx, string name, Sprite icon, LaunchConditionManager target)
	{
		GameObject gameObject = ((idx >= rowContainer.childCount) ? Util.KInstantiateUI(rowPrefab, rowContainer.gameObject, force_active: true) : rowContainer.GetChild(idx).gameObject);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<LocText>("label").text = name;
		component.GetReference<Image>("icon").sprite = icon;
		MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
		component2.ChangeState((detector.GetTargetCraft() == target) ? 1 : 0);
		LaunchConditionManager _target = target;
		component2.onClick = delegate
		{
			detector.SetTargetCraft(_target);
			RefreshOptions();
		};
	}

	public override bool IsValidForTarget(GameObject target)
	{
		if (DlcManager.IsExpansion1Active())
		{
			return target.GetSMI<ClusterCometDetector.Instance>() != null;
		}
		return target.GetSMI<CometDetector.Instance>() != null;
	}
}
