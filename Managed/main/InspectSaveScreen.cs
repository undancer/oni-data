using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class InspectSaveScreen : KModalScreen
{
	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton mainSaveBtn;

	[SerializeField]
	private KButton backupBtnPrefab;

	[SerializeField]
	private KButton deleteSaveBtn;

	[SerializeField]
	private GameObject buttonGroup;

	private UIPool<KButton> buttonPool;

	private Dictionary<KButton, string> buttonFileMap = new Dictionary<KButton, string>();

	private ConfirmDialogScreen confirmScreen;

	private string currentPath = "";

	protected override void OnSpawn()
	{
		base.OnSpawn();
		closeButton.onClick += CloseScreen;
		deleteSaveBtn.onClick += DeleteSave;
	}

	private void CloseScreen()
	{
		LoadScreen.Instance.Show();
		Show(show: false);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (!show)
		{
			buttonPool.ClearAll();
			buttonFileMap.Clear();
		}
	}

	public void SetTarget(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			Debug.LogError("The directory path provided is empty.");
			Show(show: false);
			return;
		}
		if (!Directory.Exists(path))
		{
			Debug.LogError("The directory provided does not exist.");
			Show(show: false);
			return;
		}
		if (buttonPool == null)
		{
			buttonPool = new UIPool<KButton>(backupBtnPrefab);
		}
		currentPath = path;
		List<string> list = (from filename in Directory.GetFiles(path)
			where Path.GetExtension(filename).ToLower() == ".sav"
			orderby File.GetLastWriteTime(filename) descending
			select filename).ToList();
		string text = list[0];
		if (File.Exists(text))
		{
			mainSaveBtn.gameObject.SetActive(value: true);
			AddNewSave(mainSaveBtn, text);
		}
		else
		{
			mainSaveBtn.gameObject.SetActive(value: false);
		}
		if (list.Count > 1)
		{
			for (int i = 1; i < list.Count; i++)
			{
				AddNewSave(buttonPool.GetFreeElement(buttonGroup, forceActive: true), list[i]);
			}
		}
		Show();
	}

	private void ConfirmDoAction(string message, System.Action action)
	{
		if (confirmScreen == null)
		{
			confirmScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject);
			confirmScreen.PopupConfirmDialog(message, action, delegate
			{
			});
			confirmScreen.GetComponent<LayoutElement>().ignoreLayout = true;
			confirmScreen.gameObject.SetActive(value: true);
		}
	}

	private void DeleteSave()
	{
		if (string.IsNullOrEmpty(currentPath))
		{
			Debug.LogError("The path provided is not valid and cannot be deleted.");
			return;
		}
		ConfirmDoAction(UI.FRONTEND.LOADSCREEN.CONFIRMDELETE, delegate
		{
			string[] files = Directory.GetFiles(currentPath);
			string[] array = files;
			foreach (string path in array)
			{
				File.Delete(path);
			}
			Directory.Delete(currentPath);
			CloseScreen();
		});
	}

	private void AddNewSave(KButton btn, string file)
	{
	}

	private void ButtonClicked(KButton btn)
	{
		LoadingOverlay.Load(delegate
		{
			Load(buttonFileMap[btn]);
		});
	}

	private void Load(string filename)
	{
		if (Game.Instance != null)
		{
			LoadScreen.ForceStopGame();
		}
		SaveLoader.SetActiveSaveFilePath(filename);
		App.LoadScene("backend");
		Deactivate();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
		{
			CloseScreen();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}
}
