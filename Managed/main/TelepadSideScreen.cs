using System.Collections.Generic;
using Database;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class TelepadSideScreen : SideScreenContent
{
	[SerializeField]
	private LocText timeLabel;

	[SerializeField]
	private KButton viewImmigrantsBtn;

	[SerializeField]
	private Telepad targetTelepad;

	[SerializeField]
	private KButton viewColonySummaryBtn;

	[SerializeField]
	private Image newAchievementsEarned;

	[SerializeField]
	private KButton openRolesScreenButton;

	[SerializeField]
	private Image skillPointsAvailable;

	[SerializeField]
	private GameObject victoryConditionsContainer;

	[SerializeField]
	private GameObject conditionContainerTemplate;

	[SerializeField]
	private GameObject checkboxLinePrefab;

	private Dictionary<string, Dictionary<ColonyAchievementRequirement, GameObject>> entries = new Dictionary<string, Dictionary<ColonyAchievementRequirement, GameObject>>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		viewImmigrantsBtn.onClick += delegate
		{
			ImmigrantScreen.InitializeImmigrantScreen(targetTelepad);
			Game.Instance.Trigger(288942073);
		};
		viewColonySummaryBtn.onClick += delegate
		{
			newAchievementsEarned.gameObject.SetActive(value: false);
			MainMenu.ActivateRetiredColoniesScreenFromData(PauseScreen.Instance.transform.parent.gameObject, RetireColonyUtility.GetCurrentColonyRetiredColonyData());
		};
		openRolesScreenButton.onClick += delegate
		{
			ManagementMenu.Instance.ToggleSkills();
		};
		BuildVictoryConditions();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Telepad>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		Telepad component = target.GetComponent<Telepad>();
		if (component == null)
		{
			Debug.LogError("Target doesn't have a telepad associated with it.");
			return;
		}
		targetTelepad = component;
		if (targetTelepad != null)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.gameObject.SetActive(value: true);
		}
	}

	private void Update()
	{
		if (!(targetTelepad != null))
		{
			return;
		}
		if (GameFlowManager.Instance != null && GameFlowManager.Instance.IsGameOver())
		{
			base.gameObject.SetActive(value: false);
			timeLabel.text = UI.UISIDESCREENS.TELEPADSIDESCREEN.GAMEOVER;
			SetContentState(isLabel: true);
		}
		else
		{
			if (targetTelepad.GetComponent<Operational>().IsOperational)
			{
				timeLabel.text = string.Format(UI.UISIDESCREENS.TELEPADSIDESCREEN.NEXTPRODUCTION, GameUtil.GetFormattedCycles(targetTelepad.GetTimeRemaining()));
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
			SetContentState(!Immigration.Instance.ImmigrantsAvailable);
		}
		UpdateVictoryConditions();
		UpdateAchievementsUnlocked();
		UpdateSkills();
	}

	private void SetContentState(bool isLabel)
	{
		if (timeLabel.gameObject.activeInHierarchy != isLabel)
		{
			timeLabel.gameObject.SetActive(isLabel);
		}
		if (viewImmigrantsBtn.gameObject.activeInHierarchy == isLabel)
		{
			viewImmigrantsBtn.gameObject.SetActive(!isLabel);
		}
	}

	private void BuildVictoryConditions()
	{
		foreach (ColonyAchievement resource in Db.Get().ColonyAchievements.resources)
		{
			if (!resource.isVictoryCondition)
			{
				continue;
			}
			Dictionary<ColonyAchievementRequirement, GameObject> dictionary = new Dictionary<ColonyAchievementRequirement, GameObject>();
			GameObject gameObject = Util.KInstantiateUI(conditionContainerTemplate, victoryConditionsContainer, force_active: true);
			gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(resource.Name);
			foreach (ColonyAchievementRequirement item in resource.requirementChecklist)
			{
				VictoryColonyAchievementRequirement victoryColonyAchievementRequirement = item as VictoryColonyAchievementRequirement;
				if (victoryColonyAchievementRequirement != null)
				{
					GameObject gameObject2 = Util.KInstantiateUI(checkboxLinePrefab, gameObject, force_active: true);
					gameObject2.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(victoryColonyAchievementRequirement.Name());
					gameObject2.GetComponent<ToolTip>().SetSimpleTooltip(victoryColonyAchievementRequirement.Description());
					dictionary.Add(item, gameObject2);
				}
				else
				{
					Debug.LogWarning($"Colony achievement {item.GetType().ToString()} is not a victory requirement but it is attached to a victory achievement {resource.Name}.");
				}
			}
			entries.Add(resource.Id, dictionary);
		}
	}

	private void UpdateVictoryConditions()
	{
		foreach (ColonyAchievement resource in Db.Get().ColonyAchievements.resources)
		{
			if (!resource.isVictoryCondition)
			{
				continue;
			}
			foreach (ColonyAchievementRequirement item in resource.requirementChecklist)
			{
				entries[resource.Id][item].GetComponent<HierarchyReferences>().GetReference<Image>("Check").enabled = item.Success();
			}
		}
	}

	private void UpdateAchievementsUnlocked()
	{
		if (SaveGame.Instance.GetComponent<ColonyAchievementTracker>().achievementsToDisplay.Count > 0)
		{
			newAchievementsEarned.gameObject.SetActive(value: true);
		}
	}

	private void UpdateSkills()
	{
		bool active = false;
		foreach (MinionResume minionResume in Components.MinionResumes)
		{
			if (!minionResume.HasTag(GameTags.Dead) && minionResume.TotalSkillPointsGained - minionResume.SkillsMastered > 0)
			{
				active = true;
				break;
			}
		}
		skillPointsAvailable.gameObject.SetActive(active);
	}
}
