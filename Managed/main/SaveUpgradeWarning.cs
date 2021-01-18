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
				Effects component = item.GetComponent<Effects>();
				component.Add(morale_effect, should_save: true);
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
		foreach (string prefab_id in array)
		{
			BuildingDef buildingDef = Assets.GetBuildingDef(prefab_id);
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
			foreach (string id in array)
			{
				BuildingDef buildingDef = Assets.GetBuildingDef(id);
				if (!(buildingComplete.Def == buildingDef))
				{
					continue;
				}
				List<ILogicUIElement> newPorts = new List<ILogicUIElement>();
				LogicPorts ports = buildingComplete.GetComponent<LogicPorts>();
				if (ports.outputPorts != null)
				{
					newPorts.AddRange(ports.outputPorts);
				}
				if (ports.inputPorts != null)
				{
					newPorts.AddRange(ports.inputPorts);
				}
				foreach (ILogicUIElement port in newPorts)
				{
					if (Grid.Objects[port.GetLogicUICell(), 31] != null)
					{
						Debug.Log("Triggering automation warning for building of type " + id);
						GenericMessage message = new GenericMessage(MISC.NOTIFICATIONS.NEW_AUTOMATION_WARNING.NAME, MISC.NOTIFICATIONS.NEW_AUTOMATION_WARNING.TOOLTIP, MISC.NOTIFICATIONS.NEW_AUTOMATION_WARNING.TOOLTIP, buildingComplete);
						Messenger.Instance.QueueMessage(message);
					}
				}
			}
		}
	}
}
