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

	public void RefreshText()
	{
		string str = LaunchInitializer.BuildPrefix() + "-";
		bool flag = true;
		bool flag2 = DistributionPlatform.Initialized && DistributionPlatform.Inst.IsArchiveBranch;
		button.ClearOnClick();
		if (Application.isEditor)
		{
			str += "<EDITOR>";
		}
		else
		{
			str += 469473u;
			if (DebugHandler.enabled)
			{
				str += "-D";
			}
		}
		if (flag)
		{
			textDisplay.SetText(string.Format(UI.DEVELOPMENTBUILDS.WATERMARK, str));
			toolTip.ClearMultiStringTooltip();
		}
		else
		{
			textDisplay.SetText(string.Format(UI.DEVELOPMENTBUILDS.TESTING_WATERMARK, str));
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
		ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas, force_active: true);
		confirmDialogScreen.PopupConfirmDialog(UI.DEVELOPMENTBUILDS.TESTING_MESSAGE, delegate
		{
			Application.OpenURL("https://forums.kleientertainment.com/klei-bug-tracker/oni/");
		}, delegate
		{
		}, null, null, UI.DEVELOPMENTBUILDS.TESTING_MESSAGE_TITLE, UI.DEVELOPMENTBUILDS.TESTING_MORE_INFO);
	}
}
