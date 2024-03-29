using System;
using System.IO;
using Klei;
using STRINGS;
using UnityEngine;

public class DebugHandler : IInputHandler
{
	public enum PaintMode
	{
		None,
		Element,
		Hot,
		Cold
	}

	public static bool InstantBuildMode;

	public static bool InvincibleMode;

	public static bool SelectInEditor;

	public static bool DebugPathFinding;

	public static bool ScreenshotMode;

	public static bool TimelapseMode;

	public static bool HideUI;

	public static bool DebugCellInfo;

	public static bool DebugNextCall;

	public static bool RevealFogOfWar;

	private bool superTestMode;

	private bool ultraTestMode;

	private bool slowTestMode;

	private static int activeWorldBeforeOverride = -1;

	public static bool NotificationsDisabled { get; private set; }

	public static bool enabled { get; private set; }

	public string handlerName => "DebugHandler";

	public KInputHandler inputHandler { get; set; }

	public DebugHandler()
	{
		enabled = File.Exists(Path.Combine(Application.dataPath, "debug_enable.txt"));
		enabled = enabled || File.Exists(Path.Combine(Application.dataPath, "../debug_enable.txt"));
		enabled = enabled || GenericGameSettings.instance.debugEnable;
	}

	public static int GetMouseCell()
	{
		Vector3 mousePos = KInputManager.GetMousePos();
		mousePos.z = 0f - Camera.main.transform.GetPosition().z - Grid.CellSizeInMeters;
		return Grid.PosToCell(Camera.main.ScreenToWorldPoint(mousePos));
	}

	public static Vector3 GetMousePos()
	{
		Vector3 mousePos = KInputManager.GetMousePos();
		mousePos.z = 0f - Camera.main.transform.GetPosition().z - Grid.CellSizeInMeters;
		return Camera.main.ScreenToWorldPoint(mousePos);
	}

