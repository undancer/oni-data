using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ScenariosMenu : KModalScreen, SteamUGCService.IClient
{
	public const string TAG_SCENARIO = "scenario";

	public KButton textButton;

	public KButton dismissButton;

	public KButton closeButton;

	public KButton workshopButton;

	public KButton loadScenarioButton;

	[Space]
	public GameObject ugcContainer;

	public GameObject ugcButtonPrefab;

	public LocText noScenariosText;

	public RectTransform contentRoot;

	public RectTransform detailsRoot;

	public LocText scenarioTitle;

	public LocText scenarioDetails;

	private PublishedFileId_t activeItem;

	private List<GameObject> buttons = new List<GameObject>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		dismissButton.onClick += delegate
		{
			Deactivate();
		};
		dismissButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Title").SetText(UI.FRONTEND.OPTIONS_SCREEN.BACK);
		closeButton.onClick += delegate
		{
			Deactivate();
		};
		workshopButton.onClick += delegate
		{
			OnClickOpenWorkshop();
		};
		RebuildScreen();
	}

	private void RebuildScreen()
	{
		foreach (GameObject button in buttons)
		{
			UnityEngine.Object.Destroy(button);
		}
		buttons.Clear();
		RebuildUGCButtons();
	}

	private void RebuildUGCButtons()
	{
		ListPool<SteamUGCService.Mod, ScenariosMenu>.PooledList pooledList = ListPool<SteamUGCService.Mod, ScenariosMenu>.Allocate();
		bool flag = pooledList.Count > 0;
		noScenariosText.gameObject.SetActive(!flag);
		contentRoot.gameObject.SetActive(flag);
		bool flag2 = true;
		if (pooledList.Count != 0)
		{
			for (int i = 0; i < pooledList.Count; i++)
			{
				GameObject gameObject = Util.KInstantiateUI(ugcButtonPrefab, ugcContainer);
				gameObject.name = pooledList[i].title + "_button";
				gameObject.gameObject.SetActive(value: true);
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				component.GetReference<LocText>("Title").SetText(pooledList[i].title);
				Texture2D previewImage = pooledList[i].previewImage;
				if (previewImage != null)
				{
					component.GetReference<Image>("Image").sprite = Sprite.Create(previewImage, new Rect(Vector2.zero, new Vector2(previewImage.width, previewImage.height)), Vector2.one * 0.5f);
				}
				KButton component2 = gameObject.GetComponent<KButton>();
				int index = i;
				PublishedFileId_t item = pooledList[index].fileId;
				component2.onClick += delegate
				{
					ShowDetails(item);
				};
				component2.onDoubleClick += delegate
				{
					LoadScenario(item);
				};
				buttons.Add(gameObject);
				if (item == activeItem)
				{
					flag2 = false;
				}
			}
		}
		if (flag2)
		{
			HideDetails();
		}
		pooledList.Recycle();
	}

	private void LoadScenario(PublishedFileId_t item)
	{
		SteamUGC.GetItemInstallInfo(item, out var punSizeOnDisk, out var pchFolder, 1024u, out var punTimeStamp);
		DebugUtil.LogArgs("LoadScenario", pchFolder, punSizeOnDisk, punTimeStamp);
		System.DateTime lastModified;
		byte[] bytesFromZip = SteamUGCService.GetBytesFromZip(item, new string[1] { ".sav" }, out lastModified);
		string text = Path.Combine(SaveLoader.GetSavePrefix(), "scenario.sav");
		File.WriteAllBytes(text, bytesFromZip);
		SaveLoader.SetActiveSaveFilePath(text);
		Time.timeScale = 0f;
		App.LoadScene("backend");
	}

	private ConfirmDialogScreen GetConfirmDialog()
	{
		KScreen component = KScreenManager.AddChild(base.transform.parent.gameObject, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<KScreen>();
		component.Activate();
		return component.GetComponent<ConfirmDialogScreen>();
	}

	private void ShowDetails(PublishedFileId_t item)
	{
		activeItem = item;
		SteamUGCService.Mod mod = SteamUGCService.Instance.FindMod(item);
		if (mod != null)
		{
			scenarioTitle.text = mod.title;
			scenarioDetails.text = mod.description;
		}
		loadScenarioButton.onClick += delegate
		{
			LoadScenario(item);
		};
		detailsRoot.gameObject.SetActive(value: true);
	}

	private void HideDetails()
	{
		detailsRoot.gameObject.SetActive(value: false);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		SteamUGCService.Instance.AddClient(this);
		HideDetails();
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		SteamUGCService.Instance.RemoveClient(this);
	}

	private void OnClickOpenWorkshop()
	{
		App.OpenWebURL("http://steamcommunity.com/workshop/browse/?appid=457140&requiredtags[]=scenario");
	}

	public void UpdateMods(IEnumerable<PublishedFileId_t> added, IEnumerable<PublishedFileId_t> updated, IEnumerable<PublishedFileId_t> removed, IEnumerable<SteamUGCService.Mod> loaded_previews)
	{
		RebuildScreen();
	}
}
