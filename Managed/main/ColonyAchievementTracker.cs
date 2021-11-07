using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Database;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ColonyAchievementTracker")]
public class ColonyAchievementTracker : KMonoBehaviour, ISaveLoadableDetails, IRenderEveryTick
{
	public Dictionary<string, ColonyAchievementStatus> achievements = new Dictionary<string, ColonyAchievementStatus>();

	[Serialize]
	public Dictionary<int, int> fetchAutomatedChoreDeliveries = new Dictionary<int, int>();

	[Serialize]
	public Dictionary<int, int> fetchDupeChoreDeliveries = new Dictionary<int, int>();

	[Serialize]
	public Dictionary<int, List<int>> dupesCompleteChoresInSuits = new Dictionary<int, List<int>>();

	[Serialize]
	public HashSet<Tag> tamedCritterTypes = new HashSet<Tag>();

	[Serialize]
	public bool defrostedDuplicant;

	[Serialize]
	public HashSet<Tag> analyzedSeeds = new HashSet<Tag>();

	[Serialize]
	public float totalMaterialsHarvestFromPOI;

	[Serialize]
	public float radBoltTravelDistance;

	[Serialize]
	public bool harvestAHiveWithoutGettingStung;

	[Serialize]
	public Dictionary<int, int> cyclesRocketDupeMoraleAboveRequirement = new Dictionary<int, int>();

	private SchedulerHandle checkAchievementsHandle;

	private int forceCheckAchievementHandle = -1;

	[Serialize]
	private int updatingAchievement;

	[Serialize]
	private List<string> completedAchievementsToDisplay = new List<string>();

	private SchedulerHandle victorySchedulerHandle;

	public static readonly string UnlockedAchievementKey = "UnlockedAchievement";

	private Dictionary<string, object> unlockedAchievementMetric = new Dictionary<string, object> { { UnlockedAchievementKey, null } };

	private static readonly EventSystem.IntraObjectHandler<ColonyAchievementTracker> OnNewDayDelegate = new EventSystem.IntraObjectHandler<ColonyAchievementTracker>(delegate(ColonyAchievementTracker component, object data)
	{
		component.OnNewDay(data);
	});

	public List<string> achievementsToDisplay => completedAchievementsToDisplay;

	public void ClearDisplayAchievements()
	{
		achievementsToDisplay.Clear();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (ColonyAchievement resource in Db.Get().ColonyAchievements.resources)
		{
			if (!achievements.ContainsKey(resource.Id))
			{
				ColonyAchievementStatus value = new ColonyAchievementStatus(resource.Id);
				achievements.Add(resource.Id, value);
			}
		}
		forceCheckAchievementHandle = Game.Instance.Subscribe(395452326, CheckAchievements);
		Subscribe(631075836, OnNewDayDelegate);
		UpgradeTamedCritterAchievements();
	}

	private void UpgradeTamedCritterAchievements()
	{
		foreach (ColonyAchievementRequirement item in Db.Get().ColonyAchievements.TameAllBasicCritters.requirementChecklist)
		{
			(item as CritterTypesWithTraits)?.UpdateSavedState();
		}
		foreach (ColonyAchievementRequirement item2 in Db.Get().ColonyAchievements.TameAGassyMoo.requirementChecklist)
		{
			(item2 as CritterTypesWithTraits)?.UpdateSavedState();
		}
	}

	public void RenderEveryTick(float dt)
	{
		if (updatingAchievement >= achievements.Count)
		{
			updatingAchievement = 0;
		}
		KeyValuePair<string, ColonyAchievementStatus> keyValuePair = achievements.ElementAt(updatingAchievement);
		updatingAchievement++;
		if (!keyValuePair.Value.success && !keyValuePair.Value.failed)
		{
			keyValuePair.Value.UpdateAchievement();
			if (keyValuePair.Value.success && !keyValuePair.Value.failed)
			{
				UnlockPlatformAchievement(keyValuePair.Key);
				completedAchievementsToDisplay.Add(keyValuePair.Key);
				TriggerNewAchievementCompleted(keyValuePair.Key);
				RetireColonyUtility.SaveColonySummaryData();
			}
		}
	}

