using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using KMod;
using STRINGS;
using TMPro;
using UnityEngine;

public class ReportErrorDialog : MonoBehaviour
{
	private enum Mode
	{
		SubmitError,
		DisableMods
	}

	public static string MOST_RECENT_SAVEFILE = null;

	private System.Action submitAction;

	private System.Action quitAction;

	private System.Action continueAction;

	public TMP_InputField messageInputField;

	public GameObject referenceMessage;

	private string m_stackTrace;

	[SerializeField]
	private KButton submitButton;

	[SerializeField]
	private KButton moreInfoButton;

	[SerializeField]
	private KButton quitButton;

	[SerializeField]
	private KButton continueGameButton;

	[SerializeField]
	private LocText CrashLabel;

	[SerializeField]
	private GameObject CrashDescription;

	[SerializeField]
	private GameObject ModsInfo;

	[SerializeField]
	private GameObject StackTrace;

	[SerializeField]
	private GameObject uploadSaveDialog;

	[SerializeField]
	private KButton uploadSaveButton;

	[SerializeField]
	private KButton skipUploadSaveButton;

	[SerializeField]
	private LocText saveFileInfoLabel;

	[SerializeField]
	private GameObject modEntryPrefab;

	[SerializeField]
	private Transform modEntryParent;

	private Mode mode;

	private void Start()
	{
		ThreadedHttps<KleiMetrics>.Instance.EndSession(crashed: true);
		if ((bool)KScreenManager.Instance)
		{
			KScreenManager.Instance.DisableInput(disable: true);
		}
		StackTrace.SetActive(value: false);
		CrashLabel.text = ((mode == Mode.SubmitError) ? UI.CRASHSCREEN.TITLE : UI.CRASHSCREEN.TITLE_MODS);
		CrashDescription.SetActive(mode == Mode.SubmitError);
		ModsInfo.SetActive(mode == Mode.DisableMods);
		if (mode == Mode.DisableMods)
		{
			BuildModsList();
		}
		submitButton.gameObject.SetActive(submitAction != null);
		submitButton.onClick += OnSelect_SUBMIT;
		moreInfoButton.onClick += OnSelect_MOREINFO;
		continueGameButton.gameObject.SetActive(continueAction != null);
		continueGameButton.onClick += OnSelect_CONTINUE;
		quitButton.onClick += OnSelect_QUIT;
		uploadSaveButton.onClick += OnSelect_UPLOADSAVE;
		skipUploadSaveButton.onClick += OnSelect_SKIPUPLOADSAVE;
		messageInputField.text = UI.CRASHSCREEN.BODY;
	}

