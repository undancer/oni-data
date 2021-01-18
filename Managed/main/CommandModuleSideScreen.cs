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

	public MultiToggle debugVictoryButton;

	private Dictionary<RocketLaunchCondition, GameObject> conditionTable = new Dictionary<RocketLaunchCondition, GameObject>();

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
		foreach (KeyValuePair<RocketLaunchCondition, GameObject> item in conditionTable)
		{
			Util.KDestroyGameObject(item.Value);
		}
		conditionTable.Clear();
	}

	private void ConfigureConditions()
	{
		foreach (RocketLaunchCondition launchCondition in target.GetLaunchConditionList())
		{
			GameObject value = Util.KInstantiateUI(prefabConditionLineItem, conditionListContainer, force_active: true);
			conditionTable.Add(launchCondition, value);
		}
		RefreshConditions();
	}

	public void RefreshConditions()
	{
		bool flag = false;
		List<RocketLaunchCondition> launchConditionList = target.GetLaunchConditionList();
		foreach (RocketLaunchCondition item in launchConditionList)
		{
			if (conditionTable.ContainsKey(item))
			{
				GameObject gameObject = conditionTable[item];
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				if (item.GetParentCondition() != null && item.GetParentCondition().EvaluateLaunchCondition() == RocketLaunchCondition.LaunchStatus.Failure)
				{
					gameObject.SetActive(value: false);
				}
				else if (!gameObject.activeSelf)
				{
					gameObject.SetActive(value: true);
				}
				bool flag2 = item.EvaluateLaunchCondition() != RocketLaunchCondition.LaunchStatus.Failure;
				component.GetReference<LocText>("Label").text = item.GetLaunchStatusMessage(flag2);
				component.GetReference<LocText>("Label").color = (flag2 ? Color.black : Color.red);
				component.GetReference<Image>("Box").color = (flag2 ? Color.black : Color.red);
				component.GetReference<Image>("Check").gameObject.SetActive(flag2);
				gameObject.GetComponent<ToolTip>().SetSimpleTooltip(item.GetLaunchStatusTooltip(flag2));
				continue;
			}
			flag = true;
			break;
		}
		foreach (KeyValuePair<RocketLaunchCondition, GameObject> item2 in conditionTable)
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
	}

	protected override void OnCleanUp()
	{
		updateHandle.ClearScheduler();
		base.OnCleanUp();
	}
}
