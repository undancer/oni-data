using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

[AddComponentMenu("KMonoBehaviour/scripts/CameraController")]
public class CameraController : KMonoBehaviour, IInputHandler
{
	public class Tuning : TuningData<Tuning>
	{
		public float maxOrthographicSizeDebug;

		public float cinemaZoomFactor = 100f;

		public float cinemaPanFactor = 50f;

		public float cinemaZoomToFactor = 100f;

		public float cinemaPanToFactor = 50f;

		public float targetZoomEasingFactor = 400f;

		public float targetPanEasingFactor = 100f;
	}

	public const float DEFAULT_MAX_ORTHO_SIZE = 20f;

	public float MAX_Y_SCALE = 1.1f;

	public LocText infoText;

	private const float FIXED_Z = -100f;

	public bool FreeCameraEnabled;

	public float zoomSpeed;

	public float minOrthographicSize;

	public float zoomFactor;

	public float keyPanningSpeed;

	public float keyPanningEasing;

	public Texture2D dayColourCube;

	public Texture2D nightColourCube;

	public Material LightBufferMaterial;

	public Material LightCircleOverlay;

	public Material LightConeOverlay;

	public Transform followTarget;

	public Vector3 followTargetPos;

	public GridVisibleArea VisibleArea = new GridVisibleArea();

	private float maxOrthographicSize = 20f;

	private float overrideZoomSpeed;

	private bool panning;

	private Vector3 keyPanDelta;

	[SerializeField]
	private LayerMask timelapseCameraCullingMask;

	[SerializeField]
	private LayerMask timelapseOverlayCameraCullingMask;

	private bool userCameraControlDisabled;

	private bool panLeft;

	private bool panRight;

	private bool panUp;

	private bool panDown;

	[NonSerialized]
	public Camera baseCamera;

	[NonSerialized]
	public Camera overlayCamera;

	[NonSerialized]
	public Camera overlayNoDepthCamera;

	[NonSerialized]
	public Camera uiCamera;

	[NonSerialized]
	public Camera lightBufferCamera;

	[NonSerialized]
	public Camera simOverlayCamera;

	[NonSerialized]
	public Camera infraredCamera;

	[NonSerialized]
	public Camera timelapseFreezeCamera;

	public List<Camera> cameras = new List<Camera>();

	private MultipleRenderTarget mrt;

	public SoundCuller soundCuller;

	private bool cinemaCamEnabled;

	private bool cinemaToggleLock;

	private bool cinemaToggleEasing;

	private bool cinemaPanLeft;

	private bool cinemaPanRight;

	private bool cinemaPanUp;

	private bool cinemaPanDown;

	private bool cinemaZoomIn;

	private bool cinemaZoomOut;

	private int cinemaZoomSpeed = 10;

	private float cinemaEasing = 0.05f;

	private float cinemaZoomVelocity;

	private Coroutine activeFadeRoutine;

	public string handlerName => base.gameObject.name;

	public KInputHandler inputHandler
	{
		get;
		set;
	}

	public float targetOrthographicSize
	{
		get;
		private set;
	}

	public bool isTargetPosSet
	{
		get;
		set;
	}

	public Vector3 targetPos
	{
		get;
		private set;
	}

	public bool DisableUserCameraControl
	{
		get
		{
			return userCameraControlDisabled;
		}
		set
		{
			userCameraControlDisabled = value;
			if (userCameraControlDisabled)
			{
				panning = false;
				panLeft = false;
				panRight = false;
				panUp = false;
				panDown = false;
			}
		}
	}

	public static CameraController Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public void ToggleColouredOverlayView(bool enabled)
	{
		mrt.ToggleColouredOverlayView(enabled);
	}

