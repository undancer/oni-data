using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/Plugins/GameScreenManager")]
public class GameScreenManager : KMonoBehaviour
{
	public enum UIRenderTarget
	{
		WorldSpace,
		ScreenSpaceCamera,
		ScreenSpaceOverlay,
		HoverTextScreen,
		ScreenshotModeCamera
	}

	public GameObject ssHoverTextCanvas;

	public GameObject ssCameraCanvas;

	public GameObject ssOverlayCanvas;

	public GameObject worldSpaceCanvas;

	public GameObject screenshotModeCanvas;

	[SerializeField]
	private Color[] uiColors;

	public Image fadePlaneBack;

	public Image fadePlaneFront;

	public static GameScreenManager Instance
	{
		get;
		private set;
	}

	public static Color[] UIColors => Instance.uiColors;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Debug.Assert(Instance == null);
		Instance = this;
	}

	protected override void OnCleanUp()
	{
		Debug.Assert(Instance != null);
		Instance = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public Camera GetCamera(UIRenderTarget target)
	{
		return target switch
		{
			UIRenderTarget.WorldSpace => worldSpaceCanvas.GetComponent<Canvas>().worldCamera, 
			UIRenderTarget.ScreenSpaceOverlay => ssOverlayCanvas.GetComponent<Canvas>().worldCamera, 
			UIRenderTarget.ScreenSpaceCamera => ssCameraCanvas.GetComponent<Canvas>().worldCamera, 
			UIRenderTarget.HoverTextScreen => ssHoverTextCanvas.GetComponent<Canvas>().worldCamera, 
			UIRenderTarget.ScreenshotModeCamera => screenshotModeCanvas.GetComponent<Canvas>().worldCamera, 
			_ => base.gameObject.GetComponent<Canvas>().worldCamera, 
		};
	}

	public void SetCamera(UIRenderTarget target, Camera camera)
	{
		switch (target)
		{
		case UIRenderTarget.WorldSpace:
			worldSpaceCanvas.GetComponent<Canvas>().worldCamera = camera;
			break;
		case UIRenderTarget.ScreenSpaceOverlay:
			ssOverlayCanvas.GetComponent<Canvas>().worldCamera = camera;
			break;
		case UIRenderTarget.ScreenshotModeCamera:
			screenshotModeCanvas.GetComponent<Canvas>().worldCamera = camera;
			break;
		default:
			ssCameraCanvas.GetComponent<Canvas>().worldCamera = camera;
			break;
		}
	}

	public GameObject GetParent(UIRenderTarget target)
	{
		return target switch
		{
			UIRenderTarget.WorldSpace => worldSpaceCanvas, 
			UIRenderTarget.ScreenSpaceOverlay => ssOverlayCanvas, 
			UIRenderTarget.ScreenSpaceCamera => ssCameraCanvas, 
			UIRenderTarget.HoverTextScreen => ssHoverTextCanvas, 
			UIRenderTarget.ScreenshotModeCamera => screenshotModeCanvas, 
			_ => base.gameObject, 
		};
	}

	public GameObject ActivateScreen(GameObject screen, GameObject parent = null, UIRenderTarget target = UIRenderTarget.ScreenSpaceOverlay)
	{
		if (parent == null)
		{
			parent = GetParent(target);
		}
		KScreenManager.AddExistingChild(parent, screen);
		screen.GetComponent<KScreen>().Activate();
		return screen;
	}

	public KScreen InstantiateScreen(GameObject screenPrefab, GameObject parent = null, UIRenderTarget target = UIRenderTarget.ScreenSpaceOverlay)
	{
		if (parent == null)
		{
			parent = GetParent(target);
		}
		return KScreenManager.AddChild(parent, screenPrefab).GetComponent<KScreen>();
	}

	public KScreen StartScreen(GameObject screenPrefab, GameObject parent = null, UIRenderTarget target = UIRenderTarget.ScreenSpaceOverlay)
	{
		if (parent == null)
		{
			parent = GetParent(target);
		}
		KScreen component = KScreenManager.AddChild(parent, screenPrefab).GetComponent<KScreen>();
		component.Activate();
		return component;
	}
}
