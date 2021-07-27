using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Timelapser")]
public class Timelapser : KMonoBehaviour
{
	private bool screenshotActive;

	private bool screenshotPending;

	private bool previewScreenshot;

	private string previewSaveGamePath = "";

	private bool screenshotToday;

	private List<int> worldsToScreenshot = new List<int>();

	private HashedString activeOverlay;

	private Camera freezeCamera;

	private RenderTexture bufferRenderTexture;

	private Vector3 camPosition;

	private float camSize;

	private bool debugScreenShot;

	private Vector2Int previewScreenshotResolution = new Vector2Int(Grid.WidthInCells * 2, Grid.HeightInCells * 2);

	private const int DEFAULT_SCREENSHOT_INTERVAL = 10;

	private int[] timelapseScreenshotCycles = new int[100]
	{
		1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
		11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
		21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
		31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
		41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
		55, 60, 65, 70, 75, 80, 85, 90, 95, 100,
		110, 120, 130, 140, 150, 160, 170, 180, 190, 200,
		210, 220, 230, 240, 250, 260, 270, 280, 290, 200,
		310, 320, 330, 340, 350, 360, 370, 380, 390, 400,
		410, 420, 430, 440, 450, 460, 470, 480, 490, 500
	};

	public bool CapturingTimelapseScreenshot => screenshotActive;

	public Texture2D freezeTexture { get; private set; }

	private bool timelapseUserEnabled => SaveGame.Instance.TimelapseResolution.x > 0;

