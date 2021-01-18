using System;
using System.Collections.Generic;
using System.IO;
using STRINGS;
using UnityEngine;

public class SaveScreen : KModalScreen
{
	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton newSaveButton;

	[SerializeField]
	private KButton oldSaveButtonPrefab;

	[SerializeField]
	private Transform oldSavesRoot;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		oldSaveButtonPrefab.gameObject.SetActive(value: false);
		newSaveButton.onClick += OnClickNewSave;
		closeButton.onClick += Deactivate;
	}

	protected override void OnCmpEnable()
	{
		List<string> allColonyFiles = SaveLoader.GetAllColonyFiles();
		foreach (string item in allColonyFiles)
		{
			AddExistingSaveFile(item);
		}
		SpeedControlScreen.Instance.Pause();
	}

	protected override void OnDeactivate()
	{
		SpeedControlScreen.Instance.Unpause();
		base.OnDeactivate();
	}

	private void AddExistingSaveFile(string filename)
	{
		KButton kButton = Util.KInstantiateUI<KButton>(oldSaveButtonPrefab.gameObject, oldSavesRoot.gameObject, force_active: true);
		HierarchyReferences component = kButton.GetComponent<HierarchyReferences>();
		LocText component2 = component.GetReference<RectTransform>("Title").GetComponent<LocText>();
		LocText component3 = component.GetReference<RectTransform>("Date").GetComponent<LocText>();
		System.DateTime lastWriteTime = File.GetLastWriteTime(filename);
		component2.text = $"{Path.GetFileNameWithoutExtension(filename)}";
		component3.text = string.Format("{0:H:mm:ss}" + Localization.GetFileDateFormat(0), lastWriteTime);
		kButton.onClick += delegate
		{
			Save(filename);
		};
	}

	public static string GetValidSaveFilename(string filename)
	{
		string text = ".sav";
		string a = Path.GetExtension(filename).ToLower();
		if (a != text)
		{
			filename += text;
		}
		return filename;
	}

	public void Save(string filename)
	{
		filename = GetValidSaveFilename(filename);
		if (File.Exists(filename))
		{
			ScreenPrefabs.Instance.ConfirmDoAction(string.Format(UI.FRONTEND.SAVESCREEN.OVERWRITEMESSAGE, Path.GetFileNameWithoutExtension(filename)), delegate
			{
				DoSave(filename);
			}, base.transform.parent);
		}
		else
		{
			DoSave(filename);
		}
	}

	private void DoSave(string filename)
	{
		ReportErrorDialog.MOST_RECENT_SAVEFILE = filename;
		try
		{
			SaveLoader.Instance.Save(filename);
			Deactivate();
		}
		catch (IOException ex)
		{
			IOException e = ex;
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, force_active: true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(string.Format(UI.FRONTEND.SAVESCREEN.IO_ERROR, e.ToString()), delegate
			{
				Deactivate();
			}, null, UI.FRONTEND.SAVESCREEN.REPORT_BUG, delegate
			{
				KCrashReporter.ReportError(e.Message, e.StackTrace.ToString(), null, null, null);
			});
		}
	}

	public void OnClickNewSave()
	{
		FileNameDialog fileNameDialog = (FileNameDialog)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.FileNameDialog.gameObject, base.transform.parent.gameObject);
		string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
		if (activeSaveFilePath != null)
		{
			string originalSaveFileName = SaveLoader.GetOriginalSaveFileName(activeSaveFilePath);
			originalSaveFileName = Path.GetFileNameWithoutExtension(originalSaveFileName);
			fileNameDialog.SetTextAndSelect(originalSaveFileName);
		}
		fileNameDialog.onConfirm = delegate(string filename)
		{
			string activeSaveColonyFolder = SaveLoader.GetActiveSaveColonyFolder();
			filename = Path.Combine(activeSaveColonyFolder, filename);
			Save(filename);
		};
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			Deactivate();
		}
		e.Consumed = true;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		e.Consumed = true;
	}
}
