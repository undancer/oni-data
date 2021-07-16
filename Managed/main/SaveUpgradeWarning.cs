using System;
using System.Collections;
using System.Collections.Generic;
using Database;
using Klei.AI;
using Klei.CustomSettings;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SaveUpgradeWarning")]
public class SaveUpgradeWarning : KMonoBehaviour
{
	private struct Upgrade
	{
		public int major;

		public int minor;

		public System.Action action;

		public Upgrade(int major, int minor, System.Action action)
		{
			this.major = major;
			this.minor = minor;
			this.action = action;
		}
	}

	[MyCmpReq]
	private Game game;

	private static string[] buildingIDsWithNewPorts = new string[6]
	{
		"LiquidVent",
		"GasVent",
		"GasVentHighPressure",
		"SolidVent",
		"LiquidReservoir",
		"GasReservoir"
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Game obj = game;
		obj.OnLoad = (Action<Game.GameSaveData>)Delegate.Combine(obj.OnLoad, new Action<Game.GameSaveData>(OnLoad));
	}

	protected override void OnCleanUp()
	{
		Game obj = game;
		obj.OnLoad = (Action<Game.GameSaveData>)Delegate.Remove(obj.OnLoad, new Action<Game.GameSaveData>(OnLoad));
		base.OnCleanUp();
	}

