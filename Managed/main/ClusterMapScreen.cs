using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClusterMapScreen : KScreen
{
	public enum Mode
	{
		Default,
		SelectDestination
	}

	public static ClusterMapScreen Instance;

	public GameObject cellVisContainer;

	public GameObject terrainVisContainer;

	public GameObject mobileVisContainer;

	public GameObject telescopeVisContainer;

	public GameObject POIVisContainer;

	public GameObject FXVisContainer;

	public ClusterMapVisualizer cellVisPrefab;

	public ClusterMapVisualizer terrainVisPrefab;

	public ClusterMapVisualizer mobileVisPrefab;

	public ClusterMapVisualizer staticVisPrefab;

	public Color rocketPathColor;

	public Color rocketSelectedPathColor;

	public Color rocketPreviewPathColor;

	private ClusterMapHex m_selectedHex;

	private ClusterMapHex m_hoveredHex;

	private ClusterGridEntity m_selectedEntity;

	public KButton closeButton;

	private const float ZOOM_SCALE_MIN = 50f;

	private const float ZOOM_SCALE_MAX = 150f;

	private const float ZOOM_SCALE_INCREMENT = 25f;

	private const float ZOOM_SCALE_SPEED = 4f;

	private const float ZOOM_NAME_THRESHOLD = 115f;

	private float m_currentZoomScale = 75f;

	private float m_targetZoomScale = 75f;

	private ClusterMapPath m_previewMapPath;

	private Dictionary<ClusterGridEntity, ClusterMapVisualizer> m_gridEntityVis = new Dictionary<ClusterGridEntity, ClusterMapVisualizer>();

	private Dictionary<ClusterGridEntity, ClusterMapVisualizer> m_gridEntityAnims = new Dictionary<ClusterGridEntity, ClusterMapVisualizer>();

	private Dictionary<AxialI, ClusterMapVisualizer> m_cellVisByLocation = new Dictionary<AxialI, ClusterMapVisualizer>();

	private Action<object> m_onDestinationChangedDelegate;

	private Action<object> m_onSelectObjectDelegate;

	[SerializeField]
	private KScrollRect mapScrollRect;

	[SerializeField]
	private float scrollSpeed = 15f;

	public GameObject selectMarkerPrefab;

	public ClusterMapPathDrawer pathDrawer;

	private SelectMarker m_selectMarker;

	private bool movingToTargetNISPosition;

	private Vector3 targetNISPosition;

	private float targetNISZoom;

	private AxialI selectOnMoveNISComplete;

	private Mode m_mode;

	private ClusterDestinationSelector m_destinationSelector;

	private bool m_closeOnSelect;

	private Coroutine activeMoveToTargetRoutine;

	public float floatCycleScale = 4f;

	public float floatCycleOffset = 0.75f;

	public float floatCycleSpeed = 0.75f;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public ClusterMapVisualizer GetEntityVisAnim(ClusterGridEntity entity)
	{
		if (m_gridEntityAnims.ContainsKey(entity))
		{
			return m_gridEntityAnims[entity];
		}
		return null;
	}

	public override float GetSortKey()
	{
		if (base.isEditing)
		{
			return 50f;
		}
		return 20f;
	}

	public float CurrentZoomPercentage()
	{
		return (m_currentZoomScale - 50f) / 100f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		m_selectMarker = Util.KInstantiateUI<SelectMarker>(selectMarkerPrefab, base.gameObject);
		m_selectMarker.gameObject.SetActive(value: false);
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Debug.Assert(cellVisPrefab.rectTransform().sizeDelta == new Vector2(2f, 2f), "The radius of the cellVisPrefab hex must be 1");
		Debug.Assert(terrainVisPrefab.rectTransform().sizeDelta == new Vector2(2f, 2f), "The radius of the terrainVisPrefab hex must be 1");
		Debug.Assert(mobileVisPrefab.rectTransform().sizeDelta == new Vector2(2f, 2f), "The radius of the mobileVisPrefab hex must be 1");
		Debug.Assert(staticVisPrefab.rectTransform().sizeDelta == new Vector2(2f, 2f), "The radius of the staticVisPrefab hex must be 1");
		GenerateGridVis(out var _, out var maxR, out var _, out var maxQ);
		Show(show: false);
		mapScrollRect.content.sizeDelta = new Vector2(maxR * 4, maxQ * 4);
		mapScrollRect.content.localScale = new Vector3(m_currentZoomScale, m_currentZoomScale, 1f);
		m_onDestinationChangedDelegate = OnDestinationChanged;
		m_onSelectObjectDelegate = OnSelectObject;
		Subscribe(1980521255, UpdateVis);
	}

	protected void MoveToNISPosition()
	{
		if (!movingToTargetNISPosition)
		{
			return;
		}
		Vector3 b = new Vector3((0f - targetNISPosition.x) * mapScrollRect.content.localScale.x, (0f - targetNISPosition.y) * mapScrollRect.content.localScale.y, targetNISPosition.z);
		m_targetZoomScale = Mathf.Lerp(m_targetZoomScale, targetNISZoom, Time.unscaledDeltaTime * 2f);
		mapScrollRect.content.SetLocalPosition(Vector3.Lerp(mapScrollRect.content.GetLocalPosition(), b, Time.unscaledDeltaTime * 2.5f));
		float num = Vector3.Distance(mapScrollRect.content.GetLocalPosition(), b);
		if (num < 100f)
		{
			ClusterMapHex component = m_cellVisByLocation[selectOnMoveNISComplete].GetComponent<ClusterMapHex>();
			if (m_selectedHex != component)
			{
				SelectHex(component);
			}
			if (num < 10f)
			{
				movingToTargetNISPosition = false;
			}
		}
	}

	public void SetTargetFocusPosition(AxialI targetPosition, float delayBeforeMove = 0.5f)
	{
		if (activeMoveToTargetRoutine != null)
		{
			StopCoroutine(activeMoveToTargetRoutine);
		}
		activeMoveToTargetRoutine = StartCoroutine(MoveToTargetRoutine(targetPosition, delayBeforeMove));
	}

	private IEnumerator MoveToTargetRoutine(AxialI targetPosition, float delayBeforeMove)
	{
		delayBeforeMove = Mathf.Max(delayBeforeMove, 0f);
		yield return new WaitForSecondsRealtime(delayBeforeMove);
		targetNISPosition = AxialUtil.AxialToWorld(targetPosition.r, targetPosition.q);
		targetNISZoom = 150f;
		movingToTargetNISPosition = true;
		selectOnMoveNISComplete = targetPosition;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed && (e.IsAction(Action.ZoomIn) || e.IsAction(Action.ZoomOut)))
		{
			List<RaycastResult> list = new List<RaycastResult>();
			PointerEventData pointerEventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
			pointerEventData.position = KInputManager.GetMousePos();
			UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
			if (current != null)
			{
				current.RaycastAll(pointerEventData, list);
				bool flag = false;
				foreach (RaycastResult item in list)
				{
					if (!item.gameObject.transform.IsChildOf(base.transform))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					float num = 0f;
					if (KInputManager.currentControllerIsGamepad)
					{
						num = 25f;
						num *= (float)(e.IsAction(Action.ZoomIn) ? 1 : (-1));
					}
					else
					{
						num = Input.mouseScrollDelta.y * 25f;
					}
					m_targetZoomScale = Mathf.Clamp(m_targetZoomScale + num, 50f, 150f);
					e.TryConsume(Action.ZoomIn);
					if (!e.Consumed)
					{
						e.TryConsume(Action.ZoomOut);
					}
				}
			}
		}
		CameraController.Instance.ChangeWorldInput(e);
		base.OnKeyDown(e);
	}

	public bool TryHandleCancel()
	{
		if (m_mode == Mode.SelectDestination && !m_closeOnSelect)
		{
			SetMode(Mode.Default);
			return true;
		}
		return false;
	}

	public void ShowInSelectDestinationMode(ClusterDestinationSelector destination_selector)
	{
		m_destinationSelector = destination_selector;
		if (!base.gameObject.activeSelf)
		{
			ManagementMenu.Instance.ToggleClusterMap();
			m_closeOnSelect = true;
		}
		ClusterGridEntity component = destination_selector.GetComponent<ClusterGridEntity>();
		SetSelectedEntity(component);
		if (m_selectedEntity != null)
		{
			m_selectedHex = m_cellVisByLocation[m_selectedEntity.Location].GetComponent<ClusterMapHex>();
		}
		else
		{
			AxialI myWorldLocation = destination_selector.GetMyWorldLocation();
			ClusterMapHex clusterMapHex = (m_selectedHex = m_cellVisByLocation[myWorldLocation].GetComponent<ClusterMapHex>());
		}
		SetMode(Mode.SelectDestination);
	}

	private void SetMode(Mode mode)
	{
		m_mode = mode;
		if (m_mode == Mode.Default)
		{
			m_destinationSelector = null;
		}
		UpdateVis();
	}

	public Mode GetMode()
	{
		return m_mode;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			MoveToNISPosition();
			UpdateVis();
			if (m_mode == Mode.Default)
			{
				TrySelectDefault();
			}
			Game.Instance.Subscribe(-1991583975, OnFogOfWarRevealed);
			Game.Instance.Subscribe(-1554423969, OnNewTelescopeTarget);
			Game.Instance.Subscribe(-1298331547, OnClusterLocationChanged);
			ClusterMapSelectTool.Instance.Activate();
			SetShowingNonClusterMapHud(show: false);
			CameraController.Instance.DisableUserCameraControl = true;
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().MENUStarmapNotPausedSnapshot);
			MusicManager.instance.PlaySong("Music_Starmap");
			UpdateTearStatus();
		}
		else
		{
			Game.Instance.Unsubscribe(-1554423969, OnNewTelescopeTarget);
			Game.Instance.Unsubscribe(-1991583975, OnFogOfWarRevealed);
			Game.Instance.Unsubscribe(-1298331547, OnClusterLocationChanged);
			m_mode = Mode.Default;
			m_closeOnSelect = false;
			m_destinationSelector = null;
			SelectTool.Instance.Activate();
			SetShowingNonClusterMapHud(show: true);
			CameraController.Instance.DisableUserCameraControl = false;
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUStarmapNotPausedSnapshot);
			if (MusicManager.instance.SongIsPlaying("Music_Starmap"))
			{
				MusicManager.instance.StopSong("Music_Starmap");
			}
		}
	}

	private void SetShowingNonClusterMapHud(bool show)
	{
		PlanScreen.Instance.gameObject.SetActive(show);
		ToolMenu.Instance.gameObject.SetActive(show);
		OverlayScreen.Instance.gameObject.SetActive(show);
	}

	private void SetSelectedEntity(ClusterGridEntity entity, bool frameDelay = false)
	{
		if (m_selectedEntity != null)
		{
			m_selectedEntity.Unsubscribe(543433792, m_onDestinationChangedDelegate);
			m_selectedEntity.Unsubscribe(-1503271301, m_onSelectObjectDelegate);
		}
		m_selectedEntity = entity;
		if (m_selectedEntity != null)
		{
			m_selectedEntity.Subscribe(543433792, m_onDestinationChangedDelegate);
			m_selectedEntity.Subscribe(-1503271301, m_onSelectObjectDelegate);
		}
		KSelectable new_selected = ((m_selectedEntity != null) ? m_selectedEntity.GetComponent<KSelectable>() : null);
		if (frameDelay)
		{
			ClusterMapSelectTool.Instance.SelectNextFrame(new_selected);
		}
		else
		{
			ClusterMapSelectTool.Instance.Select(new_selected);
		}
	}

	private void OnDestinationChanged(object data)
	{
		UpdateVis();
	}

	private void OnSelectObject(object data)
	{
		if (m_selectedEntity == null)
		{
			return;
		}
		KSelectable component = m_selectedEntity.GetComponent<KSelectable>();
		if (component == null || component.IsSelected)
		{
			return;
		}
		SetSelectedEntity(null);
		if (m_mode == Mode.SelectDestination)
		{
			if (m_closeOnSelect)
			{
				ManagementMenu.Instance.CloseAll();
			}
			else
			{
				SetMode(Mode.Default);
			}
		}
		UpdateVis();
	}

	private void OnFogOfWarRevealed(object data = null)
	{
		UpdateVis();
	}

	private void OnNewTelescopeTarget(object data = null)
	{
		UpdateVis();
	}

	private void Update()
	{
		if (KInputManager.currentControllerIsGamepad)
		{
			mapScrollRect.AnalogUpdate(KInputManager.steamInputInterpreter.GetSteamCameraMovement() * scrollSpeed);
		}
	}

	private void TrySelectDefault()
	{
		if (m_selectedHex != null && m_selectedEntity != null)
		{
			UpdateVis();
			return;
		}
		WorldContainer activeWorld = ClusterManager.Instance.activeWorld;
		if (!(activeWorld == null))
		{
			ClusterGridEntity component = activeWorld.GetComponent<ClusterGridEntity>();
			if (!(component == null))
			{
				SelectEntity(component);
			}
		}
	}

	private void GenerateGridVis(out int minR, out int maxR, out int minQ, out int maxQ)
	{
		minR = int.MaxValue;
		maxR = int.MinValue;
		minQ = int.MaxValue;
		maxQ = int.MinValue;
		foreach (KeyValuePair<AxialI, List<ClusterGridEntity>> cellContent in ClusterGrid.Instance.cellContents)
		{
			ClusterMapVisualizer clusterMapVisualizer = UnityEngine.Object.Instantiate(cellVisPrefab, Vector3.zero, Quaternion.identity, cellVisContainer.transform);
			clusterMapVisualizer.rectTransform().SetLocalPosition(cellContent.Key.ToWorld());
			clusterMapVisualizer.gameObject.SetActive(value: true);
			ClusterMapHex component = clusterMapVisualizer.GetComponent<ClusterMapHex>();
			component.SetLocation(cellContent.Key);
			m_cellVisByLocation.Add(cellContent.Key, clusterMapVisualizer);
			minR = Mathf.Min(minR, component.location.R);
			maxR = Mathf.Max(maxR, component.location.R);
			minQ = Mathf.Min(minQ, component.location.Q);
			maxQ = Mathf.Max(maxQ, component.location.Q);
		}
		SetupVisGameObjects();
		UpdateVis();
	}

	public Transform GetGridEntityNameTarget(ClusterGridEntity entity)
	{
		if (m_currentZoomScale >= 115f && m_gridEntityVis.TryGetValue(entity, out var value))
		{
			return value.nameTarget;
		}
		return null;
	}

	public override void ScreenUpdate(bool topLevel)
	{
		float t = Mathf.Min(4f * Time.unscaledDeltaTime, 0.9f);
		m_currentZoomScale = Mathf.Lerp(m_currentZoomScale, m_targetZoomScale, t);
		Vector2 vector = KInputManager.GetMousePos();
		Vector3 vector2 = mapScrollRect.content.InverseTransformPoint(vector);
		mapScrollRect.content.localScale = new Vector3(m_currentZoomScale, m_currentZoomScale, 1f);
		Vector3 vector3 = mapScrollRect.content.InverseTransformPoint(vector);
		mapScrollRect.content.localPosition += (vector3 - vector2) * m_currentZoomScale;
		MoveToNISPosition();
		FloatyAsteroidAnimation();
	}

	private void FloatyAsteroidAnimation()
	{
		float num = 0f;
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			AsteroidGridEntity component = worldContainer.GetComponent<AsteroidGridEntity>();
			if (component != null && m_gridEntityVis.ContainsKey(component) && GetRevealLevel(component) == ClusterRevealLevel.Visible)
			{
				KBatchedAnimController firstAnimController = m_gridEntityVis[component].GetFirstAnimController();
				float y = floatCycleOffset + floatCycleScale * Mathf.Sin(floatCycleSpeed * (num + GameClock.Instance.GetTime()));
				firstAnimController.Offset = new Vector2(0f, y);
			}
			num += 1f;
		}
	}

	private void SetupVisGameObjects()
	{
		foreach (KeyValuePair<AxialI, List<ClusterGridEntity>> cellContent in ClusterGrid.Instance.cellContents)
		{
			foreach (ClusterGridEntity item in cellContent.Value)
			{
				ClusterGrid.Instance.GetCellRevealLevel(cellContent.Key);
				_ = item.IsVisibleInFOW;
				ClusterRevealLevel revealLevel = GetRevealLevel(item);
				if (item.IsVisible && revealLevel != 0 && !m_gridEntityVis.ContainsKey(item))
				{
					ClusterMapVisualizer original = null;
					GameObject gameObject = null;
					switch (item.Layer)
					{
					case EntityLayer.Asteroid:
						original = terrainVisPrefab;
						gameObject = terrainVisContainer;
						break;
					case EntityLayer.Craft:
						original = mobileVisPrefab;
						gameObject = mobileVisContainer;
						break;
					case EntityLayer.POI:
						original = staticVisPrefab;
						gameObject = POIVisContainer;
						break;
					case EntityLayer.Telescope:
						original = staticVisPrefab;
						gameObject = telescopeVisContainer;
						break;
					case EntityLayer.Payload:
						original = mobileVisPrefab;
						gameObject = mobileVisContainer;
						break;
					case EntityLayer.FX:
						original = staticVisPrefab;
						gameObject = FXVisContainer;
						break;
					}
					ClusterNameDisplayScreen.Instance.AddNewEntry(item);
					ClusterMapVisualizer clusterMapVisualizer = UnityEngine.Object.Instantiate(original, gameObject.transform);
					clusterMapVisualizer.Init(item, pathDrawer);
					clusterMapVisualizer.gameObject.SetActive(value: true);
					m_gridEntityAnims.Add(item, clusterMapVisualizer);
					m_gridEntityVis.Add(item, clusterMapVisualizer);
					item.positionDirty = false;
					item.Subscribe(1502190696, RemoveDeletedEntities);
				}
			}
		}
		RemoveDeletedEntities();
		foreach (KeyValuePair<ClusterGridEntity, ClusterMapVisualizer> gridEntityVi in m_gridEntityVis)
		{
			ClusterGridEntity key = gridEntityVi.Key;
			if (key.Layer == EntityLayer.Asteroid)
			{
				int id = key.GetComponent<WorldContainer>().id;
				gridEntityVi.Value.alertVignette.worldID = id;
			}
		}
	}

	private void RemoveDeletedEntities(object obj = null)
	{
		foreach (ClusterGridEntity item in m_gridEntityVis.Keys.Where((ClusterGridEntity x) => x == null || x.gameObject == (GameObject)obj).ToList())
		{
			Util.KDestroyGameObject(m_gridEntityVis[item]);
			m_gridEntityVis.Remove(item);
			m_gridEntityAnims.Remove(item);
		}
	}

	private void OnClusterLocationChanged(object data)
	{
		UpdateVis();
	}

	public static ClusterRevealLevel GetRevealLevel(ClusterGridEntity entity)
	{
		ClusterRevealLevel cellRevealLevel = ClusterGrid.Instance.GetCellRevealLevel(entity.Location);
		ClusterRevealLevel isVisibleInFOW = entity.IsVisibleInFOW;
		if (cellRevealLevel == ClusterRevealLevel.Visible || isVisibleInFOW == ClusterRevealLevel.Visible)
		{
			return ClusterRevealLevel.Visible;
		}
		if (cellRevealLevel == ClusterRevealLevel.Peeked && isVisibleInFOW == ClusterRevealLevel.Peeked)
		{
			return ClusterRevealLevel.Peeked;
		}
		return ClusterRevealLevel.Hidden;
	}

	private void UpdateVis(object data = null)
	{
		SetupVisGameObjects();
		UpdatePaths();
		foreach (KeyValuePair<ClusterGridEntity, ClusterMapVisualizer> gridEntityAnim in m_gridEntityAnims)
		{
			ClusterRevealLevel revealLevel = GetRevealLevel(gridEntityAnim.Key);
			gridEntityAnim.Value.Show(revealLevel);
			bool selected = m_selectedEntity == gridEntityAnim.Key;
			gridEntityAnim.Value.Select(selected);
			if (gridEntityAnim.Key.positionDirty)
			{
				Vector3 position = ClusterGrid.Instance.GetPosition(gridEntityAnim.Key);
				gridEntityAnim.Value.rectTransform().SetLocalPosition(position);
				gridEntityAnim.Key.positionDirty = false;
			}
		}
		if (m_selectedEntity != null && m_gridEntityVis.ContainsKey(m_selectedEntity))
		{
			ClusterMapVisualizer clusterMapVisualizer = m_gridEntityVis[m_selectedEntity];
			m_selectMarker.SetTargetTransform(clusterMapVisualizer.transform);
			m_selectMarker.gameObject.SetActive(value: true);
			clusterMapVisualizer.transform.SetAsLastSibling();
		}
		else
		{
			m_selectMarker.gameObject.SetActive(value: false);
		}
		foreach (KeyValuePair<AxialI, ClusterMapVisualizer> item in m_cellVisByLocation)
		{
			ClusterMapHex component = item.Value.GetComponent<ClusterMapHex>();
			AxialI key = item.Key;
			component.SetRevealed(ClusterGrid.Instance.GetCellRevealLevel(key));
		}
		UpdateHexToggleStates();
		FloatyAsteroidAnimation();
	}

	private void OnEntityDestroyed(object obj)
	{
		RemoveDeletedEntities();
	}

	private void UpdateHexToggleStates()
	{
		bool flag = m_hoveredHex != null && (bool)ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(m_hoveredHex.location, EntityLayer.Asteroid);
		foreach (KeyValuePair<AxialI, ClusterMapVisualizer> item in m_cellVisByLocation)
		{
			ClusterMapHex component = item.Value.GetComponent<ClusterMapHex>();
			AxialI key = item.Key;
			ClusterMapHex.ToggleState state = ((m_selectedHex != null && m_selectedHex.location == key) ? ClusterMapHex.ToggleState.Selected : ((flag && m_hoveredHex.location.IsAdjacent(key)) ? ClusterMapHex.ToggleState.OrbitHighlight : ClusterMapHex.ToggleState.Unselected));
			component.UpdateToggleState(state);
		}
	}

	public void SelectEntity(ClusterGridEntity entity, bool frameDelay = false)
	{
		if (entity != null)
		{
			SetSelectedEntity(entity, frameDelay);
			ClusterMapHex clusterMapHex = (m_selectedHex = m_cellVisByLocation[entity.Location].GetComponent<ClusterMapHex>());
		}
		UpdateVis();
	}

	public void SelectHex(ClusterMapHex newSelectionHex)
	{
		if (m_mode == Mode.Default)
		{
			List<ClusterGridEntity> visibleEntitiesAtCell = ClusterGrid.Instance.GetVisibleEntitiesAtCell(newSelectionHex.location);
			for (int num = visibleEntitiesAtCell.Count - 1; num >= 0; num--)
			{
				KSelectable component = visibleEntitiesAtCell[num].GetComponent<KSelectable>();
				if (component == null || !component.IsSelectable)
				{
					visibleEntitiesAtCell.RemoveAt(num);
				}
			}
			if (visibleEntitiesAtCell.Count == 0)
			{
				SetSelectedEntity(null);
			}
			else
			{
				int num2 = visibleEntitiesAtCell.IndexOf(m_selectedEntity);
				int index = 0;
				if (num2 >= 0)
				{
					index = (num2 + 1) % visibleEntitiesAtCell.Count;
				}
				SetSelectedEntity(visibleEntitiesAtCell[index]);
			}
			m_selectedHex = newSelectionHex;
		}
		else if (m_mode == Mode.SelectDestination)
		{
			Debug.Assert(m_destinationSelector != null, "Selected a hex in SelectDestination mode with no ClusterDestinationSelector");
			if (ClusterGrid.Instance.GetPath(m_selectedHex.location, newSelectionHex.location, m_destinationSelector) != null)
			{
				m_destinationSelector.SetDestination(newSelectionHex.location);
				if (m_closeOnSelect)
				{
					ManagementMenu.Instance.CloseAll();
				}
				else
				{
					SetMode(Mode.Default);
				}
			}
		}
		UpdateVis();
	}

	public bool HasCurrentHover()
	{
		return m_hoveredHex != null;
	}

	public AxialI GetCurrentHoverLocation()
	{
		return m_hoveredHex.location;
	}

	public void OnHoverHex(ClusterMapHex newHoverHex)
	{
		m_hoveredHex = newHoverHex;
		if (m_mode == Mode.SelectDestination)
		{
			UpdateVis();
		}
		UpdateHexToggleStates();
	}

	public void OnUnhoverHex(ClusterMapHex unhoveredHex)
	{
		if (m_hoveredHex == unhoveredHex)
		{
			m_hoveredHex = null;
			UpdateHexToggleStates();
		}
	}

	public void SetLocationHighlight(AxialI location, bool highlight)
	{
		m_cellVisByLocation[location].GetComponent<ClusterMapHex>().ChangeState(highlight ? 1 : 0);
	}

	private void UpdatePaths()
	{
		ClusterDestinationSelector clusterDestinationSelector = ((m_selectedEntity != null) ? m_selectedEntity.GetComponent<ClusterDestinationSelector>() : null);
		if (m_mode == Mode.SelectDestination && m_hoveredHex != null)
		{
			Debug.Assert(m_destinationSelector != null, "In SelectDestination mode without a destination selector");
			AxialI myWorldLocation = m_destinationSelector.GetMyWorldLocation();
			string fail_reason;
			List<AxialI> path = ClusterGrid.Instance.GetPath(myWorldLocation, m_hoveredHex.location, m_destinationSelector, out fail_reason);
			if (path != null)
			{
				if (m_previewMapPath == null)
				{
					m_previewMapPath = pathDrawer.AddPath();
				}
				ClusterMapVisualizer clusterMapVisualizer = m_gridEntityVis[GetSelectorGridEntity(m_destinationSelector)];
				m_previewMapPath.SetPoints(ClusterMapPathDrawer.GetDrawPathList(clusterMapVisualizer.transform.localPosition, path));
				m_previewMapPath.SetColor(rocketPreviewPathColor);
			}
			else if (m_previewMapPath != null)
			{
				Util.KDestroyGameObject(m_previewMapPath);
				m_previewMapPath = null;
			}
			int num = path?.Count ?? (-1);
			if (m_selectedEntity != null)
			{
				float range = m_selectedEntity.GetComponent<IClusterRange>().GetRange();
				if ((float)num > range / 600f && string.IsNullOrEmpty(fail_reason))
				{
					fail_reason = string.Format(UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_OUT_OF_RANGE, range / 600f);
				}
				bool repeat = clusterDestinationSelector.GetComponent<RocketClusterDestinationSelector>().Repeat;
				m_hoveredHex.SetDestinationStatus(fail_reason, num, (int)range, repeat);
			}
			else
			{
				m_hoveredHex.SetDestinationStatus(fail_reason);
			}
		}
		else if (m_previewMapPath != null)
		{
			Util.KDestroyGameObject(m_previewMapPath);
			m_previewMapPath = null;
		}
	}

	private ClusterGridEntity GetSelectorGridEntity(ClusterDestinationSelector selector)
	{
		ClusterGridEntity component = selector.GetComponent<ClusterGridEntity>();
		if (component != null && ClusterGrid.Instance.IsVisible(component))
		{
			return component;
		}
		ClusterGridEntity visibleEntityOfLayerAtCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(selector.GetMyWorldLocation(), EntityLayer.Asteroid);
		Debug.Assert(component != null || visibleEntityOfLayerAtCell != null, $"{selector} has no grid entity and isn't located at a visible asteroid at {selector.GetMyWorldLocation()}");
		if ((bool)visibleEntityOfLayerAtCell)
		{
			return visibleEntityOfLayerAtCell;
		}
		return component;
	}

	private void UpdateTearStatus()
	{
		ClusterPOIManager clusterPOIManager = null;
		if (ClusterManager.Instance != null)
		{
			clusterPOIManager = ClusterManager.Instance.GetComponent<ClusterPOIManager>();
		}
		if (clusterPOIManager != null)
		{
			TemporalTear temporalTear = clusterPOIManager.GetTemporalTear();
			if (temporalTear != null)
			{
				temporalTear.UpdateStatus();
			}
		}
	}
}