	private void CheckAchievements(object data = null)
	{
		foreach (KeyValuePair<string, ColonyAchievementStatus> achievement in achievements)
		{
			if (!achievement.Value.success && !achievement.Value.failed)
			{
				achievement.Value.UpdateAchievement();
				if (achievement.Value.success && !achievement.Value.failed)
				{
					UnlockPlatformAchievement(achievement.Key);
					completedAchievementsToDisplay.Add(achievement.Key);
					TriggerNewAchievementCompleted(achievement.Key);
				}
			}
		}
		RetireColonyUtility.SaveColonySummaryData();
	}

	private static void UnlockPlatformAchievement(string achievement_id)
	{
		if (DebugHandler.InstantBuildMode)
		{
			Debug.LogWarningFormat("UnlockPlatformAchievement {0} skipping: instant build mode", achievement_id);
			return;
		}
		if (SaveGame.Instance.sandboxEnabled)
		{
			Debug.LogWarningFormat("UnlockPlatformAchievement {0} skipping: sandbox mode", achievement_id);
			return;
		}
		if (Game.Instance.debugWasUsed)
		{
			Debug.LogWarningFormat("UnlockPlatformAchievement {0} skipping: debug was used.", achievement_id);
			return;
		}
		ColonyAchievement colonyAchievement = Db.Get().ColonyAchievements.Get(achievement_id);
		if (colonyAchievement != null && !string.IsNullOrEmpty(colonyAchievement.platformAchievementId))
		{
			if ((bool)SteamAchievementService.Instance)
			{
				SteamAchievementService.Instance.Unlock(colonyAchievement.platformAchievementId);
				return;
			}
			Debug.LogWarningFormat("Steam achievement [{0}] was achieved, but achievement service was null", colonyAchievement.platformAchievementId);
		}
	}

	public void DebugTriggerAchievement(string id)
	{
		achievements[id].failed = false;
		achievements[id].success = true;
	}