	protected override void OnPrefabInit()
	{
		RefreshRenderTextureSize();
		Game.Instance.Subscribe(75424175, RefreshRenderTextureSize);
		freezeCamera = CameraController.Instance.timelapseFreezeCamera;
		if (CycleTimeToScreenshot() > 0f)
		{
			OnNewDay();
		}
		GameClock.Instance.Subscribe(631075836, OnNewDay);
		OnResize();
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(OnResize));
		StartCoroutine(Render());
	}

	private void OnResize()
	{
		if (freezeTexture != null)
		{
			UnityEngine.Object.Destroy(freezeTexture);
		}
		freezeTexture = new Texture2D(Camera.main.pixelWidth, Camera.main.pixelHeight, TextureFormat.ARGB32, mipChain: false);
	}

	private void RefreshRenderTextureSize(object data = null)
	{
		if (previewScreenshot)
		{
			bufferRenderTexture = new RenderTexture(previewScreenshotResolution.x, previewScreenshotResolution.y, 32, RenderTextureFormat.ARGB32);
		}
		else if (timelapseUserEnabled)
		{
			bufferRenderTexture = new RenderTexture(SaveGame.Instance.TimelapseResolution.x, SaveGame.Instance.TimelapseResolution.y, 32, RenderTextureFormat.ARGB32);
		}
	}

	private void OnNewDay(object data = null)
	{
		DebugUtil.LogWarningArgs(worldsToScreenshot.Count == 0, "Timelapse.OnNewDay but worldsToScreenshot is not empty");
		int cycle = GameClock.Instance.GetCycle();
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			if (!worldContainer.IsDiscovered || worldContainer.IsModuleInterior)
			{
				continue;
			}
			if (worldContainer.DiscoveryTimestamp + (float)cycle > (float)timelapseScreenshotCycles[timelapseScreenshotCycles.Length - 1])
			{
				if (worldContainer.DiscoveryTimestamp + (float)(cycle % 10) == 0f)
				{
					screenshotToday = true;
					worldsToScreenshot.Add(worldContainer.id);
				}
				continue;
			}
			for (int i = 0; i < timelapseScreenshotCycles.Length; i++)
			{
				if ((int)worldContainer.DiscoveryTimestamp + cycle == timelapseScreenshotCycles[i])
				{
					screenshotToday = true;
					worldsToScreenshot.Add(worldContainer.id);
				}
			}
		}
	}

	private void Update()
	{
		if (screenshotToday)
		{
			if (CycleTimeToScreenshot() <= 0f || GameClock.Instance.GetCycle() == 0)
			{
				if (!timelapseUserEnabled)
				{
					screenshotToday = false;
					worldsToScreenshot.Clear();
				}
				else if (!PlayerController.Instance.IsDragging())
				{
					CameraController.Instance.ForcePanningState(state: false);
					screenshotToday = false;
					SaveScreenshot();
				}
			}
		}
		else
		{
			screenshotToday = !screenshotPending && worldsToScreenshot.Count > 0;
		}
	}

	private float CycleTimeToScreenshot()
	{
		return 300f - GameClock.Instance.GetTime() % 600f;
	}

	private IEnumerator Render()
	{
		WaitForEndOfFrame wait = new WaitForEndOfFrame();
		while (true)
		{
			yield return wait;
			if (!screenshotPending)
			{
				continue;
			}
			int num = (previewScreenshot ? ClusterManager.Instance.GetStartWorld().id : worldsToScreenshot[0]);
			if (!freezeCamera.enabled)
			{
				freezeTexture.ReadPixels(new Rect(0f, 0f, Camera.main.pixelWidth, Camera.main.pixelHeight), 0, 0);
				freezeTexture.Apply();
				freezeCamera.gameObject.GetComponent<FillRenderTargetEffect>().SetFillTexture(freezeTexture);
				freezeCamera.enabled = true;
				screenshotActive = true;
				RefreshRenderTextureSize();
				DebugHandler.SetTimelapseMode(enabled: true, num);
				SetPostionAndOrtho(num);
				activeOverlay = OverlayScreen.Instance.mode;
				OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID, allowSound: false);
				continue;
			}
			RenderAndPrint(num);
			if (!previewScreenshot)
			{
				worldsToScreenshot.Remove(num);
			}
			freezeCamera.enabled = false;
			DebugHandler.SetTimelapseMode(enabled: false);
			screenshotPending = false;
			previewScreenshot = false;
			screenshotActive = false;
			debugScreenShot = false;
			previewSaveGamePath = "";
			OverlayScreen.Instance.ToggleOverlay(activeOverlay, allowSound: false);
		}
	}

	public void InitialScreenshot()
	{
		worldsToScreenshot.Add(ClusterManager.Instance.GetStartWorld().id);
		SaveScreenshot();
	}

	private void SaveScreenshot()
	{
		screenshotPending = true;
	}

	public void SaveColonyPreview(string saveFileName)
	{
		previewSaveGamePath = saveFileName;
		previewScreenshot = true;
		SaveScreenshot();
	}

	private void SetPostionAndOrtho(int world_id)
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(world_id);
		if (world == null)
		{
			return;
		}
		float num = 0f;
		Camera overlayCamera = CameraController.Instance.overlayCamera;
		camSize = overlayCamera.orthographicSize;
		camPosition = CameraController.Instance.transform.position;
		if (world.IsStartWorld)
		{
			GameObject telepad = GameUtil.GetTelepad(world_id);
			if (telepad == null)
			{
				return;
			}
			Vector3 position = telepad.transform.GetPosition();
			foreach (BuildingComplete item in Components.BuildingCompletes.Items)
			{
				Vector3 position2 = item.transform.GetPosition();
				float num2 = (float)bufferRenderTexture.width / (float)bufferRenderTexture.height;
				Vector3 vector = position - position2;
				num = Mathf.Max(num, vector.x / num2, vector.y);
			}
			num += 10f;
			num = Mathf.Max(num, 18f);
			CameraController.Instance.SetOrthographicsSize(num);
			CameraController.Instance.SetPosition(new Vector3(telepad.transform.position.x, telepad.transform.position.y, CameraController.Instance.transform.position.z));
		}
		else
		{
			CameraController.Instance.SetOrthographicsSize(world.WorldSize.y / 2);
			CameraController.Instance.SetPosition(new Vector3(world.WorldOffset.x + world.WorldSize.x / 2, world.WorldOffset.y + world.WorldSize.y / 2, CameraController.Instance.transform.position.z));
		}
	}

	private void RenderAndPrint(int world_id)
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(world_id);
		if (world == null)
		{
			return;
		}
		if (world.IsStartWorld)
		{
			GameObject telepad = GameUtil.GetTelepad(0);
			if (telepad == null)
			{
				Debug.Log("No telepad present, aborting screenshot.");
				return;
			}
			Vector3 position = telepad.transform.position;
			position.z = CameraController.Instance.transform.position.z;
			CameraController.Instance.SetPosition(position);
		}
		else
		{
			CameraController.Instance.SetPosition(new Vector3(world.WorldOffset.x + world.WorldSize.x / 2, world.WorldOffset.y + world.WorldSize.y / 2, CameraController.Instance.transform.position.z));
		}
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = bufferRenderTexture;
		CameraController.Instance.RenderForTimelapser(ref bufferRenderTexture);
		WriteToPng(bufferRenderTexture, world.GetComponent<ClusterGridEntity>().Name);
		CameraController.Instance.SetOrthographicsSize(camSize);
		CameraController.Instance.SetPosition(camPosition);
		RenderTexture.active = active;
	}

	public void WriteToPng(RenderTexture renderTex, string world_name = "")
	{
		Texture2D texture2D = new Texture2D(renderTex.width, renderTex.height, TextureFormat.ARGB32, mipChain: false);
		texture2D.ReadPixels(new Rect(0f, 0f, renderTex.width, renderTex.height), 0, 0);
		texture2D.Apply();
		byte[] bytes = texture2D.EncodeToPNG();
		UnityEngine.Object.Destroy(texture2D);
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string text = Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string path = RetireColonyUtility.StripInvalidCharacters(SaveGame.Instance.BaseName);
		if (!previewScreenshot)
		{
			string text2 = Path.Combine(text, path);
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			string path2 = text2;
			if (!world_name.IsNullOrWhiteSpace())
			{
				path2 = Path.Combine(path2, world_name);
				if (!Directory.Exists(path2))
				{
					Directory.CreateDirectory(path2);
				}
				path2 = Path.Combine(path2, world_name);
			}
			else
			{
				path2 = Path.Combine(path2, path);
			}
			DebugUtil.LogArgs("Saving screenshot to", path2);
			string text3 = "0000.##";
			path2 = path2 + "_cycle_" + GameClock.Instance.GetCycle().ToString(text3);
			if (debugScreenShot)
			{
				path2 = path2 + "_" + System.DateTime.Now.Day + "-" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Hour + "-" + System.DateTime.Now.Minute + "-" + System.DateTime.Now.Second;
			}
			File.WriteAllBytes(path2 + ".png", bytes);
		}
		else
		{
			string path3 = previewSaveGamePath;
			path3 = Path.ChangeExtension(path3, ".png");
			DebugUtil.LogArgs("Saving screenshot to", path3);
			File.WriteAllBytes(path3, bytes);
		}
	}
}
