using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClusterMapScreen : KScreen
{
	public enum Mode
	{
		Default,
		SelectDestination
	}

	public static ClusterMapScreen Instance;

	public GameObject cellVisPrefab;

	public GameObject cellVisContainer;

	public GameObject terrainVisPrefab;

	public GameObject terrainVisContainer;

	public GameObject mobileVisPrefab;

	public GameObject mobileVisContainer;

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

	private Dictionary<ClusterGridEntity, GameObject> m_gridEntityVis = new Dictionary<ClusterGridEntity, GameObject>();

	private Dictionary<ClusterGridEntity, KBatchedAnimController> m_gridEntityAnims = new Dictionary<ClusterGridEntity, KBatchedAnimController>();

	private Dictionary<AxialI, GameObject> m_cellVisByLocation = new Dictionary<AxialI, GameObject>();

	private Action<object> m_onDestinationChangedDelegate;

	private Action<object> m_onSelectObjectDelegate;

	[SerializeField]
	private KScrollRect mapScrollRect;

	public GameObject selectMarkerPrefab;

	private SelectMarker m_selectMarker;

	private ClusterMapPathDrawer m_pathDrawer;

	private bool movingToTargetNISPosition = false;

	private Vector3 targetNISPosition;

	private float targetNISZoom;

	private AxialI selectOnMoveNISComplete;

	private Mode m_mode;

	private ClusterDestinationSelector m_destinationSelector;

	private bool m_closeOnSelect;

	private Coroutine activeMoveToTargetRoutine = null;

	private float floatCycleScale = 0.05f;

	private float floatCycleSpeed = 0.75f;

	public override float GetSortKey()
	{
		if (base.isEditing)
		{
			return 50f;
		}
		return 20f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		m_selectMarker = Util.KInstantiateUI<SelectMarker>(selectMarkerPrefab, base.gameObject);
		m_selectMarker.gameObject.SetActive(value: false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		m_pathDrawer = GetComponentInChildren<ClusterMapPathDrawer>();
		Instance = this;
		Debug.Assert(cellVisPrefab.rectTransform().sizeDelta == new Vector2(2f, 2f), "The radius of the cellVisPrefab hex must be 1");
		Debug.Assert(terrainVisPrefab.rectTransform().sizeDelta == new Vector2(2f, 2f), "The radius of the terrainVisPrefab hex must be 1");
		Debug.Assert(mobileVisPrefab.rectTransform().sizeDelta == new Vector2(2f, 2f), "The radius of the mobileVisPrefab hex must be 1");
		GenerateGridVis(out var _, out var maxR, out var _, out var maxQ);
		Show(show: false);
		mapScrollRect.content.sizeDelta = new Vector2(maxR * 4, maxQ * 4);
		mapScrollRect.content.localScale = new Vector3(m_currentZoomScale, m_currentZoomScale, 1f);
		m_onDestinationChangedDelegate = OnDestinationChanged;
		m_onSelectObjectDelegate = OnSelectObject;
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
		if (!e.Consumed && (e.TryConsume(Action.ZoomIn) || e.TryConsume(Action.ZoomOut)))
		{
			float num = Input.mouseScrollDelta.y * 25f;
			m_targetZoomScale = Mathf.Clamp(m_targetZoomScale + num, 50f, 150f);
		}
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
			Game.Instance.Subscribe(-1298331547, OnClusterLocationChanged);
			ClusterMapSelectTool.Instance.Activate();
			SetShowingNonClusterMapHud(show: false);
			CameraController.Instance.DisableUserCameraControl = true;
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().MENUStarmapNotPausedSnapshot);
			MusicManager.instance.PlaySong("Music_Starmap");
		}
		else
		{
			Game.Instance.Unsubscribe(-1991583975, OnFogOfWarRevealed);
			Game.Instance.Unsubscribe(-1298331547, OnClusterLocationChanged);
			m_mode = Mode.Default;
			m_closeOnSelect = false;
			m_destinationSelector = null;
			SelectTool.Instance.Activate();
			SetShowingNonClusterMapHud(show: true);
			CameraController.Instance.DisableUserCameraControl = false;
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUStarmapNotPausedSnapshot);
			MusicManager.instance.StopSong("Music_Starmap");
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

	private void TrySelectDefault()
	{
		if (m_selectedHex != null && m_selectedEntity != null)
		{
			UpdateVis();
		}
		else if (m_cellVisByLocation.Count > 0 && ClusterGrid.Instance.cellContents[AxialI.ZERO].Count > 0)
		{
			SelectHex(m_cellVisByLocation[AxialI.ZERO].GetComponent<ClusterMapHex>());
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
			GameObject gameObject = UnityEngine.Object.Instantiate(cellVisPrefab, Vector3.zero, Quaternion.identity, cellVisContainer.transform);
			gameObject.rectTransform().SetLocalPosition(cellContent.Key.ToWorld());
			gameObject.SetActive(value: true);
			ClusterMapHex component = gameObject.GetComponent<ClusterMapHex>();
			component.SetLocation(cellContent.Key);
			m_cellVisByLocation.Add(cellContent.Key, gameObject);
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
			return value.transform.Find("NameTarget");
		}
		return null;
	}

	private void Update()
	{
		float t = Mathf.Min(4f * Time.unscaledDeltaTime, 0.9f);
		m_currentZoomScale = Mathf.Lerp(m_currentZoomScale, m_targetZoomScale, t);
		Vector2 v = KInputManager.GetMousePos();
		Vector3 b = mapScrollRect.content.InverseTransformPoint(v);
		mapScrollRect.content.localScale = new Vector3(m_currentZoomScale, m_currentZoomScale, 1f);
		Vector3 a = mapScrollRect.content.InverseTransformPoint(v);
		mapScrollRect.content.localPosition += (a - b) * m_currentZoomScale;
		MoveToNISPosition();
		FloatyAsteroidAnimation();
	}

	private void FloatyAsteroidAnimation()
	{
		float num = 0f;
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			AsteroidGridEntity component = worldContainer.GetComponent<AsteroidGridEntity>();
			if (component != null && m_gridEntityVis.ContainsKey(component))
			{
				KBatchedAnimController componentInChildren = m_gridEntityVis[component].GetComponentInChildren<KBatchedAnimController>();
				float b = 0f - floatCycleScale * 4f + floatCycleScale * Mathf.Sin(floatCycleSpeed * (num + Time.time));
				componentInChildren.transform.SetLocalPosition(new Vector3(componentInChildren.transform.localPosition.x, Mathf.Lerp(componentInChildren.transform.localPosition.y, b, Time.unscaledDeltaTime), componentInChildren.transform.localPosition.z));
			}
			num += 1f;
		}
	}

	private void SetupVisGameObjects()
	{
		foreach (KeyValuePair<AxialI, List<ClusterGridEntity>> cellContent in ClusterGrid.Instance.cellContents)
		{
			if (!ClusterGrid.Instance.IsCellVisible(cellContent.Key))
			{
				continue;
			}
			foreach (ClusterGridEntity item in cellContent.Value)
			{
				if (m_gridEntityVis.ContainsKey(item))
				{
					continue;
				}
				GameObject original = null;
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
					original = terrainVisPrefab;
					gameObject = terrainVisContainer;
					break;
				}
				GameObject gameObject2 = UnityEngine.Object.Instantiate(original, gameObject.transform);
				ClusterNameDisplayScreen.Instance.AddNewEntry(item);
				Transform transform = gameObject2.transform.Find("Anim");
				if (transform != null)
				{
					bool flag = true;
					foreach (ClusterGridEntity.AnimConfig animConfig in item.AnimConfigs)
					{
						KBatchedAnimController component = UnityEngine.Object.Instantiate(transform, gameObject2.transform).GetComponent<KBatchedAnimController>();
						component.AnimFiles = new KAnimFile[1]
						{
							animConfig.animFile
						};
						component.initialMode = KAnim.PlayMode.Loop;
						component.initialAnim = animConfig.initialAnim;
						component.gameObject.SetActive(value: true);
						if (flag)
						{
							m_gridEntityAnims.Add(item, component);
							flag = false;
						}
					}
				}
				gameObject2.SetActive(value: true);
				m_gridEntityVis.Add(item, gameObject2);
			}
		}
		List<ClusterGridEntity> list = m_gridEntityVis.Keys.Where((ClusterGridEntity x) => x == null).ToList();
		foreach (ClusterGridEntity item2 in list)
		{
			Util.KDestroyGameObject(m_gridEntityVis[item2]);
			m_gridEntityVis.Remove(item2);
		}
		foreach (KeyValuePair<ClusterGridEntity, GameObject> gridEntityVi in m_gridEntityVis)
		{
			ClusterGridEntity key = gridEntityVi.Key;
			if (key.Layer == EntityLayer.Asteroid)
			{
				int id = key.GetComponent<WorldContainer>().id;
				gridEntityVi.Value.GetComponentInChildren<AlertVignette>().worldID = id;
			}
		}
	}

	private void OnClusterLocationChanged(object data)
	{
		UpdateVis();
	}

	private void UpdateVis()
	{
		SetupVisGameObjects();
		foreach (KeyValuePair<ClusterGridEntity, GameObject> gridEntityVi in m_gridEntityVis)
		{
			gridEntityVi.Value.rectTransform().SetLocalPosition(ClusterGrid.Instance.GetPosition(gridEntityVi.Key));
			bool active = ClusterGrid.Instance.IsCellVisible(gridEntityVi.Key.Location) && gridEntityVi.Key.IsVisible;
			gridEntityVi.Value.SetActive(active);
		}
		UpdatePaths();
		foreach (KeyValuePair<ClusterGridEntity, GameObject> gridEntityVi2 in m_gridEntityVis)
		{
			ClusterMapPath pathForRotateParent = m_pathDrawer.GetPathForRotateParent(gridEntityVi2.Value);
			if (pathForRotateParent != null)
			{
				pathForRotateParent.RotateTransformAlongPath(gridEntityVi2.Value.transform);
			}
			else
			{
				gridEntityVi2.Value.transform.localRotation = Quaternion.identity;
			}
		}
		foreach (KeyValuePair<ClusterGridEntity, KBatchedAnimController> gridEntityAnim in m_gridEntityAnims)
		{
			bool is_visible = m_selectedEntity == gridEntityAnim.Key;
			KBatchedAnimController value = gridEntityAnim.Value;
			value.SetSymbolVisiblity("selected", is_visible);
		}
		if (m_selectedEntity != null)
		{
			GameObject gameObject = m_gridEntityVis[m_selectedEntity];
			m_selectMarker.SetTargetTransform(gameObject.transform);
			m_selectMarker.gameObject.SetActive(value: true);
			gameObject.transform.SetAsLastSibling();
		}
		else
		{
			m_selectMarker.gameObject.SetActive(value: false);
		}
		foreach (KeyValuePair<AxialI, GameObject> item in m_cellVisByLocation)
		{
			ClusterMapHex component = item.Value.GetComponent<ClusterMapHex>();
			AxialI key = item.Key;
			component.SetRevealed(ClusterGrid.Instance.IsCellVisible(key));
		}
		UpdateHexToggleStates();
	}

	private void UpdateHexToggleStates()
	{
		bool flag = m_hoveredHex != null && (bool)ClusterGrid.Instance.GetVisibleAsteroidAtCell(m_hoveredHex.location);
		foreach (KeyValuePair<AxialI, GameObject> item in m_cellVisByLocation)
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
			if (visibleEntitiesAtCell.Count == 0)
			{
				SetSelectedEntity(null);
			}
			else
			{
				int num = visibleEntitiesAtCell.IndexOf(m_selectedEntity);
				int index = 0;
				if (num >= 0)
				{
					index = (num + 1) % visibleEntitiesAtCell.Count;
				}
				SetSelectedEntity(visibleEntitiesAtCell[index]);
			}
			m_selectedHex = newSelectionHex;
		}
		else if (m_mode == Mode.SelectDestination)
		{
			Debug.Assert(m_destinationSelector != null, "Selected a hex in SelectDestination mode with no ClusterDestinationSelector");
			List<AxialI> path = ClusterGrid.Instance.GetPath(m_selectedHex.location, newSelectionHex.location, m_destinationSelector);
			if (path != null)
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
		m_pathDrawer.ClearPaths();
		ClusterDestinationSelector selectedEntitySelector = ((m_selectedEntity != null) ? m_selectedEntity.GetComponent<ClusterDestinationSelector>() : null);
		HashSet<ClusterDestinationSelector> hashSet = new HashSet<ClusterDestinationSelector>(from entity in m_gridEntityVis.Keys
			where ClusterGrid.Instance.IsVisible(entity)
			select entity.GetComponent<ClusterDestinationSelector>() into selector
			where selector != null
			orderby selector == selectedEntitySelector
			select selector);
		if (m_destinationSelector != null)
		{
			hashSet.Add(m_destinationSelector);
		}
		foreach (ClusterDestinationSelector item in hashSet)
		{
			if (!item.IsAtDestination())
			{
				AxialI myWorldLocation = item.GetMyWorldLocation();
				List<AxialI> path = ClusterGrid.Instance.GetPath(myWorldLocation, item.GetDestination(), item);
				if (path != null)
				{
					bool flag = item == selectedEntitySelector;
					ClusterGridEntity selectorGridEntity = GetSelectorGridEntity(item);
					GameObject gameObject = m_gridEntityVis[selectorGridEntity];
					m_pathDrawer.AddPath(gameObject.transform.localPosition, path, flag ? rocketSelectedPathColor : rocketPathColor, gameObject, item.shouldPointTowardsPath);
				}
			}
		}
		if (m_mode == Mode.SelectDestination && m_hoveredHex != null)
		{
			Debug.Assert(m_destinationSelector != null, "In SelectDestination mode without a destination selector");
			AxialI myWorldLocation2 = m_destinationSelector.GetMyWorldLocation();
			string fail_reason;
			List<AxialI> path2 = ClusterGrid.Instance.GetPath(myWorldLocation2, m_hoveredHex.location, m_destinationSelector, out fail_reason);
			if (path2 != null)
			{
				GameObject gameObject2 = m_gridEntityVis[GetSelectorGridEntity(m_destinationSelector)];
				m_pathDrawer.AddPath(gameObject2.transform.localPosition, path2, rocketPreviewPathColor, gameObject2, rotateTransform: false);
			}
			m_hoveredHex.SetDestinationStatus(fail_reason);
		}
		m_pathDrawer.RefreshVisiblePaths();
	}

	private ClusterGridEntity GetSelectorGridEntity(ClusterDestinationSelector selector)
	{
		ClusterGridEntity component = selector.GetComponent<ClusterGridEntity>();
		if (component != null && ClusterGrid.Instance.IsVisible(component))
		{
			return component;
		}
		component = ClusterGrid.Instance.GetVisibleAsteroidAtCell(selector.GetMyWorldLocation());
		Debug.Assert(component != null, $"{selector} has no grid entity and isn't located at a visible asteroid at {selector.GetMyWorldLocation()}");
		return component;
	}
}