	private void BeginVictorySequence(string achievementID)
	{
		RootMenu.Instance.canTogglePauseScreen = false;
		CameraController.Instance.DisableUserCameraControl = true;
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(playSound: false);
		}
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryMessageSnapshot);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().MuteDynamicMusicSnapshot);
		ToggleVictoryUI(victoryUIActive: true);
		StoryMessageScreen component = GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.StoryMessageScreen.gameObject).GetComponent<StoryMessageScreen>();
		component.restoreInterfaceOnClose = false;
		component.title = COLONY_ACHIEVEMENTS.PRE_VICTORY_MESSAGE_HEADER;
		component.body = string.Format(COLONY_ACHIEVEMENTS.PRE_VICTORY_MESSAGE_BODY, "<b>" + Db.Get().ColonyAchievements.Get(achievementID).Name + "</b>\n" + Db.Get().ColonyAchievements.Get(achievementID).description);
		component.Show();
		CameraController.Instance.SetWorldInteractive(state: false);
		component.OnClose = (System.Action)Delegate.Combine(component.OnClose, (System.Action)delegate
		{
			SpeedControlScreen.Instance.SetSpeed(1);
			if (!SpeedControlScreen.Instance.IsPaused)
			{
				SpeedControlScreen.Instance.Pause(playSound: false);
			}
			CameraController.Instance.SetWorldInteractive(state: true);
			Db.Get().ColonyAchievements.Get(achievementID).victorySequence(this);
		});
	}

	public bool IsAchievementUnlocked(ColonyAchievement achievement)
	{
		foreach (KeyValuePair<string, ColonyAchievementStatus> achievement2 in achievements)
		{
			if (achievement2.Key == achievement.Id)
			{
				if (achievement2.Value.success)
				{
					return true;
				}
				achievement2.Value.UpdateAchievement();
				return achievement2.Value.success;
			}
		}
		return false;
	}

	protected override void OnCleanUp()
	{
		victorySchedulerHandle.ClearScheduler();
		Game.Instance.Unsubscribe(forceCheckAchievementHandle);
		checkAchievementsHandle.ClearScheduler();
		base.OnCleanUp();
	}

	private void TriggerNewAchievementCompleted(string achievement, GameObject cameraTarget = null)
	{
		unlockedAchievementMetric[UnlockedAchievementKey] = achievement;
		ThreadedHttps<KleiMetrics>.Instance.SendEvent(unlockedAchievementMetric, "TriggerNewAchievementCompleted");
		bool flag = false;
		if (Db.Get().ColonyAchievements.Get(achievement).isVictoryCondition)
		{
			flag = true;
			BeginVictorySequence(achievement);
		}
		if (!flag)
		{
			AchievementEarnedMessage message = new AchievementEarnedMessage();
			Messenger.Instance.QueueMessage(message);
		}
	}

	private void ToggleVictoryUI(bool victoryUIActive)
	{
		List<KScreen> list = new List<KScreen>();
		list.Add(NotificationScreen.Instance);
		list.Add(OverlayMenu.Instance);
		if (PlanScreen.Instance != null)
		{
			list.Add(PlanScreen.Instance);
		}
		if (BuildMenu.Instance != null)
		{
			list.Add(BuildMenu.Instance);
		}
		list.Add(ManagementMenu.Instance);
		list.Add(ToolMenu.Instance);
		list.Add(ToolMenu.Instance.PriorityScreen);
		list.Add(ResourceCategoryScreen.Instance);
		list.Add(TopLeftControlScreen.Instance);
		list.Add(DateTime.Instance);
		list.Add(BuildWatermark.Instance);
		list.Add(HoverTextScreen.Instance);
		list.Add(DetailsScreen.Instance);
		list.Add(DebugPaintElementScreen.Instance);
		list.Add(DebugBaseTemplateButton.Instance);
		list.Add(StarmapScreen.Instance);
		foreach (KScreen item in list)
		{
			if (item != null)
			{
				item.Show(!victoryUIActive);
			}
		}
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write(achievements.Count);
		foreach (KeyValuePair<string, ColonyAchievementStatus> achievement in achievements)
		{
			writer.WriteKleiString(achievement.Key);
			achievement.Value.Serialize(writer);
		}
	}

	public void Deserialize(IReader reader)
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 10))
		{
			return;
		}
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			string text = reader.ReadKleiString();
			ColonyAchievementStatus value = ColonyAchievementStatus.Deserialize(reader, text);
			if (Db.Get().ColonyAchievements.Exists(text))
			{
				achievements.Add(text, value);
			}
		}
	}

	public void LogFetchChore(GameObject fetcher, ChoreType choreType)
	{
		if (choreType == Db.Get().ChoreTypes.StorageFetch || choreType == Db.Get().ChoreTypes.BuildFetch || choreType == Db.Get().ChoreTypes.RepairFetch || choreType == Db.Get().ChoreTypes.FoodFetch || choreType == Db.Get().ChoreTypes.Transport)
		{
			return;
		}
		Dictionary<int, int> dictionary = null;
		if (fetcher.GetComponent<SolidTransferArm>() != null)
		{
			dictionary = fetchAutomatedChoreDeliveries;
		}
		else if (fetcher.GetComponent<MinionIdentity>() != null)
		{
			dictionary = fetchDupeChoreDeliveries;
		}
		if (dictionary != null)
		{
			int cycle = GameClock.Instance.GetCycle();
			if (!dictionary.ContainsKey(cycle))
			{
				dictionary.Add(cycle, 0);
			}
			dictionary[cycle]++;
		}
	}

	public void LogCritterTamed(Tag prefabId)
	{
		tamedCritterTypes.Add(prefabId);
	}

	public void LogSuitChore(ChoreDriver driver)
	{
		if (driver == null || driver.GetComponent<MinionIdentity>() == null)
		{
			return;
		}
		bool flag = false;
		foreach (EquipmentSlotInstance slot in driver.GetComponent<MinionIdentity>().GetEquipment().Slots)
		{
			Equippable equippable = slot.assignable as Equippable;
			if ((bool)equippable)
			{
				KPrefabID component = equippable.GetComponent<KPrefabID>();
				if (component.HasTag(GameTags.AtmoSuit) || component.HasTag(GameTags.JetSuit) || component.HasTag(GameTags.LeadSuit))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			int cycle = GameClock.Instance.GetCycle();
			int instanceID = driver.GetComponent<KPrefabID>().InstanceID;
			if (!dupesCompleteChoresInSuits.ContainsKey(cycle))
			{
				dupesCompleteChoresInSuits.Add(cycle, new List<int> { instanceID });
			}
			else if (!dupesCompleteChoresInSuits[cycle].Contains(instanceID))
			{
				dupesCompleteChoresInSuits[cycle].Add(instanceID);
			}
		}
	}

	public void LogAnalyzedSeed(Tag seed)
	{
		analyzedSeeds.Add(seed);
	}

	public void OnNewDay(object data)
	{
		foreach (MinionStorage item in Components.MinionStorages.Items)
		{
			if (!(item.GetComponent<CommandModule>() != null))
			{
				continue;
			}
			List<MinionStorage.Info> storedMinionInfo = item.GetStoredMinionInfo();
			if (storedMinionInfo.Count <= 0)
			{
				continue;
			}
			int cycle = GameClock.Instance.GetCycle();
			if (!dupesCompleteChoresInSuits.ContainsKey(cycle))
			{
				dupesCompleteChoresInSuits.Add(cycle, new List<int>());
			}
			for (int i = 0; i < storedMinionInfo.Count; i++)
			{
				KPrefabID kPrefabID = storedMinionInfo[i].serializedMinion.Get();
				if (kPrefabID != null)
				{
					dupesCompleteChoresInSuits[cycle].Add(kPrefabID.InstanceID);
				}
			}
		}
		if (!DlcManager.IsExpansion1Active())
		{
			return;
		}
		SurviveARocketWithMinimumMorale surviveARocketWithMinimumMorale = Db.Get().ColonyAchievements.SurviveInARocket.requirementChecklist[0] as SurviveARocketWithMinimumMorale;
		if (surviveARocketWithMinimumMorale == null)
		{
			return;
		}
		float minimumMorale = surviveARocketWithMinimumMorale.minimumMorale;
		int numberOfCycles = surviveARocketWithMinimumMorale.numberOfCycles;
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			if (!worldContainer.IsModuleInterior)
			{
				continue;
			}
			if (!cyclesRocketDupeMoraleAboveRequirement.ContainsKey(worldContainer.id))
			{
				cyclesRocketDupeMoraleAboveRequirement.Add(worldContainer.id, 0);
			}
			if (worldContainer.GetComponent<Clustercraft>().Status != 0)
			{
				List<MinionIdentity> worldItems = Components.MinionIdentities.GetWorldItems(worldContainer.id);
				bool flag = worldItems.Count > 0;
				foreach (MinionIdentity item2 in worldItems)
				{
					if (Db.Get().Attributes.QualityOfLife.Lookup(item2).GetTotalValue() < minimumMorale)
					{
						flag = false;
						break;
					}
				}
				cyclesRocketDupeMoraleAboveRequirement[worldContainer.id] = (flag ? (cyclesRocketDupeMoraleAboveRequirement[worldContainer.id] + 1) : 0);
			}
			else if (cyclesRocketDupeMoraleAboveRequirement[worldContainer.id] < numberOfCycles)
			{
				cyclesRocketDupeMoraleAboveRequirement[worldContainer.id] = 0;
			}
		}
	}
}