	protected override void OnPrefabInit()
	{
		Util.Reset(base.transform);
		base.transform.SetLocalPosition(new Vector3(Grid.WidthInMeters / 2f, Grid.HeightInMeters / 2f, -100f));
		targetOrthographicSize = maxOrthographicSize;
		Instance = this;
		DisableUserCameraControl = false;
		baseCamera = CopyCamera(Camera.main, "baseCamera");
		mrt = baseCamera.gameObject.AddComponent<MultipleRenderTarget>();
		mrt.onSetupComplete += OnMRTSetupComplete;
		baseCamera.gameObject.AddComponent<LightBufferCompositor>();
		baseCamera.transparencySortMode = TransparencySortMode.Orthographic;
		baseCamera.transform.parent = base.transform;
		Util.Reset(baseCamera.transform);
		int mask = LayerMask.GetMask("PlaceWithDepth", "Overlay");
		int mask2 = LayerMask.GetMask("Construction");
		cameras.Add(baseCamera);
		baseCamera.cullingMask &= ~mask;
		baseCamera.cullingMask |= mask2;
		baseCamera.tag = "Untagged";
		baseCamera.gameObject.AddComponent<CameraRenderTexture>().TextureName = "_LitTex";
		infraredCamera = CopyCamera(baseCamera, "Infrared");
		infraredCamera.cullingMask = 0;
		infraredCamera.clearFlags = CameraClearFlags.Color;
		infraredCamera.depth = baseCamera.depth - 1f;
		infraredCamera.transform.parent = base.transform;
		infraredCamera.gameObject.AddComponent<Infrared>();
		simOverlayCamera = CopyCamera(baseCamera, "SimOverlayCamera");
		simOverlayCamera.cullingMask = LayerMask.GetMask("SimDebugView");
		simOverlayCamera.clearFlags = CameraClearFlags.Color;
		simOverlayCamera.depth = baseCamera.depth + 1f;
		simOverlayCamera.transform.parent = base.transform;
		simOverlayCamera.gameObject.AddComponent<CameraRenderTexture>().TextureName = "_SimDebugViewTex";
		overlayCamera = Camera.main;
		overlayCamera.name = "Overlay";
		overlayCamera.cullingMask = mask | mask2;
		overlayCamera.clearFlags = CameraClearFlags.Nothing;
		overlayCamera.transform.parent = base.transform;
		overlayCamera.depth = baseCamera.depth + 3f;
		overlayCamera.transform.SetLocalPosition(Vector3.zero);
		overlayCamera.transform.localRotation = Quaternion.identity;
		overlayCamera.renderingPath = RenderingPath.Forward;
		overlayCamera.allowHDR = false;
		overlayCamera.tag = "Untagged";
		overlayCamera.gameObject.AddComponent<CameraReferenceTexture>().referenceCamera = baseCamera;
		ColorCorrectionLookup component = overlayCamera.GetComponent<ColorCorrectionLookup>();
		component.Convert(dayColourCube, "");
		component.Convert2(nightColourCube, "");
		cameras.Add(overlayCamera);
		lightBufferCamera = CopyCamera(overlayCamera, "Light Buffer");
		lightBufferCamera.clearFlags = CameraClearFlags.Color;
		lightBufferCamera.cullingMask = LayerMask.GetMask("Lights");
		lightBufferCamera.depth = baseCamera.depth - 1f;
		lightBufferCamera.transform.parent = base.transform;
		lightBufferCamera.transform.SetLocalPosition(Vector3.zero);
		lightBufferCamera.rect = new Rect(0f, 0f, 1f, 1f);
		LightBuffer lightBuffer = lightBufferCamera.gameObject.AddComponent<LightBuffer>();
		lightBuffer.Material = LightBufferMaterial;
		lightBuffer.CircleMaterial = LightCircleOverlay;
		lightBuffer.ConeMaterial = LightConeOverlay;
		overlayNoDepthCamera = CopyCamera(overlayCamera, "overlayNoDepth");
		int mask3 = LayerMask.GetMask("Overlay", "Place");
		baseCamera.cullingMask &= ~mask3;
		overlayNoDepthCamera.clearFlags = CameraClearFlags.Depth;
		overlayNoDepthCamera.cullingMask = mask3;
		overlayNoDepthCamera.transform.parent = base.transform;
		overlayNoDepthCamera.transform.SetLocalPosition(Vector3.zero);
		overlayNoDepthCamera.depth = baseCamera.depth + 4f;
		overlayNoDepthCamera.tag = "MainCamera";
		overlayNoDepthCamera.gameObject.AddComponent<NavPathDrawer>();
		uiCamera = CopyCamera(overlayCamera, "uiCamera");
		uiCamera.clearFlags = CameraClearFlags.Depth;
		uiCamera.cullingMask = LayerMask.GetMask("UI");
		uiCamera.transform.parent = base.transform;
		uiCamera.transform.SetLocalPosition(Vector3.zero);
		uiCamera.depth = baseCamera.depth + 5f;
		timelapseFreezeCamera = CopyCamera(uiCamera, "timelapseFreezeCamera");
		timelapseFreezeCamera.depth = uiCamera.depth + 3f;
		timelapseFreezeCamera.gameObject.AddComponent<FillRenderTargetEffect>();
		timelapseFreezeCamera.enabled = false;
		Camera camera = CloneCamera(overlayCamera, "timelapseCamera");
		Timelapser timelapser = camera.gameObject.AddComponent<Timelapser>();
		camera.transparencySortMode = TransparencySortMode.Orthographic;
		Game.Instance.timelapser = timelapser;
		GameScreenManager.Instance.SetCamera(GameScreenManager.UIRenderTarget.ScreenSpaceCamera, uiCamera);
		GameScreenManager.Instance.SetCamera(GameScreenManager.UIRenderTarget.WorldSpace, uiCamera);
		GameScreenManager.Instance.SetCamera(GameScreenManager.UIRenderTarget.ScreenshotModeCamera, uiCamera);
		infoText = GameScreenManager.Instance.screenshotModeCanvas.GetComponentInChildren<LocText>();
	}