	private void SpawnMinion(bool addAtmoSuit = false)
	{
		if (Immigration.Instance == null)
		{
			return;
		}
		if (!Grid.IsValidBuildingCell(GetMouseCell()))
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.DEBUG_TOOLS.INVALID_LOCATION, null, GetMousePos(), 1.5f, track_target: false, force_spawn: true);
			return;
		}
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID));
		gameObject.name = Assets.GetPrefab(MinionConfig.ID).name;
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		Vector3 position = Grid.CellToPosCBC(GetMouseCell(), Grid.SceneLayer.Move);
		gameObject.transform.SetLocalPosition(position);
		gameObject.SetActive(value: true);
		new MinionStartingStats(is_starter_minion: false).Apply(gameObject);
		if (addAtmoSuit)
		{
			GameObject gameObject2 = GameUtil.KInstantiate(Assets.GetPrefab("Atmo_Suit"), position, Grid.SceneLayer.Creatures);
			gameObject2.SetActive(value: true);
			SuitTank component = gameObject2.GetComponent<SuitTank>();
			GameObject gameObject3 = GameUtil.KInstantiate(Assets.GetPrefab(GameTags.Oxygen), position, Grid.SceneLayer.Ore);
			gameObject3.GetComponent<PrimaryElement>().Units = component.capacity;
			gameObject3.SetActive(value: true);
			component.storage.Store(gameObject3, hide_popups: true);
			Equippable component2 = gameObject2.GetComponent<Equippable>();
			gameObject.GetComponent<MinionIdentity>().ValidateProxy();
			Equipment component3 = gameObject.GetComponent<MinionIdentity>().assignableProxy.Get().GetComponent<Equipment>();
			component2.Assign(component3.GetComponent<IAssignableIdentity>());
			gameObject2.GetComponent<EquippableWorkable>().CancelChore("Debug Handler");
			component3.Equip(component2);
		}
		gameObject.GetMyWorld().SetDupeVisited();
	}

	public static void SetDebugEnabled(bool debugEnabled)
	{
		enabled = debugEnabled;
	}

	public static void ToggleDisableNotifications()
	{
		NotificationsDisabled = !NotificationsDisabled;
	}

	private string GetScreenshotFileName()
	{
		string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
		string text = Path.Combine(Path.GetDirectoryName(activeSaveFilePath), "screenshot");
		string fileName = Path.GetFileName(activeSaveFilePath);
		Directory.CreateDirectory(text);
		return Path.ChangeExtension(Path.Combine(text, fileName), ".png");
	}

	public unsafe void OnKeyDown(KButtonEvent e)
	{
		if (!enabled)
		{
			return;
		}
		if (e.TryConsume(Action.DebugSpawnMinion))
		{
			SpawnMinion();
		}
		else if (e.TryConsume(Action.DebugSpawnMinionAtmoSuit))
		{
			SpawnMinion(addAtmoSuit: true);
		}
		else if (e.TryConsume(Action.DebugCheerEmote))
		{
			for (int i = 0; i < Components.MinionIdentities.Count; i++)
			{
				new EmoteChore(Components.MinionIdentities[i].GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[3] { "cheer_pre", "cheer_loop", "cheer_pst" }, null);
				new EmoteChore(Components.MinionIdentities[i].GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[3] { "cheer_pre", "cheer_loop", "cheer_pst" }, null);
			}
		}
		else if (e.TryConsume(Action.DebugSpawnStressTest))
		{
			for (int j = 0; j < 60; j++)
			{
				SpawnMinion();
			}
		}
		else if (e.TryConsume(Action.DebugSuperTestMode))
		{
			if (!superTestMode)
			{
				Time.timeScale = 15f;
				superTestMode = true;
			}
			else
			{
				Time.timeScale = 1f;
				superTestMode = false;
			}
		}
		else if (e.TryConsume(Action.DebugUltraTestMode))
		{
			if (!ultraTestMode)
			{
				Time.timeScale = 30f;
				ultraTestMode = true;
			}
			else
			{
				Time.timeScale = 1f;
				ultraTestMode = false;
			}
		}
		else if (e.TryConsume(Action.DebugSlowTestMode))
		{
			if (!slowTestMode)
			{
				Time.timeScale = 0.06f;
				slowTestMode = true;
			}
			else
			{
				Time.timeScale = 1f;
				slowTestMode = false;
			}
		}
		else if (e.TryConsume(Action.DebugDig))
		{
			SimMessages.Dig(GetMouseCell());
		}
		else if (e.TryConsume(Action.DebugToggleFastWorkers))
		{
			Game.Instance.FastWorkersModeActive = !Game.Instance.FastWorkersModeActive;
		}
		else if (e.TryConsume(Action.DebugInstantBuildMode))
		{
			InstantBuildMode = !InstantBuildMode;
			Game.Instance.Trigger(1557339983);
			if (Game.Instance == null)
			{
				return;
			}
			if (PlanScreen.Instance != null)
			{
				PlanScreen.Instance.Refresh();
			}
			if (BuildMenu.Instance != null)
			{
				BuildMenu.Instance.Refresh();
			}
			if (OverlayMenu.Instance != null)
			{
				OverlayMenu.Instance.Refresh();
			}
			if (ConsumerManager.instance != null)
			{
				ConsumerManager.instance.RefreshDiscovered();
			}
			if (ManagementMenu.Instance != null)
			{
				ManagementMenu.Instance.CheckResearch(null);
				ManagementMenu.Instance.CheckSkills();
				ManagementMenu.Instance.CheckStarmap();
			}
			Game.Instance.Trigger(1594320620, "all_the_things");
		}
		else if (e.TryConsume(Action.DebugExplosion))
		{
			Vector3 mousePos = KInputManager.GetMousePos();
			mousePos.z = 0f - Camera.main.transform.GetPosition().z - Grid.CellSizeInMeters;
			GameUtil.CreateExplosion(Camera.main.ScreenToWorldPoint(mousePos));
		}
		else if (e.TryConsume(Action.DebugLockCursor))
		{
			if (GenericGameSettings.instance.developerDebugEnable)
			{
				KInputManager.isMousePosLocked = !KInputManager.isMousePosLocked;
				KInputManager.lockedMousePos = KInputManager.GetMousePos();
			}
		}
		else if (e.TryConsume(Action.DebugDiscoverAllElements))
		{
			if (DiscoveredResources.Instance != null)
			{
				foreach (Element element in ElementLoader.elements)
				{
					DiscoveredResources.Instance.Discover(element.tag, element.GetMaterialCategoryTag());
				}
			}
		}
		else if (e.TryConsume(Action.DebugToggleUI))
		{
			ToggleScreenshotMode();
		}
		else if (e.TryConsume(Action.SreenShot1x))
		{
			ScreenCapture.CaptureScreenshot(GetScreenshotFileName(), 1);
		}
		else if (e.TryConsume(Action.SreenShot2x))
		{
			ScreenCapture.CaptureScreenshot(GetScreenshotFileName(), 2);
		}
		else if (e.TryConsume(Action.SreenShot8x))
		{
			ScreenCapture.CaptureScreenshot(GetScreenshotFileName(), 8);
		}
		else if (e.TryConsume(Action.SreenShot32x))
		{
			ScreenCapture.CaptureScreenshot(GetScreenshotFileName(), 32);
		}
		else if (e.TryConsume(Action.DebugCellInfo))
		{
			DebugCellInfo = !DebugCellInfo;
		}
		else if (e.TryConsume(Action.DebugToggle))
		{
			if (Game.Instance != null)
			{
				SaveGame.Instance.worldGenSpawner.SpawnEverything();
			}
			if (DebugPaintElementScreen.Instance != null)
			{
				bool activeSelf = DebugPaintElementScreen.Instance.gameObject.activeSelf;
				DebugPaintElementScreen.Instance.gameObject.SetActive(!activeSelf);
				if ((bool)DebugElementMenu.Instance && DebugElementMenu.Instance.root.activeSelf)
				{
					DebugElementMenu.Instance.root.SetActive(value: false);
				}
				DebugBaseTemplateButton.Instance.gameObject.SetActive(!activeSelf);
				PropertyTextures.FogOfWarScale = ((!activeSelf) ? 1 : 0);
				if (CameraController.Instance != null)
				{
					CameraController.Instance.EnableFreeCamera(!activeSelf);
				}
				RevealFogOfWar = !RevealFogOfWar;
				Game.Instance.Trigger(-1991583975);
			}
		}
		else if (e.TryConsume(Action.DebugCollectGarbage))
		{
			GC.Collect();
		}
		else if (e.TryConsume(Action.DebugInvincible))
		{
			InvincibleMode = !InvincibleMode;
		}
		else if (e.TryConsume(Action.DebugVisualTest))
		{
			Scenario.Instance.SetupVisualTest();
		}
		else if (e.TryConsume(Action.DebugGameplayTest))
		{
			Scenario.Instance.SetupGameplayTest();
		}
		else if (e.TryConsume(Action.DebugElementTest))
		{
			Scenario.Instance.SetupElementTest();
		}
		else if (e.TryConsume(Action.ToggleProfiler))
		{
			Sim.SIM_HandleMessage(-409964931, 0, null);
		}
		else if (e.TryConsume(Action.DebugRefreshNavCell))
		{
			Pathfinding.Instance.RefreshNavCell(GetMouseCell());
		}
		else if (e.TryConsume(Action.DebugToggleSelectInEditor))
		{
			SetSelectInEditor(!SelectInEditor);
		}
		else if (e.TryConsume(Action.DebugGotoTarget))
		{
			Debug.Log("Debug GoTo");
			Game.Instance.Trigger(775300118);
			foreach (Brain item in Components.Brains.Items)
			{
				item.GetSMI<DebugGoToMonitor.Instance>()?.GoToCursor();
				item.GetSMI<CreatureDebugGoToMonitor.Instance>()?.GoToCursor();
			}
		}
		else if (e.TryConsume(Action.DebugTeleport))
		{
			if (SelectTool.Instance == null)
			{
				return;
			}
			KSelectable selected = SelectTool.Instance.selected;
			if (selected != null)
			{
				int mouseCell = GetMouseCell();
				if (!Grid.IsValidBuildingCell(mouseCell))
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.DEBUG_TOOLS.INVALID_LOCATION, null, GetMousePos(), 1.5f, track_target: false, force_spawn: true);
					return;
				}
				selected.transform.SetPosition(Grid.CellToPosCBC(mouseCell, Grid.SceneLayer.Move));
			}
		}
		else if (!e.TryConsume(Action.DebugPlace) && !e.TryConsume(Action.DebugSelectMaterial))
		{
			if (e.TryConsume(Action.DebugNotification))
			{
				if (GenericGameSettings.instance.developerDebugEnable)
				{
					Tutorial.Instance.DebugNotification();
				}
			}
			else if (e.TryConsume(Action.DebugNotificationMessage))
			{
				if (GenericGameSettings.instance.developerDebugEnable)
				{
					Tutorial.Instance.DebugNotificationMessage();
				}
			}
			else if (e.TryConsume(Action.DebugSuperSpeed))
			{
				if (SpeedControlScreen.Instance != null)
				{
					SpeedControlScreen.Instance.ToggleRidiculousSpeed();
				}
			}
			else if (e.TryConsume(Action.DebugGameStep))
			{
				if (SpeedControlScreen.Instance != null)
				{
					SpeedControlScreen.Instance.DebugStepFrame();
				}
			}
			else if (e.TryConsume(Action.DebugSimStep))
			{
				Game.Instance.ForceSimStep();
			}
			else if (e.TryConsume(Action.DebugToggleMusic))
			{
				AudioDebug.Get().ToggleMusic();
			}
			else if (e.TryConsume(Action.DebugTileTest))
			{
				Scenario.Instance.SetupTileTest();
			}
			else if (e.TryConsume(Action.DebugForceLightEverywhere))
			{
				PropertyTextures.instance.ForceLightEverywhere = !PropertyTextures.instance.ForceLightEverywhere;
			}
			else if (e.TryConsume(Action.DebugPathFinding))
			{
				DebugPathFinding = !DebugPathFinding;
				Debug.Log("DebugPathFinding=" + DebugPathFinding);
			}
			else if (!e.TryConsume(Action.DebugFocus))
			{
				if (e.TryConsume(Action.DebugReportBug))
				{
					if (GenericGameSettings.instance.developerDebugEnable)
					{
						int num = 0;
						string validSaveFilename;
						while (true)
						{
							validSaveFilename = SaveScreen.GetValidSaveFilename("bug_report_savefile_" + num);
							if (!File.Exists(validSaveFilename))
							{
								break;
							}
							num++;
						}
						string save_file = "No save file (front end)";
						if (SaveLoader.Instance != null)
						{
							save_file = SaveLoader.Instance.Save(validSaveFilename, isAutoSave: false, updateSavePointer: false);
						}
						KCrashReporter.ReportBug("Bug Report", save_file, GameObject.Find("ScreenSpaceOverlayCanvas"));
					}
					else
					{
						Debug.Log("Debug crash keys are not enabled.");
					}
				}
				else if (e.TryConsume(Action.DebugTriggerException))
				{
					if (GenericGameSettings.instance.developerDebugEnable)
					{
						throw new ArgumentException("My test exception");
					}
				}
				else if (e.TryConsume(Action.DebugTriggerError))
				{
					if (GenericGameSettings.instance.developerDebugEnable)
					{
						Debug.LogError("Oooops! Testing error!");
					}
				}
				else if (e.TryConsume(Action.DebugDumpGCRoots))
				{
					GarbageProfiler.DebugDumpRootItems();
				}
				else if (e.TryConsume(Action.DebugDumpGarbageReferences))
				{
					GarbageProfiler.DebugDumpGarbageStats();
				}
				else if (e.TryConsume(Action.DebugDumpEventData))
				{
					if (GenericGameSettings.instance.developerDebugEnable)
					{
						KObjectManager.Instance.DumpEventData();
					}
				}
				else if (e.TryConsume(Action.DebugDumpSceneParitionerLeakData))
				{
					if (!GenericGameSettings.instance.developerDebugEnable)
					{
					}
				}
				else if (e.TryConsume(Action.DebugCrashSim))
				{
					if (GenericGameSettings.instance.developerDebugEnable)
					{
						Sim.SIM_DebugCrash();
					}
				}
				else if (e.TryConsume(Action.DebugNextCall))
				{
					DebugNextCall = true;
				}
				else if (e.TryConsume(Action.DebugTogglePersonalPriorityComparison))
				{
					Chore.ENABLE_PERSONAL_PRIORITIES = !Chore.ENABLE_PERSONAL_PRIORITIES;
				}
				else if (e.TryConsume(Action.DebugToggleClusterFX))
				{
					CameraController.Instance.ToggleClusterFX();
				}
			}
		}
		if (e.Consumed && Game.Instance != null)
		{
			Game.Instance.debugWasUsed = true;
			KCrashReporter.debugWasUsed = true;
		}
	}

	public static void SetSelectInEditor(bool select_in_editor)
	{
	}

	public static void ToggleScreenshotMode()
	{
		ScreenshotMode = !ScreenshotMode;
		UpdateUI();
		if (CameraController.Instance != null)
		{
			CameraController.Instance.EnableFreeCamera(ScreenshotMode);
		}
		if (KScreenManager.Instance != null)
		{
			KScreenManager.Instance.DisableInput(ScreenshotMode);
		}
	}

	public static void SetTimelapseMode(bool enabled, int world_id = 0)
	{
		TimelapseMode = enabled;
		if (enabled)
		{
			activeWorldBeforeOverride = ClusterManager.Instance.activeWorldId;
			ClusterManager.Instance.TimelapseModeOverrideActiveWorld(world_id);
		}
		else
		{
			ClusterManager.Instance.TimelapseModeOverrideActiveWorld(activeWorldBeforeOverride);
		}
		World.Instance.zoneRenderData.OnActiveWorldChanged();
		UpdateUI();
	}

	private static void UpdateUI()
	{
		HideUI = TimelapseMode || ScreenshotMode;
		float num = (HideUI ? 0f : 1f);
		GameScreenManager.Instance.ssHoverTextCanvas.GetComponent<CanvasGroup>().alpha = num;
		GameScreenManager.Instance.ssCameraCanvas.GetComponent<CanvasGroup>().alpha = num;
		GameScreenManager.Instance.ssOverlayCanvas.GetComponent<CanvasGroup>().alpha = num;
		GameScreenManager.Instance.worldSpaceCanvas.GetComponent<CanvasGroup>().alpha = num;
		GameScreenManager.Instance.screenshotModeCanvas.GetComponent<CanvasGroup>().alpha = 1f - num;
	}
}