	private void BuildModsList()
	{
		DebugUtil.Assert(Global.Instance != null && Global.Instance.modManager != null);
		Manager mod_mgr = Global.Instance.modManager;
		List<Mod> allCrashableMods = mod_mgr.GetAllCrashableMods();
		allCrashableMods.Sort((Mod x, Mod y) => y.foundInStackTrace.CompareTo(x.foundInStackTrace));
		foreach (Mod item in allCrashableMods)
		{
			if (item.foundInStackTrace && item.label.distribution_platform != Label.DistributionPlatform.Dev)
			{
				mod_mgr.EnableMod(item.label, enabled: false, this);
			}
			HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(modEntryPrefab, modEntryParent.gameObject);
			LocText reference = hierarchyReferences.GetReference<LocText>("Title");
			reference.text = item.title;
			reference.color = (item.foundInStackTrace ? Color.red : Color.white);
			MultiToggle toggle = hierarchyReferences.GetReference<MultiToggle>("EnabledToggle");
			toggle.ChangeState(item.IsEnabledForActiveDlc() ? 1 : 0);
			Label mod_label = item.label;
			MultiToggle multiToggle = toggle;
			multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
			{
				bool flag = !mod_mgr.IsModEnabled(mod_label);
				toggle.ChangeState(flag ? 1 : 0);
				mod_mgr.EnableMod(mod_label, flag, this);
			});
			toggle.GetComponent<ToolTip>().OnToolTip = () => mod_mgr.IsModEnabled(mod_label) ? UI.FRONTEND.MODS.TOOLTIPS.ENABLED : UI.FRONTEND.MODS.TOOLTIPS.DISABLED;
			hierarchyReferences.gameObject.SetActive(value: true);
		}
	}

	private void Update()
	{
		Debug.developerConsoleVisible = false;
	}

	private void OnDestroy()
	{
		if (KCrashReporter.terminateOnError)
		{
			App.Quit();
		}
		if ((bool)KScreenManager.Instance)
		{
			KScreenManager.Instance.DisableInput(disable: false);
		}
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			OnSelect_QUIT();
		}
	}

	public void PopupSubmitErrorDialog(string stackTrace, System.Action onSubmit, System.Action onQuit, System.Action onContinue)
	{
		mode = Mode.SubmitError;
		m_stackTrace = stackTrace;
		submitAction = onSubmit;
		quitAction = onQuit;
		continueAction = onContinue;
	}

	public void PopupDisableModsDialog(string stackTrace, System.Action onQuit, System.Action onContinue)
	{
		mode = Mode.DisableMods;
		m_stackTrace = stackTrace;
		quitAction = onQuit;
		continueAction = onContinue;
	}

	public void OnSelect_MOREINFO()
	{
		StackTrace.GetComponentInChildren<LocText>().text = m_stackTrace;
		StackTrace.SetActive(value: true);
		moreInfoButton.GetComponentInChildren<LocText>().text = UI.CRASHSCREEN.COPYTOCLIPBOARDBUTTON;
		moreInfoButton.ClearOnClick();
		moreInfoButton.onClick += OnSelect_COPYTOCLIPBOARD;
	}

	public void OnSelect_COPYTOCLIPBOARD()
	{
		TextEditor textEditor = new TextEditor();
		textEditor.text = m_stackTrace;
		textEditor.SelectAll();
		textEditor.Copy();
	}

	public void OnSelect_SUBMIT()
	{
		submitButton.GetComponentInChildren<LocText>().text = UI.CRASHSCREEN.REPORTING;
		submitButton.GetComponent<KButton>().isInteractable = false;
		StartCoroutine(WaitForUIUpdateBeforeReporting());
	}

	private IEnumerator WaitForUIUpdateBeforeReporting()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		bool delay = false;
		if (MOST_RECENT_SAVEFILE != null && File.Exists(MOST_RECENT_SAVEFILE))
		{
			delay = true;
			FileInfo info = new FileInfo(MOST_RECENT_SAVEFILE);
			long length = info.Length;
			saveFileInfoLabel.text = Path.GetFileName(MOST_RECENT_SAVEFILE) + " " + length + " bytes";
			uploadSaveDialog.SetActive(value: true);
		}
		if (!delay)
		{
			Submit();
		}
	}

	public void OnSelect_QUIT()
	{
		if (quitAction != null)
		{
			quitAction();
		}
	}

	public void OnSelect_CONTINUE()
	{
		if (continueAction != null)
		{
			continueAction();
		}
	}

	public void OpenRefMessage()
	{
		submitButton.gameObject.SetActive(value: false);
		referenceMessage.SetActive(value: true);
	}

	public string UserMessage()
	{
		return messageInputField.text;
	}

	private void OnSelect_UPLOADSAVE()
	{
		uploadSaveDialog.SetActive(value: false);
		KCrashReporter.MOST_RECENT_SAVEFILE = MOST_RECENT_SAVEFILE;
		Submit();
	}

	private void OnSelect_SKIPUPLOADSAVE()
	{
		uploadSaveDialog.SetActive(value: false);
		KCrashReporter.MOST_RECENT_SAVEFILE = null;
		Submit();
	}

	private void Submit()
	{
		submitAction();
		OpenRefMessage();
	}
}