	public static Camera CloneCamera(Camera camera, string name)
	{
		Camera camera2 = new GameObject
		{
			name = name
		}.AddComponent<Camera>();
		camera2.CopyFrom(camera);
		return camera2;
	}

	private Camera CopyCamera(Camera camera, string name)
	{
		Camera camera2 = CloneCamera(camera, name);
		cameras.Add(camera2);
		return camera2;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Restore();
	}

	public void FadeOut(float targetPercentage = 1f, float speed = 1f)
	{
		if (activeFadeRoutine != null)
		{
			StopCoroutine(activeFadeRoutine);
		}
		activeFadeRoutine = StartCoroutine(FadeToBlack(targetPercentage, speed));
	}

	public void FadeIn(float targetPercentage = 0f, float speed = 1f)
	{
		if (activeFadeRoutine != null)
		{
			StopCoroutine(activeFadeRoutine);
		}
		activeFadeRoutine = StartCoroutine(FadeInFromBlack(targetPercentage, speed));
	}

	public void SetWorldInteractive(bool state)
	{
		GameScreenManager.Instance.fadePlane.raycastTarget = !state;
	}

	private IEnumerator FadeToBlack(float targetBlackPercent = 1f, float speed = 1f)
	{
		Mathf.Max(0f, GameScreenManager.Instance.fadePlane.color.a);
		float duration = 1f;
		for (float i = 0f; i <= duration; i += Time.unscaledDeltaTime * speed)
		{
			float a = Mathf.Max(Mathf.Min(i / duration, targetBlackPercent), GameScreenManager.Instance.fadePlane.color.a);
			GameScreenManager.Instance.fadePlane.color = new Color(0f, 0f, 0f, a);
			yield return 0;
		}
		GameScreenManager.Instance.fadePlane.color = new Color(0f, 0f, 0f, targetBlackPercent);
		activeFadeRoutine = null;
		yield return 0;
	}

	private IEnumerator FadeInFromBlack(float targetBlackPercent = 0f, float speed = 1f)
	{
		Mathf.Min(1f, GameScreenManager.Instance.fadePlane.color.a);
		float duration = 1f;
		for (float i = 0f; i <= duration; i += Time.unscaledDeltaTime * speed)
		{
			float a = Mathf.Min(Mathf.Max(1f - i / duration, targetBlackPercent), GameScreenManager.Instance.fadePlane.color.a);
			GameScreenManager.Instance.fadePlane.color = new Color(0f, 0f, 0f, a);
			yield return 0;
		}
		GameScreenManager.Instance.fadePlane.color = new Color(0f, 0f, 0f, targetBlackPercent);
		activeFadeRoutine = null;
		yield return 0;
	}

	public void EnableFreeCamera(bool enable)
	{
		FreeCameraEnabled = enable;
		SetInfoText("Screenshot Mode (ESC to exit)");
	}

	private static bool WithinInputField()
	{
		UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
		if (current == null)
		{
			return false;
		}
		bool result = false;
		if (current.currentSelectedGameObject != null && (current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null || current.currentSelectedGameObject.GetComponent<InputField>() != null))
		{
			result = true;
		}
		return result;
	}

