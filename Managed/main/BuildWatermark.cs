using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class BuildWatermark : KScreen
{
	public bool interactable = true;

	public LocText textDisplay;

	public ToolTip toolTip;

	public KButton button;

	public List<GameObject> archiveIcons;

	public static BuildWatermark Instance;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		RefreshText();
	}

	public static string GetBuildText()
	{
		string text = (DistributionPlatform.Initialized ? (LaunchInitializer.BuildPrefix() + "-") : "??-");
		if (Application.isEditor)
		{
			text += "<EDITOR>";
		}
		else
		{
			text += 479045u;
			text = ((!DistributionPlatform.Initialized) ? (text + "-?") : (text + "-" + DlcManager.GetActiveContentLetters()));
			if (DebugHandler.enabled)
			{
				text += "D";
			}
		}
		return text;
	}

	public void RefreshText()
	{
		bool flag = true;
		bool flag2 = DistributionPlatform.Initialized && DistributionPlatform.Inst.IsArchiveBranch;
		string buildText = GetBuildText();
		button.ClearOnClick();
		if (flag)
		{
			textDisplay.SetText(string.Format(UI.DEVELOPMENTBUILDS.WATERMARK, buildText));
			toolTip.ClearMultiStringTooltip();
		}
		else
		{
			textDisplay.SetText(string.Format(UI.DEVELOPMENTBUILDS.TESTING_WATERMARK, buildText));
			toolTip.SetSimpleTooltip(UI.DEVELOPMENTBUILDS.TESTING_TOOLTIP);
			if (interactable)
			{
				button.onClick += ShowTestingMessage;
			}
		}
		foreach (GameObject archiveIcon in archiveIcons)
		{
			archiveIcon.SetActive(flag && flag2);
		}
	}

	private void ShowTestingMessage()
	{
		Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas, force_active: true).PopupConfirmDialog(UI.DEVELOPMENTBUILDS.TESTING_MESSAGE, delegate
		{
			Application.OpenURL("https://forums.kleientertainment.com/klei-bug-tracker/oni/");
		}, delegate
		{
		}, null, null, UI.DEVELOPMENTBUILDS.TESTING_MESSAGE_TITLE, UI.DEVELOPMENTBUILDS.TESTING_MORE_INFO);
	}
}
