using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResearchScreen : KModalScreen
{
	public enum ResearchState
	{
		Available,
		ActiveResearch,
		ResearchComplete,
		MissingPrerequisites,
		StateCount
	}

	private const float SCROLL_BUFFER = 250f;

	[SerializeField]
	private Image BG;

	public ResearchEntry entryPrefab;

	public ResearchTreeTitle researchTreeTitlePrefab;

	public GameObject foreground;

	public GameObject scrollContent;

	public GameObject treeTitles;

	public GameObject pointDisplayCountPrefab;

	public GameObject pointDisplayContainer;

	private Dictionary<string, LocText> pointDisplayMap;

	private Dictionary<Tech, ResearchEntry> entryMap;

	[SerializeField]
	private KButton zoomOutButton;

	[SerializeField]
	private KButton zoomInButton;

	[SerializeField]
	private ResearchScreenSideBar sideBar;

	private Tech currentResearch;

	public KButton CloseButton;

	private GraphicRaycaster m_Raycaster;

	private PointerEventData m_PointerEventData;

	private Vector3 currentScrollPosition;

	private bool panUp;

	private bool panDown;

	private bool panLeft;

	private bool panRight;

	private bool rightMouseDown = false;

	private bool leftMouseDown = false;

	private bool isDragging = false;

	private Vector3 dragStartPosition;

	private Vector3 dragLastPosition;

	private Vector2 dragInteria;

	private Vector2 forceTargetPosition;

	private bool zoomingToTarget = false;

	private float targetZoom = 1f;

	private float currentZoom = 1f;

	private bool zoomCenterLock = false;

	private Vector2 keyPanDelta = Vector3.zero;

	[SerializeField]
	private float effectiveZoomSpeed = 5f;

	[SerializeField]
	private float zoomAmountPerScroll = 0.05f;

	[SerializeField]
	private float zoomAmountPerButton = 0.5f;

	[SerializeField]
	private float minZoom = 0.15f;

	[SerializeField]
	private float maxZoom = 1f;

	[SerializeField]
	private float keyboardScrollSpeed = 200f;

	[SerializeField]
	private float keyPanEasing = 1f;

	[SerializeField]
	private float edgeClampFactor = 0.5f;

	public bool IsBeingResearched(Tech tech)
	{
		return Research.Instance.IsBeingResearched(tech);
	}

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
		base.ConsumeMouseScroll = true;
		Transform transform = base.transform;
		while (m_Raycaster == null)
		{
			m_Raycaster = transform.GetComponent<GraphicRaycaster>();
			if (m_Raycaster == null)
			{
				transform = transform.parent;
			}
		}
	}

	private void ZoomOut()
	{
		targetZoom = Mathf.Clamp(targetZoom - zoomAmountPerButton, minZoom, maxZoom);
		zoomCenterLock = true;
	}

	private void ZoomIn()
	{
		targetZoom = Mathf.Clamp(targetZoom + zoomAmountPerButton, minZoom, maxZoom);
		zoomCenterLock = true;
	}

	public void ZoomToTech(string techID)
	{
		Vector2 a = (Vector2)entryMap[Db.Get().Techs.Get(techID)].rectTransform().GetLocalPosition() + new Vector2((0f - foreground.rectTransform().rect.size.x) / 2f, foreground.rectTransform().rect.size.y / 2f);
		forceTargetPosition = -a;
		zoomingToTarget = true;
		targetZoom = maxZoom;
	}

	private void Update()
	{
		RectTransform component = scrollContent.GetComponent<RectTransform>();
		if (!isDragging && (rightMouseDown || leftMouseDown) && Vector2.Distance(dragStartPosition, KInputManager.GetMousePos()) > 1f)
		{
			isDragging = true;
		}
		if (isDragging && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
		{
			leftMouseDown = false;
			rightMouseDown = false;
			isDragging = false;
		}
		Vector2 anchoredPosition = component.anchoredPosition;
		float t = Mathf.Min(effectiveZoomSpeed * Time.unscaledDeltaTime, 0.9f);
		currentZoom = Mathf.Lerp(currentZoom, targetZoom, t);
		Vector2 zero = Vector2.zero;
		Vector2 v = KInputManager.GetMousePos();
		Vector2 b = (zoomCenterLock ? (component.InverseTransformPoint(new Vector2(Screen.width / 2, Screen.height / 2)) * currentZoom) : (component.InverseTransformPoint(v) * currentZoom));
		component.localScale = new Vector3(currentZoom, currentZoom, 1f);
		Vector2 a = (zoomCenterLock ? (component.InverseTransformPoint(new Vector2(Screen.width / 2, Screen.height / 2)) * currentZoom) : (component.InverseTransformPoint(v) * currentZoom));
		zero = a - b;
		float d = keyboardScrollSpeed;
		if (panUp)
		{
			keyPanDelta -= Vector2.up * Time.unscaledDeltaTime * d;
		}
		else if (panDown)
		{
			keyPanDelta += Vector2.up * Time.unscaledDeltaTime * d;
		}
		if (panLeft)
		{
			keyPanDelta += Vector2.right * Time.unscaledDeltaTime * d;
		}
		else if (panRight)
		{
			keyPanDelta -= Vector2.right * Time.unscaledDeltaTime * d;
		}
		Vector2 vector = new Vector2(Mathf.Lerp(0f, keyPanDelta.x, Time.unscaledDeltaTime * keyPanEasing), Mathf.Lerp(0f, keyPanDelta.y, Time.unscaledDeltaTime * keyPanEasing));
		keyPanDelta -= vector;
		Vector2 zero2 = Vector2.zero;
		if (isDragging)
		{
			Vector2 vector2 = KInputManager.GetMousePos() - dragLastPosition;
			zero2 += vector2;
			dragLastPosition = KInputManager.GetMousePos();
			dragInteria = Vector2.ClampMagnitude(dragInteria + vector2, 400f);
		}
		dragInteria *= Mathf.Max(0f, 1f - Time.unscaledDeltaTime * 4f);
		Vector2 vector3 = anchoredPosition + zero + keyPanDelta + zero2;
		if (!isDragging)
		{
			Vector2 size = GetComponent<RectTransform>().rect.size;
			Vector2 vector4 = new Vector2(((0f - component.rect.size.x) / 2f - 250f) * currentZoom, -250f * currentZoom);
			Vector2 vector5 = new Vector2(250f * currentZoom, (component.rect.size.y + 250f) * currentZoom - size.y);
			Vector2 a2 = new Vector2(Mathf.Clamp(vector3.x, vector4.x, vector5.x), Mathf.Clamp(vector3.y, vector4.y, vector5.y));
			forceTargetPosition = new Vector2(Mathf.Clamp(forceTargetPosition.x, vector4.x, vector5.x), Mathf.Clamp(forceTargetPosition.y, vector4.y, vector5.y));
			a2 += dragInteria;
			Vector2 vector6 = a2 - vector3;
			if (!panLeft && !panRight && !panUp && !panDown)
			{
				vector3 += vector6 * edgeClampFactor * Time.unscaledDeltaTime;
			}
			else
			{
				vector3 += vector6;
				if (vector6.x < 0f)
				{
					keyPanDelta.x = Mathf.Min(0f, keyPanDelta.x);
				}
				if (vector6.x > 0f)
				{
					keyPanDelta.x = Mathf.Max(0f, keyPanDelta.x);
				}
				if (vector6.y < 0f)
				{
					keyPanDelta.y = Mathf.Min(0f, keyPanDelta.y);
				}
				if (vector6.y > 0f)
				{
					keyPanDelta.y = Mathf.Max(0f, keyPanDelta.y);
				}
			}
		}
		if (zoomingToTarget)
		{
			vector3 = Vector2.Lerp(vector3, forceTargetPosition, Time.unscaledDeltaTime * 4f);
			if (Vector3.Distance(vector3, forceTargetPosition) < 1f || isDragging || panLeft || panRight || panUp || panDown)
			{
				zoomingToTarget = false;
			}
		}
		component.anchoredPosition = vector3;
	}

	protected override void OnSpawn()
	{
		Subscribe(Research.Instance.gameObject, -1914338957, OnActiveResearchChanged);
		Subscribe(Game.Instance.gameObject, -107300940, OnResearchComplete);
		Subscribe(Game.Instance.gameObject, -1974454597, delegate
		{
			Show(show: false);
		});
		pointDisplayMap = new Dictionary<string, LocText>();
		foreach (ResearchType type in Research.Instance.researchTypes.Types)
		{
			pointDisplayMap[type.id] = Util.KInstantiateUI(pointDisplayCountPrefab, pointDisplayContainer, force_active: true).GetComponentInChildren<LocText>();
			pointDisplayMap[type.id].text = Research.Instance.globalPointInventory.PointsByTypeID[type.id].ToString();
			pointDisplayMap[type.id].transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(type.description);
			pointDisplayMap[type.id].transform.parent.GetComponentInChildren<Image>().sprite = type.sprite;
		}
		pointDisplayContainer.transform.parent.gameObject.SetActive(Research.Instance.UseGlobalPointInventory);
		entryMap = new Dictionary<Tech, ResearchEntry>();
		List<Tech> resources = Db.Get().Techs.resources;
		resources.Sort((Tech x, Tech y) => y.center.y.CompareTo(x.center.y));
		List<TechTreeTitle> resources2 = Db.Get().TechTreeTitles.resources;
		resources2.Sort((TechTreeTitle x, TechTreeTitle y) => y.center.y.CompareTo(x.center.y));
		float x2 = 0f;
		float y2 = 125f;
		Vector2 b = new Vector2(x2, y2);
		for (int i = 0; i < resources2.Count; i++)
		{
			ResearchTreeTitle researchTreeTitle = Util.KInstantiateUI<ResearchTreeTitle>(researchTreeTitlePrefab.gameObject, treeTitles);
			TechTreeTitle techTreeTitle = resources2[i];
			researchTreeTitle.name = techTreeTitle.Name + " Title";
			Vector3 v = techTreeTitle.center + b;
			researchTreeTitle.transform.rectTransform().anchoredPosition = v;
			float height = techTreeTitle.height;
			if (i + 1 < resources2.Count)
			{
				TechTreeTitle techTreeTitle2 = resources2[i + 1];
				Vector3 vector = techTreeTitle2.center + b;
				height += v.y - (vector.y + techTreeTitle2.height);
			}
			else
			{
				height += 600f;
			}
			researchTreeTitle.transform.rectTransform().sizeDelta = new Vector2(techTreeTitle.width, height);
			researchTreeTitle.SetLabel(techTreeTitle.Name);
			researchTreeTitle.SetColor(i);
		}
		List<Vector2> list = new List<Vector2>();
		float x3 = 0f;
		float y3 = 0f;
		Vector2 b2 = new Vector2(x3, y3);
		for (int j = 0; j < resources.Count; j++)
		{
			ResearchEntry researchEntry = Util.KInstantiateUI<ResearchEntry>(entryPrefab.gameObject, scrollContent);
			Tech tech = resources[j];
			researchEntry.name = tech.Name + " Panel";
			Vector3 v2 = tech.center + b2;
			researchEntry.transform.rectTransform().anchoredPosition = v2;
			researchEntry.transform.rectTransform().sizeDelta = new Vector2(tech.width, tech.height);
			entryMap.Add(tech, researchEntry);
			if (tech.edges.Count <= 0)
			{
				continue;
			}
			for (int k = 0; k < tech.edges.Count; k++)
			{
				ResourceTreeNode.Edge edge = tech.edges[k];
				if (edge.path == null)
				{
					list.AddRange(edge.SrcTarget);
					continue;
				}
				ResourceTreeNode.Edge.EdgeType edgeType = edge.edgeType;
				if ((uint)edgeType <= 1u || (uint)(edgeType - 4) <= 1u)
				{
					list.Add(edge.SrcTarget[0]);
					list.Add(edge.path[0]);
					for (int l = 1; l < edge.path.Count; l++)
					{
						list.Add(edge.path[l - 1]);
						list.Add(edge.path[l]);
					}
					list.Add(edge.path[edge.path.Count - 1]);
					list.Add(edge.SrcTarget[1]);
				}
				else
				{
					list.AddRange(edge.path);
				}
			}
		}
		for (int m = 0; m < list.Count; m++)
		{
			list[m] = new Vector2(list[m].x, list[m].y + foreground.transform.rectTransform().rect.height);
		}
		foreach (KeyValuePair<Tech, ResearchEntry> item in entryMap)
		{
			item.Value.SetTech(item.Key);
		}
		CloseButton.soundPlayer.Enabled = false;
		CloseButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		StartCoroutine(WaitAndSetActiveResearch());
		base.OnSpawn();
		RectTransform component = scrollContent.GetComponent<RectTransform>();
		component.anchoredPosition = new Vector2(250f, -250f);
		Show(show: false);
		zoomOutButton.onClick += delegate
		{
			ZoomOut();
		};
		zoomInButton.onClick += delegate
		{
			ZoomIn();
		};
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Unsubscribe(Game.Instance.gameObject, -1974454597, delegate
		{
			Deactivate();
		});
	}

	private IEnumerator WaitAndSetActiveResearch()
	{
		yield return new WaitForEndOfFrame();
		TechInstance tech = Research.Instance.GetTargetResearch();
		if (tech != null)
		{
			SetActiveResearch(tech.tech);
		}
	}

	public Vector3 GetEntryPosition(Tech tech)
	{
		if (!entryMap.ContainsKey(tech))
		{
			Debug.LogError("The Tech provided was not present in the dictionary");
			return Vector3.zero;
		}
		return entryMap[tech].transform.GetPosition();
	}

	public ResearchEntry GetEntry(Tech tech)
	{
		if (entryMap == null)
		{
			return null;
		}
		if (!entryMap.ContainsKey(tech))
		{
			Debug.LogError("The Tech provided was not present in the dictionary");
			return null;
		}
		return entryMap[tech];
	}

	public void SetEntryPercentage(Tech tech, float percent)
	{
		ResearchEntry entry = GetEntry(tech);
		if (entry != null)
		{
			entry.SetPercentage(percent);
		}
	}

	public void TurnEverythingOff()
	{
		foreach (KeyValuePair<Tech, ResearchEntry> item in entryMap)
		{
			item.Value.SetEverythingOff();
		}
	}

	public void TurnEverythingOn()
	{
		foreach (KeyValuePair<Tech, ResearchEntry> item in entryMap)
		{
			item.Value.SetEverythingOn();
		}
	}

	private void SelectAllEntries(Tech tech, bool isSelected)
	{
		ResearchEntry entry = GetEntry(tech);
		if (entry != null)
		{
			entry.QueueStateChanged(isSelected);
		}
		foreach (Tech item in tech.requiredTech)
		{
			SelectAllEntries(item, isSelected);
		}
	}

	private void OnResearchComplete(object data)
	{
		Tech tech = (Tech)data;
		ResearchEntry entry = GetEntry(tech);
		if (entry != null)
		{
			entry.ResearchCompleted();
		}
		UpdateProgressBars();
		UpdatePointDisplay();
	}

	private void UpdatePointDisplay()
	{
		foreach (ResearchType type in Research.Instance.researchTypes.Types)
		{
			pointDisplayMap[type.id].text = $"{Research.Instance.researchTypes.GetResearchType(type.id).name}: {Research.Instance.globalPointInventory.PointsByTypeID[type.id].ToString()}";
		}
	}

	private void OnActiveResearchChanged(object data)
	{
		List<TechInstance> list = (List<TechInstance>)data;
		foreach (TechInstance item in list)
		{
			ResearchEntry entry = GetEntry(item.tech);
			if (entry != null)
			{
				entry.QueueStateChanged(isSelected: true);
			}
		}
		UpdateProgressBars();
		UpdatePointDisplay();
		if (list.Count > 0)
		{
			currentResearch = list[list.Count - 1].tech;
		}
	}

	private void UpdateProgressBars()
	{
		foreach (KeyValuePair<Tech, ResearchEntry> item in entryMap)
		{
			item.Value.UpdateProgressBars();
		}
	}

	public void CancelResearch()
	{
		List<TechInstance> researchQueue = Research.Instance.GetResearchQueue();
		foreach (TechInstance item in researchQueue)
		{
			ResearchEntry entry = GetEntry(item.tech);
			if (entry != null)
			{
				entry.QueueStateChanged(isSelected: false);
			}
		}
		researchQueue.Clear();
	}

	private void SetActiveResearch(Tech newResearch)
	{
		if (newResearch != currentResearch && currentResearch != null)
		{
			SelectAllEntries(currentResearch, isSelected: false);
		}
		currentResearch = newResearch;
		if (currentResearch != null)
		{
			SelectAllEntries(currentResearch, isSelected: true);
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			if (DetailsScreen.Instance != null)
			{
				DetailsScreen.Instance.gameObject.SetActive(value: false);
			}
		}
		else if (SelectTool.Instance.selected != null && !DetailsScreen.Instance.gameObject.activeSelf)
		{
			DetailsScreen.Instance.gameObject.SetActive(value: true);
			DetailsScreen.Instance.Refresh(SelectTool.Instance.selected.gameObject);
		}
		UpdateProgressBars();
		UpdatePointDisplay();
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (e.IsAction(Action.MouseRight))
			{
				if (!isDragging)
				{
					ManagementMenu.Instance.CloseAll();
				}
				isDragging = false;
				rightMouseDown = false;
			}
			if (e.IsAction(Action.MouseRight) || e.IsAction(Action.MouseLeft))
			{
				isDragging = false;
				rightMouseDown = false;
				leftMouseDown = false;
			}
			if (panUp && e.TryConsume(Action.PanUp))
			{
				panUp = false;
				return;
			}
			if (panDown && e.TryConsume(Action.PanDown))
			{
				panDown = false;
				return;
			}
			if (panRight && e.TryConsume(Action.PanRight))
			{
				panRight = false;
				return;
			}
			if (panLeft && e.TryConsume(Action.PanLeft))
			{
				panLeft = false;
				return;
			}
		}
		base.OnKeyUp(e);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (e.TryConsume(Action.MouseRight))
			{
				dragStartPosition = KInputManager.GetMousePos();
				dragLastPosition = KInputManager.GetMousePos();
				rightMouseDown = true;
				return;
			}
			if (e.TryConsume(Action.MouseLeft))
			{
				dragStartPosition = KInputManager.GetMousePos();
				dragLastPosition = KInputManager.GetMousePos();
				leftMouseDown = true;
				return;
			}
			if (KInputManager.GetMousePos().x > sideBar.rectTransform().sizeDelta.x)
			{
				if (e.TryConsume(Action.ZoomIn))
				{
					targetZoom = Mathf.Clamp(targetZoom + zoomAmountPerScroll, minZoom, maxZoom);
					zoomCenterLock = false;
					return;
				}
				if (e.TryConsume(Action.ZoomOut))
				{
					targetZoom = Mathf.Clamp(targetZoom - zoomAmountPerScroll, minZoom, maxZoom);
					zoomCenterLock = false;
					return;
				}
			}
			if (e.TryConsume(Action.Escape))
			{
				ManagementMenu.Instance.CloseAll();
				return;
			}
			if (e.TryConsume(Action.PanLeft))
			{
				panLeft = true;
				return;
			}
			if (e.TryConsume(Action.PanRight))
			{
				panRight = true;
				return;
			}
			if (e.TryConsume(Action.PanUp))
			{
				panUp = true;
				return;
			}
			if (e.TryConsume(Action.PanDown))
			{
				panDown = true;
				return;
			}
		}
		base.OnKeyDown(e);
	}

	public static bool TechPassesSearchFilter(string techID, string filterString)
	{
		if (!string.IsNullOrEmpty(filterString))
		{
			filterString = filterString.ToUpper();
			bool flag = false;
			Tech tech = Db.Get().Techs.Get(techID);
			string text = UI.StripLinkFormatting(tech.Name).ToLower();
			flag = text.ToUpper().Contains(filterString);
			if (!flag)
			{
				flag = tech.category.ToUpper().Contains(filterString);
				foreach (TechItem unlockedItem in tech.unlockedItems)
				{
					string text2 = UI.StripLinkFormatting(unlockedItem.Name).ToLower();
					if (text2.ToUpper().Contains(filterString))
					{
						flag = true;
						break;
					}
					string text3 = UI.StripLinkFormatting(unlockedItem.description).ToLower();
					if (text3.ToUpper().Contains(filterString))
					{
						flag = true;
						break;
					}
				}
			}
			return flag;
		}
		return true;
	}

	public static bool TechItemPassesSearchFilter(string techItemID, string filterString)
	{
		if (!string.IsNullOrEmpty(filterString))
		{
			bool flag = false;
			filterString = filterString.ToUpper();
			TechItem techItem = Db.Get().TechItems.Get(techItemID);
			string text = UI.StripLinkFormatting(techItem.Name).ToLower();
			flag = text.ToUpper().Contains(filterString);
			if (!flag)
			{
				flag = techItem.Name.ToUpper().Contains(filterString) && techItem.description.ToUpper().Contains(filterString);
			}
			return flag;
		}
		return true;
	}
}