	private void SetInfoText(string text)
	{
		infoText.text = text;
		Color color = infoText.color;
		color.a = 0.5f;
		infoText.color = color;
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (e.Consumed || DisableUserCameraControl || WithinInputField() || (SaveGame.Instance != null && SaveGame.Instance.GetComponent<UserNavigation>().Handle(e)))
		{
			return;
		}
		if (e.TryConsume(Action.ZoomIn))
		{
			float a = targetOrthographicSize - zoomFactor * targetOrthographicSize;
			targetOrthographicSize = Mathf.Max(a, minOrthographicSize);
			overrideZoomSpeed = 0f;
			isTargetPosSet = false;
		}
		else if (e.TryConsume(Action.ZoomOut))
		{
			float a2 = targetOrthographicSize + zoomFactor * targetOrthographicSize;
			targetOrthographicSize = Mathf.Min(a2, FreeCameraEnabled ? TuningData<Tuning>.Get().maxOrthographicSizeDebug : maxOrthographicSize);
			overrideZoomSpeed = 0f;
			isTargetPosSet = false;
		}
		else if (e.TryConsume(Action.MouseMiddle) || e.IsAction(Action.MouseRight))
		{
			panning = true;
			overrideZoomSpeed = 0f;
			isTargetPosSet = false;
		}
		else if (FreeCameraEnabled && e.TryConsume(Action.CinemaCamEnable))
		{
			cinemaCamEnabled = !cinemaCamEnabled;
			DebugUtil.LogArgs("Cinema Cam Enabled ", cinemaCamEnabled);
			SetInfoText(cinemaCamEnabled ? "Cinema Cam Enabled" : "Cinema Cam Disabled");
		}
		else if (FreeCameraEnabled && cinemaCamEnabled)
		{
			if (e.TryConsume(Action.CinemaToggleLock))
			{
				cinemaToggleLock = !cinemaToggleLock;
				DebugUtil.LogArgs("Cinema Toggle Lock ", cinemaToggleLock);
				SetInfoText(cinemaToggleLock ? "Cinema Input Lock ON" : "Cinema Input Lock OFF");
			}
			else if (e.TryConsume(Action.CinemaToggleEasing))
			{
				cinemaToggleEasing = !cinemaToggleEasing;
				DebugUtil.LogArgs("Cinema Toggle Easing ", cinemaToggleEasing);
				SetInfoText(cinemaToggleEasing ? "Cinema Easing ON" : "Cinema Easing OFF");
			}
			else if (e.TryConsume(Action.CinemaPanLeft))
			{
				cinemaPanLeft = !cinemaToggleLock || !cinemaPanLeft;
				cinemaPanRight = false;
			}
			else if (e.TryConsume(Action.CinemaPanRight))
			{
				cinemaPanRight = !cinemaToggleLock || !cinemaPanRight;
				cinemaPanLeft = false;
			}
			else if (e.TryConsume(Action.CinemaPanUp))
			{
				cinemaPanUp = !cinemaToggleLock || !cinemaPanUp;
				cinemaPanDown = false;
			}
			else if (e.TryConsume(Action.CinemaPanDown))
			{
				cinemaPanDown = !cinemaToggleLock || !cinemaPanDown;
				cinemaPanUp = false;
			}
			else if (e.TryConsume(Action.CinemaZoomIn))
			{
				cinemaZoomIn = !cinemaToggleLock || !cinemaZoomIn;
				cinemaZoomOut = false;
			}
			else if (e.TryConsume(Action.CinemaZoomOut))
			{
				cinemaZoomOut = !cinemaToggleLock || !cinemaZoomOut;
				cinemaZoomIn = false;
			}
			else if (e.TryConsume(Action.CinemaZoomSpeedPlus))
			{
				cinemaZoomSpeed++;
				DebugUtil.LogArgs("Cinema Zoom Speed ", cinemaZoomSpeed);
				SetInfoText("Cinema Zoom Speed: " + cinemaZoomSpeed);
			}
			else if (e.TryConsume(Action.CinemaZoomSpeedMinus))
			{
				cinemaZoomSpeed--;
				DebugUtil.LogArgs("Cinema Zoom Speed ", cinemaZoomSpeed);
				SetInfoText("Cinema Zoom Speed: " + cinemaZoomSpeed);
			}
		}
		else if (e.TryConsume(Action.PanLeft))
		{
			panLeft = true;
		}
		else if (e.TryConsume(Action.PanRight))
		{
			panRight = true;
		}
		else if (e.TryConsume(Action.PanUp))
		{
			panUp = true;
		}
		else if (e.TryConsume(Action.PanDown))
		{
			panDown = true;
		}
		if (!e.Consumed && OverlayMenu.Instance != null)
		{
			OverlayMenu.Instance.OnKeyDown(e);
		}
	}

	public void OnKeyUp(KButtonEvent e)
	{
		if (DisableUserCameraControl || WithinInputField())
		{
			return;
		}
		if (e.TryConsume(Action.MouseMiddle) || e.IsAction(Action.MouseRight))
		{
			panning = false;
		}
		else if (FreeCameraEnabled && cinemaCamEnabled)
		{
			if (e.TryConsume(Action.CinemaPanLeft))
			{
				cinemaPanLeft = cinemaToggleLock && cinemaPanLeft;
			}
			else if (e.TryConsume(Action.CinemaPanRight))
			{
				cinemaPanRight = cinemaToggleLock && cinemaPanRight;
			}
			else if (e.TryConsume(Action.CinemaPanUp))
			{
				cinemaPanUp = cinemaToggleLock && cinemaPanUp;
			}
			else if (e.TryConsume(Action.CinemaPanDown))
			{
				cinemaPanDown = cinemaToggleLock && cinemaPanDown;
			}
			else if (e.TryConsume(Action.CinemaZoomIn))
			{
				cinemaZoomIn = cinemaToggleLock && cinemaZoomIn;
			}
			else if (e.TryConsume(Action.CinemaZoomOut))
			{
				cinemaZoomOut = cinemaToggleLock && cinemaZoomOut;
			}
		}
		else if (e.TryConsume(Action.CameraHome))
		{
			CameraGoHome();
		}
		else if (e.TryConsume(Action.PanLeft))
		{
			panLeft = false;
		}
		else if (e.TryConsume(Action.PanRight))
		{
			panRight = false;
		}
		else if (e.TryConsume(Action.PanUp))
		{
			panUp = false;
		}
		else if (e.TryConsume(Action.PanDown))
		{
			panDown = false;
		}
	}

