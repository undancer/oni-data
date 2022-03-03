using System;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityEngine.EventSystems
{
	[AddComponentMenu("Event/Virtual Input Module")]
	public class VirtualInputModule : PointerInputModule, IInputHandler
	{
		[Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
		public enum InputMode
		{
			Mouse,
			Buttons
		}

		private struct ControllerButtonStates
		{
			public bool affirmativeDown;

			public float affirmativeHoldTime;

			public bool negativeDown;

			public float negativeHoldTime;
		}

		private float m_PrevActionTime;

		private Vector2 m_LastMoveVector;

		private int m_ConsecutiveMoveCount;

		private string debugName;

		private Vector2 m_LastMousePosition;

		private Vector2 m_MousePosition;

		public bool mouseMovementOnly;

		[SerializeField]
		private RectTransform m_VirtualCursor;

		[SerializeField]
		private float m_VirtualCursorSpeed = 1.5f;

		[SerializeField]
		private Vector2 m_VirtualCursorOffset = Vector2.zero;

		[SerializeField]
		private Camera m_canvasCamera;

		private Camera VCcam;

		public bool CursorCanvasShouldBeOverlay;

		private Canvas m_virtualCursorCanvas;

		private CanvasScaler m_virtualCursorScaler;

		private PointerEventData leftClickData;

		private PointerEventData rightClickData;

		private ControllerButtonStates conButtonStates;

		private bool leftReleased;

		private bool rightReleased;

		private bool leftFirstClick;

		private bool rightFirstClick;

		[SerializeField]
		private string m_HorizontalAxis = "Horizontal";

		[SerializeField]
		private string m_VerticalAxis = "Vertical";

		[SerializeField]
		private string m_SubmitButton = "Submit";

		[SerializeField]
		private string m_CancelButton = "Cancel";

		[SerializeField]
		private float m_InputActionsPerSecond = 10f;

		[SerializeField]
		private float m_RepeatDelay = 0.5f;

		[SerializeField]
		[FormerlySerializedAs("m_AllowActivationOnMobileDevice")]
		private bool m_ForceModuleActive;

		private readonly MouseState m_MouseState = new MouseState();

		public string handlerName => "VirtualCursorInput";

		public KInputHandler inputHandler { get; set; }

		[Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
		public InputMode inputMode => InputMode.Mouse;

		[Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead (UnityUpgradable) -> forceModuleActive")]
		public bool allowActivationOnMobileDevice
		{
			get
			{
				return m_ForceModuleActive;
			}
			set
			{
				m_ForceModuleActive = value;
			}
		}

		public bool forceModuleActive
		{
			get
			{
				return m_ForceModuleActive;
			}
			set
			{
				m_ForceModuleActive = value;
			}
		}

		public float inputActionsPerSecond
		{
			get
			{
				return m_InputActionsPerSecond;
			}
			set
			{
				m_InputActionsPerSecond = value;
			}
		}

		public float repeatDelay
		{
			get
			{
				return m_RepeatDelay;
			}
			set
			{
				m_RepeatDelay = value;
			}
		}

		public string horizontalAxis
		{
			get
			{
				return m_HorizontalAxis;
			}
			set
			{
				m_HorizontalAxis = value;
			}
		}

		public string verticalAxis
		{
			get
			{
				return m_VerticalAxis;
			}
			set
			{
				m_VerticalAxis = value;
			}
		}

		public string submitButton
		{
			get
			{
				return m_SubmitButton;
			}
			set
			{
				m_SubmitButton = value;
			}
		}

		public string cancelButton
		{
			get
			{
				return m_CancelButton;
			}
			set
			{
				m_CancelButton = value;
			}
		}

		protected VirtualInputModule()
		{
		}

		public void SetCursor(Texture2D tex)
		{
			UpdateModule();
			if ((bool)m_VirtualCursor)
			{
				m_VirtualCursor.GetComponent<RawImage>().texture = tex;
			}
		}

		public override void UpdateModule()
		{
			if (Global.Instance.GetInputManager().GetControllerCount() <= 1)
			{
				return;
			}
			if (inputHandler == null || !inputHandler.UsesController(this, Global.Instance.GetInputManager().GetController(1)))
			{
				KInputHandler.Add(Global.Instance.GetInputManager().GetController(1), this, int.MaxValue);
				if (!Global.Instance.GetInputManager().usedMenus.Contains(this))
				{
					Global.Instance.GetInputManager().usedMenus.Add(this);
				}
				debugName = SceneManager.GetActiveScene().name + "-VirtualInputModule";
			}
			if (m_VirtualCursor == null)
			{
				m_VirtualCursor = GameObject.Find("VirtualCursor").GetComponent<RectTransform>();
			}
			if (m_canvasCamera == null)
			{
				m_canvasCamera = base.gameObject.AddComponent<Camera>();
				m_canvasCamera.enabled = false;
			}
			if (CameraController.Instance != null)
			{
				m_canvasCamera.CopyFrom(CameraController.Instance.overlayCamera);
			}
			else if (CursorCanvasShouldBeOverlay)
			{
				m_canvasCamera.CopyFrom(GameObject.Find("FrontEndCamera").GetComponent<Camera>());
			}
			if (m_canvasCamera != null && VCcam == null)
			{
				VCcam = GameObject.Find("VirtualCursorCamera").GetComponent<Camera>();
				if (VCcam != null)
				{
					if (m_virtualCursorCanvas == null)
					{
						m_virtualCursorCanvas = GameObject.Find("VirtualCursorCanvas").GetComponent<Canvas>();
						m_virtualCursorScaler = m_virtualCursorCanvas.GetComponent<CanvasScaler>();
					}
					if (CursorCanvasShouldBeOverlay)
					{
						m_virtualCursorCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
						VCcam.orthographic = false;
					}
					else
					{
						VCcam.orthographic = m_canvasCamera.orthographic;
						VCcam.orthographicSize = m_canvasCamera.orthographicSize;
						VCcam.transform.position = m_canvasCamera.transform.position;
						VCcam.enabled = true;
						m_virtualCursorCanvas.renderMode = RenderMode.ScreenSpaceCamera;
						m_virtualCursorCanvas.worldCamera = VCcam;
					}
				}
			}
			if (m_canvasCamera != null && VCcam != null)
			{
				VCcam.orthographic = m_canvasCamera.orthographic;
				VCcam.orthographicSize = m_canvasCamera.orthographicSize;
				VCcam.transform.position = m_canvasCamera.transform.position;
				VCcam.aspect = m_canvasCamera.aspect;
				VCcam.enabled = true;
			}
			Vector2 vector = new Vector2(Screen.width, Screen.height);
			if (m_virtualCursorScaler != null && m_virtualCursorScaler.referenceResolution != vector)
			{
				m_virtualCursorScaler.referenceResolution = vector;
			}
			m_LastMousePosition = m_MousePosition;
			m_VirtualCursor.localScale = Vector2.one;
			m_VirtualCursor.anchoredPosition += KInputManager.steamInputInterpreter.GetSteamCursorMovement() * m_VirtualCursorSpeed;
			m_VirtualCursor.anchoredPosition = new Vector2(Mathf.Clamp(m_VirtualCursor.anchoredPosition.x, 0f, vector.x), Mathf.Clamp(m_VirtualCursor.anchoredPosition.y, 0f, vector.y));
			KInputManager.virtualCursorPos = new Vector3F(m_VirtualCursor.anchoredPosition.x, m_VirtualCursor.anchoredPosition.y, 0f);
			m_MousePosition = m_VirtualCursor.anchoredPosition;
		}

		public override bool IsModuleSupported()
		{
			if (!m_ForceModuleActive)
			{
				return Input.mousePresent;
			}
			return true;
		}

		public override bool ShouldActivateModule()
		{
			if (!base.ShouldActivateModule())
			{
				return false;
			}
			bool num = m_ForceModuleActive;
			Input.GetButtonDown(m_SubmitButton);
			return num | Input.GetButtonDown(m_CancelButton) | !Mathf.Approximately(Input.GetAxisRaw(m_HorizontalAxis), 0f) | !Mathf.Approximately(Input.GetAxisRaw(m_VerticalAxis), 0f) | ((m_MousePosition - m_LastMousePosition).sqrMagnitude > 0f) | Input.GetMouseButtonDown(0);
		}

		public override void ActivateModule()
		{
			base.ActivateModule();
			if (m_canvasCamera == null)
			{
				m_canvasCamera = base.gameObject.AddComponent<Camera>();
				m_canvasCamera.enabled = false;
			}
			if (Input.mousePosition.x > 0f && Input.mousePosition.x < (float)Screen.width && Input.mousePosition.y > 0f && Input.mousePosition.y < (float)Screen.height)
			{
				m_VirtualCursor.anchoredPosition = Input.mousePosition;
			}
			else
			{
				m_VirtualCursor.anchoredPosition = new Vector2(Screen.width / 2, Screen.height / 2);
			}
			m_VirtualCursor.anchoredPosition = new Vector2(Mathf.Clamp(m_VirtualCursor.anchoredPosition.x, 0f, Screen.width), Mathf.Clamp(m_VirtualCursor.anchoredPosition.y, 0f, Screen.height));
			m_VirtualCursor.localScale = Vector2.zero;
			m_MousePosition = m_VirtualCursor.anchoredPosition;
			m_LastMousePosition = m_VirtualCursor.anchoredPosition;
			GameObject gameObject = base.eventSystem.currentSelectedGameObject;
			if (gameObject == null)
			{
				gameObject = base.eventSystem.firstSelectedGameObject;
			}
			if (m_VirtualCursor == null)
			{
				m_VirtualCursor = GameObject.Find("VirtualCursor").GetComponent<RectTransform>();
			}
			if (m_canvasCamera == null)
			{
				m_canvasCamera = GameObject.Find("FrontEndCamera").GetComponent<Camera>();
			}
			base.eventSystem.SetSelectedGameObject(gameObject, GetBaseEventData());
		}

		public override void DeactivateModule()
		{
			base.DeactivateModule();
			ClearSelection();
			conButtonStates.affirmativeDown = false;
			conButtonStates.affirmativeHoldTime = 0f;
			conButtonStates.negativeDown = false;
			conButtonStates.negativeHoldTime = 0f;
		}

		public override void Process()
		{
			bool flag = SendUpdateEventToSelectedObject();
			if (base.eventSystem.sendNavigationEvents)
			{
				if (!flag)
				{
					flag |= SendMoveEventToSelectedObject();
				}
				if (!flag)
				{
					SendSubmitEventToSelectedObject();
				}
			}
			ProcessMouseEvent();
		}

		protected bool SendSubmitEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			BaseEventData baseEventData = GetBaseEventData();
			if (Input.GetButtonDown(m_SubmitButton) || leftFirstClick)
			{
				ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
			}
			if (Input.GetButtonDown(m_CancelButton))
			{
				ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
			}
			return baseEventData.used;
		}

		private Vector2 GetRawMoveVector()
		{
			Vector2 zero = Vector2.zero;
			zero.x = Input.GetAxisRaw(m_HorizontalAxis);
			zero.y = Input.GetAxisRaw(m_VerticalAxis);
			if (Input.GetButtonDown(m_HorizontalAxis))
			{
				if (zero.x < 0f)
				{
					zero.x = -1f;
				}
				if (zero.x > 0f)
				{
					zero.x = 1f;
				}
			}
			if (Input.GetButtonDown(m_VerticalAxis))
			{
				if (zero.y < 0f)
				{
					zero.y = -1f;
				}
				if (zero.y > 0f)
				{
					zero.y = 1f;
				}
			}
			return zero;
		}

		protected bool SendMoveEventToSelectedObject()
		{
			float unscaledTime = Time.unscaledTime;
			Vector2 rawMoveVector = GetRawMoveVector();
			if (Mathf.Approximately(rawMoveVector.x, 0f) && Mathf.Approximately(rawMoveVector.y, 0f))
			{
				m_ConsecutiveMoveCount = 0;
				return false;
			}
			bool flag = Input.GetButtonDown(m_HorizontalAxis) || Input.GetButtonDown(m_VerticalAxis);
			bool flag2 = Vector2.Dot(rawMoveVector, m_LastMoveVector) > 0f;
			if (!flag)
			{
				flag = ((!flag2 || m_ConsecutiveMoveCount != 1) ? (unscaledTime > m_PrevActionTime + 1f / m_InputActionsPerSecond) : (unscaledTime > m_PrevActionTime + m_RepeatDelay));
			}
			if (!flag)
			{
				return false;
			}
			AxisEventData axisEventData = GetAxisEventData(rawMoveVector.x, rawMoveVector.y, 0.6f);
			ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
			if (!flag2)
			{
				m_ConsecutiveMoveCount = 0;
			}
			m_ConsecutiveMoveCount++;
			m_PrevActionTime = unscaledTime;
			m_LastMoveVector = rawMoveVector;
			return axisEventData.used;
		}

		protected void ProcessMouseEvent()
		{
			ProcessMouseEvent(0);
		}

		protected void ProcessMouseEvent(int id)
		{
			if (!mouseMovementOnly)
			{
				MouseState mousePointerEventData = GetMousePointerEventData(id);
				MouseButtonEventData eventData = mousePointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
				ProcessControllerPress(eventData, leftClick: true);
				ProcessControllerPress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData, leftClick: false);
				ProcessMove(eventData.buttonData);
				ProcessDrag(eventData.buttonData);
				ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
				ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
				if (!Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0f))
				{
					ExecuteEvents.ExecuteHierarchy(ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject), eventData.buttonData, ExecuteEvents.scrollHandler);
				}
			}
		}

		protected bool SendUpdateEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			BaseEventData baseEventData = GetBaseEventData();
			ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
			return baseEventData.used;
		}

		protected void ProcessMousePress(MouseButtonEventData data)
		{
			PointerEventData buttonData = data.buttonData;
			GameObject gameObject = buttonData.pointerCurrentRaycast.gameObject;
			if (data.PressedThisFrame())
			{
				buttonData.eligibleForClick = true;
				buttonData.delta = Vector2.zero;
				buttonData.dragging = false;
				buttonData.useDragThreshold = true;
				buttonData.pressPosition = buttonData.position;
				buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
				buttonData.position = m_VirtualCursor.anchoredPosition;
				DeselectIfSelectionChanged(gameObject, buttonData);
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy(gameObject, buttonData, ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				float unscaledTime = Time.unscaledTime;
				if (gameObject2 == buttonData.lastPress)
				{
					if (unscaledTime - buttonData.clickTime < 0.3f)
					{
						buttonData.clickCount++;
					}
					else
					{
						buttonData.clickCount = 1;
					}
					buttonData.clickTime = unscaledTime;
				}
				else
				{
					buttonData.clickCount = 1;
				}
				buttonData.pointerPress = gameObject2;
				buttonData.rawPointerPress = gameObject;
				buttonData.clickTime = unscaledTime;
				buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (buttonData.pointerDrag != null)
				{
					ExecuteEvents.Execute(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
				}
			}
			if (data.ReleasedThisFrame())
			{
				ExecuteEvents.Execute(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (buttonData.pointerPress == eventHandler && buttonData.eligibleForClick)
				{
					ExecuteEvents.Execute(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
				}
				else if (buttonData.pointerDrag != null && buttonData.dragging)
				{
					ExecuteEvents.ExecuteHierarchy(gameObject, buttonData, ExecuteEvents.dropHandler);
				}
				buttonData.eligibleForClick = false;
				buttonData.pointerPress = null;
				buttonData.rawPointerPress = null;
				if (buttonData.pointerDrag != null && buttonData.dragging)
				{
					ExecuteEvents.Execute(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
				}
				buttonData.dragging = false;
				buttonData.pointerDrag = null;
				if (gameObject != buttonData.pointerEnter)
				{
					HandlePointerExitAndEnter(buttonData, null);
					HandlePointerExitAndEnter(buttonData, gameObject);
				}
			}
		}

		public void OnKeyDown(KButtonEvent e)
		{
			if (!KInputManager.currentControllerIsGamepad)
			{
				return;
			}
			if (e.IsAction(Action.MouseLeft) || e.IsAction(Action.ShiftMouseLeft))
			{
				if (conButtonStates.affirmativeDown)
				{
					conButtonStates.affirmativeHoldTime += Time.unscaledDeltaTime;
				}
				if (!conButtonStates.affirmativeDown)
				{
					leftFirstClick = true;
					leftReleased = false;
				}
				conButtonStates.affirmativeDown = true;
			}
			else if (e.IsAction(Action.MouseRight))
			{
				if (conButtonStates.negativeDown)
				{
					conButtonStates.negativeHoldTime += Time.unscaledDeltaTime;
				}
				if (!conButtonStates.negativeDown)
				{
					rightFirstClick = true;
					rightReleased = false;
				}
				conButtonStates.negativeDown = true;
			}
		}

		public void OnKeyUp(KButtonEvent e)
		{
			if (KInputManager.currentControllerIsGamepad)
			{
				if (e.IsAction(Action.MouseLeft) || e.IsAction(Action.ShiftMouseLeft))
				{
					conButtonStates.affirmativeHoldTime = 0f;
					leftReleased = true;
					leftFirstClick = false;
					conButtonStates.affirmativeDown = false;
				}
				else if (e.IsAction(Action.MouseRight))
				{
					conButtonStates.negativeHoldTime = 0f;
					rightReleased = true;
					rightFirstClick = false;
					conButtonStates.negativeDown = false;
				}
			}
		}

		protected void ProcessControllerPress(MouseButtonEventData data, bool leftClick)
		{
			if (leftClickData == null)
			{
				leftClickData = data.buttonData;
			}
			if (rightClickData == null)
			{
				rightClickData = data.buttonData;
			}
			PointerEventData buttonData = data.buttonData;
			GameObject gameObject = buttonData.pointerCurrentRaycast.gameObject;
			buttonData.position = m_VirtualCursor.anchoredPosition;
			if (leftClick)
			{
				if (leftFirstClick)
				{
					buttonData.button = PointerEventData.InputButton.Left;
					buttonData.eligibleForClick = true;
					buttonData.delta = Vector2.zero;
					buttonData.dragging = false;
					buttonData.useDragThreshold = true;
					buttonData.pressPosition = buttonData.position;
					buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
					buttonData.position = new Vector2(KInputManager.virtualCursorPos.x, KInputManager.virtualCursorPos.y);
					DeselectIfSelectionChanged(gameObject, buttonData);
					GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy(gameObject, buttonData, ExecuteEvents.pointerDownHandler);
					if (gameObject2 == null)
					{
						gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
					}
					float unscaledTime = Time.unscaledTime;
					if (gameObject2 == buttonData.lastPress)
					{
						if (unscaledTime - buttonData.clickTime < 0.3f)
						{
							buttonData.clickCount++;
						}
						else
						{
							buttonData.clickCount = 1;
						}
						buttonData.clickTime = unscaledTime;
					}
					else
					{
						buttonData.clickCount = 1;
					}
					buttonData.pointerPress = gameObject2;
					buttonData.rawPointerPress = gameObject;
					buttonData.clickTime = unscaledTime;
					buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
					if (buttonData.pointerDrag != null)
					{
						ExecuteEvents.Execute(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
					}
					leftFirstClick = false;
				}
				if (leftReleased)
				{
					buttonData.button = PointerEventData.InputButton.Left;
					ExecuteEvents.Execute(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
					GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
					if (buttonData.pointerPress == eventHandler && buttonData.eligibleForClick)
					{
						ExecuteEvents.Execute(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
					}
					else if (buttonData.pointerDrag != null && buttonData.dragging)
					{
						ExecuteEvents.ExecuteHierarchy(gameObject, buttonData, ExecuteEvents.dropHandler);
					}
					buttonData.eligibleForClick = false;
					buttonData.pointerPress = null;
					buttonData.rawPointerPress = null;
					if (buttonData.pointerDrag != null && buttonData.dragging)
					{
						ExecuteEvents.Execute(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
					}
					buttonData.dragging = false;
					buttonData.pointerDrag = null;
					if (gameObject != buttonData.pointerEnter)
					{
						HandlePointerExitAndEnter(buttonData, null);
						HandlePointerExitAndEnter(buttonData, gameObject);
					}
					leftReleased = false;
				}
				return;
			}
			if (rightFirstClick)
			{
				buttonData.button = PointerEventData.InputButton.Right;
				buttonData.eligibleForClick = true;
				buttonData.delta = Vector2.zero;
				buttonData.dragging = false;
				buttonData.useDragThreshold = true;
				buttonData.pressPosition = buttonData.position;
				buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
				buttonData.position = m_VirtualCursor.anchoredPosition;
				DeselectIfSelectionChanged(gameObject, buttonData);
				GameObject gameObject3 = ExecuteEvents.ExecuteHierarchy(gameObject, buttonData, ExecuteEvents.pointerDownHandler);
				if (gameObject3 == null)
				{
					gameObject3 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				float unscaledTime2 = Time.unscaledTime;
				if (gameObject3 == buttonData.lastPress)
				{
					if (unscaledTime2 - buttonData.clickTime < 0.3f)
					{
						buttonData.clickCount++;
					}
					else
					{
						buttonData.clickCount = 1;
					}
					buttonData.clickTime = unscaledTime2;
				}
				else
				{
					buttonData.clickCount = 1;
				}
				buttonData.pointerPress = gameObject3;
				buttonData.rawPointerPress = gameObject;
				buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (buttonData.pointerDrag != null)
				{
					ExecuteEvents.Execute(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
				}
				rightFirstClick = false;
			}
			if (rightReleased)
			{
				buttonData.button = PointerEventData.InputButton.Right;
				ExecuteEvents.Execute(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
				GameObject eventHandler2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (buttonData.pointerPress == eventHandler2 && buttonData.eligibleForClick)
				{
					ExecuteEvents.Execute(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
				}
				else if (buttonData.pointerDrag != null && buttonData.dragging)
				{
					ExecuteEvents.ExecuteHierarchy(gameObject, buttonData, ExecuteEvents.dropHandler);
				}
				buttonData.eligibleForClick = false;
				buttonData.pointerPress = null;
				buttonData.rawPointerPress = null;
				if (buttonData.pointerDrag != null && buttonData.dragging)
				{
					ExecuteEvents.Execute(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
				}
				buttonData.dragging = false;
				buttonData.pointerDrag = null;
				if (gameObject != buttonData.pointerEnter)
				{
					HandlePointerExitAndEnter(buttonData, null);
					HandlePointerExitAndEnter(buttonData, gameObject);
				}
				rightReleased = false;
			}
		}

		protected override MouseState GetMousePointerEventData(int id)
		{
			PointerEventData data;
			bool pointerData = GetPointerData(-1, out data, create: true);
			data.Reset();
			Vector2 position = RectTransformUtility.WorldToScreenPoint(m_canvasCamera, m_VirtualCursor.position);
			if (pointerData)
			{
				data.position = position;
			}
			Vector2 anchoredPosition = m_VirtualCursor.anchoredPosition;
			data.delta = anchoredPosition - data.position;
			data.position = anchoredPosition;
			data.scrollDelta = Input.mouseScrollDelta;
			data.button = PointerEventData.InputButton.Left;
			base.eventSystem.RaycastAll(data, m_RaycastResultCache);
			RaycastResult raycastResult2 = (data.pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(m_RaycastResultCache));
			m_RaycastResultCache.Clear();
			GetPointerData(-2, out var data2, create: true);
			CopyFromTo(data, data2);
			data2.button = PointerEventData.InputButton.Right;
			GetPointerData(-3, out var data3, create: true);
			CopyFromTo(data, data3);
			data3.button = PointerEventData.InputButton.Middle;
			m_MouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), data);
			m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), data2);
			m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), data3);
			return m_MouseState;
		}
	}
}
