using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandModuleSideScreen : SideScreenContent
{
	private LaunchConditionManager target;

	public GameObject conditionListContainer;

	public GameObject prefabConditionLineItem;

	public MultiToggle destinationButton;

	public MultiToggle destinationButtonExpansion;

	public MultiToggle debugVictoryButton;

	[Tooltip("This list is indexed by the ProcessCondition.Status enum")]
	public List<Color> statusColors;

	private Dictionary<ProcessCondition, GameObject> conditionTable = new Dictionary<ProcessCondition, GameObject>();

	private SchedulerHandle updateHandle;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ScheduleUpdate();
		MultiToggle multiToggle = debugVictoryButton;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			SpaceDestination destination = SpacecraftManager.instance.destinations.Find((SpaceDestination match) => match.GetDestinationType() == Db.Get().SpaceDestinationTypes.Wormhole);
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.Clothe8Dupes.Id);
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.Build4NatureReserves.Id);
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.ReachedSpace.Id);
			target.Launch(destination);
		});
		debugVictoryButton.gameObject.SetActive(DebugHandler.InstantBuildMode && CheckHydrogenRocket());
	}

	private bool CheckHydrogenRocket()
	{
		RocketModule rocketModule = target.rocketModules.Find((RocketModule match) => match.GetComponent<RocketEngine>());
		if (rocketModule != null)
		{
			return rocketModule.GetComponent<RocketEngine>().fuelTag == ElementLoader.FindElementByHash(SimHashes.LiquidHydrogen).tag;
		}
		return false;
	}

	private void ScheduleUpdate()
	{
		updateHandle = UIScheduler.Instance.Schedule("RefreshCommandModuleSideScreen", 1f, delegate
		{
			RefreshConditions();
			ScheduleUpdate();
		});
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LaunchConditionManager>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		target = new_target.GetComponent<LaunchConditionManager>();
		if (target == null)
		{
			Debug.LogError("The gameObject received does not contain a LaunchConditionManager component");
			return;
		}
		ClearConditions();
		ConfigureConditions();
		debugVictoryButton.gameObject.SetActive(DebugHandler.InstantBuildMode && CheckHydrogenRocket());
	}

	private void ClearConditions()
	{
		foreach (KeyValuePair<ProcessCondition, GameObject> item in conditionTable)
		{
			Util.KDestroyGameObject(item.Value);
		}
		conditionTable.Clear();
	}

	private void ConfigureConditions()
	{
		foreach (ProcessCondition launchCondition in target.GetLaunchConditionList())
		{
			GameObject value = Util.KInstantiateUI(prefabConditionLineItem, conditionListContainer, force_active: true);
			conditionTable.Add(launchCondition, value);
		}
		RefreshConditions();
	}

	public void RefreshConditions()
	{
		bool flag = false;
		List<ProcessCondition> launchConditionList = target.GetLaunchConditionList();
		foreach (ProcessCondition item in launchConditionList)
		{
			if (conditionTable.ContainsKey(item))
			{
				GameObject gameObject = conditionTable[item];
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				if (item.GetParentCondition() != null && item.GetParentCondition().EvaluateCondition() == ProcessCondition.Status.Failure)
				{
					gameObject.SetActive(value: false);
				}
				else if (!gameObject.activeSelf)
				{
					gameObject.SetActive(value: true);
				}
				ProcessCondition.Status status = item.EvaluateCondition();
				component.GetReference<LocText>("Label").text = item.GetStatusMessage(status);
				component.GetReference<LocText>("Label").color = statusColors[(int)status];
				component.GetReference<Image>("Box").color = statusColors[(int)status];
				component.GetReference<Image>("Check").gameObject.SetActive(status != ProcessCondition.Status.Failure);
				gameObject.GetComponent<ToolTip>().SetSimpleTooltip(item.GetStatusTooltip(status));
				continue;
			}
			flag = true;
			break;
		}
		foreach (KeyValuePair<ProcessCondition, GameObject> item2 in conditionTable)
		{
			if (!launchConditionList.Contains(item2.Key))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			ClearConditions();
			ConfigureConditions();
		}
		destinationButton.onClick = delegate
		{
			ManagementMenu.Instance.ToggleStarmap();
		};
		destinationButtonExpansion.onClick = delegate
		{
			ClusterMapScreen.Instance.Show();
		};
	}

	protected override void OnCleanUp()
	{
		updateHandle.ClearScheduler();
		base.OnCleanUp();
	}
}
