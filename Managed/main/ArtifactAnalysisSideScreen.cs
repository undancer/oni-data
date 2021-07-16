using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactAnalysisSideScreen : SideScreenContent
{
	[SerializeField]
	private GameObject rowPrefab;

	private GameObject targetArtifactStation;

	[SerializeField]
	private GameObject rowContainer;

	private Dictionary<string, GameObject> rows = new Dictionary<string, GameObject>();

	private GameObject undiscoveredRow;

	public override string GetTitle()
	{
		if (targetArtifactStation != null)
		{
			return string.Format(base.GetTitle(), targetArtifactStation.GetProperName());
		}
		return base.GetTitle();
	}

	public override void ClearTarget()
	{
		targetArtifactStation = null;
		base.ClearTarget();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetSMI<ArtifactAnalysisStation.StatesInstance>() != null;
	}

	private void RefreshRows()
	{
		if (undiscoveredRow == null)
		{
			undiscoveredRow = Util.KInstantiateUI(rowPrefab, rowContainer, force_active: true);
			HierarchyReferences component = undiscoveredRow.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("label").SetText(UI.UISIDESCREENS.ARTIFACTANALYSISSIDESCREEN.NO_ARTIFACTS_DISCOVERED);
			component.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.ARTIFACTANALYSISSIDESCREEN.NO_ARTIFACTS_DISCOVERED_TOOLTIP);
			component.GetReference<Image>("icon").sprite = Assets.GetSprite("unknown");
			component.GetReference<Image>("icon").color = Color.grey;
		}
		List<string> analyzedArtifactIDs = ArtifactSelector.Instance.GetAnalyzedArtifactIDs();
		undiscoveredRow.SetActive(analyzedArtifactIDs.Count == 0);
		foreach (string item in analyzedArtifactIDs)
		{
			if (!rows.ContainsKey(item))
			{
				GameObject gameObject = Util.KInstantiateUI(rowPrefab, rowContainer, force_active: true);
				rows.Add(item, gameObject);
				GameObject artifactPrefab = Assets.GetPrefab(item);
				HierarchyReferences component2 = gameObject.GetComponent<HierarchyReferences>();
				component2.GetReference<LocText>("label").SetText(artifactPrefab.GetProperName());
				component2.GetReference<Image>("icon").sprite = Def.GetUISprite(artifactPrefab, item).first;
				component2.GetComponent<KButton>().onClick += delegate
				{
					OpenEvent(artifactPrefab);
				};
			}
		}
	}

	private void OpenEvent(GameObject artifactPrefab)
	{
		SimpleEvent.StatesInstance statesInstance = GameplayEventManager.Instance.StartNewEvent(Db.Get().GameplayEvents.ArtifactReveal).smi as SimpleEvent.StatesInstance;
		statesInstance.artifact = artifactPrefab;
		InfoDescription component = artifactPrefab.GetComponent<InfoDescription>();
		if (component != null)
		{
			statesInstance.SetTextParameter("desc", component.description);
		}
		statesInstance.ShowEventPopup();
	}

	public override void SetTarget(GameObject target)
	{
		targetArtifactStation = target;
		base.SetTarget(target);
		RefreshRows();
	}
}
