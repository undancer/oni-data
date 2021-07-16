using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LoreBearer")]
public class LoreBearer : KMonoBehaviour, ISidescreenButtonControl
{
	[Serialize]
	private bool BeenClicked;

	public string BeenSearched = UI.USERMENUACTIONS.READLORE.ALREADY_SEARCHED;

	public string content => Strings.Get("STRINGS.LORE.BUILDINGS." + base.gameObject.name + ".ENTRY");

	public string SidescreenButtonText => BeenClicked ? UI.USERMENUACTIONS.READLORE.ALREADYINSPECTED : UI.USERMENUACTIONS.READLORE.NAME;

	public string SidescreenButtonTooltip => BeenClicked ? UI.USERMENUACTIONS.READLORE.TOOLTIP_ALREADYINSPECTED : UI.USERMENUACTIONS.READLORE.TOOLTIP;

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	private Action<InfoDialogScreen> OpenCodexByLockKeyID(string key)
	{
		return delegate(InfoDialogScreen dialog)
		{
			dialog.Deactivate();
			string entryForLock = CodexCache.GetEntryForLock(key);
			if (entryForLock == null)
			{
				DebugUtil.DevLogError("Missing codex entry for lock: " + key);
			}
			else
			{
				ManagementMenu.Instance.OpenCodexToEntry(entryForLock);
			}
		};
	}

	private void OnClickRead()
	{
		InfoDialogScreen infoDialogScreen = (InfoDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject);
		infoDialogScreen.SetHeader(base.gameObject.GetComponent<KSelectable>().GetProperName()).AddDefaultOK(escapeCloses: true);
		if (BeenClicked)
		{
			infoDialogScreen.AddPlainText(BeenSearched);
			return;
		}
		BeenClicked = true;
		if (base.gameObject.name == "GeneShuffler")
		{
			Game.Instance.unlocks.Unlock("neuralvacillator");
		}
		if (base.gameObject.name == "PropDesk")
		{
			string text = Game.Instance.unlocks.UnlockNext("emails");
			if (text != null)
			{
				string str = "SEARCH" + UnityEngine.Random.Range(1, 6);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_SUCCESS." + str));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID(text));
			}
			else
			{
				string str2 = "SEARCH" + UnityEngine.Random.Range(1, 8);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_FAIL." + str2));
			}
		}
		else if (base.gameObject.name == "GeneShuffler" || base.gameObject.name == "MassiveHeatSink")
		{
			string text2 = Game.Instance.unlocks.UnlockNext("researchnotes");
			if (text2 != null)
			{
				string str3 = "SEARCH" + UnityEngine.Random.Range(1, 3);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TECHNOLOGY_SUCCESS." + str3));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID(text2));
			}
			else
			{
				string str4 = "SEARCH1";
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + str4));
			}
		}
		else if (base.gameObject.GetProperName() == (string)BUILDINGS.PREFABS.PROPGRAVITASJAR1.NAME || base.gameObject.name == (string)BUILDINGS.PREFABS.PROPGRAVITASJAR2.NAME || base.gameObject.name == (string)BUILDINGS.PREFABS.PROPGRAVITASDISPLAY4.NAME)
		{
			string text3 = Game.Instance.unlocks.UnlockNext("dimensionallore");
			if (text3 != null)
			{
				string str5 = "SEARCH" + UnityEngine.Random.Range(1, 6);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS." + str5));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID(text3));
			}
			else
			{
				string str6 = "SEARCH1";
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + str6));
			}
		}
		else if (base.gameObject.name == "PropReceptionDesk")
		{
			Game.Instance.unlocks.Unlock("email_pens");
			infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_ELLIESDESK);
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID("email_pens"));
		}
		else if (base.gameObject.name == "PropFacilityDesk")
		{
			Game.Instance.unlocks.Unlock("journal_magazine");
			infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_STERNSDESK);
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID("journal_magazine"));
		}
		else if (base.gameObject.name == "HeadquartersComplete")
		{
			Game.Instance.unlocks.Unlock("pod_evacuation");
			infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_POD);
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID("pod_evacuation"));
		}
		else if (base.gameObject.name == "PropFacilityDisplay")
		{
			Game.Instance.unlocks.Unlock("display_prop1");
			infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID("display_prop1"));
		}
		else if (base.gameObject.name == "PropFacilityDisplay2")
		{
			Game.Instance.unlocks.Unlock("display_prop2");
			infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID("display_prop2"));
		}
		else if (base.gameObject.name == "PropFacilityDisplay3")
		{
			Game.Instance.unlocks.Unlock("display_prop3");
			infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID("display_prop3"));
		}
		else if (base.gameObject.name == "PropFacilityGlobeDroors")
		{
			Game.Instance.unlocks.Unlock("journal_newspaper");
			infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_CABINET"));
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID("journal_newspaper"));
		}
		else if (base.gameObject.GetProperName() == (string)BUILDINGS.PREFABS.WARPRECEIVER.NAME)
		{
			Game.Instance.unlocks.Unlock("notes_AI");
			infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TELEPORTER_RECEIVER"));
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID("notes_AI"));
		}
		else if (base.gameObject.GetProperName() == (string)BUILDINGS.PREFABS.WARPPORTAL.NAME)
		{
			Game.Instance.unlocks.Unlock("notes_teleportation");
			infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TELEPORTER_SENDER"));
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID("notes_teleportation"));
		}
		else if (base.gameObject.GetProperName() == (string)BUILDINGS.PREFABS.CRYOTANK.NAME)
		{
			Game.Instance.unlocks.Unlock("cryotank_warning");
			infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_CRYO_TANK"));
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID("cryotank_warning"));
		}
		else if (base.gameObject.name.Contains("ArtifactSpacePOI"))
		{
			string text4 = Game.Instance.unlocks.UnlockNext("space");
			if (text4 != null)
			{
				string str7 = "SEARCH" + UnityEngine.Random.Range(1, 7);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_SPACEPOI_SUCCESS." + str7));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID(text4));
			}
			else
			{
				string str8 = "SEARCH" + UnityEngine.Random.Range(1, 4);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_SPACEPOI_FAIL." + str8));
			}
		}
		else
		{
			string text5 = Game.Instance.unlocks.UnlockNext("journals");
			if (text5 != null)
			{
				string str9 = "SEARCH" + UnityEngine.Random.Range(1, 6);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS." + str9));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, OpenCodexByLockKeyID(text5));
			}
			else
			{
				string str10 = "SEARCH1";
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + str10));
			}
		}
	}

	public bool SidescreenEnabled()
	{
		return true;
	}

	public void OnSidescreenButtonPressed()
	{
		OnClickRead();
	}

	public bool SidescreenButtonInteractable()
	{
		return !BeenClicked;
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}
}