	public void ForcePanningState(bool state)
	{
		panning = false;
	}

	public void CameraGoHome(float speed = 2f)
	{
		GameObject telepad = GameUtil.GetTelepad();
		if (telepad != null)
		{
			Vector3 pos = new Vector3(telepad.transform.GetPosition().x, telepad.transform.GetPosition().y + 1f, base.transform.GetPosition().z);
			SetTargetPos(pos, 10f, playSound: true);
			SetOverrideZoomSpeed(speed);
		}
	}

	public void CameraGoTo(Vector3 pos, float speed = 2f, bool playSound = true)
	{
		pos.z = base.transform.GetPosition().z;
		SetTargetPos(pos, 10f, playSound);
		SetOverrideZoomSpeed(speed);
	}

	public void SnapTo(Vector3 pos)
	{
		ClearFollowTarget();
		pos.z = -100f;
		base.transform.SetPosition(pos);
		keyPanDelta = Vector3.zero;
		SetOrthographicsSize(targetOrthographicSize);
	}

	public void SetOverrideZoomSpeed(float tempZoomSpeed)
	{
		overrideZoomSpeed = tempZoomSpeed;
	}

	public void SetTargetPos(Vector3 pos, float orthographic_size, bool playSound)
	{
		ClearFollowTarget();
		if (playSound && !isTargetPosSet)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Click_Notification"));
		}
		isTargetPosSet = true;
		pos.z = -100f;
		targetPos = pos;
		targetOrthographicSize = orthographic_size;
		PlayerController.Instance.CancelDragging();
	}

	public void SetMaxOrthographicSize(float size)
	{
		maxOrthographicSize = size;
	}

	public void SetOrthographicsSize(float size)
	{
		for (int i = 0; i < cameras.Count; i++)
		{
			cameras[i].orthographicSize = size;
		}
	}

	public void SetPosition(Vector3 pos)
	{
		base.transform.SetPosition(pos);
	}

	private Vector3 PointUnderCursor(Vector3 mousePos, Camera cam)
	{
		Ray ray = cam.ScreenPointToRay(mousePos);
		Vector3 direction = ray.direction;
		Vector3 b = direction * Mathf.Abs(cam.transform.GetPosition().z / direction.z);
		return ray.origin + b;
	}

	private void CinemaCamUpdate()
	{
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		Camera main = Camera.main;
		Vector3 localPosition = base.transform.GetLocalPosition();
		float num = Mathf.Pow(cinemaZoomSpeed, 3f);
		if (cinemaZoomIn)
		{
			overrideZoomSpeed = (0f - num) / TuningData<Tuning>.Get().cinemaZoomFactor;
			isTargetPosSet = false;
		}
		else if (cinemaZoomOut)
		{
			overrideZoomSpeed = num / TuningData<Tuning>.Get().cinemaZoomFactor;
			isTargetPosSet = false;
		}
		else
		{
			overrideZoomSpeed = 0f;
		}
		if (cinemaToggleEasing)
		{
			cinemaZoomVelocity += (overrideZoomSpeed - cinemaZoomVelocity) * cinemaEasing;
		}
		else
		{
			cinemaZoomVelocity = overrideZoomSpeed;
		}
		if (cinemaZoomVelocity != 0f)
		{
			SetOrthographicsSize(main.orthographicSize + cinemaZoomVelocity * unscaledDeltaTime * (main.orthographicSize / 20f));
			targetOrthographicSize = main.orthographicSize;
		}
		float num2 = num / TuningData<Tuning>.Get().cinemaZoomToFactor;
		float num3 = keyPanningSpeed / 20f * main.orthographicSize;
		float num4 = num3 * (num / TuningData<Tuning>.Get().cinemaPanToFactor);
		if (!isTargetPosSet && targetOrthographicSize != main.orthographicSize)
		{
			float t = Mathf.Min(num2 * unscaledDeltaTime, 0.1f);
			SetOrthographicsSize(Mathf.Lerp(main.orthographicSize, targetOrthographicSize, t));
		}
		Vector3 b = Vector3.zero;
		float num5 = 0f;
		if (isTargetPosSet)
		{
			float num6 = cinemaEasing * TuningData<Tuning>.Get().targetZoomEasingFactor;
			float num7 = cinemaEasing * TuningData<Tuning>.Get().targetPanEasingFactor;
			float num8 = targetOrthographicSize - main.orthographicSize;
			Vector3 vector = targetPos - localPosition;
			float num9;
			float num10;
			if (!cinemaToggleEasing)
			{
				num9 = num2 * unscaledDeltaTime;
				num10 = num4 * unscaledDeltaTime;
			}
			else
			{
				DebugUtil.LogArgs("Min zoom of:", num2 * unscaledDeltaTime, Mathf.Abs(num8) * num6 * unscaledDeltaTime);
				num9 = Mathf.Min(num2 * unscaledDeltaTime, Mathf.Abs(num8) * num6 * unscaledDeltaTime);
				DebugUtil.LogArgs("Min pan of:", num4 * unscaledDeltaTime, vector.magnitude * num7 * unscaledDeltaTime);
				num10 = Mathf.Min(num4 * unscaledDeltaTime, vector.magnitude * num7 * unscaledDeltaTime);
			}
			num5 = ((!(Mathf.Abs(num8) < num9)) ? (Mathf.Sign(num8) * num9) : num8);
			b = ((!(vector.magnitude < num10)) ? (vector.normalized * num10) : vector);
			if (Mathf.Abs(num5) < 0.001f && b.magnitude < 0.001f)
			{
				isTargetPosSet = false;
				num5 = num8;
				b = vector;
			}
			SetOrthographicsSize(main.orthographicSize + num5 * (main.orthographicSize / 20f));
		}
		if (!PlayerController.Instance.IsDragging())
		{
			panning = false;
		}
		Vector3 b2 = Vector3.zero;
		if (panning)
		{
			b2 = -PlayerController.Instance.GetWorldDragDelta();
			isTargetPosSet = false;
			if (b2.magnitude > 0f)
			{
				ClearFollowTarget();
			}
			keyPanDelta = Vector3.zero;
		}
		else
		{
			float num11 = num / TuningData<Tuning>.Get().cinemaPanFactor;
			Vector3 zero = Vector3.zero;
			if (cinemaPanLeft)
			{
				ClearFollowTarget();
				zero.x = (0f - num3) * num11;
				isTargetPosSet = false;
			}
			if (cinemaPanRight)
			{
				ClearFollowTarget();
				zero.x = num3 * num11;
				isTargetPosSet = false;
			}
			if (cinemaPanUp)
			{
				ClearFollowTarget();
				zero.y = num3 * num11;
				isTargetPosSet = false;
			}
			if (cinemaPanDown)
			{
				ClearFollowTarget();
				zero.y = (0f - num3) * num11;
				isTargetPosSet = false;
			}
			if (cinemaToggleEasing)
			{
				keyPanDelta += (zero - keyPanDelta) * cinemaEasing;
			}
			else
			{
				keyPanDelta = zero;
			}
		}
		Vector3 vector2 = localPosition + b + b2 + keyPanDelta * unscaledDeltaTime;
		if (followTarget != null)
		{
			vector2.x = followTargetPos.x;
			vector2.y = followTargetPos.y;
		}
		vector2.z = -100f;
		if ((double)(vector2 - base.transform.GetLocalPosition()).magnitude > 0.001)
		{
			base.transform.SetLocalPosition(vector2);
		}
	}

	private void NormalCamUpdate()
	{
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		Camera main = Camera.main;
		float num = ((overrideZoomSpeed != 0f) ? overrideZoomSpeed : zoomSpeed);
		Vector3 localPosition = base.transform.GetLocalPosition();
		Vector3 vector = ((overrideZoomSpeed != 0f) ? new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f) : KInputManager.GetMousePos());
		Vector3 position = PointUnderCursor(vector, main);
		Vector3 position2 = main.ScreenToViewportPoint(vector);
		float num2 = keyPanningSpeed / 20f * main.orthographicSize;
		float t = Mathf.Min(num * unscaledDeltaTime, 0.1f);
		SetOrthographicsSize(Mathf.Lerp(main.orthographicSize, targetOrthographicSize, t));
		base.transform.SetLocalPosition(localPosition);
		Vector3 position3 = main.WorldToViewportPoint(position);
		position2.z = position3.z;
		Vector3 b = main.ViewportToWorldPoint(position3) - main.ViewportToWorldPoint(position2);
		if (isTargetPosSet)
		{
			b = Vector3.Lerp(localPosition, targetPos, num * unscaledDeltaTime) - localPosition;
			if (b.magnitude < 0.001f)
			{
				isTargetPosSet = false;
				b = targetPos - localPosition;
			}
		}
		if (!PlayerController.Instance.IsDragging())
		{
			panning = false;
		}
		Vector3 b2 = Vector3.zero;
		if (panning)
		{
			b2 = -PlayerController.Instance.GetWorldDragDelta();
			isTargetPosSet = false;
		}
		Vector3 vector2 = localPosition + b + b2;
		if (panning)
		{
			if (b2.magnitude > 0f)
			{
				ClearFollowTarget();
			}
			keyPanDelta = Vector3.zero;
		}
		else if (!DisableUserCameraControl)
		{
			if (panLeft)
			{
				ClearFollowTarget();
				keyPanDelta.x -= num2;
				isTargetPosSet = false;
				overrideZoomSpeed = 0f;
			}
			if (panRight)
			{
				ClearFollowTarget();
				keyPanDelta.x += num2;
				isTargetPosSet = false;
				overrideZoomSpeed = 0f;
			}
			if (panUp)
			{
				ClearFollowTarget();
				keyPanDelta.y += num2;
				isTargetPosSet = false;
				overrideZoomSpeed = 0f;
			}
			if (panDown)
			{
				ClearFollowTarget();
				keyPanDelta.y -= num2;
				isTargetPosSet = false;
				overrideZoomSpeed = 0f;
			}
			Vector3 vector3 = new Vector3(Mathf.Lerp(0f, keyPanDelta.x, unscaledDeltaTime * keyPanningEasing), Mathf.Lerp(0f, keyPanDelta.y, unscaledDeltaTime * keyPanningEasing), 0f);
			keyPanDelta -= vector3;
			vector2.x += vector3.x;
			vector2.y += vector3.y;
		}
		if (followTarget != null)
		{
			vector2.x = followTargetPos.x;
			vector2.y = followTargetPos.y;
		}
		vector2.z = -100f;
		if ((double)(vector2 - base.transform.GetLocalPosition()).magnitude > 0.001)
		{
			base.transform.SetLocalPosition(vector2);
		}
	}

	private void Update()
	{
		if (!Game.Instance.timelapser.CapturingTimelapseScreenshot)
		{
			if (FreeCameraEnabled && cinemaCamEnabled)
			{
				CinemaCamUpdate();
			}
			else
			{
				NormalCamUpdate();
			}
		}
		if (infoText.color.a > 0f)
		{
			Color color = infoText.color;
			color.a = Mathf.Max(0f, infoText.color.a - Time.unscaledDeltaTime * 0.5f);
			infoText.color = color;
		}
		ConstrainToWorld();
		Vector3 vector = PointUnderCursor(KInputManager.GetMousePos(), Camera.main);
		Shader.SetGlobalVector("_WorldCameraPos", new Vector4(base.transform.GetPosition().x, base.transform.GetPosition().y, base.transform.GetPosition().z, Camera.main.orthographicSize));
		Shader.SetGlobalVector("_WorldCursorPos", new Vector4(vector.x, vector.y, 0f, 0f));
		VisibleArea.Update();
		soundCuller = SoundCuller.CreateCuller();
	}

	private Vector3 GetFollowPos()
	{
		if (followTarget != null)
		{
			Vector3 result = followTarget.transform.GetPosition();
			KAnimControllerBase component = followTarget.GetComponent<KAnimControllerBase>();
			if (component != null)
			{
				result = component.GetWorldPivot();
			}
			return result;
		}
		return Vector3.zero;
	}

	private void ConstrainToWorld()
	{
		if (!Game.Instance.IsLoading() && !FreeCameraEnabled)
		{
			Camera main = Camera.main;
			Ray ray = main.ViewportPointToRay(Vector3.zero);
			Ray ray2 = main.ViewportPointToRay(Vector3.one);
			float distance = Mathf.Abs(ray.origin.z / ray.direction.z);
			float distance2 = Mathf.Abs(ray2.origin.z / ray2.direction.z);
			Vector3 point = ray.GetPoint(distance);
			Vector3 point2 = ray2.GetPoint(distance2);
			if (!(point2.x - point.x > Grid.WidthInMeters) && !(point2.y - point.y > Grid.HeightInMeters))
			{
				Vector3 b = base.transform.GetPosition() - ray.origin;
				Vector3 vector = point;
				vector.x = Mathf.Max(0f, vector.x);
				vector.y = Mathf.Max(0f, vector.y);
				ray.origin = vector;
				ray.direction = -ray.direction;
				vector = ray.GetPoint(distance);
				base.transform.SetPosition(vector + b);
				b = base.transform.GetPosition() - ray2.origin;
				vector = point2;
				vector.x = Mathf.Min(Grid.WidthInMeters, vector.x);
				vector.y = Mathf.Min(Grid.HeightInMeters * MAX_Y_SCALE, vector.y);
				ray2.origin = vector;
				ray2.direction = -ray2.direction;
				vector = ray2.GetPoint(distance2);
				Vector3 position = vector + b;
				position.z = -100f;
				base.transform.SetPosition(position);
			}
		}
	}

	public void Save(BinaryWriter writer)
	{
		writer.Write(base.transform.GetPosition());
		writer.Write(base.transform.localScale);
		writer.Write(base.transform.rotation);
		writer.Write(targetOrthographicSize);
		CameraSaveData.position = base.transform.GetPosition();
		CameraSaveData.localScale = base.transform.localScale;
		CameraSaveData.rotation = base.transform.rotation;
	}

	private void Restore()
	{
		if (CameraSaveData.valid)
		{
			int cell = Grid.PosToCell(CameraSaveData.position);
			if (Grid.IsValidCell(cell) && !Grid.IsVisible(cell))
			{
				Debug.LogWarning("Resetting Camera Position... camera was saved in an undiscovered area of the map.");
				CameraGoHome();
				return;
			}
			base.transform.SetPosition(CameraSaveData.position);
			base.transform.localScale = CameraSaveData.localScale;
			base.transform.rotation = CameraSaveData.rotation;
			targetOrthographicSize = Mathf.Clamp(CameraSaveData.orthographicsSize, minOrthographicSize, FreeCameraEnabled ? TuningData<Tuning>.Get().maxOrthographicSizeDebug : maxOrthographicSize);
			SnapTo(base.transform.GetPosition());
		}
	}

	private void OnMRTSetupComplete(Camera cam)
	{
		cameras.Add(cam);
	}

	public bool IsAudibleSound(Vector2 pos)
	{
		return soundCuller.IsAudible(pos);
	}

	public bool IsAudibleSound(Vector3 pos, HashedString sound_path)
	{
		return soundCuller.IsAudible(pos, sound_path);
	}

	public Vector3 GetVerticallyScaledPosition(Vector3 pos, bool objectIsSelectedAndVisible = false)
	{
		return soundCuller.GetVerticallyScaledPosition(pos, objectIsSelectedAndVisible);
	}

	public bool IsVisiblePos(Vector3 pos)
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		if (visibleArea.Min <= pos)
		{
			return pos <= visibleArea.Max;
		}
		return false;
	}

	protected override void OnCleanUp()
	{
		Instance = null;
	}

	public void SetFollowTarget(Transform follow_target)
	{
		ClearFollowTarget();
		if (!(follow_target == null))
		{
			followTarget = follow_target;
			SetOrthographicsSize(6f);
			targetOrthographicSize = 6f;
			Vector3 followPos = GetFollowPos();
			followTargetPos = new Vector3(followPos.x, followPos.y, base.transform.GetPosition().z);
			base.transform.SetPosition(followTargetPos);
			followTarget.GetComponent<KMonoBehaviour>().Trigger(-1506069671);
		}
	}

	public void ClearFollowTarget()
	{
		if (!(followTarget == null))
		{
			followTarget.GetComponent<KMonoBehaviour>().Trigger(-485480405);
			followTarget = null;
		}
	}

	public void UpdateFollowTarget()
	{
		if (followTarget != null)
		{
			Vector3 followPos = GetFollowPos();
			Vector2 vector = Vector2.Lerp(new Vector2(base.transform.GetLocalPosition().x, base.transform.GetLocalPosition().y), followPos, Time.unscaledDeltaTime * 25f);
			followTargetPos = new Vector3(vector.x, vector.y, base.transform.GetLocalPosition().z);
		}
	}

	public void RenderForTimelapser(ref RenderTexture tex)
	{
		RenderCameraForTimelapse(baseCamera, ref tex, timelapseCameraCullingMask);
		CameraClearFlags clearFlags = overlayCamera.clearFlags;
		overlayCamera.clearFlags = CameraClearFlags.Nothing;
		RenderCameraForTimelapse(overlayCamera, ref tex, timelapseOverlayCameraCullingMask);
		overlayCamera.clearFlags = clearFlags;
	}

	private void RenderCameraForTimelapse(Camera cam, ref RenderTexture tex, LayerMask mask, float overrideAspect = -1f)
	{
		int cullingMask = cam.cullingMask;
		RenderTexture targetTexture = cam.targetTexture;
		cam.targetTexture = tex;
		cam.aspect = (float)tex.width / (float)tex.height;
		if (overrideAspect != -1f)
		{
			cam.aspect = overrideAspect;
		}
		if ((int)mask != -1)
		{
			cam.cullingMask = mask;
		}
		cam.Render();
		cam.ResetAspect();
		cam.cullingMask = cullingMask;
		cam.targetTexture = targetTexture;
	}
}