	private void OnLoad(Game.GameSaveData data)
	{
		List<Upgrade> list = new List<Upgrade>
		{
			new Upgrade(7, 5, SuddenMoraleHelper),
			new Upgrade(7, 13, BedAndBathHelper),
			new Upgrade(7, 16, NewAutomationWarning)
		};
		if (DlcManager.IsPureVanilla())
		{
			list.Add(new Upgrade(7, 25, MergedownWarning));
		}
		foreach (Upgrade item in list)
		{
			if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(item.major, item.minor))
			{
				item.action();
			}
		}
	}

	private void SuddenMoraleHelper()
	{
		Effect morale_effect = Db.Get().effects.Get("SuddenMoraleHelper");
		CustomizableDialogScreen screen = Util.KInstantiateUI<CustomizableDialogScreen>(ScreenPrefabs.Instance.CustomizableDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, force_active: true);
		screen.AddOption(UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER_BUFF, delegate
		{
			foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
			{
				item.GetComponent<Effects>().Add(morale_effect, should_save: true);
			}
			screen.Deactivate();
		});
		screen.AddOption(UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER_DISABLE, delegate
		{
			SettingConfig morale = CustomGameSettingConfigs.Morale;
			CustomGameSettings.Instance.customGameMode = CustomGameSettings.CustomGameMode.Custom;
			CustomGameSettings.Instance.SetQualitySetting(morale, morale.GetLevel("Disabled").id);
			screen.Deactivate();
		});
		screen.PopupConfirmDialog(string.Format(UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER, Mathf.RoundToInt(morale_effect.duration / 600f)), UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER_TITLE);
	}

	private void BedAndBathHelper()
	{
		if (SaveGame.Instance == null)
		{
			return;
		}
		ColonyAchievementTracker component = SaveGame.Instance.GetComponent<ColonyAchievementTracker>();
		if (!(component == null))
		{
			ColonyAchievement basicComforts = Db.Get().ColonyAchievements.BasicComforts;
			ColonyAchievementStatus value = null;
			if (component.achievements.TryGetValue(basicComforts.Id, out value))
			{
				value.failed = false;
			}
		}
	}

	private void NewAutomationWarning()
	{
		SpriteListDialogScreen screen = Util.KInstantiateUI<SpriteListDialogScreen>(ScreenPrefabs.Instance.SpriteListDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, force_active: true);
		screen.AddOption(UI.CONFIRMDIALOG.OK, delegate
		{
			screen.Deactivate();
		});
		string[] array = buildingIDsWithNewPorts;
		for (int i = 0; i < array.Length; i++)
		{
			BuildingDef buildingDef = Assets.GetBuildingDef(array[i]);
			screen.AddSprite(buildingDef.GetUISprite(), buildingDef.Name);
		}
		screen.PopupConfirmDialog(UI.FRONTEND.SAVEUPGRADEWARNINGS.NEWAUTOMATIONWARNING, UI.FRONTEND.SAVEUPGRADEWARNINGS.NEWAUTOMATIONWARNING_TITLE);
		StartCoroutine(SendAutomationWarningNotifications());
	}

	private IEnumerator SendAutomationWarningNotifications()
	{
		yield return new WaitForEndOfFrame();
		if (Components.BuildingCompletes.Count == 0)
		{
			Debug.LogWarning("Could not send automation warnings because buildings have not yet loaded");
		}
		foreach (BuildingComplete buildingComplete in Components.BuildingCompletes)
		{
			string[] array = buildingIDsWithNewPorts;
			foreach (string text in array)
			{
				BuildingDef buildingDef = Assets.GetBuildingDef(text);
				if (!(buildingComplete.Def == buildingDef))
				{
					continue;
				}
				List<ILogicUIElement> list = new List<ILogicUIElement>();
				LogicPorts component = buildingComplete.GetComponent<LogicPorts>();
				if (component.outputPorts != null)
				{
					list.AddRange(component.outputPorts);
				}
				if (component.inputPorts != null)
				{
					list.AddRange(component.inputPorts);
				}
				foreach (ILogicUIElement item in list)
				{
					if (Grid.Objects[item.GetLogicUICell(), 31] != null)
					{
						Debug.Log("Triggering automation warning for building of type " + text);
						GenericMessage message = new GenericMessage(MISC.NOTIFICATIONS.NEW_AUTOMATION_WARNING.NAME, MISC.NOTIFICATIONS.NEW_AUTOMATION_WARNING.TOOLTIP, MISC.NOTIFICATIONS.NEW_AUTOMATION_WARNING.TOOLTIP, buildingComplete);
						Messenger.Instance.QueueMessage(message);
					}
				}
			}
		}
	}

	private void MergedownWarning()
	{
		SpriteListDialogScreen screen = Util.KInstantiateUI<SpriteListDialogScreen>(ScreenPrefabs.Instance.SpriteListDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, force_active: true);
		screen.AddOption(UI.DEVELOPMENTBUILDS.FULL_PATCH_NOTES, delegate
		{
			Application.OpenURL("https://forums.kleientertainment.com/game-updates/oni-alpha/");
		});
		screen.AddOption(UI.CONFIRMDIALOG.OK, delegate
		{
			screen.Deactivate();
		});
		screen.AddSprite(Assets.GetSprite("upgrade_mergedown_fridge"), UI.FRONTEND.SAVEUPGRADEWARNINGS.MERGEDOWNCHANGES_FOOD, 150f, 120f);
		screen.AddSprite(Assets.GetSprite("upgrade_mergedown_deodorizer"), UI.FRONTEND.SAVEUPGRADEWARNINGS.MERGEDOWNCHANGES_AIRFILTER, 150f, 120f);
		screen.AddSprite(Assets.GetSprite("upgrade_mergedown_steamturbine"), UI.FRONTEND.SAVEUPGRADEWARNINGS.MERGEDOWNCHANGES_SIMULATION, 150f, 120f);
		screen.AddSprite(Assets.GetSprite("upgrade_mergedown_oxygen_meter"), UI.FRONTEND.SAVEUPGRADEWARNINGS.MERGEDOWNCHANGES_BUILDINGS, 150f, 120f);
		screen.PopupConfirmDialog(UI.FRONTEND.SAVEUPGRADEWARNINGS.MERGEDOWNCHANGES, UI.FRONTEND.SAVEUPGRADEWARNINGS.MERGEDOWNCHANGES_TITLE);
		StartCoroutine(SendAutomationWarningNotifications());
	}
}
