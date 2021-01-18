using System;
using STRINGS;
using UnityEngine;

public class LoadingOverlay : KModalScreen
{
	private bool loadNextFrame;

	private bool showLoad;

	private System.Action loadCb;

	private static LoadingOverlay instance;

	protected override void OnPrefabInit()
	{
		pause = false;
		fadeIn = false;
		base.OnPrefabInit();
	}

	private void Update()
	{
		if (!loadNextFrame && showLoad)
		{
			loadNextFrame = true;
			showLoad = false;
		}
		else if (loadNextFrame)
		{
			loadNextFrame = false;
			loadCb();
		}
	}

	public static void DestroyInstance()
	{
		instance = null;
	}

	public static void Load(System.Action cb)
	{
		GameObject gameObject = GameObject.Find("/SceneInitializerFE/FrontEndManager");
		if (instance == null)
		{
			instance = Util.KInstantiateUI<LoadingOverlay>(ScreenPrefabs.Instance.loadingOverlay.gameObject, (GameScreenManager.Instance == null) ? gameObject : GameScreenManager.Instance.ssOverlayCanvas);
			instance.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.LOADING);
		}
		if (GameScreenManager.Instance != null)
		{
			instance.transform.SetParent(GameScreenManager.Instance.ssOverlayCanvas.transform);
			instance.transform.SetSiblingIndex(GameScreenManager.Instance.ssOverlayCanvas.transform.childCount - 1);
		}
		else
		{
			instance.transform.SetParent(gameObject.transform);
			instance.transform.SetSiblingIndex(gameObject.transform.childCount - 1);
		}
		instance.loadCb = cb;
		instance.showLoad = true;
		instance.Activate();
	}

	public static void Clear()
	{
		if (instance != null)
		{
			instance.Deactivate();
		}
	}
}
